using System;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles;

public class Portalmaker : RoleBase<Portalmaker>
{
    public static Color color = new Color32(69, 69, 169, byte.MaxValue);

    public Portalmaker()
    {
        RoleId = roleId = RoleId.Portalmaker;
        acTokenChallenge = null;
    }

    public static HelpSprite[] helpSprite = [new(getPlacePortalButtonSprite(), "portalmakerCreateHint")];

    public static float cooldown;
    public static float usePortalCooldown;
    public static bool logOnlyHasColors;
    public static bool logShowsTime;
    public static bool canPortalFromAnywhere;

    private static Sprite placePortalButtonSprite;
    private static Sprite usePortalButtonSprite;
    public AchievementToken<(int portal, bool cleared)> acTokenChallenge = null;

    public static Sprite getPlacePortalButtonSprite()
    {
        if (placePortalButtonSprite) return placePortalButtonSprite;
        placePortalButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlacePortalButton.png", 115f);
        return placePortalButtonSprite;
    }

    public static Sprite getUsePortalButtonSprite()
    {
        if (usePortalButtonSprite) return usePortalButtonSprite;
        usePortalButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalButton.png", 115f);
        return usePortalButtonSprite;
    }

    public override void OnMeetingStart()
    {
        if (player != null && (PlayerControl.LocalPlayer == player || Helpers.shouldShowGhostInfo()) && !player.Data.IsDead)
        {
            if (Portal.teleportedPlayers.Count > 0)
            {
                string msg = ModTranslation.getString("portalmakerLog");
                foreach (var entry in Portal.teleportedPlayers)
                {
                    float timeBeforeMeeting = ((float)(DateTime.UtcNow - entry.time).TotalMilliseconds) / 1000;
                    msg += logShowsTime ? string.Format(ModTranslation.getString("portalmakerLogTime"), (int)timeBeforeMeeting) : "";
                    msg = msg + string.Format(ModTranslation.getString("portalmakerLogName"), entry.name, entry.startingRoom, entry.endingRoom);
                }
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, $"{msg}", false);
            }
        }
    }

    public static void clearAndReload()
    {
        cooldown = CustomOptionHolder.portalmakerCooldown.getFloat();
        usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.getFloat();
        logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.getBool();
        logShowsTime = CustomOptionHolder.portalmakerLogHasTime.getBool();
        canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.getBool();
        players = [];
    }

    public override void PostInit()
    {
        if (PlayerControl.LocalPlayer != player) return;
        acTokenChallenge ??= new("portalmaker.challenge", (0, false), (val, _) => val.cleared);
    }
}
