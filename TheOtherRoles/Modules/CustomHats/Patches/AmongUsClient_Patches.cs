using HarmonyLib;
using TheOtherRoles.Modules.CustomHats;

namespace TheOtherRoles.Modules.CustomHats.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    public static class AmongUsClient_Awake_Patch
    {
        private static bool _executed;

        public static void Prefix()
        {
            if (!_executed)
            {
                CustomHatManager.LoadHats();
                _executed = true;
            }
        }
    }
}
