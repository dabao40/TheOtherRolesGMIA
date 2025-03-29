using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.MetaContext;
using UnityEngine;

namespace TheOtherRoles.Modules;
public static class HelpMenu
{
    [Flags]
    public enum HelpTab
    {
        Achievements = 0x01,
        Objects = 0x02,
    }

    private static TextAttribute TabButtonAttr = new(TextAttribute.BoldAttr) { Size = new(1.15f, 0.26f) };
    private static float HelpHeight = 4.1f;

    private static MetaScreen lastHelpScreen = null;
    public static bool OpenedAnyHelpScreen => lastHelpScreen;
    public static MetaScreen LastHelpScreen => lastHelpScreen;

    public record HelpTabInfo(HelpTab Tab, string TranslateKey)
    {
        public MetaContextOld.Button GetButton(MetaScreen screen, HelpTab currentTab, HelpTab validTabs) => new(() => ShowScreen(screen, Tab, validTabs), TabButtonAttr) { Color = currentTab == Tab ? Color.white : Color.gray, TranslationKey = TranslateKey };
    }

    public static HelpTabInfo[] AllHelpTabInfo = {
        new(HelpTab.Objects, "helpObjectsKey"),
        new(HelpTab.Achievements, "helpAchievementsKey")
    };

    private static IMetaContextOld GetTabsContext(MetaScreen screen, HelpTab tab, HelpTab validTabs)
    {
        List<IMetaParallelPlacableOld> tabs = new();

        foreach (var info in AllHelpTabInfo) if ((validTabs & info.Tab) != 0) tabs.Add(info.GetButton(screen, tab, validTabs));

        return new CombinedContextOld(0.5f, tabs.ToArray());
    }

    private static IMetaContextOld ShowAchievementScreen()
    {
        static GUIContext GenerateContext() => AchievementViewer.GenerateContext(3.15f, 6.2f);
        Artifact<GUIScreen> artifact = null!;
        var screen = new GUIFixedView(GUIAlignment.Top, new(5.7f, 3.8f), GenerateContext()) { WithMask = false };
        artifact = screen.Artifact;
        return new MetaContextOld.WrappedContext(new HorizontalContextsHolder(GUIAlignment.Center, screen));
    }


    static public GUIContext GetImageContent(Image image, GUIContext context) => TORGUIContextEngine.API.HorizontalHolder(GUIAlignment.Left,
        new TORGUIImage(GUIAlignment.Left, image, new(0.55f, null)) { IsMasked = true}, TORGUIContextEngine.API.HorizontalMargin(0.15f), context);
    static public GUIContext GetImageContent(string imagePath, string rawText) => GetImageContent(SpriteLoader.FromResource(imagePath, 100f),
        rawText == null ? TORGUIContextEngine.API.EmptyContext : TORGUIContextEngine.API.RawText(GUIAlignment.Left, TORGUIContextEngine.API.GetAttribute(AttributeAsset.DocumentStandard), rawText));

    static private Dictionary<string, string> Objects = new() { { "Garlic", "garlicObjectHint" }, { "FoxShrineHint", "foxShrineHint"}, { "BombEffect", "bombEffectHint" }, {"PortalAnimation.plattform", "portalHint" },
        {"TricksterAnimation.trickster_box_0001", "tricksterBoxHint" }, {"SilhouetteObjectHint", "silhouetteHint" }, {"AssassinTraceW", "assassinTraceHint" },
        { "ArchaeologistAntique", "antiqueHint"}, {"Trap", "trapHint" }, { "StaticVentSealed", "ventSealedHint"} };
    private static IMetaContextOld ShowObjectsScreen()
    {
        MetaContextOld context = new();
        var inner = new List<GUIContext>();
        var holder = new VerticalContextsHolder(GUIAlignment.Left, inner);
        foreach (var obj in Objects) {
            inner.Add(GetImageContent($"TheOtherRoles.Resources.{obj.Key}.png", ModTranslation.getString(obj.Value)));
            inner.Add(TORGUIContextEngine.API.VerticalMargin(0.25f));
        }
        var scrollView = new GUIScrollView(GUIAlignment.Left, new(6f, HelpHeight - 0.5f), holder);
        context.Append(new MetaContextOld.WrappedContext(scrollView));
        return context;
    }

    public static void TryCloseHelpScreen()
    {
        if (lastHelpScreen) lastHelpScreen!.CloseScreen();
    }
    public static void TryOpenHelpScreen(HelpTab tab)
    {
        if (!lastHelpScreen) lastHelpScreen = OpenHelpScreen(tab);
    }

    private static MetaScreen OpenHelpScreen(HelpTab tab)
    {
        var screen = MetaScreen.GenerateWindow(new(6f, HelpHeight + 0.6f), HudManager.Instance.transform, new Vector3(0, 0, -600), true, false);

        HelpTab validTabs = HelpTab.Objects | HelpTab.Achievements;

        ShowScreen(screen, tab, validTabs);

        return screen;
    }

    private static void ShowScreen(MetaScreen screen, HelpTab tab, HelpTab validTabs)
    {
        MetaContextOld context = new();
        context.Append(GetTabsContext(screen, tab, validTabs));
        context.Append(new MetaContextOld.VerticalMargin(0.1f));

        switch (tab)
        {
            case HelpTab.Achievements:
                context.Append(ShowAchievementScreen());
                break;
            case HelpTab.Objects:
                context.Append(ShowObjectsScreen());
                break;
        }

        screen.SetContext(context);
    }
}
