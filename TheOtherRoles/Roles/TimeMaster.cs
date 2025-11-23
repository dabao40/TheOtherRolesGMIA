using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class TimeMaster : RoleBase<TimeMaster> {
        public static Color color = new Color32(112, 142, 239, byte.MaxValue);

        public static float rewindTime = 3f;
        public static float shieldDuration = 3f;
        public static float cooldown = 30f;

        public bool shieldActive = false;
        public static bool isRewinding = false;

        private static Sprite buttonSprite;
        private static Sprite buttonSprite2;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TimeShieldButton.png", 115f);
            return buttonSprite;
        }
        public static Sprite getButtonSprite2()
        {
            if (buttonSprite2) return buttonSprite2;
            buttonSprite2 = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TimeMasterRewindButton.png", 115f);
            return buttonSprite2;
        }

        public TimeMaster()
        {
            RoleId = roleId = RoleId.TimeMaster;
            shieldActive = false;
        }

        public static RemoteProcess<byte> UseShield = RemotePrimitiveProcess.OfByte("TimeMasterShield", (message, _) =>
        {
            var timeMaster = getRole(Helpers.playerById(message));
            timeMaster.shieldActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(shieldDuration, new System.Action<float>((p) => {
                if (p == 1f) timeMaster.shieldActive = false;
            })));
        });

        public static void clearAndReload() {
            isRewinding = false;
            rewindTime = CustomOptionHolder.timeMasterRewindTime.getFloat();
            shieldDuration = CustomOptionHolder.timeMasterShieldDuration.getFloat();
            cooldown = CustomOptionHolder.timeMasterCooldown.getFloat();
            players = [];
        }
    }
}
