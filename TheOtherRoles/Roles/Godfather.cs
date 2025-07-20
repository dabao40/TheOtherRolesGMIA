using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles;

public class Godfather : RoleBase<Godfather>
{
    public static Color color = Palette.ImpostorRed;

    public Godfather()
    {
        RoleId = roleId = RoleId.Godfather;
    }

    public override void OnDeath(PlayerControl killer = null)
    {
        if (PlayerControl.LocalPlayer.isRole(RoleId.Mafioso) && !PlayerControl.LocalPlayer.Data.IsDead)
            _ = new StaticAchievementToken("mafioso.another1");
    }

    public static void clearAndReload()
    {
        players = [];
    }
}
