using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Lighter : RoleBase<Lighter>
    {
        public static Color color = new Color32(238, 229, 190, byte.MaxValue);

        public static float lighterModeLightsOnVision = 2f;
        public static float lighterModeLightsOffVision = 0.75f;
        public static float flashlightWidth = 0.75f;
        public static bool canSeeInvisible = true;

        public Lighter()
        {
            RoleId = roleId = RoleId.Lighter;
        }

        public static void clearAndReload()
        {
            flashlightWidth = CustomOptionHolder.lighterFlashlightWidth.getFloat();
            lighterModeLightsOnVision = CustomOptionHolder.lighterModeLightsOnVision.getFloat();
            lighterModeLightsOffVision = CustomOptionHolder.lighterModeLightsOffVision.getFloat();
            canSeeInvisible = CustomOptionHolder.lighterCanSeeInvisible.getBool();
            players = [];
        }
    }
}
