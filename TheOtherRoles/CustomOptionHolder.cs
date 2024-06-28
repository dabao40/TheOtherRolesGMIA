using Epic.OnlineServices.RTCAudio;
using System.Collections.Generic;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Types = TheOtherRoles.CustomOption.CustomOptionType;

namespace TheOtherRoles {
    public class CustomOptionHolder {
        public static string[] rates = new string[]{"0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"};
        public static string[] ratesModifier = new string[]{"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
        public static string[] presets = new string[]{ "preset1", "preset2", "Random Preset Skeld", "Random Preset Mira HQ", "Random Preset Polus", "Random Preset Airship", "Random Preset Submerged" };

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

        public static CustomOption enableEventMode;
        public static CustomOption anyPlayerCanStopStart;

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

        public static CustomOption CatcherSpawnRate;
        public static CustomOption catchCooldown;
        public static CustomOption catchChance;

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
        public static CustomOption jesterCanVent;

        public static CustomOption arsonistSpawnRate;
        public static CustomOption arsonistCooldown;
        public static CustomOption arsonistDuration;

        public static CustomOption jackalSpawnRate;
        public static CustomOption jackalKillCooldown;
        public static CustomOption jackalCreateSidekickCooldown;
        public static CustomOption jackalCanSabotageLights;
        public static CustomOption jackalCanUseVents;
        public static CustomOption jackalCanCreateSidekick;
        public static CustomOption sidekickPromotesToJackal;
        public static CustomOption sidekickCanKill;
        public static CustomOption sidekickCanUseVents;
        public static CustomOption sidekickCanSabotageLights;
        public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
        public static CustomOption jackalCanCreateSidekickFromImpostor;
        public static CustomOption jackalAndSidekickHaveImpostorVision;

        public static CustomOption opportunistSpawnRate;

        public static CustomOption plagueDoctorSpawnRate;
        public static CustomOption plagueDoctorInfectCooldown;
        public static CustomOption plagueDoctorNumInfections;
        public static CustomOption plagueDoctorDistance;
        public static CustomOption plagueDoctorDuration;
        public static CustomOption plagueDoctorImmunityTime;
        public static CustomOption plagueDoctorInfectKiller;
        public static CustomOption plagueDoctorWinDead;

        public static CustomOption jekyllAndHydeSpawnRate;
        public static CustomOption jekyllAndHydeNumberToWin;
        public static CustomOption jekyllAndHydeCooldown;
        public static CustomOption jekyllAndHydeSuicideTimer;
        public static CustomOption jekyllAndHydeResetAfterMeeting;
        public static CustomOption jekyllAndHydeCommonTasks;
        public static CustomOption jekyllAndHydeShortTasks;
        public static CustomOption jekyllAndHydeLongTasks;
        public static CustomOption jekyllAndHydeNumTasks;

        public static CustomOption foxSpawnRate;
        public static CustomOption foxNumTasks;
        public static CustomOption foxStayTime;
        public static CustomOption foxTaskType;
        public static CustomOption foxCanCreateImmoralist;
        public static CustomOption foxCrewWinsByTasks;
        public static CustomOption foxImpostorWinsBySabotage;
        public static CustomOption foxStealthCooldown;
        public static CustomOption foxStealthDuration;
        public static CustomOption foxNumRepairs;

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
        public static CustomOption trackerTrackingMethod;

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

        public static CustomOption prophetSpawnRate;
        public static CustomOption prophetCooldown;
        public static CustomOption prophetNumExamines;
        public static CustomOption prophetPowerCrewAsRed;
        public static CustomOption prophetNeutralAsRed;
        public static CustomOption prophetCanCallEmergency;
        public static CustomOption prophetIsRevealed;
        public static CustomOption prophetExaminesToBeRevealed;

        public static CustomOption teleporterSpawnRate;
        public static CustomOption teleporterCooldown;
        public static CustomOption teleporterSampleCooldown;
        public static CustomOption teleporterTeleportNumber;

        public static CustomOption tricksterSpawnRate;
        public static CustomOption tricksterPlaceBoxCooldown;
        public static CustomOption tricksterLightsOutCooldown;
        public static CustomOption tricksterLightsOutDuration;

        public static CustomOption blackmailerSpawnRate;
        public static CustomOption blackmailerCooldown;

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
        public static CustomOption evilTrackerCanSeeTargetTask;
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

        public static CustomOption trapperSpawnRate;
        public static CustomOption trapperNumTrap;
        public static CustomOption trapperKillTimer;
        public static CustomOption trapperCooldown;
        public static CustomOption trapperMaxDistance;
        public static CustomOption trapperTrapRange;
        public static CustomOption trapperExtensionTime;
        public static CustomOption trapperPenaltyTime;
        public static CustomOption trapperBonusTime;

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
        public static CustomOption lawyerWinsAfterMeetings;
        public static CustomOption lawyerNeededMeetings;
        public static CustomOption pursuerCooldown;
        public static CustomOption pursuerBlanksNumber;

        public static CustomOption cupidSpawnRate;
        public static CustomOption cupidTimeLimit;
        public static CustomOption cupidShield;

        public static CustomOption moriartySpawnRate;
        public static CustomOption moriartyBrainwashTime;
        public static CustomOption moriartyBrainwashCooldown;
        public static CustomOption moriartyNumberToWin;

        public static CustomOption akujoSpawnRate;
        public static CustomOption akujoTimeLimit;
        public static CustomOption akujoKnowsRoles;
        public static CustomOption akujoNumKeeps;

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
        public static CustomOption additionalVents;
        public static CustomOption specimenVital;
        public static CustomOption airshipLadder;
        public static CustomOption airshipOptimize;
        public static CustomOption airshipAdditionalSpawn;
        public static CustomOption fungleElectrical;
        public static CustomOption miraVitals;
        public static CustomOption randomGameStartPosition;
        public static CustomOption activateProps;
        public static CustomOption numAccelTraps;
        public static CustomOption accelerationDuration;
        public static CustomOption speedAcceleration;
        public static CustomOption numDecelTraps;
        public static CustomOption decelerationDuration;
        public static CustomOption speedDeceleration;
        public static CustomOption decelUpdateInterval;

        public static CustomOption dynamicMap;
        public static CustomOption dynamicMapEnableSkeld;
        public static CustomOption dynamicMapEnableMira;
        public static CustomOption dynamicMapEnablePolus;
        public static CustomOption dynamicMapEnableAirShip;
        public static CustomOption dynamicMapEnableSubmerged;
        public static CustomOption dynamicMapEnableFungle;
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
        public static CustomOption guesserGamemodeSidekickIsAlwaysGuesser;

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
            presetSelection = CustomOption.Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, null, true);
            activateRoles = CustomOption.Create(1, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "blockOriginal"), true, null, true);

