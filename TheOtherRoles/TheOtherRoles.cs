using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using static TheOtherRoles.TheOtherRoles;
using AmongUs.Data;
using Hazel;
using TheOtherRoles.Patches;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Modules;
using AmongUs.GameOptions;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Reactor.Utilities;

namespace TheOtherRoles
{
    [HarmonyPatch]
    public static class TheOtherRoles
    {
        public static System.Random rnd = new((int)DateTime.Now.Ticks);

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
            //Trapper.clearAndReload();
            //Bomber.clearAndReload();

            // GMIA
            Ninja.clearAndReload();
            NekoKabocha.clearAndReload();
            SerialKiller.clearAndReload();
            EvilTracker.clearAndReload();
            Undertaker.clearAndReload();
            MimicK.clearAndReload();
            MimicA.clearAndReload();
            BomberA.clearAndReload();
            BomberB.clearAndReload();
            EvilHacker.clearAndReload();
            Trapper.clearAndReload();
            Blackmailer.clearAndReload();
            Yoyo.clearAndReload();
            FortuneTeller.clearAndReload();
            Sprinter.clearAndReload();
            Veteran.clearAndReload();
            Sherlock.clearAndReload();
            TaskMaster.clearAndReload();
            Yasuna.clearAndReload();
            Prophet.clearAndReload();
            Madmate.clearAndReload();
            CreatedMadmate.clearAndReload();
            Teleporter.clearAndReload();
            Busker.clearAndReload();
            Noisemaker.clearAndReload();
            Watcher.clearAndReload();
            Opportunist.clearAndReload();
            Moriarty.clearAndReload();
            Akujo.clearAndReload();
            PlagueDoctor.clearAndReload();
            JekyllAndHyde.clearAndReload();
            Cupid.clearAndReload();
            Fox.clearAndReload();
            Immoralist.clearAndReload();
            SchrodingersCat.clearAndReload();
            Kataomoi.clearAndReload();
            Husk.clearAndReload();

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
            Armored.clearAndReload();

            // Gamemodes
            HandleGuesser.clearAndReload();
            HideNSeek.clearAndReload();
            FreePlayGM.clearAndReload();
        }

        public static class Jester {
            public static PlayerControl jester;
            public static Color color = new Color32(236, 98, 165, byte.MaxValue);

            public static bool triggerJesterWin = false;
            public static bool canCallEmergency = true;
            public static bool hasImpostorVision = false;
            public static bool canUseVents = false;

            public static void unlockAch()
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockJesterAcCommon, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unlockJesterAcCommon();
            }

