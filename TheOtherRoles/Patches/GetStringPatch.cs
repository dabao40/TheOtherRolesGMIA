using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch]
    class GetStringPatch
    {
        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), [
                typeof(StringNames),
                typeof(Il2CppReferenceArray<Il2CppSystem.Object>)
            ])]
        public static bool Prefix(TranslationController __instance, StringNames id, ref string __result)
        {
            if ((int)id < 6000)
            {
                return true;
            }
            string ourString = "";

            // For now only do this in custom options.
            int idInt = (int)id - 6000;
            CustomOption opt = CustomOption.options.FirstOrDefault(x => x.id == idInt);
            ourString = opt?.name;

            __result = ourString;

            return false;
        }
    }

    [HarmonyPatch(typeof(TextTranslatorTMP), nameof(TextTranslatorTMP.ResetText))]
    public static class TextPatch
    {
        static public bool Prefix(TextTranslatorTMP __instance)
        {
            if ((short)__instance.TargetText != short.MaxValue) return true;
            __instance.GetComponent<TMPro.TextMeshPro>().text = ModTranslation.getString(__instance.defaultStr);
            return false;
        }
    }
}
