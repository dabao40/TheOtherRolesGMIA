using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Patches;
using TheOtherRoles.Roles;
using TheOtherRoles.Utilities;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Objects
{
    public class Trap
    {
        public GameObject trap;
        public static Sprite trapSprite;
        public static Sprite trapActiveSprite;
        public static AudioClip place;
        public static AudioClip activate;
        public static AudioClip disable;
        public static AudioClip countdown;
        public static AudioClip kill;
        public static AudioRolloffMode rollOffMode = AudioRolloffMode.Linear;
        private static byte maxId = 0;
        public AudioSource audioSource;
        public static SortedDictionary<byte, Trap> traps = new();
        public bool isActive = false;
        public PlayerControl target;
        public DateTime placedTime;

        public static void loadSprite()
        {
            if (trapSprite == null)
                trapSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Trap.png", 300f);

            if (trapActiveSprite == null)
                trapActiveSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrapActive.png", 300f);
        }

        private static byte getAvailableId()
        {
            byte ret = maxId;
            maxId++;
            return ret;
        }

        public Trap(Vector3 pos)
        {
            // 最初の罠を消す
            if (traps.Count == Trapper.numTrap)
            {
                foreach (var key in traps.Keys)
                {
                    var firstTrap = traps[key];
                    if (firstTrap.trap != null)
                        UnityEngine.Object.DestroyObject(firstTrap.trap);
                    traps.Remove(key);
                    break;
                }
            }

            // 罠を設置
            this.trap = new GameObject("Trap");
            var trapRenderer = trap.AddComponent<SpriteRenderer>();
            trap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            trapRenderer.sprite = trapSprite;
            Vector3 position = new(pos.x, pos.y, pos.y / 1000 + 0.001f);
            this.trap.transform.position = position;
            this.trap.SetActive(true);

            // 音を鳴らす
            this.audioSource = trap.gameObject.AddComponent<AudioSource>();
            this.audioSource.priority = 0;
            this.audioSource.spatialBlend = 1;
            this.audioSource.clip = place;
            this.audioSource.loop = false;
            this.audioSource.playOnAwake = false;
            this.audioSource.maxDistance = 2 * Trapper.maxDistance / 3;
            this.audioSource.minDistance = Trapper.minDistance;
            this.audioSource.rolloffMode = rollOffMode;
            this.audioSource.PlayOneShot(place);

            // 設置時刻を設定
            this.placedTime = DateTime.UtcNow;

            traps.Add(getAvailableId(), this);
        }

        public static void activateTrap(byte trapId, PlayerControl trapper, PlayerControl target)
        {
            var trap = traps[trapId];

            // 有効にする
            trap.isActive = true;
            trap.target = target;
            var spriteRenderer = trap.trap.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = trapActiveSprite;

            // 他のトラップを全て無効化する
            var newTraps = new SortedDictionary<byte, Trap>
            {
                { trapId, trap }
            };
            foreach (var t in traps.Values)
            {
                if (t.trap == null || t == trap) continue;
                t.trap.SetActive(false);
                UnityEngine.Object.Destroy(t.trap);
            }
            traps = newTraps;

            // 音を鳴らす
            trap.audioSource.Stop();
            trap.audioSource.loop = true;
            trap.audioSource.priority = 0;
            trap.audioSource.spatialBlend = 1;
            trap.audioSource.maxDistance = Trapper.maxDistance;
            trap.audioSource.clip = countdown;
            trap.audioSource.Play();

            // ターゲットを動けなくする
            target.NetTransform.Halt();

            bool moveableFlag = false;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Trapper.killTimer, new Action<float>((p) =>
            {
                try
                {
                    if (Trapper.meetingFlag) return;
                    if (trap == null || trap.trap == null || !trap.isActive) //　解除された場合の処理
                    {
                        target.moveable = true;
                        return;
                    }
                    else if (p == 1f)
                    { // 正常にキルが発生する場合の処理
                        target.moveable = true;
                        if (PlayerControl.LocalPlayer.isRole(RoleId.Trapper))
                        {
                            Trapper.TrapKill.Invoke((trapId, PlayerControl.LocalPlayer.PlayerId, target.PlayerId));
                        }
                    }
                    else
                    { // カウントダウン中の処理
                        if (!moveableFlag)
                        {
                            target.moveable = false;
                            target.NetTransform.Halt();
                            target.transform.position = trap.trap.transform.position + new Vector3(0, 0.3f, 0);
                            moveableFlag = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    TheOtherRolesPlugin.Logger.LogError("An error occured during the countdown");
                    TheOtherRolesPlugin.Logger.LogError(e.Message);
                }
            })));
        }

        public static void disableTrap(byte trapId)
        {
            var trap = traps[trapId];
            trap.isActive = false;
            trap.audioSource.Stop();
            trap.audioSource.PlayOneShot(disable);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(disable.length, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    if (trap.trap != null)
                        trap.trap.SetActive(false);
                    UnityEngine.Object.Destroy(trap.trap);
                    traps.Remove(trapId);
                }
            })));

            if (PlayerControl.LocalPlayer.isRole(RoleId.Trapper))
            {
                PlayerControl.LocalPlayer.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Trapper.penaltyTime;
                HudManagerStartPatch.trapperSetTrapButton.Timer = Trapper.cooldown + Trapper.penaltyTime;
            }
        }

        public static void onMeeting()
        {
            Trapper.meetingFlag = true;
            foreach (var trap in traps)
            {
                trap.Value.audioSource.Stop();
                if (trap.Value.target != null)
                {
                    if (PlayerControl.LocalPlayer.isRole(RoleId.Trapper))
                    {
                        Trapper.TrapKill.Invoke((trap.Key, PlayerControl.LocalPlayer.PlayerId, trap.Value.target.PlayerId));
                    }
                }
            }
        }

        public static bool hasTrappedPlayer()
        {
            foreach (var trap in traps.Values)
            {
                if (trap.target != null) return true;
            }
            return false;
        }

        public static Trap getActiveTrap()
        {
            foreach (var trap in traps.Values)
            {
                if (trap.target != null) return trap;
            }
            return null;
        }

        public static bool isTrapped(PlayerControl p)
        {
            foreach (var trap in traps.Values)
            {
                if (trap.target == p) return true;
            }
            return false;
        }

        public static void trapKill(byte trapId, PlayerControl trapper, PlayerControl target)
        {
            var trap = traps[trapId];
            var audioSource = trap.audioSource;
            audioSource.Stop();
            audioSource.maxDistance = Trapper.maxDistance;
            var murder = Helpers.checkMuderAttempt(trapper, target);
            if (murder == MurderAttemptResult.ReverseKill) target.MurderPlayer(trapper, MurderResultFlags.Succeeded);
            if (murder == MurderAttemptResult.PerformKill)
            {
                audioSource.PlayOneShot(kill);
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(kill.length, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        clearAllTraps();
                    }
                })));
            }
            else
            {
                clearAllTraps();
                return;
            }
            Trapper.isTrapKill = true;
            KillAnimationCoPerformKillPatch.hideNextAnimation = true;
            trapper.MurderPlayer(target, MurderResultFlags.Succeeded);
        }

        public static void clearAllTraps()
        {
            loadSprite();
            foreach (var trap in traps.Values)
            {
                if (trap.trap != null)
                    UnityEngine.GameObject.DestroyObject(trap.trap);
            }
            traps = new SortedDictionary<byte, Trap>();
            maxId = 0;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsTrapPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                foreach (var trap in Trap.traps.Values)
                {
                    bool canSee =
                        trap.isActive ||
                        PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        Lighter.isLightActive(PlayerControl.LocalPlayer) ||
                        PlayerControl.LocalPlayer.isRole(RoleId.Fox);
                    var opacity = canSee ? 1.0f : 0.0f;
                    if (trap.trap != null)
                        trap.trap.GetComponent<SpriteRenderer>().material.color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
                }
            }
        }
    }
}
