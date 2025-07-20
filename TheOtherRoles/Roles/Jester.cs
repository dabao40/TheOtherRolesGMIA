using HarmonyLib;
using Hazel;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Jester : RoleBase<Jester>
    {
        public static Color color = new Color32(236, 98, 165, byte.MaxValue);

        public bool triggerJesterWin = false;
        public static bool canCallEmergency = true;
        public static bool hasImpostorVision = false;
        public static bool canUseVents = false;

        public Jester()
        {
            RoleId = roleId = RoleId.Jester;
            triggerJesterWin = false;
        }

        public static void unlockAch(byte playerId)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockJesterAcCommon, SendOption.Reliable, -1);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.unlockJesterAcCommon(playerId);
        }

        public static void clearAndReload()
        {
            if (players != null) players.Do(x => x.triggerJesterWin = false);
            canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
            hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.getBool();
            canUseVents = CustomOptionHolder.jesterCanVent.getBool();
            players = [];
        }
    }
}
