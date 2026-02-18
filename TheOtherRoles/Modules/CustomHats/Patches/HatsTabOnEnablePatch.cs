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
                    new PaddedComparer<string>("Innersloth", ""));

                foreach (var hat in HatManager.Instance.GetUnlockedHats())
                {
                    var group = string.IsNullOrEmpty(hat.StoreName) ? "Innersloth" : hat.StoreName;
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

            _leftButtonObj = BuildNavButton(
                tab.transform,
                new Vector3(-1.05f, -0.18f, -55f),
                () => GoToPreviousPage(tab),
                true);

            _rightButtonObj = BuildNavButton(
                tab.transform,
                new Vector3(2.75f, -0.18f, -55f),
                () => GoToNextPage(tab),
                false);

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
            Action onClick,
            bool flip)
        {
            var go = new GameObject("HatPageNavBtn");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.layer = parent.gameObject.layer;

            var arrowSprite = Helpers.NextButtonSprite.GetSprite(0);
            var arrowSpriteActive = Helpers.NextButtonSprite.GetSprite(1);

            // SpriteRenderer (visible icon)
            var sr = go.AddComponent<SpriteRenderer>();
            if (flip) sr.flipX = true;
            sr.sprite = arrowSprite;

            // Collider so the PassiveButton can detect clicks
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;

            // PassiveButton — same component TOR uses everywhere
            var btn = go.AddComponent<PassiveButton>();
            btn.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            btn.OnMouseOut = new UnityEngine.Events.UnityEvent();
            btn.OnMouseOver = new UnityEngine.Events.UnityEvent();
            btn.Colliders = new Collider2D[] { col };

            btn.OnClick.AddListener((Action)(() => { onClick.Invoke(); MetaContext.VanillaAsset.PlaySelectSE(); }));
            btn.OnMouseOver.AddListener((Action)(() => { sr.sprite = arrowSpriteActive; MetaContext.VanillaAsset.PlayHoverSE(); }));
            btn.OnMouseOut.AddListener((Action)(() => sr.sprite = arrowSprite));

            return go;
        }
    }
}
