using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.MetaContext;
using UnityEngine;

namespace TheOtherRoles.Modules
{
    public static class ButtonEffect
    {
        public class KeyCodeInfo
        {
            public static string GetKeyDisplayName(KeyCode keyCode)
            {
                if (keyCode == KeyCode.Return)
                    return "Return";
                if (AllKeyInfo.TryGetValue(keyCode, out var val)) return val.TranslationKey;
                return null;
            }

            static public Dictionary<KeyCode, KeyCodeInfo> AllKeyInfo = new();
            public KeyCode keyCode { get; private set; }
            public DividedSpriteLoader textureHolder { get; private set; }
            public int num { get; private set; }
            public string TranslationKey { get; private set; }
            public KeyCodeInfo(KeyCode keyCode, string translationKey, DividedSpriteLoader spriteLoader, int num)
            {
                this.keyCode = keyCode;
                this.TranslationKey = translationKey;
                this.textureHolder = spriteLoader;
                this.num = num;

                AllKeyInfo.Add(keyCode, this);
            }

            public Sprite Sprite => textureHolder.GetSprite(num);

            static KeyCodeInfo()
            {
                DividedSpriteLoader spriteLoader;
                spriteLoader = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindCharacters0.png", 100f, 18, 19, true);
                new KeyCodeInfo(KeyCode.Tab, "Tab", spriteLoader, 0);
                new KeyCodeInfo(KeyCode.Space, "Space", spriteLoader, 1);
                new KeyCodeInfo(KeyCode.Comma, "<", spriteLoader, 2);
                new KeyCodeInfo(KeyCode.Period, ">", spriteLoader, 3);
                spriteLoader = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindCharacters1.png", 100f, 18, 19, true);
                for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
                    new KeyCodeInfo(key, ((char)('A' + key - KeyCode.A)).ToString(), spriteLoader, key - KeyCode.A);
                spriteLoader = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindCharacters2.png", 100f, 18, 19, true);
                for (int i = 0; i < 15; i++)
                    new KeyCodeInfo(KeyCode.F1 + i, "F" + (i + 1), spriteLoader, i);
                spriteLoader = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindCharacters3.png", 100f, 18, 19, true);
                new KeyCodeInfo(KeyCode.RightShift, "RShift", spriteLoader, 0);
                new KeyCodeInfo(KeyCode.LeftShift, "LShift", spriteLoader, 1);
                new KeyCodeInfo(KeyCode.RightControl, "RControl", spriteLoader, 2);
                new KeyCodeInfo(KeyCode.LeftControl, "LControl", spriteLoader, 3);
                new KeyCodeInfo(KeyCode.RightAlt, "RAlt", spriteLoader, 4);
                new KeyCodeInfo(KeyCode.LeftAlt, "LAlt", spriteLoader, 5);
                spriteLoader = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindCharacters4.png", 100f, 18, 19, true);
                for (int i = 0; i < 6; i++)
                    new KeyCodeInfo(KeyCode.Mouse1 + i, "Mouse " + (i == 0 ? "Right" : i == 1 ? "Middle" : (i + 1).ToString()), spriteLoader, i);
                spriteLoader = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindCharacters5.png", 100f, 18, 19, true);
                for (int i = 0; i < 10; i++)
                    new KeyCodeInfo(KeyCode.Alpha0 + i, "0" + (i + 1), spriteLoader, i);
            }
        }

        static Image keyBindBackgroundSprite = SpriteLoader.FromResource("TheOtherRoles.Resources.KeyBindBackground.png", 100f);
        static Image mouseDisableActionSprite = SpriteLoader.FromResource("TheOtherRoles.Resources.MouseActionDisableIcon.png", 100f);
        static Image infoActionSprite = SpriteLoader.FromResource("TheOtherRoles.Resources.ButtonInfoIcon.png", 100f);

        public enum ActionIconType
        {
            NonClickAction,
            InfoAction
        }

        static public GameObject AddKeyGuide(GameObject button, KeyCode key, UnityEngine.Vector2 pos, bool removeExistingGuide, string action = null)
        {
#if WINDOWS
            if (removeExistingGuide) button.gameObject.ForEachChild((Il2CppSystem.Action<GameObject>)(obj => { if (obj.name == "HotKeyGuide") UnityEngine.Object.Destroy(obj); }));

            Sprite numSprite = null;
            if (KeyCodeInfo.AllKeyInfo.ContainsKey(key)) numSprite = KeyCodeInfo.AllKeyInfo[key].Sprite;
            if (numSprite == null) return null;

            GameObject obj = new()
            {
                name = "HotKeyGuide"
            };
            obj.transform.SetParent(button.transform);
            obj.layer = button.layer;
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.transform.localPosition = (UnityEngine.Vector3)pos + new UnityEngine.Vector3(0f, 0f, -10f);
            renderer.sprite = keyBindBackgroundSprite.GetSprite();

            GameObject numObj = new()
            {
                name = "HotKeyText"
            };
            numObj.transform.SetParent(obj.transform);
            numObj.layer = button.layer;
            renderer = numObj.AddComponent<SpriteRenderer>();
            renderer.transform.localPosition = new(0, 0, -1f);
            renderer.sprite = numSprite;

            SetHintOverlay(obj, key, action);

            return obj;
#else
            return null;
#endif
        }

