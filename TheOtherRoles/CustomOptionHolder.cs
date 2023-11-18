using Epic.OnlineServices.RTCAudio;
using System.Collections.Generic;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Types = TheOtherRoles.CustomOption.CustomOptionType;

namespace TheOtherRoles {
    public class CustomOptionHolder {
        public static string[] rates = new string[]{"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"};
        public static string[] ratesModifier = new string[]{"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
        public static string[] presets = new string[]{ModTranslation.getString("preset1"), ModTranslation.getString("preset2"), "Random Preset Skeld", "Random Preset Mira HQ", "Random Preset Polus", "Random Preset Airship", "Random Preset Submerged" };

        public static CustomOption presetSelection;
        public static CustomOption activateRoles;
        public static CustomOption crewmateRolesCountMin;
        public static CustomOption crewmateRolesCountMax;
        public static CustomOption crewmateRolesFill;
        public static CustomOption neutralRolesCountMin;
        public static CustomOption neutralRolesCountMax;
        public static CustomOption impostorRolesCountMin;
        public static CustomOption impostorRolesCountMax;
        public static CustomOption modifiersCountMin;
        public static CustomOption modifiersCountMax;

        public static CustomOption enableCodenameHorsemode;
        public static CustomOption enableCodenameDisableHorses;

        public static CustomOption additionalVents;
        public static CustomOption specimenVital;
        public static CustomOption airshipLadder;
        public static CustomOption airshipOptimize;
        public static CustomOption randomGameStartPosition;

        public static CustomOption mafiaSpawnRate;
        public static CustomOption janitorCooldown;

        public static CustomOption morphlingSpawnRate;
        public static CustomOption morphlingCooldown;
        public static CustomOption morphlingDuration;

        public static CustomOption camouflagerSpawnRate;
        public static CustomOption camouflagerCooldown;
        public static CustomOption camouflagerDuration;

        public static CustomOption vampireSpawnRate;
        public static CustomOption vampireKillDelay;
        public static CustomOption vampireCooldown;
        public static CustomOption vampireCanKillNearGarlics;

        public static CustomOption eraserSpawnRate;
        public static CustomOption eraserCooldown;
        public static CustomOption eraserCanEraseAnyone;
        public static CustomOption guesserSpawnRate;
        public static CustomOption guesserIsImpGuesserRate;
        public static CustomOption guesserNumberOfShots;
        public static CustomOption guesserHasMultipleShotsPerMeeting;
        public static CustomOption guesserKillsThroughShield;
        public static CustomOption guesserEvilCanKillSpy;
        public static CustomOption guesserSpawnBothRate;
        public static CustomOption guesserCantGuessSnitchIfTaksDone;

        public static CustomOption watcherSpawnRate;
        public static CustomOption watcherisImpWatcherRate;
        public static CustomOption watcherSeeGuesses;
        public static CustomOption watcherSpawnBothRate;

        public static CustomOption jesterSpawnRate;
        public static CustomOption jesterCanCallEmergency;
        public static CustomOption jesterHasImpostorVision;

        public static CustomOption arsonistSpawnRate;
        public static CustomOption arsonistCooldown;
        public static CustomOption arsonistDuration;

        public static CustomOption jackalSpawnRate;
        public static CustomOption jackalKillCooldown;
        public static CustomOption jackalCreateSidekickCooldown;
        public static CustomOption jackalCanUseVents;
        public static CustomOption jackalCanCreateSidekick;
        public static CustomOption sidekickPromotesToJackal;
        public static CustomOption sidekickCanKill;
        public static CustomOption sidekickCanUseVents;
        public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
        public static CustomOption jackalCanCreateSidekickFromImpostor;
        public static CustomOption jackalAndSidekickHaveImpostorVision;

        public static CustomOption opportunistSpawnRate;

        public static CustomOption bountyHunterSpawnRate;
        public static CustomOption bountyHunterBountyDuration;
        public static CustomOption bountyHunterReducedCooldown;
        public static CustomOption bountyHunterPunishmentTime;
        public static CustomOption bountyHunterShowArrow;
        public static CustomOption bountyHunterArrowUpdateIntervall;

        public static CustomOption witchSpawnRate;
        public static CustomOption witchCooldown;
        public static CustomOption witchAdditionalCooldown;
        public static CustomOption witchCanSpellAnyone;
        public static CustomOption witchSpellCastingDuration;
        public static CustomOption witchTriggerBothCooldowns;
        public static CustomOption witchVoteSavesTargets;

        public static CustomOption assassinSpawnRate;
        public static CustomOption assassinCooldown;
        public static CustomOption assassinKnowsTargetLocation;
        public static CustomOption assassinTraceTime;
        public static CustomOption assassinTraceColorTime;
        //public static CustomOption assassinInvisibleDuration;

        public static CustomOption ninjaSpawnRate;
        public static CustomOption ninjaStealthCooldown;
        public static CustomOption ninjaStealthDuration;
        public static CustomOption ninjaKillPenalty;
        public static CustomOption ninjaSpeedBonus;
        public static CustomOption ninjaFadeTime;
        public static CustomOption ninjaCanVent;
        public static CustomOption ninjaCanBeTargeted;

        public static CustomOption serialKillerSpawnRate;
        public static CustomOption serialKillerKillCooldown;
        public static CustomOption serialKillerSuicideTimer;
        public static CustomOption serialKillerResetTimer;

        public static CustomOption mayorSpawnRate;
        public static CustomOption mayorCanSeeVoteColors;
        public static CustomOption mayorTasksNeededToSeeVoteColors;
        public static CustomOption mayorMeetingButton;
        public static CustomOption mayorMaxRemoteMeetings;
        public static CustomOption mayorChooseSingleVote;

        public static CustomOption portalmakerSpawnRate;
        public static CustomOption portalmakerCooldown;
        public static CustomOption portalmakerUsePortalCooldown;
        public static CustomOption portalmakerLogOnlyColorType;
        public static CustomOption portalmakerLogHasTime;
        public static CustomOption portalmakerCanPortalFromAnywhere;

        public static CustomOption engineerSpawnRate;
        public static CustomOption engineerNumberOfFixes;
        public static CustomOption engineerHighlightForImpostors;
        public static CustomOption engineerHighlightForTeamJackal;

        public static CustomOption sheriffSpawnRate;
        public static CustomOption sheriffCooldown;
        public static CustomOption sheriffCanKillNeutrals;
        public static CustomOption deputySpawnRate;

        public static CustomOption deputyNumberOfHandcuffs;
        public static CustomOption deputyHandcuffCooldown;
        public static CustomOption deputyGetsPromoted;
        public static CustomOption deputyKeepsHandcuffs;
        public static CustomOption deputyHandcuffDuration;
        public static CustomOption deputyKnowsSheriff;
        public static CustomOption deputyStopsGameEnd;

        public static CustomOption lighterSpawnRate;
        public static CustomOption lighterModeLightsOnVision;
        public static CustomOption lighterModeLightsOffVision;
        public static CustomOption lighterFlashlightWidth;
        public static CustomOption lighterCanSeeInvisible;

        public static CustomOption sprinterSpawnRate;
        public static CustomOption sprinterCooldown;
        public static CustomOption sprinterDuration;
        public static CustomOption sprinterFadeTime;

        public static CustomOption fortuneTellerSpawnRate;
        public static CustomOption fortuneTellerNumTasks;
        public static CustomOption fortuneTellerResults;
        public static CustomOption fortuneTellerDistance;
        public static CustomOption fortuneTellerDuration;

        public static CustomOption detectiveSpawnRate;
        public static CustomOption detectiveAnonymousFootprints;
        public static CustomOption detectiveFootprintIntervall;
        public static CustomOption detectiveFootprintDuration;
        public static CustomOption detectiveReportNameDuration;
        public static CustomOption detectiveReportColorDuration;

        public static CustomOption timeMasterSpawnRate;
        public static CustomOption timeMasterCooldown;
        public static CustomOption timeMasterRewindTime;
        public static CustomOption timeMasterShieldDuration;

        public static CustomOption medicSpawnRate;
        public static CustomOption medicShowShielded;
        public static CustomOption medicShowAttemptToShielded;
        public static CustomOption medicSetOrShowShieldAfterMeeting;
        public static CustomOption medicShowAttemptToMedic;
        public static CustomOption medicSetShieldAfterMeeting;

        public static CustomOption veteranSpawnRate;
        public static CustomOption veteranCooldown;
        public static CustomOption veteranAlertDuration;
        public static CustomOption veteranAlertNumber;

        public static CustomOption sherlockSpawnRate;
        public static CustomOption sherlockCooldown;
        public static CustomOption sherlockRechargeTasksNumber;
        public static CustomOption sherlockInvestigateDistance;

        public static CustomOption swapperSpawnRate;
        public static CustomOption swapperIsImpRate;
        public static CustomOption swapperCanCallEmergency;
        public static CustomOption swapperCanOnlySwapOthers;
        public static CustomOption swapperSwapsNumber;
        public static CustomOption swapperRechargeTasksNumber;

        public static CustomOption seerSpawnRate;
        public static CustomOption seerMode;
        public static CustomOption seerSoulDuration;
        public static CustomOption seerLimitSoulDuration;

        public static CustomOption hackerSpawnRate;
        public static CustomOption hackerCooldown;
        public static CustomOption hackerHackeringDuration;
        public static CustomOption hackerOnlyColorType;
        public static CustomOption hackerToolsNumber;
        public static CustomOption hackerRechargeTasksNumber;
        public static CustomOption hackerNoMove;

        public static CustomOption baitSpawnRate;
        public static CustomOption baitHighlightAllVents;
        public static CustomOption baitReportDelay;
        public static CustomOption baitShowKillFlash;

        public static CustomOption trackerSpawnRate;
        public static CustomOption trackerUpdateIntervall;
        public static CustomOption trackerResetTargetAfterMeeting;
        public static CustomOption trackerCanTrackCorpses;
        public static CustomOption trackerCorpsesTrackingCooldown;
        public static CustomOption trackerCorpsesTrackingDuration;

        public static CustomOption snitchSpawnRate;
        public static CustomOption snitchLeftTasksForReveal;
        public static CustomOption snitchMode;
        public static CustomOption snitchTargets;

        public static CustomOption shifterSpawnRate;
        public static CustomOption shifterIsNeutralRate;
        public static CustomOption shifterShiftsModifiers;
        public static CustomOption shifterPastShifters;

        public static CustomOption spySpawnRate;
        public static CustomOption spyCanDieToSheriff;
        public static CustomOption spyImpostorsCanKillAnyone;
        public static CustomOption spyCanEnterVents;
        public static CustomOption spyHasImpostorVision;

        public static CustomOption taskMasterSpawnRate;
        public static CustomOption taskMasterBecomeATaskMasterWhenCompleteAllTasks;
        public static CustomOption taskMasterExtraCommonTasks;
        public static CustomOption taskMasterExtraShortTasks;
        public static CustomOption taskMasterExtraLongTasks;

        public static CustomOption tricksterSpawnRate;
        public static CustomOption tricksterPlaceBoxCooldown;
        public static CustomOption tricksterLightsOutCooldown;
        public static CustomOption tricksterLightsOutDuration;

        public static CustomOption nekoKabochaSpawnRate;
        public static CustomOption nekoKabochaRevengeCrew;
        public static CustomOption nekoKabochaRevengeNeutral;
        public static CustomOption nekoKabochaRevengeImpostor;
        public static CustomOption nekoKabochaRevengeExile;

        public static CustomOption evilTrackerSpawnRate;
        public static CustomOption evilTrackerCooldown;
        public static CustomOption evilTrackerResetTargetAfterMeeting;
        public static CustomOption evilTrackerCanSeeDeathFlash;
        public static CustomOption evilTrackerCanSeeTargetPosition;
        public static CustomOption evilTrackerCanSetTargetOnMeeting;

        public static CustomOption evilHackerSpawnRate;
        public static CustomOption evilHackerCanHasBetterAdmin;
        public static CustomOption evilHackerCanSeeDoorStatus;
        public static CustomOption evilHackerCanCreateMadmate;
        public static CustomOption evilHackerCanCreateMadmateFromJackal;
        public static CustomOption evilHackerCanInheritAbility;
        public static CustomOption createdMadmateCanDieToSheriff;
        public static CustomOption createdMadmateCanEnterVents;
        public static CustomOption createdMadmateHasImpostorVision;
        public static CustomOption createdMadmateCanSabotage;
        public static CustomOption createdMadmateCanFixComm;
        public static CustomOption createdMadmateAbility;
        public static CustomOption createdMadmateCommonTasks;

        public static CustomOption undertakerSpawnRate;
        public static CustomOption undertakerSpeedDecrease;
        public static CustomOption undertakerDisableVent;

        public static CustomOption cleanerSpawnRate;
        public static CustomOption cleanerCooldown;

        public static CustomOption warlockSpawnRate;
        public static CustomOption warlockCooldown;
        public static CustomOption warlockRootTime;

        public static CustomOption securityGuardSpawnRate;
        public static CustomOption securityGuardCooldown;
        public static CustomOption securityGuardTotalScrews;
        public static CustomOption securityGuardCamPrice;
        public static CustomOption securityGuardVentPrice;
        public static CustomOption securityGuardCamDuration;
        public static CustomOption securityGuardCamMaxCharges;
        public static CustomOption securityGuardCamRechargeTasksNumber;
        public static CustomOption securityGuardNoMove;

        public static CustomOption vultureSpawnRate;
        public static CustomOption vultureCooldown;
        public static CustomOption vultureNumberToWin;
        public static CustomOption vultureCanUseVents;
        public static CustomOption vultureShowArrows;

        public static CustomOption mediumSpawnRate;
        public static CustomOption mediumCooldown;
        public static CustomOption mediumDuration;
        public static CustomOption mediumOneTimeUse;
        public static CustomOption mediumChanceAdditionalInfo;

        public static CustomOption lawyerSpawnRate;
        public static CustomOption lawyerTargetKnows;
        //public static CustomOption lawyerIsProsecutorChance;
        public static CustomOption lawyerTargetCanBeJester;
        public static CustomOption lawyerVision;
        public static CustomOption lawyerKnowsRole;
        public static CustomOption lawyerCanCallEmergency;
        public static CustomOption pursuerCooldown;
        public static CustomOption pursuerBlanksNumber;

        public static CustomOption yasunaSpawnRate;
        public static CustomOption yasunaIsImpYasunaRate;
        public static CustomOption yasunaNumberOfSpecialVotes;
        public static CustomOption yasunaSpecificMessageMode;

        public static CustomOption thiefSpawnRate;
        public static CustomOption thiefCooldown;
        public static CustomOption thiefHasImpVision;
        public static CustomOption thiefCanUseVents;
        public static CustomOption thiefCanKillSheriff;
        public static CustomOption thiefCanStealWithGuess;

        public static CustomOption mimicSpawnRate;
        public static CustomOption mimicCountAsOne;
        public static CustomOption mimicIfOneDiesBothDie;
        public static CustomOption mimicHasOneVote;

        public static CustomOption bomberSpawnRate;
        public static CustomOption bomberCooldown;
        public static CustomOption bomberDuration;
        public static CustomOption bomberCountAsOne;
        public static CustomOption bomberShowEffects;
        public static CustomOption bomberIfOneDiesBothDie;
        public static CustomOption bomberHasOneVote;
        public static CustomOption bomberAlwaysShowArrow;

        /*public static CustomOption trapperSpawnRate;
        public static CustomOption trapperCooldown;
        public static CustomOption trapperMaxCharges;
        public static CustomOption trapperRechargeTasksNumber;
        public static CustomOption trapperTrapNeededTriggerToReveal;
        public static CustomOption trapperAnonymousMap;
        public static CustomOption trapperInfoType;
        public static CustomOption trapperTrapDuration;*/

        /*public static CustomOption bomberSpawnRate;
        public static CustomOption bomberBombDestructionTime;
        public static CustomOption bomberBombDestructionRange;
        public static CustomOption bomberBombHearRange;
        public static CustomOption bomberDefuseDuration;
        public static CustomOption bomberBombCooldown;
        public static CustomOption bomberBombActiveAfter;*/

        public static CustomOption modifiersAreHidden;

        /*public static CustomOption modifierBait;
        public static CustomOption modifierBaitQuantity;
        public static CustomOption modifierBaitReportDelayMin;
        public static CustomOption modifierBaitReportDelayMax;
        public static CustomOption modifierBaitShowKillFlash;*/
        // Bait is no longer a modifier

        public static CustomOption modifierLover;
        public static CustomOption modifierLoverImpLoverRate;
        public static CustomOption modifierLoverBothDie;
        public static CustomOption modifierLoverEnableChat;

        public static CustomOption modifierBloody;
        public static CustomOption modifierBloodyQuantity;
        public static CustomOption modifierBloodyDuration;

        public static CustomOption modifierAntiTeleport;
        public static CustomOption modifierAntiTeleportQuantity;

        public static CustomOption modifierTieBreaker;

        public static CustomOption modifierSunglasses;
        public static CustomOption modifierSunglassesQuantity;
        public static CustomOption modifierSunglassesVision;
        
        public static CustomOption modifierMini;
        public static CustomOption modifierMiniGrowingUpDuration;
        public static CustomOption modifierMiniGrowingUpInMeeting;

        public static CustomOption modifierVip;
        public static CustomOption modifierVipQuantity;
        public static CustomOption modifierVipShowColor;

        public static CustomOption modifierInvert;
        public static CustomOption modifierInvertQuantity;
        public static CustomOption modifierInvertDuration;

        public static CustomOption modifierChameleon;
        public static CustomOption modifierChameleonQuantity;
        public static CustomOption modifierChameleonHoldDuration;
        public static CustomOption modifierChameleonFadeDuration;
        public static CustomOption modifierChameleonMinVisibility;

        public static CustomOption madmateSpawnRate;
        public static CustomOption madmateQuantity;
        public static CustomOption madmateCanDieToSheriff;
        public static CustomOption madmateCanEnterVents;
        public static CustomOption madmateCanSabotage;
        public static CustomOption madmateHasImpostorVision;
        public static CustomOption madmateCanFixComm;
        public static CustomOption madmateAbility;
        public static CustomOption madmateCommonTasks;
        public static CustomOption madmateShortTasks;
        public static CustomOption madmateLongTasks;

        //public static CustomOption modifierShifter;

        public static CustomOption maxNumberOfMeetings;
        public static CustomOption blockSkippingInEmergencyMeetings;
        public static CustomOption noVoteIsSelfVote;
        public static CustomOption hidePlayerNames;
        public static CustomOption allowParallelMedBayScans;
        public static CustomOption shieldFirstKill;
        public static CustomOption finishTasksBeforeHauntingOrZoomingOut;
        public static CustomOption camsNightVision;
        public static CustomOption camsNoNightVisionIfImpVision;

        public static CustomOption dynamicMap;
        public static CustomOption dynamicMapEnableSkeld;
        public static CustomOption dynamicMapEnableMira;
        public static CustomOption dynamicMapEnablePolus;
        public static CustomOption dynamicMapEnableAirShip;
        public static CustomOption dynamicMapEnableSubmerged;
        public static CustomOption dynamicMapSeparateSettings;

        //Guesser Gamemode
        public static CustomOption guesserGamemodeCrewNumber;
        public static CustomOption guesserGamemodeNeutralNumber;
        public static CustomOption guesserGamemodeImpNumber;
        public static CustomOption guesserForceJackalGuesser;
        public static CustomOption guesserForceThiefGuesser;
        public static CustomOption guesserGamemodeHaveModifier;
        public static CustomOption guesserGamemodeNumberOfShots;
        public static CustomOption guesserGamemodeHasMultipleShotsPerMeeting;
        public static CustomOption guesserGamemodeKillsThroughShield;
        public static CustomOption guesserGamemodeEvilCanKillSpy;
        public static CustomOption guesserGamemodeCantGuessSnitchIfTaksDone;

        // Hide N Seek Gamemode
        public static CustomOption hideNSeekHunterCount;
        public static CustomOption hideNSeekKillCooldown;
        public static CustomOption hideNSeekHunterVision;
        public static CustomOption hideNSeekHuntedVision;
        public static CustomOption hideNSeekTimer;
        public static CustomOption hideNSeekCommonTasks;
        public static CustomOption hideNSeekShortTasks;
        public static CustomOption hideNSeekLongTasks;
        public static CustomOption hideNSeekTaskWin;
        public static CustomOption hideNSeekTaskPunish;
        public static CustomOption hideNSeekCanSabotage;
        public static CustomOption hideNSeekMap;
        public static CustomOption hideNSeekHunterWaiting;

        public static CustomOption hunterLightCooldown;
        public static CustomOption hunterLightDuration;
        public static CustomOption hunterLightVision;
        public static CustomOption hunterLightPunish;
        public static CustomOption hunterAdminCooldown;
        public static CustomOption hunterAdminDuration;
        public static CustomOption hunterAdminPunish;
        public static CustomOption hunterArrowCooldown;
        public static CustomOption hunterArrowDuration;
        public static CustomOption hunterArrowPunish;

        public static CustomOption huntedShieldCooldown;
        public static CustomOption huntedShieldDuration;
        public static CustomOption huntedShieldRewindTime;
        public static CustomOption huntedShieldNumber;

        internal static Dictionary<byte, byte[]> blockedRolePairings = new Dictionary<byte, byte[]>();

        public static string cs(Color c, string s) {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
 
        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void Load() {

            CustomOption.vanillaSettings = TheOtherRolesPlugin.Instance.Config.Bind("Preset0", "VanillaOptions", "");

            // Role Options
            presetSelection = CustomOption.Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("presetSelection")), presets, null, true);
            activateRoles = CustomOption.Create(1, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("blockOriginal")), true, null, true);

