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
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using AmongUs.Data;
using AmongUs.GameOptions;

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
        //Shifter
    }

    enum CustomRPC
    {
        // Main Controls

        ResetVaribles = 60,
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
        SetGameStarting,
        ShareGamemode,
        StopStart,

        // Role functionality

        EngineerFixLights = 101,
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
        DeactivateDecel
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
            SpawnInMinigamePatch.reset();
            Props.clearProps();
            //Trap.clearTraps();
            Trap.clearAllTraps();
            clearAndReloadMapOptions();
            clearAndReloadRoles();
            clearGameHistory();
            setCustomButtonCooldowns();
            reloadPluginOptions();
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
                    option.updateSelection((int)selection);
                }
            } catch (Exception e) {
                TheOtherRolesPlugin.Logger.LogError("Error while deserializing options: " + e.Message);
            }
        }

        public static void forceEnd() {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
            {
                if (!player.Data.Role.IsImpostor)
                {
                    
                    GameData.Instance.GetPlayerById(player.PlayerId); // player.RemoveInfected(); (was removed in 2022.12.08, no idea if we ever need that part again, replaced by these 2 lines.) 
                    player.SetRole(RoleTypes.Crewmate);

                    player.MurderPlayer(player, MurderResultFlags.Succeeded);
                    player.Data.IsDead = true;
                }
            }
        }

        public static void shareGamemode(byte gm) {
            TORMapOptions.gameMode = (CustomGamemodes) gm;
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
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
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
                    case RoleId.Swapper:
                        Swapper.swapper = player;
                        break;
                    case RoleId.Seer:
                        Seer.seer = player;
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
                    case RoleId.TaskMaster:
                        TaskMaster.taskMaster = player;
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
            MessageReader reader = new MessageReader();
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
            if (target != null) target.Exiled();
        }

        public static void uncheckedSetTasks(byte playerId, byte[] taskTypeIds)
        {
            var player = Helpers.playerById(playerId);
            player.clearAllTasks();

            GameData.Instance.SetTasks(playerId, taskTypeIds);
        }

        public static void dynamicMapOption(byte mapId) {
           GameOptionsManager.Instance.currentNormalGameOptions.MapId = mapId;
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
            if (Medium.futureDeadBodies != null) {
                var deadBody = Medium.futureDeadBodies.Find(x => x.Item1.player.PlayerId == playerId).Item1;
                if (deadBody != null) deadBody.wasCleaned = true;
            }

            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++) {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId) {
                    UnityEngine.Object.Destroy(array[i].gameObject);
                }     
            }
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
            if(TimeMaster.timeMaster != null && TimeMaster.timeMaster == CachedPlayer.LocalPlayer.PlayerControl) {
                resetTimeMasterButton();
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (TimeMaster.timeMaster == null || CachedPlayer.LocalPlayer.PlayerControl == TimeMaster.timeMaster) return; // Time Master himself does not rewind

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
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
            
            bool isShieldedAndShow = Medic.shielded == CachedPlayer.LocalPlayer.PlayerControl && Medic.showAttemptToShielded;
            isShieldedAndShow = isShieldedAndShow && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting);  // Dont show attempt, if shield is not shown yet
            bool isMedicAndShow = Medic.medic == CachedPlayer.LocalPlayer.PlayerControl && Medic.showAttemptToMedic;

            if (isShieldedAndShow || isMedicAndShow || Helpers.shouldShowGhostInfo()) Helpers.showFlash(Palette.ImpostorRed, duration: 0.5f, ModTranslation.getString("medicInfo"));
        }

        public static void shifterShift(byte targetId)
        {
            PlayerControl oldShifter = Shifter.shifter;
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            if (!Shifter.isNeutral) Shifter.clearAndReload();

            // Suicide (exile) when impostor or impostor variants
            if (!Shifter.isNeutral && (player.Data.Role.IsImpostor || Helpers.isNeutral(player) || Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) || player == CreatedMadmate.createdMadmate))
            {
                oldShifter.Exiled();
                GameHistory.overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
                if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }
                return;
            }

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
            }

            // Shift role
            if (Mayor.mayor != null && Mayor.mayor == player)
                Mayor.mayor = oldShifter;
            if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player)
                Portalmaker.portalmaker = oldShifter;
            if (Engineer.engineer != null && Engineer.engineer == player)
                Engineer.engineer = oldShifter;
            if (Sheriff.sheriff != null && Sheriff.sheriff == player)
            {
                if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff) Sheriff.formerDeputy = oldShifter;  // Shifter also shifts info on promoted deputy (to get handcuffs)
                Sheriff.sheriff = oldShifter;
            }
            if (Deputy.deputy != null && Deputy.deputy == player)
                Deputy.deputy = oldShifter;
            if (Lighter.lighter != null && Lighter.lighter == player)
                Lighter.lighter = oldShifter;
            if (Detective.detective != null && Detective.detective == player)
                Detective.detective = oldShifter;
            if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player)
                TimeMaster.timeMaster = oldShifter;
            if (Medic.medic != null && Medic.medic == player)
                Medic.medic = oldShifter;
            if (Swapper.swapper != null && Swapper.swapper == player)
                Swapper.swapper = oldShifter;
            if (Seer.seer != null && Seer.seer == player)
                Seer.seer = oldShifter;
            if (Hacker.hacker != null && Hacker.hacker == player)
                Hacker.hacker = oldShifter;
            if (Tracker.tracker != null && Tracker.tracker == player)
                Tracker.tracker = oldShifter;
            if (Snitch.snitch != null && Snitch.snitch == player)
                Snitch.snitch = oldShifter;
            if (Spy.spy != null && Spy.spy == player)
                Spy.spy = oldShifter;
            if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player)
                SecurityGuard.securityGuard = oldShifter;
            if (Guesser.niceGuesser != null && Guesser.niceGuesser == player)
                Guesser.niceGuesser = oldShifter;
            if (Bait.bait != null && Bait.bait == player)
                Bait.bait = oldShifter;
            if (Medium.medium != null && Medium.medium == player)
                Medium.medium = oldShifter;
            if (Watcher.nicewatcher != null && Watcher.nicewatcher == player)
                Watcher.nicewatcher = oldShifter;
            if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == player)
                FortuneTeller.fortuneTeller = oldShifter;
            if (Sherlock.sherlock != null && Sherlock.sherlock == player)
                Sherlock.sherlock = oldShifter;
            if (Sprinter.sprinter != null && Sprinter.sprinter == player)
                Sprinter.sprinter = oldShifter;
            if (Veteran.veteran != null && Veteran.veteran == player)
                Veteran.veteran = oldShifter;
            if (Yasuna.yasuna != null && Yasuna.yasuna == player)
                Yasuna.yasuna = oldShifter;
            if (player == TaskMaster.taskMaster)
                TaskMaster.taskMaster = oldShifter;
            if (Teleporter.teleporter != null && Teleporter.teleporter == player)
                Teleporter.teleporter = oldShifter;
            if (Prophet.prophet != null && Prophet.prophet == player)
                Prophet.prophet = oldShifter;

            if (player == Godfather.godfather) Godfather.godfather = oldShifter;
            if (player == Mafioso.mafioso) Mafioso.mafioso = oldShifter;
            if (player == Janitor.janitor) Janitor.janitor = oldShifter;
            if (player == Morphling.morphling) Morphling.morphling = oldShifter;
            if (player == Trickster.trickster) Trickster.trickster = oldShifter;
            if (player == Cleaner.cleaner) Cleaner.cleaner = oldShifter;
            if (player == Ninja.ninja) Ninja.ninja = oldShifter;
            if (player == NekoKabocha.nekoKabocha) NekoKabocha.nekoKabocha = oldShifter;
            if (player == Assassin.assassin) Assassin.assassin = oldShifter;
            if (player == SerialKiller.serialKiller) SerialKiller.serialKiller = oldShifter;
            if (player == EvilTracker.evilTracker) EvilTracker.evilTracker = oldShifter;
            if (player == EvilHacker.evilHacker) EvilHacker.evilHacker = oldShifter;
            if (player == Witch.witch) Witch.witch = oldShifter;
            if (player == Camouflager.camouflager) Camouflager.camouflager = oldShifter;
            if (player == Guesser.evilGuesser) Guesser.evilGuesser = oldShifter;
            if (player == Eraser.eraser) Eraser.eraser = oldShifter;
            if (player == Warlock.warlock) Warlock.warlock = oldShifter;
            if (player == BountyHunter.bountyHunter) BountyHunter.bountyHunter = oldShifter;
            if (player == Vampire.vampire) Vampire.vampire = oldShifter;
            if (player == CreatedMadmate.createdMadmate) CreatedMadmate.createdMadmate = oldShifter;
            if (player == Blackmailer.blackmailer) Blackmailer.blackmailer = oldShifter;
            if (player == MimicK.mimicK)
            {
                MimicK.mimicK = oldShifter;
                MimicK.name = oldShifter.Data.PlayerName;
            }
            if (player == MimicA.mimicA) MimicA.mimicA = oldShifter;
            if (player == BomberA.bomberA) BomberA.bomberA = oldShifter;
            if (player == BomberB.bomberB) BomberB.bomberB = oldShifter;
            if (player == Trapper.trapper) Trapper.trapper = oldShifter;
            if (player == Jester.jester) Jester.jester = oldShifter;
            if (player == Arsonist.arsonist) Arsonist.arsonist = oldShifter;
            if (player == Opportunist.opportunist) Opportunist.opportunist = oldShifter;
            if (player == Moriarty.moriarty) Moriarty.moriarty = oldShifter;
            if (player == PlagueDoctor.plagueDoctor) PlagueDoctor.plagueDoctor = oldShifter;
            if (player == Thief.thief) Thief.thief = oldShifter;
            if (player == Pursuer.pursuer) Pursuer.pursuer = oldShifter;
            if (player == Vulture.vulture) Vulture.vulture = oldShifter;
            if (player == Jackal.jackal) Jackal.jackal = oldShifter;
            if (player == Sidekick.sidekick) Sidekick.sidekick = oldShifter;
            if (player == Lawyer.lawyer) Lawyer.lawyer = oldShifter;
            if (player == Akujo.akujo) Akujo.akujo = oldShifter;
            if (player == Cupid.cupid) Cupid.cupid = oldShifter;

            if (Lawyer.lawyer != null && Lawyer.target == player)
            {
                Lawyer.target = oldShifter;
            }

            if (Shifter.isNeutral)
            {
                Shifter.shifter = player;
                Shifter.pastShifters.Add(oldShifter.PlayerId);
                if (player.Data.Role.IsImpostor)
                {
                    FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                    FastDestroyableSingleton<RoleManager>.Instance.SetRole(oldShifter, RoleTypes.Impostor);
                }
            }

            // Set cooldowns to max for both players
            if (CachedPlayer.LocalPlayer.PlayerControl == oldShifter || CachedPlayer.LocalPlayer.PlayerControl == player)
                CustomButton.ResetAllCooldowns();
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
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }
                return;
            }
            
            Shifter.shiftRole(oldShifter, player);

            // Set cooldowns to max for both players
            if (CachedPlayer.LocalPlayer.PlayerControl == oldShifter || CachedPlayer.LocalPlayer.PlayerControl == player)
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
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
                player.setLook("", 6, "", "", "", "");
        }

        public static void vampireSetBitten(byte targetId, byte performReset) {
            if (performReset != 0) {
                Vampire.bitten = null;
                return;
            }

            if (Vampire.vampire == null) return;
            foreach (PlayerControl player in CachedPlayer.AllPlayers) {
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
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
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
                if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
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
                Vector2 pos = new Vector2(x, y);
                if (!MapBehaviourPatch.realTasks.ContainsKey(playerId)) MapBehaviourPatch.realTasks[playerId] = new Il2CppSystem.Collections.Generic.List<Vector2>();
                MapBehaviourPatch.realTasks[playerId].Add(pos);
            }
        }

        // Hmm... Lots of bugs huh?
        public static void fortuneTellerUsedDivine(byte fortuneTellerId, byte targetId)
        {
            PlayerControl fortuneTeller = Helpers.playerById(fortuneTellerId);
            PlayerControl target = Helpers.playerById(targetId);
            if (target == null) return;
            if (target.Data.IsDead) return;

            // インポスターの場合は占い師の位置に矢印を表示
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                FortuneTeller.fortuneTellerMessage(ModTranslation.getString("fortuneTellerDivinedSomeone"), 7f, Color.white);
                FortuneTeller.setDivinedFlag(fortuneTeller, true);
            }

            // The ghosts will also receive a message
            /*if (CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
            {
                FortuneTeller.fortuneTellerMessage($"{target.Data.PlayerName} Was Just Divined", 7f, Color.white);
            }*/
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
                Sheriff.formerDeputy = Deputy.deputy;
                Deputy.deputy = null;
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
                if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                if (wasSpy || wasImpostor) Sidekick.wasTeamRed = true;
                Sidekick.wasSpy = wasSpy;
                Sidekick.wasImpostor = wasImpostor;
                if (player == CachedPlayer.LocalPlayer.PlayerControl) SoundEffectsManager.play("jackalSidekick");
                if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.getBool() && !HandleGuesser.isGuesser(targetId))
                    setGuesserGm(targetId);
            }
            Jackal.canCreateSidekick = false;
        }

        public static void sidekickPromotes() {
            Jackal.removeCurrentJackal();
            Jackal.jackal = Sidekick.sidekick;
            Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
            Jackal.wasTeamRed = Sidekick.wasTeamRed;
            Jackal.wasSpy = Sidekick.wasSpy;
            Jackal.wasImpostor = Sidekick.wasImpostor;
            Sidekick.clearAndReload();
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
            //if (player == Bomber.bomber) Bomber.clearAndReload();

            // Other roles
            if (player == Jester.jester) Jester.clearAndReload();
            if (player == Arsonist.arsonist) Arsonist.clearAndReload();
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
            if (player == PlagueDoctor.plagueDoctor) PlagueDoctor.clearAndReload();
            if (player == Cupid.cupid) Cupid.clearAndReload(false);

            // Always remove the Madmate
            if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId)) Madmate.madmate.RemoveAll(x => x.PlayerId == player.PlayerId);

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
            }
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

        public static void placeAccel(byte id)
        {
            new Props.AccelTrap(Props.propPos[id]);
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
            if (CachedPlayer.LocalPlayer.PlayerControl == player) SoundEffectsManager.play("triggerDeceleration");
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
            if (CachedPlayer.LocalPlayer.PlayerControl == player) SoundEffectsManager.play("untriggerDeceleration");
        }

        public static void activateAccel(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (Props.AccelTrap.acceled.ContainsKey(player)) Props.AccelTrap.acceled.Remove(player);
            Props.AccelTrap.acceled.Add(player, DateTime.UtcNow);
            if (CachedPlayer.LocalPlayer.PlayerControl == player) SoundEffectsManager.play("jekyllAndHydeDrug");
        }

        public static void deactivateAccel(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);

            if (Props.AccelTrap.acceled.ContainsKey(player)) Props.AccelTrap.acceled.Remove(player);
            if (CachedPlayer.LocalPlayer.PlayerControl == player) SoundEffectsManager.play("jekyllAndHydeDrug");
        }

        public static void placeAssassinTrace(byte[] buff) {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new AssassinTrace(position, Assassin.traceTime);
            if (CachedPlayer.LocalPlayer.PlayerControl != Assassin.assassin)
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
            bool canSee = CachedPlayer.LocalPlayer.Data.Role.IsImpostor || CachedPlayer.LocalPlayer.Data.IsDead;
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
            if(Helpers.hasImpVision(GameData.Instance.GetPlayerById(CachedPlayer.LocalPlayer.PlayerId))) {
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
            GameHistory.overrideDeathReasonAndKiller(Helpers.playerById(targetId), DeadPlayer.CustomDeathReason.Revenge, killer: NekoKabocha.nekoKabocha);
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
            target1.NetTransform.RpcSnapTo(new Vector2(target2.GetTruePosition().x, target2.GetTruePosition().y + 0.3636f));
            target1.cosmetics.currentBodySprite.BodySprite.flipX = target2.cosmetics.currentBodySprite.BodySprite.flipX;
            target2.NetTransform.RpcSnapTo(new Vector2(targetPosition.x, targetPosition.y + 0.3636f));
            target2.cosmetics.currentBodySprite.BodySprite.flipX = TempFacing;

            if (CachedPlayer.LocalPlayer.PlayerControl == target1 || CachedPlayer.LocalPlayer.PlayerControl == target2)
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
            }
        }

        public static void setCupidShield(byte targetId)
        {
            Cupid.shielded = Helpers.playerById(targetId);
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
            if (CachedPlayer.LocalPlayer.PlayerControl == Veteran.veteran)
            {
                PlayerControl player = Helpers.playerById(targetId);
                Helpers.checkMurderAttemptAndKill(Veteran.veteran, player);
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
            if (!Yasuna.isYasuna(CachedPlayer.LocalPlayer.PlayerControl.PlayerId)) return;
            PlayerControl target = Helpers.playerById(Yasuna.specialVoteTargetPlayerId);
            if (target == null) return;
            MeetingHud.Instance.CmdCastVote(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, target.PlayerId);
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
            GameData.PlayerInfo player = GameData.Instance.GetPlayerById(playerId);
            if (player == null)
                return;

            if (taskTypeIds != null && taskTypeIds.Length > 0)
            {
                player.Object.clearAllTasks();
                player.Tasks = new Il2CppSystem.Collections.Generic.List<GameData.TaskInfo>(taskTypeIds.Length);
                for (int i = 0; i < taskTypeIds.Length; i++)
                {
                    player.Tasks.Add(new GameData.TaskInfo(taskTypeIds[i], (uint)i));
                    player.Tasks[i].Id = (uint)i;
                }
                for (int i = 0; i < player.Tasks.Count; i++)
                {
                    GameData.TaskInfo taskInfo = player.Tasks[i];
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
            if (Moriarty.numberToWin == Moriarty.counter) Moriarty.triggerMoriartyWin = true;
        }

        public static void plantBomb(byte playerId)
        {
            var p = Helpers.playerById(playerId);
            if (CachedPlayer.LocalPlayer.PlayerControl == BomberA.bomberA) BomberB.bombTarget = p;
            if (CachedPlayer.LocalPlayer.PlayerControl == BomberB.bomberB) BomberA.bombTarget = p;
        }

        public static void releaseBomb(byte killer, byte target)
        {
            // 同時押しでダブルキルが発生するのを防止するためにBomberAで一度受け取ってから実行する
            if (CachedPlayer.LocalPlayer.PlayerControl == BomberA.bomberA)
            {
                if (BomberA.bombTarget != null && BomberB.bombTarget != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.BomberKill, Hazel.SendOption.Reliable, -1);
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


            if (CachedPlayer.LocalPlayer.PlayerControl == SecurityGuard.securityGuard) {
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
            if (CachedPlayer.LocalPlayer.PlayerControl == SecurityGuard.securityGuard) {
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
            foreach (PlayerControl p in CachedPlayer.AllPlayers) {
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

            if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId && client != null) {
                    Transform playerInfoTransform = client.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
            }
        }

        public static void guesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleId) {
            PlayerControl killer = Helpers.playerById(killerId);
            PlayerControl dyingTarget = Helpers.playerById(dyingTargetId);
            PlayerControl dyingMimicPartner;
            PlayerControl dyingBomberPartner;
            PlayerControl dyingAkujoPartner;
            PlayerControl dyingCupidLover;
            byte NekoKabochaKillerId = byte.MaxValue;
            if (dyingTarget == null ) return;
            bool revengeFlag = (NekoKabocha.revengeCrew && (!Helpers.isNeutral(killer) && !killer.Data.Role.IsImpostor)) ||
                    (NekoKabocha.revengeNeutral && Helpers.isNeutral(killer)) ||
                    (NekoKabocha.revengeImpostor && killer.Data.Role.IsImpostor);
            if (dyingTarget == NekoKabocha.nekoKabocha)
            {
                NekoKabocha.meetingKiller = killer;
                NekoKabochaKillerId = killer.PlayerId;
                if (revengeFlag)
                {
                    killer.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(killer, DeadPlayer.CustomDeathReason.Revenge, NekoKabocha.nekoKabocha);
                }
            }
            if ((dyingTarget == MimicK.mimicK || dyingTarget == MimicA.mimicA) && MimicK.ifOneDiesBothDie)
            {
                dyingMimicPartner = dyingTarget == MimicK.mimicK ? MimicA.mimicA : MimicK.mimicK;
                //dyingMimicPartner.Exiled();
                //GameHistory.overrideDeathReasonAndKiller(dyingMimicPartner, DeadPlayer.CustomDeathReason.Suicide);
            }
            else dyingMimicPartner = null;

            if ((dyingTarget == BomberA.bomberA || dyingTarget == BomberB.bomberB) && BomberA.ifOneDiesBothDie)
            {
                dyingBomberPartner = dyingTarget == BomberA.bomberA ? BomberB.bomberB : BomberA.bomberA;
                //dyingBomberPartner.Exiled();
                //GameHistory.overrideDeathReasonAndKiller(dyingBomberPartner, DeadPlayer.CustomDeathReason.Suicide);
            }
            else dyingBomberPartner = null;

            if ((Akujo.akujo != null && dyingTarget == Akujo.akujo) || (Akujo.honmei != null && dyingTarget == Akujo.honmei)) dyingAkujoPartner = dyingTarget == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
            else dyingAkujoPartner = null;

            if (Cupid.lovers1 != null && Cupid.lovers2 != null && (dyingTarget == Cupid.lovers1 || dyingTarget == Cupid.lovers2)) dyingCupidLover = dyingTarget == Cupid.lovers1 ? Cupid.lovers2 : Cupid.lovers1;
            else dyingCupidLover = null;

            PlayerControl cupid = Cupid.cupid != null && !Cupid.cupid.Data.IsDead && dyingCupidLover != null ? Cupid.cupid : null;

            if (Lawyer.target != null && dyingTarget == Lawyer.target) Lawyer.targetWasGuessed = true;  // Lawyer shouldn't be exiled with the client for guesses
            if (Yasuna.yasuna != null && dyingTarget == Yasuna.yasuna) Yasuna.specialVoteTargetPlayerId = byte.MaxValue;
            if (Yasuna.yasuna != null && dyingTarget.PlayerId == Yasuna.specialVoteTargetPlayerId) Yasuna.specialVoteTargetPlayerId = byte.MaxValue;
            PlayerControl dyingLoverPartner = Lovers.bothDie ? dyingTarget.getPartner() : null; // Lover check
            if (Lawyer.target != null && dyingLoverPartner == Lawyer.target) Lawyer.targetWasGuessed = true;  // Lawyer shouldn't be exiled with the client for guesses

            PlayerControl guesser = Helpers.playerById(killerId);
            if (Thief.thief != null && Thief.thief.PlayerId == killerId && Thief.canStealWithGuess) {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                if (!Thief.thief.Data.IsDead && !Thief.isFailedThiefKill(dyingTarget, guesser, roleInfo)) {
                    RPCProcedure.thiefStealsRole(dyingTarget.PlayerId);
                }
            }

            dyingTarget.Exiled();
            GameHistory.overrideDeathReasonAndKiller(dyingTarget, DeadPlayer.CustomDeathReason.Guess, guesser);

            byte partnerId = dyingLoverPartner != null ? dyingLoverPartner.PlayerId : dyingTargetId;
            byte mimicPartnerId = dyingMimicPartner != null ? dyingMimicPartner.PlayerId: byte.MaxValue;
            byte bomberPartnerId = dyingBomberPartner != null ? dyingBomberPartner.PlayerId : byte.MaxValue;
            byte akujoPartnerId = dyingAkujoPartner != null ? dyingAkujoPartner.PlayerId : byte.MaxValue;
            byte cupidLoverId = dyingCupidLover != null ? dyingCupidLover.PlayerId : byte.MaxValue;
            byte cupidId = cupid != null ? Cupid.cupid.PlayerId : byte.MaxValue;
            //byte nKkillerId = (NekoKabocha.meetingKiller != null && revengeFlag) ? NekoKabocha.meetingKiller.PlayerId : dyingTargetId;

            HandleGuesser.remainingShots(killerId, true);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
            if (MeetingHud.Instance) {
                MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, dyingTargetId);
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates) {
                    if (pva.TargetPlayerId == dyingTargetId || pva.TargetPlayerId == partnerId || pva.TargetPlayerId == mimicPartnerId || (pva.TargetPlayerId == NekoKabochaKillerId && revengeFlag) || pva.TargetPlayerId == bomberPartnerId || pva.TargetPlayerId == akujoPartnerId || pva.TargetPlayerId == cupidLoverId || pva.TargetPlayerId == cupidId) {
                        pva.SetDead(pva.DidReport, true);
                        pva.Overlay.gameObject.SetActive(true);
                    }

                    //Give players back their vote if target is shot dead
                    if (pva.VotedFor != dyingTargetId || pva.VotedFor != partnerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();

                }
                if (AmongUsClient.Instance.AmHost) 
                    MeetingHud.Instance.CheckForEndVoting();
            }
            if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
                if (CachedPlayer.LocalPlayer.PlayerControl == dyingTarget) {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                } else if (dyingLoverPartner != null && CachedPlayer.LocalPlayer.PlayerControl == dyingLoverPartner) {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data, dyingLoverPartner.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
                
                else if (CachedPlayer.LocalPlayer.PlayerControl == NekoKabocha.meetingKiller && revengeFlag)
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(NekoKabocha.nekoKabocha.Data, killer.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }

                else if (dyingMimicPartner != null && CachedPlayer.LocalPlayer.PlayerControl == dyingMimicPartner)
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingMimicPartner.Data, dyingMimicPartner.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }

                else if (dyingAkujoPartner != null && CachedPlayer.LocalPlayer.PlayerControl == dyingAkujoPartner)
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingAkujoPartner.Data, dyingAkujoPartner.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }

                else if (dyingCupidLover != null && CachedPlayer.LocalPlayer.PlayerControl == dyingCupidLover)
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingCupidLover.Data, dyingCupidLover.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
                
                if (cupid != null && CachedPlayer.LocalPlayer.PlayerControl == cupid)
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(cupid.Data, cupid.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }

            // remove shoot button from targets for all guessers and close their guesserUI
            if (GuesserGM.isGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer != guesser && !PlayerControl.LocalPlayer.Data.IsDead && GuesserGM.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 && MeetingHud.Instance) {
                MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                if (dyingLoverPartner != null)
                    MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingLoverPartner.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                if (MeetingHudPatch.guesserUI != null && MeetingHudPatch.guesserUIExitButton != null) {
                    if (MeetingHudPatch.guesserCurrentTarget == dyingTarget.PlayerId)
                        MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                    else if (dyingLoverPartner != null && MeetingHudPatch.guesserCurrentTarget == dyingLoverPartner.PlayerId)
                        MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
            }


            PlayerControl guessedTarget = Helpers.playerById(guessedTargetId);
            if (CachedPlayer.LocalPlayer.Data.IsDead && guessedTarget != null && guesser != null) {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                string msg = string.Format(ModTranslation.getString("guesserGuessChat"), guesser.Data.PlayerName, roleInfo?.name ?? "", guessedTarget.Data.PlayerName);
                if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(guesser, msg);
                //if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                    //FastDestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
            }
            if (!CachedPlayer.LocalPlayer.Data.IsDead && (CachedPlayer.LocalPlayer.PlayerControl == Watcher.nicewatcher || CachedPlayer.LocalPlayer.PlayerControl == Watcher.evilwatcher) && guessedTarget != null && guesser != null && CustomOptionHolder.watcherSeeGuesses.getBool())
            {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                string msg = string.Format(ModTranslation.getString("watcherGuessChat"), roleInfo?.name ?? "", guessedTarget.Data.PlayerName);
                if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == Watcher.nicewatcher)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Watcher.nicewatcher, msg);
                    }
                    else
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Watcher.evilwatcher, msg);
                    }
                }
                //if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                    //FastDestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
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
            if (target == Sheriff.sheriff) Sheriff.sheriff = thief;
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
                Moriarty.moriarty = thief;
                Moriarty.formerMoriarty = target;
            }
            if (target == JekyllAndHyde.jekyllAndHyde)
            {
                JekyllAndHyde.jekyllAndHyde = thief;
                JekyllAndHyde.formerJekyllAndHyde = target;
            }
            if (target == Guesser.evilGuesser) Guesser.evilGuesser = thief;
            if (target == Watcher.evilwatcher) Watcher.evilwatcher = thief;
            if (target == Godfather.godfather) Godfather.godfather = thief;
            if (target == Mafioso.mafioso) Mafioso.mafioso = thief;
            if (target == Janitor.janitor) Janitor.janitor = thief;
            if (target == Morphling.morphling) Morphling.morphling = thief;
            if (target == Camouflager.camouflager) Camouflager.camouflager = thief;
            if (target == Vampire.vampire) Vampire.vampire = thief;
            if (target == Eraser.eraser) Eraser.eraser = thief;
            if (target == Trickster.trickster) Trickster.trickster = thief;
            if (target == Cleaner.cleaner) Cleaner.cleaner = thief;
            if (target == Warlock.warlock) Warlock.warlock = thief;
            if (target == BountyHunter.bountyHunter) BountyHunter.bountyHunter = thief;
            if (target == Ninja.ninja) Ninja.ninja = thief;
            if (target == EvilTracker.evilTracker) EvilTracker.evilTracker = thief;
            if (target == NekoKabocha.nekoKabocha && !NekoKabocha.revengeNeutral) NekoKabocha.nekoKabocha = thief;
            if (target == SerialKiller.serialKiller) SerialKiller.serialKiller = thief;
            if (target == Swapper.swapper && target.Data.Role.IsImpostor) Swapper.swapper = thief;
            if (target == Undertaker.undertaker) Undertaker.undertaker = thief;
            if (target == EvilHacker.evilHacker) EvilHacker.evilHacker = thief;
            if (target == Trapper.trapper) Trapper.trapper = thief;
            if (target == Blackmailer.blackmailer) Blackmailer.blackmailer = thief;
            if (target == Witch.witch) {
                Witch.witch = thief;
                if (MeetingHud.Instance) 
                    if (Witch.witchVoteSavesTargets)  // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                        Witch.futureSpelled = new();
                else  // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                    Witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
            if (target == Assassin.assassin) Assassin.assassin = thief;
            //if (target == Bomber.bomber) Bomber.bomber = thief;
            if (target.Data.Role.IsImpostor) {
                RoleManager.Instance.SetRole(Thief.thief, RoleTypes.Impostor);
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(Thief.thief.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }
            if (Lawyer.lawyer != null && target == Lawyer.target)
                Lawyer.target = thief;
            if (Thief.thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
            Thief.clearAndReload();
            Thief.formerThief = thief;  // After clearAndReload, else it would get reset...
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
            if (playerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) {
                resetHuntedRewindButton();
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Hunted.shieldRewindTime, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor) return; // only rewind hunter

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
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
                    	FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, mediumInfo);
                    break;
                case GhostInfoTypes.DetectiveOrMedicInfo:
                    string detectiveInfo = reader.ReadString();
                    if (Helpers.shouldShowGhostInfo())
		    	        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, detectiveInfo);
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

        public static void shareRoom(byte playerId, byte roomId) {
            if (Snitch.playerRoomMap.ContainsKey(playerId)) Snitch.playerRoomMap[playerId] = roomId;
            else Snitch.playerRoomMap.Add(playerId, roomId);
        }
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
                case (byte)CustomRPC.AkujoSetHonmei:
                    RPCProcedure.akujoSetHonmei(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.AkujoSetKeep:
                    RPCProcedure.akujoSetKeep(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.AkujoSuicide:
                    RPCProcedure.akujoSuicide(reader.ReadByte());
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
                    if (AmongUsClient.Instance.AmHost && Yasuna.isYasuna(id))
                    {
                        int clientId = Helpers.GetClientId(Yasuna.yasuna);
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.YasunaSpecialVote_DoCastVote, Hazel.SendOption.Reliable, clientId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
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
                    RPCProcedure.guesserShoot(killerId, dyingTarget, guessedTarget, guessedRoleId);
                    break;
                case (byte)CustomRPC.LawyerSetTarget:
                    RPCProcedure.lawyerSetTarget(reader.ReadByte()); 
                    break;
                case (byte)CustomRPC.LawyerPromotesToPursuer:
                    RPCProcedure.lawyerPromotesToPursuer();
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


                case (byte)CustomRPC.ShareRoom:
                    byte roomPlayer = reader.ReadByte();
                    byte roomId = reader.ReadByte();
                    RPCProcedure.shareRoom(roomPlayer, roomId);
                    break;
            }
        }
    }
} 
