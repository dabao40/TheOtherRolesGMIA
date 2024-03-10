using HarmonyLib;
using System;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Objects;

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
            if (CachedPlayer.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null) {
                float aspect = Camera.main.aspect;
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                float xpos = 1.75f - safeOrthographicSize * aspect * 1.70f;
                float ypos = 0.15f - safeOrthographicSize * 1.7f;
                bottomLeft = new Vector3(xpos / 2, ypos/2, -61f);

                foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                    GameData.PlayerInfo data = p.Data;
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

                    if (CachedPlayer.LocalPlayer.PlayerControl == Arsonist.arsonist && p != Arsonist.arsonist) {
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
            if (BountyHunter.bounty != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter) {
                BountyHunter.bountyUpdateTimer = 0f;
                if (FastDestroyableSingleton<HudManager>.Instance != null) {
                    BountyHunter.cooldownText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                    BountyHunter.cooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                    BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -62f);
                    BountyHunter.cooldownText.transform.localScale = Vector3.one * 0.4f;
                    BountyHunter.cooldownText.gameObject.SetActive(true);
                }
            }

            // Set the name of the Mimic(Killer)
            if (MimicK.mimicK != null) MimicK.name = MimicK.mimicK.Data.PlayerName;

            // Force Reload of SoundEffectHolder
            SoundEffectsManager.Load();

            if (CustomOptionHolder.randomGameStartPosition.getBool())
            { //Random spawn on game start

                List<Vector3> skeldSpawn = new List<Vector3>() {
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

                List<Vector3> miraSpawn = new List<Vector3>() {
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

                List<Vector3> polusSpawn = new List<Vector3>() {
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

                List<Vector3> dleksSpawn = new List<Vector3>() {
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

                List<Vector3> fungleSpawn = new List<Vector3>() {
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

                if (Helpers.isSkeld()) CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(skeldSpawn[rnd.Next(skeldSpawn.Count)]);
                if (Helpers.isMira()) CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(miraSpawn[rnd.Next(miraSpawn.Count)]);
                if (Helpers.isPolus()) CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(polusSpawn[rnd.Next(polusSpawn.Count)]);
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3) CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(dleksSpawn[rnd.Next(dleksSpawn.Count)]);
                if (Helpers.isFungle()) CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo(fungleSpawn[rnd.Next(fungleSpawn.Count)]);

            }

            // First kill
            if (AmongUsClient.Instance.AmHost && TORMapOptions.shieldFirstKill && TORMapOptions.firstKillName != "" && !HideNSeek.isHideNSeekGM) {
                PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(TORMapOptions.firstKillName));
                if (target != null) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFirstKill, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFirstKill(target.PlayerId);
                }
            }
            TORMapOptions.firstKillName = "";

            if (Helpers.isAirship() && CustomOptionHolder.airshipOptimize.getBool() && Helpers.hasImpVision(GameData.Instance.GetPlayerById(CachedPlayer.LocalPlayer.PlayerId)))
            {
                var obj = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].gameObject;
                OneWayShadows oneWayShadow = obj.transform.FindChild("Shadow").FindChild("LedgeShadow").GetComponent<OneWayShadows>();
                oneWayShadow.gameObject.SetActive(false);
            }

            // Additional Vents
            AdditionalVents.AddAdditionalVents();

            // Specimen Vitals
            SpecimenVital.moveVital();

            // Add Electrical
            FungleAdditionalElectrical.CreateElectrical();

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
        public static void setupIntroTeamIcons(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            // Intro solo teams
            if (Helpers.isNeutral(CachedPlayer.LocalPlayer.PlayerControl)) {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(CachedPlayer.LocalPlayer.PlayerControl);
                yourTeam = soloTeam;
            }

            // Add the Spy to the Impostor team (for the Impostors)
            if (Spy.spy != null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor) {
                List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
                var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
                fakeImpostorTeam.Add(CachedPlayer.LocalPlayer.PlayerControl);
                foreach (PlayerControl p in players) {
                    if (CachedPlayer.LocalPlayer.PlayerControl != p && (p == Spy.spy || p.Data.Role.IsImpostor))
                        fakeImpostorTeam.Add(p);
                }
                yourTeam = fakeImpostorTeam;
            }
        }

        public static void setupIntroTeam(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl);
            RoleInfo roleInfo = infos.Where(info => !info.isModifier).FirstOrDefault();
            if (roleInfo == null) return;
            if (roleInfo.isNeutral) {
                var neutralColor = new Color32(76, 84, 78, 255);
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


        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
        class SetUpRoleTextPatch {
            static int seed = 0;
            static public void SetRoleTexts(IntroCutscene __instance) {
                // Don't override the intro of the vanilla roles
                List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl);
                RoleInfo roleInfo = infos.Where(info => !info.isModifier).FirstOrDefault();
                RoleInfo modifierInfo = infos.Where(info => info.isModifier).FirstOrDefault();

                if (roleInfo == RoleInfo.fortuneTeller && FortuneTeller.numTasks > 0)
                {
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

                __instance.RoleBlurbText.text = "";
                if (roleInfo != null) {
                    __instance.YouAreText.color = roleInfo.color;
                    __instance.RoleText.text = roleInfo.name;
                    __instance.RoleText.color = roleInfo.color;
                    __instance.RoleBlurbText.text = roleInfo.introDescription;
                    __instance.RoleBlurbText.color = roleInfo.color;
                }

                // Setup Madmate Intro
                if (Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId))
                {
                    if (roleInfo == RoleInfo.crewmate) __instance.RoleText.text = ModTranslation.getString("madmate");
                    else __instance.RoleText.text = ModTranslation.getString("madmatePrefix") + __instance.RoleText.text;
                    __instance.YouAreText.color = Madmate.color;
                    __instance.RoleText.color = Madmate.color;
                    __instance.RoleBlurbText.text = ModTranslation.getString("madmateIntroDesc");
                    __instance.RoleBlurbText.color = Madmate.color;
                }

                if (modifierInfo != null) {
                    if (modifierInfo.roleId != RoleId.Lover)
                        __instance.RoleBlurbText.text += Helpers.cs(modifierInfo.color, $"\n{modifierInfo.introDescription}");
                    else {
                        PlayerControl otherLover = CachedPlayer.LocalPlayer.PlayerControl == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                        __instance.RoleBlurbText.text += "\n" + Helpers.cs(Lovers.color, String.Format(ModTranslation.getString("loversFlavor"), otherLover?.Data?.PlayerName ?? ""));
                    }
                }
                if (Deputy.knowsSheriff && Deputy.deputy != null && Sheriff.sheriff != null) {
                    if (infos.Any(info => info.roleId == RoleId.Sheriff))
                        __instance.RoleBlurbText.text += Helpers.cs(Sheriff.color, string.Format(ModTranslation.getString("deputyIntroLine"), Deputy.deputy?.Data?.PlayerName ?? ""));
                    else if (infos.Any(info => info.roleId == RoleId.Deputy))
                        __instance.RoleBlurbText.text += Helpers.cs(Sheriff.color, string.Format(ModTranslation.getString("sheriffIntroLine"), Sheriff.sheriff?.Data?.PlayerName ?? ""));
                }
            }
            public static bool Prefix(IntroCutscene __instance) {
                if (!CustomOptionHolder.activateRoles.getBool()) return true;
                seed = rnd.Next(5000);
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) => {
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

                if (Madmate.hasTasks && Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.clearAllTasks();
                    CachedPlayer.LocalPlayer.PlayerControl.generateAndAssignTasks(Madmate.commonTasks, Madmate.shortTasks, Madmate.longTasks);
                }
                if (JekyllAndHyde.jekyllAndHyde != null && CachedPlayer.LocalPlayer.PlayerControl == JekyllAndHyde.jekyllAndHyde)
                {
                    PlayerControl.LocalPlayer.clearAllTasks();
                    CachedPlayer.LocalPlayer.PlayerControl.generateAndAssignTasks(JekyllAndHyde.numCommonTasks, JekyllAndHyde.numShortTasks, JekyllAndHyde.numLongTasks);
                    JekyllAndHyde.oddIsJekyll = rnd.Next(0, 2) == 1;
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetOddIsJekyll, Hazel.SendOption.Reliable, -1);
                    writer.Write(JekyllAndHyde.oddIsJekyll);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    HudManagerStartPatch.jekyllAndHydeSuicideButton.Timer = JekyllAndHyde.suicideTimer;
                }
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

