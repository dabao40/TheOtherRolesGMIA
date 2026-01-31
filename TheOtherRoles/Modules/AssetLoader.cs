using System.Reflection;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Modules
{
    public static class AssetLoader
    {
        private static readonly Assembly dll = Assembly.GetExecutingAssembly();
        private static bool flag = false;
        static internal AssetBundle AssetBundle { get; private set; } = null!;

        public static void LoadAssets()
        {
            if (flag) return;
            flag = true;
            LoadAudioAssets();
            LoadSpriteAssets();
            LoadShaderAssets();
#if WINDOWS
            var resourceStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.illustrationassets_Win");
#else
            var resourceStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.illustrationasset_Android");
#endif
            AssetBundle = AssetBundle.LoadFromMemory(resourceStream!.ReadFully());
        }

        private static void LoadAudioAssets()
        {
#if WINDOWS
            var resourceAudioAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.audiobundle_Win");
#else
            var resourceAudioAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.audiobundle_Android");
#endif
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceAudioAssetBundleStream.ReadFully());
            Trap.activate = assetBundleBundle.LoadAsset<AudioClip>("TrapperActivate.mp3").DontUnload();
            Trap.countdown = assetBundleBundle.LoadAsset<AudioClip>("TrapperCountdown.mp3").DontUnload();
            Trap.disable = assetBundleBundle.LoadAsset<AudioClip>("TrapperDisable.mp3").DontUnload();
            Trap.kill = assetBundleBundle.LoadAsset<AudioClip>("TrapperKill.mp3").DontUnload();
            Trap.place = assetBundleBundle.LoadAsset<AudioClip>("TrapperPlace.mp3").DontUnload();
        }

        private static void LoadSpriteAssets()
        {
#if WINDOWS
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.spriteassets_Win");
#else
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.spriteassets_Android");
#endif
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceTestAssetBundleStream.ReadFully());
            FoxTask.prefab = assetBundleBundle.LoadAsset<GameObject>("FoxTask.prefab").DontUnload();
            Shrine.sprite = assetBundleBundle.LoadAsset<Sprite>("shrine2.png").DontUnload();
        }

        private static void LoadShaderAssets()
        {
#if WINDOWS
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.shaderassets_Win");
#else
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.shaderassets_Android");
#endif
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceTestAssetBundleStream.ReadFully());
            Achievement.materialShader = assetBundleBundle.LoadAsset<Shader>("Sprites-White").DontUnload();
        }
    }
}
