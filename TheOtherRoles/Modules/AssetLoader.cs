using System.Collections.Generic;
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

        public static Dictionary<string, AudioClip> AudioClips { get; private set; } = new();
        public static Dictionary<string, Sprite> Sprites { get; private set; } = new();
        public static Dictionary<string, Shader> Shaders { get; private set; } = new();

        public static void LoadAssets()
        {
            if (flag)
            {
                return;
            }

            flag = true;

            try
            {
#if WINDOWS
                var AssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.assetsbundle-Windows");
#else
                var AssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.assetsbundle-Android");
#endif

                if (AssetBundleStream == null)
                {
                    return;
                }

                AssetBundle = AssetBundle.LoadFromMemory(AssetBundleStream.ReadFully());

                if (AssetBundle == null)
                {
                    return;
                }

                var allAssetNames = AssetBundle.GetAllAssetNames();

                foreach (var assetName in allAssetNames)
                {
                    var asset = AssetBundle.LoadAsset(assetName);

                    if (asset == null)
                    {
                        continue;
                    }

                    var lowerName = assetName.ToLower();

                    if (asset.TryCast<AudioClip>() is AudioClip audioClip)
                    {
                        AudioClips[lowerName] = audioClip.DontUnload();
                    }
                    else if (asset.TryCast<Texture2D>() is Texture2D texture)
                    {
                        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        sprite.DontUnload();
                        Sprites[lowerName] = sprite;
                    }
                    else if (asset.TryCast<Shader>() is Shader shader)
                    {
                        Shaders[lowerName] = shader.DontUnload();
                    }
                    else
                    {
                    }
                }

                Trap.activate = GetAudioClip("trapperactivate.ogg");
                Trap.countdown = GetAudioClip("trappercountdown.ogg");
                Trap.disable = GetAudioClip("trapperdisable.ogg");
                Trap.kill = GetAudioClip("trapperkill.ogg");
                Trap.place = GetAudioClip("trapperplace.ogg");

                FoxTask.prefab = AssetBundle.LoadAsset<GameObject>("FoxTask.prefab").DontUnload();

                Shrine.sprite = AssetBundle.LoadAsset<Sprite>("assets/sprites/shrine2.png").DontUnload();

                Helpers.achievementMaterialShader = GetShader("sprites-white.shader");
                Helpers.achievementProgressShader = GetShader("sprites-progress.shader");
            }
            catch
            {
            }
        }

        public static AudioClip GetAudioClip(string path)
        {
            var key = path.StartsWith("assets/") ? path.ToLower() : $"assets/audio/{path.ToLower()}";

            if (AudioClips.TryGetValue(key, out var clip))
            {
                return clip;
            }

            foreach (var existingKey in AudioClips.Keys)
            {
                if (existingKey.EndsWith(path.ToLower()) || existingKey.Contains(path.ToLower()))
                {
                    return AudioClips[existingKey];
                }
            }

            return null;
        }

        public static Sprite GetSprite(string path)
        {
            var key = path.StartsWith("assets/") ? path.ToLower() : $"assets/sprites/{path.ToLower()}";

            if (Sprites.TryGetValue(key, out var sprite))
            {
                return sprite;
            }

            var altKey = path.StartsWith("assets/") ? path.ToLower() : $"assets/prefab/{path.ToLower()}";
            if (Sprites.TryGetValue(altKey, out sprite))
            {
                return sprite;
            }

            foreach (var existingKey in Sprites.Keys)
            {
                if (existingKey.EndsWith(path.ToLower()) || existingKey.Contains(path.ToLower()))
                {
                    return Sprites[existingKey];
                }
            }

            return null;
        }

        public static Shader GetShader(string path)
        {
            var key = path.StartsWith("assets/") ? path.ToLower() : $"assets/shaders/{path.ToLower()}";

            if (Shaders.TryGetValue(key, out var shader))
            {
                return shader;
            }

            foreach (var existingKey in Shaders.Keys)
            {
                if (existingKey.EndsWith(path.ToLower()) || existingKey.Contains(path.ToLower()))
                {
                    return Shaders[existingKey];
                }
            }

            return null;
        }
    }
}
