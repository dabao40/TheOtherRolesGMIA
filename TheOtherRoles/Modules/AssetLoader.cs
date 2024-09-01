using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using TheOtherRoles.Objects;
using UnityEngine;
using UnityEngine.UI;
using Reactor.Utilities.Extensions;
using Il2CppSystem;

namespace TheOtherRoles.Modules
{
    public static class AssetLoader
    {
        private static readonly Assembly dll = Assembly.GetExecutingAssembly();
        private static bool flag = false;

        public static void LoadAssets()
        {
            if (flag) return;
            flag = true;
            LoadAudioAssets();
            LoadSpriteAssets();
            LoadShaderAssets();
        }

        private static void LoadAudioAssets()
        {
            var resourceAudioAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.audiobundle");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceAudioAssetBundleStream.ReadFully());
            Trap.activate = assetBundleBundle.LoadAsset<AudioClip>("TrapperActivate.mp3").DontUnload();
            Trap.countdown = assetBundleBundle.LoadAsset<AudioClip>("TrapperCountdown.mp3").DontUnload();
            Trap.disable = assetBundleBundle.LoadAsset<AudioClip>("TrapperDisable.mp3").DontUnload();
            Trap.kill = assetBundleBundle.LoadAsset<AudioClip>("TrapperKill.mp3").DontUnload();
            Trap.place = assetBundleBundle.LoadAsset<AudioClip>("TrapperPlace.mp3").DontUnload();
        }

        private static void LoadSpriteAssets()
        {
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.spriteassets");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceTestAssetBundleStream.ReadFully());
            FoxTask.prefab = assetBundleBundle.LoadAsset<GameObject>("FoxTask.prefab").DontUnload();
            Shrine.sprite = assetBundleBundle.LoadAsset<Sprite>("shrine2.png").DontUnload();
        }

        private static void LoadShaderAssets()
        {
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.shaderassets");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceTestAssetBundleStream.ReadFully());
            Achievement.materialShader = assetBundleBundle.LoadAsset<Shader>("Sprites-White").DontUnload();
            Helpers.HSVShader = assetBundleBundle.LoadAsset<Shader>("Sprites-HSV").DontUnload();
        }
    }
}
