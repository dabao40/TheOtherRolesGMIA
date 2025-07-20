using Hazel;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Lawyer : RoleBase<Lawyer> {
        public static PlayerControl target;
        public static Color color = new Color32(134, 153, 25, byte.MaxValue);
        public static Sprite targetSprite;
        public static bool targetKnows = true;
        public static bool triggerLawyerWin = false;
        public static int meetings = 0;

        public Lawyer()
        {
            RoleId = roleId = RoleId.Lawyer;
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;

            // Meeting win
            if (winsAfterMeetings && neededMeetings == meetings && target != null && !target.Data.IsDead)
            {
                winsAfterMeetings = false; // Avoid sending mutliple RPCs until the host finshes the game
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerWin, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                RPCProcedure.lawyerWin();
                return;
            }

            // Promote to Pursuer
            if (target != null && target.Data.Disconnected && !player.Data.IsDead)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable, -1);
                writer.Write(player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer(player.PlayerId);
                return;
            }
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            if (PlayerControl.LocalPlayer == player && !player.Data.IsDead)
                meetings++;
        }

        public override void OnDeath(PlayerControl killer = null)
        {
            if (!FreePlayGM.isFreePlayGM) player.clearAllTasks();
            if (killer != null && PlayerControl.LocalPlayer == player && killer == target)
                _ = new StaticAchievementToken("lawyer.another1");
        }

        public override void ResetRole(bool isShifted)
        {
            if (target != null && player == PlayerControl.LocalPlayer) {
                Transform playerInfoTransform = target.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo != null) playerInfo.text = "";
            }
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
        {
            if (this.player == player) target = null;
        }

        public static bool winsAfterMeetings = false;
        public static int neededMeetings = 4;
        public static float vision = 1f;
        public static bool lawyerTargetKnows = true;
        public static bool lawyerKnowsRole = false;
        public static bool targetCanBeJester = false;
        public static bool targetWasGuessed = false;

        public static void clearTarget()
        {
            target = null;
            targetWasGuessed = false;
        }

        public static void clearAndReload() {
            clearTarget();
            triggerLawyerWin = false;
            meetings = 0;
            vision = CustomOptionHolder.lawyerVision.getFloat();
            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
            lawyerTargetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
            targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.getBool();
            winsAfterMeetings = CustomOptionHolder.lawyerWinsAfterMeetings.getBool();
            neededMeetings = Mathf.RoundToInt(CustomOptionHolder.lawyerNeededMeetings.getFloat());
            targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
            players = [];
        }
    }
}
