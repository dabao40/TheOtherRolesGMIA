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

namespace TheOtherRoles.Modules
{
    public static class AssetLoader
    {
        private static readonly Assembly dll = Assembly.GetExecutingAssembly();

        public static void LoadAudioAssets()
        {
            var resourceAudioAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.audiobundle");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceAudioAssetBundleStream.ReadFully());
            Trap.activate = assetBundleBundle.LoadAsset<AudioClip>("TrapperActivate.mp3").DontUnload();
            Trap.countdown = assetBundleBundle.LoadAsset<AudioClip>("TrapperCountdown.mp3").DontUnload();
            Trap.disable = assetBundleBundle.LoadAsset<AudioClip>("TrapperDisable.mp3").DontUnload();
            Trap.kill = assetBundleBundle.LoadAsset<AudioClip>("TrapperKill.mp3").DontUnload();
            Trap.place = assetBundleBundle.LoadAsset<AudioClip>("TrapperPlace.mp3").DontUnload();
        }
    }
}
