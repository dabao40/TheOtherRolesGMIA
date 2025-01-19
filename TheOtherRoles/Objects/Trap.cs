using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Patches;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
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
            // this.trap.transform.localPosition = pos;
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
                    else if ((p == 1f) && !target.Data.IsDead)
                    { // 正常にキルが発生する場合の処理
                        target.moveable = true;
                        if (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrapperKill, Hazel.SendOption.Reliable, -1);
                            writer.Write(trapId);
                            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.trapperKill(trapId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId, target.PlayerId);
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

            if (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper)
            {
                CachedPlayer.LocalPlayer.PlayerControl.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Trapper.penaltyTime;
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

                    if (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper)
                    {
                        if (!trap.Value.target.Data.IsDead)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrapperKill, Hazel.SendOption.Reliable, -1);
                            writer.Write(trap.Key);
                            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                            writer.Write(trap.Value.target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.trapperKill(trap.Key, CachedPlayer.LocalPlayer.PlayerControl.PlayerId, trap.Value.target.PlayerId);
                        }
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
            audioSource.PlayOneShot(kill);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(kill.length, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    clearAllTraps();
                }
            })));
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
                        CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor ||
                        CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || 
                        CachedPlayer.LocalPlayer.PlayerControl == Lighter.lighter ||
                        CachedPlayer.LocalPlayer.PlayerControl == Fox.fox;
                    var opacity = canSee ? 1.0f : 0.0f;
                    if (trap.trap != null)
                        trap.trap.GetComponent<SpriteRenderer>().material.color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
                }
            }
        }
    }
}

/*namespace TheOtherRoles.Objects {
    class Trap {
        public static List<Trap> traps = new List<Trap>();
        public static Dictionary<byte, Trap> trapPlayerIdMap = new Dictionary<byte, Trap>();

        private static int instanceCounter = 0;
        public int instanceId = 0;
        public GameObject trap;
        public bool revealed = false;
        public bool triggerable = false;
        private int usedCount = 0;
        private int neededCount = Trapper.trapCountToReveal;
        public List<PlayerControl> trappedPlayer = new List<PlayerControl>();
        private Arrow arrow = new Arrow(Color.blue);

        private static Sprite trapSprite;
        public static Sprite getTrapSprite() {
            if (trapSprite) return trapSprite;
            trapSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Trapper_Trap_Ingame.png", 300f);
            return trapSprite;
        }

        public Trap(Vector2 p) {
            trap = new GameObject("Trap") { layer = 11 };
            trap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            Vector3 position = new Vector3(p.x, p.y, p.y / 1000 + 0.001f); // just behind player
            trap.transform.position = position;
            neededCount = Trapper.trapCountToReveal;

            var trapRenderer = trap.AddComponent<SpriteRenderer>();
            trapRenderer.sprite = getTrapSprite();
            trap.SetActive(false);
            if (CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) trap.SetActive(true);
            this.instanceId = ++instanceCounter;
            traps.Add(this);
            arrow.Update(position);
            arrow.arrow.SetActive(false);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5, new Action<float>((x) => {
                if (x == 1f) {
                    this.triggerable = true;
                }
            })));
        }

        public static void clearTraps() {
            foreach (Trap t in traps) {
                UnityEngine.Object.Destroy(t.arrow.arrow);
                UnityEngine.Object.Destroy(t.trap); 
            }
            traps = new List<Trap>();
            trapPlayerIdMap = new Dictionary<byte, Trap>();
            instanceCounter = 0;
        }

        public static void clearRevealedTraps() {
            var trapsToClear = traps.FindAll(x => x.revealed);

            foreach (Trap t in trapsToClear) {
                traps.Remove(t);
                UnityEngine.Object.Destroy(t.trap);
            }
        }

        public static void triggerTrap(byte playerId, byte trapId) {            
            Trap t = traps.FirstOrDefault(x => x.instanceId == (int)trapId);
            PlayerControl player = Helpers.playerById(playerId);
            if (Trapper.trapper == null || t == null || player == null) return;
            bool localIsTrapper = CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId;
            if (!trapPlayerIdMap.ContainsKey(playerId)) trapPlayerIdMap.Add(playerId, t);
            t.usedCount ++;
            t.triggerable = false;
            if (playerId == CachedPlayer.LocalPlayer.PlayerId || playerId == Trapper.trapper.PlayerId) {
                t.trap.SetActive(true);
                SoundEffectsManager.play("trapperTrap");
            }
            player.moveable = false;
            player.NetTransform.Halt();
            Trapper.playersOnMap.Add(player); 
            if (localIsTrapper) t.arrow.arrow.SetActive(true);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Trapper.trapDuration, new Action<float>((p) => { 
                if (p == 1f) {
                    player.moveable = true;
                    Trapper.playersOnMap.RemoveAll(x => x == player);
                    if (trapPlayerIdMap.ContainsKey(playerId)) trapPlayerIdMap.Remove(playerId);
                    t.arrow.arrow.SetActive(false);
                }
            })));

            if (t.usedCount == t.neededCount) {
                t.revealed = true;
            }

            t.trappedPlayer.Add(player);
            t.triggerable = true;

        }

        public static void Update() {
            if (Trapper.trapper == null) return;
            CachedPlayer player = CachedPlayer.LocalPlayer;
            Vent vent = MapUtilities.CachedShipStatus.AllVents[0];
            float closestDistance = float.MaxValue;

            if (vent == null || player == null) return;
            float ud = vent.UsableDistance / 2;
            Trap target = null;
            foreach (Trap trap in traps) {
                if (trap.arrow.arrow.active) trap.arrow.Update();
                if (trap.revealed || !trap.triggerable || trap.trappedPlayer.Contains(player.PlayerControl)) continue;
                if (player.PlayerControl.inVent || !player.PlayerControl.CanMove) continue;
                float distance = Vector2.Distance(trap.trap.transform.position, player.PlayerControl.GetTruePosition());
                if (distance <= ud && distance < closestDistance) {
                    closestDistance = distance;
                    target = trap;
                }
            }
            if (target != null && player.PlayerId != Trapper.trapper.PlayerId && !player.Data.IsDead) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TriggerTrap, Hazel.SendOption.Reliable, -1);
                writer.Write(player.PlayerId);
                writer.Write(target.instanceId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.triggerTrap(player.PlayerId,(byte)target.instanceId);
            }


            if (!player.Data.IsDead || player.PlayerId == Trapper.trapper.PlayerId) return;
            foreach (Trap trap in traps) {
                if (!trap.trap.active) trap.trap.SetActive(true);
            }
        }
    }
}*/
