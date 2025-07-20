using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Opportunist : RoleBase<Opportunist>
    {
        public static Color color = new Color32(0, 255, 00, byte.MaxValue);
        public bool notAckedExile = false;

        public Opportunist()
        {
            RoleId = roleId = RoleId.Opportunist;
            notAckedExile = false;
        }

        public static void clearAndReload()
        {
            players = [];
        }
    }
}