            if (Utilities.EventUtility.canBeEnabled) enableCodenameHorsemode = CustomOption.Create(10423, Types.General, cs(Color.green, ModTranslation.getString("Enable Codename Horsemode")), true, null, true);
            if (Utilities.EventUtility.canBeEnabled) enableCodenameDisableHorses = CustomOption.Create(10424, Types.General, cs(Color.green, ModTranslation.getString("Disable Horses")), false, enableCodenameHorsemode, false);

            // Using new id's for the options to not break compatibilty with older versions
            crewmateRolesCountMin = CustomOption.Create(300, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("crewmateRolesCountMin")), 15f, 0f, 15f, 1f, null, true);
            crewmateRolesCountMax = CustomOption.Create(301, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("crewmateRolesCountMax")), 15f, 0f, 15f, 1f);
            crewmateRolesFill = CustomOption.Create(308, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("crewmateRolesFill")), false);
            neutralRolesCountMin = CustomOption.Create(302, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("neutralRolesCountMin")), 15f, 0f, 15f, 1f);
            neutralRolesCountMax = CustomOption.Create(303, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("neutralRolesCountMax")), 15f, 0f, 15f, 1f);
            impostorRolesCountMin = CustomOption.Create(304, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("impostorRolesCountMin")), 15f, 0f, 15f, 1f);
            impostorRolesCountMax = CustomOption.Create(305, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("impostorRolesCountMax")), 15f, 0f, 15f, 1f);
            modifiersCountMin = CustomOption.Create(306, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("modifiersCountMin")), 15f, 0f, 15f, 1f);
            modifiersCountMax = CustomOption.Create(307, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("modifiersCountMax")), 15f, 0f, 15f, 1f);            

            mafiaSpawnRate = CustomOption.Create(18, Types.Impostor, cs(Janitor.color, ModTranslation.getString("mafia")), rates, null, true);
            janitorCooldown = CustomOption.Create(19, Types.Impostor, ModTranslation.getString("janitorCooldown"), 30f, 10f, 60f, 2.5f, mafiaSpawnRate);

            morphlingSpawnRate = CustomOption.Create(20, Types.Impostor, cs(Morphling.color, ModTranslation.getString("morphling")), rates, null, true);
            morphlingCooldown = CustomOption.Create(21, Types.Impostor, ModTranslation.getString("morphlingCooldown"), 30f, 10f, 60f, 2.5f, morphlingSpawnRate);
            morphlingDuration = CustomOption.Create(22, Types.Impostor, ModTranslation.getString("morphlingDuration"), 10f, 1f, 20f, 0.5f, morphlingSpawnRate);

            camouflagerSpawnRate = CustomOption.Create(30, Types.Impostor, cs(Camouflager.color, ModTranslation.getString("camouflager")), rates, null, true);
            camouflagerCooldown = CustomOption.Create(31, Types.Impostor, ModTranslation.getString("camouflagerCooldown"), 30f, 10f, 60f, 2.5f, camouflagerSpawnRate);
            camouflagerDuration = CustomOption.Create(32, Types.Impostor, ModTranslation.getString("camouflagerDuration"), 10f, 1f, 20f, 0.5f, camouflagerSpawnRate);

            vampireSpawnRate = CustomOption.Create(40, Types.Impostor, cs(Vampire.color, ModTranslation.getString("vampire")), rates, null, true);
            vampireKillDelay = CustomOption.Create(41, Types.Impostor, ModTranslation.getString("vampireKillDelay"), 10f, 1f, 20f, 1f, vampireSpawnRate);
            vampireCooldown = CustomOption.Create(42, Types.Impostor, ModTranslation.getString("vampireCooldown"), 30f, 10f, 60f, 2.5f, vampireSpawnRate);
            vampireCanKillNearGarlics = CustomOption.Create(43, Types.Impostor, ModTranslation.getString("vampireCanKillNearGarlics"), true, vampireSpawnRate);

            eraserSpawnRate = CustomOption.Create(230, Types.Impostor, cs(Eraser.color, ModTranslation.getString("eraser")), rates, null, true);
            eraserCooldown = CustomOption.Create(231, Types.Impostor, ModTranslation.getString("eraserCooldown"), 30f, 10f, 120f, 5f, eraserSpawnRate);
            eraserCanEraseAnyone = CustomOption.Create(232, Types.Impostor, ModTranslation.getString("eraserCanEraseAnyone"), false, eraserSpawnRate);

            tricksterSpawnRate = CustomOption.Create(250, Types.Impostor, cs(Trickster.color, ModTranslation.getString("trickster")), rates, null, true);
            tricksterPlaceBoxCooldown = CustomOption.Create(251, Types.Impostor, ModTranslation.getString("tricksterPlaceBoxCooldown"), 10f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
            tricksterLightsOutCooldown = CustomOption.Create(252, Types.Impostor, ModTranslation.getString("tricksterLightsOutCooldown"), 30f, 10f, 60f, 5f, tricksterSpawnRate);
            tricksterLightsOutDuration = CustomOption.Create(253, Types.Impostor, ModTranslation.getString("tricksterLightsOutDuration"), 15f, 5f, 60f, 2.5f, tricksterSpawnRate);

            cleanerSpawnRate = CustomOption.Create(260, Types.Impostor, cs(Cleaner.color, ModTranslation.getString("cleaner")), rates, null, true);
            cleanerCooldown = CustomOption.Create(261, Types.Impostor, ModTranslation.getString("cleanerCooldown"), 30f, 10f, 60f, 2.5f, cleanerSpawnRate);

            warlockSpawnRate = CustomOption.Create(270, Types.Impostor, cs(Cleaner.color, ModTranslation.getString("warlock")), rates, null, true);
            warlockCooldown = CustomOption.Create(271, Types.Impostor, ModTranslation.getString("warlockCooldown"), 30f, 10f, 60f, 2.5f, warlockSpawnRate);
            warlockRootTime = CustomOption.Create(272, Types.Impostor, ModTranslation.getString("warlockRootTime"), 5f, 0f, 15f, 1f, warlockSpawnRate);

            bountyHunterSpawnRate = CustomOption.Create(320, Types.Impostor, cs(BountyHunter.color, ModTranslation.getString("bountyHunter")), rates, null, true);
            bountyHunterBountyDuration = CustomOption.Create(321, Types.Impostor, ModTranslation.getString("bountyHunterBountyDuration"),  60f, 10f, 180f, 10f, bountyHunterSpawnRate);
            bountyHunterReducedCooldown = CustomOption.Create(322, Types.Impostor, ModTranslation.getString("bountyHunterReducedCooldown"), 2.5f, 0f, 30f, 2.5f, bountyHunterSpawnRate);
            bountyHunterPunishmentTime = CustomOption.Create(323, Types.Impostor, ModTranslation.getString("bountyHunterPunishmentTime"), 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
            bountyHunterShowArrow = CustomOption.Create(324, Types.Impostor, ModTranslation.getString("bountyHunterShowArrow"), true, bountyHunterSpawnRate);
            bountyHunterArrowUpdateIntervall = CustomOption.Create(325, Types.Impostor, ModTranslation.getString("bountyHunterArrowUpdateInterval"), 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow);

            witchSpawnRate = CustomOption.Create(370, Types.Impostor, cs(Witch.color, ModTranslation.getString("witch")), rates, null, true);
            witchCooldown = CustomOption.Create(371, Types.Impostor, ModTranslation.getString("witchSpellCooldown"), 30f, 10f, 120f, 5f, witchSpawnRate);
            witchAdditionalCooldown = CustomOption.Create(372, Types.Impostor, ModTranslation.getString("witchAdditionalCooldown"), 10f, 0f, 60f, 5f, witchSpawnRate);
            witchCanSpellAnyone = CustomOption.Create(373, Types.Impostor, ModTranslation.getString("witchCanSpellAnyone"), false, witchSpawnRate);
            witchSpellCastingDuration = CustomOption.Create(374, Types.Impostor, ModTranslation.getString("witchSpellDuration"), 1f, 0f, 10f, 1f, witchSpawnRate);
            witchTriggerBothCooldowns = CustomOption.Create(375, Types.Impostor, ModTranslation.getString("witchTriggerBoth"), true, witchSpawnRate);
            witchVoteSavesTargets = CustomOption.Create(376, Types.Impostor, ModTranslation.getString("witchSaveTargets"), true, witchSpawnRate);

            assassinSpawnRate = CustomOption.Create(380, Types.Impostor, cs(Assassin.color, ModTranslation.getString("assassin")), rates, null, true);
            assassinCooldown = CustomOption.Create(381, Types.Impostor, ModTranslation.getString("assassinCooldown"), 30f, 10f, 120f, 5f, assassinSpawnRate);
            assassinKnowsTargetLocation = CustomOption.Create(382, Types.Impostor, ModTranslation.getString("assassinKnowsTargetLocation"), true, assassinSpawnRate);
            assassinTraceTime = CustomOption.Create(383, Types.Impostor, ModTranslation.getString("assassinTraceDuration"), 5f, 1f, 20f, 0.5f, assassinSpawnRate);
            assassinTraceColorTime = CustomOption.Create(384, Types.Impostor, ModTranslation.getString("assassinTraceColorTime"), 2f, 0f, 20f, 0.5f, assassinSpawnRate);
            //assassinInvisibleDuration = CustomOption.Create(385, Types.Impostor, "Time The Assassin Is Invisible", 3f, 0f, 20f, 1f, assassinSpawnRate);

            ninjaSpawnRate = CustomOption.Create(5050, Types.Impostor, cs(Ninja.color, ModTranslation.getString("ninja")), rates, null, true);
            ninjaStealthCooldown = CustomOption.Create(5051, Types.Impostor, ModTranslation.getString("ninjaStealthCooldown"), 30f, 2.5f, 60f, 2.5f, ninjaSpawnRate);
            ninjaStealthDuration = CustomOption.Create(5052, Types.Impostor, ModTranslation.getString("ninjaStealthDuration"), 15f, 2.5f, 60f, 2.5f, ninjaSpawnRate);
            ninjaFadeTime = CustomOption.Create(5053, Types.Impostor, ModTranslation.getString("ninjaFadeTime"), 0.5f, 0.0f, 2.5f, 0.5f, ninjaSpawnRate);
            ninjaKillPenalty = CustomOption.Create(5054, Types.Impostor, ModTranslation.getString("ninjaKillPenalty"), 10f, 0f, 60f, 2.5f, ninjaSpawnRate);
            ninjaSpeedBonus = CustomOption.Create(5055, Types.Impostor, ModTranslation.getString("ninjaSpeedBonus"), 1.25f, 0.5f, 2f, 0.25f, ninjaSpawnRate);
            ninjaCanBeTargeted = CustomOption.Create(5056, Types.Impostor, ModTranslation.getString("ninjaCanBeTargeted"), true, ninjaSpawnRate);
            ninjaCanVent = CustomOption.Create(5057, Types.Impostor, ModTranslation.getString("ninjaCanVent"), false, ninjaSpawnRate);

            serialKillerSpawnRate = CustomOption.Create(4010, Types.Impostor, cs(SerialKiller.color, ModTranslation.getString("serialKiller")), rates, null, true);
            serialKillerKillCooldown = CustomOption.Create(4011, Types.Impostor, ModTranslation.getString("serialKillerKillCooldown"), 15f, 2.5f, 60f, 2.5f, serialKillerSpawnRate);
            serialKillerSuicideTimer = CustomOption.Create(4012, Types.Impostor, ModTranslation.getString("serialKillerSuicideTimer"), 40f, 2.5f, 60f, 2.5f, serialKillerSpawnRate);
            serialKillerResetTimer = CustomOption.Create(4013, Types.Impostor, ModTranslation.getString("serialKillerResetTimer"), true, serialKillerSpawnRate);

            nekoKabochaSpawnRate = CustomOption.Create(880, Types.Impostor, cs(NekoKabocha.color, ModTranslation.getString("nekoKabocha")), rates, null, true);
            nekoKabochaRevengeCrew = CustomOption.Create(881, Types.Impostor, ModTranslation.getString("nekoKabochaRevengeCrew"), true, nekoKabochaSpawnRate);
            nekoKabochaRevengeImpostor = CustomOption.Create(882, Types.Impostor, ModTranslation.getString("nekoKabochaRevengeImpostor"), true, nekoKabochaSpawnRate);
            nekoKabochaRevengeNeutral = CustomOption.Create(883, Types.Impostor, ModTranslation.getString("nekoKabochaRevengeNeutral"), true, nekoKabochaSpawnRate);
            nekoKabochaRevengeExile = CustomOption.Create(884, Types.Impostor, ModTranslation.getString("nekoKabochaRevengeExile"), false, nekoKabochaSpawnRate);

            evilTrackerSpawnRate = CustomOption.Create(4026, Types.Impostor, cs(EvilTracker.color, ModTranslation.getString("evilTracker")), rates, null, true);
            evilTrackerCooldown = CustomOption.Create(4027, Types.Impostor, ModTranslation.getString("evilTrackerCooldown"), 10f, 0f, 60f, 5f, evilTrackerSpawnRate);
            evilTrackerResetTargetAfterMeeting = CustomOption.Create(4028, Types.Impostor, ModTranslation.getString("evilTrackerResetTargetAfterMeeting"), true, evilTrackerSpawnRate);
            evilTrackerCanSeeDeathFlash = CustomOption.Create(4029, Types.Impostor, ModTranslation.getString("evilTrackerCanSeeDeathFlash"), true, evilTrackerSpawnRate);
            evilTrackerCanSeeTargetPosition = CustomOption.Create(4031, Types.Impostor, ModTranslation.getString("evilTrackerCanSeeTargetPosition"), true, evilTrackerSpawnRate);
            evilTrackerCanSetTargetOnMeeting = CustomOption.Create(4032, Types.Impostor, ModTranslation.getString("evilTrackerCanSetTargetOnMeeting"), true, evilTrackerSpawnRate);

            undertakerSpawnRate = CustomOption.Create(4056, Types.Impostor, cs(Undertaker.color, ModTranslation.getString("undertaker")), rates, null, true);
            undertakerSpeedDecrease = CustomOption.Create(4057, Types.Impostor, ModTranslation.getString("undertakerSpeedDecrease"), -50f, -80f, 0f, 10f, undertakerSpawnRate);
            undertakerDisableVent = CustomOption.Create(4058, Types.Impostor, ModTranslation.getString("undertakerDisableVent"), true, undertakerSpawnRate);

            evilHackerSpawnRate = CustomOption.Create(8001, Types.Impostor, cs(EvilHacker.color, ModTranslation.getString("evilHacker")), rates, null, true);
            evilHackerCanHasBetterAdmin = CustomOption.Create(8002, Types.Impostor, ModTranslation.getString("evilHackerCanHasBetterAdmin"), false, evilHackerSpawnRate);
            evilHackerCanSeeDoorStatus = CustomOption.Create(8020, Types.Impostor, ModTranslation.getString("evilHackerCanSeeDoorStatus"), true, evilHackerSpawnRate);
            evilHackerCanCreateMadmate = CustomOption.Create(8000, Types.Impostor, ModTranslation.getString("evilHackerCanCreateMadmate"), true, evilHackerSpawnRate);
            evilHackerCanCreateMadmateFromJackal = CustomOption.Create(8005, Types.Impostor, ModTranslation.getString("evilHackerCanCreateMadmateFromJackal"), true, evilHackerCanCreateMadmate);
            createdMadmateCanDieToSheriff = CustomOption.Create(8004, Types.Impostor, ModTranslation.getString("createdMadmateCanDieToSheriff"), true, evilHackerCanCreateMadmate);
            createdMadmateCanEnterVents = CustomOption.Create(8005, Types.Impostor, ModTranslation.getString("createdMadmateCanEnterVents"), true, evilHackerCanCreateMadmate);
            createdMadmateCanFixComm = CustomOption.Create(8006, Types.Impostor, ModTranslation.getString("createdMadmateCanFixComm"), false, evilHackerCanCreateMadmate);
            createdMadmateCanSabotage = CustomOption.Create(8007, Types.Impostor, ModTranslation.getString("createdMadmateCanSabotage"), false, evilHackerCanCreateMadmate);
            createdMadmateHasImpostorVision = CustomOption.Create(8008, Types.Impostor, ModTranslation.getString("createdMadmateHasImpostorVision"), true, evilHackerCanCreateMadmate);
            createdMadmateAbility = CustomOption.Create(8009, Types.Impostor, ModTranslation.getString("createdMadmateAbility"), true, evilHackerCanCreateMadmate);
            createdMadmateCommonTasks = CustomOption.Create(8010, Types.Impostor, ModTranslation.getString("createdMadmateCommonTasks"), 1f, 1f, 3f, 1f, createdMadmateAbility);
            evilHackerCanInheritAbility = CustomOption.Create(8014, Types.Impostor, ModTranslation.getString("evilHackerCanInheritAbility"), false, evilHackerSpawnRate);

            mimicSpawnRate = CustomOption.Create(5000, Types.Impostor, cs(MimicK.color, ModTranslation.getString("mimic")), rates, null, true);
            mimicCountAsOne = CustomOption.Create(5001, Types.Impostor, ModTranslation.getString("mimicCountAsOne"), true, mimicSpawnRate);
            mimicIfOneDiesBothDie = CustomOption.Create(5002, Types.Impostor, ModTranslation.getString("mimicIfOneDiesBothDies"), true, mimicSpawnRate);
            mimicHasOneVote = CustomOption.Create(5003, Types.Impostor, ModTranslation.getString("mimicHasOneVote"), true, mimicSpawnRate);

            bomberSpawnRate = CustomOption.Create(6076, Types.Impostor, cs(BomberA.color, ModTranslation.getString("bomber")), rates, null, true);
            bomberCooldown = CustomOption.Create(6077, Types.Impostor, ModTranslation.getString("bomberCooldown"), 20f, 2f, 30f, 2f, bomberSpawnRate);
            bomberDuration = CustomOption.Create(6078, Types.Impostor, ModTranslation.getString("bomberDuration"), 2f, 1f, 10f, 0.5f, bomberSpawnRate);
            bomberCountAsOne = CustomOption.Create(6079, Types.Impostor, ModTranslation.getString("bomberCountAsOne"), true, bomberSpawnRate);
            bomberShowEffects = CustomOption.Create(6080, Types.Impostor, ModTranslation.getString("bomberShowEffects"), true, bomberSpawnRate);
            bomberIfOneDiesBothDie = CustomOption.Create(6081, Types.Impostor, ModTranslation.getString("bomberIfOneDiesBothDie"), true, bomberSpawnRate);
            bomberHasOneVote = CustomOption.Create(6801, Types.Impostor, ModTranslation.getString("bomberHasOneVote"), true, bomberSpawnRate);
            bomberAlwaysShowArrow = CustomOption.Create(6802, Types.Impostor, ModTranslation.getString("bomberAlwaysShowArrow"), true, bomberSpawnRate);

            /*bomberSpawnRate = CustomOption.Create(460, Types.Impostor, cs(Bomber.color, "Bomber"), rates, null, true);
            bomberBombDestructionTime = CustomOption.Create(461, Types.Impostor, "Bomb Destruction Time", 20f, 2.5f, 120f, 2.5f, bomberSpawnRate);
            bomberBombDestructionRange = CustomOption.Create(462, Types.Impostor, "Bomb Destruction Range", 50f, 5f, 150f, 5f, bomberSpawnRate);
            bomberBombHearRange = CustomOption.Create(463, Types.Impostor, "Bomb Hear Range", 60f, 5f, 150f, 5f, bomberSpawnRate);
            bomberDefuseDuration = CustomOption.Create(464, Types.Impostor, "Bomb Defuse Duration", 3f, 0.5f, 30f, 0.5f, bomberSpawnRate);
            bomberBombCooldown = CustomOption.Create(465, Types.Impostor, "Bomb Cooldown", 15f, 2.5f, 30f, 2.5f, bomberSpawnRate);
            bomberBombActiveAfter = CustomOption.Create(466, Types.Impostor, "Bomb Is Active After", 3f, 0.5f, 15f, 0.5f, bomberSpawnRate);*/

            guesserSpawnRate = CustomOption.Create(310, Types.Neutral, cs(Guesser.color, ModTranslation.getString("guesser")), rates, null, true);
            guesserIsImpGuesserRate = CustomOption.Create(311, Types.Neutral, ModTranslation.getString("guesserIsImpGuesserRate"), rates, guesserSpawnRate);
            guesserNumberOfShots = CustomOption.Create(312, Types.Neutral, ModTranslation.getString("guesserNumberOfShots"), 2f, 1f, 15f, 1f, guesserSpawnRate);
            guesserHasMultipleShotsPerMeeting = CustomOption.Create(313, Types.Neutral, ModTranslation.getString("guesserHasMultipleShotsPerMeeting"), false, guesserSpawnRate);
            guesserKillsThroughShield  = CustomOption.Create(315, Types.Neutral, ModTranslation.getString("guesserKillsThroughShield"), true, guesserSpawnRate);
            guesserEvilCanKillSpy  = CustomOption.Create(316, Types.Neutral, ModTranslation.getString("guesserEvilCanKillSpy"), true, guesserSpawnRate);
            guesserSpawnBothRate = CustomOption.Create(317, Types.Neutral, ModTranslation.getString("guesserSpawnBothRate"), rates, guesserSpawnRate);
            guesserCantGuessSnitchIfTaksDone = CustomOption.Create(318, Types.Neutral, ModTranslation.getString("guesserCantGuessSnitchIfTaksDone"), true, guesserSpawnRate);

            swapperSpawnRate = CustomOption.Create(150, Types.Neutral, cs(Swapper.color, ModTranslation.getString("swapper")), rates, null, true);
            swapperIsImpRate = CustomOption.Create(4036, Types.Neutral, ModTranslation.getString("swapperIsImpRate"), rates, swapperSpawnRate);
            swapperCanCallEmergency = CustomOption.Create(151, Types.Neutral, ModTranslation.getString("swapperCanCallEmergency"), false, swapperSpawnRate);
            swapperCanOnlySwapOthers = CustomOption.Create(152, Types.Neutral, ModTranslation.getString("swapperCanOnlySwapOthers"), false, swapperSpawnRate);

            swapperSwapsNumber = CustomOption.Create(153, Types.Neutral, ModTranslation.getString("swapperSwapsNumber"), 1f, 0f, 15f, 1f, swapperSpawnRate);
            swapperRechargeTasksNumber = CustomOption.Create(154, Types.Neutral, ModTranslation.getString("swapperRechargeTasksNumber"), 2f, 1f, 10f, 1f, swapperSpawnRate);

            watcherSpawnRate = CustomOption.Create(1035, Types.Neutral, cs(Watcher.color, ModTranslation.getString("watcher")), rates, null, true);
            watcherisImpWatcherRate = CustomOption.Create(1036, Types.Neutral, ModTranslation.getString("watcherisImpWatcherRate"), rates, watcherSpawnRate);
            watcherSeeGuesses = CustomOption.Create(5080, Types.Neutral, ModTranslation.getString("watcherSeeGuesses"), true, watcherSpawnRate);
            watcherSpawnBothRate = CustomOption.Create(1037, Types.Neutral, ModTranslation.getString("watcherSpawnBothRate"), rates, watcherSpawnRate);

            yasunaSpawnRate = CustomOption.Create(6040, Types.Neutral, cs(Yasuna.color, ModTranslation.getString("yasuna")), rates, null, true);
            yasunaIsImpYasunaRate = CustomOption.Create(6041, Types.Neutral, ModTranslation.getString("yasunaIsImpYasunaRate"), rates, yasunaSpawnRate);
            yasunaNumberOfSpecialVotes = CustomOption.Create(6042, Types.Neutral, ModTranslation.getString("yasunaNumberOfSpecialVotes"), 1f, 1f, 15f, 1f, yasunaSpawnRate);
            yasunaSpecificMessageMode = CustomOption.Create(6043, Types.Neutral, ModTranslation.getString("yasunaSpecificMessageMode"), true, yasunaSpawnRate);

            jesterSpawnRate = CustomOption.Create(60, Types.Neutral, cs(Jester.color, ModTranslation.getString("jester")), rates, null, true);
            jesterCanCallEmergency = CustomOption.Create(61, Types.Neutral, ModTranslation.getString("jesterCanCallEmergency"), true, jesterSpawnRate);
            jesterHasImpostorVision = CustomOption.Create(62, Types.Neutral, ModTranslation.getString("jesterHasImpostorVision"), false, jesterSpawnRate);

            arsonistSpawnRate = CustomOption.Create(290, Types.Neutral, cs(Arsonist.color, ModTranslation.getString("arsonist")), rates, null, true);
            arsonistCooldown = CustomOption.Create(291, Types.Neutral, ModTranslation.getString("arsonistCooldown"), 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate);
            arsonistDuration = CustomOption.Create(292, Types.Neutral, ModTranslation.getString("arsonistDuration"), 3f, 1f, 10f, 1f, arsonistSpawnRate);

            jackalSpawnRate = CustomOption.Create(220, Types.Neutral, cs(Jackal.color, ModTranslation.getString("jackal")), rates, null, true);
            jackalKillCooldown = CustomOption.Create(221, Types.Neutral, ModTranslation.getString("jackalKillCooldown"), 30f, 10f, 60f, 2.5f, jackalSpawnRate);
            jackalCreateSidekickCooldown = CustomOption.Create(222, Types.Neutral, ModTranslation.getString("jackalCreateSidekickCooldown"), 30f, 10f, 60f, 2.5f, jackalSpawnRate);
            jackalCanUseVents = CustomOption.Create(223, Types.Neutral, ModTranslation.getString("jackalCanUseVents"), true, jackalSpawnRate);
            jackalCanCreateSidekick = CustomOption.Create(224, Types.Neutral, ModTranslation.getString("jackalCanCreateSidekick"), false, jackalSpawnRate);
            sidekickPromotesToJackal = CustomOption.Create(225, Types.Neutral, ModTranslation.getString("sidekickPromotesToJackal"), false, jackalCanCreateSidekick);
            sidekickCanKill = CustomOption.Create(226, Types.Neutral, ModTranslation.getString("sidekickCanKill"), false, jackalCanCreateSidekick);
            sidekickCanUseVents = CustomOption.Create(227, Types.Neutral, ModTranslation.getString("sidekickCanUseVents"), true, jackalCanCreateSidekick);
            jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(228, Types.Neutral, ModTranslation.getString("jackalPromotedFromSidekickCanCreateSidekick"), true, sidekickPromotesToJackal);
            jackalCanCreateSidekickFromImpostor = CustomOption.Create(229, Types.Neutral, ModTranslation.getString("jackalCanCreateSidekickFromImpostor"), true, jackalCanCreateSidekick);
            jackalAndSidekickHaveImpostorVision = CustomOption.Create(430, Types.Neutral, ModTranslation.getString("jackalAndSidekickHaveImpostorVision"), false, jackalSpawnRate);

            vultureSpawnRate = CustomOption.Create(340, Types.Neutral, cs(Vulture.color, ModTranslation.getString("vulture")), rates, null, true);
            vultureCooldown = CustomOption.Create(341, Types.Neutral, ModTranslation.getString("vultureCooldown"), 15f, 10f, 60f, 2.5f, vultureSpawnRate);
            vultureNumberToWin = CustomOption.Create(342, Types.Neutral, ModTranslation.getString("vultureNumberToWin"), 4f, 1f, 10f, 1f, vultureSpawnRate);
            vultureCanUseVents = CustomOption.Create(343, Types.Neutral, ModTranslation.getString("vultureCanUseVents"), true, vultureSpawnRate);
            vultureShowArrows = CustomOption.Create(344, Types.Neutral, ModTranslation.getString("vultureShowArrows"), true, vultureSpawnRate);

            lawyerSpawnRate = CustomOption.Create(350, Types.Neutral, cs(Lawyer.color, ModTranslation.getString("lawyer")), rates, null, true);
            lawyerTargetKnows = CustomOption.Create(351, Types.Neutral, ModTranslation.getString("lawyerTargetKnows"), true, lawyerSpawnRate);
            //lawyerIsProsecutorChance = CustomOption.Create(358, Types.Neutral, "Chance That The Lawyer Is Prosecutor", rates, lawyerSpawnRate);
            lawyerVision = CustomOption.Create(354, Types.Neutral, ModTranslation.getString("lawyerVision"), 1f, 0.25f, 3f, 0.25f, lawyerSpawnRate);
            lawyerKnowsRole = CustomOption.Create(355, Types.Neutral, ModTranslation.getString("lawyerKnowsRole"), false, lawyerSpawnRate);
            lawyerCanCallEmergency = CustomOption.Create(352, Types.Neutral, ModTranslation.getString("lawyerCanCallMeeting"), true, lawyerSpawnRate);
            lawyerTargetCanBeJester = CustomOption.Create(351, Types.Neutral, ModTranslation.getString("lawyerTargetCanBeJester"), false, lawyerSpawnRate);
            pursuerCooldown = CustomOption.Create(356, Types.Neutral, ModTranslation.getString("pursuerCooldown"), 30f, 5f, 60f, 2.5f, lawyerSpawnRate);
            pursuerBlanksNumber = CustomOption.Create(357, Types.Neutral, ModTranslation.getString("pursuerBlanksNumber"), 5f, 1f, 20f, 1f, lawyerSpawnRate);

            shifterSpawnRate = CustomOption.Create(1100, Types.Neutral, cs(Shifter.color, ModTranslation.getString("shifter")), rates, null, true);
            shifterIsNeutralRate = CustomOption.Create(6007, Types.Neutral, ModTranslation.getString("shifterIsNeutralRate"), rates, shifterSpawnRate);
            shifterShiftsModifiers = CustomOption.Create(1101, Types.Neutral, ModTranslation.getString("shifterShiftsModifiers"), false, shifterSpawnRate);
            shifterPastShifters = CustomOption.Create(6008, Types.Neutral, ModTranslation.getString("shifterPastShifters"), false, shifterSpawnRate);

            opportunistSpawnRate = CustomOption.Create(4003, Types.Neutral, cs(Opportunist.color, ModTranslation.getString("opportunist")), rates, null, true);            

            mayorSpawnRate = CustomOption.Create(80, Types.Crewmate, cs(Mayor.color, ModTranslation.getString("mayor")), rates, null, true);
            mayorCanSeeVoteColors = CustomOption.Create(81, Types.Crewmate, ModTranslation.getString("mayorCanSeeVoteColor"), false, mayorSpawnRate);
            mayorTasksNeededToSeeVoteColors = CustomOption.Create(82, Types.Crewmate, ModTranslation.getString("mayorTasksNeededToSeeVoteColors"), 5f, 0f, 20f, 1f, mayorCanSeeVoteColors);
            mayorMeetingButton = CustomOption.Create(83, Types.Crewmate, ModTranslation.getString("mayorMeetingButton"), true, mayorSpawnRate);
            mayorMaxRemoteMeetings = CustomOption.Create(84, Types.Crewmate, ModTranslation.getString("mayorMaxRemoteMeetings"), 1f, 1f, 5f, 1f, mayorMeetingButton);
            mayorChooseSingleVote = CustomOption.Create(85, Types.Crewmate, ModTranslation.getString("mayorChooseSingleVote"), new string[] { ModTranslation.getString("mayorOff"), ModTranslation.getString("mayorBeforeVoting"), ModTranslation.getString("mayorUntilMeetingEnd") }, mayorSpawnRate);

            engineerSpawnRate = CustomOption.Create(90, Types.Crewmate, cs(Engineer.color, ModTranslation.getString("engineer")), rates, null, true);
            engineerNumberOfFixes = CustomOption.Create(91, Types.Crewmate, ModTranslation.getString("engineerNumberOfFixes"), 1f, 1f, 3f, 1f, engineerSpawnRate);
            engineerHighlightForImpostors = CustomOption.Create(92, Types.Crewmate, ModTranslation.getString("engineerHighlightForImpostors"), true, engineerSpawnRate);
            engineerHighlightForTeamJackal = CustomOption.Create(93, Types.Crewmate, ModTranslation.getString("engineerHighlightForTeamJackal"), true, engineerSpawnRate);

            sheriffSpawnRate = CustomOption.Create(100, Types.Crewmate, cs(Sheriff.color, ModTranslation.getString("sheriff")), rates, null, true);
            sheriffCooldown = CustomOption.Create(101, Types.Crewmate, ModTranslation.getString("sheriffCooldown"), 30f, 10f, 60f, 2.5f, sheriffSpawnRate);
            sheriffCanKillNeutrals = CustomOption.Create(102, Types.Crewmate, ModTranslation.getString("sheriffCanKillNeutrals"), false, sheriffSpawnRate);
            deputySpawnRate = CustomOption.Create(103, Types.Crewmate, ModTranslation.getString("sheriffDeputy"), rates, sheriffSpawnRate);
            deputyNumberOfHandcuffs = CustomOption.Create(104, Types.Crewmate, ModTranslation.getString("deputyNumberOfHandcuffs"), 3f, 1f, 10f, 1f, deputySpawnRate);
            deputyHandcuffCooldown = CustomOption.Create(105, Types.Crewmate, ModTranslation.getString("deputyHandcuffCooldown"), 30f, 10f, 60f, 2.5f, deputySpawnRate);
            deputyHandcuffDuration = CustomOption.Create(106, Types.Crewmate, ModTranslation.getString("deputyHandcuffDuration"), 15f, 5f, 60f, 2.5f, deputySpawnRate);
            deputyKnowsSheriff = CustomOption.Create(107, Types.Crewmate, ModTranslation.getString("deputyKnowsSheriff"), true, deputySpawnRate);
            deputyGetsPromoted = CustomOption.Create(108, Types.Crewmate, ModTranslation.getString("deputyGetsPromoted"), new string[] { ModTranslation.getString("deputyOff"), ModTranslation.getString("deputyOnImmediately"), ModTranslation.getString("deputyOnAfterMeeting") }, deputySpawnRate);
            deputyKeepsHandcuffs = CustomOption.Create(109, Types.Crewmate, ModTranslation.getString("deputyKeepsHandcuffs"), true, deputyGetsPromoted);
            deputyStopsGameEnd = CustomOption.Create(4016, Types.Crewmate, ModTranslation.getString("deputyStopsGameEnd"), false, deputySpawnRate);

            lighterSpawnRate = CustomOption.Create(110, Types.Crewmate, cs(Lighter.color, ModTranslation.getString("lighter")), rates, null, true);
            lighterModeLightsOnVision = CustomOption.Create(111, Types.Crewmate, ModTranslation.getString("lighterModeLightsOnVision"), 1.5f, 0.25f, 5f, 0.25f, lighterSpawnRate);
            lighterModeLightsOffVision = CustomOption.Create(112, Types.Crewmate, ModTranslation.getString("lighterModeLightsOffVision"), 0.5f, 0.25f, 5f, 0.25f, lighterSpawnRate);
            lighterFlashlightWidth = CustomOption.Create(113, Types.Crewmate, ModTranslation.getString("lighterFlashlightWidth"), 0.3f, 0.1f, 1f, 0.1f, lighterSpawnRate);
            lighterCanSeeInvisible = CustomOption.Create(114, Types.Crewmate, ModTranslation.getString("lighterCanSeeInvisible"), true, lighterSpawnRate);

            sprinterSpawnRate = CustomOption.Create(4005, Types.Crewmate, cs(Sprinter.color, ModTranslation.getString("sprinter")), rates, null, true);
            sprinterCooldown = CustomOption.Create(4006, Types.Crewmate, ModTranslation.getString("sprinterCooldown"), 30f, 20f, 60f, 2.5f, sprinterSpawnRate);
            sprinterDuration = CustomOption.Create(4007, Types.Crewmate, ModTranslation.getString("sprintDuration"), 15f, 10f, 60f, 2.5f, sprinterSpawnRate);
            sprinterFadeTime = CustomOption.Create(4008, Types.Crewmate, ModTranslation.getString("sprintFadeTime"), 0.5f, 0.0f, 2.5f, 0.5f, sprinterSpawnRate);

            detectiveSpawnRate = CustomOption.Create(120, Types.Crewmate, cs(Detective.color, ModTranslation.getString("detective")), rates, null, true);
            detectiveAnonymousFootprints = CustomOption.Create(121, Types.Crewmate, ModTranslation.getString("detectiveAnonymousFootprints"), false, detectiveSpawnRate); 
            detectiveFootprintIntervall = CustomOption.Create(122, Types.Crewmate, ModTranslation.getString("detectiveFootprintInterval"), 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
            detectiveFootprintDuration = CustomOption.Create(123, Types.Crewmate, ModTranslation.getString("detectiveFootprintDuration"), 5f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
            detectiveReportNameDuration = CustomOption.Create(124, Types.Crewmate, ModTranslation.getString("detectiveReportNameDuration"), 0, 0, 60, 2.5f, detectiveSpawnRate);
            detectiveReportColorDuration = CustomOption.Create(125, Types.Crewmate, ModTranslation.getString("detectiveReportColorDuration"), 20, 0, 120, 2.5f, detectiveSpawnRate);

            timeMasterSpawnRate = CustomOption.Create(130, Types.Crewmate, cs(TimeMaster.color, ModTranslation.getString("timeMaster")), rates, null, true);
            timeMasterCooldown = CustomOption.Create(131, Types.Crewmate, ModTranslation.getString("timeMasterCooldown"), 30f, 10f, 120f, 2.5f, timeMasterSpawnRate);
            timeMasterRewindTime = CustomOption.Create(132, Types.Crewmate, ModTranslation.getString("timeMasterRewindTime"), 3f, 1f, 10f, 1f, timeMasterSpawnRate);
            timeMasterShieldDuration = CustomOption.Create(133, Types.Crewmate, ModTranslation.getString("timeMasterShieldDuration"), 3f, 1f, 20f, 1f, timeMasterSpawnRate);            

            medicSpawnRate = CustomOption.Create(140, Types.Crewmate, cs(Medic.color, ModTranslation.getString("medic")), rates, null, true);
            medicShowShielded = CustomOption.Create(143, Types.Crewmate, ModTranslation.getString("medicShowShielded"), new string[] {ModTranslation.getString("medicShowShieldedAll"), ModTranslation.getString("medicShowShieldedBoth"), ModTranslation.getString("medicShowShieldedMedic") }, medicSpawnRate);
            medicShowAttemptToShielded = CustomOption.Create(144, Types.Crewmate, ModTranslation.getString("medicShowAttemptToShielded"), false, medicSpawnRate);
            medicSetOrShowShieldAfterMeeting = CustomOption.Create(145, Types.Crewmate, ModTranslation.getString("medicSetOrShowShieldAfterMeeting"), new string[] { ModTranslation.getString("medicInstantly"), ModTranslation.getString("medicVisibleAfterMeeting"), ModTranslation.getString("medicAftermeeting") }, medicSpawnRate);

            medicShowAttemptToMedic = CustomOption.Create(146, Types.Crewmate, ModTranslation.getString("medicShowAttemptToMedic"), false, medicSpawnRate);            

            fortuneTellerSpawnRate = CustomOption.Create(940, Types.Crewmate, cs(FortuneTeller.color, ModTranslation.getString("fortuneTeller")), rates, null, true);
            fortuneTellerResults = CustomOption.Create(941, Types.Crewmate, ModTranslation.getString("fortuneTellerResults"), new string[] { ModTranslation.getString("fortuneTellerResultCrew"), ModTranslation.getString("fortuneTellerResultTeam"), ModTranslation.getString("fortuneTellerResultRole") }, fortuneTellerSpawnRate);
            fortuneTellerNumTasks = CustomOption.Create(942, Types.Crewmate, ModTranslation.getString("fortuneTellerNumTasks"), 4f, 0f, 25f, 1f, fortuneTellerSpawnRate);
            fortuneTellerDuration = CustomOption.Create(943, Types.Crewmate, ModTranslation.getString("fortuneTellerDuration"), 20f, 1f, 50f, 1f, fortuneTellerSpawnRate);
            fortuneTellerDistance = CustomOption.Create(944, Types.Crewmate, ModTranslation.getString("fortuneTellerDistance"), 2.5f, 1f, 10f, 0.5f, fortuneTellerSpawnRate);

            seerSpawnRate = CustomOption.Create(160, Types.Crewmate, cs(Seer.color, ModTranslation.getString("seer")), rates, null, true);
            seerMode = CustomOption.Create(161, Types.Crewmate, ModTranslation.getString("seerMode"), new string[]{ ModTranslation.getString("seerModeBoth"), ModTranslation.getString("seerModeFlash"), ModTranslation.getString("seerModeSouls")}, seerSpawnRate);
            seerLimitSoulDuration = CustomOption.Create(163, Types.Crewmate, ModTranslation.getString("seerLimitSoulDuration"), false, seerSpawnRate);
            seerSoulDuration = CustomOption.Create(162, Types.Crewmate, ModTranslation.getString("seerSoulDuration"), 15f, 0f, 120f, 5f, seerLimitSoulDuration);
        
            hackerSpawnRate = CustomOption.Create(170, Types.Crewmate, cs(Hacker.color, ModTranslation.getString("hacker")), rates, null, true);
            hackerCooldown = CustomOption.Create(171, Types.Crewmate, ModTranslation.getString("hackerCooldown"), 30f, 5f, 60f, 5f, hackerSpawnRate);
            hackerHackeringDuration = CustomOption.Create(172, Types.Crewmate, ModTranslation.getString("hackerHackeringDuration"), 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
            hackerOnlyColorType = CustomOption.Create(173, Types.Crewmate, ModTranslation.getString("hackerOnlyColorType"), false, hackerSpawnRate);
            hackerToolsNumber = CustomOption.Create(174, Types.Crewmate, ModTranslation.getString("hackerToolsNumber"), 5f, 1f, 30f, 1f, hackerSpawnRate);
            hackerRechargeTasksNumber = CustomOption.Create(175, Types.Crewmate, ModTranslation.getString("hackerRechargeTasksNumber"), 2f, 1f, 5f, 1f, hackerSpawnRate);
            hackerNoMove = CustomOption.Create(176, Types.Crewmate, ModTranslation.getString("hackerNoMove"), true, hackerSpawnRate);

            baitSpawnRate = CustomOption.Create(1030, Types.Crewmate, cs(Bait.color, ModTranslation.getString("bait")), rates, null, true);
            baitHighlightAllVents = CustomOption.Create(1031, Types.Crewmate, ModTranslation.getString("baitHighlightAllVents"), false, baitSpawnRate);
            baitReportDelay = CustomOption.Create(1032, Types.Crewmate, ModTranslation.getString("baitReportDelay"), 0f, 0f, 10f, 1f, baitSpawnRate);
            baitShowKillFlash = CustomOption.Create(1033, Types.Crewmate, ModTranslation.getString("baitShowKillFlash"), true, baitSpawnRate);

            veteranSpawnRate = CustomOption.Create(4050, Types.Crewmate, cs(Veteran.color, ModTranslation.getString("veteran")), rates, null, true);
            veteranCooldown = CustomOption.Create(4051, Types.Crewmate, ModTranslation.getString("veteranCooldown"), 30f, 10f, 60f, 2.5f, veteranSpawnRate);
            veteranAlertDuration = CustomOption.Create(4052, Types.Crewmate, ModTranslation.getString("veteranAlertDuration"), 3f, 1f, 20f, 1f, veteranSpawnRate);
            veteranAlertNumber = CustomOption.Create(4053, Types.Crewmate, ModTranslation.getString("veteranAlertNumber"), 5f, 1f, 15f, 1f, veteranSpawnRate);

            trackerSpawnRate = CustomOption.Create(200, Types.Crewmate, cs(Tracker.color, ModTranslation.getString("tracker")), rates, null, true);
            trackerUpdateIntervall = CustomOption.Create(201, Types.Crewmate, ModTranslation.getString("trackerUpdateInterval"), 5f, 1f, 30f, 1f, trackerSpawnRate);
            trackerResetTargetAfterMeeting = CustomOption.Create(202, Types.Crewmate, ModTranslation.getString("trackerResetTargetAfterMeeting"), false, trackerSpawnRate);
            trackerCanTrackCorpses = CustomOption.Create(203, Types.Crewmate, ModTranslation.getString("trackerCanTrackCorpses"), true, trackerSpawnRate);
            trackerCorpsesTrackingCooldown = CustomOption.Create(204, Types.Crewmate, ModTranslation.getString("trackerCorpsesTrackingCooldown"), 30f, 5f, 120f, 5f, trackerCanTrackCorpses);
            trackerCorpsesTrackingDuration = CustomOption.Create(205, Types.Crewmate, ModTranslation.getString("trackerCorpsesTrackingDuration"), 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);

            sherlockSpawnRate = CustomOption.Create(5070, Types.Crewmate, cs(Sherlock.color, ModTranslation.getString("sherlock")), rates, null, true);
            sherlockCooldown = CustomOption.Create(5071, Types.Crewmate, ModTranslation.getString("sherlockCooldown"), 10f, 0f, 40f, 2.5f, sherlockSpawnRate);
            sherlockInvestigateDistance = CustomOption.Create(5072, Types.Crewmate, ModTranslation.getString("sherlockInvestigateDistance"), 5f, 1f, 15f, 1f, sherlockSpawnRate);
            sherlockRechargeTasksNumber = CustomOption.Create(5073, Types.Crewmate, ModTranslation.getString("sherlockRechargeTasksNumber"), 2f, 1f, 5f, 1f, sherlockSpawnRate);

            snitchSpawnRate = CustomOption.Create(210, Types.Crewmate, cs(Snitch.color, ModTranslation.getString("snitch")), rates, null, true);
            snitchLeftTasksForReveal = CustomOption.Create(219, Types.Crewmate, ModTranslation.getString("snitchLeftTasksForReveal"), 5f, 0f, 25f, 1f, snitchSpawnRate);
            snitchMode = CustomOption.Create(211, Types.Crewmate, ModTranslation.getString("snitchMode"), new string[] { ModTranslation.getString("snitchChat"), ModTranslation.getString("snitchMap"), ModTranslation.getString("snitchChatAndMap") }, snitchSpawnRate);
            snitchTargets = CustomOption.Create(212, Types.Crewmate, ModTranslation.getString("snitchTargets"), new string[] { ModTranslation.getString("snitchAllEvilPlayers"), ModTranslation.getString("snitchKillingPlayers") }, snitchSpawnRate);

            spySpawnRate = CustomOption.Create(240, Types.Crewmate, cs(Spy.color, ModTranslation.getString("spy")), rates, null, true);
            spyCanDieToSheriff = CustomOption.Create(241, Types.Crewmate, ModTranslation.getString("spyCanDieToSheriff"), false, spySpawnRate);
            spyImpostorsCanKillAnyone = CustomOption.Create(242, Types.Crewmate, ModTranslation.getString("spyImpostorsCanKillAnyone"), true, spySpawnRate);
            spyCanEnterVents = CustomOption.Create(243, Types.Crewmate, ModTranslation.getString("spyCanEnterVents"), false, spySpawnRate);
            spyHasImpostorVision = CustomOption.Create(244, Types.Crewmate, ModTranslation.getString("spyHasImpostorVision"), false, spySpawnRate);

            taskMasterSpawnRate = CustomOption.Create(7020, Types.Crewmate, cs(TaskMaster.color, ModTranslation.getString("taskMaster")), rates, null, true);
            taskMasterBecomeATaskMasterWhenCompleteAllTasks = CustomOption.Create(7021, Types.Crewmate, ModTranslation.getString("taskMasterBecomeATaskMasterWhenCompleteAllTasks"), false, taskMasterSpawnRate);
            taskMasterExtraCommonTasks = CustomOption.Create(7022, Types.Crewmate, ModTranslation.getString("taskMasterExtraCommonTasks"), 2f, 0f, 3f, 1f, taskMasterSpawnRate);
            taskMasterExtraShortTasks = CustomOption.Create(7023, Types.Crewmate, ModTranslation.getString("taskMasterExtraShortTasks"), 2f, 1f, 23f, 1f, taskMasterSpawnRate);
            taskMasterExtraLongTasks = CustomOption.Create(7024, Types.Crewmate, ModTranslation.getString("taskMasterExtraLongTasks"), 2f, 0f, 15f, 1f, taskMasterSpawnRate);

            portalmakerSpawnRate = CustomOption.Create(390, Types.Crewmate, cs(Portalmaker.color, ModTranslation.getString("portalmaker")), rates, null, true);
            portalmakerCooldown = CustomOption.Create(391, Types.Crewmate, ModTranslation.getString("portalmakerCooldown"), 30f, 10f, 60f, 2.5f, portalmakerSpawnRate);
            portalmakerUsePortalCooldown = CustomOption.Create(392, Types.Crewmate, ModTranslation.getString("portalmakerUsePortalCooldown"), 30f, 10f, 60f, 2.5f, portalmakerSpawnRate);
            portalmakerLogOnlyColorType = CustomOption.Create(393, Types.Crewmate, ModTranslation.getString("portalmakerLogOnlyColorType"), true, portalmakerSpawnRate);
            portalmakerLogHasTime = CustomOption.Create(394, Types.Crewmate, ModTranslation.getString("portalmakerLogHasTime"), true, portalmakerSpawnRate);
            portalmakerCanPortalFromAnywhere = CustomOption.Create(395, Types.Crewmate, ModTranslation.getString("portalmakerCanPortalFromAnywhere"), true, portalmakerSpawnRate);

            securityGuardSpawnRate = CustomOption.Create(280, Types.Crewmate, cs(SecurityGuard.color, ModTranslation.getString("securityGuard")), rates, null, true);
            securityGuardCooldown = CustomOption.Create(281, Types.Crewmate, ModTranslation.getString("securityGuardCooldown"), 30f, 10f, 60f, 2.5f, securityGuardSpawnRate);
            securityGuardTotalScrews = CustomOption.Create(282, Types.Crewmate, ModTranslation.getString("securityGuardTotalScrews"), 7f, 1f, 15f, 1f, securityGuardSpawnRate);
            securityGuardCamPrice = CustomOption.Create(283, Types.Crewmate, ModTranslation.getString("securityGuardCamPrice"), 2f, 1f, 15f, 1f, securityGuardSpawnRate);
            securityGuardVentPrice = CustomOption.Create(284, Types.Crewmate, ModTranslation.getString("securityGuardVentPrice"), 1f, 1f, 15f, 1f, securityGuardSpawnRate);
            securityGuardCamDuration = CustomOption.Create(285, Types.Crewmate, ModTranslation.getString("securityGuardCamDuration"), 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
            securityGuardCamMaxCharges = CustomOption.Create(286, Types.Crewmate, ModTranslation.getString("securityGuardCamMaxCharges"), 5f, 1f, 30f, 1f, securityGuardSpawnRate);
            securityGuardCamRechargeTasksNumber = CustomOption.Create(287, Types.Crewmate, ModTranslation.getString("securityGuardCamRechargeTasksNumber"), 3f, 1f, 10f, 1f, securityGuardSpawnRate);
            securityGuardNoMove = CustomOption.Create(288, Types.Crewmate, ModTranslation.getString("securityGuardNoMove"), true, securityGuardSpawnRate);

            mediumSpawnRate = CustomOption.Create(360, Types.Crewmate, cs(Medium.color, ModTranslation.getString("medium")), rates, null, true);
            mediumCooldown = CustomOption.Create(361, Types.Crewmate, ModTranslation.getString("mediumCooldown"), 30f, 5f, 120f, 5f, mediumSpawnRate);
            mediumDuration = CustomOption.Create(362, Types.Crewmate, ModTranslation.getString("mediumDuration"), 3f, 0f, 15f, 1f, mediumSpawnRate);
            mediumOneTimeUse = CustomOption.Create(363, Types.Crewmate, ModTranslation.getString("mediumOneTimeUse"), false, mediumSpawnRate);
            mediumChanceAdditionalInfo = CustomOption.Create(364, Types.Crewmate, ModTranslation.getString("mediumChanceAdditionalInfo"), rates, mediumSpawnRate);

            thiefSpawnRate = CustomOption.Create(400, Types.Neutral, cs(Thief.color, ModTranslation.getString("thief")), rates, null, true);
            thiefCooldown = CustomOption.Create(401, Types.Neutral, ModTranslation.getString("thiefCooldown"), 30f, 5f, 120f, 5f, thiefSpawnRate);
            thiefCanKillSheriff = CustomOption.Create(402, Types.Neutral, ModTranslation.getString("thiefCanKillSheriff"), true, thiefSpawnRate);
            thiefHasImpVision = CustomOption.Create(403, Types.Neutral, ModTranslation.getString("thiefHasImpVision"), true, thiefSpawnRate);
            thiefCanUseVents = CustomOption.Create(404, Types.Neutral, ModTranslation.getString("thiefCanUseVents"), true, thiefSpawnRate);
            thiefCanStealWithGuess = CustomOption.Create(405, Types.Neutral, ModTranslation.getString("thiefCanStealWithGuess"), false, thiefSpawnRate);

            /*trapperSpawnRate = CustomOption.Create(410, Types.Crewmate, cs(Trapper.color, "Trapper"), rates, null, true);
            trapperCooldown = CustomOption.Create(420, Types.Crewmate, "Trapper Cooldown", 30f, 5f, 120f, 5f, trapperSpawnRate);
            trapperMaxCharges = CustomOption.Create(440, Types.Crewmate, "Max Traps Charges", 5f, 1f, 15f, 1f, trapperSpawnRate);
            trapperRechargeTasksNumber = CustomOption.Create(450, Types.Crewmate, "Number Of Tasks Needed For Recharging", 2f, 1f, 15f, 1f, trapperSpawnRate);
            trapperTrapNeededTriggerToReveal = CustomOption.Create(451, Types.Crewmate, "Trap Needed Trigger To Reveal", 3f, 2f, 10f, 1f, trapperSpawnRate);
            trapperAnonymousMap = CustomOption.Create(452, Types.Crewmate, "Show Anonymous Map", false, trapperSpawnRate);
            trapperInfoType = CustomOption.Create(453, Types.Crewmate, "Trap Information Type", new string[] { "Role", "Good/Evil Role", "Name" }, trapperSpawnRate);
            trapperTrapDuration = CustomOption.Create(454, Types.Crewmate, "Trap Duration", 5f, 1f, 15f, 1f, trapperSpawnRate);*/

            // Modifier (1000 - 1999)
            modifiersAreHidden = CustomOption.Create(1009, Types.Modifier, cs(Color.yellow, ModTranslation.getString("modifiersAreHidden")), true, null, true);

            modifierBloody = CustomOption.Create(1000, Types.Modifier, cs(Color.yellow, ModTranslation.getString("bloody")), rates, null, true);
            modifierBloodyQuantity = CustomOption.Create(1001, Types.Modifier, cs(Color.yellow, ModTranslation.getString("bloodyQuantity")), ratesModifier, modifierBloody);
            modifierBloodyDuration = CustomOption.Create(1002, Types.Modifier, ModTranslation.getString("bloodDuration"), 10f, 3f, 60f, 1f, modifierBloody);

            modifierAntiTeleport = CustomOption.Create(1010, Types.Modifier, cs(Color.yellow, ModTranslation.getString("antiTeleport")), rates, null, true);
            modifierAntiTeleportQuantity = CustomOption.Create(1011, Types.Modifier, cs(Color.yellow, ModTranslation.getString("antiTeleportQuantity")), ratesModifier, modifierAntiTeleport);

            modifierTieBreaker = CustomOption.Create(1020, Types.Modifier, cs(Color.yellow, ModTranslation.getString("tiebreakerLongDesc")), rates, null, true);

            /*modifierBait = CustomOption.Create(1030, Types.Modifier, cs(Color.yellow, "Bait"), rates, null, true);
            modifierBaitQuantity = CustomOption.Create(1031, Types.Modifier, cs(Color.yellow, "Bait Quantity"), ratesModifier, modifierBait);
            modifierBaitReportDelayMin = CustomOption.Create(1032, Types.Modifier, "Bait Report Delay Min", 0f, 0f, 10f, 1f, modifierBait);
            modifierBaitReportDelayMax = CustomOption.Create(1033, Types.Modifier, "Bait Report Delay Max", 0f, 0f, 10f, 1f, modifierBait);
            modifierBaitShowKillFlash = CustomOption.Create(1034, Types.Modifier, "Warn The Killer With A Flash", true, modifierBait);*/

            modifierLover = CustomOption.Create(1040, Types.Modifier, cs(Color.yellow, ModTranslation.getString("lovers")), rates, null, true);
            modifierLoverImpLoverRate = CustomOption.Create(1041, Types.Modifier, ModTranslation.getString("loversImpLoverRate"), rates, modifierLover);
            modifierLoverBothDie = CustomOption.Create(1042, Types.Modifier, ModTranslation.getString("loversBothDie"), true, modifierLover);
            modifierLoverEnableChat = CustomOption.Create(1043, Types.Modifier, ModTranslation.getString("loversEnableChat"), true, modifierLover);

            modifierSunglasses = CustomOption.Create(1050, Types.Modifier, cs(Color.yellow, ModTranslation.getString("sunglasses")), rates, null, true);
            modifierSunglassesQuantity = CustomOption.Create(1051, Types.Modifier, cs(Color.yellow, ModTranslation.getString("sunglassesQuantity")), ratesModifier, modifierSunglasses);
            modifierSunglassesVision = CustomOption.Create(1052, Types.Modifier, ModTranslation.getString("sunglassesVision"), new string[] { "-10%", "-20%", "-30%", "-40%", "-50%" }, modifierSunglasses);

            modifierMini = CustomOption.Create(1061, Types.Modifier, cs(Color.yellow, ModTranslation.getString("mini")), rates, null, true);
            modifierMiniGrowingUpDuration = CustomOption.Create(1062, Types.Modifier, ModTranslation.getString("miniGrowingUpDuration"), 400f, 100f, 1500f, 100f, modifierMini);
            modifierMiniGrowingUpInMeeting = CustomOption.Create(1063, Types.Modifier, ModTranslation.getString("miniGrowingUpInMeeting"), true, modifierMini);

            modifierVip = CustomOption.Create(1070, Types.Modifier, cs(Color.yellow, ModTranslation.getString("vip")), rates, null, true);
            modifierVipQuantity = CustomOption.Create(1071, Types.Modifier, cs(Color.yellow, ModTranslation.getString("vipQuantity")), ratesModifier, modifierVip);
            modifierVipShowColor = CustomOption.Create(1072, Types.Modifier, ModTranslation.getString("vipShowColor"), true, modifierVip);

            modifierInvert = CustomOption.Create(1080, Types.Modifier, cs(Color.yellow, ModTranslation.getString("invert")), rates, null, true);
            modifierInvertQuantity = CustomOption.Create(1081, Types.Modifier, cs(Color.yellow, ModTranslation.getString("invertQuantity")), ratesModifier, modifierInvert);
            modifierInvertDuration = CustomOption.Create(1082, Types.Modifier, ModTranslation.getString("invertDuration"), 3f, 1f, 15f, 1f, modifierInvert);

            modifierChameleon = CustomOption.Create(1090, Types.Modifier, cs(Color.yellow, ModTranslation.getString("chameleon")), rates, null, true);
            modifierChameleonQuantity = CustomOption.Create(1091, Types.Modifier, cs(Color.yellow, ModTranslation.getString("chameleonQuantity")), ratesModifier, modifierChameleon);
            modifierChameleonHoldDuration = CustomOption.Create(1092, Types.Modifier, ModTranslation.getString("chameleonHoldDuration"), 3f, 1f, 10f, 0.5f, modifierChameleon);
            modifierChameleonFadeDuration = CustomOption.Create(1093, Types.Modifier, ModTranslation.getString("chameleonFadeDuration"), 1f, 0.25f, 10f, 0.25f, modifierChameleon);
            modifierChameleonMinVisibility = CustomOption.Create(1094, Types.Modifier, ModTranslation.getString("chameleonMinVisibility"), new string[] { "0%", "10%", "20%", "30%", "40%", "50%" }, modifierChameleon);

            madmateSpawnRate = CustomOption.Create(4041, Types.Modifier, cs(Color.yellow, ModTranslation.getString("madmate")), rates, null, true);
            madmateQuantity = CustomOption.Create(7005, Types.Modifier, cs(Color.yellow, ModTranslation.getString("madmateQuantity")), ratesModifier, madmateSpawnRate);
            madmateAbility = CustomOption.Create(4047, Types.Modifier, ModTranslation.getString("madmateAbility"), true, madmateSpawnRate);
            madmateCommonTasks = CustomOption.Create(4049, Types.Modifier, ModTranslation.getString("madmateCommonTasks"), 1f, 0f, 3f, 1f, madmateAbility);
            madmateShortTasks = CustomOption.Create(4048, Types.Modifier, ModTranslation.getString("madmateShortTasks"), 3f, 0f, 4f, 1f, madmateAbility);
            madmateLongTasks = CustomOption.Create(7000, Types.Modifier, ModTranslation.getString("madmateLongTasks"), 1f, 0f, 4f, 1f, madmateAbility);
            madmateCanDieToSheriff = CustomOption.Create(4042, Types.Modifier, ModTranslation.getString("madmateCanDieToSheriff"), false, madmateSpawnRate);
            madmateCanEnterVents = CustomOption.Create(4043, Types.Modifier, ModTranslation.getString("madmateCanEnterVents"), false, madmateSpawnRate);
            madmateCanSabotage = CustomOption.Create(7010, Types.Modifier, ModTranslation.getString("madmateCanSabotage"), false, madmateSpawnRate);
            madmateHasImpostorVision = CustomOption.Create(4044, Types.Modifier, ModTranslation.getString("madmateHasImpostorVision"), false, madmateSpawnRate);
            madmateCanFixComm = CustomOption.Create(7001, Types.Modifier, ModTranslation.getString("madmateCanFixComm"), true, madmateSpawnRate);

            //modifierShifter = CustomOption.Create(1100, Types.Modifier, cs(Color.yellow, "Shifter"), rates, null, true);

            // Guesser Gamemode (2000 - 2999)
            guesserGamemodeCrewNumber = CustomOption.Create(2001, Types.Guesser, cs(Guesser.color, ModTranslation.getString("guesserGamemodeCrewNumber")), 15f, 1f, 15f, 1f, null, true);
            guesserGamemodeNeutralNumber = CustomOption.Create(2002, Types.Guesser, cs(Guesser.color, ModTranslation.getString("guesserGamemodeNeutralNumber")), 15f, 1f, 15f, 1f, null, true);
            guesserGamemodeImpNumber = CustomOption.Create(2003, Types.Guesser, cs(Guesser.color, ModTranslation.getString("guesserGamemodeImpNumber")), 15f, 1f, 15f, 1f, null, true);
            guesserForceJackalGuesser = CustomOption.Create(2007, Types.Guesser, ModTranslation.getString("guesserForceJackalGuesser"), false, null, true);
            guesserForceThiefGuesser = CustomOption.Create(2011, Types.Guesser, ModTranslation.getString("guesserForceThiefGuesser"), false, null, true);
            guesserGamemodeHaveModifier = CustomOption.Create(2004, Types.Guesser, ModTranslation.getString("guesserGamemodeHaveModifier"), true, null);
            guesserGamemodeNumberOfShots = CustomOption.Create(2005, Types.Guesser, ModTranslation.getString("guesserGamemodeNumberOfShots"), 3f, 1f, 15f, 1f, null);
            guesserGamemodeHasMultipleShotsPerMeeting = CustomOption.Create(2006, Types.Guesser, ModTranslation.getString("guesserGamemodeHasMultipleShotsPerMeeting"), false, null);
            guesserGamemodeKillsThroughShield = CustomOption.Create(2008, Types.Guesser, ModTranslation.getString("guesserGamemodeKillsThroughShield"), true, null);
            guesserGamemodeEvilCanKillSpy = CustomOption.Create(2009, Types.Guesser, ModTranslation.getString("guesserGamemodeEvilCanKillSpy"), true, null);
            guesserGamemodeCantGuessSnitchIfTaksDone = CustomOption.Create(2010, Types.Guesser, ModTranslation.getString("guesserGamemodeCantGuessSnitchIfTaksDone"), true, null);

            // Hide N Seek Gamemode (3000 - 3999)
            hideNSeekMap = CustomOption.Create(3020, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekMap")), new string[] { "The Skeld", "Mira", "Polus", "Airship", "Submerged" }, null, true);
            hideNSeekHunterCount = CustomOption.Create(3000, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekHunterCount")), 1f, 1f, 3f, 1f);
            hideNSeekKillCooldown = CustomOption.Create(3021, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekKillCooldown")), 10f, 2.5f, 60f, 2.5f);
            hideNSeekHunterVision = CustomOption.Create(3001, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekHunterVision")), 0.5f, 0.25f, 2f, 0.25f);
            hideNSeekHuntedVision = CustomOption.Create(3002, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekHuntedVision")), 2f, 0.25f, 5f, 0.25f);
            hideNSeekCommonTasks = CustomOption.Create(3023, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekCommonTasks")), 1f, 0f, 4f, 1f);
            hideNSeekShortTasks = CustomOption.Create(3024, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekShortTasks")), 3f, 1f, 23f, 1f);
            hideNSeekLongTasks = CustomOption.Create(3025, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekLongTasks")), 3f, 0f, 15f, 1f);
            hideNSeekTimer = CustomOption.Create(3003, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekTimer")), 5f, 1f, 30f, 1f);
            hideNSeekTaskWin = CustomOption.Create(3004, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekTaskWin")), false);
            hideNSeekTaskPunish = CustomOption.Create(3017, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekTaskPunish")), 10f, 0f, 30f, 1f);
            hideNSeekCanSabotage = CustomOption.Create(3019, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekCanSabotage")), false);
            hideNSeekHunterWaiting = CustomOption.Create(3022, Types.HideNSeekMain, cs(Color.yellow, ModTranslation.getString("hideNSeekHunterWaiting")), 15f, 2.5f, 60f, 2.5f);

            hunterLightCooldown = CustomOption.Create(3005, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterLightCooldown")), 30f, 5f, 60f, 1f, null, true);
            hunterLightDuration = CustomOption.Create(3006, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterLightDuration")), 5f, 1f, 60f, 1f);
            hunterLightVision = CustomOption.Create(3007, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterLightVision")), 3f, 1f, 5f, 0.25f);
            hunterLightPunish = CustomOption.Create(3008, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterLightPunish")), 5f, 0f, 30f, 1f);
            hunterAdminCooldown = CustomOption.Create(3009, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterAdminCooldown")), 30f, 5f, 60f, 1f);
            hunterAdminDuration = CustomOption.Create(3010, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterAdminDuration")), 5f, 1f, 60f, 1f);
            hunterAdminPunish = CustomOption.Create(3011, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterAdminPunish")), 5f, 0f, 30f, 1f);
            hunterArrowCooldown = CustomOption.Create(3012, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterArrowCooldown")), 30f, 5f, 60f, 1f);
            hunterArrowDuration = CustomOption.Create(3013, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterArrowDuration")), 5f, 0f, 60f, 1f);
            hunterArrowPunish = CustomOption.Create(3014, Types.HideNSeekRoles, cs(Color.red, ModTranslation.getString("hunterArrowPunish")), 5f, 0f, 30f, 1f);

            huntedShieldCooldown = CustomOption.Create(3015, Types.HideNSeekRoles, cs(Color.gray, ModTranslation.getString("huntedShieldCooldown")), 30f, 5f, 60f, 1f, null, true);
            huntedShieldDuration = CustomOption.Create(3016, Types.HideNSeekRoles, cs(Color.gray, ModTranslation.getString("huntedShieldDuration")), 5f, 1f, 60f, 1f);
            huntedShieldRewindTime = CustomOption.Create(3018, Types.HideNSeekRoles, cs(Color.gray, ModTranslation.getString("huntedShieldRewindTime")), 3f, 1f, 10f, 1f);
            huntedShieldNumber = CustomOption.Create(3026, Types.HideNSeekRoles, cs(Color.gray, ModTranslation.getString("huntedShieldNumber")), 3f, 1f, 15f, 1f);

            // Other options
            maxNumberOfMeetings = CustomOption.Create(3, Types.General, ModTranslation.getString("maxNumberOfMeetings"), 10, 0, 15, 1, null, true);
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, Types.General, ModTranslation.getString("blockSkippingInEmergencyMeetings"), false);
            noVoteIsSelfVote = CustomOption.Create(5, Types.General, ModTranslation.getString("noVoteIsSelfVote"), false, blockSkippingInEmergencyMeetings);
            hidePlayerNames = CustomOption.Create(6, Types.General,  ModTranslation.getString("hidePlayerNames"), false);
            allowParallelMedBayScans = CustomOption.Create(7, Types.General, ModTranslation.getString("allowParallelMedBayScans"), false);
            shieldFirstKill = CustomOption.Create(8, Types.General, ModTranslation.getString("shieldFirstKill"), false);
            finishTasksBeforeHauntingOrZoomingOut = CustomOption.Create(9, Types.General, ModTranslation.getString("finishTasksBeforeHauntingOrZoomingOut"), true);
            camsNightVision = CustomOption.Create(11, Types.General, ModTranslation.getString("camsNightVision"), false, null, true);
            camsNoNightVisionIfImpVision = CustomOption.Create(12, Types.General, ModTranslation.getString("camsNoNightVisionIfImpVision"), false, camsNightVision, false);
            additionalVents = CustomOption.Create(5060, Types.General, ModTranslation.getString("additionalVents"), false);
            specimenVital = CustomOption.Create(5061, Types.General, ModTranslation.getString("specimenVital"), false);
            airshipLadder = CustomOption.Create(6070, Types.General, ModTranslation.getString("airshipLadder"), false);
            airshipOptimize = CustomOption.Create(6072, Types.General, ModTranslation.getString("airshipOptimize"), false);
            randomGameStartPosition = CustomOption.Create(6071, Types.General, ModTranslation.getString("randomGameStartPosition"), false);


            dynamicMap = CustomOption.Create(500, Types.General, ModTranslation.getString("dynamicMap"), false, null, true);
            dynamicMapEnableSkeld = CustomOption.Create(501, Types.General, "Skeld", rates, dynamicMap, false);
            dynamicMapEnableMira = CustomOption.Create(502, Types.General, "Mira", rates, dynamicMap, false);
            dynamicMapEnablePolus = CustomOption.Create(503, Types.General, "Polus", rates, dynamicMap, false);
            dynamicMapEnableAirShip = CustomOption.Create(504, Types.General, "Airship", rates, dynamicMap, false);
            dynamicMapEnableSubmerged = CustomOption.Create(505, Types.General, "Submerged", rates, dynamicMap, false);
            dynamicMapSeparateSettings = CustomOption.Create(509, Types.General, "Use Random Map Setting Presets", false, dynamicMap, false);

            blockedRolePairings.Add((byte)RoleId.Vampire, new [] { (byte)RoleId.Warlock});
            blockedRolePairings.Add((byte)RoleId.Warlock, new [] { (byte)RoleId.Vampire});
            blockedRolePairings.Add((byte)RoleId.Spy, new [] { (byte)RoleId.Mini});
            blockedRolePairings.Add((byte)RoleId.Mini, new [] { (byte)RoleId.Spy});
            blockedRolePairings.Add((byte)RoleId.Vulture, new [] { (byte)RoleId.Cleaner});
            blockedRolePairings.Add((byte)RoleId.Cleaner, new [] { (byte)RoleId.Vulture});
            
        }
    }
}
