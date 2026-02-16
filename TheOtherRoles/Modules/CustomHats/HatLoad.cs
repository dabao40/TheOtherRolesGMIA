using HarmonyLib;
using PowerTools;
using TheOtherRoles.Modules.CustomHats;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Modules.CustomHats.Patches
{
    [HarmonyPatch]
    public static class HatLoad
    {
        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
        public static class SetHat_Patch
        {
            public static bool Prefix(HatParent __instance, int color)
            {
                if (!CustomHatManager.ViewDataCache.ContainsKey(__instance.Hat.name)) return true;
                __instance.UnloadAsset();
                __instance.PopulateFromViewData();
                __instance.SetMaterialColor(color);
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.PopulateFromViewData))]
        public static class PopulateFromViewData_Patch
        {
            public static bool Prefix(HatParent __instance)
            {
                if (!CustomHatManager.ViewDataCache.TryGetValue(__instance.Hat.name, out var viewData))
                    return true;

                var mainImage = viewData.MainImage;
                if (__instance.Hat.InFront)
                {
                    __instance.BackLayer.enabled = false;
                    __instance.FrontLayer.enabled = true;
                    __instance.FrontLayer.sprite = mainImage;
                }
                else
                {
                    __instance.BackLayer.enabled = true;
                    __instance.FrontLayer.enabled = false;
                    __instance.FrontLayer.sprite = null;
                    __instance.BackLayer.sprite = mainImage;
                }

                var spriteAnimNodeSync = __instance.SpriteSyncNode ?? __instance.GetComponent<SpriteAnimNodeSync>();
                if ((bool)(Object)spriteAnimNodeSync)
                    spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;

                if (__instance.options.Initialized && __instance.HideHat())
                {
                    __instance.FrontLayer.enabled = false;
                    __instance.BackLayer.enabled = false;
                }

                __instance.UpdateMaterial();
                return false;
            }
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.UpdateMaterial))]
        [HarmonyPrefix]
        public static bool UpdateMaterial_Prefix(HatParent __instance)
        {
            if (__instance == null) return true;
            if (__instance.Hat == null) return true;
            if (__instance.FrontLayer == null) return false;

            if (!CustomHatManager.ViewDataCache.ContainsKey(__instance.Hat.name))
                return true;

            var hat = __instance.Hat;
            var hatManager = DestroyableSingleton<HatManager>.Instance;
            if (hatManager == null)
            {
                __instance.FrontLayer.sharedMaterial = null;
                return false;
            }

            bool isAdaptive = CustomHatManager.AdaptiveCache.TryGetValue(hat.name, out var adaptive) && adaptive;

            if (isAdaptive)
            {
                __instance.FrontLayer.sharedMaterial = hatManager.PlayerMaterial;
                if (__instance.BackLayer != null)
                    __instance.BackLayer.sharedMaterial = hatManager.PlayerMaterial;
            }
            else
            {
                __instance.FrontLayer.sharedMaterial = hatManager.DefaultShader;
                if (__instance.BackLayer != null)
                    __instance.BackLayer.sharedMaterial = hatManager.DefaultShader;
            }

            int colorId = __instance.matProperties.ColorId;
            PlayerMaterial.SetColors(colorId, __instance.FrontLayer);
            if (__instance.BackLayer != null)
                PlayerMaterial.SetColors(colorId, __instance.BackLayer);

            __instance.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
            if (__instance.BackLayer != null)
                __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

            var maskType = __instance.matProperties.MaskType;
            SpriteMaskInteraction interaction = maskType switch
            {
                PlayerMaterial.MaskType.ScrollingUI => SpriteMaskInteraction.VisibleInsideMask,
                PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
                _ => SpriteMaskInteraction.None
            };
            __instance.FrontLayer.maskInteraction = interaction;
            if (__instance.BackLayer != null)
                __instance.BackLayer.maskInteraction = interaction;

            if (__instance.matProperties.MaskLayer <= 0)
            {
                PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);
                if (__instance.BackLayer != null)
                    PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);
            }

            return false;
        }

        [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetClimbAnim))]
        public static class SetClimbAnim_Patch
        {
            public static bool Prefix(HatParent __instance)
            {
                if (!CustomHatManager.ViewDataCache.ContainsKey(__instance.Hat.name)) return true;
                __instance.FrontLayer.sprite = null;
                return false;
            }
        }
    }
}
