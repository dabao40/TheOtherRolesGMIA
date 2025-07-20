using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class BomberB : RoleBase<BomberB>
    {
        public BomberB()
        {
            RoleId = roleId = RoleId.BomberB;
        }

        public static Color color = Palette.ImpostorRed;

        public static PlayerControl bombTarget;
        public static PlayerControl tmpTarget;
        public static PlayerControl currentTarget;
        public static TMPro.TextMeshPro targetText;
        public static TMPro.TextMeshPro partnerTargetText;
        public static Dictionary<byte, PoolablePlayer> playerIcons = [];
        public static Sprite bomberButtonSprite;
        public static Sprite releaseButtonSprite;
        public static float updateTimer = 0f;
        public static List<Arrow> arrows = [];
        public static float arrowUpdateInterval = 0.5f;

        public static bool isAlive()
        {
            foreach (var bomber in allPlayers)
                if (!(bomber.Data.IsDead || bomber.Data.Disconnected))
                    return true;
            return false;
        }

        public void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            foreach (PoolablePlayer pp in playerIcons.Values) pp.gameObject.SetActive(false);
            if (BomberA.isAlive() && !player.Data.IsDead && !MeetingHud.Instance)
            {
                if (bombTarget != null && TORMapOptions.playerIcons.ContainsKey(bombTarget.PlayerId) && TORMapOptions.playerIcons[bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (targetText == null)
                    {
                        targetText = Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        targetText.enableWordWrapping = false;
                        targetText.transform.localScale = Vector3.one * 1.5f;
                        targetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    targetText.text = ModTranslation.getString("bomberYourTarget");
                    targetText.gameObject.SetActive(true);
                    targetText.transform.SetParent(icon.gameObject.transform);
                }
                // 相方の設置したターゲットを表示する
                if (BomberA.bombTarget != null && playerIcons.ContainsKey(BomberA.bombTarget.PlayerId) && playerIcons[BomberA.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = playerIcons[BomberA.bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(1.0f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (partnerTargetText == null)
                    {
                        partnerTargetText = Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        partnerTargetText.enableWordWrapping = false;
                        partnerTargetText.transform.localScale = Vector3.one * 1.5f;
                        partnerTargetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    partnerTargetText.text = ModTranslation.getString("bomberPartnerTarget");
                    partnerTargetText.gameObject.SetActive(true);
                    partnerTargetText.transform.SetParent(icon.gameObject.transform);
                }
            }
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) {
                foreach (Arrow arrow in arrows)                     if (arrow != null && arrow.arrow != null) {
                        arrow.arrow.SetActive(false);
                        Object.Destroy(arrow.arrow);
                    }
                return;
            }
            if ((BomberA.bombTarget == null || bombTarget == null) && !BomberA.alwaysShowArrow) return;
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                    if (arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        Object.Destroy(arrow.arrow);
                    }

                // Arrows一覧
                arrows = [];
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (p.isRole(RoleId.BomberA))
                    {
                        Arrow arrow;
                        arrow = new Arrow(Color.red);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer == player) {
                playerIconsUpdate();
                arrowUpdate();
                List<PlayerControl> untargetables = [];
                if (tmpTarget != null)
                    untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != tmpTarget.PlayerId).ToList();
                currentTarget = setTarget(untargetablePlayers: untargetables);
                setPlayerOutline(currentTarget, Palette.ImpostorRed);
            }
        }

        public override void OnDeath(PlayerControl killer = null)
        {
            if (BomberA.ifOneDiesBothDie)
                foreach (var partner in BomberA.allPlayers)
                    if (partner != null && !partner.Data.IsDead)
                    {
                        if (killer != null)
                            partner.MurderPlayer(partner, MurderResultFlags.Succeeded);
                        else
                        {
                            partner.Exiled();
                            if (PlayerControl.LocalPlayer == partner && Helpers.ShowKillAnimation)
                                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(partner.Data, partner.Data);
                        }
                        GameHistory.overrideDeathReasonAndKiller(partner, DeadPlayer.CustomDeathReason.Suicide);
                    }
        }

        public static void clearPlayerIcons()
        {
            foreach (PoolablePlayer p in playerIcons.Values)                 if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            TORMapOptions.resetPoolables();
            if (arrows != null)
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            targetText = null;
            partnerTargetText = null;
        }

        public override void ResetRole(bool isShifted)
        {
            clearPlayerIcons();
        }

        public override void OnMeetingStart()
        {
            bombTarget = null;
        }

        public static void clearAndReload()
        {
            clearPlayerIcons();
            tmpTarget = null;
            arrows = [];
            playerIcons = [];
            bombTarget = null;
            currentTarget = null;
            players = [];
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatchForBomber
        {
            public static void Prefix(IntroCutscene __instance)
            {
                if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null)
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                    {
                        NetworkedPlayerInfo data = p.Data;
                        PoolablePlayer player = Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                        p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                        player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                        player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                        player.cosmetics.nameText.text = data.PlayerName;
                        player.SetFlipX(true);
                        player.gameObject.SetActive(false);
                        playerIcons[p.PlayerId] = player;
                    }
            }
        }
    }
}
