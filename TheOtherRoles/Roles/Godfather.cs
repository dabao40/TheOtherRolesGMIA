using System.Collections.Generic;
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

    public static List<PlayerControl> killed = [];
    public static bool shareInfo = true;

    public override void OnKill(PlayerControl target)
    {
        killed.TryAdd(target);
    }

    public static bool shouldShowInfo(PlayerControl player) => isRole(player) || ((player.isRole(RoleId.Mafioso) || player.isRole(RoleId.Janitor)) && shareInfo);

    public override void OnDeath(PlayerControl killer = null)
    {
        if (PlayerControl.LocalPlayer.isRole(RoleId.Mafioso) && !PlayerControl.LocalPlayer.Data.IsDead)
            _ = new StaticAchievementToken("mafioso.another1");
    }

    public static void clearAndReload()
    {
        killed = [];
        shareInfo = CustomOptionHolder.godfatherShareInfo.getBool();
        players = [];
    }
}
