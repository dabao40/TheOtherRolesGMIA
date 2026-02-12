using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Veteran : RoleBase<Veteran>
    {
        public static Color color = new Color32(255, 77, 0, byte.MaxValue);

        public Veteran()
        {
            RoleId = roleId = RoleId.Veteran;
            remainingAlerts = Mathf.RoundToInt(CustomOptionHolder.veteranAlertNumber.getFloat());
            alertActive = false;
        }

        public static RemoteProcess<byte> ActivateAlert = RemotePrimitiveProcess.OfByte("VeteranAlert", (message, _) =>
        {
            PlayerControl player = Helpers.playerById(message);
            var veteran = getRole(player);
            if (player == null || veteran == null) return;
            veteran.alertActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(alertDuration, new System.Action<float>((p) => {
                if (p == 1f) veteran.alertActive = false;
            })));
        });

        public static float alertDuration = 3f;
        public static float cooldown = 30f;

        public int remainingAlerts = 5;

        public bool alertActive = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AlertButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            alertDuration = CustomOptionHolder.veteranAlertDuration.getFloat();
            cooldown = CustomOptionHolder.veteranCooldown.getFloat();
            players = [];
        }
    }
}
