using System;
using Hazel;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Busker : RoleBase<Busker>
    {
        public static Color color = new(255f / 255f, 172f / 255f, 117f / 255f);

        public Busker()
        {
            RoleId = roleId = RoleId.Busker;
            pseudocideFlag = false;
            buttonInterrupted = false;
            pseudocideComplete = false;
            deathPosition = new Vector3();
            acTokenChallenge = null;
        }

        public static float cooldown = 30f;
        public static float duration = 10f;
        public bool pseudocideFlag = false;
        public bool buttonInterrupted = false;
        public bool pseudocideComplete = false;
        public static bool restrictInformation = true;
        public Vector3 deathPosition = new();
        public AchievementToken<(DateTime pseudocide, bool cleared)> acTokenChallenge = null;

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("busker.challenge", (DateTime.UtcNow, false), (val, _) => val.cleared);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BuskerButton.png", 115f);
            return buttonSprite;
        }

        public void dieBusker(bool isLoverSuicide = false)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRPC.BuskerPseudocide, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            writer.Write(true);
            writer.Write(isLoverSuicide);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.buskerPseudocide(player.PlayerId, true, isLoverSuicide);
        }

        public bool checkPseudocide()
        {
            if (!pseudocideFlag) return false;

            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            foreach (var deadBody in array)
                if (deadBody.ParentId == player.PlayerId)
                    return true;
            dieBusker();
            return false;
        }

        public static void clearAndReload()
        {
            cooldown = CustomOptionHolder.buskerCooldown.getFloat();
            duration = CustomOptionHolder.buskerDuration.getFloat();
            restrictInformation = CustomOptionHolder.buskerRestrictInformation.getBool();
            players = [];
        }
    }
}
