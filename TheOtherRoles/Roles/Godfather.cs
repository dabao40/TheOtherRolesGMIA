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

    public static void clearAndReload()
    {
        killed = [];
        shareInfo = CustomOptionHolder.godfatherShareInfo.getBool();
        players = [];
    }
}
