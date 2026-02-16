using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheOtherRoles.Modules
{
    public class PlayManager : MonoBehaviour
    {
        static PlayManager() => ClassInjector.RegisterTypeInIl2Cpp<PlayManager>();
        public static MainMenuManager MainMenu;

        public static void Open(MainMenuManager mainMenu)
        {
            MainMenu = mainMenu;

            var obj = Helpers.CreateObject<PlayManager>("HowToPlay", Camera.main.transform, new Vector3(0, 0, -30f));
            TransitionFade.Instance.DoTransitionFade(null!, obj.gameObject, () => { mainMenu.mainMenuUI.SetActive(false); }, () => { obj.OnShown(); });
        }

        public void OnShown()
        {
            gameObject.SetActive(true);
            GameObject screen = Helpers.CreateObject("Screen", transform, new Vector3(0, -0.1f, -10f));
            var roleText = screen.AddComponent<TextMeshPro>();
            string content = $"<size=90%>{ModTranslation.getString("modHowToPlay")}\n\n</size><size=50%>{ModTranslation.getString("howToPlayCrewmate")}\n\n{ModTranslation.getString("howToPlayImpostor")}\n\n{ModTranslation.getString("howToPlayNeutral")}\n\n{ModTranslation.getString("howToPlayMadmate")}</size>";
            roleText.SetText(content);
            roleText.alignment = TextAlignmentOptions.Top;
            roleText.enableWordWrapping = true;
            roleText.autoSizeTextContainer = true;
            Helpers.TextFeatures features = Helpers.AnalyzeTextFeatures(content);

            roleText.fontSize = 3f * features.fontSizeMultiplier;
            roleText.lineSpacing = features.lineSpacingOffset;
            roleText.ForceMeshUpdate();

            float textHeight = roleText.preferredHeight * features.heightMultiplier;
            float screenHeight = Camera.main.orthographicSize * 2f;
            float visibleHeight = screenHeight * 0.8f;

            GameObject centerContainer = Helpers.CreateObject("CenterContainer", screen.transform, new Vector3(0, 0f, -10f));
            var containerRect = centerContainer.AddComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(screenHeight * Camera.main.aspect * 0.9f, visibleHeight);

            roleText.transform.SetParent(centerContainer.transform);
            roleText.rectTransform.anchoredPosition = Vector2.zero;
            roleText.rectTransform.sizeDelta = containerRect.sizeDelta;
            roleText.ForceMeshUpdate();
            textHeight = roleText.preferredHeight * features.heightMultiplier;

            var scroller = screen.AddComponent<Scroller>();
            scroller.Inner = containerRect;
            scroller.allowY = true;
            scroller.velocity = Vector2.zero;

            if (textHeight > visibleHeight)
            {
                float scrollRange = (textHeight - visibleHeight) / 2f;
                scroller.ContentYBounds = new FloatRange(-scrollRange, scrollRange);
                scroller.ScrollWheelSpeed = Mathf.Clamp(textHeight * 0.05f, 0.5f, 3f);
                containerRect.anchoredPosition = Vector2.zero;
            }
            else
            {
                scroller.enabled = false;
                containerRect.anchoredPosition = Vector2.zero;
            }

            roleText.alignment = TextAlignmentOptions.Center;
        }

        protected void Close()
        {
            TransitionFade.Instance.DoTransitionFade(gameObject, null!, () => MainMenu?.mainMenuUI.SetActive(true), () => GameObject.Destroy(gameObject));
        }

        public void Awake()
        {
            if (MainMenu != null)
            {
                var backBlackPrefab = MainMenu.playerCustomizationPrefab.transform.GetChild(1);
                GameObject.Instantiate(backBlackPrefab.gameObject, transform);
                var backGroundPrefab = MainMenu.playerCustomizationPrefab.transform.GetChild(2);
                var backGround = GameObject.Instantiate(backGroundPrefab.gameObject, transform);
                GameObject.Destroy(backGround.transform.GetChild(2).gameObject);

                var closeButtonPrefab = MainMenu.playerCustomizationPrefab.transform.GetChild(0).GetChild(0);
                var closeButton = GameObject.Instantiate(closeButtonPrefab.gameObject, transform);
                GameObject.Destroy(closeButton.GetComponent<AspectPosition>());
                var button = closeButton.GetComponent<PassiveButton>();
                button.gameObject.SetActive(true);
                button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                button.OnClick.AddListener((System.Action)(() => Close()));
                button.gameObject.SetAsUIAspectContent(AspectPosition.EdgeAlignments.LeftTop, new(0.4f, 0.4f, -50f));
            }
        }
    }
}
