using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Hazel;
using PowerTools;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.MetaContext;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class IntroCutsceneOnDestroyPatch
    {
        public static PoolablePlayer playerPrefab;
        public static Vector3 bottomLeft;
        public static void Prefix(IntroCutscene __instance) {
            // Generate and initialize player icons
            int playerCounter = 0;
            int hideNSeekCounter = 0;
            if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null) {
                float aspect = Camera.main.aspect;
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                float xpos = 1.75f - safeOrthographicSize * aspect * 1.70f;
                float ypos = 0.15f - safeOrthographicSize * 1.7f;
                bottomLeft = new Vector3(xpos / 2, ypos/2, -61f);

                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    NetworkedPlayerInfo data = p.Data;
                    PoolablePlayer player = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                    playerPrefab = __instance.PlayerPrefab;
                    p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                    player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                    player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                   // PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, player.PetSlot);
                    player.cosmetics.nameText.text = data.PlayerName;
                    player.SetFlipX(true);
                    TORMapOptions.playerIcons[p.PlayerId] = player;

                    player.gameObject.SetActive(false);

                    if (PlayerControl.LocalPlayer == Arsonist.arsonist && p != Arsonist.arsonist) {
                        player.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) + Vector3.right * playerCounter++ * 0.35f;
                        player.transform.localScale = Vector3.one * 0.2f;
                        player.setSemiTransparent(true);
                        player.gameObject.SetActive(true);
                    } else if (HideNSeek.isHideNSeekGM) {
                        if (HideNSeek.isHunted() && p.Data.Role.IsImpostor) {
                            player.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0.4f, 0) + Vector3.right * playerCounter++ * 0.6f;
                            player.transform.localScale = Vector3.one * 0.3f;
                            player.cosmetics.nameText.text += $"{Helpers.cs(Color.red, $" ({ModTranslation.getString("hunter")})")}";
                            player.gameObject.SetActive(true);
                        } else if (!p.Data.Role.IsImpostor) {
                            player.transform.localPosition = bottomLeft + new Vector3(-0.35f, -0.25f, 0) + Vector3.right * hideNSeekCounter++ * 0.35f;
                            player.transform.localScale = Vector3.one * 0.2f;
                            player.setSemiTransparent(true);
                            player.gameObject.SetActive(true);
                        }

                    } else {   //  This can be done for all players not just for the bounty hunter as it was before. Allows the thief to have the correct position and scaling
                        player.transform.localPosition = bottomLeft;
                        player.transform.localScale = Vector3.one * 0.4f;
                        player.gameObject.SetActive(false);
                    }
                }
            }

            // Force Bounty Hunter to load a new Bounty when the Intro is over
            if (BountyHunter.bounty != null) {
                if (PlayerControl.LocalPlayer == BountyHunter.bountyHunter) BountyHunter.bountyUpdateTimer = 0f;
                if (FastDestroyableSingleton<HudManager>.Instance != null) {
                    BountyHunter.cooldownText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                    BountyHunter.cooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                    BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -62f);
                    BountyHunter.cooldownText.transform.localScale = Vector3.one * 0.4f;
                    BountyHunter.cooldownText.gameObject.SetActive(PlayerControl.LocalPlayer == BountyHunter.bountyHunter);
                }
            }

            // Place props
            if (CustomOptionHolder.activateProps.getBool() && !FreePlayGM.isFreePlayGM)
            {
                Props.placeProps();
            }

            if (AmongUsClient.Instance.AmHost)
            {
                MessageWriter roleWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRoleHistory, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(roleWriter);
                RPCProcedure.setRoleHistory();
                GameStatistics.Event.GameStatistics.RpcRecordEvent(GameStatistics.EventVariation.GameStart, EventDetail.GameStart, null, 0);
            }

            if (CustomOptionHolder.randomGameStartPosition.getBool())
            { //Random spawn on game start

                List<Vector3> skeldSpawn = new() {
                new Vector3(-2.2f, 2.2f, 0.0f), //cafeteria. botton. top left.
                new Vector3(0.7f, 2.2f, 0.0f), //caffeteria. button. top right.
                new Vector3(-2.2f, -0.2f, 0.0f), //caffeteria. button. bottom left.
                new Vector3(0.7f, -0.2f, 0.0f), //caffeteria. button. bottom right.
                new Vector3(10.0f, 3.0f, 0.0f), //weapons top
                new Vector3(9.0f, 1.0f, 0.0f), //weapons bottom
                new Vector3(6.5f, -3.5f, 0.0f), //O2
                new Vector3(11.5f, -3.5f, 0.0f), //O2-nav hall
                new Vector3(17.0f, -3.5f, 0.0f), //navigation top
                new Vector3(18.2f, -5.7f, 0.0f), //navigation bottom
                new Vector3(11.5f, -6.5f, 0.0f), //nav-shields top
                new Vector3(9.5f, -8.5f, 0.0f), //nav-shields bottom
                new Vector3(9.2f, -12.2f, 0.0f), //shields top
                new Vector3(8.0f, -14.3f, 0.0f), //shields bottom
                new Vector3(2.5f, -16f, 0.0f), //coms left
                new Vector3(4.2f, -16.4f, 0.0f), //coms middle
                new Vector3(5.5f, -16f, 0.0f), //coms right
                new Vector3(-1.5f, -10.0f, 0.0f), //storage top
                new Vector3(-1.5f, -15.5f, 0.0f), //storage bottom
                new Vector3(-4.5f, -12.5f, 0.0f), //storrage left
                new Vector3(0.3f, -12.5f, 0.0f), //storrage right
                new Vector3(4.5f, -7.5f, 0.0f), //admin top
                new Vector3(4.5f, -9.5f, 0.0f), //admin bottom
                new Vector3(-9.0f, -8.0f, 0.0f), //elec top left
                new Vector3(-6.0f, -8.0f, 0.0f), //elec top right
                new Vector3(-8.0f, -11.0f, 0.0f), //elec bottom
                new Vector3(-12.0f, -13.0f, 0.0f), //elec-lower hall
                new Vector3(-17f, -10f, 0.0f), //lower engine top
                new Vector3(-17.0f, -13.0f, 0.0f), //lower engine bottom
                new Vector3(-21.5f, -3.0f, 0.0f), //reactor top
                new Vector3(-21.5f, -8.0f, 0.0f), //reactor bottom
                new Vector3(-13.0f, -3.0f, 0.0f), //security top
                new Vector3(-12.6f, -5.6f, 0.0f), // security bottom
                new Vector3(-17.0f, 2.5f, 0.0f), //upper engibe top
                new Vector3(-17.0f, -1.0f, 0.0f), //upper engine bottom
                new Vector3(-10.5f, 1.0f, 0.0f), //upper-mad hall
                new Vector3(-10.5f, -2.0f, 0.0f), //medbay top
                new Vector3(-6.5f, -4.5f, 0.0f) //medbay bottom
                };

                List<Vector3> miraSpawn = new() {
                new Vector3(-4.5f, 3.5f, 0.0f), //launchpad top
                new Vector3(-4.5f, -1.4f, 0.0f), //launchpad bottom
                new Vector3(8.5f, -1f, 0.0f), //launchpad- med hall
                new Vector3(14f, -1.5f, 0.0f), //medbay
                new Vector3(16.5f, 3f, 0.0f), // comms
                new Vector3(10f, 5f, 0.0f), //lockers
                new Vector3(6f, 1.5f, 0.0f), //locker room
                new Vector3(2.5f, 13.6f, 0.0f), //reactor
                new Vector3(6f, 12f, 0.0f), //reactor middle
                new Vector3(9.5f, 13f, 0.0f), //lab
                new Vector3(15f, 9f, 0.0f), //bottom left cross
                new Vector3(17.9f, 11.5f, 0.0f), //middle cross
                new Vector3(14f, 17.3f, 0.0f), //office
                new Vector3(19.5f, 21f, 0.0f), //admin
                new Vector3(14f, 24f, 0.0f), //greenhouse left
                new Vector3(22f, 24f, 0.0f), //greenhouse right
                new Vector3(21f, 8.5f, 0.0f), //bottom right cross
                new Vector3(28f, 3f, 0.0f), //caf right
                new Vector3(22f, 3f, 0.0f), //caf left
                new Vector3(19f, 4f, 0.0f), //storage
                new Vector3(22f, -2f, 0.0f), //balcony
                };

                List<Vector3> polusSpawn = new() {
                new Vector3(16.6f, -1f, 0.0f), //dropship top
                new Vector3(16.6f, -5f, 0.0f), //dropship bottom
                new Vector3(20f, -9f, 0.0f), //above storrage
                new Vector3(22f, -7f, 0.0f), //right fuel
                new Vector3(25.5f, -6.9f, 0.0f), //drill
                new Vector3(29f, -9.5f, 0.0f), //lab lockers
                new Vector3(29.5f, -8f, 0.0f), //lab weather notes
                new Vector3(35f, -7.6f, 0.0f), //lab table
                new Vector3(40.4f, -8f, 0.0f), //lab scan
                new Vector3(33f, -10f, 0.0f), //lab toilet
                new Vector3(39f, -15f, 0.0f), //specimen hall top
                new Vector3(36.5f, -19.5f, 0.0f), //specimen top
                new Vector3(36.5f, -21f, 0.0f), //specimen bottom
                new Vector3(28f, -21f, 0.0f), //specimen hall bottom
                new Vector3(24f, -20.5f, 0.0f), //admin tv
                new Vector3(22f, -25f, 0.0f), //admin books
                new Vector3(16.6f, -17.5f, 0.0f), //office coffe
                new Vector3(22.5f, -16.5f, 0.0f), //office projector
                new Vector3(24f, -17f, 0.0f), //office figure
                new Vector3(27f, -16.5f, 0.0f), //office lifelines
                new Vector3(32.7f, -15.7f, 0.0f), //lavapool
                new Vector3(31.5f, -12f, 0.0f), //snowmad below lab
                new Vector3(10f, -14f, 0.0f), //below storrage
                new Vector3(21.5f, -12.5f, 0.0f), //storrage vent
                new Vector3(19f, -11f, 0.0f), //storrage toolrack
                new Vector3(12f, -7f, 0.0f), //left fuel
                new Vector3(5f, -7.5f, 0.0f), //above elec
                new Vector3(10f, -12f, 0.0f), //elec fence
                new Vector3(9f, -9f, 0.0f), //elec lockers
                new Vector3(5f, -9f, 0.0f), //elec window
                new Vector3(4f, -11.2f, 0.0f), //elec tapes
                new Vector3(5.5f, -16f, 0.0f), //elec-O2 hall
                new Vector3(1f, -17.5f, 0.0f), //O2 tree hayball
                new Vector3(3f, -21f, 0.0f), //O2 middle
                new Vector3(2f, -19f, 0.0f), //O2 gas
                new Vector3(1f, -24f, 0.0f), //O2 water
                new Vector3(7f, -24f, 0.0f), //under O2
                new Vector3(9f, -20f, 0.0f), //right outside of O2
                new Vector3(7f, -15.8f, 0.0f), //snowman under elec
                new Vector3(11f, -17f, 0.0f), //comms table
                new Vector3(12.7f, -15.5f, 0.0f), //coms antenna pult
                new Vector3(13f, -24.5f, 0.0f), //weapons window
                new Vector3(15f, -17f, 0.0f), //between coms-office
                new Vector3(17.5f, -25.7f, 0.0f), //snowman under office
                };

                List<Vector3> dleksSpawn = new() {
                new Vector3(2.2f, 2.2f, 0.0f), //cafeteria. botton. top left.
                new Vector3(-0.7f, 2.2f, 0.0f), //caffeteria. button. top right.
                new Vector3(2.2f, -0.2f, 0.0f), //caffeteria. button. bottom left.
                new Vector3(-0.7f, -0.2f, 0.0f), //caffeteria. button. bottom right.
                new Vector3(-10.0f, 3.0f, 0.0f), //weapons top
                new Vector3(-9.0f, 1.0f, 0.0f), //weapons bottom
                new Vector3(-6.5f, -3.5f, 0.0f), //O2
                new Vector3(-11.5f, -3.5f, 0.0f), //O2-nav hall
                new Vector3(-17.0f, -3.5f, 0.0f), //navigation top
                new Vector3(-18.2f, -5.7f, 0.0f), //navigation bottom
                new Vector3(-11.5f, -6.5f, 0.0f), //nav-shields top
                new Vector3(-9.5f, -8.5f, 0.0f), //nav-shields bottom
                new Vector3(-9.2f, -12.2f, 0.0f), //shields top
                new Vector3(-8.0f, -14.3f, 0.0f), //shields bottom
                new Vector3(-2.5f, -16f, 0.0f), //coms left
                new Vector3(-4.2f, -16.4f, 0.0f), //coms middle
                new Vector3(-5.5f, -16f, 0.0f), //coms right
                new Vector3(1.5f, -10.0f, 0.0f), //storage top
                new Vector3(1.5f, -15.5f, 0.0f), //storage bottom
                new Vector3(4.5f, -12.5f, 0.0f), //storrage left
                new Vector3(-0.3f, -12.5f, 0.0f), //storrage right
                new Vector3(-4.5f, -7.5f, 0.0f), //admin top
                new Vector3(-4.5f, -9.5f, 0.0f), //admin bottom
                new Vector3(9.0f, -8.0f, 0.0f), //elec top left
                new Vector3(6.0f, -8.0f, 0.0f), //elec top right
                new Vector3(8.0f, -11.0f, 0.0f), //elec bottom
                new Vector3(12.0f, -13.0f, 0.0f), //elec-lower hall
                new Vector3(17f, -10f, 0.0f), //lower engine top
                new Vector3(17.0f, -13.0f, 0.0f), //lower engine bottom
                new Vector3(21.5f, -3.0f, 0.0f), //reactor top
                new Vector3(21.5f, -8.0f, 0.0f), //reactor bottom
                new Vector3(13.0f, -3.0f, 0.0f), //security top
                new Vector3(12.6f, -5.6f, 0.0f), // security bottom
                new Vector3(17.0f, 2.5f, 0.0f), //upper engibe top
                new Vector3(17.0f, -1.0f, 0.0f), //upper engine bottom
                new Vector3(10.5f, 1.0f, 0.0f), //upper-mad hall
                new Vector3(10.5f, -2.0f, 0.0f), //medbay top
                new Vector3(6.5f, -4.5f, 0.0f) //medbay bottom
                };

                List<Vector3> fungleSpawn = new() {
                new Vector3(-10.0842f, 13.0026f, 0.013f),
                new Vector3(0.9815f, 6.7968f, 0.0068f),
                new Vector3(22.5621f, 3.2779f, 0.0033f),
                new Vector3(-1.8699f, -1.3406f, -0.0013f),
                new Vector3(12.0036f, 2.6763f, 0.0027f),
                new Vector3(21.705f, -7.8691f, -0.0079f),
                new Vector3(1.4485f, -1.6105f, -0.0016f),
                new Vector3(-4.0766f, -8.7178f, -0.0087f),
                new Vector3(2.9486f, 1.1347f, 0.0011f),
                new Vector3(-4.2181f, -8.6795f, -0.0087f),
                new Vector3(19.5553f, -12.5014f, -0.0125f),
                new Vector3(15.2497f, -16.5009f, -0.0165f),
                new Vector3(-22.7174f, -7.0523f, 0.0071f),
                new Vector3(-16.5819f, -2.1575f, 0.0022f),
                new Vector3(9.399f, -9.7127f, -0.0097f),
                new Vector3(7.3723f, 1.7373f, 0.0017f), 
                new Vector3(22.0777f, -7.9315f, -0.0079f),
                new Vector3(-15.3916f, -9.3659f, -0.0094f),
                new Vector3(-16.1207f, -0.1746f, -0.0002f),
                new Vector3(-23.1353f, -7.2472f, -0.0072f),
                new Vector3(-20.0692f, -2.6245f, -0.0026f),
                new Vector3(-4.2181f, -8.6795f, -0.0087f),
                new Vector3(-9.9285f, 12.9848f, 0.013f),
                new Vector3(-8.3475f, 1.6215f, 0.0016f),
                new Vector3(-17.7614f, 6.9115f, 0.0069f),
                new Vector3(-0.5743f, -4.7235f, -0.0047f),
                new Vector3(-20.8897f, 2.7606f, 0.002f)
                };

                if (Helpers.isSkeld()) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(skeldSpawn[rnd.Next(skeldSpawn.Count)]);
                if (Helpers.isMira()) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(miraSpawn[rnd.Next(miraSpawn.Count)]);
                if (Helpers.isPolus()) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(polusSpawn[rnd.Next(polusSpawn.Count)]);
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(dleksSpawn[rnd.Next(dleksSpawn.Count)]);
                if (Helpers.isFungle()) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(fungleSpawn[rnd.Next(fungleSpawn.Count)]);

            }

            // First kill
            if (AmongUsClient.Instance.AmHost && TORMapOptions.shieldFirstKill && TORMapOptions.firstKillName != "" && !HideNSeek.isHideNSeekGM) {
                PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(TORMapOptions.firstKillName));
                if (target != null) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFirstKill, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFirstKill(target.PlayerId);
                }
            }
            TORMapOptions.firstKillName = "";

            if (Helpers.isAirship() && CustomOptionHolder.airshipOptimize.getBool() && Helpers.hasImpVision(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId)))
            {
                var obj = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].gameObject;
                OneWayShadows oneWayShadow = obj.transform.FindChild("Shadow").FindChild("LedgeShadow").GetComponent<OneWayShadows>();
                oneWayShadow.gameObject.SetActive(false);
            }

            HudManager.Instance.ShowVanillaKeyGuide();
            GameStatistics.MinimapPrefab = ShipStatus.Instance.MapPrefab;
            GameStatistics.MapScale = ShipStatus.Instance.MapScale;

            if (AmongUsClient.Instance.AmHost) {
                LastImpostor.promoteToLastImpostor();
            }

            if (Kataomoi.kataomoi != null)
            {
                if (FastDestroyableSingleton<HudManager>.Instance != null)
                {
                    Kataomoi.stareText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                    Kataomoi.stareText.alignment = TMPro.TextAlignmentOptions.Center;
                    Kataomoi.stareText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -62f);
                    Kataomoi.stareText.transform.localScale = Vector3.one * 0.5f;
                    Kataomoi.stareText.gameObject.SetActive(PlayerControl.LocalPlayer == Kataomoi.kataomoi && Kataomoi.target != null);

                    Kataomoi.gaugeRenderer[0] = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.graphic, FastDestroyableSingleton<HudManager>.Instance.transform);
                    var killButton = Kataomoi.gaugeRenderer[0].GetComponent<KillButton>();
                    killButton.SetCoolDown(0.00000001f, 0.00000001f);
                    killButton.SetFillUp(0.00000001f, 0.00000001f);
                    killButton.SetDisabled();
                    Helpers.hideGameObjects(Kataomoi.gaugeRenderer[0].gameObject);
                    var components = killButton.GetComponents<Component>();
                    foreach (var c in components)
                    {
                        if ((c as KillButton) == null && (c as SpriteRenderer) == null)
                            GameObject.Destroy(c.gameObject);
                    }

                    Kataomoi.gaugeRenderer[0].sprite = Kataomoi.getLoveGaugeSprite(0);
                    Kataomoi.gaugeRenderer[0].color = new Color32(175, 175, 176, 255);
                    Kataomoi.gaugeRenderer[0].size = new Vector2(300f, 64f);
                    Kataomoi.gaugeRenderer[0].gameObject.SetActive(true);
                    Kataomoi.gaugeRenderer[0].transform.localPosition = new Vector3(-3.354069f + 1f, -2.429999f, -8f);
                    Kataomoi.gaugeRenderer[0].transform.localScale = Vector3.one;

                    Kataomoi.gaugeRenderer[1] = UnityEngine.Object.Instantiate(Kataomoi.gaugeRenderer[0], FastDestroyableSingleton<HudManager>.Instance.transform);
                    Kataomoi.gaugeRenderer[1].sprite = Kataomoi.getLoveGaugeSprite(1);
                    Kataomoi.gaugeRenderer[1].size = new Vector2(261f, 7f);
                    Kataomoi.gaugeRenderer[1].color = Kataomoi.color;
                    Kataomoi.gaugeRenderer[1].transform.localPosition = new Vector3(-3.482069f + 1f, -2.626999f, -8.1f);
                    Kataomoi.gaugeRenderer[1].transform.localScale = Vector3.one;

                    Kataomoi.gaugeRenderer[2] = UnityEngine.Object.Instantiate(Kataomoi.gaugeRenderer[0], FastDestroyableSingleton<HudManager>.Instance.transform);
                    Kataomoi.gaugeRenderer[2].sprite = Kataomoi.getLoveGaugeSprite(2);
                    Kataomoi.gaugeRenderer[2].color = Kataomoi.gaugeRenderer[0].color;
                    Kataomoi.gaugeRenderer[2].size = new Vector2(300f, 64f);
                    Kataomoi.gaugeRenderer[2].transform.localPosition = new Vector3(-3.354069f + 1f, -2.429999f, -8.2f);
                    Kataomoi.gaugeRenderer[2].transform.localScale = Vector3.one;

                    Kataomoi.gaugeTimer = 1.0f;

                    for (int i = 0; i < 3; i++) if (Kataomoi.gaugeRenderer[i] != null) Kataomoi.gaugeRenderer[i].gameObject.SetActive(false);
                }
            }

            SchrodingersCat.playerTemplate = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
            SchrodingersCat.playerTemplate.UpdateFromPlayerOutfit(PlayerControl.LocalPlayer.Data.DefaultOutfit, PlayerMaterial.MaskType.ComplexUI, false, true);
            SchrodingersCat.playerTemplate.SetFlipX(true);
            SchrodingersCat.playerTemplate.gameObject.SetActive(false);
            SchrodingersCat.playerTemplate.cosmetics.currentPet?.gameObject.SetActive(false);
            SchrodingersCat.playerTemplate.cosmetics.nameText.text = "";
            SchrodingersCat.playerTemplate.gameObject.SetActive(false);

            if (AmongUsClient.Instance.AmHost && (Archaeologist.archaeologist != null || FreePlayGM.isFreePlayGM))
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceAntique, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.placeAntique();
            }

            // Additional Vents
            AdditionalVents.AddAdditionalVents();

            // Specimen Vitals
            SpecimenVital.moveVital();

            // Add Electrical
            FungleAdditionalElectrical.CreateElectrical();

            if (CustomOptionHolder.foxSpawnRate.getSelection() > 0 && (Shrine.allShrine == null || Shrine.allShrine.Count == 0))
            {
                Shrine.activateShrines(GameOptionsManager.Instance.currentNormalGameOptions.MapId);
                List<Byte> taskIdList = new();
                Shrine.allShrine.ForEach(shrine => taskIdList.Add((byte)shrine.console.ConsoleId));
                taskIdList.Shuffle();
                var cpt = new CustomNormalPlayerTask("foxTaskStay", Il2CppType.Of<FoxTask>(), Fox.numTasks, taskIdList.ToArray(), Shrine.allShrine.Find(x => x.console.ConsoleId == taskIdList.ToArray()[0]).console.Room, true);
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p == Fox.fox)
                    {
                        p.clearAllTasks();
                        cpt.addTaskToPlayer(p.PlayerId);
                    }
                }
            }

            EventUtility.gameStartsUpdate();

            if (HideNSeek.isHideNSeekGM) {
                foreach (PlayerControl player in HideNSeek.getHunters()) {
                    player.moveable = false;
                    player.NetTransform.Halt();
                    HideNSeek.timer = HideNSeek.hunterWaitingTime;
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(HideNSeek.hunterWaitingTime, new Action<float>((p) => {
                        if (p == 1f) {
                            player.moveable = true;
                            HideNSeek.timer = CustomOptionHolder.hideNSeekTimer.getFloat() * 60;
                            HideNSeek.isWaitingTimer = false;
                        }
                    })));
                    player.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                }

                if (HideNSeek.polusVent == null && GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2) {
                    var list = GameObject.FindObjectsOfType<Vent>().ToList();
                    var adminVent = list.FirstOrDefault(x => x.gameObject.name == "AdminVent");
                    var bathroomVent = list.FirstOrDefault(x => x.gameObject.name == "BathroomVent");
                    HideNSeek.polusVent = UnityEngine.Object.Instantiate<Vent>(adminVent);
                    HideNSeek.polusVent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    HideNSeek.polusVent.transform.position = new Vector3(36.55068f, -21.5168f, -0.0215168f);
                    HideNSeek.polusVent.Left = adminVent;
                    HideNSeek.polusVent.Right = bathroomVent;
                    HideNSeek.polusVent.Center = null;
                    HideNSeek.polusVent.Id = MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
                    var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
                    allVentsList.Add(HideNSeek.polusVent);
                    MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
                    HideNSeek.polusVent.gameObject.SetActive(true);
                    HideNSeek.polusVent.name = "newVent_" + HideNSeek.polusVent.Id;

                    adminVent.Center = HideNSeek.polusVent;
                    bathroomVent.Center = HideNSeek.polusVent;
                }

                ShipStatusPatch.originalNumCrewVisionOption = GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                ShipStatusPatch.originalNumImpVisionOption = GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                ShipStatusPatch.originalNumKillCooldownOption = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;

                GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod = CustomOptionHolder.hideNSeekHunterVision.getFloat();
                GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = CustomOptionHolder.hideNSeekHuntedVision.getFloat();
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown = CustomOptionHolder.hideNSeekKillCooldown.getFloat();
            }
        }
    }

    [HarmonyPatch]
    class IntroPatch {
        public static IEnumerator ShowTeam(IntroCutscene __instance, Il2CppSystem.Collections.Generic.List<PlayerControl> teamToShow, float duration)
        {
            if (__instance.overlayHandle == null)
            {
                __instance.overlayHandle = DestroyableSingleton<DualshockLightManager>.Instance.AllocateLight();
            }
            yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
            if (!teamToShow.TrueForAll((Il2CppSystem.Predicate<PlayerControl>)(p => p.Data.Role.IsImpostor)))
            {
                __instance.BeginCrewmate(teamToShow);
                __instance.overlayHandle.color = Palette.CrewmateBlue;
            }
            else
            {
                __instance.BeginImpostor(teamToShow);
                __instance.overlayHandle.color = Palette.ImpostorRed;
            }
            Color c = __instance.TeamTitle.color;
            Color fade = Color.black;
            Color impColor = Color.white;
            Vector3 titlePos = __instance.TeamTitle.transform.localPosition;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float num = Mathf.Min(1f, timer / duration);
                __instance.Foreground.material.SetFloat("_Rad", __instance.ForegroundRadius.ExpOutLerp(num * 2f));
                fade.a = Mathf.Lerp(1f, 0f, num * 3f);
                __instance.FrontMost.color = fade;
                c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
                __instance.TeamTitle.color = c;
                __instance.RoleText.color = c;
                impColor.a = Mathf.Lerp(0f, 1f, (num - 0.3f) * 3f);
                __instance.ImpostorText.color = impColor;
                titlePos.y = 2.7f - num * 0.3f;
                __instance.TeamTitle.transform.localPosition = titlePos;
                __instance.overlayHandle.color.SetAlpha(Mathf.Min(1f, timer * 2f));
                yield return null;
            }
            timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float num2 = timer / 1f;
                fade.a = Mathf.Lerp(0f, 1f, num2 * 3f);
                __instance.FrontMost.color = fade;
                __instance.overlayHandle.color.SetAlpha(1f - fade.a);
                yield return null;
            }
            yield break;
        }

        public static IEnumerator CoBegin(IntroCutscene __instance)
        {
            Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Starting intro cutscene", null);
            SoundManager.Instance.PlaySound(__instance.IntroStinger, false, 1f, null);
            if (GameManager.Instance.IsNormal())
            {
                Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Normal", null);
                __instance.LogPlayerRoleData();
                __instance.HideAndSeekPanels.SetActive(false);
                __instance.CrewmateRules.SetActive(false);
                __instance.ImpostorRules.SetActive(false);
                __instance.ImpostorName.gameObject.SetActive(false);
                __instance.ImpostorTitle.gameObject.SetActive(false);
                var list = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                list =
                    IntroCutscene.SelectTeamToShow(
                        (Func<NetworkedPlayerInfo, bool>)(pcd =>
                            !PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                            pcd.Role.TeamType == PlayerControl.LocalPlayer.Data.Role.TeamType
                        )
                    );
                if (list == null || list.Count < 1)
                {
                    Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
                }
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    __instance.ImpostorText.gameObject.SetActive(false);
                }
                else
                {
                    int adjustedNumImpostors = GameManager.Instance.LogicOptions.GetAdjustedNumImpostors(GameData.Instance.PlayerCount);
                    if (adjustedNumImpostors == 1)
                    {
                        __instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsS, new UnityEngine.Object());
                    }
                    else
                    {
                        __instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, (Il2CppReferenceArray<Il2CppSystem.Object>)(new object[] { adjustedNumImpostors }));
                    }
                    __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
                    __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");
                }
                yield return ShowTeam(__instance, list, 3f);
                yield return RoleDraft.CoSelectRoles(__instance).WrapToIl2Cpp();
                yield return __instance.ShowRole();
            }
            else
            {
                Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Hide and Seek", null);
                __instance.LogPlayerRoleData();
                __instance.HideAndSeekPanels.SetActive(true);
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    __instance.CrewmateRules.SetActive(false);
                    __instance.ImpostorRules.SetActive(true);
                }
                else
                {
                    __instance.CrewmateRules.SetActive(true);
                    __instance.ImpostorRules.SetActive(false);
                }
                Il2CppSystem.Collections.Generic.List<PlayerControl> list2 = IntroCutscene.SelectTeamToShow(
                    (Func<NetworkedPlayerInfo, bool>)(pcd => PlayerControl.LocalPlayer.Data.Role.IsImpostor != pcd.Role.IsImpostor)
                ); if (list2 == null || list2.Count < 1)
                {
                    Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
                }
                PlayerControl impostor = PlayerControl.AllPlayerControls.Find(
                    (Il2CppSystem.Predicate<PlayerControl>)(pc => pc.Data.Role.IsImpostor)
                );
                if (impostor == null)
                {
                    Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: impostor is NULL", null);
                }
                GameManager.Instance.SetSpecialCosmetics(impostor);
                __instance.ImpostorName.gameObject.SetActive(true);
                __instance.ImpostorTitle.gameObject.SetActive(true);
                __instance.BackgroundBar.enabled = false;
                __instance.TeamTitle.gameObject.SetActive(false);
                if (impostor != null)
                {
                    __instance.ImpostorName.text = impostor.Data.PlayerName;
                }
                else
                {
                    __instance.ImpostorName.text = "???";
                }
                yield return new WaitForSecondsRealtime(0.1f);
                PoolablePlayer playerSlot = null;
                if (impostor != null)
                {
                    playerSlot = __instance.CreatePlayer(1, 1, impostor.Data, false);
                    playerSlot.SetBodyType(PlayerBodyTypes.Normal);
                    playerSlot.SetFlipX(false);
                    playerSlot.transform.localPosition = __instance.impostorPos;
                    playerSlot.transform.localScale = Vector3.one * __instance.impostorScale;
                }
                yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
                yield return new WaitForSecondsRealtime(6f);
                if (playerSlot != null)
                {
                    playerSlot.gameObject.SetActive(false);
                }
                __instance.HideAndSeekPanels.SetActive(false);
                __instance.CrewmateRules.SetActive(false);
                __instance.ImpostorRules.SetActive(false);
                LogicOptionsHnS logicOptionsHnS = GameManager.Instance.LogicOptions as LogicOptionsHnS;
                LogicHnSMusic logicHnSMusic = GameManager.Instance.GetLogicComponent<LogicHnSMusic>() as LogicHnSMusic;
                if (logicHnSMusic != null)
                {
                    logicHnSMusic.StartMusicWithIntro();
                }
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    float crewmateLeadTime = (float)logicOptionsHnS.GetCrewmateLeadTime();
                    __instance.HideAndSeekTimerText.gameObject.SetActive(true);
                    PoolablePlayer poolablePlayer;
                    AnimationClip animationClip;
                    if (AprilFoolsMode.ShouldHorseAround())
                    {
                        poolablePlayer = __instance.HorseWrangleVisualSuit;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                        animationClip = __instance.HnSSeekerSpawnHorseAnim;
                        __instance.HorseWrangleVisualPlayer.SetBodyType(PlayerBodyTypes.Normal);
                        __instance.HorseWrangleVisualPlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
                    }
                    else if (AprilFoolsMode.ShouldLongAround())
                    {
                        poolablePlayer = __instance.HideAndSeekPlayerVisual;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.LongSeeker);
                        animationClip = __instance.HnSSeekerSpawnLongAnim;
                    }
                    else
                    {
                        poolablePlayer = __instance.HideAndSeekPlayerVisual;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                        animationClip = __instance.HnSSeekerSpawnAnim;
                    }
                    poolablePlayer.SetBodyCosmeticsVisible(false);
                    poolablePlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
                    SpriteAnim component = poolablePlayer.GetComponent<SpriteAnim>();
                    poolablePlayer.gameObject.SetActive(true);
                    poolablePlayer.ToggleName(false);
                    component.Play(animationClip, 1f);
                    while (crewmateLeadTime > 0f)
                    {
                        __instance.HideAndSeekTimerText.text = Mathf.RoundToInt(crewmateLeadTime).ToString();
                        crewmateLeadTime -= Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    ShipStatus.Instance.HideCountdown = (float)logicOptionsHnS.GetCrewmateLeadTime();
                    if (AprilFoolsMode.ShouldHorseAround())
                    {
                        if (impostor != null)
                        {
                            impostor.AnimateCustom(__instance.HnSSeekerSpawnHorseInGameAnim);
                        }
                    }
                    else if (AprilFoolsMode.ShouldLongAround())
                    {
                        if (impostor != null)
                        {
                            impostor.AnimateCustom(__instance.HnSSeekerSpawnLongInGameAnim);
                        }
                    }
                    else if (impostor != null)
                    {
                        impostor.AnimateCustom(__instance.HnSSeekerSpawnAnim);
                        impostor.cosmetics.SetBodyCosmeticsVisible(false);
                    }
                }
                impostor = null;
                playerSlot = null;
            }
            ShipStatus.Instance.StartSFX();
            UnityEngine.Object.Destroy(__instance.gameObject);
            yield break;
        }

        public static void setupIntroTeamIcons(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            // Intro solo teams
            if (Helpers.isNeutral(PlayerControl.LocalPlayer) && !(PlayerControl.LocalPlayer == SchrodingersCat.schrodingersCat && SchrodingersCat.hideRole)) {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }

            // Add the Spy to the Impostor team (for the Impostors)
            if (Spy.spy != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
                var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
                fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
                foreach (PlayerControl p in players) {
                    if (PlayerControl.LocalPlayer != p && (p == Spy.spy || p.Data.Role.IsImpostor))
                        fakeImpostorTeam.Add(p);
                }
                yourTeam = fakeImpostorTeam;
            }

            // Role draft: If spy is enabled, don't show the team
            if (RoleDraft.isEnabled && CustomOptionHolder.spySpawnRate.getSelection() > 0 && PlayerControl.AllPlayerControls.ToArray().ToList().Where(x => x.Data.Role.IsImpostor).Count() > 1)
            {
                var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
                fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = fakeImpostorTeam;
            }
        }

        public static void setupIntroTeam(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
            RoleInfo roleInfo = infos.Where(info => !info.isModifier).FirstOrDefault();
            var neutralColor = new Color32(76, 84, 78, 255);
            if (roleInfo == null || roleInfo == RoleInfo.crewmate) {
                if (RoleDraft.isEnabled && CustomOptionHolder.neutralRolesCountMax.getSelection() > 0) {
                    __instance.TeamTitle.text = $"<size=60%>{FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Crewmate)}" +
                        Helpers.cs(Color.white, " / ") + Helpers.cs(neutralColor, ModTranslation.getString("roleIntroNeutral")) + "</size>";
                }
                return;
            }
            if (roleInfo == null) return;
            if (roleInfo.isNeutral && !(PlayerControl.LocalPlayer == SchrodingersCat.schrodingersCat && SchrodingersCat.hideRole)) {
                __instance.BackgroundBar.material.color = neutralColor;
                __instance.TeamTitle.text = ModTranslation.getString("roleIntroNeutral");
                __instance.TeamTitle.color = neutralColor;
            }
        }

        public static IEnumerator<WaitForSeconds> EndShowRole(IntroCutscene __instance) {
            yield return new WaitForSeconds(5f);
            __instance.YouAreText.gameObject.SetActive(false);
            __instance.RoleText.gameObject.SetActive(false);
            __instance.RoleBlurbText.gameObject.SetActive(false);
            __instance.ourCrewmate.gameObject.SetActive(false);
           
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
        class CreatePlayerPatch {
            public static void Postfix(IntroCutscene __instance, bool impostorPositioning, ref PoolablePlayer __result) {
                if (impostorPositioning) __result.SetNameColor(Palette.ImpostorRed);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
        class IntroCutsceneCoBeginPatch
        {
            public static bool Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
            {
                __result = CoBegin(__instance).WrapToIl2Cpp();

                return false;
            }
        }

        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
        class SetUpRoleTextPatch {
            static int seed = 0;
            static public void SetRoleTexts(IntroCutscene._ShowRole_d__41 __instance) {
                // Don't override the intro of the vanilla roles
                List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
                RoleInfo roleInfo = infos.Where(info => !info.isModifier).FirstOrDefault();
                List<RoleInfo> modifierInfo = infos.Where(info => info.isModifier).ToList();

                if (roleInfo == RoleInfo.fortuneTeller && FortuneTeller.numTasks > 0) {
                    roleInfo = RoleInfo.crewmate;
                }

                if (EventUtility.isEnabled) {
                    var roleInfos = RoleInfo.allRoleInfos.Where(x => !x.isModifier).ToList();
                    if (roleInfo.isNeutral) roleInfos.RemoveAll(x => !x.isNeutral);
                    if (roleInfo.color == Palette.ImpostorRed) roleInfos.RemoveAll(x => x.color != Palette.ImpostorRed);
                    if (!roleInfo.isNeutral && roleInfo.color != Palette.ImpostorRed) roleInfos.RemoveAll(x => x.color == Palette.ImpostorRed || x.isNeutral);
                    var rnd = new System.Random(seed);
                    roleInfo = roleInfos[rnd.Next(roleInfos.Count)];
                }

                __instance.__4__this.RoleBlurbText.text = "";
                if (roleInfo != null) {
                    __instance.__4__this.YouAreText.color = roleInfo.color;
                    __instance.__4__this.RoleText.text = roleInfo.name;
                    __instance.__4__this.RoleText.color = roleInfo.color;
                    __instance.__4__this.RoleBlurbText.text = roleInfo.introDescription;
                    __instance.__4__this.RoleBlurbText.color = roleInfo.color;
                }

                // Setup Madmate Intro
                if (Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
                {
                    if (roleInfo == RoleInfo.crewmate) __instance.__4__this.RoleText.text = ModTranslation.getString("madmate");
                    else __instance.__4__this.RoleText.text = ModTranslation.getString("madmatePrefix") + __instance.__4__this.RoleText.text;
                    __instance.__4__this.YouAreText.color = Madmate.color;
                    __instance.__4__this.RoleText.color = Madmate.color;
                    __instance.__4__this.RoleBlurbText.text = ModTranslation.getString("madmateIntroDesc");
                    __instance.__4__this.RoleBlurbText.color = Madmate.color;
                }

                if (modifierInfo != null) {
                    foreach (var info in modifierInfo) {
                        if (info.roleId != RoleId.Lover)
                            __instance.__4__this.RoleBlurbText.text += Helpers.cs(info.color, $"\n{info.introDescription}");
                        else {
                            PlayerControl otherLover = PlayerControl.LocalPlayer == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                            __instance.__4__this.RoleBlurbText.text += "\n" + Helpers.cs(Lovers.color, String.Format(ModTranslation.getString("loversFlavor"), otherLover?.Data?.PlayerName ?? ""));
                        }
                    }
                }
                if (Deputy.knowsSheriff && Deputy.deputy != null && Sheriff.sheriff != null) {
                    if (infos.Any(info => info.roleId == RoleId.Sheriff))
                        __instance.__4__this.RoleBlurbText.text += Helpers.cs(Sheriff.color, string.Format(ModTranslation.getString("deputyIntroLine"), Deputy.deputy?.Data?.PlayerName ?? ""));
                    else if (infos.Any(info => info.roleId == RoleId.Deputy))
                        __instance.__4__this.RoleBlurbText.text += Helpers.cs(Sheriff.color, string.Format(ModTranslation.getString("sheriffIntroLine"), Sheriff.sheriff?.Data?.PlayerName ?? ""));
                }
                if (infos.Any(info => info.roleId == RoleId.Kataomoi)) {
                    __instance.__4__this.RoleBlurbText.text += Helpers.cs(Kataomoi.color, string.Format(ModTranslation.getString("kataomoiIntroLine"), Kataomoi.target?.Data?.PlayerName ?? ""));
                }
            }
            public static bool Prefix(IntroCutscene._ShowRole_d__41 __instance)
            {
                seed = rnd.Next(5000);
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
                {
                    SetRoleTexts(__instance);
                })));
                return true;
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) {
                setupIntroTeamIcons(__instance, ref teamToDisplay);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) {
                setupIntroTeam(__instance, ref teamToDisplay);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                setupIntroTeamIcons(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                setupIntroTeam(__instance, ref yourTeam);
            }
        }
    }

    /* Horses are broken since 2024.3.5 - keeping this code in case they return.
     * [HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldHorseAround))]
    public static class ShouldAlwaysHorseAround {
        public static bool Prefix(ref bool __result) {
            __result = EventUtility.isEnabled && !EventUtility.disableEventMode;
            return false;
        }
    }*/

    [HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldShowAprilFoolsToggle))]
    public static class ShouldShowAprilFoolsToggle
    {
        public static void Postfix(ref bool __result)
        {
            __result = __result || EventUtility.isEventDate || EventUtility.canBeEnabled;  // Extend it to a 7 day window instead of just 1st day of the Month
        }
    }
}

