using HarmonyLib;
using Hazel;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.HudManagerStartPatch;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.TORMapOptions;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils;
using TheOtherRoles.Modules;
using Reactor.Utilities.Extensions;

namespace TheOtherRoles
{
    public enum RoleId {
        Jester,
        Mayor,
        Portalmaker,
        Engineer,
        Sheriff,
        Deputy,
        Lighter,
        Godfather,
        Mafioso,
        Janitor,
        Detective,
        TimeMaster,
        Medic,
        Swapper,
        Seer,
        Sprinter,
        Morphling,
        Camouflager,
        Hacker,
        Tracker,
        Vampire,
        Snitch,
        Jackal,
        Sidekick,
        Eraser,
        FortuneTeller,
        Bait,
        Veteran,
        Sherlock,
        Spy,
        Trickster,
        Cleaner,
        Warlock,
        SecurityGuard,
        Arsonist,
        EvilGuesser,
        NiceGuesser,
        NiceWatcher,
        EvilWatcher, 
        BountyHunter,
        Vulture,
        Medium,
        Shifter, 
        Yasuna,
        Prophet,
        TaskMaster,
        Teleporter,
        EvilYasuna,
        //Trapper,
        Lawyer, 
        //Prosecutor,
        Pursuer,
        Moriarty,
        PlagueDoctor,
        Akujo,
        Cupid,
        JekyllAndHyde,
        Fox,
        Immoralist,
        Witch,
        Assassin,
        Ninja, 
        NekoKabocha,
        Thief,
        SerialKiller,
        EvilTracker,
        MimicK,
        MimicA,
        BomberA,
        BomberB,
        EvilHacker,
        Undertaker,
        Trapper,
        Blackmailer,
        Opportunist,
        Yoyo,
        Kataomoi,
        Busker,
        Noisemaker,
        Archaeologist,
        SchrodingersCat,
        Madmate,
        //Bomber,
        Crewmate,
        Impostor,
        // Modifier ---
        Lover,
        //Bait, Bait is no longer a modifier
        Bloody,
        AntiTeleport,
        Tiebreaker,
        Sunglasses,
        Mini,
        Vip,
        Invert,
        Chameleon,
        Armored,
        //Shifter
    }

    enum CustomRPC
    {
        // Main Controls

        ResetVaribles = 100,
        ShareOptions,
        ForceEnd,
        WorkaroundSetRoles,
        SetRole,
        SetModifier,
        VersionHandshake,
        UseUncheckedVent,
        UncheckedMurderPlayer,
        UncheckedCmdReportDeadBody,
        UncheckedExilePlayer,
        UncheckedSetTasks,
        DynamicMapOption,
        FinishShipStatusBegin,
        SetGameStarting,
        ShareGamemode,
        StopStart,

        // Role functionality

        EngineerFixLights = 120,
        EngineerFixSubmergedOxygen,
        EngineerUsedRepair,
        CleanBody,
        MedicSetShielded,
        ShieldedMurderAttempt,
        TimeMasterShield,
        TimeMasterRewindTime,
        ShifterShift,
        SwapperSwap,
        MorphlingMorph,
        CamouflagerCamouflage,
        TrackerUsedTracker,
        VampireSetBitten,
        PlaceGarlic,
        DeputyUsedHandcuffs,
        DeputyPromotes,
        JackalCreatesSidekick,
        SidekickPromotes,
        ErasePlayerRoles,
        SetFutureErased,
        SetFutureShifted,
        SetFutureShielded,
        SetFutureSpelled,
        PlaceAssassinTrace,
        PlacePortal,
        UsePortal,
        PlaceJackInTheBox,
        LightsOut,
        PlaceCamera,
        SealVent,
        ArsonistWin,
        GuesserShoot,
        LawyerSetTarget,
        LawyerPromotesToPursuer,
        SetBlanked,
        Bloody,
        SetFirstKill,
        Invert,
        SetTiebreak,
        SetInvisible,
        ThiefStealsRole,
        //SetTrap,
        //TriggerTrap,
        MayorSetVoteTwice,
        //PlaceBomb,
        //DefuseBomb,
        ShareRoom,

        // Gamemode
        SetGuesserGm,
        HuntedShield,
        HuntedRewindTime,

        // Other functionality
        ShareTimer,
        ShareGhostInfo,

        // GMIA Special functionality
        NinjaStealth,
        FortuneTellerUsedDivine, 
        NekoKabochaExile,
        SprinterSprint,
        SerialKillerSuicide,
        VeteranAlert,
        VeteranKill,
        UndertakerDragBody,
        UndertakerDropBody,
        MimicMorph,
        MimicResetMorph,
        SetShifterType,
        YasunaSpecialVote,
        YasunaSpecialVote_DoCastVote,
        TaskMasterSetExTasks,
        TaskMasterUpdateExTasks,
        PlantBomb,
        ReleaseBomb,
        BomberKill,
        EvilHackerCreatesMadmate,
        TrapperKill,
        PlaceTrap,
        ClearTrap,
        ActivateTrap,
        DisableTrap,
        TrapperMeetingFlag,
        SetBrainwash,
        MoriartyKill,
        AkujoSetHonmei,
        AkujoSetKeep,
        AkujoSuicide,
        PlagueDoctorWin,
        PlagueDoctorSetInfected,
        PlagueDoctorUpdateProgress,
        SetOddIsJekyll,
        TeleporterTeleport,
        SetCupidLovers,
        CupidSuicide,
        SetCupidShield,
        BlackmailPlayer,
        UnblackmailPlayer,
        ProphetExamine,
        ShareRealTasks,
        PlaceAccel,
        ActivateAccel,
        DeactivateAccel,
        PlaceDecel,
        ActivateDecel,
        UntriggerDecel,
        DeactivateDecel,
        LawyerWin,
        FoxStealth,
        FoxCreatesImmoralist,
        BuskerPseudocide,
        BuskerRevive,
        UnlockMayorAcCommon,
        UnlockDetectiveAcChallenge,
        UnlockMedicAcChallenge,
        UnlockTrackerAcChallenge,
        UnlockVeteranAcChallenge,
        UnlockTaskMasterAcChallenge,
        UnlockJesterAcCommon,
        ResetAchievement,
        RecordStatistics,
        SetRoleHistory,
        NoisemakerSetSounded,
        SchrodingersCatSetTeam,
        KataomoiSetTarget,
        KataomoiWin,
        KataomoiStalking,
        YoyoMarkLocation,
        YoyoBlink,
        BreakArmor,
        PlaceAntique,
        ArchaeologistDetect,
        ArchaeologistExcavate
    }

    public static class RPCProcedure {

        // Main Controls

        public static void resetVariables() {
            Garlic.clearGarlics();
            JackInTheBox.clearJackInTheBoxes();
            AssassinTrace.clearTraces();
            Portal.clearPortals();
            Bloodytrail.resetSprites();
            SpecimenVital.clearAndReload();
            AdditionalVents.clearAndReload();
            BombEffect.clearBombEffects();
            MapBehaviourPatch.reset();
            MapBehaviourPatch.resetRealTasks();
            MapBehaviourPatch2.ResetIcons();
            SpawnInMinigamePatch.reset();
            MeetingOverlayHolder.clearAndReload();
            Props.clearAndReload();
            Silhouette.clearSilhouettes();
            //Trap.clearTraps();
            Trap.clearAllTraps();
            CustomNormalPlayerTask.reset();
            Shrine.reset();
            RolloverMessage.Initialize();
            Antique.clearAllAntiques();
            clearAndReloadMapOptions();
            clearAndReloadRoles();
            clearGameHistory();
            setCustomButtonCooldowns();
            reloadPluginOptions();
            CustomOverlay.resetOverlays();
            Helpers.toggleZoom(reset : true);
            GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
            SurveillanceMinigamePatch.nightVisionOverlays = null;
            EventUtility.clearAndReload();
        }

    public static void HandleShareOptions(byte numberOfOptions, MessageReader reader) {            
            try {
                for (int i = 0; i < numberOfOptions; i++) {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption option = CustomOption.options.First(option => option.id == (int)optionId);
                    option.updateSelection((int)selection, i == numberOfOptions - 1);
                }
            } catch (Exception e) {
                TheOtherRolesPlugin.Logger.LogError("Error while deserializing options: " + e.Message);
            }
        }

