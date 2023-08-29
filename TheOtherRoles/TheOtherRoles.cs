using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using static TheOtherRoles.TheOtherRoles;
using AmongUs.Data;
using Hazel;
using JetBrains.Annotations;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public static class TheOtherRoles
    {
        public static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);

        public static void clearAndReloadRoles() {
            Jester.clearAndReload();
            Mayor.clearAndReload();
            Portalmaker.clearAndReload();
            Engineer.clearAndReload();
            Sheriff.clearAndReload();
            Deputy.clearAndReload();
            Lighter.clearAndReload();
            Godfather.clearAndReload();
            Mafioso.clearAndReload();
            Janitor.clearAndReload();
            Detective.clearAndReload();
            TimeMaster.clearAndReload();
            Undertaker.ClearAndReload();
            Medic.clearAndReload();
            Shifter.clearAndReload();
            Swapper.clearAndReload();
            Lovers.clearAndReload();
            Seer.clearAndReload();
            Bait.clearAndReload();
            Morphling.clearAndReload();
            Camouflager.clearAndReload();
            Hacker.clearAndReload();
            Tracker.clearAndReload();
            Vampire.clearAndReload();
            Snitch.clearAndReload();
            Jackal.clearAndReload();
            Sidekick.clearAndReload();
            Eraser.clearAndReload();
            Spy.clearAndReload();
            Trickster.clearAndReload();
            Cleaner.clearAndReload();
            Warlock.clearAndReload();
            SecurityGuard.clearAndReload();
            Arsonist.clearAndReload();
            BountyHunter.clearAndReload();
            Vulture.clearAndReload();
            Medium.clearAndReload();
            Lawyer.clearAndReload();
            Pursuer.clearAndReload();
            Witch.clearAndReload();
            Assassin.clearAndReload();
            Thief.clearAndReload();
            Amnisiac.clearAndReload();
            Madmate.clearAndReload();
            Overlord.clearAndReload();
            //Trapper.clearAndReload();
            //Bomber.clearAndReload();

            // GMIA
            Ninja.clearAndReload();
            NekoKabocha.clearAndReload();
            SerialKiller.clearAndReload();
            FortuneTeller.clearAndReload();
            Sprinter.clearAndReload();
            Opportunist.clearAndReload();

            // Modifier
            //Bait.clearAndReload();
            Bloody.clearAndReload();
            AntiTeleport.clearAndReload();
            Tiebreaker.clearAndReload();
            Sunglasses.clearAndReload();
            Mini.clearAndReload();
            Vip.clearAndReload();
            Invert.clearAndReload();
            Chameleon.clearAndReload();
            Cursed.clearAndReload();

            // Gamemodes
            HandleGuesser.clearAndReload();
            HideNSeek.clearAndReload();

        }

        public static class Jester {
            public static PlayerControl jester;
            public static Color color = new Color32(236, 98, 165, byte.MaxValue);

            public static bool triggerJesterWin = false;
            public static bool canCallEmergency = true;
            public static bool hasImpostorVision = false;

            public static void clearAndReload() {
                jester = null;
                triggerJesterWin = false;
                canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
                hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.getBool();
            }
        }

        public static class Madmate
        {
            public static PlayerControl madmate;
            public static Color color = Palette.ImpostorRed;

            public static bool canEnterVents = false;
            public static bool madmateCanDieToSheriff = false;
            public static bool hasImpostorVision = false;

            public static void clearAndReload()
            {
                madmate = null;
                canEnterVents = CustomOptionHolder.madmateCanEnterVents.getBool();
                madmateCanDieToSheriff = CustomOptionHolder.madmateCanDieToSheriff.getBool();
                hasImpostorVision = CustomOptionHolder.madmateHasImpostorVision.getBool();
            }
        }

        public static class Cursed
        {
            public static PlayerControl cursed;
            public static Color crewColor = new Color32(0, 247, 255, byte.MaxValue);
            public static Color impColor = Palette.ImpostorRed;
            public static Color color = crewColor;
            public static void clearAndReload()
            {
                cursed = null;
            }
        }
        public static class Overlord
        {
            public static PlayerControl overlord;
            public static Color color = new Color32(0, 128, 128, 255);

            public static int meetingsAlive = 0;
            public static int meetingNumber = 2;
            public static bool triggerWin = false;

            public static void clearAndReload()
            {
                overlord = null;
                meetingsAlive = 0;
                triggerWin = false;
                meetingNumber = Mathf.RoundToInt(CustomOptionHolder.overlordMeetingNumber.getFloat());
            }
        }
        public static class Undertaker
        {
            public static PlayerControl Player;
            public static readonly Color Color = Palette.ImpostorRed;
            public static DeadBody DraggedBody;
            public static DeadBody TargetBody;
            public static bool CanDropBody;

            public static Sprite DragButtonSprite =>
                Helpers.loadSpriteFromResources("BetterOtherRoles.Resources.DragButton.png", 115f);
            public static Sprite DropButtonSprite => Helpers.loadSpriteFromResources("BetterOtherRoles.Resources.DropButton.png", 115f);

            public static void ClearAndReload()
            {
                Player = null;
                DraggedBody = null;
                TargetBody = null;
            }

            public static void RpcDropBody(Vector3 position)
            {
                if (Player == null) return;
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UndertakerDropBody, Hazel.SendOption.Reliable, -1);
                writer.Write(position.x);
                writer.Write(position.y);
                writer.Write(position.z);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                DropBody(position);
            }

            public static void DropBody(Vector3 position)
            {
                if (!DraggedBody) return;
                DraggedBody.transform.position = position;
                DraggedBody = null;
                TargetBody = null;
            }

            public static void RpcDragBody(byte playerId)
            {
                if (Player == null) return;
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UndertakerDragBody, Hazel.SendOption.Reliable, -1);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                DragBody(playerId);
            }

            public static void DragBody(byte playerId)
            {
                if (Player == null) return;
                var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == playerId);
                if (body == null) return;
                DraggedBody = body;
            }
        }

        public static class Portalmaker {
            public static PlayerControl portalmaker;
            public static Color color = new Color32(69, 69, 169, byte.MaxValue);

            public static float cooldown;
            public static float usePortalCooldown;
            public static bool logOnlyHasColors;
            public static bool logShowsTime;
            public static bool canPortalFromAnywhere;

            private static Sprite placePortalButtonSprite;
            private static Sprite usePortalButtonSprite;
            private static Sprite usePortalSpecialButtonSprite1;
            private static Sprite usePortalSpecialButtonSprite2;
            private static Sprite logSprite;

            public static Sprite getPlacePortalButtonSprite() {
                if (placePortalButtonSprite) return placePortalButtonSprite;
                placePortalButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlacePortalButton.png", 115f);
                return placePortalButtonSprite;
            }

            public static Sprite getUsePortalButtonSprite() {
                if (usePortalButtonSprite) return usePortalButtonSprite;
                usePortalButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalButton.png", 115f);
                return usePortalButtonSprite;
            }

            public static Sprite getUsePortalSpecialButtonSprite(bool first) {
                if (first) {
                    if (usePortalSpecialButtonSprite1) return usePortalSpecialButtonSprite1;
                    usePortalSpecialButtonSprite1 = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton1.png", 115f);
                    return usePortalSpecialButtonSprite1;
                } else {
                    if (usePortalSpecialButtonSprite2) return usePortalSpecialButtonSprite2;
                    usePortalSpecialButtonSprite2 = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton2.png", 115f);
                    return usePortalSpecialButtonSprite2;
                }
            }

            public static Sprite getLogSprite() {
                if (logSprite) return logSprite;
                logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
                return logSprite;
            }

            public static void clearAndReload() {
                portalmaker = null;
                cooldown = CustomOptionHolder.portalmakerCooldown.getFloat();
                usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.getFloat();
                logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.getBool();
                logShowsTime = CustomOptionHolder.portalmakerLogHasTime.getBool();
                canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.getBool();
            }


        }
        public static class Amnisiac
        {
            public static PlayerControl amnisiac;
            public static List<Arrow> localArrows = new List<Arrow>();
            public static Color color = new Color(0.5f, 0.7f, 1f, 1f);
            public static List<PoolablePlayer> poolIcons = new List<PoolablePlayer>();

            public static bool showArrows = true;
            public static bool resetRole = false;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Remember.png", 115f);
                return buttonSprite;
            }

            public static void clearAndReload()
            {
                amnisiac = null;
                showArrows = CustomOptionHolder.amnisiacShowArrows.getBool();
                resetRole = CustomOptionHolder.amnisiacResetRole.getBool();
                if (localArrows != null)
                {
                    foreach (Arrow arrow in localArrows)
                        if (arrow?.arrow != null)
                            UnityEngine.Object.Destroy(arrow.arrow);
                }
                localArrows = new List<Arrow>();
            }
        }

        public static class Mayor {
            public static PlayerControl mayor;
            public static Color color = new Color32(32, 77, 66, byte.MaxValue);
            public static Minigame emergency = null;
            public static Sprite emergencySprite = null;
            public static int remoteMeetingsLeft = 1;

            public static bool canSeeVoteColors = false;
            public static int tasksNeededToSeeVoteColors;
            public static bool meetingButton = true;
            public static int mayorChooseSingleVote;

            public static bool voteTwice = true;

            public static Sprite getMeetingSprite()
            {
                if (emergencySprite) return emergencySprite;
                emergencySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
                return emergencySprite;
            }

            public static void clearAndReload() {
                mayor = null;
                emergency = null;
                emergencySprite = null;
		        remoteMeetingsLeft = Mathf.RoundToInt(CustomOptionHolder.mayorMaxRemoteMeetings.getFloat()); 
                canSeeVoteColors = CustomOptionHolder.mayorCanSeeVoteColors.getBool();
                tasksNeededToSeeVoteColors = (int)CustomOptionHolder.mayorTasksNeededToSeeVoteColors.getFloat();
                meetingButton = CustomOptionHolder.mayorMeetingButton.getBool();
                mayorChooseSingleVote = CustomOptionHolder.mayorChooseSingleVote.getSelection();
                voteTwice = true;
            }
        }

        public static class Engineer {
            public static PlayerControl engineer;
            public static Color color = new Color32(0, 40, 245, byte.MaxValue);
            private static Sprite buttonSprite;

            public static int remainingFixes = 1;           
            public static bool highlightForImpostors = true;
            public static bool highlightForTeamJackal = true; 

            public static Sprite getButtonSprite() {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
                return buttonSprite;
            }

            public static void clearAndReload() {
                engineer = null;
                remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.getFloat());
                highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.getBool();
                highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.getBool();
            }
        }

        public static class Godfather {
            public static PlayerControl godfather;
            public static Color color = Palette.ImpostorRed;

            public static void clearAndReload() {
                godfather = null;
            }
        }

        public static class Mafioso {
            public static PlayerControl mafioso;
            public static Color color = Palette.ImpostorRed;

            public static void clearAndReload() {
                mafioso = null;
            }
        }


        public static class Janitor {
            public static PlayerControl janitor;
            public static Color color = Palette.ImpostorRed;

            public static float cooldown = 30f;

            private static Sprite buttonSprite;
            public static Sprite getButtonSprite() {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
                return buttonSprite;
            }

            public static void clearAndReload() {
                janitor = null;
                cooldown = CustomOptionHolder.janitorCooldown.getFloat();
            }
        }

        public static class Sheriff {
            public static PlayerControl sheriff;
            public static Color color = new Color32(248, 205, 70, byte.MaxValue);

            public static float cooldown = 30f;
            public static bool canKillNeutrals = false;
            public static bool spyCanDieToSheriff = false;

            public static PlayerControl currentTarget;

            public static PlayerControl formerDeputy;  // Needed for keeping handcuffs + shifting
            public static PlayerControl formerSheriff;  // When deputy gets promoted...

            public static void replaceCurrentSheriff(PlayerControl deputy)
            {
                if (!formerSheriff) formerSheriff = sheriff;
                sheriff = deputy;
                currentTarget = null;
                cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
            }

            public static void clearAndReload() {
                sheriff = null;
                currentTarget = null;
                formerDeputy = null;
                formerSheriff = null;
                cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
                canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.getBool();
                spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.getBool();
            }
        }

        public static class Deputy
        {
            public static PlayerControl deputy;
            public static Color color = Sheriff.color;

            public static PlayerControl currentTarget;
            public static List<byte> handcuffedPlayers = new List<byte>();
            public static int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
            public static bool keepsHandcuffsOnPromotion;
            public static float handcuffDuration;
            public static float remainingHandcuffs;
            public static float handcuffCooldown;
            public static bool knowsSheriff;
            public static Dictionary<byte, float> handcuffedKnows = new Dictionary<byte, float>();

            private static Sprite buttonSprite;
            private static Sprite handcuffedSprite;
            
            public static Sprite getButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DeputyHandcuffButton.png", 115f);
                return buttonSprite;
            }

            public static Sprite getHandcuffedButtonSprite()
            {
                if (handcuffedSprite) return handcuffedSprite;
                handcuffedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DeputyHandcuffed.png", 115f);
                return handcuffedSprite;
            }

            // Can be used to enable / disable the handcuff effect on the target's buttons
            public static void setHandcuffedKnows(bool active = true, byte playerId = Byte.MaxValue)
            {
                if (playerId == Byte.MaxValue)
                    playerId = CachedPlayer.LocalPlayer.PlayerId;

                if (active && playerId == CachedPlayer.LocalPlayer.PlayerId) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (active) {
                    handcuffedKnows.Add(playerId, handcuffDuration);
                    handcuffedPlayers.RemoveAll(x => x == playerId);
               }

                if (playerId == CachedPlayer.LocalPlayer.PlayerId) {
                    HudManagerStartPatch.setAllButtonsHandcuffedStatus(active);
                    SoundEffectsManager.play("deputyHandcuff");
		}
 
	    }

            public static void clearAndReload()
            {
                deputy = null;
                currentTarget = null;
                handcuffedPlayers = new List<byte>();
                handcuffedKnows = new Dictionary<byte, float>();
                HudManagerStartPatch.setAllButtonsHandcuffedStatus(false, true);
                promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.getSelection();
                remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.getFloat();
                handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.getFloat();
                keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.getBool();
                handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.getFloat();
                knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.getBool();
            }
        }

        public static class Lighter {
            public static PlayerControl lighter;
            public static Color color = new Color32(238, 229, 190, byte.MaxValue);
            
            public static float lighterModeLightsOnVision = 2f;
            public static float lighterModeLightsOffVision = 0.75f;
            public static float flashlightWidth = 0.75f;
            public static bool canSeeInvisible = true;

            public static void clearAndReload() {
                lighter = null;
                flashlightWidth = CustomOptionHolder.lighterFlashlightWidth.getFloat();
                lighterModeLightsOnVision = CustomOptionHolder.lighterModeLightsOnVision.getFloat();
                lighterModeLightsOffVision = CustomOptionHolder.lighterModeLightsOffVision.getFloat();
                canSeeInvisible = CustomOptionHolder.lighterCanSeeInvisible.getBool();
            }
        }

        public static class Detective {
            public static PlayerControl detective;
            public static Color color = new Color32(45, 106, 165, byte.MaxValue);

            public static float footprintIntervall = 1f;
            public static float footprintDuration = 1f;
            public static bool anonymousFootprints = false;
            public static float reportNameDuration = 0f;
            public static float reportColorDuration = 20f;
            public static float timer = 6.2f;

            public static void clearAndReload() {
                detective = null;
                anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
                footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
                footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
                reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
                reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
                timer = 6.2f;
            }
        }
    }

    public static class TimeMaster {
        public static PlayerControl timeMaster;
        public static Color color = new Color32(112, 142, 239, byte.MaxValue);

        public static bool reviveDuringRewind = false;
        public static float rewindTime = 3f;
        public static float shieldDuration = 3f;
        public static float cooldown = 30f;

        public static bool shieldActive = false;
        public static bool isRewinding = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TimeShieldButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            timeMaster = null;
            isRewinding = false;
            shieldActive = false;
            rewindTime = CustomOptionHolder.timeMasterRewindTime.getFloat();
            shieldDuration = CustomOptionHolder.timeMasterShieldDuration.getFloat();
            cooldown = CustomOptionHolder.timeMasterCooldown.getFloat();
        }
    }

    public static class Medic {
        public static PlayerControl medic;
        public static PlayerControl shielded;
        public static PlayerControl futureShielded;
        
        public static Color color = new Color32(126, 251, 194, byte.MaxValue);
        public static bool usedShield;

        public static int showShielded = 0;
        public static bool showAttemptToShielded = false;
        public static bool showAttemptToMedic = false;
        public static bool setShieldAfterMeeting = false;
        public static bool showShieldAfterMeeting = false;
        public static bool meetingAfterShielding = false;

        public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
        public static PlayerControl currentTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShieldButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            medic = null;
            shielded = null;
            futureShielded = null;
            currentTarget = null;
            usedShield = false;
            showShielded = CustomOptionHolder.medicShowShielded.getSelection();
            showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.getBool();
            showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.getBool();
            setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 2;
            showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 1;
            meetingAfterShielding = false;
        }
    }

    public static class Swapper {
        public static PlayerControl swapper;
        public static Color color = new Color32(134, 55, 86, byte.MaxValue);
        private static Sprite spriteCheck;
        public static bool canCallEmergency = false;
        public static bool canOnlySwapOthers = false;
        public static int charges;
        public static float rechargeTasksNumber;
        public static float rechargedTasks;
 
        public static byte playerId1 = Byte.MaxValue;
        public static byte playerId2 = Byte.MaxValue;

        public static Sprite getCheckSprite() {
            if (spriteCheck) return spriteCheck;
            spriteCheck = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SwapperCheck.png", 150f);
            return spriteCheck;
        }

        public static void clearAndReload() {
            swapper = null;
            playerId1 = Byte.MaxValue;
            playerId2 = Byte.MaxValue;
            canCallEmergency = CustomOptionHolder.swapperCanCallEmergency.getBool();
            canOnlySwapOthers = CustomOptionHolder.swapperCanOnlySwapOthers.getBool();
            charges = Mathf.RoundToInt(CustomOptionHolder.swapperSwapsNumber.getFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.getFloat());
        }
    }

    public static class Lovers {
        public static PlayerControl lover1;
        public static PlayerControl lover2;
        public static Color color = new Color32(232, 57, 185, byte.MaxValue);

        public static bool bothDie = true;
        public static bool enableChat = true;
        // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
        public static bool notAckedExiledIsLover = false;

        public static bool existing() {
            return lover1 != null && lover2 != null && !lover1.Data.Disconnected && !lover2.Data.Disconnected;
        }

        public static bool existingAndAlive() {
            return existing() && !lover1.Data.IsDead && !lover2.Data.IsDead && !notAckedExiledIsLover; // ADD NOT ACKED IS LOVER
        }

        public static PlayerControl otherLover(PlayerControl oneLover) {
            if (!existingAndAlive()) return null;
            if (oneLover == lover1) return lover2;
            if (oneLover == lover2) return lover1;
            return null;
        }

        public static bool existingWithKiller() {
            return existing() && (lover1 == Jackal.jackal     || lover2 == Jackal.jackal
                               || lover1 == Sidekick.sidekick || lover2 == Sidekick.sidekick
                               || lover1.Data.Role.IsImpostor      || lover2.Data.Role.IsImpostor);
        }

        public static bool hasAliveKillingLover(this PlayerControl player) {
            if (!Lovers.existingAndAlive() || !existingWithKiller())
                return false;
            return (player != null && (player == lover1 || player == lover2));
        }

        public static void clearAndReload() {
            lover1 = null;
            lover2 = null;
            notAckedExiledIsLover = false;
            bothDie = CustomOptionHolder.modifierLoverBothDie.getBool();
            enableChat = CustomOptionHolder.modifierLoverEnableChat.getBool();
        }

        public static PlayerControl getPartner(this PlayerControl player) {
            if (player == null)
                return null;
            if (lover1 == player)
                return lover2;
            if (lover2 == player)
                return lover1;
            return null;
        }
    }

    public static class Seer {
        public static PlayerControl seer;
        public static Color color = new Color32(97, 178, 108, byte.MaxValue);
        public static List<Vector3> deadBodyPositions = new List<Vector3>();

        public static float soulDuration = 15f;
        public static bool limitSoulDuration = false;
        public static int mode = 0;

        private static Sprite soulSprite;
        public static Sprite getSoulSprite() {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
            return soulSprite;
        }

        public static void clearAndReload() {
            seer = null;
            deadBodyPositions = new List<Vector3>();
            limitSoulDuration = CustomOptionHolder.seerLimitSoulDuration.getBool();
            soulDuration = CustomOptionHolder.seerSoulDuration.getFloat();
            mode = CustomOptionHolder.seerMode.getSelection();
        }
    }

    public static class Morphling {
        public static PlayerControl morphling;
        public static Color color = Palette.ImpostorRed;
        private static Sprite sampleSprite;
        private static Sprite morphSprite;
    
        public static float cooldown = 30f;
        public static float duration = 10f;

        public static PlayerControl currentTarget;
        public static PlayerControl sampledTarget;
        public static PlayerControl morphTarget;
        public static float morphTimer = 0f;

        public static void resetMorph() {
            morphTarget = null;
            morphTimer = 0f;
            if (morphling == null) return;
            morphling.setDefaultLook();
        }

        public static void clearAndReload() {
            resetMorph();
            morphling = null;
            currentTarget = null;
            sampledTarget = null;
            morphTarget = null;
            morphTimer = 0f;
            cooldown = CustomOptionHolder.morphlingCooldown.getFloat();
            duration = CustomOptionHolder.morphlingDuration.getFloat();
        }

        public static Sprite getSampleSprite() {
            if (sampleSprite) return sampleSprite;
            sampleSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SampleButton.png", 115f);
            return sampleSprite;
        }

        public static Sprite getMorphSprite() {
            if (morphSprite) return morphSprite;
            morphSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MorphButton.png", 115f);
            return morphSprite;
        }
    }

    public static class Camouflager {
        public static PlayerControl camouflager;
        public static Color color = Palette.ImpostorRed;
    
        public static float cooldown = 30f;
        public static float duration = 10f;
        public static float camouflageTimer = 0f;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CamoButton.png", 115f);
            return buttonSprite;
        }

        public static void resetCamouflage() {
            camouflageTimer = 0f;
            foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                if ((p == Ninja.ninja && Ninja.stealthed) || (p == Sprinter.sprinter && Sprinter.sprinting))
                    continue;
                p.setDefaultLook();
            }
        }

        public static void clearAndReload() {
            resetCamouflage();
            camouflager = null;
            camouflageTimer = 0f;
            cooldown = CustomOptionHolder.camouflagerCooldown.getFloat();
            duration = CustomOptionHolder.camouflagerDuration.getFloat();
        }
    }

    public static class Hacker {
        public static PlayerControl hacker;
        public static Minigame vitals = null;
        public static Minigame doorLog = null;
        public static Color color = new Color32(117, 250, 76, byte.MaxValue);

        public static float cooldown = 30f;
        public static float duration = 10f;
        public static float toolsNumber = 5f;
        public static bool onlyColorType = false;
        public static float hackerTimer = 0f;
        public static int rechargeTasksNumber = 2;
        public static int rechargedTasks = 2;
        public static int chargesVitals = 1;
        public static int chargesAdminTable = 1;
        public static bool cantMove = true;

        private static Sprite buttonSprite;
        private static Sprite vitalsSprite;
        private static Sprite logSprite;
        private static Sprite adminSprite;

        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.HackerButton.png", 115f);
            return buttonSprite;
        }

        public static Sprite getVitalsSprite() {
            if (vitalsSprite) return vitalsSprite;
            vitalsSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
            return vitalsSprite;
        }

        public static Sprite getLogSprite() {
            if (logSprite) return logSprite;
            logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
        }

        public static Sprite getAdminSprite() {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (mapId == 0 || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (mapId == 1) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (mapId == 4) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            adminSprite = button.Image;
            return adminSprite;
        }

        public static void clearAndReload() {
            hacker = null;
            vitals = null;
            doorLog = null;
            hackerTimer = 0f;
            adminSprite = null;
            cooldown = CustomOptionHolder.hackerCooldown.getFloat();
            duration = CustomOptionHolder.hackerHackeringDuration.getFloat();
            onlyColorType = CustomOptionHolder.hackerOnlyColorType.getBool();
            toolsNumber = CustomOptionHolder.hackerToolsNumber.getFloat();
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.getFloat());
            chargesVitals = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
            chargesAdminTable = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.getFloat()) / 2;
            cantMove = CustomOptionHolder.hackerNoMove.getBool();
        }
    }

    public static class Tracker {
        public static PlayerControl tracker;
        public static Color color = new Color32(100, 58, 220, byte.MaxValue);
        public static List<Arrow> localArrows = new List<Arrow>();

        public static float updateIntervall = 5f;
        public static bool resetTargetAfterMeeting = false;
        public static bool canTrackCorpses = false;
        public static float corpsesTrackingCooldown = 30f;
        public static float corpsesTrackingDuration = 5f;
        public static float corpsesTrackingTimer = 0f;
        public static List<Vector3> deadBodyPositions = new List<Vector3>();

        public static PlayerControl currentTarget;
        public static PlayerControl tracked;
        public static bool usedTracker = false;
        public static float timeUntilUpdate = 0f;
        public static Arrow arrow = new Arrow(Color.blue);

        private static Sprite trackCorpsesButtonSprite;
        public static Sprite getTrackCorpsesButtonSprite()
        {
            if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
            trackCorpsesButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PathfindButton.png", 115f);
            return trackCorpsesButtonSprite;
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
            return buttonSprite;
        }

        public static void resetTracked() {
            currentTarget = tracked = null;
            usedTracker = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }

        public static void clearAndReload() {
            tracker = null;
            resetTracked();
            timeUntilUpdate = 0f;
            updateIntervall = CustomOptionHolder.trackerUpdateIntervall.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.getBool();
            if (localArrows != null) {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            deadBodyPositions = new List<Vector3>();
            corpsesTrackingTimer = 0f;
            corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.getFloat();
            corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.getFloat();
            canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.getBool();
        }
    }

    public static class Vampire {
        public static PlayerControl vampire;
        public static Color color = Palette.ImpostorRed;

        public static float delay = 10f;
        public static float cooldown = 30f;
        public static bool canKillNearGarlics = true;
        public static bool localPlacedGarlic = false;
        public static bool garlicsActive = true;

        public static PlayerControl currentTarget;
        public static PlayerControl bitten; 
        public static bool targetNearGarlic = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VampireButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite garlicButtonSprite;
        public static Sprite getGarlicButtonSprite() {
            if (garlicButtonSprite) return garlicButtonSprite;
            garlicButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.GarlicButton.png", 115f);
            return garlicButtonSprite;
        }

        public static void clearAndReload() {
            vampire = null;
            bitten = null;
            targetNearGarlic = false;
            localPlacedGarlic = false;
            currentTarget = null;
            garlicsActive = CustomOptionHolder.vampireSpawnRate.getSelection() > 0;
            delay = CustomOptionHolder.vampireKillDelay.getFloat();
            cooldown = CustomOptionHolder.vampireCooldown.getFloat();
            canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.getBool();
        }
    }

    public static class Snitch {
        public static PlayerControl snitch;
        public static Color color = new Color32(184, 251, 79, byte.MaxValue);
        public enum Mode {
            Chat = 0,
            Map = 1,
            ChatAndMap = 2
        }
        public enum Targets {
            EvilPlayers = 0,
            Killers = 1
        }

        public static Mode mode = Mode.Chat;
        public static Targets targets = Targets.EvilPlayers;
        public static int taskCountForReveal = 1;

        public static bool isRevealed = false;
        public static Dictionary<byte, byte> playerRoomMap = new Dictionary<byte, byte>();
        public static TMPro.TextMeshPro text = null;
        public static bool needsUpdate = true;

        public static void clearAndReload() {
            taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
            snitch = null;
            isRevealed = false;
            playerRoomMap = new Dictionary<byte, byte>();
            if (text != null) UnityEngine.Object.Destroy(text);
            text = null;
            needsUpdate = true;
            mode = (Mode) CustomOptionHolder.snitchMode.getSelection();
            targets = (Targets) CustomOptionHolder.snitchTargets.getSelection();
        }
    }

    public static class Jackal {
        public static PlayerControl jackal;
        public static Color color = new Color32(0, 180, 235, byte.MaxValue);
        public static PlayerControl fakeSidekick;
        public static PlayerControl currentTarget;
        public static List<PlayerControl> formerJackals = new List<PlayerControl>();
        
        public static float cooldown = 30f;
        public static float createSidekickCooldown = 30f;
        public static bool canUseVents = true;
        public static bool canCreateSidekick = true;
        public static Sprite buttonSprite;
        public static bool jackalPromotedFromSidekickCanCreateSidekick = true;
        public static bool canCreateSidekickFromImpostor = true;
        public static bool hasImpostorVision = false;
        public static bool wasTeamRed;
        public static bool wasImpostor;
        public static bool wasSpy;

        public static Sprite getSidekickButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
            return buttonSprite;
        }

        public static void removeCurrentJackal() {
            if (!formerJackals.Any(x => x.PlayerId == jackal.PlayerId)) formerJackals.Add(jackal);
            jackal = null;
            currentTarget = null;
            fakeSidekick = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
        }

        public static void clearAndReload() {
            jackal = null;
            currentTarget = null;
            fakeSidekick = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
            canUseVents = CustomOptionHolder.jackalCanUseVents.getBool();
            canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.getBool();
            jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.getBool();
            canCreateSidekickFromImpostor = CustomOptionHolder.jackalCanCreateSidekickFromImpostor.getBool();
            formerJackals.Clear();
            hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            wasTeamRed = wasImpostor = wasSpy = false;
        }
        
    }

    public static class Sidekick {
        public static PlayerControl sidekick;
        public static Color color = new Color32(0, 180, 235, byte.MaxValue);

        public static PlayerControl currentTarget;

        public static bool wasTeamRed;
        public static bool wasImpostor;
        public static bool wasSpy;

        public static float cooldown = 30f;
        public static bool canUseVents = true;
        public static bool canKill = true;
        public static bool promotesToJackal = true;
        public static bool hasImpostorVision = false;

        public static void clearAndReload() {
            sidekick = null;
            currentTarget = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            canUseVents = CustomOptionHolder.sidekickCanUseVents.getBool();
            canKill = CustomOptionHolder.sidekickCanKill.getBool();
            promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.getBool();
            hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            wasTeamRed = wasImpostor = wasSpy = false;
        }
    }

    public static class Eraser {
        public static PlayerControl eraser;
        public static Color color = Palette.ImpostorRed;

        public static List<byte> alreadyErased = new List<byte>();

        public static List<PlayerControl> futureErased = new List<PlayerControl>();
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static bool canEraseAnyone = false; 

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EraserButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            eraser = null;
            futureErased = new List<PlayerControl>();
            currentTarget = null;
            cooldown = CustomOptionHolder.eraserCooldown.getFloat();
            canEraseAnyone = CustomOptionHolder.eraserCanEraseAnyone.getBool();
            alreadyErased = new List<byte>();
        }
    }
    
    public static class Spy {
        public static PlayerControl spy;
        public static Color color = Palette.ImpostorRed;

        public static bool impostorsCanKillAnyone = true;
        public static bool canEnterVents = false;
        public static bool hasImpostorVision = false;

        public static void clearAndReload() {
            spy = null;
            impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.getBool();
            canEnterVents = CustomOptionHolder.spyCanEnterVents.getBool();
            hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.getBool();
        }
    }

    public static class Trickster {
        public static PlayerControl trickster;
        public static Color color = Palette.ImpostorRed;
        public static float placeBoxCooldown = 30f;
        public static float lightsOutCooldown = 30f;
        public static float lightsOutDuration = 10f;
        public static float lightsOutTimer = 0f;

        private static Sprite placeBoxButtonSprite;
        private static Sprite lightOutButtonSprite;
        private static Sprite tricksterVentButtonSprite;

        public static Sprite getPlaceBoxButtonSprite() {
            if (placeBoxButtonSprite) return placeBoxButtonSprite;
            placeBoxButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlaceJackInTheBoxButton.png", 115f);
            return placeBoxButtonSprite;
        }

        public static Sprite getLightsOutButtonSprite() {
            if (lightOutButtonSprite) return lightOutButtonSprite;
            lightOutButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.LightsOutButton.png", 115f);
            return lightOutButtonSprite;
        }

        public static Sprite getTricksterVentButtonSprite() {
            if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
            tricksterVentButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TricksterVentButton.png", 115f);
            return tricksterVentButtonSprite;
        }

        public static void clearAndReload() {
            trickster = null;
            lightsOutTimer = 0f;
            placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.getFloat();
            lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.getFloat();
            lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.getFloat();
            JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
        }

    }

    public static class Cleaner {
        public static PlayerControl cleaner;
        public static Color color = Palette.ImpostorRed;

        public static float cooldown = 30f;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            cleaner = null;
            cooldown = CustomOptionHolder.cleanerCooldown.getFloat();
        }
    }

    public static class Warlock {

        public static PlayerControl warlock;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl currentTarget;
        public static PlayerControl curseVictim;
        public static PlayerControl curseVictimTarget;

        public static float cooldown = 30f;
        public static float rootTime = 5f;

        private static Sprite curseButtonSprite;
        private static Sprite curseKillButtonSprite;

        public static Sprite getCurseButtonSprite() {
            if (curseButtonSprite) return curseButtonSprite;
            curseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseButton.png", 115f);
            return curseButtonSprite;
        }

        public static Sprite getCurseKillButtonSprite() {
            if (curseKillButtonSprite) return curseKillButtonSprite;
            curseKillButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseKillButton.png", 115f);
            return curseKillButtonSprite;
        }

        public static void clearAndReload() {
            warlock = null;
            currentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
            cooldown = CustomOptionHolder.warlockCooldown.getFloat();
            rootTime = CustomOptionHolder.warlockRootTime.getFloat();
        }

        public static void resetCurse() {
            HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
            HudManagerStartPatch.warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
            HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            currentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
        }
    }

    public static class SecurityGuard {
        public static PlayerControl securityGuard;
        public static Color color = new Color32(195, 178, 95, byte.MaxValue);

        public static float cooldown = 30f;
        public static int remainingScrews = 7;
        public static int totalScrews = 7;
        public static int ventPrice = 1;
        public static int camPrice = 2;
        public static int placedCameras = 0;
        public static float duration = 10f;
        public static int maxCharges = 5;
        public static int rechargeTasksNumber = 3;
        public static int rechargedTasks = 3;
        public static int charges = 1;
        public static bool cantMove = true;
        public static Vent ventTarget = null;
        public static Minigame minigame = null;

        private static Sprite closeVentButtonSprite;
        public static Sprite getCloseVentButtonSprite() {
            if (closeVentButtonSprite) return closeVentButtonSprite;
            closeVentButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CloseVentButton.png", 115f);
            return closeVentButtonSprite;
        }

        private static Sprite placeCameraButtonSprite;
        public static Sprite getPlaceCameraButtonSprite() {
            if (placeCameraButtonSprite) return placeCameraButtonSprite;
            placeCameraButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlaceCameraButton.png", 115f);
            return placeCameraButtonSprite;
        }

        private static Sprite animatedVentSealedSprite;
        private static float lastPPU;
        public static Sprite getAnimatedVentSealedSprite() {
            float ppu = 185f;
            if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
            if (lastPPU != ppu) {
                animatedVentSealedSprite = null;
                lastPPU = ppu;
            }
            if (animatedVentSealedSprite) return animatedVentSealedSprite;
            animatedVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AnimatedVentSealed.png", ppu);
            return animatedVentSealedSprite;
        }

        private static Sprite staticVentSealedSprite;
        public static Sprite getStaticVentSealedSprite() {
            if (staticVentSealedSprite) return staticVentSealedSprite;
            staticVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.StaticVentSealed.png", 160f);
            return staticVentSealedSprite;
        }

        private static Sprite submergedCentralUpperVentSealedSprite;
        public static Sprite getSubmergedCentralUpperSealedSprite() {
            if (submergedCentralUpperVentSealedSprite) return submergedCentralUpperVentSealedSprite;
            submergedCentralUpperVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CentralUpperBlocked.png", 145f);
            return submergedCentralUpperVentSealedSprite;
        }

        private static Sprite submergedCentralLowerVentSealedSprite;
        public static Sprite getSubmergedCentralLowerSealedSprite() {
            if (submergedCentralLowerVentSealedSprite) return submergedCentralLowerVentSealedSprite;
            submergedCentralLowerVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CentralLowerBlocked.png", 145f);
            return submergedCentralLowerVentSealedSprite;
        }

        private static Sprite camSprite;
        public static Sprite getCamSprite() {
            if (camSprite) return camSprite;
            camSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
            return camSprite;
        }

        private static Sprite logSprite;
        public static Sprite getLogSprite() {
            if (logSprite) return logSprite;
            logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
        }

        public static void clearAndReload() {
            securityGuard = null;
            ventTarget = null;
            minigame = null;
            duration = CustomOptionHolder.securityGuardCamDuration.getFloat();
            maxCharges = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.getFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.getFloat());
            charges = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.getFloat()) /2;
            placedCameras = 0;
            cooldown = CustomOptionHolder.securityGuardCooldown.getFloat();
            totalScrews = remainingScrews = Mathf.RoundToInt(CustomOptionHolder.securityGuardTotalScrews.getFloat());
            camPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardCamPrice.getFloat());
            ventPrice = Mathf.RoundToInt(CustomOptionHolder.securityGuardVentPrice.getFloat());
            cantMove = CustomOptionHolder.securityGuardNoMove.getBool();
        }
    }

    public static class Arsonist {
        public static PlayerControl arsonist;
        public static Color color = new Color32(238, 112, 46, byte.MaxValue);

        public static float cooldown = 30f;
        public static float duration = 3f;
        public static bool triggerArsonistWin = false;

        public static PlayerControl currentTarget;
        public static PlayerControl douseTarget;
        public static List<PlayerControl> dousedPlayers = new List<PlayerControl>();

        private static Sprite douseSprite;
        public static Sprite getDouseSprite() {
            if (douseSprite) return douseSprite;
            douseSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DouseButton.png", 115f);
            return douseSprite;
        }

        private static Sprite igniteSprite;
        public static Sprite getIgniteSprite() {
            if (igniteSprite) return igniteSprite;
            igniteSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.IgniteButton.png", 115f);
            return igniteSprite;
        }

        public static bool dousedEveryoneAlive() {
            return CachedPlayer.AllPlayers.All(x => { return x.PlayerControl == Arsonist.arsonist || x.Data.IsDead || x.Data.Disconnected || Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
        }

        public static void clearAndReload() {
            arsonist = null;
            currentTarget = null;
            douseTarget = null; 
            triggerArsonistWin = false;
            dousedPlayers = new List<PlayerControl>();
            foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values) {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
            cooldown = CustomOptionHolder.arsonistCooldown.getFloat();
            duration = CustomOptionHolder.arsonistDuration.getFloat();
        }
    }

    public static class Guesser {
        public static PlayerControl niceGuesser;
        public static PlayerControl evilGuesser;
        public static Color color = new Color32(255, 255, 0, byte.MaxValue);

        public static int remainingShotsEvilGuesser = 2;
        public static int remainingShotsNiceGuesser = 2;

        public static bool isGuesser (byte playerId) {
            if ((niceGuesser != null && niceGuesser.PlayerId == playerId) || (evilGuesser != null && evilGuesser.PlayerId == playerId)) return true;
            return false;
        }

        public static void clear (byte playerId) {
            if (niceGuesser != null && niceGuesser.PlayerId == playerId) niceGuesser = null;
            else if (evilGuesser != null && evilGuesser.PlayerId == playerId) evilGuesser = null;
        }

        public static int remainingShots(byte playerId, bool shoot = false) {
            int remainingShots = remainingShotsEvilGuesser;
            if (niceGuesser != null && niceGuesser.PlayerId == playerId) {
                remainingShots = remainingShotsNiceGuesser;
                if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
            } else if (shoot) {
                remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
            }
            return remainingShots;
        }

        public static void clearAndReload() {
            niceGuesser = null;
            evilGuesser = null;
            remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
            remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
        }
    }

    public static class BountyHunter {
        public static PlayerControl bountyHunter;
        public static Color color = Palette.ImpostorRed;

        public static Arrow arrow;
        public static float bountyDuration = 30f;
        public static bool showArrow = true;
        public static float bountyKillCooldown = 0f;
        public static float punishmentTime = 15f;
        public static float arrowUpdateIntervall = 10f;

        public static float arrowUpdateTimer = 0f;
        public static float bountyUpdateTimer = 0f;
        public static PlayerControl bounty;
        public static TMPro.TextMeshPro cooldownText;

        public static void clearAndReload() {
            arrow = new Arrow(color);
            bountyHunter = null;
            bounty = null;
            arrowUpdateTimer = 0f;
            bountyUpdateTimer = 0f;
            if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = null;
            if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
            cooldownText = null;
            foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values) {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }


            bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.getFloat();
            bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.getFloat();
            punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.getFloat();
            showArrow = CustomOptionHolder.bountyHunterShowArrow.getBool();
            arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.getFloat();
        }
    }

    public static class Vulture {
        public static PlayerControl vulture;
        public static Color color = new Color32(139, 69, 19, byte.MaxValue);
        public static List<Arrow> localArrows = new List<Arrow>();
        public static float cooldown = 30f;
        public static int vultureNumberToWin = 4;
        public static int eatenBodies = 0;
        public static bool triggerVultureWin = false;
        public static bool canUseVents = true;
        public static bool showArrows = true;
        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VultureButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            vulture = null;
            vultureNumberToWin = Mathf.RoundToInt(CustomOptionHolder.vultureNumberToWin.getFloat());
            eatenBodies = 0;
            cooldown = CustomOptionHolder.vultureCooldown.getFloat();
            triggerVultureWin = false;
            canUseVents = CustomOptionHolder.vultureCanUseVents.getBool();
            showArrows = CustomOptionHolder.vultureShowArrows.getBool();
            if (localArrows != null) {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            localArrows = new List<Arrow>();
        }
    }


    public static class Medium {
        public static PlayerControl medium;
        public static DeadPlayer target;
        public static DeadPlayer soulTarget;
        public static Color color = new Color32(98, 120, 115, byte.MaxValue);
        public static List<Tuple<DeadPlayer, Vector3>> deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public static List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public static List<SpriteRenderer> souls = new List<SpriteRenderer>();
        public static DateTime meetingStartTime = DateTime.UtcNow;

        public static float cooldown = 30f;
        public static float duration = 3f;
        public static bool oneTimeUse = false;
        public static float chanceAdditionalInfo = 0f;

        private static Sprite soulSprite;

        enum SpecialMediumInfo {
            SheriffSuicide,
            ThiefSuicide,
            ActiveLoverDies,
            PassiveLoverSuicide,
            LawyerKilledByClient,
            JackalKillsSidekick,
            ImpostorTeamkill,
            SubmergedO2,
            WarlockSuicide,
            BodyCleaned,
        }

        public static Sprite getSoulSprite() {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
            return soulSprite;
        }

        private static Sprite question;
        public static Sprite getQuestionSprite() {
            if (question) return question;
            question = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MediumButton.png", 115f);
            return question;
        }

        public static void clearAndReload() {
            medium = null;
            target = null;
            soulTarget = null;
            deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            souls = new List<SpriteRenderer>();
            meetingStartTime = DateTime.UtcNow;
            cooldown = CustomOptionHolder.mediumCooldown.getFloat();
            duration = CustomOptionHolder.mediumDuration.getFloat();
            oneTimeUse = CustomOptionHolder.mediumOneTimeUse.getBool();
            chanceAdditionalInfo = CustomOptionHolder.mediumChanceAdditionalInfo.getSelection() / 10f;
        }

        public static string getInfo(PlayerControl target, PlayerControl killer) {
            string msg = "";

            List<SpecialMediumInfo> infos = new List<SpecialMediumInfo>();
            // collect fitting death info types.
            // suicides:
            if (killer == target) {
                if (target == Sheriff.sheriff || target == Sheriff.formerSheriff) infos.Add(SpecialMediumInfo.SheriffSuicide);
                if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
                if (target == Thief.thief) infos.Add(SpecialMediumInfo.ThiefSuicide);
                if (target == Warlock.warlock) infos.Add(SpecialMediumInfo.WarlockSuicide);
            } else {
                if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.ActiveLoverDies);
                if (target.Data.Role.IsImpostor && killer.Data.Role.IsImpostor && Thief.formerThief != killer) infos.Add(SpecialMediumInfo.ImpostorTeamkill);
            }
            if (target == Sidekick.sidekick && (killer == Jackal.jackal || Jackal.formerJackals.Any(x => x.PlayerId == killer.PlayerId))) infos.Add(SpecialMediumInfo.JackalKillsSidekick);
            if (target == Lawyer.lawyer && killer == Lawyer.target) infos.Add(SpecialMediumInfo.LawyerKilledByClient);
            if (Medium.target.wasCleaned) infos.Add(SpecialMediumInfo.BodyCleaned);
            
            if (infos.Count > 0) {
                var selectedInfo = infos[rnd.Next(infos.Count)];
                switch (selectedInfo) {
                    case SpecialMediumInfo.SheriffSuicide:
                        msg = "Yikes, that Sheriff shot backfired.";
                        break;
                    case SpecialMediumInfo.WarlockSuicide:
                        msg = "MAYBE I cursed the person next to me and killed myself. Oops.";
                        break;
                    case SpecialMediumInfo.ThiefSuicide:
                        msg = "I tried to steal the gun from their pocket, but they were just happy to see me.";
                        break;
                    case SpecialMediumInfo.ActiveLoverDies:
                        msg = "I wanted to get out of this toxic relationship anyways.";
                        break;
                    case SpecialMediumInfo.PassiveLoverSuicide:
                        msg = "The love of my life died, thus with a kiss I die.";
                        break;
                    case SpecialMediumInfo.LawyerKilledByClient:
                        msg = "My client killed me. Do I still get paid?";
                        break;
                    case SpecialMediumInfo.JackalKillsSidekick:
                        msg = "First they sidekicked me, then they killed me. At least I don't need to do tasks anymore.";
                        break;
                    case SpecialMediumInfo.ImpostorTeamkill:
                        msg = "I guess they confused me for the Spy, is there even one?";
                        break;
                    case SpecialMediumInfo.BodyCleaned:
                        msg = "Is my dead body some kind of art now or... aaand it's gone.";
                        break;
                }
            } else {
                int randomNumber = rnd.Next(4);
                string typeOfColor = Helpers.isLighterColor(Medium.target.killerIfExisting.Data.DefaultOutfit.ColorId) ? "lighter" : "darker";
                float timeSinceDeath = ((float)(Medium.meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds);
                
                if (randomNumber == 0) msg = "If my role hasn't been saved, there's no " + RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true) + " in the game anymore.";
                else if (randomNumber == 1) msg = "I'm not sure, but I guess a " + typeOfColor + " color killed me.";
                else if (randomNumber == 2) msg = "If I counted correctly, I died " + Math.Round(timeSinceDeath / 1000) + "s before the next meeting started.";
                else msg = "It seems like my killer was the " + RoleInfo.GetRolesString(Medium.target.killerIfExisting, false, false, true, includeHidden: true) + ".";
            }

            if (rnd.NextDouble() < chanceAdditionalInfo) {
                int count = 0;
                string condition = "";
                var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
                switch (rnd.Next(3)) {
                    case 0:
                        count = alivePlayersList.Where(pc => pc.Data.Role.IsImpostor || new List<RoleInfo>() { RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.sheriff, RoleInfo.thief }.Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault())).Count();
                        condition = "killer" + (count == 1 ? "" : "s");
                        break;
                    case 1:
                        count = alivePlayersList.Where(Helpers.roleCanUseVents).Count();
                        condition = "player" + (count == 1 ? "" : "s") + " who can use vents";
                        break;
                    case 2:
                        count = alivePlayersList.Where(pc => Helpers.isNeutral(pc) && pc != Jackal.jackal && pc != Sidekick.sidekick && pc != Thief.thief).Count();
                        condition = "player" + (count == 1 ? "" : "s") + " who " + (count == 1 ? "is" : "are") + " neutral but cannot kill";
                        break;
                    case 3:
                        //count = alivePlayersList.Where(pc =>
                        break;               
                }
                msg += $"\nWhen you asked, {count} " + condition + (count == 1 ? " was" : " were") + " still alive";
            }

            return Medium.target.player.Data.PlayerName + "'s Soul:\n" + msg;
        }
    }

    public static class NekoKabocha
    {
        public static PlayerControl nekoKabocha;
        public static Color color = Palette.ImpostorRed;

        public static bool revengeCrew = true;
        public static bool revengeImpostor = true;
        public static bool revengeNeutral = true;
        public static bool revengeExile = false;

        public static PlayerControl meetingKiller = null;

        public static void meetingKill(PlayerControl player, PlayerControl killer)
        {
            if (NekoKabocha.nekoKabocha != null && player == NekoKabocha.nekoKabocha)
            {
                NekoKabocha.meetingKiller = killer;
            }
        }

        public static void OnDeath(PlayerControl killer = null)
        {
            killer ??= meetingKiller;
            if (killer != null && !killer.Data.IsDead && NekoKabocha.nekoKabocha != killer)
            {
                if ((revengeCrew && (!Helpers.isNeutral(killer) && !killer.Data.Role.IsImpostor)) ||
                    (revengeNeutral && Helpers.isNeutral(killer)) ||
                    (revengeImpostor && killer.Data.Role.IsImpostor))
                {
                    if (meetingKiller == null)
                    {
                        NekoKabocha.nekoKabocha.MurderPlayer(killer);
                    }
                    else
                    {
                        killer.Exiled();
                        if (CachedPlayer.LocalPlayer.PlayerControl == killer)
                            FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(NekoKabocha.nekoKabocha.Data, killer.Data);
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(killer.PlayerId);
                    writer.Write((byte)DeadPlayer.CustomDeathReason.Revenge);
                    writer.Write(NekoKabocha.nekoKabocha.PlayerId);
                    
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    GameHistory.overrideDeathReasonAndKiller(killer, DeadPlayer.CustomDeathReason.Revenge, NekoKabocha.nekoKabocha);
                }
            }
            else if (killer == null && NekoKabocha.revengeExile)
            {
                var candidates = PlayerControl.AllPlayerControls.ToArray().Where(x => x != NekoKabocha.nekoKabocha && !x.Data.IsDead).ToList();
                int targetID = rnd.Next(0, candidates.Count);
                var target = candidates[targetID];

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.NekoKabochaExile, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write((byte)DeadPlayer.CustomDeathReason.Revenge);
                AmongUsClient.Instance.FinishRpcImmediately(writer);                
                //RPCProcedure.nekoKabochaExile(target.PlayerId);

                GameHistory.overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.Revenge, NekoKabocha.nekoKabocha);
            }
            meetingKiller = null;
        }

        public static void clearAndReload()
        {
            nekoKabocha = null;
            meetingKiller = null;
            revengeCrew = CustomOptionHolder.nekoKabochaRevengeCrew.getBool();
            revengeImpostor = CustomOptionHolder.nekoKabochaRevengeImpostor.getBool();
            revengeNeutral = CustomOptionHolder.nekoKabochaRevengeNeutral.getBool();
            revengeExile = CustomOptionHolder.nekoKabochaRevengeExile.getBool();
        }
    }

    public static class Lawyer {
        public static PlayerControl lawyer;
        public static PlayerControl target;
        public static Color color = new Color32(134, 153, 25, byte.MaxValue);
        public static Sprite targetSprite;
        //public static bool triggerProsecutorWin = false;
        //public static bool isProsecutor = false;
        public static bool canCallEmergency = true;
        public static bool targetKnows = true;

        public static float vision = 1f;
        public static bool lawyerTargetKnows = true;
        public static bool lawyerKnowsRole = false;
        public static bool targetCanBeJester = false;
        public static bool targetWasGuessed = false;

        public static Sprite getTargetSprite() {
            if (targetSprite) return targetSprite;
            targetSprite = Helpers.loadSpriteFromResources("", 150f);
            return targetSprite;
        }

        public static void clearAndReload(bool clearTarget = true) {
            lawyer = null;
            if (clearTarget) {
                target = null;
                targetWasGuessed = false;
            }
            //isProsecutor = false;
            //triggerProsecutorWin = false;
            vision = CustomOptionHolder.lawyerVision.getFloat();
            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
            lawyerTargetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
            targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.getBool();
            canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
            targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
        }
    }

    public static class Pursuer {
        public static PlayerControl pursuer;
        public static PlayerControl target;
        public static Color color = Lawyer.color;
        public static List<PlayerControl> blankedList = new List<PlayerControl>();
        public static int blanks = 0;
        public static Sprite blank;
        public static bool notAckedExiled = false;

        public static float cooldown = 30f;
        public static int blanksNumber = 5;

        public static Sprite getTargetSprite() {
            if (blank) return blank;
            blank = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PursuerButton.png", 115f);
            return blank;
        }

        public static void clearAndReload() {
            pursuer = null;
            target = null;
            blankedList = new List<PlayerControl>();
            blanks = 0;
            notAckedExiled = false;

            cooldown = CustomOptionHolder.pursuerCooldown.getFloat();
            blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.getFloat());
        }
    }

    public static class FortuneTeller
    {
        public static PlayerControl fortuneTeller;
        public static Color color = new Color32(175, 198, 241, byte.MaxValue);

        public enum DivineResults
        {
            BlackWhite,
            Team,
            Role,
        }

        public static int numTasks;
        public static DivineResults divineResult;
        public static float duration;
        public static float distance;

        public static bool endGameFlag = false;
        public static bool meetingFlag = false;

        public static Dictionary<byte, float> progress = new Dictionary<byte, float>();
        public static Dictionary<byte, bool> playerStatus = new Dictionary<byte, bool>();
        public static bool divinedFlag = false;
        public static int numUsed = 0;

        public static List<Arrow> arrows = new List<Arrow>();
        public static float updateTimer = 0f;

        public static bool isCompletedNumTasks(PlayerControl p)
        {
            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
            return tasksCompleted >= numTasks;
        }

        public static void setDivinedFlag(PlayerControl player, bool flag)
        {
            if (player == FortuneTeller.fortuneTeller)
            {
                FortuneTeller.divinedFlag = flag;
            }
        }

        public static bool canDivine(byte index)
        {
            bool status = true;
            if (playerStatus.ContainsKey(index))
            {
                status = playerStatus[index];
            }
            return (progress.ContainsKey(index) && progress[index] >= duration) || !status;
        }

        private static TMPro.TMP_Text text;
        public static void fortuneTellerMessage(string message, float duration, Color color)
        {
            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            if (roomTracker != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                gameObject.transform.localScale *= 1.5f;

                text = gameObject.GetComponent<TMPro.TMP_Text>();
                text.text = message;
                text.color = color;

                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
                {
                    if (p == 1f && text != null && text.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(text.gameObject);
                    }
                })));
            }
        }

        public static void divine(PlayerControl p)
        {
            string msg = "";
            Color color = Color.white;

            if (FortuneTeller.divineResult == DivineResults.BlackWhite)
            {
                if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
                {
                    msg = $"{p.name} is a Crewmate";
                    color = Color.white;
                }
                else
                {
                    msg = $"{p.name} is not a Crewmate";
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Team)
            {
                if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
                {
                    msg = $"{p.name} is a Crewmate";
                    color = Color.white;
                }
                else if (Helpers.isNeutral(p))
                {
                    msg = $"{p.name} is a Neutral";
                    color = Color.yellow;
                }
                else
                {
                    msg = $"{p.name} is an Impostor";
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Role)
            {
                msg = $"{p.Data.PlayerName} was The {String.Join(" ", RoleInfo.getRoleInfoForPlayer(p, false).Select(x => Helpers.cs(x.color, x.name)))}";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                fortuneTellerMessage(msg, 7f, color);
            }

            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
            numUsed += 1;

            // 占いを実行したことで発火される処理を他クライアントに通知
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FortuneTellerUsedDivine, Hazel.SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(p.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.fortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
        }

        public static void clearAndReload()
        {
            meetingFlag = true;
            duration = CustomOptionHolder.fortuneTellerDuration.getFloat();
            List<Arrow> arrows = new List<Arrow>();
            numTasks = (int)CustomOptionHolder.fortuneTellerNumTasks.getFloat();
            distance = CustomOptionHolder.fortuneTellerDistance.getFloat();
            divineResult = (DivineResults)CustomOptionHolder.fortuneTellerResults.getSelection();
            fortuneTeller = null;
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatch
        {
            public static void Prefix(IntroCutscene __instance)
            {
                HudManager.Instance.StartCoroutine(Effects.Lerp(16.2f, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        meetingFlag = false;
                    }
                })));
            }
        }
    }

    public static class SerialKiller
    {
        public static PlayerControl serialKiller;
        public static Color color = Palette.ImpostorRed;

        public static float killCooldown = 15f;
        public static float suicideTimer = 40f;
        public static bool resetTimer = true;

        public static bool isCountDown = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
            return buttonSprite;
        }

        public static void suicide()
        {
            byte targetId = PlayerControl.LocalPlayer.PlayerId;
            MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SerialKillerSuicide, Hazel.SendOption.Reliable, -1);
            killWriter.Write(targetId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.serialKillerSuicide(targetId);
        }

        public static void OnKill(PlayerControl target)
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == SerialKiller.serialKiller)
                SerialKiller.serialKiller.SetKillTimer(killCooldown);

            //serialKillerButton.Timer = suicideTimer;
            HudManagerStartPatch.serialKillerButton.Timer = suicideTimer;
            isCountDown = true;
        }

        public static void clearAndReload()
        {
            serialKiller = null;
            killCooldown = CustomOptionHolder.serialKillerKillCooldown.getFloat();
            suicideTimer = Mathf.Max(CustomOptionHolder.serialKillerSuicideTimer.getFloat(), killCooldown + 2.5f);
            resetTimer = CustomOptionHolder.serialKillerResetTimer.getBool();
            isCountDown = false;
        }
    }

    public static class Shifter
    {
        public static PlayerControl shifter;
        public static Color color = new Color32(102, 102, 102, byte.MaxValue);

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;
        public static bool shiftModifiers = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            shifter = null;
            currentTarget = null;
            futureShift = null;
            shiftModifiers = CustomOptionHolder.shifterShiftsModifiers.getBool();
        }
    }

    public static class Witch {
        public static PlayerControl witch;
        public static Color color = Palette.ImpostorRed;

        public static List<PlayerControl> futureSpelled = new List<PlayerControl>();
        public static PlayerControl currentTarget;
        public static PlayerControl spellCastingTarget;
        public static float cooldown = 30f;
        public static float spellCastingDuration = 2f;
        public static float cooldownAddition = 10f;
        public static float currentCooldownAddition = 0f;
        public static bool canSpellAnyone = false;
        public static bool triggerBothCooldowns = true;
        public static bool witchVoteSavesTargets = true;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite spelledOverlaySprite;
        public static Sprite getSpelledOverlaySprite() {
            if (spelledOverlaySprite) return spelledOverlaySprite;
            spelledOverlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButtonMeeting.png", 225f);
            return spelledOverlaySprite;
        }


        public static void clearAndReload() {
            witch = null;
            futureSpelled = new List<PlayerControl>();
            currentTarget = spellCastingTarget = null;
            cooldown = CustomOptionHolder.witchCooldown.getFloat();
            cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.getFloat();
            currentCooldownAddition = 0f;
            canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.getBool();
            spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.getFloat();
            triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.getBool();
            witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.getBool();
        }
    }

    public static class Watcher
    {
        public static PlayerControl nicewatcher;
        public static PlayerControl evilwatcher;
        public static Color color = Palette.Purple;

        public static void clear(byte playerId)
        {
            if (nicewatcher != null && nicewatcher.PlayerId == playerId) nicewatcher = null;
            else if (evilwatcher != null &&  evilwatcher.PlayerId == playerId) evilwatcher = null;
        }

        public static void clearAndReload()
        {
            nicewatcher = null;
            evilwatcher = null;
        }
    }

    public static class Bait
    {
        public static PlayerControl bait;
        public static Color color = new Color32(0, 247, 255, byte.MaxValue);

        public static bool highlightAllVents = false;
        public static float reportDelay = 0f;
        public static bool showKillFlash = true;

        public static bool reported = false;

        public static void clearAndReload()
        {
            bait = null;
            reported = false;
            highlightAllVents = CustomOptionHolder.baitHighlightAllVents.getBool();
            reportDelay = CustomOptionHolder.baitReportDelay.getFloat();
            showKillFlash = CustomOptionHolder.baitShowKillFlash.getBool();
        }
    }

    public static class Assassin {
        public static PlayerControl assassin;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl assassinMarked;
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static float traceTime = 1f;
        public static bool knowsTargetLocation = false;
        //public static float invisibleDuration = 5f;

        //public static float invisibleTimer = 0f;
        //public static bool isInvisble = false;
        private static Sprite markButtonSprite;
        private static Sprite killButtonSprite;
        public static Arrow arrow = new Arrow(Color.black);
        public static Sprite getMarkButtonSprite() {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AssassinMarkButton.png", 115f);
            return markButtonSprite;
        }

        public static Sprite getKillButtonSprite() {
            if (killButtonSprite) return killButtonSprite;
            killButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AssassinAssassinateButton.png", 115f);
            return killButtonSprite;
        }

        public static void clearAndReload() {
            assassin = null;
            currentTarget = assassinMarked = null;
            cooldown = CustomOptionHolder.assassinCooldown.getFloat();
            knowsTargetLocation = CustomOptionHolder.assassinKnowsTargetLocation.getBool();
            traceTime = CustomOptionHolder.assassinTraceTime.getFloat();
            //invisibleDuration = CustomOptionHolder.assassinInvisibleDuration.getFloat();
            //invisibleTimer = 0f;
            //isInvisble = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.black);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }
    }

    public static class Opportunist
    {
        public static PlayerControl opportunist;
        public static Color color = new Color32(0, 255, 00, byte.MaxValue);

        public static void clearAndReload()
        {
            opportunist = null;
        }
    }

    public static class Ninja
    {
        public static PlayerControl ninja;
        public static Color color = Palette.ImpostorRed;
        public static float stealthCooldown = 30f;
        public static float stealthDuration = 15f;
        public static float killPenalty = 10f;
        public static float speedBonus = 1.25f;
        public static float fadeTime = 0.5f;
        public static bool canUseVents = false;
        public static bool canBeTargeted;
        public static float addition = 0f;

        public static float invisibleTimer = 0f;

        public static bool penalized = false;
        public static bool stealthed = false;
        public static DateTime stealthedAt = DateTime.UtcNow;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NinjaButton.png", 115f);
            return buttonSprite;
        }

        public static bool isStealthed(PlayerControl player)
        {
            if (Ninja.ninja != null && !Ninja.ninja.Data.IsDead && Ninja.ninja == player)
            {
                return Ninja.stealthed;
            }
            return false;
        }

        public static float stealthFade(PlayerControl player)
        {
            if (Ninja.ninja == player && fadeTime > 0 && !Ninja.ninja.Data.IsDead)
            {
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - Ninja.stealthedAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static bool isPenalized(PlayerControl player)
        {
            if (Ninja.ninja == player && !Ninja.ninja.Data.IsDead)
            {
                return Ninja.penalized;
            }
            return false;
        }

        public static void setStealthed(PlayerControl player, bool stealthed = true)
        {
            if (Ninja.ninja == player && Ninja.ninja != null)
            {
                Ninja.stealthed = stealthed;
                Ninja.stealthedAt = DateTime.UtcNow;
            }
        }

        public static void OnKill(PlayerControl target)
        {
            if (Ninja.stealthed)
            {
                Ninja.addition += Ninja.killPenalty;
                if (CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja)
                {
                    Ninja.penalized = true;
                    CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Ninja.addition);
                    Helpers.checkMurderAttemptAndKill(Ninja.ninja, target, false, false);
                }
            }
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (player.MyPhysics?.myPlayer.cosmetics.currentBodySprite.BodySprite != null)
                {
                    player.MyPhysics.myPlayer.cosmetics.currentBodySprite.BodySprite.color = color;
                }

                if (player.MyPhysics?.myPlayer.cosmetics.skin?.layer != null)
                    player.MyPhysics.myPlayer.cosmetics.skin.layer.color = color;

                if (player.cosmetics.hat != null)
                    player.cosmetics.hat.SpriteColor = color;

                if (player.cosmetics.currentPet?.rend != null)
                    player.cosmetics.currentPet.rend.color = color;

                if (player.cosmetics.currentPet?.shadowRend != null)
                    player.cosmetics.currentPet.shadowRend.color = color;

                if (player.cosmetics.visor != null)
                    player.cosmetics.visor.Image.color = color;

                if (player.cosmetics.colorBlindText != null)
                    player.cosmetics.colorBlindText.color = color;
            }
            catch { }
        }

        public static void clearAndReload()
        {
            ninja = null;
            stealthCooldown = CustomOptionHolder.ninjaStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.ninjaStealthDuration.getFloat();
            killPenalty = CustomOptionHolder.ninjaKillPenalty.getFloat();
            speedBonus = CustomOptionHolder.ninjaSpeedBonus.getFloat();
            fadeTime = CustomOptionHolder.ninjaFadeTime.getFloat();
            canUseVents = CustomOptionHolder.ninjaCanVent.getBool();
            canBeTargeted = CustomOptionHolder.ninjaCanBeTargeted.getBool();

            setOpacity(Ninja.ninja, 1.0f);

            penalized = false;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsNinjaPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.AmOwner && __instance.myPlayer.CanMove && GameData.Instance && isStealthed(__instance.myPlayer))
                {
                    __instance.body.velocity *= (speedBonus + 1);
                }

                if (__instance.myPlayer == Ninja.ninja)
                {
                    var ninja = __instance.myPlayer;
                    if (ninja == null || ninja.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                        (Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter);

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (isStealthed(ninja))
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade(ninja));
                        ninja.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade(ninja));
                    }

                    setOpacity(ninja, opacity);
                }
            }
        }
    }

    public static class Sprinter
    {
        public static PlayerControl sprinter;
        public static Color color = new Color32(128, 128, 255, byte.MaxValue);

        public static float sprintCooldown = 30f;
        public static float sprintDuration = 15f;
        public static float fadeTime = 0.5f;
        public static float invisibleTimer = 0f;

        public static bool sprinting = false;

        public static DateTime sprintAt = DateTime.UtcNow;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SprintButton.png", 115f);
            return buttonSprite;
        }

        public static float sprintFade(PlayerControl player)
        {
            if (Sprinter.sprinter == player && fadeTime > 0 && !Sprinter.sprinter.Data.IsDead)
            {
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - Sprinter.sprintAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static bool isSprinting()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter && !Sprinter.sprinter.Data.IsDead)
            {
                return Sprinter.sprinting;
            }
            return false;
        }

        public static void setSprinting(PlayerControl player, bool sprinting = true)
        {
            if (player == Sprinter.sprinter && !Sprinter.sprinter.Data.IsDead)
            {
                Sprinter.sprinting = sprinting;
                Sprinter.sprintAt = DateTime.UtcNow;
            }
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (player.MyPhysics?.myPlayer.cosmetics.currentBodySprite.BodySprite != null)
                {
                    player.MyPhysics.myPlayer.cosmetics.currentBodySprite.BodySprite.color = color;
                }

                if (player.MyPhysics?.myPlayer.cosmetics.skin?.layer != null)
                    player.MyPhysics.myPlayer.cosmetics.skin.layer.color = color;

                if (player.cosmetics.hat != null)
                    player.cosmetics.hat.SpriteColor = color;

                if (player.cosmetics.currentPet?.rend != null)
                    player.cosmetics.currentPet.rend.color = color;

                if (player.cosmetics.currentPet?.shadowRend != null)
                    player.cosmetics.currentPet.shadowRend.color = color;

                if (player.cosmetics.visor != null)
                    player.cosmetics.visor.Image.color = color;

                if (player.cosmetics.colorBlindText != null)
                    player.cosmetics.colorBlindText.color = color;
            }
            catch { }
        }

        public static void clearAndReload()
        {
            sprinter = null;
            sprinting = false;
            sprintCooldown = CustomOptionHolder.sprinterCooldown.getFloat();
            sprintDuration = CustomOptionHolder.sprinterDuration.getFloat();
            fadeTime = CustomOptionHolder.sprinterFadeTime.getFloat();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsSprinterPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.myPlayer == Sprinter.sprinter)
                {
                    var sprinter = __instance.myPlayer;
                    if (sprinter == null || sprinter.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter ||
                        (Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter);

                    var opacity = canSee ? 0.1f : 0.0f;

                    if (Sprinter.sprinting)
                    {
                        opacity = Math.Max(opacity, 1.0f - sprintFade(sprinter));
                        sprinter.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, sprintFade(sprinter));
                    }

                    setOpacity(sprinter, opacity);
                }
            }
        }
    }

    public static class Thief {
        public static PlayerControl thief;
        public static Color color = new Color32(71, 99, 45, Byte.MaxValue);
        public static PlayerControl currentTarget;
        public static PlayerControl formerThief;

        public static float cooldown = 30f;

        public static bool suicideFlag = false;  // Used as a flag for suicide

        public static bool hasImpostorVision;
        public static bool canUseVents;
        public static bool canKillSheriff;
        public static bool canStealWithGuess;

        public static void clearAndReload() {
            thief = null;
            suicideFlag = false;
            currentTarget = null;
            formerThief = null;
            hasImpostorVision = CustomOptionHolder.thiefHasImpVision.getBool();
            cooldown = CustomOptionHolder.thiefCooldown.getFloat();
            canUseVents = CustomOptionHolder.thiefCanUseVents.getBool();
            canKillSheriff = CustomOptionHolder.thiefCanKillSheriff.getBool();
            canStealWithGuess = CustomOptionHolder.thiefCanStealWithGuess.getBool();
        }

        public static bool isFailedThiefKill(PlayerControl target, PlayerControl killer, RoleInfo targetRole) {
            return killer == Thief.thief && !target.Data.Role.IsImpostor && !new List<RoleInfo> { RoleInfo.jackal, canKillSheriff ? RoleInfo.sheriff : null, RoleInfo.sidekick }.Contains(targetRole);
        }
    }

        /*public static class Trapper {
        public static PlayerControl trapper;
        public static Color color = new Color32(110, 57, 105, byte.MaxValue);

        public static float cooldown = 30f;
        public static int maxCharges = 5;
        public static int rechargeTasksNumber = 3;
        public static int rechargedTasks = 3;
        public static int charges = 1;
        public static int trapCountToReveal = 2;
        public static List<PlayerControl> playersOnMap = new List<PlayerControl>();
        public static bool anonymousMap = false;
        public static int infoType = 0; // 0 = Role, 1 = Good/Evil, 2 = Name
        public static float trapDuration = 5f; 

        private static Sprite trapButtonSprite;

        public static Sprite getButtonSprite() {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Trapper_Place_Button.png", 115f);
            return trapButtonSprite;
        }

        public static void clearAndReload() {
            trapper = null;
            cooldown = CustomOptionHolder.trapperCooldown.getFloat();
            maxCharges = Mathf.RoundToInt(CustomOptionHolder.trapperMaxCharges.getFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.trapperRechargeTasksNumber.getFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.trapperRechargeTasksNumber.getFloat());
            charges = Mathf.RoundToInt(CustomOptionHolder.trapperMaxCharges.getFloat()) / 2;
            trapCountToReveal = Mathf.RoundToInt(CustomOptionHolder.trapperTrapNeededTriggerToReveal.getFloat());
            playersOnMap = new List<PlayerControl>();
            anonymousMap = CustomOptionHolder.trapperAnonymousMap.getBool();
            infoType = CustomOptionHolder.trapperInfoType.getSelection();
            trapDuration = CustomOptionHolder.trapperTrapDuration.getFloat();
        }
    }*/

    /*public static class Bomber {
        public static PlayerControl bomber = null;
        public static Color color = Palette.ImpostorRed;

        public static Bomb bomb = null;
        public static bool isPlanted = false;
        public static bool isActive = false;
        public static float destructionTime = 20f;
        public static float destructionRange = 2f;
        public static float hearRange = 30f;
        public static float defuseDuration = 3f;
        public static float bombCooldown = 15f;
        public static float bombActiveAfter = 3f;

        private static Sprite buttonSprite;

        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Bomb_Button_Plant.png", 115f);
            return buttonSprite;
        }

        public static void clearBomb(bool flag = true) {
            if (bomb != null) {
                UnityEngine.Object.Destroy(bomb.bomb);
                UnityEngine.Object.Destroy(bomb.background);
                bomb = null;
            }
            isPlanted = false;
            isActive = false;
            if (flag) SoundEffectsManager.stop("bombFuseBurning");
        }

        public static void clearAndReload() {
            clearBomb(false);
            bomber = null;
            bomb = null;
            isPlanted = false;
            isActive = false;
            destructionTime = CustomOptionHolder.bomberBombDestructionTime.getFloat();
            destructionRange = CustomOptionHolder.bomberBombDestructionRange.getFloat() / 10;
            hearRange = CustomOptionHolder.bomberBombHearRange.getFloat() / 10;
            defuseDuration = CustomOptionHolder.bomberDefuseDuration.getFloat();
            bombCooldown = CustomOptionHolder.bomberBombCooldown.getFloat();
            bombActiveAfter = CustomOptionHolder.bomberBombActiveAfter.getFloat();
            Bomb.clearBackgroundSprite();
        }
    }*/

    // Modifier
    /*public static class Bait {
        public static List<PlayerControl> bait = new List<PlayerControl>();
        public static Dictionary<DeadPlayer, float> active = new Dictionary<DeadPlayer, float>();
        public static Color color = new Color32(0, 247, 255, byte.MaxValue);

        public static float reportDelayMin = 0f;
        public static float reportDelayMax = 0f;
        public static bool showKillFlash = true;

        public static void clearAndReload() {
            bait = new List<PlayerControl>();
            active = new Dictionary<DeadPlayer, float>();
            reportDelayMin = CustomOptionHolder.modifierBaitReportDelayMin.getFloat();
            reportDelayMax = CustomOptionHolder.modifierBaitReportDelayMax.getFloat();
            if (reportDelayMin > reportDelayMax) reportDelayMin = reportDelayMax;
            showKillFlash = CustomOptionHolder.modifierBaitShowKillFlash.getBool();
        }
    }*/

    public static class Bloody {
        public static List<PlayerControl> bloody = new List<PlayerControl>();
        public static Dictionary<byte, float> active = new Dictionary<byte, float>();
        public static Dictionary<byte, byte> bloodyKillerMap = new Dictionary<byte, byte>();

        public static float duration = 5f;

        public static void clearAndReload() {
            bloody = new List<PlayerControl>();
            active = new Dictionary<byte, float>();
            bloodyKillerMap = new Dictionary<byte, byte>();
            duration = CustomOptionHolder.modifierBloodyDuration.getFloat();
        }
    }

    public static class AntiTeleport {
        public static List<PlayerControl> antiTeleport = new List<PlayerControl>();
        public static Vector3 position;

        public static void clearAndReload() {
            antiTeleport = new List<PlayerControl>();
            position = Vector3.zero;
        }

        public static void setPosition() {
            if (position == Vector3.zero) return;  // Check if this has been set, otherwise first spawn on submerged will fail
            if (antiTeleport.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0) {
                CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(position);
                if (SubmergedCompatibility.IsSubmerged) {
                    SubmergedCompatibility.ChangeFloor(position.y > -7);
                }
            }
        }
    }

    public static class Tiebreaker {
        public static PlayerControl tiebreaker;

        public static bool isTiebreak = false;

        public static void clearAndReload() {
            tiebreaker = null;
            isTiebreak = false;
        }
    }

    public static class Sunglasses {
        public static List<PlayerControl> sunglasses = new List<PlayerControl>();
        public static int vision = 1;

        public static void clearAndReload() {
            sunglasses = new List<PlayerControl>();
            vision = CustomOptionHolder.modifierSunglassesVision.getSelection() + 1;
        }
    }
    public static class Mini {
        public static PlayerControl mini;
        public static Color color = Color.yellow;
        public const float defaultColliderRadius = 0.2233912f;
        public const float defaultColliderOffset = 0.3636057f;

        public static float growingUpDuration = 400f;
        public static bool isGrowingUpInMeeting = true;
        public static DateTime timeOfGrowthStart = DateTime.UtcNow;
        public static DateTime timeOfMeetingStart = DateTime.UtcNow;
        public static float ageOnMeetingStart = 0f;
        public static bool triggerMiniLose = false;

        public static void clearAndReload() {
            mini = null;
            triggerMiniLose = false;
            growingUpDuration = CustomOptionHolder.modifierMiniGrowingUpDuration.getFloat();
            isGrowingUpInMeeting = CustomOptionHolder.modifierMiniGrowingUpInMeeting.getBool();
            timeOfGrowthStart = DateTime.UtcNow;
        }

        public static float growingProgress() {
            float timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
            return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
        }

        public static bool isGrownUp() {
            return growingProgress() == 1f;
        }

    }
    public static class Vip {
        public static List<PlayerControl> vip = new List<PlayerControl>();
        public static bool showColor = true;

        public static void clearAndReload() {
            vip = new List<PlayerControl>();
            showColor = CustomOptionHolder.modifierVipShowColor.getBool();
        }
    }

    public static class Invert {
        public static List<PlayerControl> invert = new List<PlayerControl>();
        public static int meetings = 3;

        public static void clearAndReload() {
            invert = new List<PlayerControl>();
            meetings = (int) CustomOptionHolder.modifierInvertDuration.getFloat();
        }
    }

    public static class Chameleon {
        public static List<PlayerControl> chameleon = new List<PlayerControl>();
        public static float minVisibility = 0.2f;
        public static float holdDuration = 1f;
        public static float fadeDuration = 0.5f;
        public static Dictionary<byte, float> lastMoved;

        public static void clearAndReload() {
            chameleon = new List<PlayerControl>();
            lastMoved = new Dictionary<byte, float>();
            holdDuration = CustomOptionHolder.modifierChameleonHoldDuration.getFloat();
            fadeDuration = CustomOptionHolder.modifierChameleonFadeDuration.getFloat();
            minVisibility = CustomOptionHolder.modifierChameleonMinVisibility.getSelection() / 10f;
        }

        public static float visibility(byte playerId) {
            float visibility = 1f;
            if (lastMoved != null && lastMoved.ContainsKey(playerId)) {
                var tStill = Time.time - lastMoved[playerId];
                if (tStill > holdDuration) {
                    if (tStill - holdDuration > fadeDuration) visibility = minVisibility;
                    else visibility = (1 - (tStill - holdDuration) / fadeDuration) * (1 - minVisibility) + minVisibility;
                }
            }
            if (PlayerControl.LocalPlayer.Data.IsDead && visibility < 0.1f) {  // Ghosts can always see!
                visibility = 0.1f;
            }
            return visibility;
        }

        public static void update() {
            foreach (var chameleonPlayer in chameleon) {
                //if (chameleonPlayer == Assassin.assassin && Assassin.isInvisble) continue;  // Dont make Assassin visible...
                // check movement by animation
                PlayerPhysics playerPhysics = chameleonPlayer.MyPhysics;
                var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
                if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim) {
                    lastMoved[chameleonPlayer.PlayerId] = Time.time;
                }
                // calculate and set visibility
                float visibility = Chameleon.visibility(chameleonPlayer.PlayerId);
                float petVisibility = visibility;
                if (chameleonPlayer.Data.IsDead) {
                    visibility = 0.5f;
                    petVisibility = 1f;
                }

                try {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                    chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color = chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
                    if (DataManager.Settings.Accessibility.ColorBlindMode) chameleonPlayer.cosmetics.colorBlindText.color = chameleonPlayer.cosmetics.colorBlindText.color.SetAlpha(visibility);
                    chameleonPlayer.SetHatAndVisorAlpha(visibility);
                    chameleonPlayer.cosmetics.skin.layer.color = chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility);
                    chameleonPlayer.cosmetics.nameText.color = chameleonPlayer.cosmetics.nameText.color.SetAlpha(visibility);
                    chameleonPlayer.cosmetics.currentPet.rend.color = chameleonPlayer.cosmetics.currentPet.rend.color.SetAlpha(petVisibility);
                    chameleonPlayer.cosmetics.currentPet.shadowRend.color = chameleonPlayer.cosmetics.currentPet.shadowRend.color.SetAlpha(petVisibility);
                } catch { }
            }
                
        }
    }

    /*public static class Shifter {
        public static PlayerControl shifter;

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static void shiftRole (PlayerControl player1, PlayerControl player2, bool repeat = true) {
            if (Mayor.mayor != null && Mayor.mayor == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Mayor.mayor = player1;
            } else if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Portalmaker.portalmaker = player1;
            } else if (Engineer.engineer != null && Engineer.engineer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Engineer.engineer = player1;
            } else if (Sheriff.sheriff != null && Sheriff.sheriff == player2) {
                if (repeat) shiftRole(player2, player1, false);
                if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff) Sheriff.formerDeputy = player1;  // Shifter also shifts info on promoted deputy (to get handcuffs)
                Sheriff.sheriff = player1;
            } else if (Deputy.deputy != null && Deputy.deputy == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Deputy.deputy = player1;
            } else if (Lighter.lighter != null && Lighter.lighter == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Lighter.lighter = player1;
            } else if (Detective.detective != null && Detective.detective == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Detective.detective = player1;
            } else if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player2) {
                if (repeat) shiftRole(player2, player1, false);
                TimeMaster.timeMaster = player1;
            }  else if (Medic.medic != null && Medic.medic == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Medic.medic = player1;
            } else if (Swapper.swapper != null && Swapper.swapper == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Swapper.swapper = player1;
            } else if (Seer.seer != null && Seer.seer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Seer.seer = player1;
            } else if (Hacker.hacker != null && Hacker.hacker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Hacker.hacker = player1;
            } else if (Tracker.tracker != null && Tracker.tracker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Tracker.tracker = player1;
            } else if (Snitch.snitch != null && Snitch.snitch == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Snitch.snitch = player1;
            } else if (Spy.spy != null && Spy.spy == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Spy.spy = player1;
            } else if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player2) {
                if (repeat) shiftRole(player2, player1, false);
                SecurityGuard.securityGuard = player1;
            } else if (Guesser.niceGuesser != null && Guesser.niceGuesser == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Guesser.niceGuesser = player1;
            } else if (Medium.medium != null && Medium.medium == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Medium.medium = player1;
            } else if (Pursuer.pursuer != null && Pursuer.pursuer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Pursuer.pursuer = player1;
            } //else if (Trapper.trapper != null && Trapper.trapper == player2) {
                //if (repeat) shiftRole(player2, player1, false);
                //Trapper.trapper = player1;
            //}
        }

        public static void clearAndReload() {
            shifter = null;
            currentTarget = null;
            futureShift = null;
        }
    }*/
}