            anyPlayerCanStopStart = CustomOption.Create(2, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "anyPlayerCanStopStart"), false, null, false);
            if (Utilities.EventUtility.canBeEnabled) enableEventMode = CustomOption.Create(10423, Types.General, cs(Color.green, "enableEventMode"), true, null, true);

            // Using new id's for the options to not break compatibilty with older versions
            crewmateRolesCountMin = CustomOption.Create(300, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMin"), 15f, 0f, 15f, 1f, null, true, "unitPlayers");
            crewmateRolesCountMax = CustomOption.Create(301, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMax"), 15f, 0f, 15f, 1f, format: "unitPlayers");
            crewmateRolesFill = CustomOption.Create(308, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesFill"), false);
            neutralRolesCountMin = CustomOption.Create(302, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMin"), 15f, 0f, 15f, 1f, format: "unitPlayers");
            neutralRolesCountMax = CustomOption.Create(303, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMax"), 15f, 0f, 15f, 1f, format: "unitPlayers");
            impostorRolesCountMin = CustomOption.Create(304, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMin"), 15f, 0f, 15f, 1f, format: "unitPlayers");
            impostorRolesCountMax = CustomOption.Create(305, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMax"), 15f, 0f, 15f, 1f, format: "unitPlayers");
            modifiersCountMin = CustomOption.Create(306, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "modifiersCountMin"), 15f, 0f, 15f, 1f, format: "unitPlayers");
            modifiersCountMax = CustomOption.Create(307, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "modifiersCountMax"), 15f, 0f, 15f, 1f, format: "unitPlayers");

            mafiaSpawnRate = CustomOption.Create(18, Types.Impostor, cs(Janitor.color, "mafia"), rates, null, true);
            janitorCooldown = CustomOption.Create(19, Types.Impostor, "janitorCooldown", 30f, 10f, 60f, 2.5f, mafiaSpawnRate, false, "unitSeconds");

            morphlingSpawnRate = CustomOption.Create(20, Types.Impostor, cs(Morphling.color, "morphling"), rates, null, true);
            morphlingCooldown = CustomOption.Create(21, Types.Impostor, "morphlingCooldown", 30f, 10f, 60f, 2.5f, morphlingSpawnRate, false, "unitSeconds");
            morphlingDuration = CustomOption.Create(22, Types.Impostor, "morphlingDuration", 10f, 1f, 20f, 0.5f, morphlingSpawnRate, false, "unitSeconds");

            camouflagerSpawnRate = CustomOption.Create(30, Types.Impostor, cs(Camouflager.color, "camouflager"), rates, null, true);
            camouflagerCooldown = CustomOption.Create(31, Types.Impostor, "camouflagerCooldown", 30f, 10f, 60f, 2.5f, camouflagerSpawnRate, false, "unitSeconds");
            camouflagerDuration = CustomOption.Create(32, Types.Impostor, "camouflagerDuration", 10f, 1f, 20f, 0.5f, camouflagerSpawnRate, false, "unitSeconds");

            vampireSpawnRate = CustomOption.Create(40, Types.Impostor, cs(Vampire.color, "vampire"), rates, null, true);
            vampireKillDelay = CustomOption.Create(41, Types.Impostor, "vampireKillDelay", 10f, 1f, 20f, 1f, vampireSpawnRate, false, "unitSeconds");
            vampireCooldown = CustomOption.Create(42, Types.Impostor, "vampireCooldown", 30f, 10f, 60f, 2.5f, vampireSpawnRate, false, "unitSeconds");
            vampireCanKillNearGarlics = CustomOption.Create(43, Types.Impostor, "vampireCanKillNearGarlics", true, vampireSpawnRate);

            eraserSpawnRate = CustomOption.Create(230, Types.Impostor, cs(Eraser.color, "eraser"), rates, null, true);
            eraserCooldown = CustomOption.Create(231, Types.Impostor, "eraserCooldown", 30f, 10f, 120f, 5f, eraserSpawnRate, false, "unitSeconds");
            eraserCanEraseAnyone = CustomOption.Create(232, Types.Impostor, "eraserCanEraseAnyone", false, eraserSpawnRate);

            tricksterSpawnRate = CustomOption.Create(250, Types.Impostor, cs(Trickster.color, "trickster"), rates, null, true);
            tricksterPlaceBoxCooldown = CustomOption.Create(251, Types.Impostor, "tricksterPlaceBoxCooldown", 10f, 2.5f, 30f, 2.5f, tricksterSpawnRate, false, "unitSeconds");
            tricksterLightsOutCooldown = CustomOption.Create(252, Types.Impostor, "tricksterLightsOutCooldown", 30f, 10f, 60f, 5f, tricksterSpawnRate, false, "unitSeconds");
            tricksterLightsOutDuration = CustomOption.Create(253, Types.Impostor, "tricksterLightsOutDuration", 15f, 5f, 60f, 2.5f, tricksterSpawnRate, false, "unitSeconds");

            cleanerSpawnRate = CustomOption.Create(260, Types.Impostor, cs(Cleaner.color, "cleaner"), rates, null, true);
            cleanerCooldown = CustomOption.Create(261, Types.Impostor, "cleanerCooldown", 30f, 10f, 60f, 2.5f, cleanerSpawnRate, false, "unitSeconds");

            warlockSpawnRate = CustomOption.Create(270, Types.Impostor, cs(Cleaner.color, "warlock"), rates, null, true);
            warlockCooldown = CustomOption.Create(271, Types.Impostor, "warlockCooldown", 30f, 10f, 60f, 2.5f, warlockSpawnRate, false, "unitSeconds");
            warlockRootTime = CustomOption.Create(272, Types.Impostor, "warlockRootTime", 5f, 0f, 15f, 1f, warlockSpawnRate, false, "unitSeconds");

            bountyHunterSpawnRate = CustomOption.Create(320, Types.Impostor, cs(BountyHunter.color, "bountyHunter"), rates, null, true);
            bountyHunterBountyDuration = CustomOption.Create(321, Types.Impostor, "bountyHunterBountyDuration", 60f, 10f, 180f, 10f, bountyHunterSpawnRate, false, "unitSeconds");
            bountyHunterReducedCooldown = CustomOption.Create(322, Types.Impostor, "bountyHunterReducedCooldown", 2.5f, 0f, 30f, 2.5f, bountyHunterSpawnRate, false, "unitSeconds");
            bountyHunterPunishmentTime = CustomOption.Create(323, Types.Impostor, "bountyHunterPunishmentTime", 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate, false, "unitSeconds");
            bountyHunterShowArrow = CustomOption.Create(324, Types.Impostor, "bountyHunterShowArrow", true, bountyHunterSpawnRate);
            bountyHunterArrowUpdateIntervall = CustomOption.Create(325, Types.Impostor, "bountyHunterArrowUpdateInterval", 15f, 2.5f, 60f, 2.5f, bountyHunterShowArrow, false, "unitSeconds");

            witchSpawnRate = CustomOption.Create(370, Types.Impostor, cs(Witch.color, "witch"), rates, null, true);
            witchCooldown = CustomOption.Create(371, Types.Impostor, "witchSpellCooldown", 30f, 10f, 120f, 5f, witchSpawnRate, false, "unitSeconds");
            witchAdditionalCooldown = CustomOption.Create(372, Types.Impostor, "witchAdditionalCooldown", 10f, 0f, 60f, 5f, witchSpawnRate, false, "unitSeconds");
            witchCanSpellAnyone = CustomOption.Create(373, Types.Impostor, "witchCanSpellAnyone", false, witchSpawnRate);
            witchSpellCastingDuration = CustomOption.Create(374, Types.Impostor, "witchSpellDuration", 1f, 0f, 10f, 1f, witchSpawnRate, false, "unitSeconds");
            witchTriggerBothCooldowns = CustomOption.Create(375, Types.Impostor, "witchTriggerBoth", true, witchSpawnRate);
            witchVoteSavesTargets = CustomOption.Create(376, Types.Impostor, "witchSaveTargets", true, witchSpawnRate);

            assassinSpawnRate = CustomOption.Create(380, Types.Impostor, cs(Assassin.color, "assassin"), rates, null, true);
            assassinCooldown = CustomOption.Create(381, Types.Impostor, "assassinCooldown", 30f, 10f, 120f, 5f, assassinSpawnRate, false, "unitSeconds");
            assassinKnowsTargetLocation = CustomOption.Create(382, Types.Impostor, "assassinKnowsTargetLocation", true, assassinSpawnRate);
            assassinTraceTime = CustomOption.Create(383, Types.Impostor, "assassinTraceDuration", 5f, 1f, 20f, 0.5f, assassinSpawnRate, false, "unitSeconds");
            assassinTraceColorTime = CustomOption.Create(384, Types.Impostor, "assassinTraceColorTime", 2f, 0f, 20f, 0.5f, assassinSpawnRate, false, "unitSeconds");
            //assassinInvisibleDuration = CustomOption.Create(385, Types.Impostor, "Time The Assassin Is Invisible", 3f, 0f, 20f, 1f, assassinSpawnRate);

            ninjaSpawnRate = CustomOption.Create(5050, Types.Impostor, cs(Ninja.color, "ninja"), rates, null, true);
            ninjaStealthCooldown = CustomOption.Create(5051, Types.Impostor, "ninjaStealthCooldown", 30f, 2.5f, 60f, 2.5f, ninjaSpawnRate, false, "unitSeconds");
            ninjaStealthDuration = CustomOption.Create(5052, Types.Impostor, "ninjaStealthDuration", 15f, 2.5f, 60f, 2.5f, ninjaSpawnRate, false, "unitSeconds");
            ninjaFadeTime = CustomOption.Create(5053, Types.Impostor, "ninjaFadeTime", 0.5f, 0.0f, 2.5f, 0.5f, ninjaSpawnRate, false, "unitSeconds");
            ninjaKillPenalty = CustomOption.Create(5054, Types.Impostor, "ninjaKillPenalty", 10f, 0f, 60f, 2.5f, ninjaSpawnRate, false, "unitSeconds");
            ninjaSpeedBonus = CustomOption.Create(5055, Types.Impostor, "ninjaSpeedBonus", 1.25f, 0.5f, 2f, 0.25f, ninjaSpawnRate, false, "unitTimes");
            ninjaCanBeTargeted = CustomOption.Create(5056, Types.Impostor, "ninjaCanBeTargeted", true, ninjaSpawnRate);
            ninjaCanVent = CustomOption.Create(5057, Types.Impostor, "ninjaCanVent", false, ninjaSpawnRate);

            serialKillerSpawnRate = CustomOption.Create(4010, Types.Impostor, cs(SerialKiller.color, "serialKiller"), rates, null, true);
            serialKillerKillCooldown = CustomOption.Create(4011, Types.Impostor, "serialKillerKillCooldown", 15f, 2.5f, 60f, 2.5f, serialKillerSpawnRate, false, "unitSeconds");
            serialKillerSuicideTimer = CustomOption.Create(4012, Types.Impostor, "serialKillerSuicideTimer", 40f, 2.5f, 60f, 2.5f, serialKillerSpawnRate, false, "unitSeconds");
            serialKillerResetTimer = CustomOption.Create(4013, Types.Impostor, "serialKillerResetTimer", true, serialKillerSpawnRate);

            nekoKabochaSpawnRate = CustomOption.Create(880, Types.Impostor, cs(NekoKabocha.color, "nekoKabocha"), rates, null, true);
            nekoKabochaRevengeCrew = CustomOption.Create(881, Types.Impostor, "nekoKabochaRevengeCrew", true, nekoKabochaSpawnRate);
            nekoKabochaRevengeImpostor = CustomOption.Create(882, Types.Impostor, "nekoKabochaRevengeImpostor", true, nekoKabochaSpawnRate);
            nekoKabochaRevengeNeutral = CustomOption.Create(883, Types.Impostor, "nekoKabochaRevengeNeutral", true, nekoKabochaSpawnRate);
            nekoKabochaRevengeExile = CustomOption.Create(884, Types.Impostor, "nekoKabochaRevengeExile", false, nekoKabochaSpawnRate);

            evilTrackerSpawnRate = CustomOption.Create(4026, Types.Impostor, cs(EvilTracker.color, "evilTracker"), rates, null, true);
            evilTrackerCooldown = CustomOption.Create(4027, Types.Impostor, "evilTrackerCooldown", 10f, 0f, 60f, 5f, evilTrackerSpawnRate, false, "unitSeconds");
            evilTrackerResetTargetAfterMeeting = CustomOption.Create(4028, Types.Impostor, "evilTrackerResetTargetAfterMeeting", true, evilTrackerSpawnRate);
            evilTrackerCanSeeDeathFlash = CustomOption.Create(4029, Types.Impostor, "evilTrackerCanSeeDeathFlash", true, evilTrackerSpawnRate);
            evilTrackerCanSeeTargetPosition = CustomOption.Create(4031, Types.Impostor, "evilTrackerCanSeeTargetPosition", true, evilTrackerSpawnRate);
            evilTrackerCanSeeTargetTask = CustomOption.Create(4030, Types.Impostor, "evilTrackerCanSeeTargetTask", false, evilTrackerSpawnRate);
            evilTrackerCanSetTargetOnMeeting = CustomOption.Create(4032, Types.Impostor, "evilTrackerCanSetTargetOnMeeting", true, evilTrackerSpawnRate);

            undertakerSpawnRate = CustomOption.Create(4056, Types.Impostor, cs(Undertaker.color, "undertaker"), rates, null, true);
            undertakerSpeedDecrease = CustomOption.Create(4057, Types.Impostor, "undertakerSpeedDecrease", -50f, -80f, 0f, 10f, undertakerSpawnRate, false, "unitPercent");
            undertakerDisableVent = CustomOption.Create(4058, Types.Impostor, "undertakerDisableVent", true, undertakerSpawnRate);

            blackmailerSpawnRate = CustomOption.Create(710, Types.Impostor, cs(Blackmailer.color, "blackmailer"), rates, null, true);
            blackmailerCooldown = CustomOption.Create(711, Types.Impostor, "blackmailerCooldown", 30f, 5f, 120f, 5f, blackmailerSpawnRate, false, "unitSeconds");

            evilHackerSpawnRate = CustomOption.Create(8001, Types.Impostor, cs(EvilHacker.color, "evilHacker"), rates, null, true);
            evilHackerCanHasBetterAdmin = CustomOption.Create(8002, Types.Impostor, "evilHackerCanHasBetterAdmin", false, evilHackerSpawnRate);
            evilHackerCanSeeDoorStatus = CustomOption.Create(8020, Types.Impostor, "evilHackerCanSeeDoorStatus", true, evilHackerSpawnRate);
            evilHackerCanCreateMadmate = CustomOption.Create(8000, Types.Impostor, "evilHackerCanCreateMadmate", true, evilHackerSpawnRate);
            evilHackerCanCreateMadmateFromJackal = CustomOption.Create(8005, Types.Impostor, "evilHackerCanCreateMadmateFromJackal", true, evilHackerCanCreateMadmate);
            createdMadmateCanDieToSheriff = CustomOption.Create(8004, Types.Impostor, "createdMadmateCanDieToSheriff", true, evilHackerCanCreateMadmate);
            createdMadmateCanEnterVents = CustomOption.Create(8005, Types.Impostor, "createdMadmateCanEnterVents", true, evilHackerCanCreateMadmate);
            createdMadmateCanFixComm = CustomOption.Create(8006, Types.Impostor, "createdMadmateCanFixComm", false, evilHackerCanCreateMadmate);
            createdMadmateCanSabotage = CustomOption.Create(8007, Types.Impostor, "createdMadmateCanSabotage", false, evilHackerCanCreateMadmate);
            createdMadmateHasImpostorVision = CustomOption.Create(8008, Types.Impostor, "createdMadmateHasImpostorVision", true, evilHackerCanCreateMadmate);
            createdMadmateAbility = CustomOption.Create(8009, Types.Impostor, "createdMadmateAbility", true, evilHackerCanCreateMadmate);
            createdMadmateCommonTasks = CustomOption.Create(8010, Types.Impostor, "createdMadmateCommonTasks", 1f, 1f, 3f, 1f, createdMadmateAbility, false, "unitScrews");
            evilHackerCanInheritAbility = CustomOption.Create(8014, Types.Impostor, "evilHackerCanInheritAbility", false, evilHackerSpawnRate);

            trapperSpawnRate = CustomOption.Create(8016, Types.Impostor, cs(Trapper.color, "trapper"), rates, null, true);
            trapperNumTrap = CustomOption.Create(8017, Types.Impostor, "trapperNumTrap", 2f, 1f, 10f, 1f, trapperSpawnRate, false, "unitScrews");
            trapperExtensionTime = CustomOption.Create(8018, Types.Impostor, "trapperExtensionTime", 5f, 2f, 10f, 0.5f, trapperSpawnRate, false, "unitSeconds");
            trapperCooldown = CustomOption.Create(8019, Types.Impostor, "trapperCooldown", 10f, 10f, 60f, 2.5f, trapperSpawnRate, false, "unitSeconds");
            trapperKillTimer = CustomOption.Create(8025, Types.Impostor, "trapperKillTimer", 5f, 1f, 30f, 1f, trapperSpawnRate, false, "unitSeconds");
            trapperTrapRange = CustomOption.Create(8021, Types.Impostor, "trapperTrapRange", 1f, 0.5f, 5f, 0.1f, trapperSpawnRate, false, "unitMeters");
            trapperMaxDistance = CustomOption.Create(8022, Types.Impostor, "trapperMaxDistance", 10f, 1f, 50f, 1f, trapperSpawnRate, false, "unitMeters");
            trapperPenaltyTime = CustomOption.Create(8023, Types.Impostor, "trapperPenaltyTime", 10f, 0f, 50f, 1f, trapperSpawnRate, false, "unitSeconds");
            trapperBonusTime = CustomOption.Create(8024, Types.Impostor, "trapperBonusTime", 8f, 0f, 9f, 1f, trapperSpawnRate, false, "unitSeconds");

            mimicSpawnRate = CustomOption.Create(5000, Types.Impostor, cs(MimicK.color, "mimic"), rates, null, true);
            mimicCountAsOne = CustomOption.Create(5001, Types.Impostor, "mimicCountAsOne", true, mimicSpawnRate);
            mimicIfOneDiesBothDie = CustomOption.Create(5002, Types.Impostor, "mimicIfOneDiesBothDies", true, mimicSpawnRate);
            mimicHasOneVote = CustomOption.Create(5003, Types.Impostor, "mimicHasOneVote", true, mimicSpawnRate);

            bomberSpawnRate = CustomOption.Create(6076, Types.Impostor, cs(BomberA.color, "bomber"), rates, null, true);
            bomberCooldown = CustomOption.Create(6077, Types.Impostor, "bomberCooldown", 20f, 2f, 30f, 2f, bomberSpawnRate, false, "unitSeconds");
            bomberDuration = CustomOption.Create(6078, Types.Impostor, "bomberDuration", 2f, 1f, 10f, 0.5f, bomberSpawnRate, false, "unitSeconds");
            bomberCountAsOne = CustomOption.Create(6079, Types.Impostor, "bomberCountAsOne", true, bomberSpawnRate);
            bomberShowEffects = CustomOption.Create(6080, Types.Impostor, "bomberShowEffects", true, bomberSpawnRate);
            bomberIfOneDiesBothDie = CustomOption.Create(6081, Types.Impostor, "bomberIfOneDiesBothDie", true, bomberSpawnRate);
            bomberHasOneVote = CustomOption.Create(6801, Types.Impostor, "bomberHasOneVote", true, bomberSpawnRate);
            bomberAlwaysShowArrow = CustomOption.Create(6802, Types.Impostor, "bomberAlwaysShowArrow", true, bomberSpawnRate);

            /*bomberSpawnRate = CustomOption.Create(460, Types.Impostor, cs(Bomber.color, "Bomber"), rates, null, true);
            bomberBombDestructionTime = CustomOption.Create(461, Types.Impostor, "Bomb Destruction Time", 20f, 2.5f, 120f, 2.5f, bomberSpawnRate);
            bomberBombDestructionRange = CustomOption.Create(462, Types.Impostor, "Bomb Destruction Range", 50f, 5f, 150f, 5f, bomberSpawnRate);
            bomberBombHearRange = CustomOption.Create(463, Types.Impostor, "Bomb Hear Range", 60f, 5f, 150f, 5f, bomberSpawnRate);
            bomberDefuseDuration = CustomOption.Create(464, Types.Impostor, "Bomb Defuse Duration", 3f, 0.5f, 30f, 0.5f, bomberSpawnRate);
            bomberBombCooldown = CustomOption.Create(465, Types.Impostor, "Bomb Cooldown", 15f, 2.5f, 30f, 2.5f, bomberSpawnRate);
            bomberBombActiveAfter = CustomOption.Create(466, Types.Impostor, "Bomb Is Active After", 3f, 0.5f, 15f, 0.5f, bomberSpawnRate);*/

            guesserSpawnRate = CustomOption.Create(310, Types.Neutral, cs(Guesser.color, "guesser"), rates, null, true);
            guesserIsImpGuesserRate = CustomOption.Create(311, Types.Neutral, "guesserIsImpGuesserRate", rates, guesserSpawnRate);
            guesserNumberOfShots = CustomOption.Create(312, Types.Neutral, "guesserNumberOfShots", 2f, 1f, 15f, 1f, guesserSpawnRate, false, "unitShots");
            guesserHasMultipleShotsPerMeeting = CustomOption.Create(313, Types.Neutral, "guesserHasMultipleShotsPerMeeting", false, guesserSpawnRate);
            guesserKillsThroughShield  = CustomOption.Create(315, Types.Neutral, "guesserKillsThroughShield", true, guesserSpawnRate);
            guesserEvilCanKillSpy  = CustomOption.Create(316, Types.Neutral, "guesserEvilCanKillSpy", true, guesserSpawnRate);
            guesserSpawnBothRate = CustomOption.Create(317, Types.Neutral, "guesserSpawnBothRate", rates, guesserSpawnRate);
            guesserCantGuessSnitchIfTaksDone = CustomOption.Create(318, Types.Neutral, "guesserCantGuessSnitchIfTaksDone", true, guesserSpawnRate);

            swapperSpawnRate = CustomOption.Create(150, Types.Neutral, cs(Swapper.color, "swapper"), rates, null, true);
            swapperIsImpRate = CustomOption.Create(4036, Types.Neutral, "swapperIsImpRate", rates, swapperSpawnRate);
            swapperCanCallEmergency = CustomOption.Create(151, Types.Neutral, "swapperCanCallEmergency", false, swapperSpawnRate);
            swapperCanOnlySwapOthers = CustomOption.Create(152, Types.Neutral, "swapperCanOnlySwapOthers", false, swapperSpawnRate);

            swapperSwapsNumber = CustomOption.Create(153, Types.Neutral, "swapperSwapsNumber", 1f, 0f, 15f, 1f, swapperSpawnRate, false, "unitShots");
            swapperRechargeTasksNumber = CustomOption.Create(154, Types.Neutral, "swapperRechargeTasksNumber", 2f, 1f, 10f, 1f, swapperSpawnRate, false, "unitScrews");

            watcherSpawnRate = CustomOption.Create(1035, Types.Neutral, cs(Watcher.color, "watcher"), rates, null, true);
            watcherisImpWatcherRate = CustomOption.Create(1036, Types.Neutral, "watcherisImpWatcherRate", rates, watcherSpawnRate);
            watcherSeeGuesses = CustomOption.Create(5080, Types.Neutral, "watcherSeeGuesses", true, watcherSpawnRate);
            watcherSpawnBothRate = CustomOption.Create(1037, Types.Neutral, "watcherSpawnBothRate", rates, watcherSpawnRate);

            yasunaSpawnRate = CustomOption.Create(6040, Types.Neutral, cs(Yasuna.color, "yasuna"), rates, null, true);
            yasunaIsImpYasunaRate = CustomOption.Create(6041, Types.Neutral, "yasunaIsImpYasunaRate", rates, yasunaSpawnRate);
            yasunaNumberOfSpecialVotes = CustomOption.Create(6042, Types.Neutral, "yasunaNumberOfSpecialVotes", 1f, 1f, 15f, 1f, yasunaSpawnRate, false, "unitShots");
            yasunaSpecificMessageMode = CustomOption.Create(6043, Types.Neutral, "yasunaSpecificMessageMode", true, yasunaSpawnRate);

            jesterSpawnRate = CustomOption.Create(60, Types.Neutral, cs(Jester.color, "jester"), rates, null, true);
            jesterCanCallEmergency = CustomOption.Create(61, Types.Neutral, "jesterCanCallEmergency", true, jesterSpawnRate);
            jesterHasImpostorVision = CustomOption.Create(62, Types.Neutral, "jesterHasImpostorVision", false, jesterSpawnRate);
            jesterCanVent = CustomOption.Create(6088, Types.Neutral, "jesterCanVent", false, jesterSpawnRate);

            arsonistSpawnRate = CustomOption.Create(290, Types.Neutral, cs(Arsonist.color, "arsonist"), rates, null, true);
            arsonistCooldown = CustomOption.Create(291, Types.Neutral, "arsonistCooldown", 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate, false, "unitSeconds");
            arsonistDuration = CustomOption.Create(292, Types.Neutral, "arsonistDuration", 3f, 1f, 10f, 1f, arsonistSpawnRate, false, "unitSeconds");

            jackalSpawnRate = CustomOption.Create(220, Types.Neutral, cs(Jackal.color, "jackal"), rates, null, true);
            jackalKillCooldown = CustomOption.Create(221, Types.Neutral, "jackalKillCooldown", 30f, 10f, 60f, 2.5f, jackalSpawnRate, false, "unitSeconds");
            jackalCreateSidekickCooldown = CustomOption.Create(222, Types.Neutral, "jackalCreateSidekickCooldown", 30f, 10f, 60f, 2.5f, jackalSpawnRate, false, "unitSeconds");
            jackalCanUseVents = CustomOption.Create(223, Types.Neutral, "jackalCanUseVents", true, jackalSpawnRate);
            jackalCanSabotageLights = CustomOption.Create(431, Types.Neutral, "jackalCanSabotageLights", true, jackalSpawnRate);
            jackalCanCreateSidekick = CustomOption.Create(224, Types.Neutral, "jackalCanCreateSidekick", false, jackalSpawnRate);
            sidekickPromotesToJackal = CustomOption.Create(225, Types.Neutral, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
            sidekickCanKill = CustomOption.Create(226, Types.Neutral, "sidekickCanKill", false, jackalCanCreateSidekick);
            sidekickCanUseVents = CustomOption.Create(227, Types.Neutral, "sidekickCanUseVents", true, jackalCanCreateSidekick);
            sidekickCanSabotageLights = CustomOption.Create(432, Types.Neutral, "sidekickCanSabotageLights", true, jackalCanCreateSidekick);
            jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(228, Types.Neutral, "jackalPromotedFromSidekickCanCreateSidekick", true, sidekickPromotesToJackal);
            jackalCanCreateSidekickFromImpostor = CustomOption.Create(229, Types.Neutral, "jackalCanCreateSidekickFromImpostor", true, jackalCanCreateSidekick);
            jackalAndSidekickHaveImpostorVision = CustomOption.Create(430, Types.Neutral, "jackalAndSidekickHaveImpostorVision", false, jackalSpawnRate);

            vultureSpawnRate = CustomOption.Create(340, Types.Neutral, cs(Vulture.color, "vulture"), rates, null, true);
            vultureCooldown = CustomOption.Create(341, Types.Neutral, "vultureCooldown", 15f, 10f, 60f, 2.5f, vultureSpawnRate, false, "unitSeconds");
            vultureNumberToWin = CustomOption.Create(342, Types.Neutral, "vultureNumberToWin", 4f, 1f, 10f, 1f, vultureSpawnRate, false, "unitScrews");
            vultureCanUseVents = CustomOption.Create(343, Types.Neutral, "vultureCanUseVents", true, vultureSpawnRate);
            vultureShowArrows = CustomOption.Create(344, Types.Neutral, "vultureShowArrows", true, vultureSpawnRate);

            lawyerSpawnRate = CustomOption.Create(350, Types.Neutral, cs(Lawyer.color, "lawyer"), rates, null, true);
            lawyerTargetKnows = CustomOption.Create(351, Types.Neutral, "lawyerTargetKnows", true, lawyerSpawnRate);
            //lawyerIsProsecutorChance = CustomOption.Create(358, Types.Neutral, "Chance That The Lawyer Is Prosecutor", rates, lawyerSpawnRate);
            lawyerVision = CustomOption.Create(354, Types.Neutral, "lawyerVision", 1f, 0.25f, 3f, 0.25f, lawyerSpawnRate, false, "unitTimes");
            lawyerKnowsRole = CustomOption.Create(355, Types.Neutral, "lawyerKnowsRole", false, lawyerSpawnRate);
            lawyerWinsAfterMeetings = CustomOption.Create(352, Types.Neutral, "lawyerWinsMeeting", false, lawyerSpawnRate);
            lawyerNeededMeetings = CustomOption.Create(353, Types.Neutral, "lawyerMeetingsNeeded", 5f, 1f, 15f, 1f, lawyerWinsAfterMeetings, false, "unitShots");
            lawyerTargetCanBeJester = CustomOption.Create(351, Types.Neutral, "lawyerTargetCanBeJester", false, lawyerSpawnRate);
            pursuerCooldown = CustomOption.Create(356, Types.Neutral, "pursuerCooldown", 30f, 5f, 60f, 2.5f, lawyerSpawnRate, false, "unitSeconds");
            pursuerBlanksNumber = CustomOption.Create(357, Types.Neutral, "pursuerBlanksNumber", 5f, 1f, 20f, 1f, lawyerSpawnRate, false, "unitScrews");

            shifterSpawnRate = CustomOption.Create(1100, Types.Neutral, cs(Shifter.color, "shifter"), rates, null, true);
            shifterIsNeutralRate = CustomOption.Create(6007, Types.Neutral, "shifterIsNeutralRate", rates, shifterSpawnRate);
            shifterShiftsModifiers = CustomOption.Create(1101, Types.Neutral, "shifterShiftsModifiers", false, shifterSpawnRate);
            shifterPastShifters = CustomOption.Create(6008, Types.Neutral, "shifterPastShifters", false, shifterSpawnRate);

            opportunistSpawnRate = CustomOption.Create(4003, Types.Neutral, cs(Opportunist.color, "opportunist"), rates, null, true);

            plagueDoctorSpawnRate = CustomOption.Create(6000, Types.Neutral, cs(PlagueDoctor.color, "plagueDoctor"), rates, null, true);
            plagueDoctorInfectCooldown = CustomOption.Create(6001, Types.Neutral, "plagueDoctorInfectCooldown", 10f, 2.5f, 60f, 2.5f, plagueDoctorSpawnRate, false, "unitSeconds");
            plagueDoctorNumInfections = CustomOption.Create(6002, Types.Neutral, "plagueDoctorNumInfections", 1f, 1f, 3f, 1f, plagueDoctorSpawnRate, false, "unitPlayers");
            plagueDoctorDistance = CustomOption.Create(6003, Types.Neutral, "plagueDoctorDistance", 1f, 0.25f, 5f, 0.25f, plagueDoctorSpawnRate, false, "unitMeters");
            plagueDoctorDuration = CustomOption.Create(6004, Types.Neutral, "plagueDoctorDuration", 5f, 1f, 30f, 1f, plagueDoctorSpawnRate, false, "unitSeconds");
            plagueDoctorImmunityTime = CustomOption.Create(6005, Types.Neutral, "plagueDoctorImmunityTime", 10f, 1f, 30f, 1f, plagueDoctorSpawnRate, false, "unitSeconds");
            plagueDoctorInfectKiller = CustomOption.Create(6006, Types.Neutral, "plagueDoctorInfectKiller", true, plagueDoctorSpawnRate);
            plagueDoctorWinDead = CustomOption.Create(5999, Types.Neutral, "plagueDoctorWinDead", true, plagueDoctorSpawnRate);

            akujoSpawnRate = CustomOption.Create(8100, Types.Neutral, cs(Akujo.color, "akujo"), rates, null, true);
            akujoTimeLimit = CustomOption.Create(8101, Types.Neutral, "akujoTimeLimit", 300f, 30f, 1200f, 30f, akujoSpawnRate, false, "unitSeconds");
            akujoNumKeeps = CustomOption.Create(8102, Types.Neutral, "akujoNumKeeps", 2f, 1f, 10f, 1f, akujoSpawnRate, false, "unitPlayers");
            akujoKnowsRoles = CustomOption.Create(8103, Types.Neutral, "akujoKnowsRoles", true, akujoSpawnRate);

            cupidSpawnRate = CustomOption.Create(9050, Types.Neutral, cs(Cupid.color, "cupid"), rates, null, true);
            cupidTimeLimit = CustomOption.Create(9051, Types.Neutral, "cupidTimeLimit", 300f, 30f, 1200f, 30f, cupidSpawnRate, false, "unitSeconds");
            cupidShield = CustomOption.Create(9052, Types.Neutral, "cupidShield", true, cupidSpawnRate);

            jekyllAndHydeSpawnRate = CustomOption.Create(8104, Types.Neutral, cs(JekyllAndHyde.color, "jekyllAndHyde"), rates, null, true);
            jekyllAndHydeNumberToWin = CustomOption.Create(8105, Types.Neutral, "jekyllAndHydeNumberToWin", 3f, 1f, 10f, 1f, jekyllAndHydeSpawnRate, false, "unitScrews");
            jekyllAndHydeCooldown = CustomOption.Create(8106, Types.Neutral, "jekyllAndHydeCooldown", 18f, 2f, 30f, 1f, jekyllAndHydeSpawnRate, false, "unitSeconds");
            jekyllAndHydeSuicideTimer = CustomOption.Create(8107, Types.Neutral, "jekyllAndHydeSuicideTimer", 40f, 2.5f, 60f, 2.5f, jekyllAndHydeSpawnRate, false, "unitSeconds");
            jekyllAndHydeResetAfterMeeting = CustomOption.Create(8112, Types.Neutral, "jekyllAndHydeResetAfterMeeting", true, jekyllAndHydeSpawnRate);
            jekyllAndHydeCommonTasks = CustomOption.Create(8108, Types.Neutral, "jekyllAndHydeCommonTasks", 1f, 1f, 4f, 1f, jekyllAndHydeSpawnRate, false, "unitScrews");
            jekyllAndHydeShortTasks = CustomOption.Create(8109, Types.Neutral, "jekyllAndHydeShortTasks", 3f, 1f, 20f, 1f, jekyllAndHydeSpawnRate, false, "unitScrews");
            jekyllAndHydeLongTasks = CustomOption.Create(8110, Types.Neutral, "jekyllAndHydeLongTasks", 2f, 0f, 6f, 1f, jekyllAndHydeSpawnRate, false, "unitScrews");
            jekyllAndHydeNumTasks = CustomOption.Create(8111, Types.Neutral, "jekyllAndHydeNumTasks", 3f, 1f, 10f, 1f, jekyllAndHydeSpawnRate, false, "unitScrews");

            foxSpawnRate = CustomOption.Create(910, Types.Neutral, cs(Fox.color, "fox"), rates, null, true);
            foxNumTasks = CustomOption.Create(911, Types.Neutral, "foxNumTasks", 4f, 1f, 10f, 1f, foxSpawnRate, false, "unitScrews");
            foxStayTime = CustomOption.Create(913, Types.Neutral, "foxStayTime", 5f, 1f, 20f, 1f, foxSpawnRate, false, "unitSeconds");
            foxTaskType = CustomOption.Create(914, Types.Neutral, "foxTaskType", new string[] { "foxTaskSerial", "foxTaskParallel" }, foxSpawnRate);
            foxCrewWinsByTasks = CustomOption.Create(912, Types.Neutral, "foxCrewWinsByTasks", true, foxSpawnRate);
            foxImpostorWinsBySabotage = CustomOption.Create(919, Types.Neutral, "foxImpostorWinsBySabotage", true, foxSpawnRate);
            foxStealthCooldown = CustomOption.Create(916, Types.Neutral, "foxStealthCooldown", 15f, 1f, 30f, 1f, foxSpawnRate, false, "unitSeconds");
            foxStealthDuration = CustomOption.Create(917, Types.Neutral, "foxStealthDuration", 15f, 1f, 30f, 1f, foxSpawnRate, false, "unitSeconds");
            foxCanCreateImmoralist = CustomOption.Create(918, Types.Neutral, "foxCanCreateImmoralist", true, foxSpawnRate);
            foxNumRepairs = CustomOption.Create(920, Types.Neutral, "foxNumRepair", 1f, 0f, 10f, 1f, foxSpawnRate, false, "unitShots");

            mayorSpawnRate = CustomOption.Create(80, Types.Crewmate, cs(Mayor.color, "mayor"), rates, null, true);
            mayorCanSeeVoteColors = CustomOption.Create(81, Types.Crewmate, "mayorCanSeeVoteColor", false, mayorSpawnRate);
            mayorTasksNeededToSeeVoteColors = CustomOption.Create(82, Types.Crewmate, "mayorTasksNeededToSeeVoteColors", 5f, 0f, 20f, 1f, mayorCanSeeVoteColors, false, "unitScrews");
            mayorMeetingButton = CustomOption.Create(83, Types.Crewmate, "mayorMeetingButton", true, mayorSpawnRate);
            mayorMaxRemoteMeetings = CustomOption.Create(84, Types.Crewmate, "mayorMaxRemoteMeetings", 1f, 1f, 5f, 1f, mayorMeetingButton, false, "unitShots");
            mayorChooseSingleVote = CustomOption.Create(85, Types.Crewmate, "mayorChooseSingleVote", new string[] { "optionOff", "mayorBeforeVoting", "mayorUntilMeetingEnd" }, mayorSpawnRate);

            engineerSpawnRate = CustomOption.Create(90, Types.Crewmate, cs(Engineer.color, "engineer"), rates, null, true);
            engineerNumberOfFixes = CustomOption.Create(91, Types.Crewmate, "engineerNumberOfFixes", 1f, 1f, 6f, 1f, engineerSpawnRate, false, "unitShots");
            engineerHighlightForImpostors = CustomOption.Create(92, Types.Crewmate, "engineerHighlightForImpostors", true, engineerSpawnRate);
            engineerHighlightForTeamJackal = CustomOption.Create(93, Types.Crewmate, "engineerHighlightForTeamJackal", true, engineerSpawnRate);

            sheriffSpawnRate = CustomOption.Create(100, Types.Crewmate, cs(Sheriff.color, "sheriff"), rates, null, true);
            sheriffCooldown = CustomOption.Create(101, Types.Crewmate, "sheriffCooldown", 30f, 10f, 60f, 2.5f, sheriffSpawnRate, false, "unitSeconds");
            sheriffCanKillNeutrals = CustomOption.Create(102, Types.Crewmate, "sheriffCanKillNeutrals", false, sheriffSpawnRate);
            deputySpawnRate = CustomOption.Create(103, Types.Crewmate, "sheriffDeputy", rates, sheriffSpawnRate);
            deputyNumberOfHandcuffs = CustomOption.Create(104, Types.Crewmate, "deputyNumberOfHandcuffs", 3f, 1f, 10f, 1f, deputySpawnRate, false, "unitScrews");
            deputyHandcuffCooldown = CustomOption.Create(105, Types.Crewmate, "deputyHandcuffCooldown", 30f, 10f, 60f, 2.5f, deputySpawnRate, false, "unitSeconds");
            deputyHandcuffDuration = CustomOption.Create(106, Types.Crewmate, "deputyHandcuffDuration", 15f, 5f, 60f, 2.5f, deputySpawnRate, false, "unitSeconds");
            deputyKnowsSheriff = CustomOption.Create(107, Types.Crewmate, "deputyKnowsSheriff", true, deputySpawnRate);
            deputyGetsPromoted = CustomOption.Create(108, Types.Crewmate, "deputyGetsPromoted", new string[] { "optionOff", "deputyOnImmediately", "deputyOnAfterMeeting" }, deputySpawnRate);
            deputyKeepsHandcuffs = CustomOption.Create(109, Types.Crewmate, "deputyKeepsHandcuffs", true, deputyGetsPromoted);
            deputyStopsGameEnd = CustomOption.Create(4016, Types.Crewmate, "deputyStopsGameEnd", false, deputySpawnRate);

            lighterSpawnRate = CustomOption.Create(110, Types.Crewmate, cs(Lighter.color, "lighter"), rates, null, true);
            lighterModeLightsOnVision = CustomOption.Create(111, Types.Crewmate, "lighterModeLightsOnVision", 1.5f, 0.25f, 5f, 0.25f, lighterSpawnRate, false, "unitTimes");
            lighterModeLightsOffVision = CustomOption.Create(112, Types.Crewmate, "lighterModeLightsOffVision", 0.5f, 0.25f, 5f, 0.25f, lighterSpawnRate, false, "unitTimes");
            lighterFlashlightWidth = CustomOption.Create(113, Types.Crewmate, "lighterFlashlightWidth", 0.3f, 0.1f, 1f, 0.1f, lighterSpawnRate, false, "unitTimes");
            lighterCanSeeInvisible = CustomOption.Create(114, Types.Crewmate, "lighterCanSeeInvisible", true, lighterSpawnRate);

            sprinterSpawnRate = CustomOption.Create(4005, Types.Crewmate, cs(Sprinter.color, "sprinter"), rates, null, true);
            sprinterCooldown = CustomOption.Create(4006, Types.Crewmate, "sprinterCooldown", 30f, 2.5f, 60f, 2.5f, sprinterSpawnRate, false, "unitSeconds");
            sprinterDuration = CustomOption.Create(4007, Types.Crewmate, "sprintDuration", 15f, 10f, 60f, 2.5f, sprinterSpawnRate, false, "unitSeconds");
            sprinterFadeTime = CustomOption.Create(4008, Types.Crewmate, "sprintFadeTime", 0.5f, 0.0f, 2.5f, 0.5f, sprinterSpawnRate, false, "unitSeconds");

            detectiveSpawnRate = CustomOption.Create(120, Types.Crewmate, cs(Detective.color, "detective"), rates, null, true);
            detectiveAnonymousFootprints = CustomOption.Create(121, Types.Crewmate, "detectiveAnonymousFootprints", false, detectiveSpawnRate);
            detectiveFootprintIntervall = CustomOption.Create(122, Types.Crewmate, "detectiveFootprintInterval", 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, false, "unitSeconds");
            detectiveFootprintDuration = CustomOption.Create(123, Types.Crewmate, "detectiveFootprintDuration", 5f, 0.25f, 10f, 0.25f, detectiveSpawnRate, false, "unitSeconds");
            detectiveReportNameDuration = CustomOption.Create(124, Types.Crewmate, "detectiveReportNameDuration", 0, 0, 60, 2.5f, detectiveSpawnRate, false, "unitSeconds");
            detectiveReportColorDuration = CustomOption.Create(125, Types.Crewmate, "detectiveReportColorDuration", 20, 0, 120, 2.5f, detectiveSpawnRate, false, "unitSeconds");

            timeMasterSpawnRate = CustomOption.Create(130, Types.Crewmate, cs(TimeMaster.color, "timeMaster"), rates, null, true);
            timeMasterCooldown = CustomOption.Create(131, Types.Crewmate, "timeMasterCooldown", 30f, 10f, 120f, 2.5f, timeMasterSpawnRate, false, "unitSeconds");
            timeMasterRewindTime = CustomOption.Create(132, Types.Crewmate, "timeMasterRewindTime", 3f, 1f, 10f, 1f, timeMasterSpawnRate, false, "unitSeconds");
            timeMasterShieldDuration = CustomOption.Create(133, Types.Crewmate, "timeMasterShieldDuration", 3f, 1f, 20f, 1f, timeMasterSpawnRate, false, "unitSeconds");

            medicSpawnRate = CustomOption.Create(140, Types.Crewmate, cs(Medic.color, "medic"), rates, null, true);
            medicShowShielded = CustomOption.Create(143, Types.Crewmate, "medicShowShielded", new string[] { "medicShowShieldedAll", "medicShowShieldedBoth", "medicShowShieldedMedic" }, medicSpawnRate);
            medicShowAttemptToShielded = CustomOption.Create(144, Types.Crewmate, "medicShowAttemptToShielded", false, medicSpawnRate);
            medicSetOrShowShieldAfterMeeting = CustomOption.Create(145, Types.Crewmate, "medicSetOrShowShieldAfterMeeting", new string[] { "medicInstantly", "medicVisibleAfterMeeting", "medicAftermeeting" }, medicSpawnRate);

            medicShowAttemptToMedic = CustomOption.Create(146, Types.Crewmate, "medicShowAttemptToMedic", false, medicSpawnRate);

            fortuneTellerSpawnRate = CustomOption.Create(940, Types.Crewmate, cs(FortuneTeller.color, "fortuneTeller"), rates, null, true);
            fortuneTellerResults = CustomOption.Create(941, Types.Crewmate, "fortuneTellerResults", new string[] { "fortuneTellerResultCrew", "fortuneTellerResultTeam", "fortuneTellerResultRole" }, fortuneTellerSpawnRate);
            fortuneTellerNumTasks = CustomOption.Create(942, Types.Crewmate, "fortuneTellerNumTasks", 4f, 0f, 25f, 1f, fortuneTellerSpawnRate, false, "unitScrews");
            fortuneTellerDuration = CustomOption.Create(943, Types.Crewmate, "fortuneTellerDuration", 20f, 1f, 50f, 1f, fortuneTellerSpawnRate, false, "unitSeconds");
            fortuneTellerDistance = CustomOption.Create(944, Types.Crewmate, "fortuneTellerDistance", 2.5f, 1f, 10f, 0.5f, fortuneTellerSpawnRate, false, "unitMeters");

            seerSpawnRate = CustomOption.Create(160, Types.Crewmate, cs(Seer.color, "seer"), rates, null, true);
            seerMode = CustomOption.Create(161, Types.Crewmate, "seerMode", new string[] { "seerModeBoth", "seerModeFlash", "seerModeSouls" }, seerSpawnRate);
            seerLimitSoulDuration = CustomOption.Create(163, Types.Crewmate, "seerLimitSoulDuration", false, seerSpawnRate);
            seerSoulDuration = CustomOption.Create(162, Types.Crewmate, "seerSoulDuration", 15f, 0f, 120f, 5f, seerLimitSoulDuration, false, "unitSeconds");

            hackerSpawnRate = CustomOption.Create(170, Types.Crewmate, cs(Hacker.color, "hacker"), rates, null, true);
            hackerCooldown = CustomOption.Create(171, Types.Crewmate, "hackerCooldown", 30f, 5f, 60f, 5f, hackerSpawnRate, false, "unitSeconds");
            hackerHackeringDuration = CustomOption.Create(172, Types.Crewmate, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate, false, "unitSeconds");
            hackerOnlyColorType = CustomOption.Create(173, Types.Crewmate, "hackerOnlyColorType", false, hackerSpawnRate);
            hackerToolsNumber = CustomOption.Create(174, Types.Crewmate, "hackerToolsNumber", 5f, 1f, 30f, 1f, hackerSpawnRate, false, "unitScrews");
            hackerRechargeTasksNumber = CustomOption.Create(175, Types.Crewmate, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, hackerSpawnRate, false, "unitScrews");
            hackerNoMove = CustomOption.Create(176, Types.Crewmate, "hackerNoMove", true, hackerSpawnRate);

            baitSpawnRate = CustomOption.Create(1030, Types.Crewmate, cs(Bait.color, "bait"), rates, null, true);
            baitHighlightAllVents = CustomOption.Create(1031, Types.Crewmate, "baitHighlightAllVents", false, baitSpawnRate);
            baitReportDelay = CustomOption.Create(1032, Types.Crewmate, "baitReportDelay", 0f, 0f, 10f, 1f, baitSpawnRate, false, "unitSeconds");
            baitShowKillFlash = CustomOption.Create(1033, Types.Crewmate, "baitShowKillFlash", true, baitSpawnRate);

            veteranSpawnRate = CustomOption.Create(4050, Types.Crewmate, cs(Veteran.color, "veteran"), rates, null, true);
            veteranCooldown = CustomOption.Create(4051, Types.Crewmate, "veteranCooldown", 30f, 10f, 60f, 2.5f, veteranSpawnRate, false, "unitSeconds");
            veteranAlertDuration = CustomOption.Create(4052, Types.Crewmate, "veteranAlertDuration", 3f, 1f, 20f, 1f, veteranSpawnRate, false, "unitSeconds");
            veteranAlertNumber = CustomOption.Create(4053, Types.Crewmate, "veteranAlertNumber", 5f, 1f, 15f, 1f, veteranSpawnRate, false, "unitScrews");

            prophetSpawnRate = CustomOption.Create(9005, Types.Crewmate, cs(Prophet.color, "prophet"), rates, null, true);
            prophetCooldown = CustomOption.Create(9011, Types.Crewmate, "prophetCooldown", 30f, 5f, 60f, 1f, prophetSpawnRate, false, "unitSeconds");
            prophetNumExamines = CustomOption.Create(9006, Types.Crewmate, "prophetNumExamines", 4f, 1f, 10f, 1f, prophetSpawnRate, false, "unitScrews");
            prophetCanCallEmergency = CustomOption.Create(9007, Types.Crewmate, "prophetCanCallEmergency", false, prophetSpawnRate);
            prophetIsRevealed = CustomOption.Create(9012, Types.Crewmate, "prophetIsRevealed", true, prophetSpawnRate);
            prophetExaminesToBeRevealed = CustomOption.Create(9008, Types.Crewmate, "prophetExaminesToBeRevealed", 3f, 1f, 10f, 1f, prophetIsRevealed, false, "unitScrews");
            prophetNeutralAsRed = CustomOption.Create(9009, Types.Crewmate, "prophetNeutralAsRed", true, prophetSpawnRate);
            prophetPowerCrewAsRed = CustomOption.Create(9010, Types.Crewmate, "prophetPowerCrewAsRed", false, prophetSpawnRate);

            teleporterSpawnRate = CustomOption.Create(9000, Types.Crewmate, cs(Teleporter.color, "teleporter"), rates, null, true);
            teleporterSampleCooldown = CustomOption.Create(9004, Types.Crewmate, "teleporterSampleCooldown", 30f, 5f, 60f, 5f, teleporterSpawnRate, false, "unitSeconds");
            teleporterCooldown = CustomOption.Create(9001, Types.Crewmate, "teleporterCooldown", 30f, 5f, 120f, 5f, teleporterSpawnRate, false, "unitSeconds");
            teleporterTeleportNumber = CustomOption.Create(9003, Types.Crewmate, "teleporterTeleportNumber", 3f, 1f, 10f, 1f, teleporterSpawnRate, false, "unitScrews");

            CatcherSpawnRate = CustomOption.Create(10009, Types.Impostor, cs(Teleporter.color, "catcher"), rates, null, true);
            catchCooldown = CustomOption.Create(10010, Types.Impostor, "catchCooldown", 30f, 5f, 120f, 5f, CatcherSpawnRate, false, "unitSeconds");
            catchChance = CustomOption.Create(10011, Types.Impostor, "Catchchance", 3f, 1f, 10f, 1f, CatcherSpawnRate, false, "unitScrews");



            trackerSpawnRate = CustomOption.Create(200, Types.Crewmate, cs(Tracker.color, "tracker"), rates, null, true);
            trackerUpdateIntervall = CustomOption.Create(201, Types.Crewmate, "trackerUpdateInterval", 5f, 1f, 30f, 1f, trackerSpawnRate, false, "unitSeconds");
            trackerResetTargetAfterMeeting = CustomOption.Create(202, Types.Crewmate, "trackerResetTargetAfterMeeting", false, trackerSpawnRate);
            trackerCanTrackCorpses = CustomOption.Create(203, Types.Crewmate, "trackerCanTrackCorpses", true, trackerSpawnRate);
            trackerCorpsesTrackingCooldown = CustomOption.Create(204, Types.Crewmate, "trackerCorpsesTrackingCooldown", 30f, 5f, 120f, 5f, trackerCanTrackCorpses, false, "unitSeconds");
            trackerCorpsesTrackingDuration = CustomOption.Create(205, Types.Crewmate, "trackerCorpsesTrackingDuration", 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses, false, "unitSeconds");
            trackerTrackingMethod = CustomOption.Create(206, Types.Crewmate, "trackerTrackingMethod", new string[] { "trackerArrow", "trackerProximity", "trackerBoth" }, trackerSpawnRate);

            sherlockSpawnRate = CustomOption.Create(5070, Types.Crewmate, cs(Sherlock.color, "sherlock"), rates, null, true);
            sherlockCooldown = CustomOption.Create(5071, Types.Crewmate, "sherlockCooldown", 10f, 0f, 40f, 2.5f, sherlockSpawnRate, false, "unitSeconds");
            sherlockInvestigateDistance = CustomOption.Create(5072, Types.Crewmate, "sherlockInvestigateDistance", 5f, 1f, 15f, 1f, sherlockSpawnRate, false, "unitMeters");
            sherlockRechargeTasksNumber = CustomOption.Create(5073, Types.Crewmate, "sherlockRechargeTasksNumber", 2f, 1f, 5f, 1f, sherlockSpawnRate, false, "unitScrews");

            snitchSpawnRate = CustomOption.Create(210, Types.Crewmate, cs(Snitch.color, "snitch"), rates, null, true);
            snitchLeftTasksForReveal = CustomOption.Create(219, Types.Crewmate, "snitchLeftTasksForReveal", 5f, 0f, 25f, 1f, snitchSpawnRate, false, "unitScrews");
            snitchMode = CustomOption.Create(211, Types.Crewmate, "snitchMode", new string[] { "snitchChat", "snitchMap", "snitchChatAndMap" }, snitchSpawnRate);
            snitchTargets = CustomOption.Create(212, Types.Crewmate, "snitchTargets", new string[] { "snitchAllEvilPlayers", "snitchKillingPlayers" }, snitchSpawnRate);

            spySpawnRate = CustomOption.Create(240, Types.Crewmate, cs(Spy.color, "spy"), rates, null, true);
            spyCanDieToSheriff = CustomOption.Create(241, Types.Crewmate, "spyCanDieToSheriff", false, spySpawnRate);
            spyImpostorsCanKillAnyone = CustomOption.Create(242, Types.Crewmate, "spyImpostorsCanKillAnyone", true, spySpawnRate);
            spyCanEnterVents = CustomOption.Create(243, Types.Crewmate, "spyCanEnterVents", false, spySpawnRate);
            spyHasImpostorVision = CustomOption.Create(244, Types.Crewmate, "spyHasImpostorVision", false, spySpawnRate);

            taskMasterSpawnRate = CustomOption.Create(7020, Types.Crewmate, cs(TaskMaster.color, "taskMaster"), rates, null, true);
            taskMasterBecomeATaskMasterWhenCompleteAllTasks = CustomOption.Create(7021, Types.Crewmate, "taskMasterBecomeATaskMasterWhenCompleteAllTasks", false, taskMasterSpawnRate);
            taskMasterExtraCommonTasks = CustomOption.Create(7022, Types.Crewmate, "taskMasterExtraCommonTasks", 2f, 0f, 3f, 1f, taskMasterSpawnRate, false, "unitScrews");
            taskMasterExtraShortTasks = CustomOption.Create(7023, Types.Crewmate, "taskMasterExtraShortTasks", 2f, 1f, 23f, 1f, taskMasterSpawnRate, false, "unitScrews");
            taskMasterExtraLongTasks = CustomOption.Create(7024, Types.Crewmate, "taskMasterExtraLongTasks", 2f, 0f, 15f, 1f, taskMasterSpawnRate, false, "unitScrews");

            portalmakerSpawnRate = CustomOption.Create(390, Types.Crewmate, cs(Portalmaker.color, "portalmaker"), rates, null, true);
            portalmakerCooldown = CustomOption.Create(391, Types.Crewmate, "portalmakerCooldown", 30f, 10f, 60f, 2.5f, portalmakerSpawnRate, false, "unitSeconds");
            portalmakerUsePortalCooldown = CustomOption.Create(392, Types.Crewmate, "portalmakerUsePortalCooldown", 30f, 10f, 60f, 2.5f, portalmakerSpawnRate, false, "unitSeconds");
            portalmakerLogOnlyColorType = CustomOption.Create(393, Types.Crewmate, "portalmakerLogOnlyColorType", true, portalmakerSpawnRate);
            portalmakerLogHasTime = CustomOption.Create(394, Types.Crewmate, "portalmakerLogHasTime", true, portalmakerSpawnRate);
            portalmakerCanPortalFromAnywhere = CustomOption.Create(395, Types.Crewmate, "portalmakerCanPortalFromAnywhere", true, portalmakerSpawnRate);

            securityGuardSpawnRate = CustomOption.Create(280, Types.Crewmate, cs(SecurityGuard.color, "securityGuard"), rates, null, true);
            securityGuardCooldown = CustomOption.Create(281, Types.Crewmate, "securityGuardCooldown", 30f, 10f, 60f, 2.5f, securityGuardSpawnRate, false, "unitSeconds");
            securityGuardTotalScrews = CustomOption.Create(282, Types.Crewmate, "securityGuardTotalScrews", 7f, 1f, 15f, 1f, securityGuardSpawnRate, false, "unitScrews");
            securityGuardCamPrice = CustomOption.Create(283, Types.Crewmate, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate, false, "unitScrews");
            securityGuardVentPrice = CustomOption.Create(284, Types.Crewmate, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate, false, "unitScrews");
            securityGuardCamDuration = CustomOption.Create(285, Types.Crewmate, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate, false, "unitSeconds");
            securityGuardCamMaxCharges = CustomOption.Create(286, Types.Crewmate, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, securityGuardSpawnRate, false, "unitScrews");
            securityGuardCamRechargeTasksNumber = CustomOption.Create(287, Types.Crewmate, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, securityGuardSpawnRate, false, "unitScrews");
            securityGuardNoMove = CustomOption.Create(288, Types.Crewmate, "securityGuardNoMove", true, securityGuardSpawnRate);

            mediumSpawnRate = CustomOption.Create(360, Types.Crewmate, cs(Medium.color, "medium"), rates, null, true);
            mediumCooldown = CustomOption.Create(361, Types.Crewmate, "mediumCooldown", 30f, 5f, 120f, 5f, mediumSpawnRate, false, "unitSeconds");
            mediumDuration = CustomOption.Create(362, Types.Crewmate, "mediumDuration", 3f, 0f, 15f, 1f, mediumSpawnRate, false, "unitSeconds");
            mediumOneTimeUse = CustomOption.Create(363, Types.Crewmate, "mediumOneTimeUse", false, mediumSpawnRate);
            mediumChanceAdditionalInfo = CustomOption.Create(364, Types.Crewmate, "mediumChanceAdditionalInfo", rates, mediumSpawnRate);

            thiefSpawnRate = CustomOption.Create(400, Types.Neutral, cs(Thief.color, "thief"), rates, null, true);
            thiefCooldown = CustomOption.Create(401, Types.Neutral, "thiefCooldown", 30f, 5f, 120f, 5f, thiefSpawnRate, false, "unitSeconds");
            thiefCanKillSheriff = CustomOption.Create(402, Types.Neutral, "thiefCanKillSheriff", true, thiefSpawnRate);
            thiefHasImpVision = CustomOption.Create(403, Types.Neutral, "thiefHasImpVision", true, thiefSpawnRate);
            thiefCanUseVents = CustomOption.Create(404, Types.Neutral, "thiefCanUseVents", true, thiefSpawnRate);
            thiefCanStealWithGuess = CustomOption.Create(405, Types.Neutral, "thiefCanStealWithGuess", false, thiefSpawnRate);

            moriartySpawnRate = CustomOption.Create(8030, Types.Neutral, cs(Moriarty.color, "moriarty"), rates, null, true);
            moriartyBrainwashCooldown = CustomOption.Create(8031, Types.Neutral, "moriartyBrainwashCooldown", 30f, 10f, 60f, 1f, moriartySpawnRate, false, "unitSeconds");
            moriartyBrainwashTime = CustomOption.Create(8032, Types.Neutral, "moriartyBrainwashTime", 30f, 1f, 60f, 1f, moriartySpawnRate, false, "unitSeconds");
            moriartyNumberToWin = CustomOption.Create(8033, Types.Neutral, "moriartyNumberToWin", 3f, 1f, 10f, 1f, moriartySpawnRate, false, "unitScrews");

            /*trapperSpawnRate = CustomOption.Create(410, Types.Crewmate, cs(Trapper.color, "Trapper"), rates, null, true);
            trapperCooldown = CustomOption.Create(420, Types.Crewmate, "Trapper Cooldown", 30f, 5f, 120f, 5f, trapperSpawnRate);
            trapperMaxCharges = CustomOption.Create(440, Types.Crewmate, "Max Traps Charges", 5f, 1f, 15f, 1f, trapperSpawnRate);
            trapperRechargeTasksNumber = CustomOption.Create(450, Types.Crewmate, "Number Of Tasks Needed For Recharging", 2f, 1f, 15f, 1f, trapperSpawnRate);
            trapperTrapNeededTriggerToReveal = CustomOption.Create(451, Types.Crewmate, "Trap Needed Trigger To Reveal", 3f, 2f, 10f, 1f, trapperSpawnRate);
            trapperAnonymousMap = CustomOption.Create(452, Types.Crewmate, "Show Anonymous Map", false, trapperSpawnRate);
            trapperInfoType = CustomOption.Create(453, Types.Crewmate, "Trap Information Type", new string[] { "Role", "Good/Evil Role", "Name" }, trapperSpawnRate);
            trapperTrapDuration = CustomOption.Create(454, Types.Crewmate, "Trap Duration", 5f, 1f, 15f, 1f, trapperSpawnRate);*/

            // Modifier (1000 - 1999)
            modifiersAreHidden = CustomOption.Create(1009, Types.Modifier, cs(Color.yellow, "modifiersAreHidden"), true, null, true);

            modifierBloody = CustomOption.Create(1000, Types.Modifier, cs(Color.yellow, "bloody"), rates, null, true);
            modifierBloodyQuantity = CustomOption.Create(1001, Types.Modifier, cs(Color.yellow, "bloodyQuantity"), ratesModifier, modifierBloody);
            modifierBloodyDuration = CustomOption.Create(1002, Types.Modifier, "bloodDuration", 10f, 3f, 60f, 1f, modifierBloody, false, "unitSeconds");

            modifierAntiTeleport = CustomOption.Create(1010, Types.Modifier, cs(Color.yellow, "antiTeleport"), rates, null, true);
            modifierAntiTeleportQuantity = CustomOption.Create(1011, Types.Modifier, cs(Color.yellow, "antiTeleportQuantity"), ratesModifier, modifierAntiTeleport);

            modifierTieBreaker = CustomOption.Create(1020, Types.Modifier, cs(Color.yellow, "tiebreakerLongDesc"), rates, null, true);

            /*modifierBait = CustomOption.Create(1030, Types.Modifier, cs(Color.yellow, "Bait"), rates, null, true);
            modifierBaitQuantity = CustomOption.Create(1031, Types.Modifier, cs(Color.yellow, "Bait Quantity"), ratesModifier, modifierBait);
            modifierBaitReportDelayMin = CustomOption.Create(1032, Types.Modifier, "Bait Report Delay Min", 0f, 0f, 10f, 1f, modifierBait);
            modifierBaitReportDelayMax = CustomOption.Create(1033, Types.Modifier, "Bait Report Delay Max", 0f, 0f, 10f, 1f, modifierBait);
            modifierBaitShowKillFlash = CustomOption.Create(1034, Types.Modifier, "Warn The Killer With A Flash", true, modifierBait);*/

            modifierLover = CustomOption.Create(1040, Types.Modifier, cs(Color.yellow, "lovers"), rates, null, true);
            modifierLoverImpLoverRate = CustomOption.Create(1041, Types.Modifier, "loversImpLoverRate", rates, modifierLover);
            modifierLoverBothDie = CustomOption.Create(1042, Types.Modifier, "loversBothDie", true, modifierLover);
            modifierLoverEnableChat = CustomOption.Create(1043, Types.Modifier, "loversEnableChat", true, modifierLover);

            modifierSunglasses = CustomOption.Create(1050, Types.Modifier, cs(Color.yellow, "sunglasses"), rates, null, true);
            modifierSunglassesQuantity = CustomOption.Create(1051, Types.Modifier, cs(Color.yellow, "sunglassesQuantity"), ratesModifier, modifierSunglasses);
            modifierSunglassesVision = CustomOption.Create(1052, Types.Modifier, "sunglassesVision", new string[] { "-10%", "-20%", "-30%", "-40%", "-50%" }, modifierSunglasses);

            modifierMini = CustomOption.Create(1061, Types.Modifier, cs(Color.yellow, "mini"), rates, null, true);
            modifierMiniGrowingUpDuration = CustomOption.Create(1062, Types.Modifier, "miniGrowingUpDuration", 400f, 100f, 1500f, 100f, modifierMini, false, "unitSeconds");
            modifierMiniGrowingUpInMeeting = CustomOption.Create(1063, Types.Modifier, "miniGrowingUpInMeeting", true, modifierMini);

            modifierVip = CustomOption.Create(1070, Types.Modifier, cs(Color.yellow, "vip"), rates, null, true);
            modifierVipQuantity = CustomOption.Create(1071, Types.Modifier, cs(Color.yellow, "vipQuantity"), ratesModifier, modifierVip);
            modifierVipShowColor = CustomOption.Create(1072, Types.Modifier, "vipShowColor", true, modifierVip);

            modifierInvert = CustomOption.Create(1080, Types.Modifier, cs(Color.yellow, "invert"), rates, null, true);
            modifierInvertQuantity = CustomOption.Create(1081, Types.Modifier, cs(Color.yellow, "invertQuantity"), ratesModifier, modifierInvert);
            modifierInvertDuration = CustomOption.Create(1082, Types.Modifier, "invertDuration", 3f, 1f, 15f, 1f, modifierInvert, false, "unitScrews");

            modifierChameleon = CustomOption.Create(1090, Types.Modifier, cs(Color.yellow, "chameleon"), rates, null, true);
            modifierChameleonQuantity = CustomOption.Create(1091, Types.Modifier, cs(Color.yellow, "chameleonQuantity"), ratesModifier, modifierChameleon);
            modifierChameleonHoldDuration = CustomOption.Create(1092, Types.Modifier, "chameleonHoldDuration", 3f, 1f, 10f, 0.5f, modifierChameleon, false, "unitSeconds");
            modifierChameleonFadeDuration = CustomOption.Create(1093, Types.Modifier, "chameleonFadeDuration", 1f, 0.25f, 10f, 0.25f, modifierChameleon, false, "unitSeconds");
            modifierChameleonMinVisibility = CustomOption.Create(1094, Types.Modifier, "chameleonMinVisibility", new string[] { "0%", "10%", "20%", "30%", "40%", "50%" }, modifierChameleon);

            madmateSpawnRate = CustomOption.Create(4041, Types.Modifier, cs(Color.yellow, "madmate"), rates, null, true);
            madmateQuantity = CustomOption.Create(7005, Types.Modifier, cs(Color.yellow, "madmateQuantity"), ratesModifier, madmateSpawnRate);
            madmateAbility = CustomOption.Create(4047, Types.Modifier, "madmateAbility", true, madmateSpawnRate);
            madmateCommonTasks = CustomOption.Create(4049, Types.Modifier, "madmateCommonTasks", 1f, 0f, 3f, 1f, madmateAbility, false, "unitScrews");
            madmateShortTasks = CustomOption.Create(4048, Types.Modifier, "madmateShortTasks", 3f, 0f, 4f, 1f, madmateAbility, false, "unitScrews");
            madmateLongTasks = CustomOption.Create(7000, Types.Modifier, "madmateLongTasks", 1f, 0f, 4f, 1f, madmateAbility, false, "unitScrews");
            madmateCanDieToSheriff = CustomOption.Create(4042, Types.Modifier, "madmateCanDieToSheriff", false, madmateSpawnRate);
            madmateCanEnterVents = CustomOption.Create(4043, Types.Modifier, "madmateCanEnterVents", false, madmateSpawnRate);
            madmateCanSabotage = CustomOption.Create(7010, Types.Modifier, "madmateCanSabotage", false, madmateSpawnRate);
            madmateHasImpostorVision = CustomOption.Create(4044, Types.Modifier, "madmateHasImpostorVision", false, madmateSpawnRate);
            madmateCanFixComm = CustomOption.Create(7001, Types.Modifier, "madmateCanFixComm", true, madmateSpawnRate);

            //modifierShifter = CustomOption.Create(1100, Types.Modifier, cs(Color.yellow, "Shifter"), rates, null, true);

            // Guesser Gamemode (2000 - 2999)
            guesserGamemodeCrewNumber = CustomOption.Create(2001, Types.Guesser, cs(Guesser.color, "guesserGamemodeCrewNumber"), 15f, 1f, 15f, 1f, null, true, "unitPlayers");
            guesserGamemodeNeutralNumber = CustomOption.Create(2002, Types.Guesser, cs(Guesser.color, "guesserGamemodeNeutralNumber"), 15f, 1f, 15f, 1f, null, true, "unitPlayers");
            guesserGamemodeImpNumber = CustomOption.Create(2003, Types.Guesser, cs(Guesser.color, "guesserGamemodeImpNumber"), 15f, 1f, 15f, 1f, null, true, "unitPlayers");
            guesserForceJackalGuesser = CustomOption.Create(2007, Types.Guesser, "guesserForceJackalGuesser", false, null, true);
            guesserGamemodeSidekickIsAlwaysGuesser = CustomOption.Create(2012, Types.Guesser, "guesserGamemodeSidekickIsAlwaysGuesser", false, null);
            guesserForceThiefGuesser = CustomOption.Create(2011, Types.Guesser, "guesserForceThiefGuesser", false, null, true);
            guesserGamemodeHaveModifier = CustomOption.Create(2004, Types.Guesser, "guesserGamemodeHaveModifier", true, null);
            guesserGamemodeNumberOfShots = CustomOption.Create(2005, Types.Guesser, "guesserGamemodeNumberOfShots", 3f, 1f, 15f, 1f, null, false, "unitShots");
            guesserGamemodeHasMultipleShotsPerMeeting = CustomOption.Create(2006, Types.Guesser, "guesserGamemodeHasMultipleShotsPerMeeting", false, null);
            guesserGamemodeKillsThroughShield = CustomOption.Create(2008, Types.Guesser, "guesserGamemodeKillsThroughShield", true, null);
            guesserGamemodeEvilCanKillSpy = CustomOption.Create(2009, Types.Guesser, "guesserGamemodeEvilCanKillSpy", true, null);
            guesserGamemodeCantGuessSnitchIfTaksDone = CustomOption.Create(2010, Types.Guesser, "guesserGamemodeCantGuessSnitchIfTaksDone", true, null);

            // Hide N Seek Gamemode (3000 - 3999)
            hideNSeekMap = CustomOption.Create(3020, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekMap"), new string[] { "The Skeld", "Mira", "Polus", "Airship", "Submerged" }, null, true);
            hideNSeekHunterCount = CustomOption.Create(3000, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHunterCount"), 1f, 1f, 3f, 1f, format: "unitPlayers");
            hideNSeekKillCooldown = CustomOption.Create(3021, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekKillCooldown"), 10f, 2.5f, 60f, 2.5f, format: "unitSeconds");
            hideNSeekHunterVision = CustomOption.Create(3001, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHunterVision"), 0.5f, 0.25f, 2f, 0.25f, format: "unitSeconds");
            hideNSeekHuntedVision = CustomOption.Create(3002, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHuntedVision"), 2f, 0.25f, 5f, 0.25f, format: "unitTimes");
            hideNSeekCommonTasks = CustomOption.Create(3023, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekCommonTasks"), 1f, 0f, 4f, 1f, format: "unitScrews");
            hideNSeekShortTasks = CustomOption.Create(3024, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekShortTasks"), 3f, 1f, 23f, 1f, format: "unitScrews");
            hideNSeekLongTasks = CustomOption.Create(3025, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekLongTasks"), 3f, 0f, 15f, 1f, format: "unitScrews");
            hideNSeekTimer = CustomOption.Create(3003, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekTimer"), 5f, 1f, 30f, 1f);
            hideNSeekTaskWin = CustomOption.Create(3004, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekTaskWin"), false);
            hideNSeekTaskPunish = CustomOption.Create(3017, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekTaskPunish"), 10f, 0f, 30f, 1f, format: "unitSeconds");
            hideNSeekCanSabotage = CustomOption.Create(3019, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekCanSabotage"), false);
            hideNSeekHunterWaiting = CustomOption.Create(3022, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHunterWaiting"), 15f, 2.5f, 60f, 2.5f, format: "unitSeconds");

            hunterLightCooldown = CustomOption.Create(3005, Types.HideNSeekRoles, cs(Color.red, "hunterLightCooldown"), 30f, 5f, 60f, 1f, null, true, "unitSeconds");
            hunterLightDuration = CustomOption.Create(3006, Types.HideNSeekRoles, cs(Color.red, "hunterLightDuration"), 5f, 1f, 60f, 1f, format: "unitSeconds");
            hunterLightVision = CustomOption.Create(3007, Types.HideNSeekRoles, cs(Color.red, "hunterLightVision"), 3f, 1f, 5f, 0.25f, format: "unitTimes");
            hunterLightPunish = CustomOption.Create(3008, Types.HideNSeekRoles, cs(Color.red, "hunterLightPunish"), 5f, 0f, 30f, 1f, format: "unitSeconds");
            hunterAdminCooldown = CustomOption.Create(3009, Types.HideNSeekRoles, cs(Color.red, "hunterAdminCooldown"), 30f, 5f, 60f, 1f, format: "unitSeconds");
            hunterAdminDuration = CustomOption.Create(3010, Types.HideNSeekRoles, cs(Color.red, "hunterAdminDuration"), 5f, 1f, 60f, 1f, format: "unitSeconds");
            hunterAdminPunish = CustomOption.Create(3011, Types.HideNSeekRoles, cs(Color.red, "hunterAdminPunish"), 5f, 0f, 30f, 1f, format: "unitSeconds");
            hunterArrowCooldown = CustomOption.Create(3012, Types.HideNSeekRoles, cs(Color.red, "hunterArrowCooldown"), 30f, 5f, 60f, 1f, format: "unitSeconds");
            hunterArrowDuration = CustomOption.Create(3013, Types.HideNSeekRoles, cs(Color.red, "hunterArrowDuration"), 5f, 0f, 60f, 1f, format: "unitSeconds");
            hunterArrowPunish = CustomOption.Create(3014, Types.HideNSeekRoles, cs(Color.red, "hunterArrowPunish"), 5f, 0f, 30f, 1f, format: "unitSeconds");

            huntedShieldCooldown = CustomOption.Create(3015, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldCooldown"), 30f, 5f, 60f, 1f, null, true);
            huntedShieldDuration = CustomOption.Create(3016, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldDuration"), 5f, 1f, 60f, 1f, format: "unitSeconds");
            huntedShieldRewindTime = CustomOption.Create(3018, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldRewindTime"), 3f, 1f, 10f, 1f, format: "unitSeconds");
            huntedShieldNumber = CustomOption.Create(3026, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldNumber"), 3f, 1f, 15f, 1f, format: "unitScrews");

            // Other options
            maxNumberOfMeetings = CustomOption.Create(3, Types.General, "maxNumberOfMeetings", 10, 0, 15, 1, null, true, "unitShots");
            blockSkippingInEmergencyMeetings = CustomOption.Create(4, Types.General, "blockSkippingInEmergencyMeetings", false);
            noVoteIsSelfVote = CustomOption.Create(5, Types.General, "noVoteIsSelfVote", false, blockSkippingInEmergencyMeetings);
            hidePlayerNames = CustomOption.Create(6, Types.General,  "hidePlayerNames", false);
            allowParallelMedBayScans = CustomOption.Create(7, Types.General, "allowParallelMedBayScans", false);
            shieldFirstKill = CustomOption.Create(8, Types.General, "shieldFirstKill", false);
            finishTasksBeforeHauntingOrZoomingOut = CustomOption.Create(9, Types.General, "finishTasksBeforeHauntingOrZoomingOut", true);
            camsNightVision = CustomOption.Create(11, Types.General, "camsNightVision", false, null, true);
            camsNoNightVisionIfImpVision = CustomOption.Create(12, Types.General, "camsNoNightVisionIfImpVision", false, camsNightVision, false);
            additionalVents = CustomOption.Create(5060, Types.General, "additionalVents", false);
            specimenVital = CustomOption.Create(5061, Types.General, "specimenVital", false);
            miraVitals = CustomOption.Create(6075, Types.General, "miraVitals", false);
            airshipLadder = CustomOption.Create(6070, Types.General, "airshipLadder", false);
            airshipOptimize = CustomOption.Create(6072, Types.General, "airshipOptimize", false);
            airshipAdditionalSpawn = CustomOption.Create(6073, Types.General, "airshipAdditionalSpawn", false);
            fungleElectrical = CustomOption.Create(6074, Types.General, "fungleElectrical", false);
            randomGameStartPosition = CustomOption.Create(6071, Types.General, "randomGameStartPosition", false);
            activateProps = CustomOption.Create(6083, Types.General, "activateProps", false, null, true);
            numAccelTraps = CustomOption.Create(6084, Types.General, "numAccelTraps", 1f, 1f, 5f, 1f, activateProps, false, "unitScrews");
            accelerationDuration = CustomOption.Create(6085, Types.General, "accelerationDuration", 5f, 1f, 20f, 1f, activateProps, false, "unitSeconds");
            speedAcceleration = CustomOption.Create(6086, Types.General, "speedAcceleration", 1.25f, 0.5f, 2f, 0.25f, activateProps, false, "unitTimes");
            numDecelTraps = CustomOption.Create(6087, Types.General, "numDecelTraps", 1f, 1f, 3f, 1f, activateProps, false, "unitScrews");
            decelerationDuration = CustomOption.Create(6091, Types.General, "decelerationDuration", 5f, 1f, 20f, 1f, activateProps, false, "unitSeconds");
            speedDeceleration = CustomOption.Create(6089, Types.General, "speedDeceleration", -0.5f, -0.8f, -0.1f, 0.1f, activateProps, false, "unitTimes");
            decelUpdateInterval = CustomOption.Create(6090, Types.General, "decelUpdateInterval", 10f, 5f, 60f, 2.5f, activateProps, false, "unitSeconds");


            dynamicMap = CustomOption.Create(500, Types.General, "dynamicMap", false, null, true);
            dynamicMapEnableSkeld = CustomOption.Create(501, Types.General, "Skeld", rates, dynamicMap, false);
            dynamicMapEnableMira = CustomOption.Create(502, Types.General, "Mira", rates, dynamicMap, false);
            dynamicMapEnablePolus = CustomOption.Create(503, Types.General, "Polus", rates, dynamicMap, false);
            dynamicMapEnableAirShip = CustomOption.Create(504, Types.General, "Airship", rates, dynamicMap, false);
            dynamicMapEnableSubmerged = CustomOption.Create(505, Types.General, "Submerged", rates, dynamicMap, false);
            dynamicMapEnableFungle = CustomOption.Create(506, Types.General, "Fungle", rates, dynamicMap, false);
            dynamicMapSeparateSettings = CustomOption.Create(509, Types.General, "dynamicMapSeparateSettings", false, dynamicMap, false);

            blockedRolePairings.Add((byte)RoleId.Vampire, new [] { (byte)RoleId.Warlock});
            blockedRolePairings.Add((byte)RoleId.Warlock, new [] { (byte)RoleId.Vampire});
            blockedRolePairings.Add((byte)RoleId.Spy, new [] { (byte)RoleId.Mini});
            blockedRolePairings.Add((byte)RoleId.Mini, new [] { (byte)RoleId.Spy});
            blockedRolePairings.Add((byte)RoleId.Vulture, new [] { (byte)RoleId.Cleaner});
            blockedRolePairings.Add((byte)RoleId.Cleaner, new [] { (byte)RoleId.Vulture});
            blockedRolePairings.Add((byte)RoleId.Cupid, new[] { (byte)RoleId.Akujo });
            blockedRolePairings.Add((byte)RoleId.Akujo, new[] { (byte)RoleId.Cupid });
        }
    }
}
