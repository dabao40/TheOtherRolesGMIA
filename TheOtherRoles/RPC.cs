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
using TheOtherRoles.MetaContext;
using TheOtherRoles.Roles;

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
        Zephyr,
        Blackmailer,
        Opportunist,
        Yoyo,
        Doomsayer,
        Kataomoi,
        Busker,
        Noisemaker,
        Archaeologist,
        SchrodingersCat,
        Madmate,
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
        RecordStatistics,
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
        ArchaeologistExcavate,
        ImpostorPromotesToLastImpostor,
        DoomsayerObserve,
        EventKick,
        DraftModePickOrder,
        DraftModePick,
        ShareAchievement,
        SherlockReceiveDetect,
        JesterWin,
        SetLovers,
        ZephyrBlowCannon,
        ZephyrCheckCannon
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
            CustomButton.ReloadHotkeys();
            reloadPluginOptions();
            Helpers.toggleZoom(reset : true);
            GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
            SurveillanceMinigamePatch.nightVisionOverlays = null;
            EventUtility.clearAndReload();
            HudManagerUpdate.CloseSummary();
            RoleDraft.isRunning = false;
        }

    public static void HandleShareOptions(byte numberOfOptions, MessageReader reader) {            
            try {
                for (int i = 0; i < numberOfOptions; i++) {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption option = CustomOption.options.First(option => option.id == (int)optionId);
                    option.updateSelection((int)selection, i == numberOfOptions - 1);
                }
                HelpMenu.OnUpdateOptions();
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
            if (LobbyViewSettingsPatch.currentButtons != null)
                LobbyViewSettingsPatch.currentButtons?.ForEach(x => { if (x != null && x.gameObject != null) x?.gameObject?.Destroy(); });
            LobbyViewSettingsPatch.currentButtons?.Clear();
            LobbyViewSettingsPatch.currentButtonTypes?.Clear();
        }

        public static void stopStart(byte playerId)
        {
            if (!CustomOptionHolder.anyPlayerCanStopStart.getBool())
                return;
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            if (AmongUsClient.Instance.AmHost)
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
            PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().DoIf(
                x => x.PlayerId == playerId,
                x => x.setRole((RoleId)roleId)
            );
        }

        public static void setModifier(byte modifierId, byte playerId, byte flag) {
            PlayerControl player = Helpers.playerById(playerId); 
            switch ((RoleId)modifierId) {
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

        public static void setLovers(byte playerId1, byte playerId2)
        {
            Lovers.addCouple(Helpers.playerById(playerId1), Helpers.playerById(playerId2));
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
                    Role.allRoles.DoIf(x => x.player == PlayerControl.LocalPlayer, x => x.OnFinishShipStatusBegin());
                    ShipStatusPatch.commonTasks.Clear();
                    foreach (var task in PlayerControl.LocalPlayer.myTasks) {
                        if (ShipStatus.Instance.CommonTasks.Any(x => x.TaskType == task.TaskType)) {
                            ShipStatusPatch.commonTasks.Add(task.TaskType);
                            TheOtherRolesPlugin.Logger.LogMessage($"Added {task.TaskType.ToString()} for common task");
                        }
                    }

                    if (Madmate.hasTasks && Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId)) {
                        PlayerControl.LocalPlayer.generateAndAssignTasks(Madmate.commonTasks, Madmate.shortTasks, Madmate.longTasks);
                    }

                    PlayerControl.AllPlayerControls.GetFastEnumerator().DoIf(x => !TORGameManager.Instance.RoleHistory.Any(history => history.PlayerId == x.PlayerId),
                        x => TORGameManager.Instance.RecordRoleHistory(x));

                    ShipStatus.Instance.AllCameras.Do
                    (
                        x =>
                        {
                            if (x.CamName == "South") x.NewName = StringNames.CamSouth;
                            else if (x.CamName == "Central") x.NewName = StringNames.CamCentral;
                            else if (x.CamName == "Northeast") x.NewName = StringNames.CamNortheast;
                            else if (x.CamName == "Northwest") x.NewName = StringNames.CamNorthwest;
                            else if (x.CamName == "Southwest") x.NewName = StringNames.CamSouthwest;
                            else if (x.CamName == "East") x.NewName = StringNames.CamEast;
                        }
                    );
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
            if (Helpers.shouldShowGhostInfo()) {
                Helpers.showFlash(Engineer.color, 0.5f, ModTranslation.getString("engineerInfo")); ;
            }
        }

        public static void cleanBody(byte playerId, byte cleaningPlayerId) {
            if (Medium.futureDeadBodies != null && !Busker.players.Any(x => x.player && x.player.PlayerId == playerId && !x.pseudocideComplete)) {
                var deadBody = Medium.futureDeadBodies.Find(x => x.Item1.player.PlayerId == playerId).Item1;
                if (deadBody != null) deadBody.wasCleaned = true;
            }
            PlayerControl player = Helpers.playerById(playerId);
            PlayerControl cleanPlayer = Helpers.playerById(cleaningPlayerId);

            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++) {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId) {
                    UnityEngine.Object.Destroy(array[i].gameObject);
                }     
            }
            TORGameManager.Instance?.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.CleanBody, cleaningPlayerId, 1 << playerId)
            {
                RelatedTag =
                cleanPlayer.isRole(RoleId.Vulture) ? EventDetail.Eat : EventDetail.Clean
            });
            if (cleanPlayer.isRole(RoleId.Vulture)) {
                var vulture = Vulture.getRole(cleanPlayer);
                vulture.eatenBodies++;
                if (vulture.eatenBodies == Vulture.vultureNumberToWin) {
                    vulture.triggerVultureWin = true;
                }
            }
        }

        public static void timeMasterRewindTime(byte playerId) {
            PlayerControl player = Helpers.playerById(playerId);
            var timeMaster = TimeMaster.getRole(player);
            timeMaster.shieldActive = false; // Shield is no longer active when rewinding
            SoundEffectsManager.stop("timemasterShield");  // Shield sound stopped when rewinding
            if(PlayerControl.LocalPlayer == player) {
                resetTimeMasterButton();
                _ = new StaticAchievementToken("timeMaster.challenge");
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (!TimeMaster.exists || PlayerControl.LocalPlayer == player) return; // Time Master himself does not rewind

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            PlayerControl.LocalPlayer.moveable = false;
        }

        public static void timeMasterShield(byte playerId) {
            var timeMaster = TimeMaster.getRole(Helpers.playerById(playerId));
            timeMaster.shieldActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration, new Action<float>((p) => {
                if (p == 1f) timeMaster.shieldActive = false;
            })));
        }

        public static void medicSetShielded(byte shieldedId, byte medicId) {
            var medic = Medic.getRole(Helpers.playerById(medicId));
            medic.usedShield = true;
            medic.shielded = Helpers.playerById(shieldedId);
            medic.futureShielded = null;
        }

        public static void shieldedMurderAttempt(byte medicId) {
            var medic = Medic.getRole(Helpers.playerById(medicId));
            if (medic == null || medic.shielded == null) return;
            
            bool isShieldedAndShow = medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded;
            isShieldedAndShow = isShieldedAndShow && (medic.meetingAfterShielding || !Medic.showShieldAfterMeeting);  // Dont show attempt, if shield is not shown yet
            bool isMedicAndShow = medic.player == PlayerControl.LocalPlayer && Medic.showAttemptToMedic;

            if (isShieldedAndShow || isMedicAndShow || Helpers.shouldShowGhostInfo()) Helpers.showFlash(Palette.ImpostorRed, duration: 0.5f, ModTranslation.getString("medicInfo"));
        }

        public static void shifterShift(byte targetId)
        {
            PlayerControl oldShifter = Shifter.allPlayers.FirstOrDefault();
            PlayerControl player = Helpers.playerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            if (PlayerControl.LocalPlayer == oldShifter && Shifter.isNeutral) _ = new StaticAchievementToken("corruptedShifter.common1");

            // Suicide (exile) when impostor or impostor variants
            if (!Shifter.isNeutral && (player.Data.Role.IsImpostor || Helpers.isNeutral(player) || Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) || CreatedMadmate.createdMadmate.Any(x => x.PlayerId == player.PlayerId)))
            {
                if (!oldShifter.Data.IsDead)
                {
                    oldShifter.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
                }
                if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost)
                {
                    foreach (var lawyer in Lawyer.allPlayers)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                        writer.Write(lawyer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.lawyerPromotesToPursuer(lawyer.PlayerId);
                    }
                }
                if (PlayerControl.LocalPlayer == oldShifter) {
                    Shifter.niceShifterAcTokenChallenge.Value.shiftId = targetId;
                }
                return;
            }

            if (!Shifter.isNeutral) {
                if (PlayerControl.LocalPlayer == oldShifter)
                    _ = new StaticAchievementToken("niceShifter.common1");
            }
            Shifter.eraseRole(oldShifter);

            // Switch shield
            if (Shifter.shiftsMedicShield)
            {
                if (Medic.IsShielded(player)) {
                    foreach (var medic in Medic.GetMedic(player)) {
                         medic.shielded = oldShifter;
                    }
                }
                else if (Medic.IsShielded(oldShifter)){
                    foreach (var medic in Medic.GetMedic(oldShifter)) {
                         medic.shielded = player;
                    }
                }
            }

            if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId))
            {
                Madmate.madmate.Add(oldShifter);
                Madmate.madmate.Remove(player);
            }
            if (CreatedMadmate.createdMadmate.Any(x => x.PlayerId == player.PlayerId))
            {
                CreatedMadmate.createdMadmate.Add(oldShifter);
                CreatedMadmate.createdMadmate.Remove(player);
            }

            if (Shifter.shiftModifiers)
            {
                Lovers.swapLovers(oldShifter, player);
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

            if ((player.Data.Tasks == null || player.Data.Tasks?.Count == 0 || player.isRole(RoleId.Fox)) && !player.Data.IsDead && PlayerControl.LocalPlayer == player)
                player.generateNormalTasks();

            player.swapRoles(oldShifter);

            if (Lawyer.target == player) Lawyer.target = oldShifter;
            if (Kataomoi.target == player)
                Kataomoi.target = oldShifter;

            if (Shifter.isNeutral)
            {
                player.setRole(RoleId.Shifter);
                Shifter.pastShifters.Add(oldShifter.PlayerId);
                if (player.Data.Role.IsImpostor)
                {
                    player.FastSetRole(RoleTypes.Crewmate);
                    oldShifter.FastSetRole(RoleTypes.Impostor);
                }
            }

            if (!oldShifter.Data.IsDead && oldShifter.isRole(RoleId.Fox))
            {
                oldShifter.clearAllTasks();
                List<byte> taskIdList = [];
                Shrine.allShrine.ForEach(shrine => taskIdList.Add((byte)shrine.console.ConsoleId));
                taskIdList.Shuffle();
                var cpt = new CustomNormalPlayerTask("foxTaskStay", Il2CppType.Of<FoxTask>(), Fox.numTasks, [.. taskIdList], Shrine.allShrine.Find(x => x.console.ConsoleId == taskIdList.ToArray()[0]).console.Room, true);
                cpt.addTaskToPlayer(oldShifter.PlayerId);
            }

            // Set cooldowns to max for both players
            if (PlayerControl.LocalPlayer == oldShifter || PlayerControl.LocalPlayer == player)
                CustomButton.ResetAllCooldowns();

            TORGameManager.Instance?.RecordRoleHistory(oldShifter);
            TORGameManager.Instance?.RecordRoleHistory(player);
        }

        static public void jesterWin(byte playerId)
        {
            Jester.players.FirstOrDefault(x => x.player.PlayerId == playerId).triggerJesterWin = true;
        }

        public static void swapperSwap(byte playerId1, byte playerId2) {
            if (MeetingHud.Instance) {
                Swapper.playerId1 = playerId1;
                Swapper.playerId2 = playerId2;
            }
        }

        public static void morphlingMorph(byte playerId, byte morphlingId) {  
            PlayerControl target = Helpers.playerById(playerId);
            Morphling morphling = Morphling.getRole(Helpers.playerById(morphlingId));
            if (morphling == null || target == null) return;

            morphling.morphTimer = Morphling.duration;
            morphling.morphTarget = target;
            if (Camouflager.camouflageTimer <= 0f)
                morphling.player.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void camouflagerCamouflage() {
            if (!Camouflager.exists) return;

            Camouflager.camouflageTimer = Camouflager.duration;
            if (Helpers.MushroomSabotageActive()) return; // Dont overwrite the fungle "camo"
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                player.setLook("", 6, "", "", "", "");
        }

        public static void vampireSetBitten(byte targetId, byte performReset, byte vampireId)
        {
            var vampire = Vampire.getRole(Helpers.playerById(vampireId));
            if (vampire == null) return;
            if (performReset != 0) {
                vampire.bitten = null;
                return;
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                if (player.PlayerId == targetId && !player.Data.IsDead) {
                        vampire.bitten = player;
                }
            }
        }

        public static void plagueDoctorWin()
        {
            PlagueDoctor.triggerPlagueDoctorWin = true;
            var pd = PlagueDoctor.allPlayers.FirstOrDefault();
            if (pd == null) return;
            var livingPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(p => !p.isRole(RoleId.PlagueDoctor) && !p.Data.IsDead);
            foreach (PlayerControl p in livingPlayers)
            {
                if (p.isRole(RoleId.NekoKabocha)) NekoKabocha.getRole(p).otherKiller = pd;
                if (!p.Data.IsDead) p.Exiled();
                GameHistory.overrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Disease, pd);
            }
        }

        public static void plagueDoctorInfected(byte targetId)
        {
            var p = Helpers.playerById(targetId);
            if (PlagueDoctor.allPlayers.Count <= 0) return;
            if (!PlagueDoctor.infected.ContainsKey(targetId)) {
                PlagueDoctor.infected[targetId] = p;
            }
        }

        public static void plagueDoctorProgress(byte targetId, float progress)
        {
            if (PlagueDoctor.allPlayers.Count <= 0) return;
            PlagueDoctor.progress[targetId] = progress;
        }

        public static void placeGarlic(byte[] buff) {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));
            new Garlic(position);
        }

        public static void trackerUsedTracker(byte targetId, byte trackerId) {
            var tracker = Tracker.getRole(Helpers.playerById(trackerId));
            tracker.usedTracker = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == targetId)
                    tracker.tracked = player;
        }

        public static void shareAchievement(byte playerId, string achievement)
        {
            if (TORGameManager.Instance != null && Helpers.playerById(playerId) != null)
            {
                var titleShower = Helpers.playerById(playerId).GetTitleShower();
                TORGameManager.Instance.TitleMap[playerId] = titleShower.SetAchievement(achievement);
            }
        }

        public static void evilHackerCreatesMadmate(byte targetId, byte evilHackerId)
        {
            PlayerControl player = Helpers.playerById(targetId);
            PlayerControl evilHacker = Helpers.playerById(evilHackerId);
            var evilHackerRole = EvilHacker.getRole(evilHacker);
            if (evilHacker == null || evilHackerRole == null) return;
            if (EvilHacker.canCreateMadmateFromJackal || !player.isRole(RoleId.Jackal))
            {
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);

                // タスクがないプレイヤーがMadmateになった場合はショートタスクを必要数割り当てる
                erasePlayerRoles(player.PlayerId, true, true, false);
                if (CreatedMadmate.hasTasks && player == PlayerControl.LocalPlayer)
                    player.generateAndAssignTasks(0, CreatedMadmate.numTasks, 0);

                CreatedMadmate.createdMadmate.Add(player);
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
            }
            evilHackerRole.canCreateMadmate = false;
            return;
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

        public static void sherlockReceiveDetect(byte[] buff)
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            if (PlayerControl.LocalPlayer.isRole(RoleId.Sherlock))
            {
                var arrow = new Sherlock.SherlockDetectArrow(Sherlock.getDetectIcon(), true)
                {
                    TargetPos = position
                };
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Sequence(Effects.Wait(7f), Effects.Action((Action)(() => arrow.MarkAsDisappering()))));
            }
        }

        // Hmm... Lots of bugs huh?
        public static void fortuneTellerUsedDivine(byte fortuneTellerId, byte targetId)
        {
            PlayerControl fortuneTeller = Helpers.playerById(fortuneTellerId);
            var ftRole = FortuneTeller.getRole(fortuneTeller);
            if (ftRole == null) return;
            PlayerControl target = Helpers.playerById(targetId);
            if (target == null) return;
            if (target.Data.IsDead) return;

            if (target.isRole(RoleId.Fox) || target.isRole(RoleId.SchrodingersCat))
            {
                KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                fortuneTeller.MurderPlayer(target, MurderResultFlags.Succeeded);
                overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.Divined, fortuneTeller);
            }

            // インポスターの場合は占い師の位置に矢印を表示
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                FortuneTeller.fortuneTellerMessage(ModTranslation.getString("fortuneTellerDivinedSomeone"), 7f, Color.white);
            }
            ftRole.setDivinedFlag(true);
            if (target.isRole(RoleId.Immoralist) && PlayerControl.LocalPlayer == target)
            {
                FortuneTeller.fortuneTellerMessage(ModTranslation.getString("fortuneTellerDivinedYou"), 7f, Color.white);
            }
            ftRole.divineTarget = target;

            if (PlayerControl.LocalPlayer == fortuneTeller)
            {
                if (target.isRole(RoleId.Fox)) _ = new StaticAchievementToken("fortuneTeller.another1");
                else if (target.Data.Role.IsImpostor) ftRole.acTokenImpostor.Value.divined = true;
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
            if (remainingList.Count <= id) return;
            var antique = remainingList[id];
            antique.isDetected = true;
            Archaeologist.arrowActive = true;
        }

        public static void archaeologistExcavate(byte index)
        {
            if (Antique.antiques == null || Antique.antiques.Count <= index) return; 
            var antique = Antique.antiques.FirstOrDefault(x => x.id == index);
            if (antique == null) return;
            antique.isBroken = true;
            antique.spriteRenderer.sprite = Antique.getBrokenSprite();
            if (PlayerControl.LocalPlayer.isRole(RoleId.Archaeologist))
            {
                (var names, var role) = Archaeologist.getRoleInfo();
                var content = string.Format(ModTranslation.getString("archaeologistClue"), Helpers.cs(role.color, role.name), names);
                RolloverMessage rolloverMessage = RolloverMessage.Create(antique.gameObject.transform.position, true, content, 5f, 0.5f, 2f, 1f, Color.white);
                rolloverMessage.velocity = new Vector3(0f, 0.1f);
                MeetingOverlayHolder.RegisterOverlay(TORGUIContextEngine.API.VerticalHolder(GUIAlignment.Left,
                    new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.API.GetAttribute(AttributeAsset.OverlayTitle), new TranslateTextComponent("archaeologistDetectInfo")),
                    new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.API.GetAttribute(AttributeAsset.OverlayContent), new RawTextComponent(content)))
                    , MeetingOverlayHolder.IconsSprite[2], Archaeologist.color);
                _ = new StaticAchievementToken("archaeologist.challenge");
                if (archaeologistDetectButton.isEffectActive && antique.isDetected) archaeologistDetectButton.Timer = 0f;
            }
            antique.isDetected = false;
            if (Archaeologist.revealAntique == Archaeologist.RevealAntique.Immediately) antique.revealAntique();
        }

        public static void deputyUsedHandcuffs(byte targetId)
        {
            Deputy.handcuffedPlayers.Add(targetId);
        }

        public static void deputyPromotes(byte playerId)
        {
            PlayerControl deputy = Helpers.playerById(playerId);
            var deputyRole = Deputy.getRole(deputy);
            if (deputy != null && deputyRole != null) {  // Deputy should never be null here, but there appeared to be a race condition during testing, which was removed.
                float remainingCuffs = deputyRole.remainingHandcuffs;
                Sheriff.replaceCurrentSheriff(deputy);
                if (PlayerControl.LocalPlayer == deputy) {
                    _ = new StaticAchievementToken("deputy.another1");
                }
                Sheriff curSheriff = Sheriff.getRole(deputy);
                curSheriff.isFormerDeputy = true;
                curSheriff.remainingHandcuffs = remainingCuffs;
                Deputy.eraseRole(deputy);
            }
        }

        public static void jackalCreatesSidekick(byte targetId, byte jackalId) {
            PlayerControl player = Helpers.playerById(targetId);
            var jackal = Jackal.getRole(Helpers.playerById(jackalId));
            if (player == null || jackal == null) return;
            //if (Lawyer.target == player && Lawyer.isProsecutor && Lawyer.lawyer != null && !Lawyer.lawyer.Data.IsDead) Lawyer.isProsecutor = false;

            if (!Jackal.canCreateSidekickFromImpostor && player.Data.Role.IsImpostor) {
                jackal.fakeSidekick = player;
            } else {
                bool wasSpy = player.isRole(RoleId.Spy);
                bool wasImpostor = player.Data.Role.IsImpostor;  // This can only be reached if impostors can be sidekicked.
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                erasePlayerRoles(player.PlayerId, true, true, false);
                player.setRole(RoleId.Sidekick);
                var sidekick = Sidekick.getRole(player);
                sidekick.jackal = jackal;
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
                if (wasSpy || wasImpostor) sidekick.wasTeamRed = true;
                sidekick.wasSpy = wasSpy;
                sidekick.wasImpostor = wasImpostor;
                if (player == PlayerControl.LocalPlayer) {
                    SoundEffectsManager.play("jackalSidekick");
                    _ = new StaticAchievementToken("sidekick.common1");
                    if (wasImpostor) {
                        _ = new StaticAchievementToken("sidekick.common2");
                        LastImpostor.promoteToLastImpostor();
                    }
                }
                if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.getBool() && !HandleGuesser.isGuesser(targetId))
                    setGuesserGm(targetId);
            }
            jackal.canCreateSidekick = false;
        }

        public static void sidekickPromotes(byte playerId) {
            PlayerControl player = Helpers.playerById(playerId);
            var sidekick = Sidekick.getRole(player);
            if (FreePlayGM.isFreePlayGM || sidekick == null) return;
            if (PlayerControl.LocalPlayer.PlayerId == playerId) _ = new StaticAchievementToken("sidekick.challenge");
            bool wasTeamRed = sidekick.wasTeamRed;
            bool wasSpy = sidekick.wasSpy;
            bool wasImpostor = sidekick.wasImpostor;
            Sidekick.eraseRole(player);
            player.setRole(RoleId.Jackal);
            var jackal = Jackal.getRole(player);
            jackal.removeCurrentJackal();
            jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
            jackal.wasTeamRed = wasTeamRed;
            jackal.wasSpy = wasSpy;
            jackal.wasImpostor = wasImpostor;
            return;
        }
        
        public static void erasePlayerRoles(byte playerId, bool ignoreModifier = true, bool isCreatedMadmate = false, bool generateTasks = true) {
            PlayerControl player = Helpers.playerById(playerId);
            if (player == null || (!player.canBeErased() && !isCreatedMadmate)) return;

            if ((player.Data.Tasks == null || player.Data.Tasks?.Count == 0 || player.isRole(RoleId.Fox)) && !player.Data.IsDead && generateTasks && PlayerControl.LocalPlayer == player)
                PlayerControl.LocalPlayer.generateNormalTasks();

            if (player.isRole(RoleId.Lawyer)) Lawyer.clearTarget();
            player.eraseAllRoles();

            // Always remove the Madmate
            if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId)) Madmate.madmate.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (CreatedMadmate.createdMadmate.Any(x => x.PlayerId == player.PlayerId)) CreatedMadmate.createdMadmate.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (player == LastImpostor.lastImpostor) LastImpostor.lastImpostor = null;

            // Modifier
            if (!ignoreModifier)
            {
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

        public static void setFutureShielded(byte playerId, byte medicId) {
            var medic = Medic.getRole(Helpers.playerById(medicId));
            if (medic == null) return;
            medic.futureShielded = Helpers.playerById(playerId);
            medic.usedShield = true;
        }

        public static void setFutureSpelled(byte playerId, byte witchId) {
            PlayerControl player = Helpers.playerById(playerId);
            PlayerControl witch = Helpers.playerById(witchId);
            var witchRole = Witch.getRole(witch);
            if (witch == null || witchRole == null) return;
            witchRole.futureSpelled ??= new List<PlayerControl>();
            if (player != null) {
                witchRole.futureSpelled.Add(player);
            }
        }

        public static void recordStatistics(byte variation, byte relatedTag, byte sourceId, byte targetMask, float timeLag)
        {
            TORGameManager.Instance?.GameStatistics.RecordEvent(new GameStatistics.Event(GameStatistics.EventVariation.ValueOf(variation), TORGameManager.Instance.CurrentTime + timeLag, sourceId == byte.MaxValue ? null : sourceId, targetMask, null) { RelatedTag = TranslatableTag.ValueOf(relatedTag) });
        }

        public static void foxStealth(bool stealthed)
        {
            Fox.setStealthed(stealthed);
        }

        public static void foxCreatesImmoralist(byte targetId)
        {
            PlayerControl player = Helpers.playerById(targetId);
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
            erasePlayerRoles(player.PlayerId, true, true, false);
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
            player.setRole(RoleId.Immoralist);
            player.clearAllTasks();
            Fox.canCreateImmoralist = false;
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

        public static void noisemakerSetSounded(byte playerId, byte noisemakerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            PlayerControl noisem = Helpers.playerById(noisemakerId);
            var noisemaker = Noisemaker.getRole(noisem);
            if (noisemaker == null || noisem == null) return;
            noisemaker.target = player;
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
                    SchrodingersCat.allPlayers.ForEach(x => FastDestroyableSingleton<RoleManager>.Instance.SetRole(x, RoleTypes.Impostor));
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
            if (PlayerControl.LocalPlayer.isRole(RoleId.SchrodingersCat)) _ = new StaticAchievementToken("schrodingersCat.another1");
            SchrodingersCat.allPlayers.ForEach(x => TORGameManager.Instance?.RecordRoleHistory(x));
        }

        public static void placeAssassinTrace(byte playerId, byte[] buff) {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            PlayerControl player = Helpers.playerById(playerId);
            var assassin = Assassin.getRole(player);
            if (player == null || assassin == null) return;
            new AssassinTrace(position, player, Assassin.traceTime);
            if (PlayerControl.LocalPlayer != player)
                assassin.assassinMarked = null;
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
            if (PlayerControl.LocalPlayer.isRole(RoleId.Portalmaker) && Portalmaker.local.acTokenChallenge != null)
            {
                Portalmaker.local.acTokenChallenge.Value.portal++;
                Portalmaker.local.acTokenChallenge.Value.cleared |= Portalmaker.local.acTokenChallenge.Value.portal >= 3;
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
            Ninja.setStealthed(player, stealthed);
        }

        public static void nekoKabochaExile(byte targetId, byte killerId)
        {
            uncheckedExilePlayer(targetId);
            overrideDeathReasonAndKiller(Helpers.playerById(targetId), DeadPlayer.CustomDeathReason.Revenge, killer: Helpers.playerById(killerId));
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
                        MeetingHudPatch.yasunaCheckAndReturnSpecialVote(MeetingHud.Instance, targetId);
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

        public static void cupidSuicide(byte cupidId, bool isScapegoat, bool isExile)
        {
            var cupid = Helpers.playerById(cupidId);
            if (cupid != null)
            {
                if (isExile)
                {
                    cupid.Exiled();
                    if (PlayerControl.LocalPlayer == cupid && Helpers.ShowKillAnimation)
                        FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(cupid.Data, cupid.Data);
                }
                cupid.MurderPlayer(cupid, MurderResultFlags.Succeeded);
                overrideDeathReasonAndKiller(cupid, isScapegoat ? DeadPlayer.CustomDeathReason.Scapegoat : DeadPlayer.CustomDeathReason.Suicide);
                if (PlayerControl.LocalPlayer == cupid && isScapegoat) _ = new StaticAchievementToken("cupid.another1");
            }
        }

        public static void setCupidShield(byte targetId, byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var cupid = Cupid.getRole(player);
            if (player == null || cupid == null) return;
            cupid.shielded = Helpers.playerById(targetId);
        }

        public static void buskerPseudocide(byte targetId, bool isTrueDead, bool isLoverSuicide)
        {
            PlayerControl player = Helpers.playerById(targetId);
            var busker = Busker.getRole(player);
            if (player == null || busker == null) return;
            if (!isTrueDead)
            {
                busker.pseudocideFlag = true;
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
                busker.deathPosition = player.transform.position;
                Helpers.HandleRoleFlashOnDeath(player);
            }
            else
            {
                busker.pseudocideFlag = false;
                Seer.deadBodyPositions?.Add(busker.deathPosition);
                if (Medium.futureDeadBodies != null) Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(new DeadPlayer(player, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Pseudocide, player), busker.deathPosition));
                if (!isLoverSuicide) overrideDeathReasonAndKiller(player, DeadPlayer.CustomDeathReason.Pseudocide);
                busker.pseudocideComplete = true;
                TORGameManager.Instance?.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Kill, targetId, 1 << targetId) { RelatedTag = isLoverSuicide ? EventDetail.Kill : EventDetail.Pseudocide });

                Lovers.killLovers(player, player);
                Kataomoi.killKataomoi(player);

                if (AmongUsClient.Instance.AmHost) FastDestroyableSingleton<RoleManager>.Instance.AssignRoleOnDeath(player, false);
            }
        }

        public static void buskerRevive(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var busker = Busker.getRole(player);
            if (player == null || busker == null) return;
            busker.pseudocideFlag = false;
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
            TORGameManager.Instance?.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Revive, null, 1 << playerId) { RelatedTag = EventDetail.Revive });
        }

        public static void veteranAlert(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var veteran = Veteran.getRole(player);
            if (player == null || veteran == null) return;
            veteran.alertActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Veteran.alertDuration, new Action<float>((p) => {
                if (p == 1f) veteran.alertActive = false;
            })));
        }

        public static void unlockMayorAcCommon(byte votedFor, byte playerId)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId)
            {
                if (!GameManager.Instance.LogicOptions.GetAnonymousVotes())
                    _ = new StaticAchievementToken("mayor.common1");
                if (Mayor.local.acTokenChallenge != null) Mayor.local.acTokenChallenge.Value.votedFor = votedFor;
            }
        }

        public static void unlockDetectiveAcChallenge(byte votedFor, byte playerId)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId) {
                Detective.local.acTokenChallenge.Value.votedFor = votedFor;
            }
        }

        public static void unlockMedicAcChallenge(byte killerId, byte medicId)
        {
            if (PlayerControl.LocalPlayer.PlayerId == medicId)
                Medic.local.acTokenChallenge.Value.killerId = killerId;
        }

        public static void unlockTrackerAcChallenge(float moveTime, byte trackerId)
        {
            if (PlayerControl.LocalPlayer.PlayerId == trackerId && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (!Tracker.local.acTokenChallenge.Value.cleared)
                {
                    if (moveTime - Tracker.local.acTokenChallenge.Value.ventTime >= Tracker.updateIntervall)
                        Tracker.local.acTokenChallenge.Value.cleared = true;
                }
            }
        }

        public static void unlockVeteranAcChallenge(byte playerId)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId)
                _ = new StaticAchievementToken("veteran.challenge");
        }

        public static void yoyoMarkLocation(byte[] buff, byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var yoyo = Yoyo.getRole(player);
            if (player == null || yoyo == null) return;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            yoyo.markLocation(position);
            _ = new Silhouette(position, player, -1, false);
        }

        public static void yoyoBlink(bool isFirstJump, byte[] buff, byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var yoyo = Yoyo.getRole(player);
            TheOtherRolesPlugin.Logger.LogMessage($"blink fistjumpo: {isFirstJump}");
            if (player == null || yoyo == null || yoyo.markedLocation == null) return;
            var markedPos = (Vector3)yoyo.markedLocation;
            player.NetTransform.SnapTo(markedPos);

            var markedSilhouette = Silhouette.silhouettes.FirstOrDefault(s => s.gameObject.transform.position.x == markedPos.x && s.gameObject.transform.position.y == markedPos.y);
            if (markedSilhouette != null)
                markedSilhouette.permanent = false;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            // Create Silhoutte At Start Position:
            if (isFirstJump)
            {
                yoyo.markLocation(position);
                new Silhouette(position, player, Yoyo.blinkDuration, true);
            }
            else
            {
                new Silhouette(position, player, 5, true);
                yoyo.markedLocation = null;
            }
            if (Chameleon.chameleon.Any(x => x.PlayerId == playerId)) // Make the Yoyo visible if chameleon!
                Chameleon.lastMoved[playerId] = Time.time;
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

        public static void doomsayerObserve(byte playerId, byte doomsayerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            PlayerControl doomsayer = Helpers.playerById(doomsayerId);
            var doomsayerRole = Doomsayer.getRole(doomsayer);
            if (player == null || doomsayer == null || doomsayerRole == null) return;
            doomsayerRole.observed = player;
        }

        public static void mimicMorph(byte mimicAId, byte mimicBId)
        {
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

        public static void setOddIsJekyll(bool b, byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            var jekyllAndHyde = JekyllAndHyde.getRole(player);
            if (player == null || jekyllAndHyde == null) return;
            jekyllAndHyde.oddIsJekyll = b;
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
                if (PlayerControl.LocalPlayer.PlayerId == oldTaskMasterPlayerId) PlayerControl.LocalPlayer.generateNormalTasks();
            }

            if (!Helpers.playerById(playerId).isRole(RoleId.TaskMaster))
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
            if (!TaskMaster.exists) return;
            TaskMaster.clearExTasks = clearExTasks;
            TaskMaster.allExTasks = allExTasks;
        }

        public static void setCupidLovers(byte playerId1, byte playerId2, byte cupidId)
        {
            var p1 = Helpers.playerById(playerId1);
            var p2 = Helpers.playerById(playerId2);
            var player = Helpers.playerById(cupidId);
            var cupid = Cupid.getRole(player);
            if (player == null || p1 == null || p2 == null || cupid == null) return;
            cupid.lovers1 = p1;
            cupid.lovers2 = p2;
            Cupid.breakLovers(p1, p2);
            Cupid.breakLovers(p2, p1);
            Lovers.addCouple(p1, p2);
        }

        public static void impostorPromotesToLastImpostor(byte targetId)
        {
            PlayerControl player = Helpers.playerById(targetId);
            if (LastImpostor.lastImpostor != null && player == LastImpostor.lastImpostor) return;
            if (LastImpostor.lastImpostor != null && !LastImpostor.isOriginalGuesser) GuesserGM.clear(LastImpostor.lastImpostor.PlayerId);
            LastImpostor.clearAndReload();
            if (!HandleGuesser.isGuesser(targetId)) setGuesserGm(targetId);
            else LastImpostor.isOriginalGuesser = true;
            LastImpostor.lastImpostor = player;
            var g = GuesserGM.guessers.FindLast(x => x.guesser.PlayerId == targetId);
            if (g == null) return;
            g.shots = Mathf.Max(g.shots, LastImpostor.numShots);
        }

        public static void blackmailPlayer(byte playerId, byte blackmailerId)
        {
            PlayerControl target = Helpers.playerById(playerId);
            PlayerControl blackmailer = Helpers.playerById(blackmailerId);
            var blackmailerRole = Blackmailer.getRole(blackmailer);
            if (blackmailerRole == null) return;
           blackmailerRole.blackmailed = target;
        }

        public static void unblackmailPlayer()
        {
            foreach (var p in Blackmailer.players)
                p.blackmailed = null;
            Blackmailer.alreadyShook = false;
        }

        public static void akujoSetHonmei(byte akujoId, byte targetId)
        {
            PlayerControl akujo = Helpers.playerById(akujoId);
            PlayerControl target = Helpers.playerById(targetId);
            var akujoRole = Akujo.getRole(akujo);

            if (akujo != null && akujoRole != null && akujoRole.honmei == null)
            {
                Akujo.breakLovers(target);
                akujoRole.honmei = target;
            }
        }

        public static void akujoSetKeep(byte akujoId, byte targetId)
        {
            var akujo = Helpers.playerById(akujoId);
            PlayerControl target = Helpers.playerById(targetId);
            var akujoRole = Akujo.getRole(akujo);

            if (akujo != null && akujoRole != null && akujoRole.keepsLeft > 0)
            {
                Akujo.breakLovers(target);
                akujoRole.keeps.Add(target);
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
            if (PlayerControl.LocalPlayer.isRole(RoleId.TaskMaster))
                _ = new StaticAchievementToken("taskMaster.challenge");
        }

        public static void unlockJesterAcCommon(byte playerId)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId) {
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
            if (!Kataomoi.exists) return;

            Kataomoi.triggerKataomoiWin = true;
            if (Kataomoi.target != null) {
                Kataomoi.target.Exiled();
                overrideDeathReasonAndKiller(Kataomoi.target, DeadPlayer.CustomDeathReason.KataomoiStare, Kataomoi.allPlayers.FirstOrDefault());
            }
        }

        public static void kataomoiStalking(byte playerId)
        {
            PlayerControl player = Helpers.playerById(playerId);
            if (!player.isRole(RoleId.Kataomoi)) return;

            Kataomoi.doStalking();
        }

        public static void setBrainwash(byte playerId, byte moriartyId)
        {
            var p = Helpers.playerById(playerId);
            var player = Helpers.playerById(moriartyId);
            var moriarty = Moriarty.getRole(player);
            if (player == null || moriarty == null) return;
            moriarty.target = p;
            Moriarty.brainwashed.Add(p);
        }

        public static void moriartyKill(byte targetId, byte playerId)
        {
            PlayerControl target = Helpers.playerById(targetId);
            PlayerControl player = Helpers.playerById(playerId);
            var moriarty = Moriarty.getRole(player);
            if (moriarty == null || player == null || target == null) return;
            GameHistory.overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.BrainwashedKilled, player);
            if (PlayerControl.LocalPlayer == moriarty.target) {
                if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);
            }
            moriarty.counter += 1;
            if (target.isRole(RoleId.Sherlock)) moriarty.counter += Moriarty.sherlockAddition;
            Moriarty.hasKilled = true;
            if (Moriarty.numberToWin == moriarty.counter) Moriarty.triggerMoriartyWin = true;
        }

        public static void plantBomb(byte playerId)
        {
            var p = Helpers.playerById(playerId);
            if (PlayerControl.LocalPlayer.isRole(RoleId.BomberA)) BomberB.bombTarget = p;
            if (PlayerControl.LocalPlayer.isRole(RoleId.BomberB)) BomberA.bombTarget = p;
        }

        public static void releaseBomb(byte killer, byte target)
        {
            // 同時押しでダブルキルが発生するのを防止するためにBomberAで一度受け取ってから実行する
            if (PlayerControl.LocalPlayer.isRole(RoleId.BomberA))
            {
                if (BomberA.bombTarget != null && BomberB.bombTarget != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BomberKill, Hazel.SendOption.Reliable, -1);
                    writer.Write(killer);
                    writer.Write(target);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    bomberKill(killer, target);
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
                overrideDeathReasonAndKiller(t, DeadPlayer.CustomDeathReason.Bomb, k);
                if (BomberA.showEffects)
                    _ = new BombEffect(t);
            }
            bomberAPlantBombButton.Timer = bomberAPlantBombButton.MaxTimer;
            bomberBPlantBombButton.Timer = bomberBPlantBombButton.MaxTimer;
        }

        public static void placeCamera(byte[] buff, byte roomId) {
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

            camera.NewName = (SystemTypes)roomId switch
            {
                SystemTypes.Hallway => StringNames.Hallway,
                SystemTypes.Storage => StringNames.Storage,
                SystemTypes.Cafeteria => StringNames.Cafeteria,
                SystemTypes.Reactor => StringNames.Reactor,
                SystemTypes.UpperEngine => StringNames.UpperEngine,
                SystemTypes.Nav => StringNames.Nav,
                SystemTypes.Admin => StringNames.Admin,
                SystemTypes.Electrical => StringNames.Electrical,
                SystemTypes.LifeSupp => StringNames.LifeSupp,
                SystemTypes.Shields => StringNames.Shields,
                SystemTypes.MedBay => StringNames.MedBay,
                SystemTypes.Security => StringNames.Security,
                SystemTypes.Weapons => StringNames.Weapons,
                SystemTypes.LowerEngine => StringNames.LowerEngine,
                SystemTypes.Comms => StringNames.Comms,
                SystemTypes.Decontamination => StringNames.Decontamination,
                SystemTypes.Launchpad => StringNames.Launchpad,
                SystemTypes.LockerRoom => StringNames.LockerRoom,
                SystemTypes.Laboratory => StringNames.Laboratory,
                SystemTypes.Balcony => StringNames.Balcony,
                SystemTypes.Office => StringNames.Office,
                SystemTypes.Greenhouse => StringNames.Greenhouse,
                SystemTypes.Dropship => StringNames.Dropship,
                SystemTypes.Decontamination2 => StringNames.Decontamination2,
                SystemTypes.Decontamination3 => StringNames.Decontamination3,
                SystemTypes.Outside => StringNames.Outside,
                SystemTypes.Specimens => StringNames.Specimens,
                SystemTypes.BoilerRoom => StringNames.BoilerRoom,
                SystemTypes.VaultRoom => StringNames.VaultRoom,
                SystemTypes.Cockpit => StringNames.Cockpit,
                SystemTypes.Armory => StringNames.Armory,
                SystemTypes.Kitchen => StringNames.Kitchen,
                SystemTypes.ViewingDeck => StringNames.ViewingDeck,
                SystemTypes.HallOfPortraits => StringNames.HallOfPortraits,
                SystemTypes.CargoBay => StringNames.CargoBay,
                SystemTypes.Ventilation => StringNames.Ventilation,
                SystemTypes.Showers => StringNames.Showers,
                SystemTypes.Engine => StringNames.Engine,
                SystemTypes.Brig => StringNames.Brig,
                SystemTypes.MeetingRoom => StringNames.MeetingRoom,
                SystemTypes.Records => StringNames.Records,
                SystemTypes.Lounge => StringNames.Lounge,
                SystemTypes.GapRoom => StringNames.GapRoom,
                SystemTypes.MainHall => StringNames.MainHall,
                SystemTypes.Medical => StringNames.Medical,
                _ => StringNames.None,
            };

            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId is 2 or 4) camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

            if (SubmergedCompatibility.IsSubmerged) {
                // remove 2d box collider of console, so that no barrier can be created. (irrelevant for now, but who knows... maybe we need it later)
                var fixConsole = camera.transform.FindChild("FixConsole");
                if (fixConsole != null) {
                    var boxCollider = fixConsole.GetComponent<BoxCollider2D>();
                    if (boxCollider != null) UnityEngine.Object.Destroy(boxCollider);
                }
            }


            if (PlayerControl.LocalPlayer.isRole(RoleId.SecurityGuard)) {
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
            if (PlayerControl.LocalPlayer.isRole(RoleId.SecurityGuard)) {
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

        public static void arsonistWin(byte playerId) {
            PlayerControl player = Helpers.playerById(playerId);
            var arsonist = Arsonist.getRole(player);
            if (player == null || arsonist == null) return;
            arsonist.triggerArsonistWin = true;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                if (p != player && !p.Data.IsDead) {
                    if (p.isRole(RoleId.NekoKabocha)) NekoKabocha.getRole(p).otherKiller = player;
                    p.Exiled();
                    overrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Arson, player);
                }
            }
        }

        public static void lawyerSetTarget(byte playerId) {
            Lawyer.target = Helpers.playerById(playerId);
        }

        public static void lawyerPromotesToPursuer(byte playerId) {
            PlayerControl player = Helpers.playerById(playerId);
            Lawyer.eraseRole(player);
            player.setRole(RoleId.Pursuer);
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
            TORGameManager.Instance?.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Kill, killerId, 1 << dyingTargetId) { RelatedTag = killerId == dyingTargetId ? EventDetail.MisGuess : EventDetail.Guessed});

            PlayerControl killer = Helpers.playerById(killerId);
            PlayerControl dyingTarget = Helpers.playerById(dyingTargetId);
            if (dyingTarget == null) return;
            if (dyingTarget.isRole(RoleId.NekoKabocha)) NekoKabocha.getRole(dyingTarget).meetingKiller = killer;
            bool revengeFlag = (NekoKabocha.revengeCrew && !Helpers.isNeutral(killer) && !killer.Data.Role.IsImpostor) ||
                    (NekoKabocha.revengeNeutral && Helpers.isNeutral(killer)) ||
                    (NekoKabocha.revengeImpostor && killer.Data.Role.IsImpostor);

            PlayerControl guesser = Helpers.playerById(killerId);
            if (killer.isRole(RoleId.Thief) && Thief.canStealWithGuess) {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                if (!killer.Data.IsDead && !Thief.isFailedThiefKill(dyingTarget, guesser, roleInfo)) {
                    RPCProcedure.thiefStealsRole(dyingTarget.PlayerId, killerId);
                }
            }

            if (killer.isRole(RoleId.Doomsayer) && dyingTarget != killer) {
                if (Doomsayer.indicateGuesses && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance) {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, ModTranslation.getString("doomsayerGuessedSomeone"), false);
                    }
                }
                var doomsayer = Doomsayer.getRole(killer);
                doomsayer.counter++;
                if (doomsayer.counter >= Doomsayer.guessesToWin) doomsayer.triggerWin = true;
            }

            dyingTarget.Exiled();
            GameHistory.overrideDeathReasonAndKiller(dyingTarget, DeadPlayer.CustomDeathReason.Guess, guesser);

            if (PlayerControl.LocalPlayer == killer && dyingTarget != killer)
            {
                if (killer.isRole(RoleId.NiceGuesser))
                {
                    NiceGuesser.acTokenNiceGuesser.Value++;
                    _ = new StaticAchievementToken("niceGuesser.common1");
                    if (dyingTarget.isRole(RoleId.EvilGuesser))
                        _ = new StaticAchievementToken("niceGuesser.challenge2");
                }
                else if (killer.isRole(RoleId.EvilGuesser))
                {
                    EvilGuesser.acTokenEvilGuesser.Value++;
                    _ = new StaticAchievementToken("evilGuesser.common1");
                    if (dyingTarget.isRole(RoleId.NiceGuesser))
                        _ = new StaticAchievementToken("evilGuesser.challenge2");
                }
            }

            HandleGuesser.remainingShots(killerId, true);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
            if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
                if (PlayerControl.LocalPlayer == dyingTarget) {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
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
                else if (!PlayerControl.LocalPlayer.Data.IsDead && (PlayerControl.LocalPlayer.isRole(RoleId.NiceWatcher) || PlayerControl.LocalPlayer.isRole(RoleId.EvilWatcher)) && Watcher.canSeeGuesses)
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

        public static void thiefStealsRole(byte playerId, byte thiefId) {
            // Notify that the Thief cannot steal the Mimic

            PlayerControl target = Helpers.playerById(playerId);
            PlayerControl thief = Helpers.playerById(thiefId);
            if (target == null) return;
            Thief.eraseRole(thief);
            if (target.isRole(RoleId.Witch))
            {
                var witch = Witch.getRole(target);
                if (MeetingHud.Instance)
                    if (Witch.witchVoteSavesTargets)  // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                        witch.futureSpelled = [];
                    else  // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                        witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
            var role = Role.allRoles.FirstOrDefault(x => x.player == target);
            target.swapRoles(thief);
            if (role != null && role.roleId is RoleId.Jackal or RoleId.JekyllAndHyde or RoleId.Moriarty) target.setRole(role.roleId);  // Keep teamed roles to the target
            if (target.Data.Role.IsImpostor) {
                RoleManager.Instance.SetRole(thief, RoleTypes.Impostor);
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(thief.killTimer, PlayerControl.LocalPlayer.GetKillCooldown());
            }
            if (target == Lawyer.target)
                Lawyer.target = thief;
            if (thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
            Thief.formerThief.Add(thief);  // After clearAndReload, else it would get reset...

            TORGameManager.Instance?.RecordRoleHistory(thief);
            TORGameManager.Instance?.RecordRoleHistory(target);
        }

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
            AssassinMarked,
            WarlockTarget,
            MediumInfo,
            BlankUsed,
            DetectiveOrMedicInfo,
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
                case GhostInfoTypes.AssassinMarked:
                    var assassin = Assassin.getRole(sender);
                    if (assassin != null) assassin.assassinMarked = Helpers.playerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.WarlockTarget:
                    var warlock = Warlock.getRole(sender);
                    if (warlock != null) warlock.curseVictim = Helpers.playerById(reader.ReadByte());
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
                case (byte)CustomRPC.SetLovers:
                    RPCProcedure.setLovers(reader.ReadByte(), reader.ReadByte());
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
                    RPCProcedure.timeMasterRewindTime(reader.ReadByte());
                    break;
                case (byte)CustomRPC.TimeMasterShield:
                    RPCProcedure.timeMasterShield(reader.ReadByte());
                    break;
                case (byte)CustomRPC.MedicSetShielded:
                    RPCProcedure.medicSetShielded(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.ShieldedMurderAttempt:
                    RPCProcedure.shieldedMurderAttempt(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ShifterShift:
                    RPCProcedure.shifterShift(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SwapperSwap:
                    byte playerId1 = reader.ReadByte();
                    byte playerId2 = reader.ReadByte();
                    RPCProcedure.swapperSwap(playerId1, playerId2);
                    break;
                case (byte)CustomRPC.MorphlingMorph:
                    RPCProcedure.morphlingMorph(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.CamouflagerCamouflage:
                    RPCProcedure.camouflagerCamouflage();
                    break;
                case (byte)CustomRPC.VampireSetBitten:
                    byte bittenId = reader.ReadByte();
                    byte reset = reader.ReadByte();
                    RPCProcedure.vampireSetBitten(bittenId, reset, reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceGarlic:
                    RPCProcedure.placeGarlic(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.TrackerUsedTracker:
                    RPCProcedure.trackerUsedTracker(reader.ReadByte(), reader.ReadByte());
                    break;               
                case (byte)CustomRPC.DeputyUsedHandcuffs:
                    RPCProcedure.deputyUsedHandcuffs(reader.ReadByte());
                    break;
                case (byte)CustomRPC.DeputyPromotes:
                    RPCProcedure.deputyPromotes(reader.ReadByte());
                    break;
                case (byte)CustomRPC.JackalCreatesSidekick:
                    RPCProcedure.jackalCreatesSidekick(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.SidekickPromotes:
                    RPCProcedure.sidekickPromotes(reader.ReadByte());
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
                    RPCProcedure.setFutureShielded(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceAssassinTrace:
                    RPCProcedure.placeAssassinTrace(reader.ReadByte(), reader.ReadBytesAndSize());
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
                    RPCProcedure.yoyoMarkLocation(reader.ReadBytesAndSize(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.YoyoBlink:
                    RPCProcedure.yoyoBlink(reader.ReadByte() == byte.MaxValue, reader.ReadBytesAndSize(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.BreakArmor:
                    RPCProcedure.breakArmor();
                    break;
                case (byte)CustomRPC.DoomsayerObserve:
                    RPCProcedure.doomsayerObserve(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.ImpostorPromotesToLastImpostor:
                    RPCProcedure.impostorPromotesToLastImpostor(reader.ReadByte());
                    break;
                case (byte)CustomRPC.NinjaStealth:
                    RPCProcedure.ninjaStealth(reader.ReadByte(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.SprinterSprint:
                    RPCProcedure.sprinterSprint(reader.ReadByte(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.NekoKabochaExile:
                    RPCProcedure.nekoKabochaExile(reader.ReadByte(), reader.ReadByte());
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
                    RPCProcedure.archaeologistExcavate(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SherlockReceiveDetect:
                    RPCProcedure.sherlockReceiveDetect(reader.ReadBytesAndSize());
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
                    RPCProcedure.setOddIsJekyll(reader.ReadBoolean(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.EvilHackerCreatesMadmate:
                    RPCProcedure.evilHackerCreatesMadmate(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.VeteranAlert:
                    RPCProcedure.veteranAlert(reader.ReadByte());
                    break;
                case (byte)CustomRPC.NoisemakerSetSounded:
                    RPCProcedure.noisemakerSetSounded(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.SchrodingersCatSetTeam:
                    RPCProcedure.schrodingersCatSetTeam(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockMayorAcCommon:
                    RPCProcedure.unlockMayorAcCommon(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockDetectiveAcChallenge:
                    RPCProcedure.unlockDetectiveAcChallenge(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockMedicAcChallenge:
                    RPCProcedure.unlockMedicAcChallenge(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockTrackerAcChallenge:
                    RPCProcedure.unlockTrackerAcChallenge(reader.ReadSingle(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockVeteranAcChallenge:
                    RPCProcedure.unlockVeteranAcChallenge(reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnlockTaskMasterAcChallenge:
                    RPCProcedure.unlockTaskMasterAcChallenge();
                    break;
                case (byte)CustomRPC.UnlockJesterAcCommon:
                    RPCProcedure.unlockJesterAcCommon(reader.ReadByte());
                    break;
                case (byte)CustomRPC.JesterWin:
                    RPCProcedure.jesterWin(reader.ReadByte());
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
                case (byte)CustomRPC.RecordStatistics:
                    RPCProcedure.recordStatistics(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadSingle());
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
                    RPCProcedure.blackmailPlayer(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.UnblackmailPlayer:
                    RPCProcedure.unblackmailPlayer();
                    break;
                case (byte)CustomRPC.UndertakerDragBody:
                    var bodyId = reader.ReadByte();
                    Undertaker.DragBody(bodyId);
                    break;
                case (byte)CustomRPC.SetCupidLovers:
                    RPCProcedure.setCupidLovers(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.CupidSuicide:
                    RPCProcedure.cupidSuicide(reader.ReadByte(), reader.ReadBoolean(), reader.ReadBoolean());
                    break;
                case (byte)CustomRPC.SetCupidShield:
                    RPCProcedure.setCupidShield(reader.ReadByte(), reader.ReadByte());
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
                case (byte)CustomRPC.ZephyrBlowCannon:
                    byte blownId = reader.ReadByte();
                    byte zephyrId = reader.ReadByte();
                    var posx = reader.ReadSingle();
                    var posy = reader.ReadSingle();
                    var player = Helpers.playerById(blownId);
                    var zephyr = Helpers.playerById(zephyrId);
                    Zephyr.fireCannon(player, zephyr, new Vector2(posx, posy));
                    break;
                case (byte)CustomRPC.ZephyrCheckCannon:
                    Zephyr.checkCannon(new(reader.ReadSingle(), reader.ReadSingle()), reader.ReadByte());
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
                case (byte)CustomRPC.ShareAchievement:
                    RPCProcedure.shareAchievement(reader.ReadByte(), reader.ReadString());
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
                    RPCProcedure.setBrainwash(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.MoriartyKill:
                    RPCProcedure.moriartyKill(reader.ReadByte(), reader.ReadByte());
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
                    RPCProcedure.placeCamera(reader.ReadBytesAndSize(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.SealVent:
                    RPCProcedure.sealVent(reader.ReadPackedInt32());
                    break;
                case (byte)CustomRPC.ArsonistWin:
                    RPCProcedure.arsonistWin(reader.ReadByte());
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
                    RPCProcedure.lawyerPromotesToPursuer(reader.ReadByte());
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
                    RPCProcedure.setFutureSpelled(reader.ReadByte(), reader.ReadByte());
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
                    RPCProcedure.thiefStealsRole(thiefTargetId, reader.ReadByte());
                    break;
                case (byte)CustomRPC.DraftModePickOrder:
                    RoleDraft.receivePickOrder(reader.ReadByte(), reader);
                    break;
                case (byte)CustomRPC.DraftModePick:
                    RoleDraft.receivePick(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean(), reader.ReadBoolean());
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
                case (byte)CustomRPC.EventKick:
                    byte kickSource = reader.ReadByte();
                    byte kickTarget = reader.ReadByte();
                    EventUtility.handleKick(Helpers.playerById(kickSource), Helpers.playerById(kickTarget), reader.ReadSingle());
                    break;
            }
        }
    }
} 
