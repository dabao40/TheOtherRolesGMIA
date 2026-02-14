using System;
using UnityEngine;
using TheOtherRoles.Modules;

namespace TheOtherRoles
{
    // Class to preload all audio/sound effects that are contained in the embedded resources.
    // The effects are made available through the soundEffects Dict / the get and the play methods.
    public static class SoundEffectsManager
    {
        public static AudioSource play(string path, float volume = 0.8f, bool loop = false, bool musicChannel = false)
        {
            if (!TORMapOptions.enableSoundEffects) return null;

            AudioClip clipToPlay = AssetLoader.GetAudioClip(path);
            stop(path);

            if (Constants.ShouldPlaySfx() && clipToPlay != null)
            {
                AudioSource source = SoundManager.Instance.PlaySound(clipToPlay, false, volume,
                    audioMixer: musicChannel ? SoundManager.Instance.MusicChannel : null);
                TheOtherRolesPlugin.Logger.LogWarning($"Play sound {path}");
                source.loop = loop;
                return source;
            }
            return null;
        }

        public static void playAtPosition(string path, Vector2 position, float maxDuration = 15f, float range = 5f, bool loop = false)
        {
            if (!TORMapOptions.enableSoundEffects || !Constants.ShouldPlaySfx()) return;

            AudioClip clipToPlay = AssetLoader.GetAudioClip(path);
            if (clipToPlay == null) return;

            AudioSource source = SoundManager.Instance.PlaySound(clipToPlay, false, 1f);
            source.loop = loop;

            HudManager.Instance.StartCoroutine(Effects.Lerp(maxDuration, new Action<float>((p) =>
            {
                if (source != null)
                {
                    if (p == 1)
                    {
                        source.Stop();
                    }

                    float distance = Vector2.Distance(position, PlayerControl.LocalPlayer.GetTruePosition());
                    float volume = distance < range ? (1f - distance / range) : 0f;
                    source.volume = volume;
                }
            })));
        }

        public static void stop(string path)
        {
            var soundToStop = AssetLoader.GetAudioClip(path);
            if (soundToStop != null)
            {
                try
                {
                    SoundManager.Instance?.StopSound(soundToStop);
                }
                catch (Exception e)
                {
                    TheOtherRolesPlugin.Logger.LogWarning($"Exception in stop sound: {e}");
                }
            }
        }

        public static void stopAll()
        {
            if (AssetLoader.AudioClips == null) return;

            try
            {
                foreach (var clip in AssetLoader.AudioClips.Values)
                {
                    SoundManager.Instance?.StopSound(clip);
                }
            }
            catch { }
        }
    }
}
