using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.MetaContext;
using UnityEngine;

namespace TheOtherRoles.Modules
{
    public class AchievementViewer : MonoBehaviour
    {
        static AchievementViewer() => ClassInjector.RegisterTypeInIl2Cpp<AchievementViewer>();
        static public MainMenuManager MainMenu;

        private MetaScreen myScreen = null!;

        protected void Close()
        {
            TransitionFade.Instance.DoTransitionFade(gameObject, null!, () => MainMenu?.mainMenuUI.SetActive(true), () => GameObject.Destroy(gameObject));
        }

        static public void Open(MainMenuManager mainMenu)
        {
            MainMenu = mainMenu;

            var obj = Helpers.CreateObject<AchievementViewer>("AchievementViewer", Camera.main.transform, new Vector3(0, 0, -30f));
            TransitionFade.Instance.DoTransitionFade(null!, obj.gameObject, () => { mainMenu.mainMenuUI.SetActive(false); }, () => { obj.OnShown(); });
        }

        static public GUIContext GenerateContext(float scrollerHeight, float width)
        {
            var gui = TORGUIContextEngine.Instance;

            List<GUIContext> inner = new();
            var holder = new VerticalContextsHolder(GUIAlignment.Left, inner);
            var attr = new TextAttributes(gui.GetAttribute(AttributeParams.OblongLeft)) { FontSize = new(1.85f) };
            var headerAttr = new TextAttributes(gui.GetAttribute(AttributeParams.StandardLeft)) { FontSize = new(1.1f) };
            var detailTitleAttr = new TextAttributes(gui.GetAttribute(AttributeParams.StandardBaredBoldLeft)) { FontSize = new(1.8f) };
            var detailDetailAttr = new TextAttributes(gui.GetAttribute(AttributeParams.StandardBaredLeft)) { FontSize = new(1.5f), Size = new(5f, 6f) };

            foreach (var a in Achievement.allAchievements)
            {
                if (a.IsHidden) continue;

                if (inner.Count != 0) inner.Add(new TORGUIMargin(GUIAlignment.Left, new(0f, 0.08f)));

                var aContenxt = new HorizontalContextsHolder(GUIAlignment.Left,
                    new TORGUIImage(GUIAlignment.Left, new WrapSpriteLoader(() => Helpers.loadSpriteFromResources($"TheOtherRoles.Resources.Trophy{a.Trophy+1}.png", 220f)), new(0.38f, 0.38f), a.IsCleared ? Color.white : new UnityEngine.Color(0.2f, 0.2f, 0.2f)) { IsMasked = true },
                    new TORGUIMargin(GUIAlignment.Left, new(0.15f, 0.1f)),
                    new VerticalContextsHolder(GUIAlignment.Center,
                        new TORGUIText(GUIAlignment.Left, headerAttr, a.GetHeaderComponent()),
                        new TORGUIMargin(GUIAlignment.Left, new(0f, -0.021f)),
                        new TORGUIText(GUIAlignment.Left, attr, a.GetTitleComponent(Achievement.HiddenComponent)) { OverlayContext = a.GetOverlayContext() }
                    )
                    );
                inner.Add(aContenxt);
            }
            var scroller = new GUIScrollView(GUIAlignment.Center, new(4.7f, scrollerHeight), holder);

            var cul = Achievement.Aggregate();
            List<GUIContext> footerList = new();
            for (int i = 0; i < cul.Length; i++)
            {
                int copiedIndex = i;
                if (footerList.Count != 0) footerList.Add(new TORGUIMargin(GUIAlignment.Center, new(0.2f, 0f)));

                footerList.Add(new TORGUIImage(GUIAlignment.Left, new WrapSpriteLoader(() => Helpers.loadSpriteFromResources($"TheOtherRoles.Resources.Trophy{copiedIndex+1}.png", 220f)), new(0.5f, 0.5f)));
                footerList.Add(new TORGUIMargin(GUIAlignment.Center, new(0.05f, 0f)));
                footerList.Add(new TORGUIText(GUIAlignment.Left, detailDetailAttr, new RawTextComponent(cul[i].num + "/" + cul[i].max)));
            }
            footerList.Add(new TORGUIMargin(GUIAlignment.Center, new(0.3f, 0f)));
            footerList.Add(new TORGUIText(GUIAlignment.Right, detailDetailAttr, new RawTextComponent(ModTranslation.getString("achievementAllAchievements") + ": " + cul.Sum(c => c.num) + "/" + cul.Sum(c => c.max))));
            var footer = new HorizontalContextsHolder(GUIAlignment.Center, footerList.ToArray());

            return new VerticalContextsHolder(GUIAlignment.Left, scroller, new TORGUIMargin(GUIAlignment.Center, new(0f, 0.15f)), footer)
            { FixedWidth = width };
        }

        public void OnShown()
        {
            var gui = TORGUIContextEngine.Instance;

            var title = new TORGUIText(GUIAlignment.Left, gui.GetAttribute(AttributeAsset.OblongHeader), new TranslateTextComponent("achievementTitle"));

            gameObject.SetActive(true);
            myScreen.SetContext(new VerticalContextsHolder(GUIAlignment.Left, title, GenerateContext(4f, 9f)), out _);

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
                button.OnClick.AddListener((Action)(() => Close()));
                button.transform.localPosition = new Vector3(-4.9733f, 2.6708f, -50f);
            }

            myScreen = Helpers.CreateObject<MetaScreen>("Screen", transform, new Vector3(0, -0.1f, -10f));
            myScreen.SetBorder(new(9f, 5.5f));
        }
    }
}