            public static void clearAndReload() {
                jester = null;
                triggerJesterWin = false;
                canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
                hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.getBool();
                canUseVents = CustomOptionHolder.jesterCanVent.getBool();
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
            public static AchievementToken<(int portal, bool cleared)> acTokenChallenge = null;

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

            public static void onAchievementActivate()
            {
                if (portalmaker == null || PlayerControl.LocalPlayer != portalmaker) return;
                acTokenChallenge ??= new("portalmaker.challenge", (0, false), (val, _) => val.cleared);
            }

            public static void clearAndReload() {
                portalmaker = null;
                cooldown = CustomOptionHolder.portalmakerCooldown.getFloat();
                usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.getFloat();
                logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.getBool();
                logShowsTime = CustomOptionHolder.portalmakerLogHasTime.getBool();
                canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.getBool();
                acTokenChallenge = null;
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
            public static AchievementToken<(bool doubleVote, bool cleared)> acTokenDoubleVote = null;
            public static AchievementToken<(byte votedFor, bool doubleVote, bool cleared)> acTokenChallenge = null;

            public static Sprite getMeetingSprite()
            {
                if (emergencySprite) return emergencySprite;
                emergencySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
                return emergencySprite;
            }

            public static void onAchievementActivate()
            {
                if (mayor == null || PlayerControl.LocalPlayer != mayor) return;
                acTokenDoubleVote ??= new("mayor.common1", (false, false), (val, _) => val.cleared);
                acTokenChallenge ??= new("mayor.challenge", (byte.MaxValue, false, false), (val, _) => val.cleared);
            }

            public static void unlockAch(byte votedFor)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockMayorAcCommon, SendOption.Reliable, -1);
                writer.Write(votedFor);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unlockMayorAcCommon(votedFor);
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
                acTokenDoubleVote = null;
                acTokenChallenge = null;
            }
        }

        public static class Engineer {
            public static PlayerControl engineer;
            public static Color color = new Color32(0, 40, 245, byte.MaxValue);
            private static Sprite buttonSprite;

            public static int remainingFixes = 1;           
            public static bool highlightForImpostors = true;
            public static bool highlightForTeamJackal = true;

            public static AchievementToken<(bool inVent, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (engineer == null || PlayerControl.LocalPlayer != engineer) return;
                acTokenChallenge ??= new("engineer.another1", (false, false), (val, _) => val.cleared);
            }

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
                acTokenChallenge = null;
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

            public static AchievementToken<(bool isTriggeredFalse, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (sheriff == null || PlayerControl.LocalPlayer != sheriff) return;
                acTokenChallenge ??= new("sheriff.challenge", (true, true), (val, _) => val.cleared && !val.isTriggeredFalse);
            }

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
                acTokenChallenge = null;
            }
        }

        public static class Deputy
        {
            public static PlayerControl deputy;
            public static Color color = Sheriff.color;

            public static PlayerControl currentTarget;
            public static List<byte> handcuffedPlayers = new();
            public static int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
            public static bool keepsHandcuffsOnPromotion;
            public static float handcuffDuration;
            public static float remainingHandcuffs;
            public static float handcuffCooldown;
            public static bool knowsSheriff;
            public static bool stopsGameEnd;
            public static Dictionary<byte, float> handcuffedKnows = new();

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
                    playerId = PlayerControl.LocalPlayer.PlayerId;

                if (active && playerId == PlayerControl.LocalPlayer.PlayerId) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (active) {
                    handcuffedKnows.Add(playerId, handcuffDuration);
                    handcuffedPlayers.RemoveAll(x => x == playerId);
               }

                if (playerId == PlayerControl.LocalPlayer.PlayerId) {
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
                stopsGameEnd = CustomOptionHolder.deputyStopsGameEnd.getBool();
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

            public static AchievementToken<bool> acTokenCommon = null;
            public static AchievementToken<(bool reported, byte votedFor, byte killerId, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (detective == null || PlayerControl.LocalPlayer != detective) return;
                acTokenChallenge ??= new("detective.challenge", (false, byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
                acTokenCommon ??= new("detective.common1", false, (val, _) => val);
            }

            public static void unlockAch(byte votedFor)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockDetectiveAcChallenge, SendOption.Reliable, -1);
                writer.Write(votedFor);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unlockDetectiveAcChallenge(votedFor);
            }

            public static void clearAndReload() {
                detective = null;
                anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
                footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
                footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
                reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
                reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
                timer = 6.2f;
                acTokenCommon = null;
                acTokenChallenge = null;
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
        public static AchievementToken<(byte killerId, bool cleared)> acTokenChallenge = null;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShieldButton.png", 115f);
            return buttonSprite;
        }

        public static void onAchievementActivate()
        {
            if (medic == null || PlayerControl.LocalPlayer != medic) return;
            acTokenChallenge ??= new("medic.challenge", (byte.MaxValue, false), (val, _) => val.cleared);
        }

        public static bool shieldVisible(PlayerControl target)
        {
            bool hasVisibleShield = false;

            bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            bool isMimicKShield = target == MimicK.mimicK && MimicK.victim != null;
            bool isMimicAMorph = target == MimicA.mimicA && MimicA.isMorph;
            if (shielded != null && ((target == shielded && !isMorphedMorphling && !isMimicKShield && !isMimicAMorph) || (isMorphedMorphling && Morphling.morphTarget == shielded) || (isMimicAMorph && MimicK.mimicK == shielded)))
            {
                hasVisibleShield = showShielded == 0 || Helpers.shouldShowGhostInfo() // Everyone or Ghost info
                    || (showShielded == 1 && (PlayerControl.LocalPlayer == shielded || PlayerControl.LocalPlayer == medic)) // Shielded + Medic
                    || (showShielded == 2 && PlayerControl.LocalPlayer == medic); // Medic only
                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (meetingAfterShielding || !showShieldAfterMeeting || PlayerControl.LocalPlayer == medic || Helpers.shouldShowGhostInfo());
            }
            return hasVisibleShield;
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
            acTokenChallenge = null;
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

        public static AchievementToken<(byte swapped1, byte swapped2, bool cleared)> acTokenChallenge = null;
        public static AchievementToken<(byte swapped1, byte swapped2, bool cleared)> evilSwapperAcTokenChallenge;

        public static void niceSwapperOnAchievementActivate()
        {
            if (swapper == null || PlayerControl.LocalPlayer != swapper) return;
            acTokenChallenge ??= new("niceSwapper.challenge", (byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
        }

        public static void evilSwapperOnAchievementActivate()
        {
            if (swapper != null && PlayerControl.LocalPlayer == swapper) {
                evilSwapperAcTokenChallenge ??= new("evilSwapper.challenge", (byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
            }
        }

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
            acTokenChallenge = null;
            evilSwapperAcTokenChallenge = null;
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
            if (!existingAndAlive() || !existingWithKiller())
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
        public static List<Vector3> deadBodyPositions = new();

        public static float soulDuration = 15f;
        public static bool limitSoulDuration = false;
        public static int mode = 0;

        public static AchievementToken<(byte flash, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (seer == null || PlayerControl.LocalPlayer != seer) return;
            acTokenChallenge ??= new("seer.challenge", (0, false), (val, _) => val.flash >= 5 || val.cleared);
        }

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
            acTokenChallenge = null;
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

        public static AchievementToken<(byte playerId, bool kill, bool cleared)> acTokenChallenge;

        public static void resetMorph() {
            morphTarget = null;
            morphTimer = 0f;
            if (morphling == null) return;
            morphling.setDefaultLook();
        }

        public static void onAchievementActivate()
        {
            if (morphling == null || PlayerControl.LocalPlayer != morphling) return;
            acTokenChallenge ??= new("morphling.challenge", (byte.MaxValue, false, false), (val, _) => val.cleared);
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
            acTokenChallenge = null;
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
        public static AchievementToken<(int kills, bool cleared)> acTokenChallenge = null;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CamoButton.png", 115f);
            return buttonSprite;
        }

        public static void onAchievementActivate()
        {
            if (camouflager == null || PlayerControl.LocalPlayer != camouflager) return;
            acTokenChallenge ??= new("camouflager.challenge", (0, false), (val, _) => val.cleared);
        }

        public static void resetCamouflage() {
            camouflageTimer = 0f;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                /*if ((p == Ninja.ninja && Ninja.stealthed) || (p == Sprinter.sprinter && Sprinter.sprinting))
                    continue;*/
                p.setDefaultLook();
            }
            if (PlayerControl.LocalPlayer == camouflager)
                acTokenChallenge.Value.kills = 0;
        }

        public static void clearAndReload() {
            resetCamouflage();
            camouflager = null;
            camouflageTimer = 0f;
            cooldown = CustomOptionHolder.camouflagerCooldown.getFloat();
            duration = CustomOptionHolder.camouflagerDuration.getFloat();
            acTokenChallenge = null;
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
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
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
        public static List<Arrow> localArrows = new();

        public static float updateIntervall = 5f;
        public static bool resetTargetAfterMeeting = false;
        public static bool canTrackCorpses = false;
        public static float corpsesTrackingCooldown = 30f;
        public static float corpsesTrackingDuration = 5f;
        public static float corpsesTrackingTimer = 0f;
        public static int trackingMode = 0;
        public static List<Vector3> deadBodyPositions = new();

        public static PlayerControl currentTarget;
        public static PlayerControl tracked;
        public static bool usedTracker = false;
        public static float timeUntilUpdate = 0f;
        public static Arrow arrow = new(Color.blue);

        public static GameObject DangerMeterParent;
        public static DangerMeter Meter;

        public static AchievementToken<(bool inVent, float ventTime, bool cleared)> acTokenChallenge = null;

        public static void unlockAch(float ventTime)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockTrackerAcChallenge, SendOption.Reliable, -1);
            writer.Write(ventTime);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.unlockTrackerAcChallenge(ventTime);
        }

        public static void onAchievementActivate()
        {
            if (tracker == null || PlayerControl.LocalPlayer != tracker) return;
            acTokenChallenge ??= new("tracker.challenge", (false, 0f, false), (val, _) => val.cleared);
        }

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
            timeUntilUpdate = 0f;
            usedTracker = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
            if (DangerMeterParent)
            {
                Meter.gameObject.Destroy();
                DangerMeterParent.Destroy();
            }
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
            trackingMode = CustomOptionHolder.trackerTrackingMethod.getSelection();
            acTokenChallenge = null;
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

        public static AchievementToken<(DateTime deathTime, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (vampire == null || PlayerControl.LocalPlayer != vampire) return;
            acTokenChallenge ??= new("vampire.challenge", (DateTime.UtcNow, false), (val, _) => val.cleared);
        }

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
            acTokenChallenge = null;
        }
    }

    public static class Snitch {
        public static PlayerControl snitch;
        public static Color color = new Color32(184, 251, 79, byte.MaxValue);

        public static List<Arrow> localArrows = new();
        public static int taskCountForReveal = 1;
        public static bool includeTeamEvil = false;
        public static bool teamEvilUseDifferentArrowColor = true;

        public static void clearAndReload() {
            if (localArrows != null)
            {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            localArrows = new List<Arrow>();
            taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
            includeTeamEvil = CustomOptionHolder.snitchIncludeTeamEvil.getBool();
            teamEvilUseDifferentArrowColor = CustomOptionHolder.snitchTeamEvilUseDifferentArrowColor.getBool();
            snitch = null;
        }
    }

    public static class Jackal {
        public static PlayerControl jackal;
        public static Color color = new Color32(0, 180, 235, byte.MaxValue);
        public static PlayerControl fakeSidekick;
        public static PlayerControl currentTarget;
        public static List<PlayerControl> formerJackals = new();
        
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
        public static bool canSabotageLights;

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
            canSabotageLights = CustomOptionHolder.jackalCanSabotageLights.getBool();
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
        public static bool canSabotageLights;

        public static void clearAndReload() {
            sidekick = null;
            currentTarget = null;
            cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
            canUseVents = CustomOptionHolder.sidekickCanUseVents.getBool();
            canKill = CustomOptionHolder.sidekickCanKill.getBool();
            promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.getBool();
            hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
            wasTeamRed = wasImpostor = wasSpy = false;
            canSabotageLights = CustomOptionHolder.sidekickCanSabotageLights.getBool();
        }
    }

    public static class Eraser {
        public static PlayerControl eraser;
        public static Color color = Palette.ImpostorRed;

        public static List<byte> alreadyErased = new();

        public static List<PlayerControl> futureErased = new();
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static bool canEraseAnyone = false;
        public static float cooldownIncrease = 10f;
        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (eraser == null || PlayerControl.LocalPlayer != eraser) return;
            acTokenChallenge ??= new("eraser.challenge", 0, (val, _) => val >= 3);
        }

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
            cooldownIncrease = CustomOptionHolder.eraserCooldownIncrease.getFloat();
            alreadyErased = new List<byte>();
            acTokenChallenge = null;
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
        public static AchievementToken<(int kills, bool cleared)> acTokenChallenge;

        private static Sprite placeBoxButtonSprite;
        private static Sprite lightOutButtonSprite;
        private static Sprite tricksterVentButtonSprite;

        public static void onAchievementActivate()
        {
            if (trickster == null || PlayerControl.LocalPlayer != trickster) return;
            acTokenChallenge ??= new("trickster.challenge", (0, false), (val, _) => val.cleared || val.kills >= 2);
        }

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
            acTokenChallenge = null;
        }

    }

    public static class Cleaner {
        public static PlayerControl cleaner;
        public static Color color = Palette.ImpostorRed;

        public static float cooldown = 30f;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (cleaner == null || PlayerControl.LocalPlayer != cleaner) return;
            acTokenChallenge ??= new("cleaner.challenge", 0, (val, _) => val >= 3);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CleanButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            cleaner = null;
            cooldown = CustomOptionHolder.cleanerCooldown.getFloat();
            acTokenChallenge = null;
        }
    }

    public static class Warlock {

        public static PlayerControl warlock;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl currentTarget;
        public static PlayerControl curseVictim;
        public static PlayerControl curseVictimTarget;
        public static AchievementToken<int> acTokenChallenge;

        public static float cooldown = 30f;
        public static float rootTime = 5f;

        private static Sprite curseButtonSprite;
        private static Sprite curseKillButtonSprite;

        public static void onAchievementActivate()
        {
            if (warlock == null || PlayerControl.LocalPlayer != warlock) return;
            acTokenChallenge ??= new("warlock.challenge", 0, (val, _) => val >= 3);
        }

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
            acTokenChallenge = null;
        }

        public static void resetCurse() {
            HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
            HudManagerStartPatch.warlockCurseButton.Sprite = getCurseButtonSprite();
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

        public static AchievementToken<(bool vent, bool camera)> acTokenCommon = null;
        public static AchievementToken<int> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (securityGuard == null || PlayerControl.LocalPlayer != securityGuard) return;
            acTokenCommon ??= new("securityGuard.common1", (false, false), (val, _) => val.vent && val.camera);
            acTokenChallenge ??= new("securityGuard.challenge", 0, (val, _) => val >= 5);
        }

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

        private static Sprite fungleVentSealedSprite;
        public static Sprite getFungleVentSealedSprite()
        {
            if (fungleVentSealedSprite) return fungleVentSealedSprite;
            fungleVentSealedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FungleVentSealed.png", 160f);
            return fungleVentSealedSprite;
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
            acTokenChallenge = null;
            acTokenCommon = null;
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
        public static List<PlayerControl> dousedPlayers = new();

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
            return PlayerControl.AllPlayerControls.ToArray().All(x => { return x == arsonist || x.Data.IsDead || x.Data.Disconnected || dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
        }

        public static void clearAndReload() {
            arsonist = null;
            currentTarget = null;
            douseTarget = null; 
            triggerArsonistWin = false;
            dousedPlayers = new List<PlayerControl>();
            TORMapOptions.resetPoolables();
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

        public static AchievementToken<int> acTokenNiceGuesser;
        public static AchievementToken<int> acTokenEvilGuesser;

        public static void niceGuesserOnAchievementActivate()
        {
            if (niceGuesser == null || PlayerControl.LocalPlayer != niceGuesser) return;
            acTokenNiceGuesser ??= new("niceGuesser.challenge1", 0, (val, _) => val >= 3);
        }

        public static void evilGuesserOnAchievementActivate()
        {
            if (evilGuesser == null || PlayerControl.LocalPlayer != evilGuesser) return;
            acTokenEvilGuesser ??= new("evilGuesser.challenge1", 0, (val, _) => val >= 3);
        }

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
            acTokenNiceGuesser = null;
            acTokenEvilGuesser = null;
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

        public static AchievementToken<(DateTime history, int kills, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (bountyHunter == null || PlayerControl.LocalPlayer != bountyHunter) return;
            acTokenChallenge ??= new("bountyHunter.challenge", (DateTime.UtcNow, 0, false), (val, _) => val.cleared);
        }

        public static void clearAllArrow() {
            if (PlayerControl.LocalPlayer != bountyHunter) return;
            if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            if (cooldownText) cooldownText.gameObject.SetActive(false);
            TORMapOptions.resetPoolables();
        }

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
            TORMapOptions.resetPoolables();


            bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.getFloat();
            bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.getFloat();
            punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.getFloat();
            showArrow = CustomOptionHolder.bountyHunterShowArrow.getBool();
            arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.getFloat();
            acTokenChallenge = null;
        }
    }

    public static class Vulture {
        public static PlayerControl vulture;
        public static Color color = new Color32(139, 69, 19, byte.MaxValue);
        public static List<Arrow> localArrows = new();
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
        public static List<Tuple<DeadPlayer, Vector3>> deadBodies = new();
        public static List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new();
        public static List<SpriteRenderer> souls = new();
        public static DateTime meetingStartTime = DateTime.UtcNow;

        public static float cooldown = 30f;
        public static float duration = 3f;
        public static bool oneTimeUse = false;
        public static float chanceAdditionalInfo = 0f;

        public static AchievementToken<int> acTokenCommon = null;
        public static AchievementToken<(List<byte> additionals, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (medium == null || PlayerControl.LocalPlayer != medium) return;
            acTokenCommon ??= new("medium.common1", 0, (val, _) => val >= 3);
            acTokenChallenge ??= new("medium.challenge", (new(), false), (val, _) => val.cleared);
        }

        private static Sprite soulSprite;

        enum SpecialMediumInfo {
            SheriffSuicide,
            ThiefSuicide,
            ActiveLoverDies,
            PassiveLoverSuicide,
            LawyerKilledByClient,
            JackalKillsSidekick,
            ImpostorTeamkill,
            BuskerPseudocide,
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
            acTokenCommon = null;
            acTokenChallenge = null;
        }

        public static string getInfo(PlayerControl target, PlayerControl killer, DeadPlayer.CustomDeathReason deathReason)
        {
            string msg = "";

            List<SpecialMediumInfo> infos = new();
            // collect fitting death info types.
            // suicides:
            if (killer == target) {
                if ((target == Sheriff.sheriff || target == Sheriff.formerSheriff) && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.SheriffSuicide);
                if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
                if (target == Thief.thief && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.ThiefSuicide);
                if (target == Warlock.warlock && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.WarlockSuicide);
                if (target == Busker.busker && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.BuskerPseudocide);
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
                        msg = ModTranslation.getString("mediumSheriffSuicide");
                        break;
                    case SpecialMediumInfo.WarlockSuicide:
                        msg = ModTranslation.getString("mediumWarlockSuicide");
                        break;
                    case SpecialMediumInfo.ThiefSuicide:
                        msg = ModTranslation.getString("mediumThiefSuicide");
                        break;
                    case SpecialMediumInfo.ActiveLoverDies:
                        msg = ModTranslation.getString("mediumActiveLoverDies");
                        break;
                    case SpecialMediumInfo.PassiveLoverSuicide:
                        msg = ModTranslation.getString("mediumPassiveLoverSuicide");
                        break;
                    case SpecialMediumInfo.LawyerKilledByClient:
                        msg = ModTranslation.getString("mediumLawyerKilledByClient");
                        break;
                    case SpecialMediumInfo.BuskerPseudocide:
                        msg = ModTranslation.getString("mediumBuskerPseudocide");
                        break;
                    case SpecialMediumInfo.JackalKillsSidekick:
                        msg = ModTranslation.getString("mediumJackalKillsSidekick");
                        break;
                    case SpecialMediumInfo.ImpostorTeamkill:
                        msg = ModTranslation.getString("mediumImpostorTeamkill");
                        break;
                    case SpecialMediumInfo.BodyCleaned:
                        msg = ModTranslation.getString("mediumBodyCleaned");
                        break;
                }
            } else {
                int randomNumber = rnd.Next(4);
                string typeOfColor = Helpers.isLighterColor(Medium.target.killerIfExisting.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("mediumSoulPlayerLighter") : ModTranslation.getString("mediumSoulPlayerDarker");
                float timeSinceDeath = ((float)(meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds);
                var roleString = RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true);
                var roleInfo = RoleInfo.getRoleInfoForPlayer(Medium.target.player);

                if (randomNumber == 0)
                {
                    if (!roleInfo.Contains(RoleInfo.impostor) && !roleInfo.Contains(RoleInfo.crewmate)) msg = string.Format(ModTranslation.getString("mediumQuestion1"), RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true));
                    else msg = string.Format(ModTranslation.getString("mediumQuestion5"), roleString);
                }
                else if (randomNumber == 1) msg = string.Format(ModTranslation.getString("mediumQuestion2"), typeOfColor);
                else if (randomNumber == 2) msg = string.Format(ModTranslation.getString("mediumQuestion3"), Math.Round(timeSinceDeath / 1000));
                else msg = string.Format(ModTranslation.getString("mediumQuestion4"), RoleInfo.GetRolesString(Medium.target.killerIfExisting, false, false, true, includeHidden: true));
            }

            if (rnd.NextDouble() < chanceAdditionalInfo) {
                int count = 0;
                string condition = "";
                var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
                switch (rnd.Next(3)) {
                    case 0:
                        count = alivePlayersList.Where(pc => pc.Data.Role.IsImpostor || new List<RoleInfo>() { RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.sheriff, RoleInfo.thief, RoleInfo.jekyllAndHyde, RoleInfo.moriarty,
                        SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate ? RoleInfo.schrodingersCat : null}.Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault())).Count();
                        condition = ModTranslation.getString($"mediumKiller{(count == 1 ? "" : "Plural")}");
                        break;
                    case 1:
                        count = alivePlayersList.Where(Helpers.roleCanUseVents).Count();
                        condition = ModTranslation.getString($"mediumPlayerUseVents{(count == 1 ? "" : "Plural")}");
                        break;
                    case 2:
                        count = alivePlayersList.Where(pc => Helpers.isNeutral(pc) && pc != Jackal.jackal && pc != Sidekick.sidekick && pc != Thief.thief && pc != JekyllAndHyde.jekyllAndHyde && pc != Moriarty.moriarty
                        && !(pc == SchrodingersCat.schrodingersCat && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate)).Count();
                        condition = ModTranslation.getString($"mediumPlayerNeutral{(count == 1 ? "" : "Plural")}");
                        break;
                    case 3:
                        //count = alivePlayersList.Where(pc =>
                        break;
                }
                msg += $"\n" + string.Format(ModTranslation.getString("mediumAskPrefix"), string.Format(ModTranslation.getString($"mediumStillAlive{(count == 1 ? "" : "Plural")}"), string.Format(condition, count)));

                acTokenChallenge.Value.additionals.Add(Medium.target.killerIfExisting.PlayerId);
            }

            return string.Format(ModTranslation.getString("mediumSoulPlayerPrefix"), Medium.target.player.Data.PlayerName) + msg;
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
        public static PlayerControl otherKiller;

        public static void clearAndReload()
        {
            nekoKabocha = null;
            meetingKiller = null;
            otherKiller = null;
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
        public static bool targetKnows = true;
        public static bool triggerLawyerWin = false;
        public static int meetings = 0;

        public static bool winsAfterMeetings = false;
        public static int neededMeetings = 4;
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
            triggerLawyerWin = false;
            meetings = 0;
            //isProsecutor = false;
            //triggerProsecutorWin = false;
            vision = CustomOptionHolder.lawyerVision.getFloat();
            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.getBool();
            lawyerTargetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
            targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.getBool();
            winsAfterMeetings = CustomOptionHolder.lawyerWinsAfterMeetings.getBool();
            neededMeetings = Mathf.RoundToInt(CustomOptionHolder.lawyerNeededMeetings.getFloat());
            targetKnows = CustomOptionHolder.lawyerTargetKnows.getBool();
        }
    }

    public static class Pursuer {
        public static PlayerControl pursuer;
        public static PlayerControl target;
        public static Color color = Lawyer.color;
        public static List<PlayerControl> blankedList = new();
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

    public static class MimicK
    {
        public static PlayerControl mimicK;
        public static Color color = Palette.ImpostorRed;

        public static bool ifOneDiesBothDie = true;
        public static bool hasOneVote = true;
        public static bool countAsOne = true;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;

        public static AchievementToken<int> acTokenChallenge;


        public static PlayerControl victim;

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer != mimicK || PlayerControl.LocalPlayer.Data.IsDead || MimicA.mimicA == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow1 in arrows)
                {
                    if (arrow1 != null && arrow1.arrow != null)
                    {
                        arrow1.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow1.arrow);
                    }
                }
                // Arrows一覧
                arrows = new List<Arrow>();

                if (MimicA.mimicA.Data.IsDead || MimicA.mimicA == null) return;
                Arrow arrow;
                arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicA.mimicA.transform.position);
                arrows.Add(arrow);

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void onAchievementActivate()
        {
            if (mimicK == null || PlayerControl.LocalPlayer != mimicK) return;
            acTokenChallenge ??= new("mimicK.challenge", 0, (val, _) => val >= 3);
        }

        public static void clearAndReload()
        {
            if (mimicK != null && mimicK?.Data != null) mimicK.setDefaultLook();
            if (MimicA.mimicA != null)
            {
                MimicA.isMorph = false;
                MimicA.mimicA.setDefaultLook();
            }

            mimicK = null;
            victim = null;
            ifOneDiesBothDie = CustomOptionHolder.mimicIfOneDiesBothDie.getBool();
            hasOneVote = CustomOptionHolder.mimicHasOneVote.getBool();
            countAsOne = CustomOptionHolder.mimicCountAsOne.getBool();

            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            acTokenChallenge = null;
        }
    }

    public static class MimicA
    {
        public static PlayerControl mimicA;
        public static Color color = Palette.ImpostorRed;

        public static bool isMorph = false;

        //public static string MimicKName = MimicK.mimicK.Data.PlayerName;

        public static Sprite adminButtonSprite;
        public static Sprite morphButtonSprite;

        public static Sprite getMorphSprite()
        {
            if (morphButtonSprite) return morphButtonSprite;
            morphButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MorphButton.png", 115f);
            return morphButtonSprite;
        }

        public static Sprite getAdminSprite()
        {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            adminButtonSprite = button.Image;
            return adminButtonSprite;
        }

        public static AchievementToken<int> acTokenCommon;

        public static void onAchievementActivate()
        {
            if (mimicA == null || PlayerControl.LocalPlayer != mimicA) return;
            acTokenCommon ??= new("mimicA.challenge", 0, (val, _) => val >= 4);
        }

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static void arrowUpdate()
        {
            if (MimicK.mimicK == null || MimicK.mimicK.Data.IsDead || MimicK.mimicK.Data.Disconnected) {
                if (mimicA != null && isMorph) {
                    isMorph = false;
                    mimicA.setDefaultLook();
                }
            }

            if (PlayerControl.LocalPlayer != mimicA || PlayerControl.LocalPlayer.Data.IsDead || MimicK.mimicK == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }

            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow1 in arrows)
                {
                    if (arrow1 != null && arrow1.arrow != null)
                    {
                        arrow1.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow1.arrow);
                    }
                }

                //if (MimicA.mimicA == null) return;

                // Arrows一覧
                arrows = new List<Arrow>();
                if (MimicK.mimicK.Data.IsDead || MimicK.mimicK == null) return;
                Arrow arrow = new(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicK.mimicK.transform.position);
                arrows.Add(arrow);

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void clearAndReload()
        {
            if (mimicA != null && mimicA?.Data != null) mimicA.setDefaultLook();
            mimicA = null;
            isMorph = false;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            acTokenCommon = null;
        }
    }

    public static class FortuneTeller
    {
        public static PlayerControl fortuneTeller;
        public static PlayerControl divineTarget;
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

        public static Dictionary<byte, float> progress = new();
        public static Dictionary<byte, bool> playerStatus = new();
        public static bool divinedFlag = false;
        public static int numUsed = 0;

        private static Sprite leftButtonSprite;
        private static Sprite rightButtonSprite;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;

        public static int pageIndex = 1;

        public static AchievementToken<(bool divined, bool cleared)> acTokenImpostor = null;

        public static void onAchievementActivate()
        {
            if (fortuneTeller == null || PlayerControl.LocalPlayer != fortuneTeller) return;
            acTokenImpostor ??= new("fortuneTeller.challenge", (false, false), (val, _) => val.cleared);
        }

        public static Sprite getLeftButtonSprite()
        {
            if (leftButtonSprite) return leftButtonSprite;
            leftButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FortuneTellerButtonLeft.png", 130f);
            return leftButtonSprite;
        }

        public static Sprite getRightButtonSprite()
        {
            if (rightButtonSprite) return rightButtonSprite;
            rightButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FortuneTellerButtonRight.png", 130f);
            return rightButtonSprite;
        }

        public static bool isCompletedNumTasks(PlayerControl p)
        {
            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
            return tasksCompleted >= numTasks;
        }

        public static void setDivinedFlag(PlayerControl player, bool flag)
        {
            if (player == fortuneTeller)
            {
                divinedFlag = flag;
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

            if (divineResult == DivineResults.BlackWhite)
            {
                if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
                {
                    msg = string.Format(ModTranslation.getString("divineMessageIsCrew"), p.Data.PlayerName);
                    color = Color.white;
                }
                else
                {
                    msg = string.Format(ModTranslation.getString("divineMessageIsntCrew"), p.Data.PlayerName);
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Team)
            {
                if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
                {
                    msg = string.Format(ModTranslation.getString("divineMessageTeamCrew"), p.Data.PlayerName);
                    color = Color.white;
                }
                else if (Helpers.isNeutral(p))
                {
                    msg = string.Format(ModTranslation.getString("divineMessageTeamNeutral"), p.Data.PlayerName);
                    color = Color.yellow;
                }
                else
                {
                    msg = string.Format(ModTranslation.getString("divineMessageTeamImp"), p.Data.PlayerName);
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Role)
            {
                msg = $"{p.Data.PlayerName} was The {String.Join(" ", RoleInfo.getRoleInfoForPlayer(p, false, true).Select(x => Helpers.cs(x.color, x.name)))}";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                fortuneTellerMessage(msg, 7f, color);
            }

            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
            numUsed += 1;

            // 占いを実行したことで発火される処理を他クライアントに通知
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FortuneTellerUsedDivine, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(p.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.fortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
        }

        public static void clearAndReload()
        {
            meetingFlag = true;
            duration = CustomOptionHolder.fortuneTellerDuration.getFloat();
            List<Arrow> arrows = new();
            numTasks = (int)CustomOptionHolder.fortuneTellerNumTasks.getFloat();
            distance = CustomOptionHolder.fortuneTellerDistance.getFloat();
            divineResult = (DivineResults)CustomOptionHolder.fortuneTellerResults.getSelection();
            fortuneTeller = null;
            playerStatus = new Dictionary<byte, bool>();
            progress = new Dictionary<byte, float>();
            numUsed = 0;
            pageIndex = 1;
            divinedFlag = false;
            divineTarget = null;
            acTokenImpostor = null;
            TORMapOptions.resetPoolables();
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatch
        {
            public static void Prefix(IntroCutscene __instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(16.2f, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        meetingFlag = false;
                    }
                })));
            }
        }
    }

    public static class TaskMaster
    {
        public static PlayerControl taskMaster = null;
        public static bool becomeATaskMasterWhenCompleteAllTasks = false;
        public static Color color = new Color32(225, 86, 75, byte.MaxValue);
        public static bool isTaskComplete = false;
        public static byte clearExTasks = 0;
        public static byte allExTasks = 0;
        public static byte oldTaskMasterPlayerId = byte.MaxValue;
        public static bool triggerTaskMasterWin = false;

        public static void clearAndReload()
        {
            taskMaster = null;
            becomeATaskMasterWhenCompleteAllTasks = CustomOptionHolder.taskMasterBecomeATaskMasterWhenCompleteAllTasks.getBool();
            isTaskComplete = false;
            clearExTasks = 0;
            allExTasks = 0;
            oldTaskMasterPlayerId = byte.MaxValue;
            triggerTaskMasterWin = false;
        }

        public static bool isTaskMaster(byte playerId)
        {
            return taskMaster != null && taskMaster.PlayerId == playerId;
        }
    }

    public static class Yasuna
    {
        public static PlayerControl yasuna;
        public static Color color = new Color32(90, 255, 25, byte.MaxValue);
        public static byte specialVoteTargetPlayerId = byte.MaxValue;
        private static int _remainingSpecialVotes = 1;
        private static Sprite targetSprite;

        public static AchievementToken<(byte targetId, bool cleared)> yasunaAcTokenChallenge = null;
        public static AchievementToken<(byte targetId, bool cleared)> evilYasunaAcTokenChallenge;

        public static void yasunaOnAchievementActivate()
        {
            if (yasuna != null && PlayerControl.LocalPlayer == yasuna)
                yasunaAcTokenChallenge ??= new("niceYasuna.challenge", (byte.MaxValue, false), (val, _) => val.cleared);
        }

        public static void evilYasunaOnAcheivementActivate()
        {
            if (yasuna == null || PlayerControl.LocalPlayer != yasuna) return;
            evilYasunaAcTokenChallenge ??= new("evilYasuna.another1", (byte.MaxValue, false), (val, _) => val.cleared);
        }

        public static void clearAndReload()
        {
            yasuna = null;
            _remainingSpecialVotes = Mathf.RoundToInt(CustomOptionHolder.yasunaNumberOfSpecialVotes.getFloat());
            specialVoteTargetPlayerId = byte.MaxValue;
            yasunaAcTokenChallenge = null;
            evilYasunaAcTokenChallenge = null;
        }

        public static Sprite getTargetSprite(bool isImpostor)
        {
            if (targetSprite) return targetSprite;
            targetSprite = Helpers.loadSpriteFromResources(isImpostor ? "TheOtherRoles.Resources.EvilYasunaTargetIcon.png" : "TheOtherRoles.Resources.YasunaTargetIcon.png", 150f);
            return targetSprite;
        }

        public static int remainingSpecialVotes(bool isVote = false)
        {
            if (yasuna == null)
                return 0;

            if (isVote)
                _remainingSpecialVotes = Mathf.Max(0, _remainingSpecialVotes - 1);
            return _remainingSpecialVotes;
        }

        public static bool isYasuna(byte playerId)
        {
            return yasuna != null && yasuna.PlayerId == playerId;
        }
    }

    public static class Trapper
    {
        public static PlayerControl trapper;
        public static Color color = Palette.ImpostorRed;

        public static float minDistance = 0f;
        public static float maxDistance;
        public static int numTrap;
        public static float extensionTime;
        public static float killTimer;
        public static float cooldown;
        public static float trapRange;
        public static float penaltyTime;
        public static float bonusTime;
        public static bool isTrapKill = false;
        public static bool meetingFlag;

        public static Sprite trapButtonSprite;
        public static DateTime placedTime;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (trapper == null || PlayerControl.LocalPlayer != trapper) return;
            acTokenChallenge ??= new("trapper.challenge", 0, (val, _) => val >= 3);
        }

        public static Sprite getTrapButtonSprite()
        {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrapperButton.png", 115f);
            return trapButtonSprite;
        }

        public static void setTrap()
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceTrap, SendOption.Reliable);
            writer.WriteBytesAndSize(buff);
            writer.EndMessage();
            RPCProcedure.placeTrap(buff);
            placedTime = DateTime.UtcNow;
        }

        public static void clearAndReload()
        {
            trapper = null;
            numTrap = (int)CustomOptionHolder.trapperNumTrap.getFloat();
            extensionTime = CustomOptionHolder.trapperExtensionTime.getFloat();
            killTimer = CustomOptionHolder.trapperKillTimer.getFloat();
            cooldown = CustomOptionHolder.trapperCooldown.getFloat();
            trapRange = CustomOptionHolder.trapperTrapRange.getFloat();
            penaltyTime = CustomOptionHolder.trapperPenaltyTime.getFloat();
            bonusTime = CustomOptionHolder.trapperBonusTime.getFloat();
            maxDistance = CustomOptionHolder.trapperMaxDistance.getFloat();
            meetingFlag = false;
            Trap.clearAllTraps();
            acTokenChallenge = null;
        }
    }

    public static class SchrodingersCat
    {
        public static Color color = Color.grey;
        public static PlayerControl schrodingersCat;
        public static PlayerControl formerSchrodingersCat;
        public static Team team;

        public static PoolablePlayer playerTemplate;
        public static GameObject parent;
        public static List<PoolablePlayer> teams;
        public static bool shownMenu = false;
        public static PlayerControl currentTarget;

        public enum Team
        {
            None,
            Impostor,
            Crewmate,
            Jackal,
            JekyllAndHyde,
            Moriarty
        }

        public static bool isTeamJackalAlive()
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == Jackal.jackal && !p.Data.IsDead)
                {
                    return true;
                }
                else if (p == Sidekick.sidekick && !p.Data.IsDead)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isLastImpostor()
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (PlayerControl.LocalPlayer != p && p.Data.Role.IsImpostor && !p.Data.IsDead) return false;
            }
            return true;
        }

        public static void setImpostorFlag()
        {
            RoleInfo.schrodingersCat.color = Palette.ImpostorRed;
            RoleInfo.schrodingersCat.isNeutral = false;
            team = Team.Impostor;
        }

        public static void setJackalFlag()
        {
            RoleInfo.schrodingersCat.color = Jackal.color;
            team = Team.Jackal;
        }

        public static void setJekyllAndHydeFlag()
        {
            RoleInfo.schrodingersCat.color = JekyllAndHyde.color;
            team = Team.JekyllAndHyde;
        }

        public static void setMoriartyFlag()
        {
            RoleInfo.schrodingersCat.color = Moriarty.color;
            team = Team.Moriarty;
        }

        public static void setCrewFlag()
        {
            RoleInfo.schrodingersCat.color = Color.white;
            RoleInfo.schrodingersCat.isNeutral = false;
            team = Team.Crewmate;
        }

        public static bool tasksComplete(PlayerControl p)
        {
            int counter = 0;
            var option = GameOptionsManager.Instance.currentNormalGameOptions;
            int totalTasks = option.NumLongTasks + option.NumShortTasks + option.NumCommonTasks;
            if (totalTasks == 0) return true;
            foreach (var task in p.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter == totalTasks;
        }

        public static float killCooldown;
        public static bool becomesImpostor;
        public static bool cantKillUntilLastOne;
        public static bool justDieOnKilledByCrew;
        public static bool hideRole;
        public static bool canChooseImpostor;

        public static bool hasTeam() => team != Team.None;

        public static void showMenu()
        {
            if (!shownMenu)
            {
                if (teams.Count == 0)
                {
                    var colorBG = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.White.png", 100f);
                    var hudManager = FastDestroyableSingleton<HudManager>.Instance;
                    parent = new GameObject("PoolableParent");
                    parent.transform.parent = hudManager.transform;
                    parent.transform.localPosition = new Vector3(0, 0, 0);
                    var impostor = createPoolable(parent, "impostor", 0, (UnityAction)((Action)(() =>
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                        writer.Write((byte)Team.Impostor);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.schrodingersCatSetTeam((byte)Team.Impostor);
                        showMenu();
                    })));
                    teams.Add(impostor);
                    if (Jackal.jackal != null || Sidekick.sidekick != null)
                    {
                        var jackal = createPoolable(parent, "jackal", 1, (UnityAction)((Action)(() =>
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                            writer.Write((byte)Team.Jackal);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.schrodingersCatSetTeam((byte)Team.Jackal);
                            showMenu();
                        })));
                        teams.Add(jackal);
                    }
                    if (Moriarty.moriarty != null)
                    {
                        var moriarty = createPoolable(parent, "moriarty", 2, (UnityAction)((Action)(() =>
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                            writer.Write((byte)Team.Moriarty);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.schrodingersCatSetTeam((byte)Team.Moriarty);
                            showMenu();
                        })));
                        teams.Add(moriarty);
                    }
                    if (JekyllAndHyde.jekyllAndHyde != null)
                    {
                        var jekyllAndHyde = createPoolable(parent, "jekyllAndHyde", 6, (UnityAction)((Action)(() =>
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                            writer.Write((byte)Team.JekyllAndHyde);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.schrodingersCatSetTeam((byte)Team.JekyllAndHyde);
                            showMenu();
                        })));
                        teams.Add(jekyllAndHyde);
                    }
                    var crewmate = createPoolable(parent, "crewmate", 10, (UnityAction)((Action)(() =>
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SchrodingersCatSetTeam, SendOption.Reliable, -1);
                        writer.Write((byte)Team.Crewmate);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.schrodingersCatSetTeam((byte)Team.Crewmate);
                        showMenu();
                    })));
                    teams.Add(crewmate);
                    layoutPoolable();
                }
                else
                {
                    teams.ForEach(x =>
                    {
                        x.gameObject.SetActive(true);
                    });
                    layoutPoolable();
                }
            }
            else
            {
                teams.ForEach(x =>
                {
                    x.gameObject.SetActive(false);
                });
            }
            shownMenu = !shownMenu;
        }

        public static bool isJackalButtonEnable()
        {
            if (team == Team.Jackal && PlayerControl.LocalPlayer == schrodingersCat && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (!isTeamJackalAlive() || !cantKillUntilLastOne)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isJekyllAndHydeButtonEnable()
        {
            if (team == Team.JekyllAndHyde && PlayerControl.LocalPlayer == schrodingersCat && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (JekyllAndHyde.jekyllAndHyde == null || JekyllAndHyde.jekyllAndHyde.Data.IsDead || !cantKillUntilLastOne)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isMoriartyButtonEnable()
        {
            if (team == Team.Moriarty && PlayerControl.LocalPlayer == schrodingersCat && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (Moriarty.moriarty == null || Moriarty.moriarty.Data.IsDead || !cantKillUntilLastOne)
                {
                    return true;
                }
            }
            return false;
        }

        private static PoolablePlayer createPoolable(GameObject parent, string name, int color, UnityAction func)
        {
            var poolable = UnityEngine.Object.Instantiate(playerTemplate, parent.transform);
            var actionButton = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton, poolable.gameObject.transform);
            SpriteRenderer spriteRenderer = actionButton.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = null;
            actionButton.transform.localPosition = new Vector3(0, 0, 0);
            actionButton.gameObject.SetActive(true);
            actionButton.gameObject.ForEachChild((Il2CppSystem.Action<GameObject>)((c) => { if (c.name.Equals("HotKeyGuide")) UnityEngine.Object.Destroy(c); }));
            PassiveButton button = actionButton.GetComponent<PassiveButton>();
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((UnityAction)func);
            var texts = actionButton.GetComponentsInChildren<TMPro.TextMeshPro>();
            texts.ForEach(x => x.gameObject.SetActive(false));
            poolable.gameObject.SetActive(true);
            poolable.SetBodyColor(color);
            poolable.SetName(ModTranslation.getString(name));
            return poolable;
        }

        public static void ForEach<T>(this IList<T> self, Action<T> todo)
        {
            for (int i = 0; i < self.Count; i++)
            {
                todo(self[i]);
            }
        }

        public static void layoutPoolable()
        {
            float offset = 2f;
            int center = teams.Count / 2;
            for (int i = 0; i < teams.Count; i++)
            {
                float x = teams.Count % 2 != 0 ? (offset * (i - center)) : (offset * (i - center)) + (offset * 0.5f);
                teams[i].transform.localPosition = new Vector3(x, 0, 0);
                teams[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                teams[i].GetComponentInChildren<ActionButton>().transform.position = teams[i].transform.position;
            }
        }

        public static void clearAndReload()
        {
            schrodingersCat = null;
            formerSchrodingersCat = null;
            currentTarget = null;
            team = Team.None;
            canChooseImpostor = CustomOptionHolder.schrodingersCatHideRole.getBool() && CustomOptionHolder.schrodingersCatCanChooseImpostor.getBool();
            hideRole = CustomOptionHolder.schrodingersCatHideRole.getBool();
            justDieOnKilledByCrew = CustomOptionHolder.schrodingersCatJustDieOnKilledByCrew.getBool();
            cantKillUntilLastOne = CustomOptionHolder.schrodingersCatCantKillUntilLastOne.getBool();
            becomesImpostor = CustomOptionHolder.schrodingersCatBecomesImpostor.getBool();
            killCooldown = CustomOptionHolder.schrodingersCatKillCooldown.getFloat();
            RoleInfo.schrodingersCat.color = color;
            RoleInfo.schrodingersCat.isNeutral = true;
            shownMenu = false;
            teams = new List<PoolablePlayer>();
        }
    }

    public static class BomberA
    {
        public static PlayerControl bomberA;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl bombTarget;
        public static PlayerControl currentTarget;
        public static PlayerControl tmpTarget;

        public static Sprite bomberButtonSprite;
        public static Sprite releaseButtonSprite;
        public static float updateTimer = 0f;
        public static List<Arrow> arrows = new();
        public static float arrowUpdateInterval = 0.5f;

        public static float duration;
        public static float cooldown;
        public static bool countAsOne;
        public static bool showEffects;
        public static bool ifOneDiesBothDie;
        public static bool hasOneVote;
        public static bool alwaysShowArrow;

        public static TMPro.TextMeshPro targetText;
        public static TMPro.TextMeshPro partnerTargetText;
        public static Dictionary<byte, PoolablePlayer> playerIcons = new();

        public static Sprite getBomberButtonSprite()
        {
            if (bomberButtonSprite) return bomberButtonSprite;
            bomberButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlantBombButton.png", 115f);
            return bomberButtonSprite;
        }
        public static Sprite getReleaseButtonSprite()
        {
            if (releaseButtonSprite) return releaseButtonSprite;
            releaseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ReleaseButton.png", 115f);
            return releaseButtonSprite;
        }

        public static void arrowUpdate()
        {            
            if ((bombTarget == null || BomberB.bombTarget == null) && !alwaysShowArrow) return;
            if (bomberA.Data.IsDead || BomberB.bomberB == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();
                /*if (BomberB.bomberB == null || BomberB.bomberB.Data.IsDead) return;
                // 相方の位置を示すArrowsを描画
                Arrow arrow = new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(BomberB.bomberB.transform.position);
                arrows.Add(arrow);*/
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (p == BomberB.bomberB)
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

        public static void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            //foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            if (bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !bomberA.Data.IsDead && !MeetingHud.Instance)
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
                        targetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        targetText.enableWordWrapping = false;
                        targetText.transform.localScale = Vector3.one * 1.5f;
                        targetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    targetText.text = ModTranslation.getString("bomberYourTarget");
                    targetText.gameObject.SetActive(true);
                    targetText.transform.parent = icon.gameObject.transform;
                }
                // 相方の設置したターゲットを表示する
                if (BomberB.bombTarget != null && TORMapOptions.playerIcons.ContainsKey(BomberB.bombTarget.PlayerId) && TORMapOptions.playerIcons[BomberB.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[BomberB.bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(1.0f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (partnerTargetText == null)
                    {
                        partnerTargetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        partnerTargetText.enableWordWrapping = false;
                        partnerTargetText.transform.localScale = Vector3.one * 1.5f;
                        partnerTargetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    partnerTargetText.text = ModTranslation.getString("bomberPartnerTarget");
                    partnerTargetText.gameObject.SetActive(true);
                    partnerTargetText.transform.parent = icon.gameObject.transform;
                }
            }
        }

        public static void clearAndReload()
        {
            bomberA = null;
            bombTarget = null;
            currentTarget = null;
            tmpTarget = null;
            playerIcons = new Dictionary<byte, PoolablePlayer>();
            targetText = null;
            partnerTargetText = null;
            TORMapOptions.resetPoolables();
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();

            duration = CustomOptionHolder.bomberDuration.getFloat();
            cooldown = CustomOptionHolder.bomberCooldown.getFloat();
            countAsOne = CustomOptionHolder.bomberCountAsOne.getBool();
            showEffects = CustomOptionHolder.bomberShowEffects.getBool();
            hasOneVote = CustomOptionHolder.bomberHasOneVote.getBool();
            ifOneDiesBothDie = CustomOptionHolder.bomberIfOneDiesBothDie.getBool();
            alwaysShowArrow = CustomOptionHolder.bomberAlwaysShowArrow.getBool();
        }
    }

    public static class BomberB
    {
        public static PlayerControl bomberB;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl bombTarget;
        public static PlayerControl tmpTarget;
        public static PlayerControl currentTarget;
        public static TMPro.TextMeshPro targetText;
        public static TMPro.TextMeshPro partnerTargetText;
        public static Dictionary<byte, PoolablePlayer> playerIcons = new();
        public static Sprite bomberButtonSprite;
        public static Sprite releaseButtonSprite;
        public static float updateTimer = 0f;
        public static List<Arrow> arrows = new();
        public static float arrowUpdateInterval = 0.5f;

        public static void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            //foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            if (BomberA.bomberA != null && bomberB != null && !bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && !MeetingHud.Instance)
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
                        targetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        targetText.enableWordWrapping = false;
                        targetText.transform.localScale = Vector3.one * 1.5f;
                        targetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    targetText.text = ModTranslation.getString("bomberYourTarget");
                    targetText.gameObject.SetActive(true);
                    targetText.transform.parent = icon.gameObject.transform;
                }
                // 相方の設置したターゲットを表示する
                if (BomberA.bombTarget != null && TORMapOptions.playerIcons.ContainsKey(BomberA.bombTarget.PlayerId) && TORMapOptions.playerIcons[BomberA.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[BomberA.bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(1.0f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (partnerTargetText == null)
                    {
                        partnerTargetText = UnityEngine.Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        partnerTargetText.enableWordWrapping = false;
                        partnerTargetText.transform.localScale = Vector3.one * 1.5f;
                        partnerTargetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    partnerTargetText.text = ModTranslation.getString("bomberPartnerTarget");
                    partnerTargetText.gameObject.SetActive(true);
                    partnerTargetText.transform.parent = icon.gameObject.transform;
                }
            }
        }

        public static void arrowUpdate()
        {            
            if ((BomberA.bombTarget == null || bombTarget == null) && !BomberA.alwaysShowArrow) return;
            if (bomberB.Data.IsDead || BomberA.bomberA == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();
                /*if (BomberA.bomberA == null || BomberA.bomberA.Data.IsDead) return;
                // 相方の位置を示すArrowsを描画
                Arrow arrow = new Arrow(Palette.ImpostorRed);
                
                arrow.arrow.SetActive(true);
                arrow.Update(BomberA.bomberA.transform.position);
                arrows.Add(arrow);*/
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (p == BomberA.bomberA)
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

        public static Sprite getBomberButtonSprite()
        {
            if (bomberButtonSprite) return bomberButtonSprite;
            bomberButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlantBombButton.png", 115f);
            return bomberButtonSprite;
        }
        public static Sprite getReleaseButtonSprite()
        {
            if (releaseButtonSprite) return releaseButtonSprite;
            releaseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ReleaseButton.png", 115f);
            return releaseButtonSprite;
        }

        public static void clearAndReload()
        {
            bomberB = null;
            bombTarget = null;
            currentTarget = null;
            tmpTarget = null;
            TORMapOptions.resetPoolables();
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            playerIcons = new Dictionary<byte, PoolablePlayer>();
            targetText = null;
            partnerTargetText = null;
        }
    }

    public static class CreatedMadmate
    {
        public static PlayerControl createdMadmate;

        public static bool canEnterVents;
        public static bool hasImpostorVision;
        public static bool canSabotage;
        public static bool canFixComm;
        public static bool canDieToSheriff;
        public static bool hasTasks;
        public static int numTasks;

        public static bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = numTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter >= totalTasks;
        }

        public static void clearAndReload()
        {
            createdMadmate = null;
            canEnterVents = CustomOptionHolder.createdMadmateCanEnterVents.getBool();
            canDieToSheriff = CustomOptionHolder.createdMadmateCanDieToSheriff.getBool();
            hasTasks = CustomOptionHolder.createdMadmateAbility.getBool();
            canSabotage = CustomOptionHolder.createdMadmateCanSabotage.getBool();
            canFixComm = CustomOptionHolder.createdMadmateCanFixComm.getBool();
            numTasks = (int)CustomOptionHolder.createdMadmateCommonTasks.getFloat();
        }
    }

    public static class Teleporter
    {
        public static PlayerControl teleporter;
        public static Color color = new Color32(164, 249, 255, byte.MaxValue);
        private static Sprite teleportButtonSprite;
        public static float teleportCooldown = 30f;
        public static float sampleCooldown = 30f;
        public static int teleportNumber = 5;
        public static PlayerControl target1;
        public static PlayerControl target2;
        public static bool SwappingMenus;

        public static AchievementToken<(byte target1, byte target2, DateTime swapTime, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (teleporter == null || PlayerControl.LocalPlayer != teleporter) return;
            acTokenChallenge ??= new("teleporter.challenge", (byte.MaxValue, byte.MaxValue, DateTime.UtcNow, false), (val, _) => val.cleared);
        }

        public static Sprite getButtonSprite()
        {
            if (teleportButtonSprite) return teleportButtonSprite;
            teleportButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TeleporterButton.png", 115f);
            return teleportButtonSprite;
        }

        public static void clearAndReload()
        {
            teleporter = null;
            target1 = null;
            target2 = null;
            teleportCooldown = CustomOptionHolder.teleporterCooldown.getFloat();
            teleportNumber = (int)CustomOptionHolder.teleporterTeleportNumber.getFloat();
            acTokenChallenge = null;
            SwappingMenus = false;
        }

        public static IEnumerator OpenSecondMenu()
        {
            try
            {
                PlayerMenu.singleton.Menu.ForceClose();
            }
            catch
            {

            }
            yield return (object)new WaitForSeconds(0.05f);
            SwappingMenus = false;
            if (MeetingHud.Instance || PlayerControl.LocalPlayer != teleporter) yield break;
            List<byte> transportTargets = new();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Disconnected && player != target1)
                {
                    if (!player.Data.IsDead) transportTargets.Add(player.PlayerId);
                    else
                    {
                        foreach (var body in UnityEngine.Object.FindObjectsOfType<DeadBody>())
                        {
                            if (body.ParentId == player.PlayerId) transportTargets.Add(player.PlayerId);
                        }
                    }
                }
            }
            byte[] transporttargetIDs = transportTargets.ToArray();
            var pk = new PlayerMenu((x) =>
            {
                target2 = x;
                if (target1 != null && target2 != null && !target1.Data.IsDead && !target2.Data.IsDead && target1.moveable && target2.moveable)
                {
                    _ = new StaticAchievementToken("teleporter.common1");
                    acTokenChallenge.Value.swapTime = DateTime.UtcNow;
                    acTokenChallenge.Value.target1 = target1.PlayerId;
                    acTokenChallenge.Value.target2 = target2.PlayerId;
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TeleporterTeleport, SendOption.Reliable, -1);
                    writer.Write(target1.PlayerId);
                    writer.Write(target2.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.teleporterTeleport(target1.PlayerId, target2.PlayerId);
                }
                target1 = null;
                target2 = null;
                HudManagerStartPatch.teleporterTeleportButton.Timer = HudManagerStartPatch.teleporterTeleportButton.MaxTimer;
                teleportNumber--;
                SoundEffectsManager.play("teleporterTeleport");
            }, (y) =>
            {
                return transporttargetIDs.Contains(y.PlayerId);
            });
            Coroutines.Start(pk.Open(0f, true));
        }
        private static float[] PanelAreaScale = { 1f, 0.95f, 0.76f };
        private static (int x, int y)[] PanelAreaSize = { (3, 5), (3, 6), (4, 6) };
        private static Vector3[] PanelAreaOffset = { new Vector3(0.0f, 0.0f, -1f), new Vector3(0.1f, 0.145f, -1f), new Vector3(-0.355f, 0.0f, -1f) };
        private static (float x, float y)[] PanelAreaMultiplier = { (1f, 1f), (1f, 0.89f), (275f * (float)Math.PI / 887f, 1f) };
        private static Vector3 ToVoteAreaPos(ShapeshifterMinigame minigame, int index, int arrangeType) => Helpers.convertPos(index, arrangeType, PanelAreaSize, new Vector3(minigame.XStart, minigame.YStart, -1f), PanelAreaOffset, new Vector3(minigame.XOffset, minigame.YOffset), PanelAreaScale, PanelAreaMultiplier);

        public class PlayerMenu
        {
            public ShapeshifterMinigame Menu;
            public Select Click;
            public Include Inclusion;
            public List<PlayerControl> Targets;
            public static PlayerMenu singleton;
            public delegate void Select(PlayerControl player);
            public delegate bool Include(PlayerControl player);

            public PlayerMenu(Select click, Include inclusion)
            {
                Click = click;
                Inclusion = inclusion;
                if (singleton != null)
                {
                    singleton.Menu.DestroyImmediate();
                    singleton = null;
                }
                singleton = this;
            }

            public IEnumerator Open(float delay, bool includeDead = false)
            {
                yield return new WaitForSecondsRealtime(delay);
                while (ExileController.Instance != null) { yield return 0; }
                Targets = PlayerControl.AllPlayerControls.ToArray().Where(x => Inclusion(x) && (!x.Data.IsDead || includeDead) && !x.Data.Disconnected).ToList();
                TheOtherRolesPlugin.Logger.LogMessage($"Targets {Targets.Count}");
                if (Menu == null)
                {
                    if (Camera.main == null)
                        yield break;

                    Menu = UnityEngine.Object.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
                }

                Menu.transform.SetParent(Camera.main.transform, false);
                Menu.transform.localPosition = new(0f, 0f, -50f);
                Menu.Begin(null);
            }

            private static ShapeshifterMinigame GetShapeshifterMenu()
            {
                var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter);
                return UnityEngine.Object.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
            }

            public void Clicked(PlayerControl player)
            {
                Click(player);
                Menu.Close();
            }

            [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
            public static class MenuPatch
            {
                public static bool Prefix(ShapeshifterMinigame __instance)
                {
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(false);
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                    var menu = singleton;

                    if (menu == null)
                        return true;

                    __instance.potentialVictims = new();
                    var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

                    for (var i = 0; i < menu.Targets.Count; i++)
                    {
                        int displayType = Helpers.GetDisplayType(menu.Targets.Count);
                        var player = menu.Targets[i];
                        bool isDead = player.Data.IsDead;
                        player.Data.IsDead = false;
                        var num = i % 3;
                        var num2 = i / 3;
                        var panel = UnityEngine.Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                        panel.transform.localScale *= PanelAreaScale[displayType];
                        panel.transform.localPosition = ToVoteAreaPos(__instance, i, displayType);
                        panel.SetPlayer(i, player.Data, (Action)(() => menu.Clicked(player)));
                        panel.transform.FindChild("Nameplate/Highlight/ShapeshifterIcon").gameObject.SetActive(false);
                        panel.Background.gameObject.GetComponent<ButtonRolloverHandler>().OverColor = color;
                        __instance.potentialVictims.Add(panel);
                        list2.Add(panel.Button);
                        player.Data.IsDead = isDead;
                    }

                    var Phone = __instance.transform.Find("PhoneUI/Background").GetComponent<SpriteRenderer>();
                    if (Phone != null)
                    {
                        Phone.material?.SetColor(PlayerMaterial.BodyColor, color);
                        Phone.material?.SetColor(PlayerMaterial.BackColor, color - new UnityEngine.Color(0.25f, 0.25f, 0.25f));
                    }
                    var PhoneButton = __instance.transform.Find("PhoneUI/UI_Phone_Button").GetComponent<SpriteRenderer>();
                    if (PhoneButton != null)
                    {
                        PhoneButton.material?.SetColor(PlayerMaterial.BodyColor, color);
                        PhoneButton.material?.SetColor(PlayerMaterial.BackColor, color - new UnityEngine.Color(0.25f, 0.25f, 0.25f));
                    }

                    ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
                    return false;
                }
            }
            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
            public static class StartMeeting
            {
                public static void Prefix(PlayerControl __instance)
                {
                    if (__instance == null) return;
                    try
                    {
                        singleton.Menu.Close();
                    }
                    catch { }
                }
            }
        }
    }

    public static class Kataomoi
    {
        public static PlayerControl kataomoi;
        public static Color color = Lovers.color;

        public static float stareCooldown = 30f;
        public static float stareDuration = 3f;
        public static int stareCount = 1;
        public static int stareCountMax = 1;
        public static float stalkingCooldown = 30f;
        public static float stalkingDuration = 5f;
        public static float stalkingFadeTime = 0.5f;
        public static float searchCooldown = 30f;
        public static float searchDuration = 5f;
        public static bool isSearch = false;
        public static float stalkingTimer = 0f;
        public static float stalkingEffectTimer = 0f;
        public static bool triggerKataomoiWin = false;
        public static PlayerControl target = null;
        public static PlayerControl currentTarget = null;
        public static TMPro.TextMeshPro stareText = null;
        public static SpriteRenderer[] gaugeRenderer = new SpriteRenderer[3];
        public static Arrow arrow;
        public static float gaugeTimer = 0.0f;
        public static float baseGauge = 0f;

        static bool _isStalking = false;

        static Sprite stareSprite;
        public static Sprite getStareSprite()
        {
            if (stareSprite) return stareSprite;
            stareSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.KataomoiStareButton.png", 115f);
            return stareSprite;
        }

        static Sprite loveSprite;
        public static Sprite getLoveSprite()
        {
            if (loveSprite) return loveSprite;
            loveSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.KataomoiLoveButton.png", 115f);
            return loveSprite;
        }

        static Sprite searchSprite;
        public static Sprite getSearchSprite()
        {
            if (searchSprite) return searchSprite;
            searchSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.KataomoiSearchButton.png", 115f);
            return searchSprite;
        }

        static Sprite stalkingSprite;
        public static Sprite getStalkingSprite()
        {
            if (stalkingSprite) return stalkingSprite;
            stalkingSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.KataomoiStalkingButton.png", 115f);
            return stalkingSprite;
        }

        static Sprite[] loveGaugeSprites = new Sprite[3];
        public static Sprite getLoveGaugeSprite(int index)
        {
            if (index < 0 || index >= loveGaugeSprites.Length) return null;
            if (loveGaugeSprites[index]) return loveGaugeSprites[index];

            int id = 0;
            switch (index)
            {
                case 0: id = 1; break;
                case 1: id = 2; break;
                case 2: id = 11; break;
            }
            loveGaugeSprites[index] = Helpers.loadSpriteFromResources(String.Format("TheOtherRoles.Resources.KataomoiGauge_{0:d2}.png", id), 115f);
            return loveGaugeSprites[index];
        }

        public static void doStare()
        {
            baseGauge = getLoveGauge();
            gaugeTimer = 1.0f;
            stareCount = Mathf.Max(0, stareCount - 1);

            if (gaugeRenderer[2] != null && stareCount == 0)
            {
                gaugeRenderer[2].color = color;
            }
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
        }

        public static void doStalking()
        {
            if (kataomoi == null) return;
            stalkingTimer = stalkingDuration;
            _isStalking = true;
        }

        public static void resetStalking()
        {
            if (kataomoi == null) return;
            _isStalking = false;
            setAlpha(1.0f);
        }

        public static bool isStalking(PlayerControl player)
        {
            if (player == null || player != kataomoi) return false;
            return _isStalking && stalkingTimer > 0;
        }

        public static bool isStalking()
        {
            return isStalking(kataomoi);
        }

        public static void doSearch()
        {
            if (kataomoi == null) return;
            isSearch = true;
        }

        public static void resetSearch()
        {
            if (kataomoi == null) return;
            isSearch = false;
        }

        public static bool canLove()
        {
            return stareCount <= 0;
        }

        public static float getLoveGauge()
        {
            return 1.0f - (stareCountMax == 0 ? 0f : (float)stareCount / (float)stareCountMax);
        }

        public static void resetAllArrow()
        {
            if (PlayerControl.LocalPlayer != kataomoi) return;
            TORMapOptions.resetPoolables();
            for (int i = 0; i < gaugeRenderer.Length; ++i)
            {
                if (gaugeRenderer[i] != null)
                {
                    gaugeRenderer[i].gameObject.SetActive(false);
                }
            }
            if (stareText != null) stareText.gameObject.SetActive(false);
        }

        public static void clearAndReload()
        {
            resetStalking();

            kataomoi = null;
            stareCooldown = CustomOptionHolder.kataomoiStareCooldown.getFloat();
            stareDuration = CustomOptionHolder.kataomoiStareDuration.getFloat();
            stareCount = stareCountMax = (int)CustomOptionHolder.kataomoiStareCount.getFloat();
            stalkingCooldown = CustomOptionHolder.kataomoiStalkingCooldown.getFloat();
            stalkingDuration = CustomOptionHolder.kataomoiStalkingDuration.getFloat();
            stalkingFadeTime = CustomOptionHolder.kataomoiStalkingFadeTime.getFloat();
            searchCooldown = CustomOptionHolder.kataomoiSearchCooldown.getFloat();
            searchDuration = CustomOptionHolder.kataomoiSearchDuration.getFloat();
            isSearch = false;
            stalkingTimer = 0f;
            stalkingEffectTimer = 0f;
            triggerKataomoiWin = false;
            target = null;
            currentTarget = null;
            TORMapOptions.resetPoolables();
            if (stareText != null) UnityEngine.Object.Destroy(stareText);
            stareText = null;
            if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            for (int i = 0; i < gaugeRenderer.Length; ++i)
            {
                if (gaugeRenderer[i] != null)
                {
                    UnityEngine.Object.Destroy(gaugeRenderer[i].gameObject);
                    gaugeRenderer[i] = null;
                }
            }
            arrow = null;
            gaugeTimer = 0.0f;
            baseGauge = 0.0f;
        }

        public static void fixedUpdate(PlayerPhysics __instance)
        {
            if (kataomoi == null) return;
            if (kataomoi != __instance.myPlayer) return;

            if (gaugeRenderer[1] != null && gaugeTimer > 0.0f)
            {
                gaugeTimer = Mathf.Max(gaugeTimer - Time.fixedDeltaTime, 0.0f);
                float gauge = getLoveGauge();
                float nowGauge = Mathf.Lerp(baseGauge, gauge, 1.0f - gaugeTimer);
                gaugeRenderer[1].transform.localPosition = new Vector3(Mathf.Lerp(-3.470784f - 1.121919f + 1.25f, -3.470784f + 1.25f, nowGauge), -2.626999f, -8.1f);
                gaugeRenderer[1].transform.localScale = new Vector3(nowGauge, 1, 1);
            }

            if (kataomoi.Data.IsDead) return;
            if (_isStalking && stalkingTimer > 0)
            {
                kataomoi.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                stalkingTimer = Mathf.Max(0f, stalkingTimer - Time.fixedDeltaTime);
                if (stalkingFadeTime > 0)
                {
                    float elapsedTime = stalkingDuration - stalkingTimer;
                    float alpha = Mathf.Min(elapsedTime, stalkingFadeTime) / stalkingFadeTime;
                    alpha = Mathf.Clamp(1f - alpha, PlayerControl.LocalPlayer == kataomoi || PlayerControl.LocalPlayer.Data.IsDead && !(PlayerControl.LocalPlayer == Busker.busker && Busker.pseudocideFlag)
                        || (PlayerControl.LocalPlayer == Lighter.lighter && Lighter.canSeeInvisible) ? 0.5f : 0f, 1f);
                    setAlpha(alpha);
                }
                else
                {
                    setAlpha(PlayerControl.LocalPlayer == kataomoi ? 0.5f : 0f);
                }

                if (stalkingTimer <= 0f)
                {
                    _isStalking = false;
                    stalkingEffectTimer = stalkingFadeTime;
                }
            }
            else if (!_isStalking && stalkingEffectTimer > 0)
            {
                stalkingEffectTimer = Mathf.Max(0f, stalkingEffectTimer - Time.fixedDeltaTime);
                if (stalkingFadeTime > 0)
                {
                    float elapsedTime = stalkingFadeTime - stalkingEffectTimer;
                    float alpha = Mathf.Min(elapsedTime, stalkingFadeTime) / stalkingFadeTime;
                    alpha = Mathf.Clamp(alpha, PlayerControl.LocalPlayer == kataomoi || PlayerControl.LocalPlayer.Data.IsDead ? 0.5f : 0f, 1f);
                    setAlpha(alpha);
                }
                else
                {
                    setAlpha(1.0f);
                }
            }
            else
            {
                setAlpha(1.0f);
            }
        }

        static void setAlpha(float alpha)
        {
            if (kataomoi == null) return;
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, alpha);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == kataomoi.PlayerId) && Chameleon.visibility(kataomoi.PlayerId) < 1f && !isStalking()) return;
                Helpers.setInvisible(kataomoi, color, alpha);
            }
            catch { }
        }
    }

    public static class Husk
    {
        public static List<PlayerControl> husk = new();
        public static void clearAndReload()
        {
            husk = new List<PlayerControl>();
        }
    }

    public static class Yoyo
    {
        public static PlayerControl yoyo = null;
        public static Color color = Palette.ImpostorRed;

        public static float blinkDuration = 0;
        public static float markCooldown = 0;
        public static bool markStaysOverMeeting = false;
        public static float SilhouetteVisibility => (silhouetteVisibility == 0 && (PlayerControl.LocalPlayer == yoyo || PlayerControl.LocalPlayer.Data.IsDead)) ? 0.1f : silhouetteVisibility;
        public static float silhouetteVisibility = 0;

        public static Vector3? markedLocation = null;

        private static Sprite markButtonSprite;

        public static Sprite getMarkButtonSprite()
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.YoyoMarkButtonSprite.png", 115f);
            return markButtonSprite;
        }
        private static Sprite blinkButtonSprite;

        public static Sprite getBlinkButtonSprite()
        {
            if (blinkButtonSprite) return blinkButtonSprite;
            blinkButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.YoyoBlinkButtonSprite.png", 115f);
            return blinkButtonSprite;
        }

        public static void markLocation(Vector3 position)
        {
            markedLocation = position;
        }

        public static void clearAndReload()
        {
            yoyo = null;
            blinkDuration = CustomOptionHolder.yoyoBlinkDuration.getFloat();
            markCooldown = CustomOptionHolder.yoyoMarkCooldown.getFloat();
            markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.getBool();
            silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.getSelection() / 10f;
            markedLocation = null;
            Silhouette.clearSilhouettes();
        }
    }

    public static class EvilHacker
    {
        public static PlayerControl evilHacker;
        public static Color color = Palette.ImpostorRed;
        public static bool canHasBetterAdmin = false;
        public static bool canCreateMadmate = false;
        public static bool canSeeDoorStatus = true;
        public static bool canCreateMadmateFromJackal;
        public static bool canInheritAbility;
        public static PlayerControl fakeMadmate;
        public static PlayerControl currentTarget;

        public static AchievementToken<(bool admin, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (evilHacker == null || PlayerControl.LocalPlayer != evilHacker) return;
            acTokenChallenge ??= new("evilHacker.challenge", (false, false), (val, _) => val.cleared);
        }

        private static Sprite buttonSprite;
        private static Sprite madmateButtonSprite;

        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            buttonSprite = button.Image;
            return buttonSprite;
        }

        public static Sprite getMadmateButtonSprite()
        {
            if (madmateButtonSprite) return madmateButtonSprite;
            madmateButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
            return madmateButtonSprite;
        }

        public static bool isInherited()
        {
            return canInheritAbility && evilHacker != null && evilHacker.Data.IsDead && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
        }

        public static void clearAndReload()
        {
            evilHacker = null;
            currentTarget = null;
            fakeMadmate = null;
            canCreateMadmate = CustomOptionHolder.evilHackerCanCreateMadmate.getBool();
            canHasBetterAdmin = CustomOptionHolder.evilHackerCanHasBetterAdmin.getBool();
            canCreateMadmateFromJackal = CustomOptionHolder.evilHackerCanCreateMadmateFromJackal.getBool();
            canInheritAbility = CustomOptionHolder.evilHackerCanInheritAbility.getBool();
            canSeeDoorStatus = CustomOptionHolder.evilHackerCanSeeDoorStatus.getBool();
            acTokenChallenge = null;
        }
    }

    public static class Blackmailer
    {
        public static PlayerControl blackmailer;
        public static Color color = Palette.ImpostorRed;
        public static Color blackmailedColor = Palette.White;

        public static bool alreadyShook = false;
        public static PlayerControl blackmailed;
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;

        public static AchievementToken<(List<byte> witness, bool cleared)> acTokenChallenge;
        private static Sprite blackmailButtonSprite;
        private static Sprite overlaySprite;

        public static void onAchievementActivate()
        {
            if (blackmailer == null || PlayerControl.LocalPlayer != blackmailer) return;
            acTokenChallenge ??= new("blackmailer.challenge", (new List<byte>(), false), (val, _) => val.cleared);
        }

        public static Sprite getBlackmailOverlaySprite()
        {
            if (overlaySprite) return overlaySprite;
            overlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerOverlay.png", 100f);
            return overlaySprite;
        }

        public static Sprite getBlackmailButtonSprite()
        {
            if (blackmailButtonSprite) return blackmailButtonSprite;
            blackmailButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerBlackmailButton.png", 115f);
            return blackmailButtonSprite;
        }

        public static void clearAndReload()
        {
            blackmailer = null;
            currentTarget = null;
            blackmailed = null;
            alreadyShook = false;
            cooldown = CustomOptionHolder.blackmailerCooldown.getFloat();
        }
    }

    public static class Sherlock
    {
        public static PlayerControl sherlock;
        public static Color color = new Color32(248, 205, 70, byte.MaxValue);

        public static int numTasks = 2;
        public static float cooldown = 10f;
        public static float investigateDistance = 5f;

        public static int numUsed;
        public static int killTimerCounter;

        public static List<Tuple<byte, Tuple<byte, Vector3>>> killLog;

        public static int numInvestigate = 0;
        public static PlayerControl currentTarget;

        private static Sprite watchIcon;
        private static Sprite investigateIcon;

        public static AchievementToken<bool> acTokenChallenge = null;

        public static HideAndSeekDeathPopup killPopup = null;

        public static void onAchievementActivate()
        {
            if (sherlock == null || PlayerControl.LocalPlayer != sherlock) return;
            acTokenChallenge ??= new("sherlock.challenge", false, (val, _) => val);
        }

        public static Sprite getInvestigateIcon()
        {
            if (investigateIcon) return investigateIcon;
            investigateIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SherlockInvestigate.png", 115f);
            return investigateIcon;
        }
        public static Sprite getWatchIcon()
        {
            if (watchIcon) return watchIcon;
            watchIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SherlockWatch.png", 115f);
            return watchIcon;
        }

        private static TMPro.TMP_Text text;

        public static void investigateMessage(string message, float duration, Color color)
        {

            RoomTracker roomTracker = HudManager.Instance?.roomTracker;

            if (roomTracker != null)

            {
                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(HudManager.Instance.transform);
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                gameObject.transform.localScale *= 1.5f;

                text = gameObject.GetComponent<TMPro.TMP_Text>();
                text.text = message;
                text.color = color;

                HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
                {
                    if (p == 1f && text != null && text.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(text.gameObject);
                    }
                })));
            }
        }

        public static int getNumInvestigate()
        {
            int counter = sherlock.Data.Tasks.ToArray().Where(t => t.Complete).Count();
            return (int)Math.Floor((float)counter / numTasks);
        }

        public static void clearAndReload()
        {
            sherlock = null;
            killPopup = null;
            numUsed = 0;
            killLog = new();
            numTasks = Mathf.RoundToInt(CustomOptionHolder.sherlockRechargeTasksNumber.getFloat());
            cooldown = CustomOptionHolder.sherlockCooldown.getFloat();
            investigateDistance = CustomOptionHolder.sherlockInvestigateDistance.getFloat();
            acTokenChallenge = null;
        }
    }

    public static class Veteran
    {
        public static PlayerControl veteran;
        public static Color color = new Color32(255, 77, 0, byte.MaxValue);

        public static float alertDuration = 3f;
        public static float cooldown = 30f;

        public static int remainingAlerts = 5;

        public static bool alertActive = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AlertButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            veteran = null;
            alertActive = false;
            alertDuration = CustomOptionHolder.veteranAlertDuration.getFloat();
            cooldown = CustomOptionHolder.veteranCooldown.getFloat();
            remainingAlerts = Mathf.RoundToInt(CustomOptionHolder.veteranAlertNumber.getFloat());
        }
    }

    public static class Noisemaker
    {
        public static Color32 color = new(160, 131, 187, byte.MaxValue);
        public static PlayerControl noisemaker;
        public static PlayerControl currentTarget;
        public static PlayerControl target;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (noisemaker == null || PlayerControl.LocalPlayer != noisemaker) return;
            acTokenChallenge ??= new("noisemaker.challenge", 0, (val, _) => val >= 3);
        }

        public enum SoundTarget
        {
            Noisemaker,
            Crewmates,
            Everyone
        }

        public static float cooldown;
        public static float duration;
        public static int numSound;
        public static SoundTarget soundTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NoisemakerButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            noisemaker = null;
            cooldown = CustomOptionHolder.noisemakerCooldown.getFloat();
            duration = CustomOptionHolder.noisemakerSoundDuration.getFloat();
            numSound = Mathf.RoundToInt(CustomOptionHolder.noisemakerSoundNumber.getFloat());
            soundTarget = (SoundTarget)CustomOptionHolder.noisemakerSoundTarget.getSelection();
            acTokenChallenge = null;
        }
    }

    public static class JekyllAndHyde
    {
        public static Color color = Color.grey;
        public static PlayerControl jekyllAndHyde;
        public static PlayerControl formerJekyllAndHyde;
        public static PlayerControl currentTarget;

        public enum Status
        {
            None,
            Jekyll,
            Hyde,
        }

        public static Status status;
        public static int counter = 0;
        public static int numberToWin = 3;
        public static float suicideTimer = 40f;
        public static bool reset = true;
        public static float cooldown = 18f;
        public static int numUsed;
        public static bool oddIsJekyll;
        public static bool triggerWin = false;
        public static int numCommonTasks;
        public static int numLongTasks;
        public static int numShortTasks;
        public static int numTasks;

        public static bool isOdd(int n)
        {
            return n % 2 == 1;
        }

        public static bool isJekyll()
        {
            if (status == Status.None)
            {
                var alive = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x =>
                {
                    return !x.Data.IsDead;
                });
                bool ret = oddIsJekyll ? isOdd(alive.Count()) : !isOdd(alive.Count());
                return ret;
            }
            return status == Status.Jekyll;
        }

        public static int getNumDrugs()
        {
            var p = jekyllAndHyde;
            int counter = p.Data.Tasks.ToArray().Where(t => t.Complete).Count();
            return (int)Math.Floor((float)counter / numTasks);
        }

        public static void clearAndReload()
        {
            jekyllAndHyde = null;
            formerJekyllAndHyde = null;
            currentTarget = null;
            status = Status.None;
            counter = 0;
            triggerWin = false;
            numUsed = 0;
            numTasks = (int)CustomOptionHolder.jekyllAndHydeNumTasks.getFloat();
            numCommonTasks = (int)CustomOptionHolder.jekyllAndHydeCommonTasks.getFloat();
            numShortTasks = (int)CustomOptionHolder.jekyllAndHydeShortTasks.getFloat();
            numLongTasks = (int)CustomOptionHolder.jekyllAndHydeLongTasks.getFloat();
            reset = CustomOptionHolder.jekyllAndHydeResetAfterMeeting.getBool();
            numberToWin = (int)CustomOptionHolder.jekyllAndHydeNumberToWin.getFloat();
            cooldown = CustomOptionHolder.jekyllAndHydeCooldown.getFloat();
            suicideTimer = CustomOptionHolder.jekyllAndHydeSuicideTimer.getFloat();
        }
    }

    public static class Undertaker
    {
        public static PlayerControl undertaker;
        public static Color color = Palette.ImpostorRed;

        public static DeadBody DraggedBody;
        public static DeadBody TargetBody;
        public static bool CanDropBody;

        public static float speedDecrease = -50f;
        public static bool disableVent = true;

        public static Sprite dragButtonSprite;
        public static Sprite dropButtonSprite;

        public static void RpcDropBody(Vector3 position)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndertakerDropBody, SendOption.Reliable, -1);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DropBody(position);
        }

        public static void RpcDragBody(byte playerId)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndertakerDragBody, SendOption.Reliable, -1);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DragBody(playerId);
        }

        public static void DropBody(Vector3 position)
        {
            if (!DraggedBody) return;
            DraggedBody.transform.position = position;
            DraggedBody = null;
            TargetBody = null;
        }

        public static void DragBody(byte playerId)
        {
            if (undertaker == null) return;
            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == playerId);
            if (body == null) return;
            DraggedBody = body;
        }

        public static Sprite getDragButtonSprite()
        {
            if (dragButtonSprite) return dragButtonSprite;
            dragButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DragButton.png", 115f);
            return dragButtonSprite;
        }

        public static Sprite getDropButtonSprite()
        {
            if (dropButtonSprite) return dropButtonSprite;
            dropButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DropButton.png", 115f);
            return dropButtonSprite;
        }

        public static void clearAndReload()
        {
            undertaker = null;
            DraggedBody = null;
            TargetBody = null;

            speedDecrease = CustomOptionHolder.undertakerSpeedDecrease.getFloat();
            disableVent = CustomOptionHolder.undertakerDisableVent.getBool();
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

        public static void clearAndReload()
        {
            serialKiller = null;
            killCooldown = CustomOptionHolder.serialKillerKillCooldown.getFloat();
            suicideTimer = Mathf.Max(CustomOptionHolder.serialKillerSuicideTimer.getFloat(), killCooldown + 2.5f);
            resetTimer = CustomOptionHolder.serialKillerResetTimer.getBool();
            isCountDown = false;
        }
    }

    public static class Busker
    {
        public static PlayerControl busker;
        public static List<byte> buskerList;
        public static Color color = new(255f / 255f, 172f / 255f, 117f / 255f);

        public static float cooldown = 30f;
        public static float duration = 10f;
        public static bool pseudocideFlag = false;
        public static bool buttonInterrupted = false;
        public static bool pseudocideComplete = false;
        public static bool restrictInformation = true;
        public static Vector3 deathPosition = new();
        public static AchievementToken<(DateTime pseudocide, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (busker == null || PlayerControl.LocalPlayer != busker) return;
            acTokenChallenge ??= new("busker.challenge", (DateTime.UtcNow, false), (val, _) => val.cleared);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BuskerButton.png", 115f);
            return buttonSprite;
        }

        public static void dieBusker(bool isLoverSuicide = false)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BuskerPseudocide, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(true);
            writer.Write(isLoverSuicide);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.buskerPseudocide(PlayerControl.LocalPlayer.PlayerId, true, isLoverSuicide);
        }

        public static bool checkPseudocide()
        {
            if (!pseudocideFlag) return false;

            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            foreach (var deadBody in array)
            {
                if (deadBody.ParentId == PlayerControl.LocalPlayer.PlayerId)
                {
                    return true;
                }
            }
            dieBusker();
            return false;
        }

        public static void clearAndReload()
        {
            busker = null;
            buskerList = new List<byte>();
            cooldown = CustomOptionHolder.buskerCooldown.getFloat();
            duration = CustomOptionHolder.buskerDuration.getFloat();
            restrictInformation = CustomOptionHolder.buskerRestrictInformation.getBool();
            pseudocideFlag = false;
            buttonInterrupted = false;
            pseudocideComplete = false;
            deathPosition = new Vector3();
            acTokenChallenge = null;
        }
    }

    public static class Prophet
    {
        public static PlayerControl prophet;
        public static Color32 color = new(255, 204, 127, byte.MaxValue);

        public static float cooldown = 30f;
        public static float accuracy = 20f;
        public static bool canCallEmergency = false;
        public static int examineNum = 3;
        public static int examinesToBeRevealed = 1;
        public static int examinesLeft;
        public static bool revealProphet = true;
        public static bool isRevealed = false;
        public static List<Arrow> arrows = new();

        public static Dictionary<PlayerControl, bool> examined = new();
        public static PlayerControl currentTarget;
        public static AchievementToken<(bool triggered, bool cleared)> acTokenEvil = null;

        public static void onAchievementActivate()
        {
            if (prophet == null || PlayerControl.LocalPlayer != prophet) return;
            acTokenEvil ??= new("prophet.challenge2", (false, true), (val, _) => val.triggered && val.cleared);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SeerButton.png", 115f);
            return buttonSprite;
        }

        public static bool isKiller(PlayerControl p)
        {
            var rand = rnd.Next(1, 101);
            return (Helpers.isEvil(p) && rand <= accuracy) || (!Helpers.isEvil(p) && rand > accuracy);
        }

        public static void clearAndReload()
        {
            prophet = null;
            currentTarget = null;
            isRevealed = false;
            examined = new Dictionary<PlayerControl, bool>();
            revealProphet = CustomOptionHolder.prophetIsRevealed.getBool();
            cooldown = CustomOptionHolder.prophetCooldown.getFloat();
            examineNum = Mathf.RoundToInt(CustomOptionHolder.prophetNumExamines.getFloat());
            accuracy = CustomOptionHolder.prophetAccuracy.getFloat();
            canCallEmergency = CustomOptionHolder.prophetCanCallEmergency.getBool();
            examinesToBeRevealed = Math.Min(examineNum, Mathf.RoundToInt(CustomOptionHolder.prophetExaminesToBeRevealed.getFloat()));
            examinesLeft = examineNum;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            acTokenEvil = null;
        }
    }

    public static class EvilTracker
    {
        public static PlayerControl evilTracker;
        public static Color color = Palette.ImpostorRed;

        public static float cooldown = 10f;
        public static bool resetTargetAfterMeeting = true;
        public static bool canSeeDeathFlash = true;
        public static bool canSeeTargetPosition = true;
        public static bool canSetTargetOnMeeting = true;
        public static bool canSeeTargetTasks = false;

        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;

        public static PlayerControl target;
        public static PlayerControl futureTarget;
        public static PlayerControl currentTarget;
        public static Sprite trackerButtonSprite;
        public static Sprite arrowSprite;
        public static List<Arrow> arrows = new();
        public static Dictionary<string, TMPro.TMP_Text> impostorPositionText;
        public static TMPro.TMP_Text targetPositionText;

        public static Sprite getEvilTrackerButtonSprite()
        {
            if (trackerButtonSprite) return trackerButtonSprite;
            trackerButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
            return trackerButtonSprite;
        }

        public static Sprite getArrowSprite()
        {
            if (!arrowSprite)
                arrowSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Arrow.png", 300f);
            return arrowSprite;
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (arrows.Count > 0) {
                    foreach (var arrow in arrows)
                        if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                }
                if (impostorPositionText.Count > 0) {
                foreach (var p in impostorPositionText.Values)
                    if (p != null) UnityEngine.Object.Destroy(p.gameObject);
                }
                if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText.gameObject);
                target = null;
                return;
            }

            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow != null && arrow.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();

                // インポスターの位置を示すArrowsを描画
                int count = 0;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead)
                    {
                        if ((p.Data.Role.IsImpostor || p == Spy.spy || (p == Sidekick.sidekick && Sidekick.wasTeamRed)
                        || (p == Jackal.jackal && Jackal.wasTeamRed)) && impostorPositionText.ContainsKey(p.Data.PlayerName))
                        {
                            impostorPositionText[p.Data.PlayerName].text = "";
                        }
                        continue;
                    }
                    Arrow arrow;
                    if ((p.Data.Role.IsImpostor && p != PlayerControl.LocalPlayer) || (Spy.spy != null && p == Spy.spy) || (p == Sidekick.sidekick && Sidekick.wasTeamRed)
                        || (p == Jackal.jackal && Jackal.wasTeamRed))
                    {
                        arrow = new Arrow(Palette.ImpostorRed);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                        count += 1;
                        if (!impostorPositionText.ContainsKey(p.Data.PlayerName))
                        {
                            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                            if (roomTracker == null) return;
                            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                            gameObject.transform.localPosition = new Vector3(0, -2.0f + 0.25f * count, gameObject.transform.localPosition.z);
                            gameObject.transform.localScale = Vector3.one * 1.0f;
                            TMPro.TMP_Text positionText = gameObject.GetComponent<TMPro.TMP_Text>();
                            positionText.alpha = 1.0f;
                            impostorPositionText.Add(p.Data.PlayerName, positionText);
                        }
                        PlainShipRoom room = Helpers.getPlainShipRoom(p);
                        impostorPositionText[p.Data.PlayerName].gameObject.SetActive(true);
                        if (room != null)
                        {
                            impostorPositionText[p.Data.PlayerName].text = "<color=#FF1919FF>" + $"{p.Data.PlayerName}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                        }
                        else
                        {
                            impostorPositionText[p.Data.PlayerName].text = "";
                        }
                    }
                }

                // ターゲットの位置を示すArrowを描画
                if (target != null && !target.Data.IsDead)
                {
                    Arrow arrow = new(Palette.CrewmateBlue);
                    arrow.arrow.SetActive(true);
                    arrow.Update(target.transform.position);
                    arrows.Add(arrow);
                    if (targetPositionText == null)
                    {
                        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        targetPositionText.alpha = 1.0f;
                    }
                    PlainShipRoom room = Helpers.getPlainShipRoom(target);
                    targetPositionText.gameObject.SetActive(true);
                    if (room != null)
                    {
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    }
                    else
                    {
                        targetPositionText.text = "";
                    }
                }
                else
                {
                    if (targetPositionText != null)
                    {
                        targetPositionText.text = "";
                    }
                }

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void clearAllArrow()
        {
            if (PlayerControl.LocalPlayer != evilTracker) return;
            if (arrows.Count > 0) {
            foreach (var arrow in arrows)
                if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            }
            if (impostorPositionText.Count > 0) {
                foreach (var p in impostorPositionText.Values)
                    if (p != null) p.gameObject.SetActive(false);
            }
            if (targetPositionText != null) targetPositionText.gameObject.SetActive(false);
        }

        public static void clearAndReload()
        {
            evilTracker = null;
            target = null;
            futureTarget = null;
            currentTarget = null;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            if (impostorPositionText != null)
            {
                foreach (var p in impostorPositionText.Values)
                    if (p != null)
                        UnityEngine.Object.Destroy(p);
            }
            impostorPositionText = new();
            if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText);
            targetPositionText = null;

            cooldown = CustomOptionHolder.evilTrackerCooldown.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.evilTrackerResetTargetAfterMeeting.getBool();
            canSeeDeathFlash = CustomOptionHolder.evilTrackerCanSeeDeathFlash.getBool();
            canSeeTargetPosition = CustomOptionHolder.evilTrackerCanSeeTargetPosition.getBool();
            canSeeTargetTasks = CustomOptionHolder.evilTrackerCanSeeTargetTask.getBool();
            canSetTargetOnMeeting = CustomOptionHolder.evilTrackerCanSetTargetOnMeeting.getBool();
        }
    }
    public static class Armored
    {
        public static PlayerControl armored;

        public static bool isBrokenArmor = false;
        public static void clearAndReload()
        {
            armored = null;
            isBrokenArmor = false;
        }
    }

    public static class Shifter
    {
        public static PlayerControl shifter;
        public static List<int> pastShifters = new();
        public static Color color = new Color32(102, 102, 102, byte.MaxValue);

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;
        public static bool shiftModifiers = false;

        public static bool isNeutral = false;
        public static bool shiftPastShifters = false;

        public static AchievementToken<(byte shiftId, byte oldShifterId, bool cleared)> niceShifterAcTokenChallenge = null;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static void niceShifterOnAchievementActivate()
        {
            niceShifterAcTokenChallenge = null;
            if (shifter != null && PlayerControl.LocalPlayer == shifter && !isNeutral)
            {
                niceShifterAcTokenChallenge ??= new("niceShifter.challenge", (byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
            }
        }

        public static void clearAndReload()
        {
            shifter = null;
            pastShifters = new List<int>();
            currentTarget = null;
            futureShift = null;
            shiftModifiers = CustomOptionHolder.shifterShiftsModifiers.getBool();
            shiftPastShifters = CustomOptionHolder.shifterPastShifters.getBool();
            isNeutral = false;
        }
    }

    public static class Witch {
        public static PlayerControl witch;
        public static Color color = Palette.ImpostorRed;

        public static List<PlayerControl> futureSpelled = new();
        public static PlayerControl currentTarget;
        public static PlayerControl spellCastingTarget;
        public static float cooldown = 30f;
        public static float spellCastingDuration = 2f;
        public static float cooldownAddition = 10f;
        public static float currentCooldownAddition = 0f;
        public static bool canSpellAnyone = false;
        public static bool triggerBothCooldowns = true;
        public static bool witchVoteSavesTargets = true;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (witch == null || PlayerControl.LocalPlayer != witch) return;
            acTokenChallenge ??= new("witch.challenge", 0, (val, _) => val >= 3);
        }

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
            acTokenChallenge = null;
        }
    }

    public static class Watcher
    {
        public static PlayerControl nicewatcher;
        public static PlayerControl evilwatcher;
        public static Color color = Palette.Purple;
        public static bool canSeeGuesses = false;

        public static void clear(byte playerId)
        {
            if (nicewatcher != null && nicewatcher.PlayerId == playerId) nicewatcher = null;
            else if (evilwatcher != null &&  evilwatcher.PlayerId == playerId) evilwatcher = null;
        }

        public static void clearAndReload()
        {
            nicewatcher = null;
            evilwatcher = null;
            canSeeGuesses = CustomOptionHolder.watcherSeeGuesses.getBool();
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

        public static AchievementToken<(byte killerId, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (bait == null || PlayerControl.LocalPlayer != bait) return;
            acTokenChallenge ??= new("bait.challenge", (byte.MaxValue, false), (val, _) => val.cleared);
        }

        public static void clearAndReload()
        {
            bait = null;
            reported = false;
            highlightAllVents = CustomOptionHolder.baitHighlightAllVents.getBool();
            reportDelay = CustomOptionHolder.baitReportDelay.getFloat();
            showKillFlash = CustomOptionHolder.baitShowKillFlash.getBool();
            acTokenChallenge = null;
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
        public static Arrow arrow = new(Color.black);

        public static AchievementToken<(bool markKill, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (assassin == null || PlayerControl.LocalPlayer != assassin) return;
            acTokenChallenge ??= new("assassin.challenge", (false, false), (val, _) => val.cleared);
        }

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
            acTokenChallenge = null;
        }
    }

    public static class Moriarty
    {
        public static PlayerControl moriarty;
        public static PlayerControl formerMoriarty;
        public static Color color = Color.green;

        public static PlayerControl tmpTarget;
        public static PlayerControl target;
        public static PlayerControl currentTarget;
        public static PlayerControl killTarget;
        public static List<PlayerControl> brainwashed = new();

        public static int counter;

        public static float brainwashTime = 2f;
        public static float brainwashCooldown = 30f;
        public static int numberToWin = 3;
        public static bool indicateKills = false;
        public static bool hasKilled = false;

        public static Sprite brainwashIcon;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static TMPro.TMP_Text targetPositionText;

        public static bool triggerMoriartyWin = false;

        public static Sprite getBrainwashIcon()
        {
            if (brainwashIcon) return brainwashIcon;
            brainwashIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BrainwashButton.png", 115f);
            return brainwashIcon;
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (arrows.Count > 0)
                {
                    foreach (var arrow in arrows)
                        if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                }
                if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText.gameObject);
                target = null;
                return;
            }

            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow != null && arrow.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();
                // ターゲットの位置を示すArrowを描画
                if (target != null && !target.Data.IsDead)
                {
                    Arrow arrow = new(Palette.CrewmateBlue);
                    arrow.arrow.SetActive(true);
                    arrow.Update(target.transform.position);
                    arrows.Add(arrow);
                    if (targetPositionText == null)
                    {
                        RoomTracker roomTracker = HudManager.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(HudManager.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        targetPositionText.alpha = 1.0f;
                    }
                    PlainShipRoom room = Helpers.getPlainShipRoom(target);
                    targetPositionText.gameObject.SetActive(true);
                    int nearestPlayer = 0;
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (p != target && !p.Data.IsDead)
                        {
                            float dist = Vector2.Distance(p.transform.position, target.transform.position);
                            if (dist < 7f) nearestPlayer += 1;
                        }
                    }
                    if (room != null)
                    {
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}({nearestPlayer})(" + DestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    }
                    else
                    {
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}({nearestPlayer})</color>";
                    }
                }
                else
                {
                    if (targetPositionText != null)
                    {
                        targetPositionText.text = "";
                    }
                }

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void clearAllArrow()
        {
            if (PlayerControl.LocalPlayer != moriarty) return;
            if (arrows.Count > 0)
            {
                foreach (var arrow in arrows)
                    if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            }
            if (targetPositionText != null) targetPositionText.gameObject.SetActive(false);
            var obj = GameObject.Find("MoriartyText(Clone)");
            if (obj != null) obj.SetActive(false);
        }

        public static void generateBrainwashText()
        {
            TMPro.TMP_Text text;
            RoomTracker roomTracker = HudManager.Instance?.roomTracker;
            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            gameObject.transform.SetParent(HudManager.Instance.transform);
            gameObject.transform.localPosition = new Vector3(0, -1.3f, gameObject.transform.localPosition.z);
            gameObject.transform.localScale = Vector3.one * 3f;
            text = gameObject.GetComponent<TMPro.TMP_Text>();
            text.name = "MoriartyText(Clone)";
            PlayerControl tmpP = target;
            bool done = false;
            HudManager.Instance.StartCoroutine(Effects.Lerp(brainwashTime, new Action<float>((p) =>
            {
                if (done)
                {
                    return;
                }
                if (target == null || MeetingHud.Instance != null || p == 1f)
                {
                    if (text != null && text.gameObject) UnityEngine.Object.Destroy(text.gameObject);
                    if (target == tmpP) target = null;
                    done = true;
                    return;
                }
                else
                {
                    string message = (brainwashTime - (p * brainwashTime)).ToString("0");
                    bool even = ((int)(p * brainwashTime / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds
                                                                                      // string prefix = even ? "<color=#555555FF>" : "<color=#FFFFFFFF>";
                    string prefix = "<color=#555555FF>";
                    text.text = prefix + message + "</color>";
                    if (text != null) text.color = even ? Color.yellow : Color.red;
                }
            })));
        }

        public static void clearAndReload()
        {
            moriarty = null;
            formerMoriarty = null;
            tmpTarget = null;
            target = null;
            currentTarget = null;
            killTarget = null;
            brainwashed = new List<PlayerControl>();
            counter = 0;
            triggerMoriartyWin = false;
            hasKilled = false;
            brainwashCooldown = CustomOptionHolder.moriartyBrainwashCooldown.getFloat();
            brainwashTime = CustomOptionHolder.moriartyBrainwashTime.getFloat();
            numberToWin = (int)CustomOptionHolder.moriartyNumberToWin.getFloat();
            indicateKills = CustomOptionHolder.moriartyKillIndicate.getBool();

            if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText);
            targetPositionText = null;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();

            var obj = GameObject.Find("MoriartyText(Clone)");
            if (obj != null) UnityEngine.Object.Destroy(obj);
        }
    }

    public static class Akujo
    {
        public static Color color = new Color32(142, 69, 147, byte.MaxValue);
        public static PlayerControl akujo;
        public static PlayerControl honmei;
        public static List<PlayerControl> keeps;
        public static PlayerControl currentTarget;
        public static DateTime startTime;

        public static float timeLimit = 1300f;
        public static bool knowsRoles = true;
        public static int timeLeft;
        public static int keepsLeft;
        public static int numKeeps;

        private static Sprite honmeiSprite;
        public static Sprite getHonmeiSprite()
        {
            if (honmeiSprite) return honmeiSprite;
            honmeiSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AkujoHonmeiButton.png", 115f);
            return honmeiSprite;
        }

        private static Sprite keepSprite;
        public static Sprite getKeepSprite()
        {
            if (keepSprite) return keepSprite;
            keepSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AkujoKeepButton.png", 115f);
            return keepSprite;
        }

        public static void breakLovers(PlayerControl lover)
        {
            if ((Lovers.lover1 != null && lover == Lovers.lover1) || (Lovers.lover2 != null && lover == Lovers.lover2))
            {
                PlayerControl otherLover = lover.getPartner();
                if (otherLover != null)
                {
                    Lovers.clearAndReload();
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
                }
            }
        }

        public static void clearAndReload()
        {
            akujo = null;
            honmei = null;
            keeps = new List<PlayerControl>();
            currentTarget = null;
            startTime = DateTime.UtcNow;
            timeLimit = CustomOptionHolder.akujoTimeLimit.getFloat() + 10f;
            knowsRoles = CustomOptionHolder.akujoKnowsRoles.getBool();
            timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
            numKeeps = Math.Min((int)CustomOptionHolder.akujoNumKeeps.getFloat(), PlayerControl.AllPlayerControls.Count - 2);
            keepsLeft = numKeeps;
        }
    }

    public static class Cupid
    {
        public static PlayerControl cupid;
        public static Color color = new Color32(246, 152, 150, byte.MaxValue);

        public static PlayerControl lovers1;
        public static PlayerControl lovers2;
        public static PlayerControl shielded;
        public static PlayerControl currentTarget;
        public static PlayerControl shieldTarget;
        public static DateTime startTime = DateTime.UtcNow;
        public static bool isShieldOn = false;
        public static int timeLeft;
        public static float timeLimit;

        private static Sprite arrowSprite;
        public static Sprite getArrowSprite()
        {
            if (arrowSprite) return arrowSprite;
            arrowSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CupidButton.png", 115f);
            return arrowSprite;
        }

        public static bool isCupidLover(this PlayerControl p) => (p == lovers1 && lovers2 != null) || (p == lovers2 && lovers1 != null);
        public static PlayerControl getCupidLover(this PlayerControl p)
        {
            if (p == null) return null;
            if (p == lovers1) return lovers2;
            if (p == lovers2) return lovers1;
            return null;
        }

        public static void breakLovers(PlayerControl lover)
        {
            if ((Lovers.lover1 != null && lover == Lovers.lover1) || (Lovers.lover2 != null && lover == Lovers.lover2))
            {
                PlayerControl otherLover = lover.getPartner();
                if (otherLover != null && !otherLover.Data.IsDead)
                {
                    Lovers.clearAndReload();
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
                }
            }
        }

        public static void clearAndReload(bool resetLovers = true)
        {
            cupid = null;
            if (resetLovers)
            {
                lovers1 = null;
                lovers2 = null;
            }
            shielded = null;
            currentTarget = null;
            shieldTarget = null;
            startTime = DateTime.UtcNow;
            timeLimit = CustomOptionHolder.cupidTimeLimit.getFloat() + 10f;
            timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
            isShieldOn = CustomOptionHolder.cupidShield.getBool();
        }
    }

    public static class Fox
    {
        public static PlayerControl fox;
        public static Color color = new Color32(167, 87, 168, byte.MaxValue);

        public enum TaskType
        {
            Serial,
            Parallel
        }

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static bool crewWinsByTasks = false;
        public static bool impostorWinsBySabotage = true;
        public static float stealthCooldown;
        public static float stealthDuration;
        public static int numTasks;
        public static float stayTime;

        public static bool stealthed = false;
        public static DateTime stealthedAt = DateTime.UtcNow;
        public static float fadeTime = 1f;

        public static int numRepair = 0;
        public static bool canCreateImmoralist;
        public static PlayerControl currentTarget;
        public static TaskType taskType;

        private static Sprite hideButtonSprite;
        private static Sprite repairButtonSprite;
        private static Sprite immoralistButtonSprite;

        public static Sprite getHideButtonSprite()
        {
            if (hideButtonSprite) return hideButtonSprite;
            hideButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FoxHideButton.png", 115f);
            return hideButtonSprite;
        }

        public static Sprite getRepairButtonSprite()
        {
            if (repairButtonSprite) return repairButtonSprite;
            repairButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.RepairButton.png", 115f);
            return repairButtonSprite;
        }

        public static Sprite getImmoralistButtonSprite()
        {
            if (immoralistButtonSprite) return immoralistButtonSprite;
            immoralistButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.FoxImmoralistButton.png", 115f);
            return immoralistButtonSprite;
        }

        public static float stealthFade()
        {
            if (fox != null && !fox.Data.IsDead)
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - stealthedAt).TotalSeconds / fadeTime);
            return 1.0f;
        }

        public static void setStealthed(bool stealthed = true)
        {
            Fox.stealthed = stealthed;
            stealthedAt = DateTime.UtcNow;
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
                Helpers.setInvisible(player, color, opacity);
            }
            catch { }
        }

        public static bool tasksComplete()
        {
            if (fox == null) return false;
            if (fox.Data.IsDead) return false;
            int counter = 0;
            int totalTasks = 1;
            foreach (var task in fox.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter == totalTasks;
        }

        public static void arrowUpdate()
        {
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();

                // インポスターの位置を示すArrowsを描画
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    // float distance = Vector2.Distance(p.transform.position, PlayerControl.LocalPlayer.transform.position);
                    if (p.Data.Role.IsImpostor || p == Jackal.jackal || p == Sheriff.sheriff || p == JekyllAndHyde.jekyllAndHyde || p == Moriarty.moriarty || p == Thief.thief || (p == SchrodingersCat.schrodingersCat && SchrodingersCat.hasTeam()))
                    {
                        if (p.Data.Role.IsImpostor)
                        {
                            arrow = new Arrow(Palette.ImpostorRed);
                        }
                        else if (p == Jackal.jackal)
                        {
                            arrow = new Arrow(Jackal.color);
                        }
                        else if (p == Sheriff.sheriff)
                        {
                            arrow = new Arrow(Palette.White);
                        }
                        else if (p == JekyllAndHyde.jekyllAndHyde)
                        {
                            arrow = new Arrow(JekyllAndHyde.color);
                        }
                        else if (p == Moriarty.moriarty)
                        {
                            arrow = new Arrow(Moriarty.color);
                        }
                        else if (p == SchrodingersCat.schrodingersCat && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate)
                        {
                            arrow = new Arrow(RoleInfo.schrodingersCat.color);
                        }
                        else
                        {
                            arrow = new Arrow(Thief.color);
                        }
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
            else
            {
                arrows.Do(x => x.Update());
            }
        }

        public static void clearAndReload()
        {
            setOpacity(fox, 1.0f);
            fox = null;
            currentTarget = null;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
            crewWinsByTasks = CustomOptionHolder.foxCrewWinsByTasks.getBool();
            impostorWinsBySabotage = CustomOptionHolder.foxImpostorWinsBySabotage.getBool();
            stealthCooldown = CustomOptionHolder.foxStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.foxStealthDuration.getFloat();
            canCreateImmoralist = CustomOptionHolder.foxCanCreateImmoralist.getBool();
            numTasks = (int)CustomOptionHolder.foxNumTasks.getFloat();
            numRepair = (int)CustomOptionHolder.foxNumRepairs.getFloat();
            stayTime = (int)CustomOptionHolder.foxStayTime.getFloat();
            taskType = (TaskType)CustomOptionHolder.foxTaskType.getSelection();
            foreach (Arrow arrow in arrows)
            {
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            }
            arrows = new List<Arrow>();
            Immoralist.clearAndReload();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsFoxPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (fox != null && fox == __instance.myPlayer)
                {
                    var fox = __instance.myPlayer;
                    if (fox == null || fox.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer == fox ||
                        (PlayerControl.LocalPlayer.Data.IsDead && !(PlayerControl.LocalPlayer == Busker.busker
                        && Busker.pseudocideFlag && Busker.restrictInformation)) ||
                        (PlayerControl.LocalPlayer == Lighter.lighter && Lighter.canSeeInvisible) ||
                        PlayerControl.LocalPlayer == Immoralist.immoralist;

                    var opacity = canSee ? 0.5f : 0.0f;

                    if (stealthed)
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade());
                        fox.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade());
                    }

                    setOpacity(fox, opacity);
                }
            }
        }
    }

    public static class Immoralist
    {
        public static PlayerControl immoralist;
        public static Color color = Fox.color;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 1f;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
            return buttonSprite;
        }

        public static void arrowUpdate()
        {
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {
                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrow一覧
                arrows = new List<Arrow>();

                // 狐の位置を示すArrowを描画
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    if (p == Fox.fox)
                    {
                        arrow = new Arrow(Fox.color);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
            else
            {
                arrows.Do(x => x.Update());
            }
        }

        public static void clearAndReload()
        {
            immoralist = null;
            foreach (Arrow arrow in arrows)
            {
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            }
            arrows = new List<Arrow>();
        }
    }

    public static class PlagueDoctor
    {
        public static PlayerControl plagueDoctor;
        public static Color color = new Color32(255, 192, 0, byte.MaxValue);

        public static Dictionary<int, PlayerControl> infected;
        public static Dictionary<int, float> progress;
        public static Dictionary<int, bool> dead;
        public static TMPro.TMP_Text statusText = null;
        public static bool triggerPlagueDoctorWin = false;

        public static PlayerControl currentTarget;
        public static int numInfections = 0;
        public static bool meetingFlag = false;

        public static float infectCooldown = 10f;
        public static int maxInfectable;
        public static float infectDistance = 1f;
        public static float infectDuration = 5f;
        public static float immunityTime = 10f;
        public static bool infectKiller = true;
        public static bool canWinDead = true;

        public static Sprite plagueDoctorIcon;

        public static Sprite getSyringeIcon()
        {
            if (plagueDoctorIcon) return plagueDoctorIcon;
            plagueDoctorIcon = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.InfectButton.png", 115f);
            return plagueDoctorIcon;
        }

        public static void updateDead()
        {
            if (statusText != null) UnityEngine.Object.Destroy(statusText);
            statusText = null; // Update positions!
            foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                dead[pc.PlayerId] = pc.Data.IsDead;
            }
        }

        public static string getProgressString(float progress)
        {
            // Go from green -> yellow -> red based on infection progress
            Color color;
            var prog = progress / infectDuration;
            if (prog < 0.5f)
                color = Color.Lerp(Color.green, Color.yellow, prog * 2);
            else
                color = Color.Lerp(Color.yellow, Color.red, prog * 2 - 1);

            float progPercent = prog * 100;
            return Helpers.cs(color, $"{progPercent.ToString("F1")}%");
        }

        public static void checkWinStatus()
        {
            bool winFlag = true;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p.Data.IsDead) continue;
                if (p == plagueDoctor) continue;
                if (!infected.ContainsKey(p.PlayerId))
                {
                    winFlag = false;
                    break;
                }
            }

            if (winFlag)
            {
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlagueDoctorWin, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                RPCProcedure.plagueDoctorWin();
            }
        }

        public static void clearAndReload()
        {
            plagueDoctor = null;
            infectCooldown = CustomOptionHolder.plagueDoctorInfectCooldown.getFloat();
            maxInfectable = Mathf.RoundToInt(CustomOptionHolder.plagueDoctorNumInfections.getFloat());
            infectDistance = CustomOptionHolder.plagueDoctorDistance.getFloat();
            infectDuration = CustomOptionHolder.plagueDoctorDuration.getFloat();
            immunityTime = CustomOptionHolder.plagueDoctorImmunityTime.getFloat();
            infectKiller = CustomOptionHolder.plagueDoctorInfectKiller.getBool();
            canWinDead = CustomOptionHolder.plagueDoctorWinDead.getBool();
            meetingFlag = false;
            triggerPlagueDoctorWin = false;
            numInfections = maxInfectable;
            currentTarget = null;
            infected = new Dictionary<int, PlayerControl>();
            progress = new Dictionary<int, float>();
            dead = new Dictionary<int, bool>();
            if (statusText != null) UnityEngine.Object.Destroy(statusText);
            statusText = null;
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

        public static bool penalized = false;
        public static bool stealthed = false;
        public static DateTime stealthedAt = DateTime.UtcNow;
        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (ninja == null || PlayerControl.LocalPlayer != ninja) return;
            acTokenChallenge ??= new("ninja.challenge", 0, (val, _) => val >= 2);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NinjaButton.png", 115f);
            return buttonSprite;
        }

        public static bool isStealthed(PlayerControl player)
        {
            if (ninja != null && !ninja.Data.IsDead && ninja == player)
            {
                return stealthed;
            }
            return false;
        }

        public static float stealthFade(PlayerControl player)
        {
            if (ninja == player && fadeTime > 0 && !ninja.Data.IsDead)
            {
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - stealthedAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static void setStealthed(PlayerControl player, bool stealthed = true)
        {
            if (ninja == player && ninja != null)
            {
                Ninja.stealthed = stealthed;
                stealthedAt = DateTime.UtcNow;
            }
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                // Block setting opacity if the Chameleon skill is active
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
                Helpers.setInvisible(player, color, opacity);
            }
            catch { }
        }

        public static void clearAndReload()
        {
            setOpacity(ninja, 1.0f);
            ninja = null;
            stealthCooldown = CustomOptionHolder.ninjaStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.ninjaStealthDuration.getFloat();
            killPenalty = CustomOptionHolder.ninjaKillPenalty.getFloat();
            speedBonus = CustomOptionHolder.ninjaSpeedBonus.getFloat();
            fadeTime = CustomOptionHolder.ninjaFadeTime.getFloat();
            canUseVents = CustomOptionHolder.ninjaCanVent.getBool();
            canBeTargeted = CustomOptionHolder.ninjaCanBeTargeted.getBool();

            penalized = false;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;

            acTokenChallenge = null;
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

                if (__instance.myPlayer == ninja)
                {
                    var ninja = __instance.myPlayer;
                    if (ninja == null || ninja.Data.IsDead) return;

                    bool canSee =
                        (PlayerControl.LocalPlayer.Data.IsDead && !(PlayerControl.LocalPlayer == Busker.busker
                        && Busker.pseudocideFlag && Busker.restrictInformation)) ||
                        PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                        (Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter);

                    var opacity = canSee ? 0.5f : 0.0f;

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
        public static float speedBonus = 0.25f;

        public static bool sprinting = false;

        public static DateTime sprintAt = DateTime.UtcNow;

        public static AchievementToken<(Vector3 pos, bool cleared)> acTokenMove = null;

        public static void onAchievementActivate()
        {
            if (sprinter == null || PlayerControl.LocalPlayer != sprinter) return;
            if (sprintDuration <= 15f)
                acTokenMove ??= new("sprinter.common2", (Vector3.zero, false), (val, _) => val.cleared);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SprintButton.png", 115f);
            return buttonSprite;
        }

        public static float sprintFade(PlayerControl player)
        {
            if (sprinter == player && fadeTime > 0 && !sprinter.Data.IsDead)
            {
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - sprintAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static void setSprinting(PlayerControl player, bool sprinting = true)
        {
            if (player == sprinter && !sprinter.Data.IsDead)
            {
                Sprinter.sprinting = sprinting;
                sprintAt = DateTime.UtcNow;
            }
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !sprinting) return;
                Helpers.setInvisible(player, color, opacity);
            }
            catch { }
        }

        public static void clearAndReload()
        {
            setOpacity(sprinter, 1.0f);
            sprinter = null;
            sprinting = false;
            sprintCooldown = CustomOptionHolder.sprinterCooldown.getFloat();
            sprintDuration = CustomOptionHolder.sprinterDuration.getFloat();
            fadeTime = CustomOptionHolder.sprinterFadeTime.getFloat();
            speedBonus = CustomOptionHolder.sprinterSpeedBonus.getFloat();
            acTokenMove = null;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsSprinterPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.myPlayer == sprinter)
                {
                    if (GameData.Instance && sprinting && __instance.AmOwner && __instance.myPlayer.CanMove) __instance.body.velocity *= 1 + speedBonus;
                    var sprinter = __instance.myPlayer;
                    if (sprinter == null || sprinter.Data.IsDead) return;

                    bool canSee =
                        (PlayerControl.LocalPlayer.Data.IsDead && !(PlayerControl.LocalPlayer == Busker.busker
                        && Busker.pseudocideFlag && Busker.restrictInformation)) ||
                        PlayerControl.LocalPlayer == Sprinter.sprinter ||
                        (Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter);

                    var opacity = canSee ? 0.5f : 0.0f;

                    if (sprinting)
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
            return killer == thief && !target.Data.Role.IsImpostor && !new List<RoleInfo> { RoleInfo.jackal, canKillSheriff ? RoleInfo.sheriff : null, RoleInfo.sidekick, RoleInfo.moriarty, RoleInfo.jekyllAndHyde, SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate ? RoleInfo.schrodingersCat : 
                null}.Contains(targetRole);
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
        public static List<PlayerControl> bloody = new();
        public static Dictionary<byte, float> active = new();
        public static Dictionary<byte, byte> bloodyKillerMap = new();

        public static float duration = 5f;

        public static void clearAndReload() {
            bloody = new List<PlayerControl>();
            active = new Dictionary<byte, float>();
            bloodyKillerMap = new Dictionary<byte, byte>();
            duration = CustomOptionHolder.modifierBloodyDuration.getFloat();
        }
    }

    public static class AntiTeleport {
        public static List<PlayerControl> antiTeleport = new();
        public static Vector3 position;

        public static void clearAndReload() {
            antiTeleport = new List<PlayerControl>();
            position = Vector3.zero;
        }

        public static void setPosition() {
            if (position == Vector3.zero) return;  // Check if this has been set, otherwise first spawn on submerged will fail
            if (antiTeleport.FindAll(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId).Count > 0) {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
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
        public static List<PlayerControl> sunglasses = new();
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
        public static List<PlayerControl> vip = new();
        public static bool showColor = true;

        public static void clearAndReload() {
            vip = new List<PlayerControl>();
            showColor = CustomOptionHolder.modifierVipShowColor.getBool();
        }
    }

    public static class Invert {
        public static List<PlayerControl> invert = new();
        public static int meetings = 3;

        public static void clearAndReload() {
            invert = new List<PlayerControl>();
            meetings = (int) CustomOptionHolder.modifierInvertDuration.getFloat();
        }
    }

    public static class Madmate
    {
        public static Color color = Palette.ImpostorRed;
        public static List<PlayerControl> madmate = new();
        public static bool hasTasks;
        public static bool canDieToSheriff;
        public static bool canVent;
        public static bool hasImpostorVision;
        public static bool canFixComm;
        public static bool canSabotage;
        public static int commonTasks;
        public static int shortTasks;
        public static int longTasks;
        public static RoleId fixedRole;
        
        public static string fullName { get { return ModTranslation.getString("madmate"); } }
        public static string prefix { get { return ModTranslation.getString("madmatePrefix"); } }

        public static List<RoleId> validRoles = new()
        {
            RoleId.Jester,
            RoleId.Shifter,
            RoleId.Mayor,
            RoleId.Engineer,
            RoleId.Sheriff,
            RoleId.Lighter,
            RoleId.Detective,
            RoleId.TimeMaster,
            RoleId.Medic,
            RoleId.Swapper,
            RoleId.Seer,
            RoleId.Hacker,
            RoleId.Tracker,
            RoleId.SecurityGuard,
            RoleId.Bait,
            RoleId.Medium,
            RoleId.NiceGuesser,
            RoleId.NiceWatcher,
            RoleId.Busker,
            RoleId.Yasuna,
            RoleId.Noisemaker
        };

        public static bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = commonTasks + longTasks + shortTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter == totalTasks;
        }

        public static void clearAndReload()
        {
            hasTasks = CustomOptionHolder.madmateAbility.getBool();
            madmate = new List<PlayerControl>();
            canDieToSheriff = CustomOptionHolder.madmateCanDieToSheriff.getBool();
            canVent = CustomOptionHolder.madmateCanEnterVents.getBool();
            hasImpostorVision = CustomOptionHolder.madmateHasImpostorVision.getBool();
            canFixComm = CustomOptionHolder.madmateCanFixComm.getBool();
            canSabotage = CustomOptionHolder.madmateCanSabotage.getBool();
            shortTasks = (int)CustomOptionHolder.madmateShortTasks.getFloat();
            commonTasks = (int)CustomOptionHolder.madmateCommonTasks.getFloat();
            longTasks = (int)CustomOptionHolder.madmateLongTasks.getFloat();
            fixedRole = TORMapOptions.gameMode == CustomGamemodes.Guesser ? validRoles.Where(x => x != RoleId.NiceGuesser).ToList()[
                CustomOptionHolder.madmateFixedRoleGuesserGamemode.getSelection()] :
                validRoles[CustomOptionHolder.madmateFixedRole.getSelection()];
        }
    }

    public static class Chameleon {
        public static List<PlayerControl> chameleon = new();
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
            if ((playerId == Ninja.ninja?.PlayerId && Ninja.stealthed) || (playerId == Sprinter.sprinter?.PlayerId && Sprinter.sprinting)
                || (playerId == Fox.fox?.PlayerId && Fox.stealthed) || (playerId == Kataomoi.kataomoi?.PlayerId && Kataomoi.isStalking())) return 1f;
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
                if ((chameleonPlayer == Ninja.ninja && Ninja.stealthed) || (chameleonPlayer == Sprinter.sprinter && Sprinter.sprinting) || (chameleonPlayer == Fox.fox && Fox.stealthed) ||
                    (chameleonPlayer == Kataomoi.kataomoi && Kataomoi.isStalking())) continue;
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
                    foreach (var rend in chameleonPlayer.cosmetics.currentPet.renderers) rend.color = rend.color.SetAlpha(petVisibility);
                    foreach (var shadowRend in chameleonPlayer.cosmetics.currentPet.shadows) shadowRend.color = shadowRend.color.SetAlpha(petVisibility);

                    //if (chameleonPlayer.cosmetics.skin.layer.color == chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility) && visibility == minVisibility) TheOtherRolesPlugin.Logger.LogMessage("Chameleon");
                    //chameleonPlayer.cosmetics.currentPet.renderers[0].color = chameleonPlayer.cosmetics.currentPet.renderers[0].color.SetAlpha(petVisibility);
                    //chameleonPlayer.cosmetics.currentPet.shadows[0].color = chameleonPlayer.cosmetics.currentPet.shadows[0].color.SetAlpha(petVisibility);
                } catch { }
            }
                
        }

        public static void removeChameleonFully(PlayerControl player) {
            try
            {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                player.cosmetics.currentBodySprite.BodySprite.color = player.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(1f);
                if (DataManager.Settings.Accessibility.ColorBlindMode) player.cosmetics.colorBlindText.color = player.cosmetics.colorBlindText.color.SetAlpha(1f);
                player.SetHatAndVisorAlpha(1f);
                player.cosmetics.skin.layer.color = player.cosmetics.skin.layer.color.SetAlpha(1f);
                player.cosmetics.nameText.color = player.cosmetics.nameText.color.SetAlpha(1f);
                foreach (var rend in player.cosmetics.currentPet.renderers) rend.color = rend.color.SetAlpha(1f);
                foreach (var shadowRend in player.cosmetics.currentPet.shadows) shadowRend.color = shadowRend.color.SetAlpha(1f);
                if (lastMoved.ContainsKey(player.PlayerId)) lastMoved.Remove(player.PlayerId);
            }
            catch { }
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
