using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Yoyo : RoleBase<Yoyo>
    {
        public static Color color = Palette.ImpostorRed;

        public Yoyo()
        {
            RoleId = roleId = RoleId.Yoyo;
            markedLocation = null;
        }

        static public HelpSprite[] helpSprite = [new(getMarkButtonSprite(), "yoyoMarkHint"), new(getBlinkButtonSprite(), "yoyoBlinkHint")];

        public static float blinkDuration = 0;
        public static float markCooldown = 0;
        public static bool markStaysOverMeeting = false;
        public float SilhouetteVisibility => silhouetteVisibility == 0 && (PlayerControl.LocalPlayer == player || PlayerControl.LocalPlayer.Data.IsDead) ? 0.1f : silhouetteVisibility;
        public static float silhouetteVisibility = 0;

        public Vector3? markedLocation = null;

        private static Sprite markButtonSprite;

        public static Sprite getMarkButtonSprite()
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.YoyoMarkButtonSprite.png", 115f);
            return markButtonSprite;
        }
        private static Sprite blinkButtonSprite;

        public static Sprite getBlinkButtonSprite()
        {
            if (blinkButtonSprite) return blinkButtonSprite;
            blinkButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.YoyoBlinkButtonSprite.png", 115f);
            return blinkButtonSprite;
        }

        public void markLocation(Vector3 position)
        {
            markedLocation = position;
        }

        public override void ResetRole(bool isShifted)
        {
            markedLocation = null;
            Silhouette.clearSilhouettes(player);
        }

        public static void clearAndReload()
        {
            blinkDuration = CustomOptionHolder.yoyoBlinkDuration.getFloat();
            markCooldown = CustomOptionHolder.yoyoMarkCooldown.getFloat();
            markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.getBool();
            silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.getSelection() / 10f;
            players = [];
        }
    }
}
