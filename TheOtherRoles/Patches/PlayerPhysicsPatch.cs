using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.Awake))]
public static class PlayerPhysiscs_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(PlayerPhysics __instance)
    {
        if (!__instance.body) return;
        __instance.body.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
}
[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
class PlayerPhysics_FixedUpdate
{
    public static void Postfix()
    {
        if(TheOtherRoles.Reverser.InEffort == true)
        {
            PlayerControl.LocalPlayer.MyPhysics.body.velocity *= -1;
        }

    }
}
