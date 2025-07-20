using System.Collections.Generic;
using Hazel;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Sidekick : RoleBase<Sidekick> {
        public static Color color = new Color32(0, 180, 235, byte.MaxValue);

        public PlayerControl currentTarget;

        public bool wasTeamRed;
        public bool wasImpostor;
        public bool wasSpy;
        public Jackal jackal;

        public static float cooldown = 30f;
        public static bool canUseVents = true;
        public static bool canKill = true;
        public static bool promotesToJackal = true;
        public static bool hasImpostorVision = false;
        public static bool canSabotageLights;

        public static int countLovers()
        {
            int counter = 0;
            foreach (var player in allPlayers)
                if (player.isLovers()) counter += 1;
            return counter;
        }

        public Sidekick()
        {
            RoleId = roleId = RoleId.Sidekick;
            jackal = null;
            currentTarget = null;
            wasTeamRed = wasImpostor = wasSpy = false;
        }

        public override void FixedUpdate()
        {
            sidekickSetTarget();
            sidekickCheckPromotion();
        }

        private void sidekickSetTarget()
        {
            if (player != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (jackal != null) untargetablePlayers.Add(jackal.player);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Sidekick from targeting the Mini unless it has grown up
            if (SchrodingersCat.exists && SchrodingersCat.team == SchrodingersCat.Team.Jackal) untargetablePlayers.AddRange(SchrodingersCat.allPlayers);
            currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            if (canKill) setPlayerOutline(currentTarget, Palette.ImpostorRed);
        }

        private void sidekickCheckPromotion()
        {
            if (player.Data.IsDead == true || !promotesToJackal) return;
            if (jackal == null || jackal.player == null || jackal.player.Data.IsDead || jackal?.player.Data?.Disconnected == true)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes(PlayerControl.LocalPlayer.PlayerId);
            }
        }

        public static void clearAndReload() {
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            canUseVents = CustomOptionHolder.sidekickCanUseVents.getBool();
            canKill = CustomOptionHolder.sidekickCanKill.getBool();
            promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.getBool();
            hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            canSabotageLights = CustomOptionHolder.sidekickCanSabotageLights.getBool();
            players = [];
        }
    }
}