        public static void forceEnd() {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Role.IsImpostor)
                {
                    
                    GameData.Instance.GetPlayerById(player.PlayerId); // player.RemoveInfected(); (was removed in 2022.12.08, no idea if we ever need that part again, replaced by these 2 lines.) 
                    player.CoSetRole(RoleTypes.Crewmate, true);

                    player.MurderPlayer(player, MurderResultFlags.Succeeded);
                    player.Data.IsDead = true;
                }
            }
        }

        public static void shareGamemode(byte gm) {
            TORMapOptions.gameMode = (CustomGamemodes) gm;
            LobbyViewSettingsPatch.currentButtons?.ForEach(x => x.gameObject?.Destroy());
            LobbyViewSettingsPatch.currentButtons?.Clear();
            LobbyViewSettingsPatch.currentButtonTypes?.Clear();
        }

        public static void stopStart(byte playerId)
        {
            if (AmongUsClient.Instance.AmHost && CustomOptionHolder.anyPlayerCanStopStart.getBool())
            {
                GameStartManager.Instance.ResetStartState();
                PlayerControl.LocalPlayer.RpcSendChat(string.Format(ModTranslation.getString("playerStopGameStartText"), Helpers.playerById(playerId).Data.PlayerName));
            }
        }

        public static void workaroundSetRoles(byte numberOfRoles, MessageReader reader)
        {
                for (int i = 0; i < numberOfRoles; i++)
                {                   
                    byte playerId = (byte) reader.ReadPackedUInt32();
                    byte roleId = (byte) reader.ReadPackedUInt32();
                    try {
                        setRole(roleId, playerId);
                    } catch (Exception e) {
                        TheOtherRolesPlugin.Logger.LogError("Error while deserializing roles: " + e.Message);
                    }
            }
            
        }

        public static void setRole(byte roleId, byte playerId) {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == playerId) {
                    switch((RoleId)roleId) {
                    case RoleId.Jester:
                        Jester.jester = player;
                        break;
                    case RoleId.Mayor:
                        Mayor.mayor = player;
                        break;
                    case RoleId.Portalmaker:
                        Portalmaker.portalmaker = player;
                        break;
                    case RoleId.Engineer:
                        Engineer.engineer = player;
                        break;
                    case RoleId.Sheriff:
                        Sheriff.sheriff = player;
                        break;
                    case RoleId.Deputy:
                        Deputy.deputy = player;
                        break;
                    case RoleId.Lighter:
                        Lighter.lighter = player;
                        break;
                    case RoleId.Godfather:
                        Godfather.godfather = player;
                        break;
                    case RoleId.Mafioso:
                        Mafioso.mafioso = player;
                        break;
                    case RoleId.Janitor:
                        Janitor.janitor = player;
                        break;
                    case RoleId.Detective:
                        Detective.detective = player;
                        break;
                    case RoleId.TimeMaster:
                        TimeMaster.timeMaster = player;
                        break;
                    case RoleId.Medic:
                        Medic.medic = player;
                        break;
                    case RoleId.Shifter:
                        Shifter.shifter = player;
                        break;
                    case RoleId.EvilWatcher:
                        Watcher.evilwatcher = player;
                        break;
                    case RoleId.NiceWatcher:
                        Watcher.nicewatcher = player;
                        break;
                    case RoleId.Akujo:
                        Akujo.akujo = player;
                        break;
                    case RoleId.Kataomoi:
                        Kataomoi.kataomoi = player;
                        break;
                    case RoleId.Swapper:
                        Swapper.swapper = player;
                        break;
                    case RoleId.Seer:
                        Seer.seer = player;
                        break;
                    case RoleId.Archaeologist:
                        Archaeologist.archaeologist = player;
                        break;
                    case RoleId.Morphling:
                        Morphling.morphling = player;
                        break;
                    case RoleId.Camouflager:
                        Camouflager.camouflager = player;
                        break;
                    case RoleId.Moriarty:
                        Moriarty.moriarty = player;
                        break;
                    case RoleId.JekyllAndHyde:
                        JekyllAndHyde.jekyllAndHyde = player;
                        break;
                    case RoleId.Noisemaker:
                        Noisemaker.noisemaker = player;
                        break;
                    case RoleId.Hacker:
                        Hacker.hacker = player;
                        break;
                    case RoleId.Tracker:
                        Tracker.tracker = player;
                        break;
                    case RoleId.Vampire:
                        Vampire.vampire = player;
                        break;
                    case RoleId.Snitch:
                        Snitch.snitch = player;
                        break;
                    case RoleId.FortuneTeller:
                        FortuneTeller.fortuneTeller = player;
                        break;
                    case RoleId.PlagueDoctor:
                        PlagueDoctor.plagueDoctor = player;
                        break;
                    case RoleId.SchrodingersCat:
                        SchrodingersCat.schrodingersCat = player;
                        break;
                    case RoleId.TaskMaster:
                        TaskMaster.taskMaster = player;
                        break;
                    case RoleId.Busker:
                        Busker.busker = player;
                        break;
                    case RoleId.Yasuna:
                        Yasuna.yasuna = player;
                        break;
                    case RoleId.EvilYasuna:
                        Yasuna.yasuna = player;
                        break;
                    case RoleId.Sprinter:
                        Sprinter.sprinter = player;
                        break;
                    case RoleId.EvilTracker:
                        EvilTracker.evilTracker = player;
                        break;
                    case RoleId.EvilHacker:
                        EvilHacker.evilHacker = player;
                        break;
                    case RoleId.Trapper:
                        Trapper.trapper = player;
                        break;
                    case RoleId.Jackal:
                        Jackal.jackal = player;
                        break;
                    case RoleId.Sidekick:
                        Sidekick.sidekick = player;
                        break;
                    case RoleId.Eraser:
                        Eraser.eraser = player;
                        break;
                    case RoleId.Yoyo:
                        Yoyo.yoyo = player;
                        break;
                    case RoleId.Spy:
                        Spy.spy = player;
                        break;
                    case RoleId.Teleporter:
                        Teleporter.teleporter = player;
                        break;
                    case RoleId.Trickster:
                        Trickster.trickster = player;
                        break;
                    case RoleId.Cleaner:
                        Cleaner.cleaner = player;
                        break;
                    case RoleId.Warlock:
                        Warlock.warlock = player;
                        break;
                    case RoleId.Bait:
                        Bait.bait = player;
                        break;
                    case RoleId.Veteran:
                        Veteran.veteran = player;
                        break;
                    case RoleId.Sherlock:
                        Sherlock.sherlock = player;
                        break;
                    case RoleId.Blackmailer:
                        Blackmailer.blackmailer = player;
                        break;
                    case RoleId.Prophet:
                        Prophet.prophet = player;
                        break;
                    case RoleId.SecurityGuard:
                        SecurityGuard.securityGuard = player;
                        break;
                    case RoleId.Arsonist:
                        Arsonist.arsonist = player;
                        break;
                    case RoleId.EvilGuesser:
                        Guesser.evilGuesser = player;
                        break;
                    case RoleId.NiceGuesser:
                        Guesser.niceGuesser = player;
                        break;
                    case RoleId.BountyHunter:
                        BountyHunter.bountyHunter = player;
                        break;
                    case RoleId.Vulture:
                        Vulture.vulture = player;
                        break;
                    case RoleId.Fox:
                        Fox.fox = player;
                        break;
                    case RoleId.Immoralist:
                        Immoralist.immoralist = player;
                        break;
                    case RoleId.Medium:
                        Medium.medium = player;
                        break;
                    //case RoleId.Trapper:
                        //Trapper.trapper = player;
                        //break;
                    case RoleId.Lawyer:
                        Lawyer.lawyer = player;
                        break;
                    //case RoleId.Prosecutor:
                        //Lawyer.lawyer = player;
                        //Lawyer.isProsecutor = true;
                        //break;
                    case RoleId.Pursuer:
                        Pursuer.pursuer = player;
                        break;
                    case RoleId.Witch:
                        Witch.witch = player;
                        break;
                    case RoleId.Assassin:
                        Assassin.assassin = player;
                        break;
                    case RoleId.Ninja:
                        Ninja.ninja = player;
                        break;
                    case RoleId.Opportunist:
                        Opportunist.opportunist = player;
                        break;
                    case RoleId.NekoKabocha:
                        NekoKabocha.nekoKabocha = player;
                        break;
                    case RoleId.SerialKiller:
                        SerialKiller.serialKiller = player;
                        break;
                    case RoleId.Undertaker:
                        Undertaker.undertaker = player;
                        break;
                    case RoleId.MimicK:
                        MimicK.mimicK = player;
                        break;
                    case RoleId.MimicA:
                        MimicA.mimicA = player;
                        break;
                    case RoleId.BomberA:
                        BomberA.bomberA = player;
                        break;
                    case RoleId.BomberB:
                        BomberB.bomberB = player;
                        break;
                    case RoleId.Cupid:
                        Cupid.cupid = player;
                        break;
                    case RoleId.Thief:
                        Thief.thief = player;
                        break;
                        //case RoleId.Bomber:
                        //Bomber.bomber = player;
                        //break;
                    }
        }
        }

        public static void setModifier(byte modifierId, byte playerId, byte flag) {
            PlayerControl player = Helpers.playerById(playerId); 
            switch ((RoleId)modifierId) {
                //case RoleId.Bait:
                    //Bait.bait.Add(player);
                    //break;
                case RoleId.Lover:
                    if (flag == 0) Lovers.lover1 = player;
                    else Lovers.lover2 = player;
                    break;
                case RoleId.Bloody:
                    Bloody.bloody.Add(player);
                    break;
                case RoleId.AntiTeleport:
                    AntiTeleport.antiTeleport.Add(player);
                    break;
                case RoleId.Tiebreaker:
                    Tiebreaker.tiebreaker = player;
                    break;
                case RoleId.Sunglasses:
                    Sunglasses.sunglasses.Add(player);
                    break;
                case RoleId.Mini:
                    Mini.mini = player;
                    break;
                case RoleId.Vip:
                    Vip.vip.Add(player);
                    break;
                case RoleId.Invert:
                    Invert.invert.Add(player);
                    break;
                case RoleId.Chameleon:
                    Chameleon.chameleon.Add(player);
                    break;
                case RoleId.Armored:
                    Armored.armored = player;
                    break;
                case RoleId.Madmate:
                    Madmate.madmate.Add(player);
                    break;
                //case RoleId.Shifter:
                    //Shifter.shifter = player;
                    //break;
            }
        }

        public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId) {
            System.Version ver;
            if (revision < 0) 
                ver = new System.Version(major, minor, build);
            else 
                ver = new System.Version(major, minor, build, revision);
            GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static void useUncheckedVent(int ventId, byte playerId, byte isEnter) {
            PlayerControl player = Helpers.playerById(playerId);
            if (player == null) return;
            // Fill dummy MessageReader and call MyPhysics.HandleRpc as the corountines cannot be accessed
            MessageReader reader = new();
            byte[] bytes = BitConverter.GetBytes(ventId);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            reader.Buffer = bytes;
            reader.Length = bytes.Length;

            JackInTheBox.startAnimation(ventId);
            player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
        }

        public static void uncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation) {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            PlayerControl source = Helpers.playerById(sourceId);
            PlayerControl target = Helpers.playerById(targetId);
            if (source != null && target != null) {
                if (showAnimation == 0) KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                source.MurderPlayer(target, MurderResultFlags.Succeeded);
            }
        }

        public static void uncheckedCmdReportDeadBody(byte sourceId, byte targetId) {
            PlayerControl source = Helpers.playerById(sourceId);
            var t = targetId == Byte.MaxValue ? null : Helpers.playerById(targetId).Data;
            if (source != null) source.ReportDeadBody(t);
        }

        public static void uncheckedExilePlayer(byte targetId) {
            PlayerControl target = Helpers.playerById(targetId);
            if (target != null) {
                target.Exiled();
                ExileControllerBeginPatch.extraVictim = true;
            }
        }

        public static void uncheckedSetTasks(byte playerId, byte[] taskTypeIds)
        {
            var player = Helpers.playerById(playerId);
            player.clearAllTasks();

            player.Data.SetTasks(taskTypeIds);
        }

        public static void dynamicMapOption(byte mapId) {
           GameOptionsManager.Instance.currentNormalGameOptions.MapId = mapId;
        }

        public static void finishShipStatusBegin()
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    if (Madmate.hasTasks && Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
                    {
                        PlayerControl.LocalPlayer.clearAllTasks();
                        PlayerControl.LocalPlayer.generateAndAssignTasks(Madmate.commonTasks, Madmate.shortTasks, Madmate.longTasks);
                    }
                    if (JekyllAndHyde.jekyllAndHyde != null && PlayerControl.LocalPlayer == JekyllAndHyde.jekyllAndHyde)
                    {
                        PlayerControl.LocalPlayer.clearAllTasks();
                        PlayerControl.LocalPlayer.generateAndAssignTasks(JekyllAndHyde.numCommonTasks, JekyllAndHyde.numShortTasks, JekyllAndHyde.numLongTasks);
                        JekyllAndHyde.oddIsJekyll = rnd.Next(0, 2) == 1;
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetOddIsJekyll, Hazel.SendOption.Reliable, -1);
                        writer.Write(JekyllAndHyde.oddIsJekyll);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        jekyllAndHydeSuicideButton.Timer = JekyllAndHyde.suicideTimer;
                    }
                }
            })));
        }

        public static void setGameStarting() {
            GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 5f;
        }

        // Role functionality

        public static void engineerFixLights() {
            SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
        }

        public static void engineerFixSubmergedOxygen() {
            SubmergedCompatibility.RepairOxygen();
        }

        public static void engineerUsedRepair() {
            Engineer.remainingFixes--;
            if (Helpers.shouldShowGhostInfo()) {
                Helpers.showFlash(Engineer.color, 0.5f, ModTranslation.getString("engineerInfo")); ;
            }
        }

        public static void cleanBody(byte playerId, byte cleaningPlayerId) {
            if (Medium.futureDeadBodies != null && !(Busker.busker == Helpers.playerById(playerId) && !Busker.pseudocideComplete)) {
                var deadBody = Medium.futureDeadBodies.Find(x => x.Item1.player.PlayerId == playerId).Item1;
                if (deadBody != null) deadBody.wasCleaned = true;
            }

            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++) {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId) {
                    UnityEngine.Object.Destroy(array[i].gameObject);
                }     
            }
            GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.CleanBody, cleaningPlayerId, 1 << playerId)
            {
                RelatedTag =
                Vulture.vulture != null && cleaningPlayerId == Vulture.vulture.PlayerId ? EventDetail.Eat : EventDetail.Clean
            });
            if (Vulture.vulture != null && cleaningPlayerId == Vulture.vulture.PlayerId) {
                Vulture.eatenBodies++;
                if (Vulture.eatenBodies == Vulture.vultureNumberToWin) {
                    Vulture.triggerVultureWin = true;
                }
            }
        }

        public static void timeMasterRewindTime() {
            TimeMaster.shieldActive = false; // Shield is no longer active when rewinding
            SoundEffectsManager.stop("timemasterShield");  // Shield sound stopped when rewinding
            if(TimeMaster.timeMaster != null && TimeMaster.timeMaster == PlayerControl.LocalPlayer) {
                resetTimeMasterButton();
                _ = new StaticAchievementToken("timeMaster.challenge");
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (TimeMaster.timeMaster == null || PlayerControl.LocalPlayer == TimeMaster.timeMaster) return; // Time Master himself does not rewind

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            PlayerControl.LocalPlayer.moveable = false;
        }

        public static void timeMasterShield() {
            TimeMaster.shieldActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration, new Action<float>((p) => {
                if (p == 1f) TimeMaster.shieldActive = false;
            })));
        }

        public static void medicSetShielded(byte shieldedId) {
            Medic.usedShield = true;
            Medic.shielded = Helpers.playerById(shieldedId);
            Medic.futureShielded = null;
        }

        public static void shieldedMurderAttempt() {
            if (Medic.shielded == null || Medic.medic == null) return;
            
            bool isShieldedAndShow = Medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded;
            isShieldedAndShow = isShieldedAndShow && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting);  // Dont show attempt, if shield is not shown yet
            bool isMedicAndShow = Medic.medic == PlayerControl.LocalPlayer && Medic.showAttemptToMedic;

            if (isShieldedAndShow || isMedicAndShow || Helpers.shouldShowGhostInfo()) Helpers.showFlash(Palette.ImpostorRed, duration: 0.5f, ModTranslation.getString("medicInfo"));
        }

        public static void shifterShift(byte targetId)
        {
            PlayerControl oldShifter = Shifter.shifter;
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            if (!Shifter.isNeutral) Shifter.clearAndReload();
            if (PlayerControl.LocalPlayer == oldShifter && Shifter.isNeutral) _ = new StaticAchievementToken("corruptedShifter.common1");

            // Suicide (exile) when impostor or impostor variants
            if (!Shifter.isNeutral && (player.Data.Role.IsImpostor || Helpers.isNeutral(player) || Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) || player == CreatedMadmate.createdMadmate))
            {
                GameStatistics.recordRoleHistory(oldShifter);   
                if (!oldShifter.Data.IsDead)
                {
                    oldShifter.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
                    ExileControllerBeginPatch.extraVictim = true;
                }
                if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }
                if (PlayerControl.LocalPlayer == oldShifter)
                {
                    Shifter.niceShifterAcTokenChallenge.Value.oldShifterId = oldShifter.PlayerId;
                    Shifter.niceShifterAcTokenChallenge.Value.shiftId = targetId;
                }
                return;
            }

            if (!Shifter.isNeutral && PlayerControl.LocalPlayer == oldShifter)
                _ = new StaticAchievementToken("niceShifter.common1");
            bool isHusk = false;

            // Switch shield
            if (Medic.shielded != null && Medic.shielded == player)
            {
                Medic.shielded = oldShifter;
            }
            else if (Medic.shielded != null && Medic.shielded == oldShifter)
            {
                Medic.shielded = player;
            }

            if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId))
            {
                Madmate.madmate.Add(oldShifter);
                Madmate.madmate.Remove(player);
            }

            if (Shifter.shiftModifiers)
            {                
                // Switch Lovers
                if (Lovers.lover1 != null && oldShifter == Lovers.lover1) Lovers.lover1 = player;
                else if (Lovers.lover1 != null && player == Lovers.lover1) Lovers.lover1 = oldShifter;

                if (Lovers.lover2 != null && oldShifter == Lovers.lover2) Lovers.lover2 = player;
                else if (Lovers.lover2 != null && player == Lovers.lover2) Lovers.lover2 = oldShifter;

                if (Cupid.lovers1 != null && oldShifter == Cupid.lovers1) Cupid.lovers1 = player;
                else if (Cupid.lovers1 != null && player ==  Cupid.lovers1) Cupid.lovers1 = oldShifter;

                if (Cupid.lovers2 != null && oldShifter == Cupid.lovers2) Cupid.lovers2 = player;
                else if (Cupid.lovers2 != null && player == Cupid.lovers2) Cupid.lovers2 = oldShifter;

                // Switch Anti-Teleport
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == player.PlayerId))
                {
                    AntiTeleport.antiTeleport.Add(oldShifter);
                    AntiTeleport.antiTeleport.Remove(player);
                }
                // Switch Bloody
                if (Bloody.bloody.Any(x => x.PlayerId == player.PlayerId))
                {
                    Bloody.bloody.Add(oldShifter);
                    Bloody.bloody.Remove(player);
                }
                // Switch Mini
                if (Mini.mini == player) Mini.mini = oldShifter;
                // Switch Tiebreaker
                if (Tiebreaker.tiebreaker == player) Tiebreaker.tiebreaker = oldShifter;
                // Switch Chameleon
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId))
                {
                    Chameleon.chameleon.Add(oldShifter);
                    Chameleon.chameleon.Remove(player);
                    Chameleon.removeChameleonFully(player);
                }
                // Switch Sunglasses
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == player.PlayerId))
                {
                    Sunglasses.sunglasses.Add(oldShifter);
                    Sunglasses.sunglasses.Remove(player);
                }
                if (Vip.vip.Any(x => x.PlayerId == player.PlayerId))
                {
                    Vip.vip.Add(oldShifter);
                    Vip.vip.Remove(player);
                }
                if (Invert.invert.Any(x => x.PlayerId == player.PlayerId))
                {
                    Invert.invert.Add(oldShifter);
                    Invert.invert.Remove(player);
                }
                if (Armored.armored == player) Armored.armored = oldShifter;
            }

            // Specify shifting onto Lawyer/Akujo
            if (player == Lawyer.lawyer && Lawyer.target != null) {
                Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo != null) playerInfo.text = "";
            } else if (player == Akujo.akujo) {
                if (Akujo.honmei != null) {
                    Transform playerInfoTransform = Akujo.honmei.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                    if (playerInfo != null) playerInfo.text = "";
                } if (Akujo.keeps != null) {
                    foreach (PlayerControl playerControl in Akujo.keeps) {
                        Transform playerInfoTransform = playerControl.cosmetics.nameText.transform.parent.FindChild("Info");
                        TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                        if (playerInfo != null) playerInfo.text = "";
                    }
                }
            }

            // Shift role
            if (Mayor.mayor != null && Mayor.mayor == player)
            {
                Mayor.mayor = oldShifter;
                Mayor.onAchievementActivate();
            }
            if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player)
            {
                Portalmaker.portalmaker = oldShifter;
                Portalmaker.onAchievementActivate();
            }
            if (Engineer.engineer != null && Engineer.engineer == player)
            {
                Engineer.engineer = oldShifter;
                Engineer.onAchievementActivate();
            }
            if (Sheriff.sheriff != null && Sheriff.sheriff == player)
            {
                if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff) Sheriff.formerDeputy = oldShifter;  // Shifter also shifts info on promoted deputy (to get handcuffs)
                Sheriff.sheriff = oldShifter;
                Sheriff.onAchievementActivate();
            }
            if (Sheriff.formerSheriff != null && Sheriff.formerSheriff == player)
            {
                Sheriff.formerSheriff = oldShifter;
                Sheriff.onAchievementActivate();
                isHusk = true;
            }
            if (Deputy.deputy != null && Deputy.deputy == player)
                Deputy.deputy = oldShifter;
            if (Lighter.lighter != null && Lighter.lighter == player)
                Lighter.lighter = oldShifter;
            if (Detective.detective != null && Detective.detective == player)
            {
                Detective.detective = oldShifter;
                Detective.onAchievementActivate();
            }
            if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player)
                TimeMaster.timeMaster = oldShifter;
            if (Medic.medic != null && Medic.medic == player)
            {
                Medic.medic = oldShifter;
                Medic.onAchievementActivate();
            }
            if (Swapper.swapper != null && Swapper.swapper == player)
            {
                Swapper.swapper = oldShifter;
                Swapper.niceSwapperOnAchievementActivate();
                Swapper.evilSwapperOnAchievementActivate();
            }
            if (Seer.seer != null && Seer.seer == player)
            {
                Seer.seer = oldShifter;
                Seer.onAchievementActivate();
            }
            if (Hacker.hacker != null && Hacker.hacker == player)
                Hacker.hacker = oldShifter;
            if (Tracker.tracker != null && Tracker.tracker == player)
            {
                Tracker.tracker = oldShifter;
                Tracker.onAchievementActivate();
            }
            if (Snitch.snitch != null && Snitch.snitch == player) Snitch.snitch = oldShifter;
            if (Spy.spy != null && Spy.spy == player)
                Spy.spy = oldShifter;
            if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player)
            {
                SecurityGuard.securityGuard = oldShifter;
                SecurityGuard.onAchievementActivate();
            }
            if (Guesser.niceGuesser != null && Guesser.niceGuesser == player)
            {
                Guesser.niceGuesser = oldShifter;
                Guesser.niceGuesserOnAchievementActivate();
            }
            if (Bait.bait != null && Bait.bait == player)
            {
                Bait.bait = oldShifter;
                if (Bait.bait.Data.IsDead) Bait.reported = true;
                Bait.onAchievementActivate();
            }
            if (Medium.medium != null && Medium.medium == player)
            {
                Medium.medium = oldShifter;
                Medium.onAchievementActivate();
            }
            if (Watcher.nicewatcher != null && Watcher.nicewatcher == player)
                Watcher.nicewatcher = oldShifter;
            if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == player) {
                if (PlayerControl.LocalPlayer == player) resetPoolables();
                FortuneTeller.fortuneTeller = oldShifter;
                FortuneTeller.onAchievementActivate();
            }
            if (Sherlock.sherlock != null && Sherlock.sherlock == player) {
                Sherlock.sherlock = oldShifter;
                Sherlock.onAchievementActivate();
            }
            if (Sprinter.sprinter != null && Sprinter.sprinter == player) {
                Sprinter.sprinter = oldShifter;
                Sprinter.onAchievementActivate();
            }
            if (Veteran.veteran != null && Veteran.veteran == player)
                Veteran.veteran = oldShifter;
            if (Yasuna.yasuna != null && Yasuna.yasuna == player) {
                Yasuna.yasuna = oldShifter;
                Yasuna.yasunaOnAchievementActivate();
                Yasuna.evilYasunaOnAcheivementActivate();
            }
            if (player == TaskMaster.taskMaster)
                TaskMaster.taskMaster = oldShifter;
            if (Teleporter.teleporter != null && Teleporter.teleporter == player) {
                Teleporter.teleporter = oldShifter;
                Teleporter.onAchievementActivate();
            }
            if (Prophet.prophet != null && Prophet.prophet == player) {
                Prophet.prophet = oldShifter;
                Prophet.onAchievementActivate();
            }
            if (Busker.busker != null && Busker.busker == player) {
                Busker.busker = oldShifter;
                Busker.onAchievementActivate();
            }
            if (Noisemaker.noisemaker != null && Noisemaker.noisemaker == player) {
                Noisemaker.noisemaker = oldShifter;
                Noisemaker.onAchievementActivate();
            }
            if (Archaeologist.archaeologist != null && Archaeologist.archaeologist == player) Archaeologist.archaeologist = player;

            if (player == Godfather.godfather) Godfather.godfather = oldShifter;
            if (player == Mafioso.mafioso) Mafioso.mafioso = oldShifter;
            if (player == Janitor.janitor) Janitor.janitor = oldShifter;
            if (player == Morphling.morphling) {
                Morphling.morphling = oldShifter;
                Morphling.onAchievementActivate();
            }
            if (player == Trickster.trickster) {
                Trickster.trickster = oldShifter;
                Trickster.onAchievementActivate();
            }
            if (player == Cleaner.cleaner) {
                Cleaner.cleaner = oldShifter;
                Cleaner.onAchievementActivate();
            }
            if (player == Ninja.ninja) {
                Ninja.ninja = oldShifter;
                Ninja.onAchievementActivate();
            }
            if (player == NekoKabocha.nekoKabocha) NekoKabocha.nekoKabocha = oldShifter;
            if (player == Assassin.assassin) {
                Assassin.assassin = oldShifter;
                Assassin.onAchievementActivate();
            }
            if (player == SerialKiller.serialKiller) SerialKiller.serialKiller = oldShifter;
            if (player == EvilTracker.evilTracker) {
                EvilTracker.clearAllArrow();
                EvilTracker.evilTracker = oldShifter;
            }
            if (player == EvilHacker.evilHacker) {
                EvilHacker.evilHacker = oldShifter;
                EvilHacker.onAchievementActivate();
            }
            if (player == Witch.witch) {
                Witch.witch = oldShifter;
                Witch.onAchievementActivate();
            }
            if (player == Camouflager.camouflager) {
                Camouflager.camouflager = oldShifter;
                Camouflager.onAchievementActivate();
            }
            if (player == Guesser.evilGuesser) {
                Guesser.evilGuesser = oldShifter;
                Guesser.evilGuesserOnAchievementActivate();
            }
            if (player == Eraser.eraser) {
                Eraser.eraser = oldShifter;
                Eraser.onAchievementActivate();
            }
            if (player == Warlock.warlock) {
                Warlock.warlock = oldShifter;
                Warlock.onAchievementActivate();
            }
            if (player == BountyHunter.bountyHunter) {
                BountyHunter.clearAllArrow();
                BountyHunter.bountyHunter = oldShifter;
                BountyHunter.onAchievementActivate();
            }
            if (player == Vampire.vampire) {
                Vampire.vampire = oldShifter;
                Vampire.onAchievementActivate();
            }
            if (player == CreatedMadmate.createdMadmate) CreatedMadmate.createdMadmate = oldShifter;
            if (player == Blackmailer.blackmailer) {
                Blackmailer.blackmailer = oldShifter;
                Blackmailer.onAchievementActivate();
            }
            if (player == MimicK.mimicK) {
                MimicK.mimicK = oldShifter;
                MimicK.onAchievementActivate();
            }
            if (player == MimicA.mimicA) {
                MimicA.mimicA = oldShifter;
                MimicA.onAchievementActivate();
            }
            if (player == BomberA.bomberA) {
                if (PlayerControl.LocalPlayer == player) {
                    resetPoolables();
                    if (BomberA.arrows.FirstOrDefault().arrow != null) BomberA.arrows.FirstOrDefault().arrow.SetActive(false);
                }
                BomberA.bomberA = oldShifter;
            }
            if (player == BomberB.bomberB) {
                if (PlayerControl.LocalPlayer == player) {
                    resetPoolables();
                    if (BomberB.arrows.FirstOrDefault().arrow != null) BomberB.arrows.FirstOrDefault().arrow.SetActive(false);
                }
                BomberB.bomberB = oldShifter;
            }
            if (player == Trapper.trapper) {
                Trapper.trapper = oldShifter;
                Trapper.onAchievementActivate();
            }
            if (player == Yoyo.yoyo) {
                Yoyo.yoyo = oldShifter;
                Yoyo.markedLocation = null;
            }
            if (player == Jester.jester) Jester.jester = oldShifter;
            if (player == Arsonist.arsonist) {
                if (PlayerControl.LocalPlayer == player) resetPoolables();
                Arsonist.arsonist = oldShifter;
            }
            if (player == Kataomoi.kataomoi) {
                Kataomoi.resetAllArrow();
                Kataomoi.kataomoi = oldShifter;
            }
            if (player == Opportunist.opportunist) Opportunist.opportunist = oldShifter;
            if (player == Moriarty.moriarty) Moriarty.moriarty = oldShifter;
            if (player == PlagueDoctor.plagueDoctor) PlagueDoctor.plagueDoctor = oldShifter;
            if (player == Thief.thief) Thief.thief = oldShifter;
            if (player == Pursuer.pursuer) Pursuer.pursuer = oldShifter;
            if (player == Vulture.vulture) Vulture.vulture = oldShifter;
            if (player == Jackal.jackal) Jackal.jackal = oldShifter;
            if (player == Sidekick.sidekick) Sidekick.sidekick = oldShifter;
            if (Jackal.formerJackals.Contains(player)) {
                Jackal.formerJackals.Add(oldShifter);
                Jackal.formerJackals.RemoveAll(x => x.PlayerId == targetId);
                isHusk = true;
            }
            if (player == Lawyer.lawyer) Lawyer.lawyer = oldShifter;
            if (player == Fox.fox) Fox.fox = oldShifter;
            if (player == Immoralist.immoralist) Immoralist.immoralist = oldShifter;
            if (player == Akujo.akujo) Akujo.akujo = oldShifter;
            if (player == Cupid.cupid) Cupid.cupid = oldShifter;
            if (player == JekyllAndHyde.jekyllAndHyde) JekyllAndHyde.jekyllAndHyde = oldShifter;
            if (player == Moriarty.formerMoriarty) {
                Moriarty.formerMoriarty = oldShifter;
                isHusk = true;
            }
            if (player == JekyllAndHyde.formerJekyllAndHyde) {
                JekyllAndHyde.formerJekyllAndHyde = oldShifter;
                isHusk = true;
            }
            if (player == SchrodingersCat.schrodingersCat) SchrodingersCat.schrodingersCat = oldShifter;
            if (player == SchrodingersCat.formerSchrodingersCat) {
                SchrodingersCat.formerSchrodingersCat = oldShifter;
                isHusk = true;
            }
            if (Husk.husk.Any(x => x.PlayerId == player.PlayerId))
            {
                Husk.husk.Add(oldShifter);
                Husk.husk.RemoveAll(x => x.PlayerId == player.PlayerId);
            }

            if (Lawyer.lawyer != null && Lawyer.target == player) Lawyer.target = oldShifter;
            if (Kataomoi.kataomoi != null && Kataomoi.target == player)
                Kataomoi.target = oldShifter;

            if (Shifter.isNeutral)
            {
                Shifter.shifter = player;
                Shifter.pastShifters.Add(oldShifter.PlayerId);
                if (player.Data.Role.IsImpostor)
                {
                    player.FastSetRole(RoleTypes.Crewmate);
                    oldShifter.FastSetRole(RoleTypes.Impostor);
                }
            }

            if (isHusk && !oldShifter.Data.IsDead) Husk.husk.Add(oldShifter);

            // Set cooldowns to max for both players
            if (PlayerControl.LocalPlayer == oldShifter || PlayerControl.LocalPlayer == player)
                CustomButton.ResetAllCooldowns();

            GameStatistics.recordRoleHistory(oldShifter);
            GameStatistics.recordRoleHistory(player);
        }

        /*public static void shifterShift(byte targetId) {
            PlayerControl oldShifter = Shifter.shifter;
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            Shifter.clearAndReload();

            // Suicide (exile) when impostor or impostor variants
            if (player.Data.Role.IsImpostor || Helpers.isNeutral(player)) {
                oldShifter.Exiled();
                GameHistory.overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
                if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }
                return;
            }
            
            Shifter.shiftRole(oldShifter, player);

            // Set cooldowns to max for both players
            if (PlayerControl.LocalPlayer == oldShifter || PlayerControl.LocalPlayer == player)
                CustomButton.ResetAllCooldowns();
        }*/

        public static void swapperSwap(byte playerId1, byte playerId2) {
            if (MeetingHud.Instance) {
                Swapper.playerId1 = playerId1;
                Swapper.playerId2 = playerId2;
            }
        }

        public static void morphlingMorph(byte playerId) {  
            PlayerControl target = Helpers.playerById(playerId);
            if (Morphling.morphling == null || target == null) return;

            Morphling.morphTimer = Morphling.duration;
            Morphling.morphTarget = target;
            if (Camouflager.camouflageTimer <= 0f)
                Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void camouflagerCamouflage() {
            if (Camouflager.camouflager == null) return;

            Camouflager.camouflageTimer = Camouflager.duration;
            if (Helpers.MushroomSabotageActive()) return; // Dont overwrite the fungle "camo"
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                player.setLook("", 6, "", "", "", "");
        }

        public static void vampireSetBitten(byte targetId, byte performReset) {
            if (performReset != 0) {
                Vampire.bitten = null;
                return;
            }

            if (Vampire.vampire == null) return;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                if (player.PlayerId == targetId && !player.Data.IsDead) {
                        Vampire.bitten = player;
                }
            }
        }

        public static void plagueDoctorWin()
        {
            PlagueDoctor.triggerPlagueDoctorWin = true;
            var livingPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(p => p != PlagueDoctor.plagueDoctor && !p.Data.IsDead);
            foreach (PlayerControl p in livingPlayers)
            {
                if (NekoKabocha.nekoKabocha != null && p == NekoKabocha.nekoKabocha) NekoKabocha.otherKiller = PlagueDoctor.plagueDoctor;
                if (!p.Data.IsDead) p.Exiled();
                GameHistory.overrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Disease, PlagueDoctor.plagueDoctor);
            }
        }

        public static void plagueDoctorInfected(byte targetId)
        {
            var p = Helpers.playerById(targetId);
            if (!PlagueDoctor.infected.ContainsKey(targetId))
            {
                PlagueDoctor.infected[targetId] = p;
            }
        }

        public static void plagueDoctorProgress(byte targetId, float progress)
        {
            PlagueDoctor.progress[targetId] = progress;
        }

        public static void placeGarlic(byte[] buff) {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));
            new Garlic(position);
        }

        public static void trackerUsedTracker(byte targetId) {
            Tracker.usedTracker = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == targetId)
                    Tracker.tracked = player;
        }

        public static void evilHackerCreatesMadmate(byte targetId)
        {
            PlayerControl player = Helpers.playerById(targetId);

            if (player == Lawyer.lawyer && Lawyer.target != null)
            {
                Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo != null) playerInfo.text = "";
            }
            else if (player == Akujo.akujo)
            {
                if (Akujo.honmei != null)
                {
                    Transform playerInfoTransform = Akujo.honmei.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                    if (playerInfo != null) playerInfo.text = "";
                }
                if (Akujo.keeps != null)
                {
                    foreach (PlayerControl playerControl in Akujo.keeps)
                    {
                        Transform playerInfoTransform = playerControl.cosmetics.nameText.transform.parent.FindChild("Info");
                        TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                        if (playerInfo != null) playerInfo.text = "";
                    }
                }
            }

            if (!EvilHacker.canCreateMadmateFromJackal && player == Jackal.jackal)
            {
                EvilHacker.fakeMadmate = player;
            }
            else
            {
                if (PlayerControl.LocalPlayer == EvilHacker.evilHacker)
                    _ = new StaticAchievementToken("evilHacker.common1");

                // Jackalバグ対応
                List<PlayerControl> tmpFormerJackals = new(Jackal.formerJackals);

                // タスクがないプレイヤーがMadmateになった場合はショートタスクを必要数割り当てる
                if (Helpers.hasFakeTasks(player))
                {
                    if (CreatedMadmate.hasTasks)
                    {
                        Helpers.clearAllTasks(player);
                        player.generateAndAssignTasks(0, CreatedMadmate.numTasks, 0);
                    }
                }
                erasePlayerRoles(player.PlayerId, true, true);

                // Jackalバグ対応
                Jackal.formerJackals = tmpFormerJackals;

                CreatedMadmate.createdMadmate = player;
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
            }
            EvilHacker.canCreateMadmate = false;
            return;
        }

        public static void prophetExamine(byte targetId)
        {
            var target = Helpers.playerById(targetId);
            if (target == null) return;
            if (Prophet.examined.ContainsKey(target)) Prophet.examined.Remove(target);
            Prophet.examined.Add(target, Prophet.isKiller(target));
            Prophet.examinesLeft--;
            if ((Prophet.examineNum - Prophet.examinesLeft >= Prophet.examinesToBeRevealed) && Prophet.revealProphet) Prophet.isRevealed = true;
        }

        public static void shareRealTasks(MessageReader reader)
        {
            byte count = reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                byte playerId = reader.ReadByte();
                byte[] taskTmp = reader.ReadBytes(4);
                float x = System.BitConverter.ToSingle(taskTmp, 0);
                taskTmp = reader.ReadBytes(4);
                float y = System.BitConverter.ToSingle(taskTmp, 0);
                Vector2 pos = new(x, y);
                if (!MapBehaviourPatch.realTasks.ContainsKey(playerId)) MapBehaviourPatch.realTasks[playerId] = new Il2CppSystem.Collections.Generic.List<Vector2>();
                MapBehaviourPatch.realTasks[playerId].Add(pos);
            }
        }

        /// <summary>
        /// Resets all the AchievementTokenBases and reactivates the achievements <br></br>
        /// On game end the achievements will not be activated again
        /// </summary>
        public static void resetAchievement()
        {
            Achievement.allAchievementTokens = new List<AchievementTokenBase>();
            Achievement.onAchievementStart();
        }

        // Hmm... Lots of bugs huh?
        public static void fortuneTellerUsedDivine(byte fortuneTellerId, byte targetId)
        {
            PlayerControl fortuneTeller = Helpers.playerById(fortuneTellerId);
            PlayerControl target = Helpers.playerById(targetId);
            if (target == null) return;
            if (target.Data.IsDead) return;

            if ((Fox.fox != null && Fox.fox == target) || (SchrodingersCat.schrodingersCat != null && SchrodingersCat.schrodingersCat == target))
            {
                KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                fortuneTeller.MurderPlayer(target, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.Divined, fortuneTeller);
            }

            // インポスターの場合は占い師の位置に矢印を表示
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                FortuneTeller.fortuneTellerMessage(ModTranslation.getString("fortuneTellerDivinedSomeone"), 7f, Color.white);
            }
            FortuneTeller.setDivinedFlag(fortuneTeller, true);
            if (Immoralist.immoralist != null && target == Immoralist.immoralist && PlayerControl.LocalPlayer == Immoralist.immoralist)
            {
                FortuneTeller.fortuneTellerMessage(ModTranslation.getString("fortuneTellerDivinedYou"), 7f, Color.white);
            }
            FortuneTeller.divineTarget = target;

            if (PlayerControl.LocalPlayer == fortuneTeller)
            {
                if (target == Fox.fox) _ = new StaticAchievementToken("fortuneTeller.another1");
                else if (target.Data.Role.IsImpostor) FortuneTeller.acTokenImpostor.Value.divined = true;
            }
        }

        public static void placeAntique()
        {
            var dictionary = new Dictionary<Vector3, SystemTypes>();
            if (Helpers.isSkeld()) dictionary = Antique.SkeldPos;
            else if (Helpers.isMira()) dictionary = Antique.MiraPos;
            else if (Helpers.isPolus()) dictionary = Antique.PolusPos;
            else if (Helpers.isAirship()) dictionary = Antique.AirsihpPos;
            else dictionary = Antique.FunglePos;

            foreach (var p in dictionary) new Antique(p.Key, p.Value);
        }

        public static void archaeologistDetect(byte id)
        {
            if (Antique.antiques == null || Antique.antiques.Count == 0) return;
            foreach (var a in Antique.antiques) if (a.isDetected) a.isDetected = false;
            var remainingList = Antique.antiques.Where(x => !x.isBroken).ToList();
            var antique = remainingList[id];
            antique.isDetected = true;
            Archaeologist.arrowActive = true;
        }

        public static void archaeologistExcavate()
        {
            var detected = Antique.antiques.Where(x => x.isDetected).ToList();
            foreach (var antique in detected) {
                antique.isDetected = false;
                antique.isBroken = true;
                antique.spriteRenderer.sprite = Antique.getBrokenSprite();
                if (PlayerControl.LocalPlayer == Archaeologist.archaeologist) {
                    var info = Archaeologist.getRoleInfo();
                    RolloverMessage rolloverMessage = RolloverMessage.Create(antique.gameObject.transform.position, true,
                        string.Format(ModTranslation.getString("archaeologistClue"), Helpers.cs(info.color, info.name)), 5f, 0.5f, 2f, 1f, Color.white);
                    rolloverMessage.velocity = new Vector3(0f, 0.1f);
                    _ = new StaticAchievementToken("archaeologist.challenge");
                }
                if (Archaeologist.revealAntique == Archaeologist.RevealAntique.Immediately) antique.revealAntique();
            }
        }

        public static void deputyUsedHandcuffs(byte targetId)
        {
            Deputy.remainingHandcuffs--;
            Deputy.handcuffedPlayers.Add(targetId);
        }

        public static void deputyPromotes()
        {
            if (Deputy.deputy != null) {  // Deputy should never be null here, but there appeared to be a race condition during testing, which was removed.
                Sheriff.replaceCurrentSheriff(Deputy.deputy);
                if (PlayerControl.LocalPlayer == Deputy.deputy)
                {
                    _ = new StaticAchievementToken("deputy.another1");
                    Sheriff.onAchievementActivate();
                }
                Sheriff.formerDeputy = Deputy.deputy;
                Deputy.deputy = null;
                GameStatistics.recordRoleHistory(Sheriff.sheriff);
                // No clear and reload, as we need to keep the number of handcuffs left etc
            }
        }

        public static void jackalCreatesSidekick(byte targetId) {
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null) return;
            //if (Lawyer.target == player && Lawyer.isProsecutor && Lawyer.lawyer != null && !Lawyer.lawyer.Data.IsDead) Lawyer.isProsecutor = false;

            if (!Jackal.canCreateSidekickFromImpostor && player.Data.Role.IsImpostor) {
                Jackal.fakeSidekick = player;
            } else {
                bool wasSpy = Spy.spy != null && player == Spy.spy;
                bool wasImpostor = player.Data.Role.IsImpostor;  // This can only be reached if impostors can be sidekicked.
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                if (player == Lawyer.lawyer && Lawyer.target != null)
                {
                    Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
                }
                else if (player == Akujo.akujo)
                {
                    if (Akujo.honmei != null)
                    {
                        Transform playerInfoTransform = Akujo.honmei.cosmetics.nameText.transform.parent.FindChild("Info");
                        TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                        if (playerInfo != null) playerInfo.text = "";
                    }
                    if (Akujo.keeps != null)
                    {
                        foreach (PlayerControl playerControl in Akujo.keeps)
                        {
                            Transform playerInfoTransform = playerControl.cosmetics.nameText.transform.parent.FindChild("Info");
                            TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                            if (playerInfo != null) playerInfo.text = "";
                        }
                    }
                }

                erasePlayerRoles(player.PlayerId, true);
                Sidekick.sidekick = player;
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
                if (wasSpy || wasImpostor) Sidekick.wasTeamRed = true;
                Sidekick.wasSpy = wasSpy;
                Sidekick.wasImpostor = wasImpostor;
                if (player == PlayerControl.LocalPlayer) {
                    SoundEffectsManager.play("jackalSidekick");
                    _ = new StaticAchievementToken("sidekick.common1");
                    if (wasImpostor) _ = new StaticAchievementToken("sidekick.common2");
                }
                if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.getBool() && !HandleGuesser.isGuesser(targetId))
                    setGuesserGm(targetId);
            }
            Jackal.canCreateSidekick = false;
            GameStatistics.recordRoleHistory(player);
        }

        public static void sidekickPromotes() {
            if (FreePlayGM.isFreePlayGM) return;
            if (PlayerControl.LocalPlayer == Sidekick.sidekick) _ = new StaticAchievementToken("sidekick.challenge");
            Jackal.removeCurrentJackal();
            Jackal.jackal = Sidekick.sidekick;
            Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
            Jackal.wasTeamRed = Sidekick.wasTeamRed;
            Jackal.wasSpy = Sidekick.wasSpy;
            Jackal.wasImpostor = Sidekick.wasImpostor;
            Sidekick.clearAndReload();
            GameStatistics.recordRoleHistory(Jackal.jackal);
            return;
        }
        
        public static void erasePlayerRoles(byte playerId, bool ignoreModifier = true, bool isCreatedMadmate = false) {
            PlayerControl player = Helpers.playerById(playerId);
            if (player == null || (!player.canBeErased() && !isCreatedMadmate)) return;

            // Crewmate roles
            if (player == Mayor.mayor) Mayor.clearAndReload();
            if (player == Portalmaker.portalmaker) Portalmaker.clearAndReload();
            if (player == Engineer.engineer) Engineer.clearAndReload();
            if (player == Sheriff.sheriff) Sheriff.clearAndReload();
            if (player == Deputy.deputy) Deputy.clearAndReload();
            if (player == Lighter.lighter) Lighter.clearAndReload();
            if (player == Detective.detective) Detective.clearAndReload();
            if (player == TimeMaster.timeMaster) TimeMaster.clearAndReload();
            if (player == Bait.bait) Bait.clearAndReload();
            if (player == Medic.medic) Medic.clearAndReload();
            if (player == Shifter.shifter) Shifter.clearAndReload();
            if (player == Seer.seer) Seer.clearAndReload();
            if (player == Hacker.hacker) Hacker.clearAndReload();
            if (player == Tracker.tracker) Tracker.clearAndReload();
            if (player == Snitch.snitch) Snitch.clearAndReload();
            if (player == Swapper.swapper) Swapper.clearAndReload();
            if (player == Spy.spy) Spy.clearAndReload();
            if (player == SecurityGuard.securityGuard) SecurityGuard.clearAndReload();
            if (player == Medium.medium) Medium.clearAndReload();
            if (player == FortuneTeller.fortuneTeller) FortuneTeller.clearAndReload();
            if (player == Sprinter.sprinter) Sprinter.clearAndReload();
            if (player == Veteran.veteran) Veteran.clearAndReload();
            if (player == Sherlock.sherlock) Sherlock.clearAndReload();
            if (player == TaskMaster.taskMaster) TaskMaster.clearAndReload();
            if (player == CreatedMadmate.createdMadmate) CreatedMadmate.clearAndReload();
            if (player == Teleporter.teleporter) Teleporter.clearAndReload();
            if (player == Prophet.prophet) Prophet.clearAndReload();
            if (player == Busker.busker) Busker.clearAndReload();
            if (player == Noisemaker.noisemaker) Noisemaker.clearAndReload();
            if (player == Archaeologist.archaeologist) Archaeologist.clearAndReload();
            //if (player == Trapper.trapper) Trapper.clearAndReload();

            // Impostor roles
            if (player == Morphling.morphling) Morphling.clearAndReload();
            if (player == Camouflager.camouflager) Camouflager.clearAndReload();
            if (player == Godfather.godfather) Godfather.clearAndReload();
            if (player == Mafioso.mafioso) Mafioso.clearAndReload();
            if (player == Janitor.janitor) Janitor.clearAndReload();
            if (player == Vampire.vampire) Vampire.clearAndReload();
            if (player == Eraser.eraser) Eraser.clearAndReload();
            if (player == Trickster.trickster) Trickster.clearAndReload();
            if (player == Cleaner.cleaner) Cleaner.clearAndReload();
            if (player == Warlock.warlock) Warlock.clearAndReload();
            if (player == Witch.witch) Witch.clearAndReload();
            if (player == Assassin.assassin) Assassin.clearAndReload();
            if (player == Ninja.ninja) Ninja.clearAndReload();
            if (player == NekoKabocha.nekoKabocha) NekoKabocha.clearAndReload();
            if (player == SerialKiller.serialKiller) SerialKiller.clearAndReload();
            if (player == EvilTracker.evilTracker) EvilTracker.clearAndReload();
            if (player == Undertaker.undertaker) Undertaker.clearAndReload();
            if (player == MimicK.mimicK) MimicK.clearAndReload();
            if (player == MimicA.mimicA) MimicA.clearAndReload();
            if (player == BomberA.bomberA) BomberA.clearAndReload();
            if (player == BomberB.bomberB) BomberB.clearAndReload();
            if (player == EvilHacker.evilHacker) EvilHacker.clearAndReload();
            if (player == Trapper.trapper) Trapper.clearAndReload();
            if (player == Blackmailer.blackmailer) Blackmailer.clearAndReload();
            if (player == Yoyo.yoyo) Yoyo.clearAndReload();
            //if (player == Bomber.bomber) Bomber.clearAndReload();

            // Other roles
            if (player == Jester.jester) Jester.clearAndReload();
            if (player == Arsonist.arsonist) Arsonist.clearAndReload();
            if (player == Kataomoi.kataomoi) Kataomoi.clearAndReload();
            if (Guesser.isGuesser(player.PlayerId)) Guesser.clear(player.PlayerId);
            if (player == Watcher.nicewatcher || player == Watcher.evilwatcher) Watcher.clear(player.PlayerId);
            if (player == Yasuna.yasuna) Yasuna.clearAndReload();
            if (player == Jackal.jackal) { // Promote Sidekick and hence override the the Jackal or erase Jackal
                if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead) {
                    RPCProcedure.sidekickPromotes();
                } else {
                    Jackal.clearAndReload();
                }
            }
            if (player == Sidekick.sidekick) Sidekick.clearAndReload();
            if (player == BountyHunter.bountyHunter) BountyHunter.clearAndReload();
            if (player == Vulture.vulture) Vulture.clearAndReload();
            if (player == Lawyer.lawyer) Lawyer.clearAndReload();
            if (player == Pursuer.pursuer) Pursuer.clearAndReload();
            if (player == Thief.thief) Thief.clearAndReload();
            if (player == Opportunist.opportunist) Opportunist.clearAndReload();
            if (player == Moriarty.moriarty) Moriarty.clearAndReload();
            if (player == JekyllAndHyde.jekyllAndHyde) JekyllAndHyde.clearAndReload();
            if (player == Akujo.akujo) Akujo.clearAndReload();
            if (player == Fox.fox) Fox.clearAndReload();
            if (player == Immoralist.immoralist) Immoralist.clearAndReload();
            if (player == PlagueDoctor.plagueDoctor) PlagueDoctor.clearAndReload();
            if (player == Cupid.cupid) Cupid.clearAndReload(false);
            if (player == SchrodingersCat.schrodingersCat) SchrodingersCat.clearAndReload();

            // Always remove the Madmate
            if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId)) Madmate.madmate.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Husk.husk.Any(x => x.PlayerId == player.PlayerId)) Husk.husk.RemoveAll(x => x.PlayerId == player.PlayerId);

            // Modifier
            if (!ignoreModifier)
            {
                if (player == Lovers.lover1 || player == Lovers.lover2) Lovers.clearAndReload(); // The whole Lover couple is being erased
                //if (Bait.bait.Any(x => x.PlayerId == player.PlayerId)) Bait.bait.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Bloody.bloody.Any(x => x.PlayerId == player.PlayerId)) Bloody.bloody.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == player.PlayerId)) AntiTeleport.antiTeleport.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == player.PlayerId)) Sunglasses.sunglasses.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (player == Tiebreaker.tiebreaker) Tiebreaker.clearAndReload();
                if (player == Mini.mini) Mini.clearAndReload();
                if (Vip.vip.Any(x => x.PlayerId == player.PlayerId)) Vip.vip.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Invert.invert.Any(x => x.PlayerId == player.PlayerId)) Invert.invert.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId)) Chameleon.chameleon.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (player == Armored.armored) Armored.clearAndReload();
            }

            if (GameStatistics.roleHistory.ContainsKey(playerId))
                GameStatistics.roleHistory[playerId].Add(new(player.Data.PlayerName, RoleInfo.crewmate, GameStatistics.currentTime, player.Data.DefaultOutfit, isCreatedMadmate, Color.white));
        }

        public static void setFutureErased(byte playerId) {
            PlayerControl player = Helpers.playerById(playerId);
            if (Eraser.futureErased == null) 
                Eraser.futureErased = new List<PlayerControl>();
            if (player != null) {
                Eraser.futureErased.Add(player);
            }
        }

        public static void setFutureShifted(byte playerId) {
            if (Shifter.isNeutral && !Shifter.shiftPastShifters && Shifter.pastShifters.Contains(playerId))
                return;
            Shifter.futureShift = Helpers.playerById(playerId);
        }

        public static void setFutureShielded(byte playerId) {
            Medic.futureShielded = Helpers.playerById(playerId);
            Medic.usedShield = true;
        }

        public static void setFutureSpelled(byte playerId) {
            PlayerControl player = Helpers.playerById(playerId);
            if (Witch.futureSpelled == null)
                Witch.futureSpelled = new List<PlayerControl>();
            if (player != null) {
                Witch.futureSpelled.Add(player);
            }
        }

        public static void recordStatistics(byte variation, byte relatedTag, byte sourceId, byte targetMask, float timeLag)
        {
            GameStatistics.Event.GameStatistics.RecordEvent(new GameStatistics.Event(GameStatistics.EventVariation.ValueOf(variation), GameStatistics.currentTime + timeLag, sourceId == byte.MaxValue ? null : sourceId, targetMask, null) { RelatedTag = TranslatableTag.ValueOf(relatedTag) });
        }

        public static void setRoleHistory()
        {
            GameStatistics.Event.GameStatistics = new();
            GameStatistics.roleHistory = new();
            GameStatistics.currentTime = 0f;

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                var info = RoleInfo.getRoleInfoForPlayer(p, false, true).FirstOrDefault();
                GameStatistics.roleHistory.Add(p.PlayerId, new());
                GameStatistics.roleHistory[p.PlayerId].Add(new GameStatistics.RoleHistory(p.Data.PlayerName, info, GameStatistics.currentTime, p.Data.DefaultOutfit, Madmate.madmate.Any(x => x.PlayerId == p.PlayerId), info.color));
            }
        }

        public static void foxStealth(bool stealthed)
        {
            Fox.setStealthed(stealthed);
        }

        public static void foxCreatesImmoralist(byte targetId)
        {
            PlayerControl player = Helpers.playerById(targetId);
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
            erasePlayerRoles(player.PlayerId, true);
            Immoralist.immoralist = player;
            player.clearAllTasks();

            if (player == Lawyer.lawyer && Lawyer.target != null)
            {
                Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo != null) playerInfo.text = "";
            }
            else if (player == Akujo.akujo)
            {
                if (Akujo.honmei != null)
                {
                    Transform playerInfoTransform = Akujo.honmei.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                    if (playerInfo != null) playerInfo.text = "";
                }
                if (Akujo.keeps != null)
                {
                    foreach (PlayerControl playerControl in Akujo.keeps)
                    {
                        Transform playerInfoTransform = playerControl.cosmetics.nameText.transform.parent.FindChild("Info");
                        TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                        if (playerInfo != null) playerInfo.text = "";
                    }
                }
            }
            Fox.canCreateImmoralist = false;
            GameStatistics.recordRoleHistory(player);
        }

        public static void placeAccel(byte id)
        {
            new Props.AccelTrap(Props.AccelTrap.findAccelPos()[id]);
        }

        public static void placeDecel(byte id)
        {
            new Props.DecelTrap(Props.DecelTrap.findDecelPos()[id]);
        }

        public static void activateDecel(byte decelId, byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var decel = Props.DecelTrap.decels[decelId];
            decel.isTriggered = true;
            decel.activateTime = DateTime.UtcNow;
            if (Props.DecelTrap.deceled.ContainsKey(player)) Props.DecelTrap.deceled.Remove(player);
            Props.DecelTrap.deceled.Add(player, DateTime.UtcNow);
            if (PlayerControl.LocalPlayer == player) SoundEffectsManager.play("triggerDeceleration");
            decel.decelTrap.SetActive(false);
        }

        public static void untriggerDecel(byte decelId)
        {
            var decel = Props.DecelTrap.decels[decelId];
            decel.decelTrap.SetActive(true);
            decel.isTriggered = false;
        }

        public static void deactivateDecel(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (Props.DecelTrap.deceled.ContainsKey(player)) Props.DecelTrap.deceled.Remove(player);
            if (PlayerControl.LocalPlayer == player) SoundEffectsManager.play("untriggerDeceleration");
        }

        public static void activateAccel(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (Props.AccelTrap.acceled.ContainsKey(player)) Props.AccelTrap.acceled.Remove(player);
            Props.AccelTrap.acceled.Add(player, DateTime.UtcNow);
            if (PlayerControl.LocalPlayer == player) SoundEffectsManager.play("jekyllAndHydeDrug");
        }

        public static void deactivateAccel(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);

            if (Props.AccelTrap.acceled.ContainsKey(player)) Props.AccelTrap.acceled.Remove(player);
            if (PlayerControl.LocalPlayer == player) SoundEffectsManager.play("jekyllAndHydeDrug");
        }

        public static void noisemakerSetSounded(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            Noisemaker.target = player;
        }

        public static void schrodingersCatSetTeam(byte team)
        {
            switch ((SchrodingersCat.Team)team)
            {
                case SchrodingersCat.Team.Crewmate:
                    SchrodingersCat.setCrewFlag();
                    break;
                case SchrodingersCat.Team.Impostor:
                    SchrodingersCat.setImpostorFlag();
                    FastDestroyableSingleton<RoleManager>.Instance.SetRole(SchrodingersCat.schrodingersCat, RoleTypes.Impostor);
                    break;
                case SchrodingersCat.Team.Jackal:
                    SchrodingersCat.setJackalFlag();
                    break;
                case SchrodingersCat.Team.JekyllAndHyde:
                    SchrodingersCat.setJekyllAndHydeFlag();
                    break;
                case SchrodingersCat.Team.Moriarty:
                    SchrodingersCat.setMoriartyFlag();
                    break;
                default:
                    SchrodingersCat.setCrewFlag();
                    break;
            }
            if (PlayerControl.LocalPlayer == SchrodingersCat.schrodingersCat) _ = new StaticAchievementToken("schrodingersCat.another1");
            GameStatistics.recordRoleHistory(SchrodingersCat.schrodingersCat);
        }

        public static void placeAssassinTrace(byte[] buff) {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new AssassinTrace(position, Assassin.traceTime);
            if (PlayerControl.LocalPlayer != Assassin.assassin)
                Assassin.assassinMarked = null;
        }

        public static void setInvisible(byte playerId, byte flag)
        {
            PlayerControl target = Helpers.playerById(playerId);
            if (target == null) return;
            if (flag == byte.MaxValue)
            {
                target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
                target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
                target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);

                if (Camouflager.camouflageTimer <= 0) target.setDefaultLook();
                //Assassin.isInvisble = false;
                return;
            }

            target.setLook("", 6, "", "", "", "");
            Color color = Color.clear;
            bool canSee = PlayerControl.LocalPlayer.Data.Role.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead;
            if (canSee) color.a = 0.1f;
            target.cosmetics.currentBodySprite.BodySprite.color = color;
            target.cosmetics.colorBlindText.gameObject.SetActive(false);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
            //Assassin.invisibleTimer = Assassin.invisibleDuration;
            //Assassin.isInvisble = true;
        }

        public static void placePortal(byte[] buff) {
            Vector3 position = Vector2.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Portal(position);
        }

        public static void usePortal(byte playerId, byte exit) {
            Portal.startTeleport(playerId, exit);
            if (PlayerControl.LocalPlayer == Portalmaker.portalmaker && Portalmaker.acTokenChallenge != null)
            {
                Portalmaker.acTokenChallenge.Value.portal++;
                Portalmaker.acTokenChallenge.Value.cleared |= Portalmaker.acTokenChallenge.Value.portal >= 3;
            }
        }

        public static void placeJackInTheBox(byte[] buff) {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));
            new JackInTheBox(position);
        }

        public static void lightsOut() {
            Trickster.lightsOutTimer = Trickster.lightsOutDuration;
            // If the local player is impostor indicate lights out
            if(Helpers.hasImpVision(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId))) {
                new CustomMessage(ModTranslation.getString("tricksterLightsOutText"), Trickster.lightsOutDuration);
            }
        }

        public static void ninjaStealth(byte playerId, bool stealthed)
        {
            PlayerControl player = Helpers.playerById(playerId);
            //if (Camouflager.camouflageTimer <= 0) player.setDefaultLook();
            Ninja.setStealthed(player, stealthed);
        }

        public static void nekoKabochaExile(byte targetId)
        {
            uncheckedExilePlayer(targetId);
            overrideDeathReasonAndKiller(Helpers.playerById(targetId), DeadPlayer.CustomDeathReason.Revenge, killer: NekoKabocha.nekoKabocha);
        }

        public static void serialKillerSuicide(byte serialKillerId)
        {
            PlayerControl serialKiller = Helpers.playerById(serialKillerId);
            if (serialKiller == null) return;
            serialKiller.MurderPlayer(serialKiller, MurderResultFlags.Succeeded);
            GameHistory.overrideDeathReasonAndKiller(serialKiller, DeadPlayer.CustomDeathReason.Suicide);
        }

        public static void sprinterSprint(byte playerId, bool sprinting)
        {
            PlayerControl player = Helpers.playerById(playerId);
            Sprinter.setSprinting(player, sprinting);
        }

        public static void updateMeeting(byte targetId)
        {
            if (MeetingHud.Instance)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.TargetPlayerId == targetId)
                    {
                        pva.SetDead(pva.DidReport, true);
                        pva.Overlay.gameObject.SetActive(true);
                        MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, targetId);
                    }

                    // Give players back their vote if target is shot dead
                    if (pva.VotedFor != targetId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();
                }

                if (AmongUsClient.Instance.AmHost)
                    MeetingHud.Instance.CheckForEndVoting();
            }
        }

        public static void teleporterTeleport(byte target1Id, byte target2Id)
        {
            PlayerControl target1 = Helpers.playerById(target1Id);
            PlayerControl target2 = Helpers.playerById(target2Id);

            target1.MyPhysics.ResetMoveState();
            target2.MyPhysics.ResetMoveState();
            var targetPosition = target1.GetTruePosition();
            var TempFacing = target1.cosmetics.currentBodySprite.BodySprite.flipX;
            target1.NetTransform.SnapTo(new Vector2(target2.GetTruePosition().x, target2.GetTruePosition().y + 0.3636f));
            target1.cosmetics.currentBodySprite.BodySprite.flipX = target2.cosmetics.currentBodySprite.BodySprite.flipX;
            target2.NetTransform.SnapTo(new Vector2(targetPosition.x, targetPosition.y + 0.3636f));
            target2.cosmetics.currentBodySprite.BodySprite.flipX = TempFacing;

            if (PlayerControl.LocalPlayer == target1 || PlayerControl.LocalPlayer == target2)
            {
                Helpers.showFlash(Teleporter.color);
                if (Minigame.Instance) Minigame.Instance.Close();
            }
        }

        public static void cupidSuicide(byte cupidId, bool isScapegoat)
        {
            var cupid = Helpers.playerById(cupidId);
            if (cupid != null)
            {
                cupid.MurderPlayer(cupid, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(cupid, isScapegoat ? DeadPlayer.CustomDeathReason.Scapegoat : DeadPlayer.CustomDeathReason.Suicide);
                if (MeetingHud.Instance) updateMeeting(cupidId);
                if (PlayerControl.LocalPlayer == cupid && isScapegoat) _ = new StaticAchievementToken("cupid.another1");
            }
        }

        public static void setCupidShield(byte targetId)
        {
            Cupid.shielded = Helpers.playerById(targetId);
        }

        public static void buskerPseudocide(byte targetId, bool isTrueDead, bool isLoverSuicide)
        {
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null) return;
            if (!isTrueDead)
            {
                Busker.pseudocideFlag = true;
                player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                if (player.AmOwner)
                {
                    if (Constants.ShouldPlaySfx())
                    {
                        SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
                    }
                    player.cosmetics.SetNameMask(false);
                    player.RpcSetScanner(false);
                }
                player.MyPhysics.StartCoroutine(player.KillAnimations.First().CoPerformKill(player, player));
                Busker.deathPosition = player.transform.position;
                Busker.buskerList.Add(targetId);
                if (Seer.seer != null && PlayerControl.LocalPlayer == Seer.seer && !Seer.seer.Data.IsDead && Seer.mode <= 1)
                {
                    Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: ModTranslation.getString("seerInfo"));
                    if (PlayerControl.LocalPlayer == Seer.seer)
                    {
                        _ = new StaticAchievementToken("seer.common1");
                        Seer.acTokenChallenge.Value.flash++;
                    }
                }
                if (Immoralist.immoralist != null && PlayerControl.LocalPlayer == Immoralist.immoralist && !PlayerControl.LocalPlayer.Data.IsDead)
                    Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
                if (PlagueDoctor.plagueDoctor != null && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead))
                    PlagueDoctor.checkWinStatus();
                Tracker.deadBodyPositions?.Add(Busker.deathPosition);
                if (Vip.vip.FindAll(x => x.PlayerId == player.PlayerId).Count > 0)
                {
                    Color color = Color.yellow;
                    if (Vip.showColor)
                        color = Color.white;
                    Helpers.showFlash(color, 1.5f);
                }
            }
            else
            {
                Busker.pseudocideFlag = false;
                Seer.deadBodyPositions?.Add(Busker.deathPosition);
                if (Medium.deadBodies != null) Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(new DeadPlayer(player, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Pseudocide, player), Busker.deathPosition));
                GameHistory.overrideDeathReasonAndKiller(player, isLoverSuicide ? DeadPlayer.CustomDeathReason.LoverSuicide : DeadPlayer.CustomDeathReason.Pseudocide);
                Busker.pseudocideComplete = true;
                Busker.buskerList.Remove(targetId);
                GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Kill, targetId, 1 << targetId) { RelatedTag = isLoverSuicide ? EventDetail.Kill : EventDetail.Pseudocide });

                PlayerControl otherLover = null;
                if ((Busker.busker == Lovers.lover1 || Busker.busker == Lovers.lover2) && Lovers.bothDie)
                    otherLover = Busker.busker == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                else if (Busker.busker == Cupid.lovers1 || Busker.busker == Cupid.lovers2)
                    otherLover = Busker.busker == Cupid.lovers1 ? Cupid.lovers2 : Cupid.lovers1;
                else if (Akujo.akujo != null && Busker.busker == Akujo.honmei)
                    otherLover = Akujo.akujo;

                if (otherLover != null && !otherLover.Data.IsDead)
                {
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }

                if (AmongUsClient.Instance.AmHost) FastDestroyableSingleton<RoleManager>.Instance.AssignRoleOnDeath(player, false);
            }
        }

        public static void buskerRevive(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (player == null) return;
            Busker.pseudocideFlag = false;
            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            foreach (DeadBody body in array)
            {
                if (body.ParentId != playerId) continue;

                UnityEngine.Object.Destroy(body.gameObject);
                break;
            }
            player.Revive();
            player.Data.IsDead = false;
            player.StartCoroutine(player.CoGush());
            Busker.buskerList.Remove(playerId);
            GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Revive, null, 1 << playerId) { RelatedTag = EventDetail.Revive });
        }

        public static void veteranAlert()
        {
            Veteran.alertActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Veteran.alertDuration, new Action<float>((p) => {
                if (p == 1f) Veteran.alertActive = false;
            })));
        }

        public static void veteranKill(byte targetId)
        {
            if (PlayerControl.LocalPlayer == Veteran.veteran)
            {
                PlayerControl player = Helpers.playerById(targetId);
                Helpers.checkMurderAttemptAndKill(Veteran.veteran, player);
            }
        }

        public static void unlockMayorAcCommon(byte votedFor)
        {
            if (PlayerControl.LocalPlayer == Mayor.mayor)
            {
                if (Mayor.acTokenDoubleVote != null)
                    Mayor.acTokenDoubleVote.Value.doubleVote |= !GameManager.Instance.LogicOptions.GetAnonymousVotes();
                if (Mayor.acTokenChallenge != null)
                {
                    Mayor.acTokenChallenge.Value.doubleVote = true;
                    Mayor.acTokenChallenge.Value.votedFor = votedFor;
                }
            }
        }

        public static void unlockDetectiveAcChallenge(byte votedFor)
        {
            if (Detective.acTokenChallenge != null && PlayerControl.LocalPlayer == Detective.detective)
            {
                Detective.acTokenChallenge.Value.votedFor = votedFor;
            }
        }

        public static void unlockMedicAcChallenge(byte killerId)
        {
            if (Medic.acTokenChallenge != null && PlayerControl.LocalPlayer == Medic.medic)
                Medic.acTokenChallenge.Value.killerId = killerId;
        }

        public static void unlockTrackerAcChallenge(float moveTime)
        {
            if (PlayerControl.LocalPlayer == Tracker.tracker)
            {
                if (!Tracker.acTokenChallenge.Value.cleared)
                {
                    if (Tracker.acTokenChallenge.Value.ventTime - moveTime >= Tracker.timeUntilUpdate)
                        Tracker.acTokenChallenge.Value.cleared = true;
                }
            }
        }

        public static void unlockVeteranAcChallenge()
        {
            if (PlayerControl.LocalPlayer == Veteran.veteran)
                _ = new StaticAchievementToken("veteran.challenge");
        }

        public static void yoyoMarkLocation(byte[] buff)
        {
            if (Yoyo.yoyo == null) return;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            Yoyo.markLocation(position);
            new Silhouette(position, -1, false);
        }

        public static void yoyoBlink(bool isFirstJump, byte[] buff)
        {
            TheOtherRolesPlugin.Logger.LogMessage($"blink fistjumpo: {isFirstJump}");
            if (Yoyo.yoyo == null || Yoyo.markedLocation == null) return;
            var markedPos = (Vector3)Yoyo.markedLocation;
            Yoyo.yoyo.NetTransform.SnapTo(markedPos);

            var markedSilhouette = Silhouette.silhouettes.FirstOrDefault(s => s.gameObject.transform.position.x == markedPos.x && s.gameObject.transform.position.y == markedPos.y);
            if (markedSilhouette != null)
                markedSilhouette.permanent = false;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            // Create Silhoutte At Start Position:
            if (isFirstJump)
            {
                Yoyo.markLocation(position);
                new Silhouette(position, Yoyo.blinkDuration, true);
            }
            else
            {
                new Silhouette(position, 5, true);
                Yoyo.markedLocation = null;
            }
            if (Chameleon.chameleon.Any(x => x.PlayerId == Yoyo.yoyo.PlayerId)) // Make the Yoyo visible if chameleon!
                Chameleon.lastMoved[Yoyo.yoyo.PlayerId] = Time.time;
        }
        public static void breakArmor()
        {
            if (Armored.armored == null || Armored.isBrokenArmor) return;
            Armored.isBrokenArmor = true;
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                Armored.armored.ShowFailedMurder();
            }
        }

        public static void mimicMorph(byte mimicAId, byte mimicBId)
        {
            //if (MimicA.mimicA == null || MimicK.mimicK == null || MimicK.mimicK.Data.IsDead) return;

            PlayerControl mimicA = Helpers.playerById(mimicAId);
            PlayerControl mimicB = Helpers.playerById(mimicBId);
            if (Camouflager.camouflageTimer <= 0f)
                mimicA.setLook(mimicB.Data.PlayerName, mimicB.Data.DefaultOutfit.ColorId, mimicB.Data.DefaultOutfit.HatId, mimicB.Data.DefaultOutfit.VisorId, mimicB.Data.DefaultOutfit.SkinId, mimicB.Data.DefaultOutfit.PetId);
            MimicA.isMorph = true;
        }

        public static void mimicResetMorph(byte mimicAId)
        {
            PlayerControl mimicA = Helpers.playerById(mimicAId);
            if (Camouflager.camouflageTimer <= 0f)
                mimicA.setDefaultLook();
            MimicA.isMorph = false;
        }

        public static void setShifterType(bool isNeutral)
        {
            Shifter.isNeutral = isNeutral;
        }

        public static void setOddIsJekyll(bool b)
        {
            JekyllAndHyde.oddIsJekyll = b;
        }

        public static void yasunaSpecialVote(byte playerid, byte targetid)
        {
            if (!MeetingHud.Instance) return;
            if (!Yasuna.isYasuna(playerid)) return;
            PlayerControl target = Helpers.playerById(targetid);
            if (target == null) return;
            Yasuna.specialVoteTargetPlayerId = targetid;
            Yasuna.remainingSpecialVotes(true);
        }

        public static void yasunaSpecialVote_DoCastVote()
        {
            if (!MeetingHud.Instance) return;
            if (!Yasuna.isYasuna(PlayerControl.LocalPlayer.PlayerId)) return;
            PlayerControl target = Helpers.playerById(Yasuna.specialVoteTargetPlayerId);
            if (target == null) return;
            MeetingHud.Instance.CmdCastVote(PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
        }

        public static void taskMasterSetExTasks(byte playerId, byte oldTaskMasterPlayerId, byte[] taskTypeIds)
        {
            PlayerControl oldTaskMasterPlayer = Helpers.playerById(oldTaskMasterPlayerId);
            if (oldTaskMasterPlayer != null)
            {
                oldTaskMasterPlayer.clearAllTasks();
                TaskMaster.oldTaskMasterPlayerId = oldTaskMasterPlayerId;
            }

            if (!TaskMaster.isTaskMaster(playerId))
                return;
            NetworkedPlayerInfo player = GameData.Instance.GetPlayerById(playerId);
            if (player == null)
                return;

            if (taskTypeIds != null && taskTypeIds.Length > 0)
            {
                player.Object.clearAllTasks();
                player.Tasks = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo.TaskInfo>(taskTypeIds.Length);
                for (int i = 0; i < taskTypeIds.Length; i++)
                {
                    player.Tasks.Add(new NetworkedPlayerInfo.TaskInfo(taskTypeIds[i], (uint)i));
                    player.Tasks[i].Id = (uint)i;
                }
                for (int i = 0; i < player.Tasks.Count; i++)
                {
                    NetworkedPlayerInfo.TaskInfo taskInfo = player.Tasks[i];
                    NormalPlayerTask normalPlayerTask = UnityEngine.Object.Instantiate(MapUtilities.CachedShipStatus.GetTaskById(taskInfo.TypeId), player.Object.transform);
                    normalPlayerTask.Id = taskInfo.Id;
                    normalPlayerTask.Owner = player.Object;
                    normalPlayerTask.Initialize();
                    player.Object.myTasks.Add(normalPlayerTask);
                }
                TaskMaster.isTaskComplete = true;
            }
            else
            {
                TaskMaster.isTaskComplete = false;
            }
        }

        public static void taskMasterUpdateExTasks(byte clearExTasks, byte allExTasks)
        {
            if (TaskMaster.taskMaster == null) return;
            TaskMaster.clearExTasks = clearExTasks;
            TaskMaster.allExTasks = allExTasks;
        }

        public static void setCupidLovers(byte playerId1, byte playerId2)
        {
            var p1 = Helpers.playerById(playerId1);
            var p2 = Helpers.playerById(playerId2);
            Cupid.lovers1 = p1;
            Cupid.lovers2 = p2;
            Cupid.breakLovers(p1);
            Cupid.breakLovers(p2);
        }

        public static void blackmailPlayer(byte playerId)
        {
            PlayerControl target = Helpers.playerById(playerId);
            Blackmailer.blackmailed = target;
        }

        public static void unblackmailPlayer()
        {
            Blackmailer.blackmailed = null;
            Blackmailer.alreadyShook = false;
        }

        public static void akujoSetHonmei(byte akujoId, byte targetId)
        {
            PlayerControl akujo = Helpers.playerById(akujoId);
            PlayerControl target = Helpers.playerById(targetId);

            if (akujo != null && Akujo.honmei == null)
            {
                Akujo.honmei = target;
                Akujo.breakLovers(target);
            }
        }

        public static void akujoSetKeep(byte akujoId, byte targetId)
        {
            var akujo = Helpers.playerById(akujoId);
            PlayerControl target = Helpers.playerById(targetId);

            if (akujo != null && Akujo.keepsLeft > 0)
            {
                Akujo.keeps.Add(target);
                Akujo.breakLovers(target);
                Akujo.keepsLeft--;
            }
        }

        public static void akujoSuicide(byte akujoId)
        {
            var akujo = Helpers.playerById(akujoId);
            if (akujo != null)
            {
                akujo.MurderPlayer(akujo, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(akujo, DeadPlayer.CustomDeathReason.Loneliness);
                if (MeetingHud.Instance) updateMeeting(akujoId);
            }
        }

        public static void unlockTaskMasterAcChallenge()
        {
            if (PlayerControl.LocalPlayer == TaskMaster.taskMaster)
                _ = new StaticAchievementToken("taskMaster.challenge");
        }

        public static void unlockJesterAcCommon()
        {
            if (PlayerControl.LocalPlayer == Jester.jester) {
                _ = new StaticAchievementToken("jester.common1");
            }
        }

        public static void activateTrap(byte trapId, byte trapperId, byte playerId)
        {
            var trapper = Helpers.playerById(trapperId);
            var player = Helpers.playerById(playerId);
            Trap.activateTrap(trapId, trapper, player);
        }

        public static void disableTrap(byte trapId)
        {
            Trap.disableTrap(trapId);
        }

        public static void placeTrap(byte[] buff)
        {
            Vector3 pos = Vector3.zero;
            pos.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            pos.y = BitConverter.ToSingle(buff, 1 * sizeof(float)) - 0.2f;
            Trap trap = new(pos);
        }

        public static void trapperKill(byte trapId, byte trapperId, byte playerId)
        {
            var trapper = Helpers.playerById(trapperId);
            var target = Helpers.playerById(playerId);
            if (PlayerControl.LocalPlayer == trapper)
            {
                _ = new StaticAchievementToken("trapper.common1");
                Trapper.acTokenChallenge.Value++;
            }
            var murder = Helpers.checkMuderAttempt(trapper, target);
            if (murder == MurderAttemptResult.ReverseKill) target.MurderPlayer(trapper, MurderResultFlags.Succeeded);
            if (murder != MurderAttemptResult.PerformKill) return;
            Trap.trapKill(trapId, trapper, target);
        }

        public static void clearTrap()
        {
            Trap.clearAllTraps();
        }

        public static void trapperMeetingFlag()
        {
            Trap.onMeeting();
        }

        public static void kataomoiSetTarget(byte playerId)
        {
            Kataomoi.target = Helpers.playerById(playerId);
        }

        public static void kataomoiWin()
        {
            if (Kataomoi.kataomoi == null) return;

            Kataomoi.triggerKataomoiWin = true;
            if (Kataomoi.target != null) {
                Kataomoi.target.Exiled();
                overrideDeathReasonAndKiller(Kataomoi.target, DeadPlayer.CustomDeathReason.KataomoiStare, Kataomoi.kataomoi);
            }
        }

        public static void kataomoiStalking(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (Kataomoi.kataomoi == null || Kataomoi.kataomoi != player) return;

            Kataomoi.doStalking();
        }

        public static void setBrainwash(byte playerId)
        {
            var p = Helpers.playerById(playerId);
            Moriarty.target = p;
            Moriarty.brainwashed.Add(p);
        }

        public static void moriartyKill(byte targetId)
        {
            PlayerControl target = Helpers.playerById(targetId);
            GameHistory.overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.BrainwashedKilled, Moriarty.moriarty);
            if (PlayerControl.LocalPlayer == Moriarty.target)
            {
                if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);
            }
            Moriarty.counter += 1;
            Moriarty.hasKilled = true;
            if (Moriarty.numberToWin == Moriarty.counter) Moriarty.triggerMoriartyWin = true;
        }

        public static void plantBomb(byte playerId)
        {
            var p = Helpers.playerById(playerId);
            if (PlayerControl.LocalPlayer == BomberA.bomberA) BomberB.bombTarget = p;
            if (PlayerControl.LocalPlayer == BomberB.bomberB) BomberA.bombTarget = p;
        }

        public static void releaseBomb(byte killer, byte target)
        {
            // 同時押しでダブルキルが発生するのを防止するためにBomberAで一度受け取ってから実行する
            if (PlayerControl.LocalPlayer == BomberA.bomberA)
            {
                if (BomberA.bombTarget != null && BomberB.bombTarget != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BomberKill, Hazel.SendOption.Reliable, -1);
                    writer.Write(killer);
                    writer.Write(target);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.bomberKill(killer, target);                    
                }
            }
        }

        public static void bomberKill(byte killer, byte target)
        {
            BomberA.bombTarget = null;
            BomberB.bombTarget = null;
            var k = Helpers.playerById(killer);
            var t = Helpers.playerById(target);
            if (!t.Data.IsDead)
            {
                KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                k.MurderPlayer(t, MurderResultFlags.Succeeded);
                if (BomberA.showEffects)
                {
                    new BombEffect(t);
                }
            }
            GameHistory.overrideDeathReasonAndKiller(t, DeadPlayer.CustomDeathReason.Bomb, k);
            bomberAPlantBombButton.Timer = bomberAPlantBombButton.MaxTimer;
            bomberBPlantBombButton.Timer = bomberBPlantBombButton.MaxTimer;
        }

        public static void placeCamera(byte[] buff) {
            var referenceCamera = UnityEngine.Object.FindObjectOfType<SurvCamera>(); 
            if (referenceCamera == null) return; // Mira HQ

            SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
            SecurityGuard.placedCameras++;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));

            var camera = UnityEngine.Object.Instantiate<SurvCamera>(referenceCamera);
            camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
            camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
            camera.Offset = new Vector3(0f, 0f, camera.Offset.z);
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2 || GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4) camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

            if (SubmergedCompatibility.IsSubmerged) {
                // remove 2d box collider of console, so that no barrier can be created. (irrelevant for now, but who knows... maybe we need it later)
                var fixConsole = camera.transform.FindChild("FixConsole");
                if (fixConsole != null) {
                    var boxCollider = fixConsole.GetComponent<BoxCollider2D>();
                    if (boxCollider != null) UnityEngine.Object.Destroy(boxCollider);
                }
            }


            if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard) {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            } else {
                camera.gameObject.SetActive(false);
            }
            TORMapOptions.camerasToAdd.Add(camera);
        }

        public static void sealVent(int ventId) {
            Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventId);
            if (vent == null) return;

            SecurityGuard.remainingScrews -= SecurityGuard.ventPrice;
            if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard) {
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>(); 
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                Sprite newSprite = animator == null ? SecurityGuard.getStaticVentSealedSprite() : SecurityGuard.getAnimatedVentSealedSprite();
                SpriteRenderer rend = vent.myRend;
                if (Helpers.isFungle())
                {
                    newSprite = SecurityGuard.getFungleVentSealedSprite();
                    rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                    animator = vent.transform.GetChild(3).GetComponent<PowerTools.SpriteAnim>();
                }
                animator?.Stop();
                rend.sprite = newSprite;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = SecurityGuard.getSubmergedCentralUpperSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = SecurityGuard.getSubmergedCentralLowerSealedSprite();
                rend.color = new Color(1f, 1f, 1f, 0.5f);
                vent.name = "FutureSealedVent_" + vent.name;
            }

            TORMapOptions.ventsToSeal.Add(vent);
        }

        public static void arsonistWin() {
            Arsonist.triggerArsonistWin = true;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                if (p != Arsonist.arsonist && !p.Data.IsDead) {
                    if (NekoKabocha.nekoKabocha != null && p == NekoKabocha.nekoKabocha) NekoKabocha.otherKiller = Arsonist.arsonist;
                    p.Exiled();
                    overrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Arson, Arsonist.arsonist);
                }
            }
        }

        public static void lawyerSetTarget(byte playerId) {
            Lawyer.target = Helpers.playerById(playerId);
        }

        public static void lawyerPromotesToPursuer() {
            PlayerControl player = Lawyer.lawyer;
            PlayerControl client = Lawyer.target;
            Lawyer.clearAndReload(false);

            Pursuer.pursuer = player;

            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId && client != null) {
                    Transform playerInfoTransform = client.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
            }

            GameStatistics.recordRoleHistory(player);
        }

        public static void lawyerWin()
        {
            Lawyer.triggerLawyerWin = true;
        }

        /// <summary>
        /// Shoots the dying target during the meeting
        /// </summary>
        /// <param name="killerId">PlayerId of the Guesser</param>
        /// <param name="dyingTargetId">PlayerId of the dying target (i.e. wrong guess = Guesser, right guess = target)</param>
        /// <param name="guessedTargetId">The PlayerId the dying target has guessed</param>
        /// <param name="guessedRoleId">The RoleId the Guesser has guessed (2 same RoleIds for Swapper and Shifter)</param>
        /// <param name="isSpecialRole">Whether or not this is a Nice Shifter or a Nice Swapper etc.</param>
        public static void guesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleId, bool isSpecialRole) {
            GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Kill, killerId, 1 << dyingTargetId) { RelatedTag = killerId == dyingTargetId ? EventDetail.MisGuess : EventDetail.Guessed});

            PlayerControl killer = Helpers.playerById(killerId);
            PlayerControl dyingTarget = Helpers.playerById(dyingTargetId);
            if (dyingTarget == null) return;
            bool revengeFlag = (NekoKabocha.revengeCrew && !Helpers.isNeutral(killer) && !killer.Data.Role.IsImpostor) ||
                    (NekoKabocha.revengeNeutral && Helpers.isNeutral(killer)) ||
                    (NekoKabocha.revengeImpostor && killer.Data.Role.IsImpostor);
            if (dyingTarget == NekoKabocha.nekoKabocha)
            {
                NekoKabocha.meetingKiller = killer;
                if (revengeFlag)
                {
                    killer.Exiled();
                    overrideDeathReasonAndKiller(killer, DeadPlayer.CustomDeathReason.Revenge, NekoKabocha.nekoKabocha);
                    updateMeeting(killerId);
                }
            }

            PlayerControl guesser = Helpers.playerById(killerId);
            if (Thief.thief != null && Thief.thief.PlayerId == killerId && Thief.canStealWithGuess) {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                if (!Thief.thief.Data.IsDead && !Thief.isFailedThiefKill(dyingTarget, guesser, roleInfo)) {
                    RPCProcedure.thiefStealsRole(dyingTarget.PlayerId);
                }
            }

            dyingTarget.Exiled();
            GameHistory.overrideDeathReasonAndKiller(dyingTarget, DeadPlayer.CustomDeathReason.Guess, guesser);

            if (PlayerControl.LocalPlayer == killer && dyingTarget != killer)
            {
                if (killer == Guesser.niceGuesser)
                {
                    Guesser.acTokenNiceGuesser.Value++;
                    _ = new StaticAchievementToken("niceGuesser.common1");
                    if (dyingTarget == Guesser.evilGuesser)
                        _ = new StaticAchievementToken("niceGuesser.challenge2");
                }
                else if (killer == Guesser.evilGuesser)
                {
                    Guesser.acTokenEvilGuesser.Value++;
                    _ = new StaticAchievementToken("evilGuesser.common1");
                    if (dyingTarget == Guesser.niceGuesser)
                        _ = new StaticAchievementToken("evilGuesser.challenge2");
                }
            }

            HandleGuesser.remainingShots(killerId, true);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
            updateMeeting(dyingTargetId);
            if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
                if (PlayerControl.LocalPlayer == dyingTarget) {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
                else if (PlayerControl.LocalPlayer == NekoKabocha.meetingKiller && revengeFlag) {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(NekoKabocha.nekoKabocha.Data, killer.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }

            // remove shoot button from targets for all guessers and close their guesserUI
            if (GuesserGM.isGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer != guesser && !PlayerControl.LocalPlayer.Data.IsDead && GuesserGM.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 && MeetingHud.Instance) {
                MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                if (NekoKabocha.meetingKiller != null && revengeFlag)
                    MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == NekoKabocha.meetingKiller.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                if (MeetingHudPatch.guesserUI != null && MeetingHudPatch.guesserUIExitButton != null) {
                    if (MeetingHudPatch.guesserCurrentTarget == dyingTarget.PlayerId)
                        MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                    else if (NekoKabocha.meetingKiller != null && revengeFlag && MeetingHudPatch.guesserCurrentTarget == NekoKabocha.meetingKiller.PlayerId)
                        MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
            }


            if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
            {
                PlayerControl guessedTarget = Helpers.playerById(guessedTargetId);
                PlayerControl sender = PlayerControl.LocalPlayer;
                RoleInfo guessedRoleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                if (isSpecialRole) {
                    if ((RoleId)guessedRoleId == RoleId.Swapper) guessedRoleInfo = RoleInfo.niceSwapper;
                    else if ((RoleId)guessedRoleId == RoleId.Shifter) guessedRoleInfo = RoleInfo.niceshifter;
                }
                string msg = "";
                if (PlayerControl.LocalPlayer.Data.IsDead && guessedTarget != null && guesser != null)
                {
                    msg = string.Format(ModTranslation.getString("guesserGuessChat"), guesser.Data.PlayerName, guessedRoleInfo?.name ?? "", guessedTarget.Data.PlayerName);
                    sender = guesser;
                }
                else if (!PlayerControl.LocalPlayer.Data.IsDead && (PlayerControl.LocalPlayer == Watcher.nicewatcher || PlayerControl.LocalPlayer == Watcher.evilwatcher) && Watcher.canSeeGuesses)
                    msg = string.Format(ModTranslation.getString("watcherGuessChat"), guessedRoleInfo?.name ?? "", guessedTarget.Data.PlayerName);

                if (!string.IsNullOrEmpty(msg))
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, msg, false);
            }
        }

        public static void setBlanked(byte playerId, byte value) {
            PlayerControl target = Helpers.playerById(playerId);
            if (target == null) return;
            Pursuer.blankedList.RemoveAll(x => x.PlayerId == playerId);
            if (value > 0) Pursuer.blankedList.Add(target);            
        }

        public static void bloody(byte killerPlayerId, byte bloodyPlayerId) {
            //if (Helpers.playerById(killerPlayerId) == MimicK.mimicK) return;
            if (Bloody.active.ContainsKey(killerPlayerId)) return;            
            Bloody.active.Add(killerPlayerId, Bloody.duration);
            Bloody.bloodyKillerMap.Add(killerPlayerId, bloodyPlayerId);
        }

        public static void setFirstKill(byte playerId) {
            PlayerControl target = Helpers.playerById(playerId);
            if (target == null) return;
            TORMapOptions.firstKillPlayer = target;
        }

        public static void setTiebreak() {
            Tiebreaker.isTiebreak = true;
        }

        public static void thiefStealsRole(byte playerId) {
            // Notify that the Thief cannot steal the Mimic

            PlayerControl target = Helpers.playerById(playerId);
            PlayerControl thief = Thief.thief;
            if (target == null) return;
            if (target == Sheriff.sheriff) {
                Sheriff.sheriff = thief;
                Sheriff.onAchievementActivate();
            }
            if (target == Jackal.jackal) {
                Jackal.jackal = thief;
                Jackal.formerJackals.Add(target);
            }
            if (target == Sidekick.sidekick) {
                Sidekick.sidekick = thief;
                Jackal.formerJackals.Add(target);
            }
            if (target == Moriarty.moriarty)
            {
                Moriarty.clearAllArrow();
                Moriarty.moriarty = thief;
                Moriarty.formerMoriarty = target;
                Moriarty.target = null;
                Moriarty.brainwashed.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
            if (target == JekyllAndHyde.jekyllAndHyde)
            {
                JekyllAndHyde.jekyllAndHyde = thief;
                JekyllAndHyde.formerJekyllAndHyde = target;
            }
            if (target == SchrodingersCat.schrodingersCat)
            {
                SchrodingersCat.schrodingersCat = thief;
                SchrodingersCat.formerSchrodingersCat = target;
            }
            if (target == Guesser.evilGuesser)
            {
                Guesser.evilGuesser = thief;
                Guesser.evilGuesserOnAchievementActivate();
            }
            if (target == Watcher.evilwatcher) Watcher.evilwatcher = thief;
            if (target == Godfather.godfather) Godfather.godfather = thief;
            if (target == Mafioso.mafioso) Mafioso.mafioso = thief;
            if (target == Janitor.janitor) Janitor.janitor = thief;
            if (target == Morphling.morphling)
            {
                Morphling.morphling = thief;
                Morphling.onAchievementActivate();
            }
            if (target == Camouflager.camouflager)
            {
                Camouflager.camouflager = thief;
                Camouflager.onAchievementActivate();
            }
            if (target == Vampire.vampire)
            {
                Vampire.vampire = thief;
                Vampire.onAchievementActivate();
            }
            if (target == Eraser.eraser)
            {
                Eraser.eraser = thief;
                Eraser.onAchievementActivate();
            }
            if (target == Trickster.trickster)
            {
                Trickster.trickster = thief;
                Trickster.onAchievementActivate();
            }
            if (target == Cleaner.cleaner)
            {
                Cleaner.cleaner = thief;
                Cleaner.onAchievementActivate();
            }
            if (target == Warlock.warlock)
            {
                Warlock.warlock = thief;
                Warlock.onAchievementActivate();
            }
            if (target == BountyHunter.bountyHunter)
            {
                BountyHunter.clearAllArrow();
                BountyHunter.bountyHunter = thief;
                BountyHunter.onAchievementActivate();
            }
            if (target == Ninja.ninja)
            {
                Ninja.ninja = thief;
                Ninja.onAchievementActivate();
            }
            if (target == EvilTracker.evilTracker)
            {
                EvilTracker.clearAllArrow();
                EvilTracker.evilTracker = thief;
            }
            if (target == NekoKabocha.nekoKabocha && !NekoKabocha.revengeNeutral) NekoKabocha.nekoKabocha = thief;
            if (target == SerialKiller.serialKiller) SerialKiller.serialKiller = thief;
            if (target == Swapper.swapper && target.Data.Role.IsImpostor)
            {
                Swapper.swapper = thief;
                Swapper.evilSwapperOnAchievementActivate();
            }
            if (target == Undertaker.undertaker) Undertaker.undertaker = thief;
            if (target == EvilHacker.evilHacker)
            {
                EvilHacker.evilHacker = thief;
                EvilHacker.onAchievementActivate();
            }
            if (target == Trapper.trapper)
            {
                Trapper.trapper = thief;
                Trapper.onAchievementActivate();
            }
            if (target == Blackmailer.blackmailer)
            {
                Blackmailer.blackmailer = thief;
                Blackmailer.onAchievementActivate();
            }
            if (target == Yasuna.yasuna)
            {
                Yasuna.yasuna = thief;
                Yasuna.evilYasunaOnAcheivementActivate();
            }
            if (target == Witch.witch) {
                Witch.witch = thief;
                Witch.onAchievementActivate();
                if (MeetingHud.Instance) 
                    if (Witch.witchVoteSavesTargets)  // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                        Witch.futureSpelled = new();
                else  // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                    Witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
            if (target == Assassin.assassin)
            {
                Assassin.assassin = thief;
                Assassin.onAchievementActivate();
            }
            if (target == Yoyo.yoyo)
            {
                Yoyo.yoyo = thief;
                Yoyo.markedLocation = null;
            }
            //if (target == Bomber.bomber) Bomber.bomber = thief;
            if (target.Data.Role.IsImpostor) {
                RoleManager.Instance.SetRole(Thief.thief, RoleTypes.Impostor);
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(Thief.thief.killTimer, PlayerControl.LocalPlayer.GetKillCooldown());
            }
            if (Lawyer.lawyer != null && target == Lawyer.target)
                Lawyer.target = thief;
            if (Thief.thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
            Thief.clearAndReload();
            Thief.formerThief = thief;  // After clearAndReload, else it would get reset...
            GameStatistics.recordRoleHistory(thief);
            GameStatistics.recordRoleHistory(target);
        }
        
        /*public static void setTrap(byte[] buff) {
            if (Trapper.trapper == null) return;
            Trapper.charges -= 1;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Trap(position);
        }*/

        /*public static void triggerTrap(byte playerId, byte trapId) {
            Trap.triggerTrap(playerId, trapId);
        }*/

        public static void setGuesserGm (byte playerId) {
            PlayerControl target = Helpers.playerById(playerId);
            if (target == null) return;
            new GuesserGM(target);
        }

        public static void shareTimer(float punish) {
            HideNSeek.timer -= punish;
        }

        public static void huntedShield(byte playerId) {
            if (!Hunted.timeshieldActive.Contains(playerId)) Hunted.timeshieldActive.Add(playerId);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Hunted.shieldDuration, new Action<float>((p) => {
                if (p == 1f) Hunted.timeshieldActive.Remove(playerId);
            })));
        }

        public static void huntedRewindTime(byte playerId) {
            Hunted.timeshieldActive.Remove(playerId); // Shield is no longer active when rewinding
            SoundEffectsManager.stop("timemasterShield");  // Shield sound stopped when rewinding
            if (playerId == PlayerControl.LocalPlayer.PlayerId) {
                resetHuntedRewindButton();
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Hunted.shieldRewindTime, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return; // only rewind hunter

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            PlayerControl.LocalPlayer.moveable = false;
        }

        public enum GhostInfoTypes {
            HandcuffNoticed,
            HandcuffOver,
            ArsonistDouse,
            BountyTarget,
            AssassinMarked,
            WarlockTarget,
            MediumInfo,
            BlankUsed,
            DetectiveOrMedicInfo,
            VampireTimer,
            DeathReasonAndKiller,
        }

        public static void receiveGhostInfo (byte senderId, MessageReader reader) {
            PlayerControl sender = Helpers.playerById(senderId);

            GhostInfoTypes infoType = (GhostInfoTypes)reader.ReadByte();
            switch (infoType) {
                case GhostInfoTypes.HandcuffNoticed:
                    Deputy.setHandcuffedKnows(true, senderId);
                    break;
                case GhostInfoTypes.HandcuffOver:
                    _ = Deputy.handcuffedKnows.Remove(senderId);
                    break;
                case GhostInfoTypes.ArsonistDouse:
                    Arsonist.dousedPlayers.Add(Helpers.playerById(reader.ReadByte()));
                    break;
                case GhostInfoTypes.BountyTarget:
                    BountyHunter.bounty = Helpers.playerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.AssassinMarked:
                    Assassin.assassinMarked = Helpers.playerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.WarlockTarget:
                    Warlock.curseVictim = Helpers.playerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.MediumInfo:
                    string mediumInfo = reader.ReadString();
		             if (Helpers.shouldShowGhostInfo())
                    	FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, mediumInfo, false);
                    break;
                case GhostInfoTypes.DetectiveOrMedicInfo:
                    string detectiveInfo = reader.ReadString();
                    if (Helpers.shouldShowGhostInfo())
		    	        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, detectiveInfo, false);
                    break;
                case GhostInfoTypes.BlankUsed:
                    Pursuer.blankedList.Remove(sender);
                    break;
                case GhostInfoTypes.VampireTimer:
                    HudManagerStartPatch.vampireKillButton.Timer = (float)reader.ReadByte();
                    break;
                case GhostInfoTypes.DeathReasonAndKiller:
                    GameHistory.overrideDeathReasonAndKiller(Helpers.playerById(reader.ReadByte()), (DeadPlayer.CustomDeathReason)reader.ReadByte(), Helpers.playerById(reader.ReadByte()));
                    break;
            }
        }

        /*public static void placeBomb(byte[] buff) {
            if (Bomber.bomber == null) return;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Bomb(position);
        }

        public static void defuseBomb() {
            try {
                SoundEffectsManager.playAtPosition("bombDefused", Bomber.bomb.bomb.transform.position, range: Bomber.hearRange);
            } catch { }
            Bomber.clearBomb();
            bomberButton.Timer = bomberButton.MaxTimer;
            bomberButton.isEffectActive = false;
            bomberButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        }*/
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class RPCHandlerPatch
    {
        static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (Modules.AntiCheat.RpcSafe(__instance, callId, reader)) return false;
            return true;
        }

        static void Postfix([HarmonyArgument(0)]byte callId, [HarmonyArgument(1)]MessageReader reader)
        {
            byte packetId = callId;
            switch (packetId) {

                // Main Controls

                case (byte)CustomRPC.ResetVaribles:
                    RPCProcedure.resetVariables();
                    break;
                case (byte)CustomRPC.ShareOptions:
                    RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
                    break;
                case (byte)CustomRPC.ForceEnd:
                    RPCProcedure.forceEnd();
                    break; 
                case (byte)CustomRPC.WorkaroundSetRoles:
                    RPCProcedure.workaroundSetRoles(reader.ReadByte(), reader);
                    break;
                case (byte)CustomRPC.SetRole:
                    byte roleId = reader.ReadByte();
                    byte playerId = reader.ReadByte();
                    RPCProcedure.setRole(roleId, playerId);
                    break;
                case (byte)CustomRPC.SetModifier:
                    byte modifierId = reader.ReadByte();
                    byte pId = reader.ReadByte();
                    byte flag = reader.ReadByte();
                    RPCProcedure.setModifier(modifierId, pId, flag);
                    break;
                case (byte)CustomRPC.VersionHandshake:
                    byte major = reader.ReadByte();
                    byte minor = reader.ReadByte();
                    byte patch = reader.ReadByte();
                    float timer = reader.ReadSingle();
                    if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.timer = timer;
                    int versionOwnerId = reader.ReadPackedInt32();
                    byte revision = 0xFF;
                    Guid guid;
                    if (reader.Length - reader.Position >= 17) { // enough bytes left to read
                        revision = reader.ReadByte();
                        // GUID
                        byte[] gbytes = reader.ReadBytes(16);
                        guid = new Guid(gbytes);
                    } else {
                        guid = new Guid(new byte[16]);
                    }
                    RPCProcedure.versionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                    break;
                case (byte)CustomRPC.UseUncheckedVent:
                    int ventId = reader.ReadPackedInt32();
                    byte ventingPlayer = reader.ReadByte();
                    byte isEnter = reader.ReadByte();
                    RPCProcedure.useUncheckedVent(ventId, ventingPlayer, isEnter);
                    break;
                case (byte)CustomRPC.UncheckedMurderPlayer:
                    byte source = reader.ReadByte();
                    byte target = reader.ReadByte();
                    byte showAnimation = reader.ReadByte();
                    RPCProcedure.uncheckedMurderPlayer(source, target, showAnimation);
                    break;
                case (byte)CustomRPC.UncheckedExilePlayer:
                    byte exileTarget = reader.ReadByte();
                    RPCProcedure.uncheckedExilePlayer(exileTarget);
                    break;
                case (byte)CustomRPC.UncheckedCmdReportDeadBody:
                    byte reportSource = reader.ReadByte();
                    byte reportTarget = reader.ReadByte();
                    RPCProcedure.uncheckedCmdReportDeadBody(reportSource, reportTarget);
                    break;
                case (byte)CustomRPC.UncheckedSetTasks:
                    RPCProcedure.uncheckedSetTasks(reader.ReadByte(), reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.FinishShipStatusBegin:
                    RPCProcedure.finishShipStatusBegin();
                    break;
                case (byte)CustomRPC.DynamicMapOption:
                    byte mapId = reader.ReadByte();
                    RPCProcedure.dynamicMapOption(mapId);
                    break;
                case (byte)CustomRPC.SetGameStarting:
                    RPCProcedure.setGameStarting();
                    break;

                // Role functionality

                case (byte)CustomRPC.EngineerFixLights:
                    RPCProcedure.engineerFixLights();
                    break;
                case (byte)CustomRPC.EngineerFixSubmergedOxygen:
                    RPCProcedure.engineerFixSubmergedOxygen();
                    break;
                case (byte)CustomRPC.EngineerUsedRepair:
                    RPCProcedure.engineerUsedRepair();
                    break;
                case (byte)CustomRPC.CleanBody:
                    RPCProcedure.cleanBody(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.TimeMasterRewindTime:
                    RPCProcedure.timeMasterRewindTime();
                    break;
                case (byte)CustomRPC.TimeMasterShield:
                    RPCProcedure.timeMasterShield();
                    break;
                case (byte)CustomRPC.MedicSetShielded:
                    RPCProcedure.medicSetShielded(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ShieldedMurderAttempt:
                    RPCProcedure.shieldedMurderAttempt();
                    break;
                case (byte)CustomRPC.ShifterShift:
                    RPCProcedure.shifterShift(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SwapperSwap:
                    byte playerId1 = reader.ReadByte();
                    byte playerId2 = reader.ReadByte();
                    RPCProcedure.swapperSwap(playerId1, playerId2);
                    break;
                case (byte)CustomRPC.MayorSetVoteTwice:
                    Mayor.voteTwice = reader.ReadBoolean();
                    break;
                case (byte)CustomRPC.MorphlingMorph:
                    RPCProcedure.morphlingMorph(reader.ReadByte());
                    break;
                case (byte)CustomRPC.CamouflagerCamouflage:
                    RPCProcedure.camouflagerCamouflage();
                    break;
                case (byte)CustomRPC.VampireSetBitten:
                    byte bittenId = reader.ReadByte();
                    byte reset = reader.ReadByte();
                    RPCProcedure.vampireSetBitten(bittenId, reset);
                    break;
                case (byte)CustomRPC.PlaceGarlic:
                    RPCProcedure.placeGarlic(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.TrackerUsedTracker:
                    RPCProcedure.trackerUsedTracker(reader.ReadByte());
                    break;               
                case (byte)CustomRPC.DeputyUsedHandcuffs:
                    RPCProcedure.deputyUsedHandcuffs(reader.ReadByte());
                    break;
                case (byte)CustomRPC.DeputyPromotes:
                    RPCProcedure.deputyPromotes();
                    break;
                case (byte)CustomRPC.JackalCreatesSidekick:
                    RPCProcedure.jackalCreatesSidekick(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SidekickPromotes:
                    RPCProcedure.sidekickPromotes();
                    break;
                case (byte)CustomRPC.ErasePlayerRoles:
                    byte eraseTarget = reader.ReadByte();
                    RPCProcedure.erasePlayerRoles(eraseTarget);
                    Eraser.alreadyErased.Add(eraseTarget);
                    break;
                case (byte)CustomRPC.SetFutureErased:
                    RPCProcedure.setFutureErased(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SetFutureShifted:
                    RPCProcedure.setFutureShifted(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SetFutureShielded:
                    RPCProcedure.setFutureShielded(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceAssassinTrace:
                    RPCProcedure.placeAssassinTrace(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.PlacePortal:
                    RPCProcedure.placePortal(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.UsePortal:
                    RPCProcedure.usePortal(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceJackInTheBox:
                    RPCProcedure.placeJackInTheBox(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.LightsOut:
                    RPCProcedure.lightsOut();
                    break;
                case (byte)CustomRPC.YoyoMarkLocation:
                    RPCProcedure.yoyoMarkLocation(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.YoyoBlink:
                    RPCProcedure.yoyoBlink(reader.ReadByte() == byte.MaxValue, reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.BreakArmor:
                    RPCProcedure.breakArmor();
                    break;
                case (byte)CustomRPC.NinjaStealth:
                    RPCProcedure.ninjaStealth(reader.ReadByte(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.SprinterSprint:
                    RPCProcedure.sprinterSprint(reader.ReadByte(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.NekoKabochaExile:
                    RPCProcedure.nekoKabochaExile(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SerialKillerSuicide:
                    RPCProcedure.serialKillerSuicide(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceAntique:
                    RPCProcedure.placeAntique();
                    break;
                case (byte)CustomRPC.ArchaeologistDetect:
                    RPCProcedure.archaeologistDetect(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ArchaeologistExcavate:
                    RPCProcedure.archaeologistExcavate();
                    break;
                case (byte)CustomRPC.FortuneTellerUsedDivine:
                    byte fId = reader.ReadByte();
                    byte tId = reader.ReadByte();
                    RPCProcedure.fortuneTellerUsedDivine(fId, tId);
                    break;
                case (byte)CustomRPC.PlagueDoctorWin:
                    RPCProcedure.plagueDoctorWin();
                    break;
                case (byte)CustomRPC.PlagueDoctorSetInfected:
                    RPCProcedure.plagueDoctorInfected(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlagueDoctorUpdateProgress:
                    byte progressTarget = reader.ReadByte();
                    byte[] progressByte = reader.ReadBytes(4);
                    float progress = System.BitConverter.ToSingle(progressByte, 0);
                    RPCProcedure.plagueDoctorProgress(progressTarget, progress);
                    break;
                case (byte)CustomRPC.PlaceAccel:
                    RPCProcedure.placeAccel(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceDecel:
                    RPCProcedure.placeDecel(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ActivateDecel:
                    RPCProcedure.activateDecel(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UntriggerDecel:
                    RPCProcedure.untriggerDecel(reader.ReadByte());
                    break;
                case (byte)CustomRPC.DeactivateDecel:
                    RPCProcedure.deactivateDecel(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ActivateAccel:
                    RPCProcedure.activateAccel(reader.ReadByte());
                    break;
                case (byte)CustomRPC.DeactivateAccel:
                    RPCProcedure.deactivateAccel(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SetOddIsJekyll:
                    RPCProcedure.setOddIsJekyll(reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.EvilHackerCreatesMadmate:
                    RPCProcedure.evilHackerCreatesMadmate(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ProphetExamine:
                    RPCProcedure.prophetExamine(reader.ReadByte());
                    break;
                case (byte)CustomRPC.VeteranAlert:
                    RPCProcedure.veteranAlert();
                    break;
                case (byte)CustomRPC.VeteranKill:
                    RPCProcedure.veteranKill(reader.ReadByte());
                    break;
                case (byte)CustomRPC.NoisemakerSetSounded:
                    RPCProcedure.noisemakerSetSounded(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SchrodingersCatSetTeam:
                    RPCProcedure.schrodingersCatSetTeam(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockMayorAcCommon:
                    RPCProcedure.unlockMayorAcCommon(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockDetectiveAcChallenge:
                    RPCProcedure.unlockDetectiveAcChallenge(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockMedicAcChallenge:
                    RPCProcedure.unlockMedicAcChallenge(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockTrackerAcChallenge:
                    RPCProcedure.unlockTrackerAcChallenge(reader.ReadSingle());
                    break;
                case (byte)CustomRPC.UnlockVeteranAcChallenge:
                    RPCProcedure.unlockVeteranAcChallenge();
                    break;
                case (byte)CustomRPC.UnlockTaskMasterAcChallenge:
                    RPCProcedure.unlockTaskMasterAcChallenge();
                    break;
                case (byte)CustomRPC.UnlockJesterAcCommon:
                    RPCProcedure.unlockJesterAcCommon();
                    break;
                case (byte)CustomRPC.KataomoiSetTarget:
                    playerId = reader.ReadByte();
                    RPCProcedure.kataomoiSetTarget(playerId);
                    break;
                case (byte)CustomRPC.KataomoiWin:
                    RPCProcedure.kataomoiWin();
                    break;
                case (byte)CustomRPC.KataomoiStalking:
                    playerId = reader.ReadByte();
                    RPCProcedure.kataomoiStalking(playerId);
                    break;
                case (byte)CustomRPC.ResetAchievement:
                    RPCProcedure.resetAchievement();
                    break;
                case (byte)CustomRPC.RecordStatistics:
                    RPCProcedure.recordStatistics(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadSingle());
                    break;
                case (byte)CustomRPC.SetRoleHistory:
                    RPCProcedure.setRoleHistory();
                    break;
                case (byte)CustomRPC.AkujoSetHonmei:
                    RPCProcedure.akujoSetHonmei(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.AkujoSetKeep:
                    RPCProcedure.akujoSetKeep(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.AkujoSuicide:
                    RPCProcedure.akujoSuicide(reader.ReadByte());
                    break;
                case (byte)CustomRPC.BuskerPseudocide:
                    RPCProcedure.buskerPseudocide(reader.ReadByte(), reader.ReadBoolean(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.BuskerRevive:
                    RPCProcedure.buskerRevive(reader.ReadByte());
                    break;
                case (byte)CustomRPC.BlackmailPlayer:
                    RPCProcedure.blackmailPlayer(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnblackmailPlayer:
                    RPCProcedure.unblackmailPlayer();
                    break;
                case (byte)CustomRPC.UndertakerDragBody:
                    var bodyId = reader.ReadByte();
                    Undertaker.DragBody(bodyId);
                    break;
                case (byte)CustomRPC.SetCupidLovers:
                    RPCProcedure.setCupidLovers(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.CupidSuicide:
                    RPCProcedure.cupidSuicide(reader.ReadByte(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.SetCupidShield:
                    RPCProcedure.setCupidShield(reader.ReadByte());
                    break;
                case (byte)CustomRPC.TeleporterTeleport:
                    RPCProcedure.teleporterTeleport(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UndertakerDropBody:
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    Undertaker.DropBody(new Vector3(x, y, z));
                    break;
                case (byte)CustomRPC.MimicMorph:
                    byte mimicA = reader.ReadByte();
                    byte mimicB = reader.ReadByte();
                    RPCProcedure.mimicMorph(mimicA, mimicB);
                    break;
                case (byte)CustomRPC.MimicResetMorph:
                    RPCProcedure.mimicResetMorph(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ShareRealTasks:
                    RPCProcedure.shareRealTasks(reader);
                    break;
                case (byte)CustomRPC.SetShifterType:
                    RPCProcedure.setShifterType(reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.YasunaSpecialVote:
                    byte id = reader.ReadByte();
                    byte targetId = reader.ReadByte();
                    RPCProcedure.yasunaSpecialVote(id, targetId);
                    break;
                case (byte)CustomRPC.YasunaSpecialVote_DoCastVote:
                    RPCProcedure.yasunaSpecialVote_DoCastVote();
                    break;
                case (byte)CustomRPC.TaskMasterSetExTasks:
                    playerId = reader.ReadByte();
                    byte oldTaskMasterPlayerId = reader.ReadByte();
                    byte[] taskTypeIds = reader.BytesRemaining > 0 ? reader.ReadBytes(reader.BytesRemaining) : null;
                    RPCProcedure.taskMasterSetExTasks(playerId, oldTaskMasterPlayerId, taskTypeIds);
                    break;
                case (byte)CustomRPC.TaskMasterUpdateExTasks:
                    byte clearExTasks = reader.ReadByte();
                    byte allExTasks = reader.ReadByte();
                    RPCProcedure.taskMasterUpdateExTasks(clearExTasks, allExTasks);
                    break;
                case (byte)CustomRPC.FoxStealth:
                    RPCProcedure.foxStealth(reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.FoxCreatesImmoralist:
                    RPCProcedure.foxCreatesImmoralist(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceTrap:
                    RPCProcedure.placeTrap(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.ClearTrap:
                    RPCProcedure.clearTrap();
                    break;
                case (byte)CustomRPC.ActivateTrap:
                    RPCProcedure.activateTrap(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.DisableTrap:
                    RPCProcedure.disableTrap(reader.ReadByte());
                    break;
                case (byte)CustomRPC.TrapperMeetingFlag:
                    RPCProcedure.trapperMeetingFlag();
                    break;
                case (byte)CustomRPC.TrapperKill:
                    RPCProcedure.trapperKill(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.SetBrainwash:
                    RPCProcedure.setBrainwash(reader.ReadByte());
                    break;
                case (byte)CustomRPC.MoriartyKill:
                    RPCProcedure.moriartyKill(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlantBomb:
                    RPCProcedure.plantBomb(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ReleaseBomb:
                    RPCProcedure.releaseBomb(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.BomberKill:
                    RPCProcedure.bomberKill(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceCamera:
                    RPCProcedure.placeCamera(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.SealVent:
                    RPCProcedure.sealVent(reader.ReadPackedInt32());
                    break;
                case (byte)CustomRPC.ArsonistWin:
                    RPCProcedure.arsonistWin();
                    break;
                case (byte)CustomRPC.GuesserShoot:
                    byte killerId = reader.ReadByte();
                    byte dyingTarget = reader.ReadByte();
                    byte guessedTarget = reader.ReadByte();
                    byte guessedRoleId = reader.ReadByte();
                    bool isSpecialRole = reader.ReadBoolean();
                    RPCProcedure.guesserShoot(killerId, dyingTarget, guessedTarget, guessedRoleId, isSpecialRole);
                    break;
                case (byte)CustomRPC.LawyerSetTarget:
                    RPCProcedure.lawyerSetTarget(reader.ReadByte()); 
                    break;
                case (byte)CustomRPC.LawyerPromotesToPursuer:
                    RPCProcedure.lawyerPromotesToPursuer();
                    break;
                case (byte)CustomRPC.LawyerWin:
                    RPCProcedure.lawyerWin();
                    break;
                case (byte)CustomRPC.SetBlanked:
                    var pid = reader.ReadByte();
                    var blankedValue = reader.ReadByte();
                    RPCProcedure.setBlanked(pid, blankedValue);
                    break;
                case (byte)CustomRPC.SetFutureSpelled:
                    RPCProcedure.setFutureSpelled(reader.ReadByte());
                    break;
                case (byte)CustomRPC.Bloody:
                    byte bloodyKiller = reader.ReadByte();
                    byte bloodyDead = reader.ReadByte();
                    RPCProcedure.bloody(bloodyKiller, bloodyDead);
                    break;
                case (byte)CustomRPC.SetFirstKill:
                    byte firstKill = reader.ReadByte();
                    RPCProcedure.setFirstKill(firstKill);
                    break;
                case (byte)CustomRPC.SetTiebreak:
                    RPCProcedure.setTiebreak();
                    break;
                case (byte)CustomRPC.SetInvisible:
                    byte invisiblePlayer = reader.ReadByte();
                    byte invisibleFlag = reader.ReadByte();
                    RPCProcedure.setInvisible(invisiblePlayer, invisibleFlag);
                    break;
                case (byte)CustomRPC.ThiefStealsRole:
                    byte thiefTargetId = reader.ReadByte();
                    RPCProcedure.thiefStealsRole(thiefTargetId);
                    break;
                //case (byte)CustomRPC.SetTrap:
                    //RPCProcedure.setTrap(reader.ReadBytesAndSize());
                    //break;
                /*case (byte)CustomRPC.TriggerTrap:
                    byte trappedPlayer = reader.ReadByte();
                    byte trapId = reader.ReadByte();
                    RPCProcedure.triggerTrap(trappedPlayer, trapId);
                    break;*/
                /*case (byte)CustomRPC.PlaceBomb:
                    RPCProcedure.placeBomb(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.DefuseBomb:
                    RPCProcedure.defuseBomb();
                    break;*/
                case (byte)CustomRPC.ShareGamemode:
                    byte gm = reader.ReadByte();
                    RPCProcedure.shareGamemode(gm);
                    break;
                case (byte)CustomRPC.StopStart:
                    RPCProcedure.stopStart(reader.ReadByte());
                    break;

                // Game mode
                case (byte)CustomRPC.SetGuesserGm:
                    byte guesserGm = reader.ReadByte();
                    RPCProcedure.setGuesserGm(guesserGm);
                    break;
                case (byte)CustomRPC.ShareTimer:
                    float punish = reader.ReadSingle();
                    RPCProcedure.shareTimer(punish);
                    break;
                case (byte)CustomRPC.HuntedShield:
                    byte huntedPlayer = reader.ReadByte();
                    RPCProcedure.huntedShield(huntedPlayer);
                    break;
                case (byte)CustomRPC.HuntedRewindTime:
                    byte rewindPlayer = reader.ReadByte();
                    RPCProcedure.huntedRewindTime(rewindPlayer);
                    break;
                case (byte)CustomRPC.ShareGhostInfo:
                    RPCProcedure.receiveGhostInfo(reader.ReadByte(), reader);
                    break;
            }
        }
    }
} 
