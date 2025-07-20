using Hazel;
using TheOtherRoles.MetaContext;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles;
public class Detective : RoleBase<Detective>
{
    public static Color color = new Color32(45, 106, 165, byte.MaxValue);

    public static float footprintIntervall = 1f;
    public static float footprintDuration = 1f;
    public static bool anonymousFootprints = false;
    public static float reportNameDuration = 0f;
    public static float reportColorDuration = 20f;
    public float timer = 6.2f;
    public static float inspectCooldown = 15f;
    public static float inspectDuration = 10f;

    public Detective()
    {
        RoleId = roleId = RoleId.Detective;
        acTokenCommon = null;
        acTokenChallenge = null;
        timer = 6.2f;
    }

    public AchievementToken<bool> acTokenCommon = null;
    public AchievementToken<(bool reported, byte votedFor, byte killerId, bool cleared)> acTokenChallenge = null;
    public static SpriteLoader detectiveIcon = SpriteLoader.FromResource("TheOtherRoles.Resources.DetectiveArrow.png", 100f);

    private static Sprite buttonSprite;
    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DetectiveButton.png", 115f);
        return buttonSprite;
    }

    public override void FixedUpdate()
    {
        if (player != PlayerControl.LocalPlayer) return;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            timer = footprintIntervall;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent && !(Ninja.isStealthed(player) || Sprinter.isSprinting(player)
                    || (player.isRole(RoleId.Fox) && Fox.stealthed) || (player.isRole(RoleId.Kataomoi) && Kataomoi.isStalking())))
                {
                    FootprintHolder.Instance.MakeFootprint(player);
                }
            }
        }
    }

    public override void PostInit()
    {
        if (PlayerControl.LocalPlayer != player) return;
        acTokenChallenge ??= new("detective.challenge", (false, byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
        acTokenCommon ??= new("detective.common1", false, (val, _) => val);
    }

    public override void OnMeetingEnd(PlayerControl exiled = null)
    {
        if (!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer == player)
        {
            if (exiled != null)
                acTokenChallenge.Value.cleared |= acTokenChallenge.Value.reported && acTokenChallenge.Value.votedFor == exiled.PlayerId;
            acTokenChallenge.Value.reported = false;
            acTokenChallenge.Value.killerId = byte.MaxValue;
        }
    }

    public static void unlockAch(byte votedFor, byte playerId)
    {
        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockDetectiveAcChallenge, SendOption.Reliable, -1);
        writer.Write(votedFor);
        writer.Write(playerId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.unlockDetectiveAcChallenge(votedFor, playerId);
    }

    public static void clearAndReload()
    {
        anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
        footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
        footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
        reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
        reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
        inspectCooldown = CustomOptionHolder.detectiveInspectCooldown.getFloat();
        inspectDuration = CustomOptionHolder.detectiveInspectDuration.getFloat();
        players = [];
    }
}
