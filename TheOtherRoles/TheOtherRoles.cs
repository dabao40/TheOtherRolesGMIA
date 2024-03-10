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
using Steamworks;
using TheOtherRoles.Patches;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

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
            Watcher.clearAndReload();
            Opportunist.clearAndReload();
            Moriarty.clearAndReload();
            Akujo.clearAndReload();
            PlagueDoctor.clearAndReload();
            JekyllAndHyde.clearAndReload();
            Cupid.clearAndReload();

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

            // Gamemodes
            HandleGuesser.clearAndReload();
            HideNSeek.clearAndReload();

            // Objects
            FootprintHolder.clearAndReload();

        }

        public static class Jester {
            public static PlayerControl jester;
            public static Color color = new Color32(236, 98, 165, byte.MaxValue);

            public static bool triggerJesterWin = false;
            public static bool canCallEmergency = true;
            public static bool hasImpostorVision = false;
            public static bool canUseVents = false;

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
            public static bool stopsGameEnd;
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

        public static bool shieldVisible(PlayerControl target)
        {
            bool hasVisibleShield = false;

            bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            bool isMimicKShield = target == MimicK.mimicK && MimicK.victim != null;
            bool isMimicAMorph = target == MimicA.mimicA && MimicA.isMorph;
            if (shielded != null && ((target == shielded && !isMorphedMorphling && !isMimicKShield && !isMimicAMorph) || (isMorphedMorphling && Morphling.morphTarget == shielded) || (isMimicAMorph && MimicK.mimicK == shielded)))
            {
                hasVisibleShield = showShielded == 0 || Helpers.shouldShowGhostInfo() // Everyone or Ghost info
                    || (showShielded == 1 && (CachedPlayer.LocalPlayer.PlayerControl == shielded || CachedPlayer.LocalPlayer.PlayerControl == medic)) // Shielded + Medic
                    || (showShielded == 2 && CachedPlayer.LocalPlayer.PlayerControl == medic); // Medic only
                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (meetingAfterShielding || !showShieldAfterMeeting || CachedPlayer.LocalPlayer.PlayerControl == medic || Helpers.shouldShowGhostInfo());
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
                /*if ((p == Ninja.ninja && Ninja.stealthed) || (p == Sprinter.sprinter && Sprinter.sprinting))
                    continue;*/
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
                    case SpecialMediumInfo.JackalKillsSidekick:
                        msg = ModTranslation.getString("mediumJackalKillsSidekick");
                        break;
                    case SpecialMediumInfo.ImpostorTeamkill:
                        msg = ModTranslation.getString("mediumImpostorTeamKill");
                        break;
                    case SpecialMediumInfo.BodyCleaned:
                        msg = ModTranslation.getString("mediumBodyCleaned");
                        break;
                }
            } else {
                int randomNumber = rnd.Next(4);
                string typeOfColor = Helpers.isLighterColor(Medium.target.killerIfExisting.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("mediumSoulPlayerLighter") : ModTranslation.getString("mediumSoulPlayerDarker");
                float timeSinceDeath = ((float)(Medium.meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds);
                var roleString = RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true);

                if (randomNumber == 0)
                {
                    if (!roleString.Contains(RoleInfo.impostor.name) && !roleString.Contains(RoleInfo.crewmate.name)) msg = string.Format(ModTranslation.getString("mediumQuestion1"), RoleInfo.GetRolesString(Medium.target.player, false, includeHidden: true));
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
                        count = alivePlayersList.Where(pc => pc.Data.Role.IsImpostor || new List<RoleInfo>() { RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.sheriff, RoleInfo.thief }.Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault())).Count();
                        condition = ModTranslation.getString("mediumKiller") + (count == 1 ? "" : ModTranslation.getString("mediumPlural"));
                        break;
                    case 1:
                        count = alivePlayersList.Where(Helpers.roleCanUseVents).Count();
                        condition = string.Format(ModTranslation.getString("mediumPlayerUseVents"), (count == 1 ? "" : ModTranslation.getString("mediumPlural")));
                        break;
                    case 2:
                        count = alivePlayersList.Where(pc => Helpers.isNeutral(pc) && pc != Jackal.jackal && pc != Sidekick.sidekick && pc != Thief.thief).Count();
                        condition = string.Format(ModTranslation.getString("mediumPlayerNeutral"), (count == 1 ? "" : ModTranslation.getString("mediumPlural")), (count == 1 ? ModTranslation.getString("mediumIs") : ModTranslation.getString("mediumAre")));
                        break;
                    case 3:
                        //count = alivePlayersList.Where(pc =>
                        break;
                }
                msg += $"\n" + ModTranslation.getString("mediumAskPrefix") + $"{count} " + $"{condition} " + string.Format(ModTranslation.getString("mediumStillAlive"), (count == 1 ? ModTranslation.getString("mediumWas") : ModTranslation.getString("mediumWere")));
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
            canCallEmergency = CustomOptionHolder.lawyerCanCallEmergency.getBool();
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

    public static class MimicK
    {
        public static PlayerControl mimicK;
        public static Color color = Palette.ImpostorRed;

        public static bool ifOneDiesBothDie = true;
        public static bool hasOneVote = true;
        public static bool countAsOne = true;

        public static string name = "";

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;

        public static PlayerControl victim;

        public static void arrowUpdate()
        {
            //if (MimicK.mimicK == null || MimicA.mimicA == null) return;
            if (arrows.FirstOrDefault()?.arrow != null)
            {
                if (mimicK == null || MimicA.mimicA == null)
                {
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                    return;
                }
            }            
            if (CachedPlayer.LocalPlayer.PlayerControl != mimicK || mimicK == null) return;
            if (mimicK.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
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

                //if (MimicK.mimicK == null) return;

                // Arrows一覧
                arrows = new List<Arrow>();

                // インポスターの位置を示すArrowsを描画
                /*foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    if (p == MimicA.mimicA)
                    {
                        arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }*/

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

        public static void clearAndReload()
        {
            mimicK?.setDefaultLook();
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

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static void arrowUpdate()
        {
            //if (MimicA.mimicA == null || MimicK.mimicK == null) return;
            if (arrows.FirstOrDefault()?.arrow != null)
            {
                if (MimicK.mimicK == null || mimicA == null)
                {
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                    return;
                }
            }            
            if (CachedPlayer.LocalPlayer.PlayerControl != mimicA) return;

            if (mimicA.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
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
                Arrow arrow = new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicK.mimicK.transform.position);
                arrows.Add(arrow);

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void clearAndReload()
        {
            mimicA?.setDefaultLook();
            mimicA = null;
            isMorph = false;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
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
            playerStatus = new Dictionary<byte, bool>();
            progress = new Dictionary<byte, float>();
            numUsed = 0;
            divinedFlag = false;
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

        public static void clearAndReload()
        {
            yasuna = null;
            _remainingSpecialVotes = Mathf.RoundToInt(CustomOptionHolder.yasunaNumberOfSpecialVotes.getFloat());
            specialVoteTargetPlayerId = byte.MaxValue;
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

        public static Sprite getTrapButtonSprite()
        {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrapperButton.png", 115f);
            return trapButtonSprite;
        }

        public static void setTrap()
        {
            var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceTrap, Hazel.SendOption.Reliable);
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
            meetingFlag = false;
            Trap.clearAllTraps();
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
            if ((BomberA.bombTarget == null || BomberB.bombTarget == null) && !alwaysShowArrow) return;
            if (bomberA.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
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
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
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
            if (BomberA.bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && !MeetingHud.Instance)
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
                        targetText = GameObject.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
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
                        partnerTargetText = GameObject.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
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
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values)
                pp?.gameObject?.SetActive(false);
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
            if (BomberA.bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && !MeetingHud.Instance)
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
                        targetText = GameObject.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
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
                        partnerTargetText = GameObject.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
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
            if ((BomberA.bombTarget == null || BomberB.bombTarget == null) && !BomberA.alwaysShowArrow) return;
            if (bomberB.Data.IsDead)
            {
                if (arrows.FirstOrDefault().arrow != null) UnityEngine.Object.Destroy(arrows.FirstOrDefault().arrow);
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
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
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
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values)
                pp?.gameObject?.SetActive(false);
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
        public static PlayerControl currentTarget;

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
            currentTarget = null;
            teleportCooldown = CustomOptionHolder.teleporterCooldown.getFloat();
            teleportNumber = (int)CustomOptionHolder.teleporterTeleportNumber.getFloat();
            sampleCooldown = CustomOptionHolder.teleporterSampleCooldown.getFloat();
        }
    }

    public static class EvilHacker
    {
        public static PlayerControl evilHacker;
        public static Color color = Palette.ImpostorRed;
        public static bool canHasBetterAdmin = false;
        public static bool canCreateMadmate = false;
        public static bool canCreateMadmateFromJackal;
        public static bool canInheritAbility;
        public static bool canSeeDoorStatus;
        public static PlayerControl fakeMadmate;
        public static PlayerControl currentTarget;

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
            return canInheritAbility && evilHacker != null && evilHacker.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor;
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
        private static Sprite blackmailButtonSprite;
        private static Sprite overlaySprite;

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
            int counter = Sherlock.sherlock.Data.Tasks.ToArray().Where(t => t.Complete).Count();
            return (int)Math.Floor((float)counter / numTasks);
        }

        public static void clearAndReload()
        {
            sherlock = null;
            numUsed = 0;
            killLog = new();
            numTasks = Mathf.RoundToInt(CustomOptionHolder.sherlockRechargeTasksNumber.getFloat());
            cooldown = CustomOptionHolder.sherlockCooldown.getFloat();
            investigateDistance = CustomOptionHolder.sherlockInvestigateDistance.getFloat();
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
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UndertakerDropBody, Hazel.SendOption.Reliable, -1);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DropBody(position);
        }

        public static void RpcDragBody(byte playerId)
        {
            if (undertaker == null) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UndertakerDragBody, Hazel.SendOption.Reliable, -1);
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

    public static class Prophet
    {
        public static PlayerControl prophet;
        public static Color32 color = new Color32(255, 204, 127, byte.MaxValue);

        public static float cooldown = 30f;
        public static bool powerCrewAsRed = false;
        public static bool neutralAsRed = true;
        public static bool canCallEmergency = false;
        public static int examineNum = 3;
        public static int examinesToBeRevealed = 1;
        public static int examinesLeft;
        public static bool revealProphet = true;
        public static bool isRevealed = false;
        public static List<Arrow> arrows = new List<Arrow>();

        public static Dictionary<PlayerControl, bool> examined = new Dictionary<PlayerControl, bool>();
        public static PlayerControl currentTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SeerButton.png", 115f);
            return buttonSprite;
        }

        public static bool isKiller(PlayerControl p)
        {
            return Helpers.isKiller(p)
                || ((p == Sheriff.sheriff || p == Deputy.deputy || p == Veteran.veteran || p == Mayor.mayor || p == Swapper.swapper || p == Guesser.niceGuesser || p == Yasuna.yasuna) && powerCrewAsRed) || (Helpers.isNeutral(p) && neutralAsRed);
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
            powerCrewAsRed = CustomOptionHolder.prophetPowerCrewAsRed.getBool();
            neutralAsRed = CustomOptionHolder.prophetNeutralAsRed.getBool();
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

        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;

        public static PlayerControl target;
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
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.IsDead)
                    {
                        if ((p.Data.Role.IsImpostor || p == Spy.spy) && impostorPositionText.ContainsKey(p.Data.PlayerName))
                        {
                            impostorPositionText[p.Data.PlayerName].text = "";
                        }
                        continue;
                    }
                    Arrow arrow;
                    if ((p.Data.Role.IsImpostor && p != CachedPlayer.LocalPlayer.PlayerControl) || (Spy.spy != null && p == Spy.spy) || (p == Sidekick.sidekick && Sidekick.wasTeamRed)
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

        public static void clearAndReload()
        {
            evilTracker = null;
            target = null;
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
            canSetTargetOnMeeting = CustomOptionHolder.evilTrackerCanSetTargetOnMeeting.getBool();
        }
    }

    public static class Shifter
    {
        public static PlayerControl shifter;
        public static List<int> pastShifters = new List<int>();
        public static Color color = new Color32(102, 102, 102, byte.MaxValue);

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;
        public static bool shiftModifiers = false;

        public static bool isNeutral = false;
        public static bool shiftPastShifters = false;

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

    public static class Moriarty
    {
        public static PlayerControl moriarty;
        public static PlayerControl formerMoriarty;
        public static Color color = Color.green;

        public static PlayerControl tmpTarget;
        public static PlayerControl target;
        public static PlayerControl currentTarget;
        public static PlayerControl killTarget;
        public static List<PlayerControl> brainwashed;

        public static int counter;

        public static float brainwashTime = 2f;
        public static float brainwashCooldown = 30f;
        public static int numberToWin = 3;

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
            brainwashCooldown = CustomOptionHolder.moriartyBrainwashCooldown.getFloat();
            brainwashTime = CustomOptionHolder.moriartyBrainwashTime.getFloat();
            numberToWin = (int)CustomOptionHolder.moriartyNumberToWin.getFloat();

            if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText);
            targetPositionText = null;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
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
            timeLimit = CustomOptionHolder.akujoTimeLimit.getFloat() + 360f;
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
            foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                dead[pc.PlayerId] = pc.Data.IsDead;
            }
        }

        public static bool hasInfected()
        {
            bool flag = false;
            foreach (var item in progress)
            {
                if (item.Value != 0f)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
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
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
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
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlagueDoctorWin, Hazel.SendOption.Reliable, -1);
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
        public static float addition = 0f;

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

                //player.cosmetics.currentPet.renderers[0].color = color;
                //player.cosmetics.currentPet.shadows[0].color = color;

                if (player.GetPet() != null)
                    player.GetPet().ForEachRenderer(true, (Il2CppSystem.Action<SpriteRenderer>)((render) => render.color = color));

                if (player.cosmetics.visor != null)
                    player.cosmetics.visor.Image.color = color;

                if (player.cosmetics.colorBlindText != null)
                    player.cosmetics.colorBlindText.color = color;
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

                //player.cosmetics.currentPet.renderers[0].color = color;
                //player.cosmetics.currentPet.shadows[0].color = color;

                if (player.GetPet() != null)
                    player.GetPet().ForEachRenderer(true, (Il2CppSystem.Action<SpriteRenderer>)((render) => render.color = color));

                if (player.cosmetics.visor != null)
                    player.cosmetics.visor.Image.color = color;

                if (player.cosmetics.colorBlindText != null)
                    player.cosmetics.colorBlindText.color = color;
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
            return killer == Thief.thief && !target.Data.Role.IsImpostor && !new List<RoleInfo> { RoleInfo.jackal, canKillSheriff ? RoleInfo.sheriff : null, RoleInfo.sidekick, RoleInfo.moriarty, RoleInfo.jekyllAndHyde }.Contains(targetRole);
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

    public static class Madmate
    {
        public static Color color = Palette.ImpostorRed;
        public static List<PlayerControl> madmate = new List<PlayerControl>();
        public static bool hasTasks;
        public static bool canDieToSheriff;
        public static bool canVent;
        public static bool hasImpostorVision;
        public static bool canFixComm;
        public static bool canSabotage;
        public static int commonTasks;
        public static int shortTasks;
        public static int longTasks;
        
        public static string fullName { get { return ModTranslation.getString("madmate"); } }
        public static string prefix { get { return ModTranslation.getString("madmatePrefix"); } }

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
                //if ((chameleonPlayer == Ninja.ninja && Ninja.stealthed) || (chameleonPlayer == Sprinter.sprinter && Sprinter.sprinting)) continue;
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
