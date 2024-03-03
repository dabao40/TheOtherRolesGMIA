using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

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
            roleText.SetText($"<size=120%>{ModTranslation.getString("modHowToPlay")}</size>\n\n<size=50%>{ModTranslation.getString("howToPlayCrewmate")}\n\n{ModTranslation.getString("howToPlayImpostor")}\n\n{ModTranslation.getString("howToPlayNeutral")}\n\n{ModTranslation.getString("howToPlayMadmate")}</size>");
            roleText.alignment = TextAlignmentOptions.Center;
            roleText.fontSize *= 0.1f;
            roleText.transform.SetParent(screen.transform);
            roleText.transform.localPosition = new Vector3(0f, 0.2f);
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
                button.transform.localPosition = new Vector3(-4.9733f, 2.6708f, -50f);
            }
        }
    }
}
