using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Janitor : RoleBase<Janitor>
    {
        public static Color color = Palette.ImpostorRed;

        public static float cooldown = 30f;

        public Janitor()
        {
            RoleId = roleId = RoleId.Janitor;
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            cooldown = CustomOptionHolder.janitorCooldown.getFloat();
            players = [];
        }
    }
}
