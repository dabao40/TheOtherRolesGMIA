using HarmonyLib;

namespace TheOtherRoles.Modules.CustomHats.Patches
{
    [HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.CheckUnlockedItems))]
    public static class InventoryManager_CheckUnlockedItems_Patch
    {
        public static void Prefix()
        {
            CustomHatManager.LoadHats();
        }
    }
}
