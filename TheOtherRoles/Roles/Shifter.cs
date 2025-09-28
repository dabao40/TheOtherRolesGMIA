using System.Collections.Generic;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Shifter : RoleBase<Shifter>
    {
        public Shifter()
        {
            RoleId = roleId = RoleId.Shifter;
        }

        public static List<int> pastShifters = [];
        public static Color color = new Color32(102, 102, 102, byte.MaxValue);

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;
        public static bool shiftModifiers = false;

        public static bool isNeutral = false;
        public static bool shiftPastShifters = false;
        public static bool shiftsMedicShield = false;

        public static AchievementToken<(byte shiftId, bool cleared)> niceShifterAcTokenChallenge = null;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static RemoteProcess<byte> Shift = RemotePrimitiveProcess.OfByte("ShifterShift", (message, _) => RPCProcedure.shifterShift(message));

        public static RemoteProcess<byte> SetFutureShifted = RemotePrimitiveProcess.OfByte("SetFutureShifted", (message, _) =>
        {
            if (isNeutral && !shiftPastShifters && pastShifters.Contains(message))
                return;
            futureShift = Helpers.playerById(message);
        });

        public static RemoteProcess<bool> SetType = RemotePrimitiveProcess.OfBoolean("SetShifterType", (message, _) => isNeutral = message);

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            List<PlayerControl> blockShift = null;
            if (isNeutral && !shiftPastShifters)
            {
                blockShift = [];
                foreach (var playerId in pastShifters)
                    blockShift.Add(Helpers.playerById((byte)playerId));
            }

            currentTarget = setTarget(untargetablePlayers: blockShift);
            if (futureShift == null) setPlayerOutline(currentTarget, color);
        }

        public override void PostInit()
        {
            if (player == PlayerControl.LocalPlayer && !isNeutral)
                niceShifterAcTokenChallenge ??= new("niceShifter.challenge", (byte.MaxValue, false), (val, _) => val.cleared);
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            if (PlayerControl.LocalPlayer == player && !isNeutral && exiled != null)
                niceShifterAcTokenChallenge.Value.cleared |= niceShifterAcTokenChallenge.Value.shiftId == exiled.PlayerId;
        }

        public static void clearAndReload()
        {
            pastShifters = [];
            currentTarget = null;
            futureShift = null;
            shiftModifiers = CustomOptionHolder.shifterShiftsModifiers.getBool();
            shiftPastShifters = CustomOptionHolder.shifterPastShifters.getBool();
            shiftsMedicShield = CustomOptionHolder.shifterShiftsMedicShield.getBool();
            isNeutral = false;
            players = [];
            niceShifterAcTokenChallenge = null;
        }
    }
}
