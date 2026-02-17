using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Modules.CustomHats.Patches
{
    [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
    public static class HatsTab_OnEnable
    {
        private static SortedList<string, List<HatData>> _hatGroups;
        private static int _currentPage;

        private static GameObject _leftButtonObj;
        private static GameObject _rightButtonObj;
        private static TextMeshPro _pageLabel;

        private static int PageCount => _hatGroups?.Count ?? 0;

        private static void GoToPreviousPage(HatsTab tab)
        {
            if (PageCount == 0) return;
            _currentPage = (_currentPage - 1 + PageCount) % PageCount;
            RenderPage(tab);
        }

        private static void GoToNextPage(HatsTab tab)
        {
            if (PageCount == 0) return;
            _currentPage = (_currentPage + 1) % PageCount;
            RenderPage(tab);
        }
        public static bool Prefix(HatsTab __instance)
        {
            __instance.currentHat = HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat);

            if (_hatGroups == null || _hatGroups.Count == 0)
            {
                _hatGroups = new SortedList<string, List<HatData>>(
                    new PaddedComparer<string>("Vanilla", ""));

                foreach (var hat in HatManager.Instance.GetUnlockedHats())
                {
                    var group = string.IsNullOrEmpty(hat.StoreName) ? "Vanilla" : hat.StoreName;
                    if (!_hatGroups.ContainsKey(group))
                        _hatGroups[group] = new List<HatData>();
                    _hatGroups[group].Add(hat);
                }
            }

            if (_currentPage >= PageCount) _currentPage = 0;

            CreateNavButtons(__instance);

            RenderPage(__instance);

            return false;
        }

        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.Update))]
        [HarmonyPrefix]
        public static void UpdatePrefix(HatsTab __instance)
        {
            if (PageCount <= 1) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow)
                || Input.GetKeyDown(KeyCode.LeftControl)
                || Input.GetKeyDown(KeyCode.RightControl))
            {
                GoToPreviousPage(__instance);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)
                     || Input.GetKeyDown(KeyCode.Tab))
            {
                GoToNextPage(__instance);
            }
        }

        private static void RenderPage(HatsTab __instance)
        {
            // Clear existing chips and section labels
            foreach (var chip in __instance.ColorChips)
                chip.gameObject.Destroy();
            __instance.ColorChips.Clear();

            foreach (var tmp in __instance.scroller.Inner.GetComponentsInChildren<TextMeshPro>())
                tmp.gameObject.Destroy();

            if (PageCount == 0) return;

            // Grab the template TMP (hidden title inside the tab)
            var groupNameText = __instance.GetComponentInChildren<TextMeshPro>(true);

            var (groupName, hats) = _hatGroups.ToArray()[_currentPage];

            int hatIdx = 0;

            var headerText = Object.Instantiate(groupNameText, __instance.scroller.Inner);
            headerText.gameObject.SetActive(true);
            headerText.gameObject.transform.localScale = Vector3.one;

            // Remove auto-translator if present
            var translator = headerText.GetComponent<TextTranslatorTMP>();
            if (translator != null) Object.Destroy(translator);

            headerText.text = groupName;
            headerText.alignment = TextAlignmentOptions.Center;
            headerText.fontSize = 3f;
            headerText.fontSizeMax = 3f;
            headerText.fontSizeMin = 0f;

            // X = 0.85f  ← midpoint between left arrow (-1.05) and right arrow (2.75),
            //              matches the page-label centre so the title sits visually centred.
            float xCenter = 0.85f;
            float yHeader = __instance.YStart - hatIdx / __instance.NumPerRow * __instance.YOffset;
            headerText.transform.localPosition = new Vector3(xCenter, yHeader, -1f);

            // Reserve rows for the header
            hatIdx += 5;

            foreach (var hat in hats.OrderBy(h => HatManager.Instance.allHats.IndexOf(h)))
            {
                float xPos = __instance.XRange.Lerp(hatIdx % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                float yPos = __instance.YStart - hatIdx / __instance.NumPerRow * __instance.YOffset;

                var colorChip = Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
                colorChip.transform.localPosition = new Vector3(xPos, yPos, -1f);

                // Keyboard vs mouse input handling (mirrors MiraAPI)
                if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                {
                    colorChip.Button.OnClick.AddListener((Action)__instance.ClickEquip);
                    colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectHat(hat)));
                    colorChip.Button.OnMouseOut.AddListener(
                        (Action)(() => __instance.SelectHat(
                            HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                }
                else
                {
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectHat(hat)));
                }

                colorChip.Button.ClickMask = __instance.scroller.Hitbox;

                int colorId = __instance.HasLocalPlayer()
                    ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId
                    : DataManager.Player.Customization.Color;

                colorChip.Inner.SetHat(hat, colorId);
                colorChip.Inner.transform.localPosition = hat.ChipOffset + new Vector2(0f, -0.3f);
                colorChip.Tag = hat;
                __instance.ColorChips.Add(colorChip);

                hatIdx += 1;
            }

            __instance.scroller.ContentYBounds.max =
                -(__instance.YStart - (hatIdx + 1) / __instance.NumPerRow * __instance.YOffset) - 3f;
            __instance.scroller.ScrollToTop();
            __instance.currentHatIsEquipped = true;

            UpdatePageLabel();
        }

        private static void CreateNavButtons(HatsTab tab)
        {
            // Destroy previous instances so re-opening the tab is clean
            if (_leftButtonObj != null) Object.Destroy(_leftButtonObj);
            if (_rightButtonObj != null) Object.Destroy(_rightButtonObj);
            if (_pageLabel != null) Object.Destroy(_pageLabel.gameObject);

            //    so the ◄ button (x ≈ -1.05) does not overlap it.
            var titleTransform = tab.transform.FindChild("Text");
            if (titleTransform != null)
            {
                var pos = titleTransform.localPosition;
                titleTransform.localPosition = new Vector3(pos.x + 0.6f, pos.y, pos.z);
            }

            var arrowSprite = BuildArrowSprite(false); // ►
            var arrowSpriteL = BuildArrowSprite(true);  // ◄

            _leftButtonObj = BuildNavButton(
                tab.transform,
                new Vector3(-1.05f, -0.18f, -55f),
                arrowSpriteL,
                () => GoToPreviousPage(tab));

            _rightButtonObj = BuildNavButton(
                tab.transform,
                new Vector3(2.75f, -0.18f, -55f),
                arrowSprite,
                () => GoToNextPage(tab));

            // We borrow the first TMP we can find on the tab as a template
            var templateTmp = tab.GetComponentInChildren<TextMeshPro>(true);
            if (templateTmp != null)
            {
                _pageLabel = Object.Instantiate(templateTmp, tab.transform);
                _pageLabel.gameObject.SetActive(true);
                _pageLabel.transform.localPosition = new Vector3(0.85f, -0.18f, -55f);
                _pageLabel.transform.localScale = Vector3.one * 0.55f;

                var lTranslator = _pageLabel.GetComponent<TextTranslatorTMP>();
                if (lTranslator != null) Object.Destroy(lTranslator);

                _pageLabel.alignment = TextAlignmentOptions.Center;
                _pageLabel.fontSize = 3.5f;
                _pageLabel.fontSizeMax = 3.5f;
                _pageLabel.fontSizeMin = 0f;
                _pageLabel.color = Color.white;
            }

            UpdatePageLabel();
        }

        private static void UpdatePageLabel()
        {
            if (_pageLabel == null) return;
            _pageLabel.text = PageCount > 0
                ? $"<color=#FFDD00>{_currentPage + 1}</color> / {PageCount}"
                : "0 / 0";
        }

        private static GameObject BuildNavButton(
            Transform parent,
            Vector3 localPos,
            Sprite icon,
            Action onClick)
        {
            var go = new GameObject("HatPageNavBtn");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.layer = parent.gameObject.layer;

            // SpriteRenderer (visible icon)
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = icon;

            // Collider so the PassiveButton can detect clicks
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;

            // PassiveButton — same component TOR uses everywhere
            var btn = go.AddComponent<PassiveButton>();
            btn.Colliders = new Collider2D[] { col };

            btn.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            btn.OnClick.AddListener((Action)onClick);

            // Hover colour feedback (white ↔ yellow), matching TOR button style
            btn.OnMouseOver = new UnityEvent();
            btn.OnMouseOver.AddListener((Action)(() => sr.color = new Color(1f, 0.85f, 0.2f)));
            btn.OnMouseOut = new UnityEvent();
            btn.OnMouseOut.AddListener((Action)(() => sr.color = Color.white));

            return go;
        }

        //  Draws a filled triangle (◄ or ►) into a 32×32 texture.
        //  This removes any dependency on external asset files.
        //
        private static Sprite BuildArrowSprite(bool pointLeft)
        {
            const int size = 32;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;

            // Fill transparent
            var pixels = new Color32[size * size];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(0, 0, 0, 0);

            // Draw a filled triangle
            // Triangle vertices (in pixel coords, Y-up):
            //   pointLeft  ◄:  tip=(4,16), top-right=(27,4), bottom-right=(27,27)
            //   pointRight ►:  tip=(27,16),top-left=(4,4),   bottom-left=(4,27)
            int tipX = pointLeft ? 4 : 27;
            int baseX = pointLeft ? 27 : 4;
            int topY = 4;
            int bottomY = 27;
            int midY = 16; // tip Y

            for (int y = topY; y <= bottomY; y++)
            {
                // Lerp the x-extent of the triangle at this scanline
                float t = (y - topY) / (float)(bottomY - topY);
                float xEdge = Mathf.Lerp(tipX, baseX, t <= 0.5f ? t * 2f : (1f - t) * 2f);

                int xStart = pointLeft ? (int)xEdge : tipX;
                int xEnd = pointLeft ? tipX : (int)xEdge;

                if (xStart > xEnd) { int tmp = xStart; xStart = xEnd; xEnd = tmp; }

                for (int x = xStart; x <= xEnd; x++)
                    pixels[y * size + x] = new Color32(255, 255, 255, 255);
            }

            tex.SetPixels32(pixels);
            tex.Apply();

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size * 1.0f);

            sprite.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }
    }
}