        static public GameObject SetKeyGuide(GameObject button, KeyCode key, bool removeExistingGuide = true, string action = null)
        {
            return AddKeyGuide(button, key, new(0.48f, 0.48f), removeExistingGuide, action: action);
        }

        public static void SetHintOverlay(GameObject gameObj, KeyCode keyCode, string action = null)
        {
            var button = gameObj.SetUpButton();
            var collider = gameObj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.125f;
            button.OnMouseOver.AddListener((Action)(() => {
                string str = keyCode != KeyCode.None ? string.Format(ModTranslation.getString("buttonsDescription"), KeyCodeInfo.GetKeyDisplayName(keyCode)) : "";
                if (action != null)
                {
                    if (str.Length > 0) str += "<br>";
                    str += "<line-indent=0.8em>" + action;
                }
                TORGUIManager.Instance.SetHelpContext(button, str);
            }));
            button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));
        }

        static public GameObject SetMouseActionIcon(GameObject button, bool show, string action = "mouseClick", bool atBottom = true, ActionIconType actionType = ActionIconType.NonClickAction)
        {
            if (!show)
            {
                button.gameObject.ForEachChild((Il2CppSystem.Action<GameObject>)(obj => { if (obj.name == "MouseAction") UnityEngine.Object.Destroy(obj); }));
                return null;
            }
            else
            {
                GameObject obj = new()
                {
                    name = "MouseAction"
                };
                obj.transform.SetParent(button.transform);
                obj.layer = button.layer;
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.transform.localPosition = new(0.48f, atBottom ? -0.29f : 0.48f, -10f);
                renderer.sprite = actionType == ActionIconType.NonClickAction ? mouseDisableActionSprite.GetSprite() : infoActionSprite.GetSprite();

                if (action != null) SetHintOverlay(obj, KeyCode.None, action);

                return obj;
            }
        }

        private static IDividedSpriteLoader textureUsesIconsSprite = XOnlyDividedSpriteLoader.FromResource("TheOtherRoles.Resources.UsesIcon.png", 120f, 10);
        static public GameObject ShowUsesIcon(this ActionButton button)
        {
            Transform template = HudManager.Instance.AbilityButton.transform.GetChild(2);
            var usesObject = UnityEngine.Object.Instantiate(template.gameObject);
            usesObject.transform.SetParent(button.gameObject.transform);
            usesObject.transform.localScale = template.localScale;
            usesObject.transform.localPosition = template.localPosition * 1.2f;
            return usesObject;
        }

        static public GameObject ShowUsesIcon(this ActionButton button, int iconVariation, out TMPro.TextMeshPro text)
        {
            GameObject result = ShowUsesIcon(button);
            var renderer = result.GetComponent<SpriteRenderer>();
            renderer.sprite = textureUsesIconsSprite.GetSprite(iconVariation);
            text = result.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            text.transform.localScale *= 0.85f;
            text.transform.SetLocalY(text.transform.localPosition.y - 0.01f);
            return result;
        }

        static public void ShowVanillaKeyGuide(this HudManager manager)
        {
#if WINDOWS
            if (manager == null) return;

            //ボタンのガイドを表示
            var keyboardMap = Rewired.ReInput.mapping.GetKeyboardMapInstanceSavedOrDefault(0, 0, 0);
            Il2CppReferenceArray<Rewired.ActionElementMap> actionArray;
            Rewired.ActionElementMap actionMap;

            //マップ
            actionArray = keyboardMap.GetButtonMapsWithAction(4);
            if (actionArray.Count > 0)
            {
                actionMap = actionArray[0];
                SetKeyGuide(manager.SabotageButton.gameObject, actionMap.keyCode);
            }

            //使用
            actionArray = keyboardMap.GetButtonMapsWithAction(6);
            if (actionArray.Count > 0)
            {
                actionMap = actionArray[0];
                SetKeyGuide(manager.UseButton.gameObject, actionMap.keyCode);
                SetKeyGuide(manager.PetButton.gameObject, actionMap.keyCode);
            }

            //レポート
            actionArray = keyboardMap.GetButtonMapsWithAction(7);
            if (actionArray.Count > 0)
            {
                actionMap = actionArray[0];
                SetKeyGuide(manager.ReportButton.gameObject, actionMap.keyCode);
            }

            //キル
            actionArray = keyboardMap.GetButtonMapsWithAction(8);
            if (actionArray.Count > 0)
            {
                actionMap = actionArray[0];
                SetKeyGuide(manager.KillButton.gameObject, actionMap.keyCode);
            }

            //ベント
            actionArray = keyboardMap.GetButtonMapsWithAction(50);
            if (actionArray.Count > 0)
            {
                actionMap = actionArray[0];
                SetKeyGuide(manager.ImpostorVentButton.gameObject, actionMap.keyCode);
            }
#endif
        }
    }
}
