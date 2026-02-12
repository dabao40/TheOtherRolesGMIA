using System.Linq;
using HarmonyLib;
using Hazel;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
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

        public static RemoteProcess<byte> TriggerWin = RemotePrimitiveProcess.OfByte("JesterWin", (message, _) => players.FirstOrDefault(x => x.player?.PlayerId == message).triggerJesterWin = true);

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
