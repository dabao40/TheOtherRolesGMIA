using System.Linq;
using Hazel;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class NekoKabocha : RoleBase<NekoKabocha>
    {
        public NekoKabocha()
        {
            RoleId = roleId = RoleId.NekoKabocha;
            meetingKiller = null;
            otherKiller = null;
        }

        public static Color color = Palette.ImpostorRed;

        public static bool revengeCrew = true;
        public static bool revengeImpostor = true;
        public static bool revengeNeutral = true;
        public static bool revengeExile = false;

        public PlayerControl meetingKiller = null;
        public PlayerControl otherKiller = null;

        public override void OnMeetingStart()
        {
            meetingKiller = null;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            meetingKiller = null;
        }

        public override void OnDeath(PlayerControl killer = null)
        {
            killer ??= meetingKiller;
            if (killer != null && killer != player)
                if (!killer.Data.IsDead)
                    if (killer.Data.Role.IsImpostor && revengeImpostor
                        || Helpers.isNeutral(killer) && revengeNeutral
                        || revengeCrew && killer.isCrew())
                    {
                        if (meetingKiller == null)
                            player.MurderPlayer(killer, MurderResultFlags.Succeeded);
                        else {
                            killer.Exiled();
                            if (PlayerControl.LocalPlayer == killer)
                                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(player.Data, killer.Data);
                            RPCProcedure.updateMeeting(killer.PlayerId);
                        }
                        if (PlayerControl.LocalPlayer == player) _ = new StaticAchievementToken("nekoKabocha.challenge");
                        GameHistory.overrideDeathReasonAndKiller(killer, DeadPlayer.CustomDeathReason.Revenge, killer: player);
                    }
            else if (killer == null && revengeExile && otherKiller == null && PlayerControl.LocalPlayer == player)
            {
                var candidates = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x != player && !x.Data.IsDead).ToList();
                int targetID = rnd.Next(0, candidates.Count);
                var target = candidates[targetID];

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NekoKabochaExile, SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.nekoKabochaExile(target.PlayerId, player.PlayerId);
            }
            meetingKiller = null;
        }

        public static void clearAndReload()
        {
            revengeCrew = CustomOptionHolder.nekoKabochaRevengeCrew.getBool();
            revengeImpostor = CustomOptionHolder.nekoKabochaRevengeImpostor.getBool();
            revengeNeutral = CustomOptionHolder.nekoKabochaRevengeNeutral.getBool();
            revengeExile = CustomOptionHolder.nekoKabochaRevengeExile.getBool();
            players = [];
        }
    }
}
