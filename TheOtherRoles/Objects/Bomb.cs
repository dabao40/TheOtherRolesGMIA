using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;

/*namespace TheOtherRoles.Objects {
    public class Bomb {
        public GameObject bomb;
        public GameObject background;

        private static Sprite bombSprite;
        private static Sprite backgroundSprite;
        private static Sprite defuseSprite;
        public static bool canDefuse = false;

        public static Sprite getBombSprite() {
            if (bombSprite) return bombSprite;
            bombSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Bomb.png", 300f);
            return bombSprite;
        }
        public static Sprite getBackgroundSprite() {
            if (backgroundSprite) return backgroundSprite;
            backgroundSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BombBackground.png", 110f / Bomber.hearRange);
            return backgroundSprite;
        }

        public static Sprite getDefuseSprite() {
            if (defuseSprite) return defuseSprite;
            defuseSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Bomb_Button_Defuse.png", 115f);
            return defuseSprite;
        }

        public Bomb(Vector2 p) {
            bomb = new GameObject("Bomb") { layer = 11 };
            bomb.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            Vector3 position = new Vector3(p.x, p.y, p.y / 1000 + 0.001f); // just behind player
            bomb.transform.position = position;

            background = new GameObject("Background") { layer = 11 };
            background.transform.SetParent(bomb.transform);
            background.transform.localPosition = new Vector3(0, 0, -1f); // before player
            background.transform.position = position;

            var bombRenderer = bomb.AddComponent<SpriteRenderer>();
            bombRenderer.sprite = getBombSprite();
            var backgroundRenderer = background.AddComponent<SpriteRenderer>();
            backgroundRenderer.sprite = getBackgroundSprite();

            bomb.SetActive(false);
            background.SetActive(false);
            if (CachedPlayer.LocalPlayer.PlayerControl == Bomber.bomber) {
                bomb.SetActive(true);
            }
            Bomber.bomb = this;
            Color c = Color.white;
            Color g = Color.red;
            backgroundRenderer.color = Color.white;
            Bomber.isActive = false;

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombActiveAfter, new Action<float>((x) => {
                if (x == 1f && this != null) {
                    bomb.SetActive(true);
                    background.SetActive(true);
                    SoundEffectsManager.playAtPosition("bombFuseBurning", p, Bomber.destructionTime, Bomber.hearRange, true);
                    Bomber.isActive = true;

                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.destructionTime, new Action<float>((x) => { // can you feel the pain?
                        Color combinedColor = Mathf.Clamp01(x) * g + Mathf.Clamp01(1 - x) * c;
                        if (backgroundRenderer) backgroundRenderer.color = combinedColor;
                        if (x == 1f && this != null) {
                            explode(this);
                        }
                    })));
                }
            })));

        }
        public static void explode(Bomb b) {
            if (b == null) return;
            if (Bomber.bomber != null) {
                var position = b.bomb.transform.position;
                var distance = Vector2.Distance(position, CachedPlayer.LocalPlayer.transform.position);  // every player only checks that for their own client (desynct with positions sucks)
                if (distance < Bomber.destructionRange && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead) {
                    Helpers.checkMurderAttemptAndKill(Bomber.bomber, CachedPlayer.LocalPlayer.PlayerControl, false, false, true, true);
                    
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.DeathReasonAndKiller);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)DeadPlayer.CustomDeathReason.Bomb);
                    writer.Write(Bomber.bomber.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    GameHistory.overrideDeathReasonAndKiller(CachedPlayer.LocalPlayer, DeadPlayer.CustomDeathReason.Bomb, killer: Bomber.bomber);
                }
                SoundEffectsManager.playAtPosition("bombExplosion", position, range: Bomber.hearRange) ;
            }
            Bomber.clearBomb();
            canDefuse = false;
            Bomber.isActive = false;
        }

        public static void update() {
            if (Bomber.bomb == null || !Bomber.isActive) {
                canDefuse = false;
                return;
            }
            Bomber.bomb.background.transform.Rotate(Vector3.forward * 50 * Time.fixedDeltaTime);

            if (MeetingHud.Instance && Bomber.bomb != null) {
                Bomber.clearBomb();
            }

            if (Vector2.Distance(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), Bomber.bomb.bomb.transform.position) > 1f) canDefuse = false;
            else canDefuse = true;
        }

        public static void clearBackgroundSprite() {
            backgroundSprite = null;
        }
    }
}*/

namespace TheOtherRoles
{
    class BombEffect
    {
        public static List<BombEffect> bombeffects = new();

        public GameObject bombeffect;
        private GameObject background = null;

        private static Sprite bombeffectSprite;
        public static Sprite getBombEffectSprite()
        {
            if (bombeffectSprite) return bombeffectSprite;
            bombeffectSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BombEffect.png", 300f);
            return bombeffectSprite;
        }

        public BombEffect(PlayerControl player)
        {
            bombeffect = new GameObject("BombEffect");
            Vector3 position = new(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z - 0.001f); // just behind player
            bombeffect.transform.position = position;
            bombeffect.transform.localPosition = position;

            var bombeffectRenderer = bombeffect.AddComponent<SpriteRenderer>();
            bombeffectRenderer.sprite = getBombEffectSprite();
            bombeffect.SetActive(true);
            bombeffects.Add(this);
        }

        public static void clearBombEffects()
        {
            foreach (var bombeffect in bombeffects)
            {
                if (bombeffect != null && bombeffect.bombeffect != null)
                {
                    bombeffect.bombeffect.SetActive(false);
                    UnityEngine.Object.Destroy(bombeffect.bombeffect);

                }
            }
            bombeffects = new List<BombEffect>();
        }

        public static void UpdateAll()
        {
            foreach (BombEffect bombeffect in bombeffects)
            {
                if (bombeffect != null)
                    bombeffect.Update();
            }
        }

        public void Update()
        {
            if (background != null)
                background.transform.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
        }
    }
}