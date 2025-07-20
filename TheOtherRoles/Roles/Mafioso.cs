using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Mafioso : RoleBase<Mafioso>
    {
        public static Color color = Palette.ImpostorRed;

        public Mafioso()
        {
            RoleId = roleId = RoleId.Mafioso;
        }

        public static void clearAndReload()
        {
            players = [];
        }
    }
}
