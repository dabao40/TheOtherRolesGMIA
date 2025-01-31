using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using System;
using TheOtherRoles.Utilities;
using UnityEngine;
using Epic.OnlineServices.Connect;
using TheOtherRoles.Modules;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class ExileControllerBeginPatch {
        public static NetworkedPlayerInfo lastExiled;
        public static bool extraVictim;
        public static void Prefix(ExileController __instance, [HarmonyArgument(0)]ref ExileController.InitProperties init) {
            lastExiled = init.networkedPlayer;
            extraVictim = false;
            // Medic shield
            if (Medic.medic != null && AmongUsClient.Instance.AmHost && Medic.futureShielded != null && !Medic.medic.Data.IsDead) { // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MedicSetShielded, Hazel.SendOption.Reliable, -1);
                writer.Write(Medic.futureShielded.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.medicSetShielded(Medic.futureShielded.PlayerId);
            }
            if (Medic.usedShield) Medic.meetingAfterShielding = true;  // Has to be after the setting of the shield

            // Shifter shift
            if (Shifter.shifter != null && AmongUsClient.Instance.AmHost && Shifter.futureShift != null) { // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
                PlayerControl oldShifter = Shifter.shifter;
                byte oldTaskMasterPlayerId = TaskMaster.isTaskMaster(Shifter.futureShift.PlayerId) && TaskMaster.isTaskComplete ? Shifter.futureShift.PlayerId : byte.MaxValue;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShifterShift, Hazel.SendOption.Reliable, -1);
                writer.Write(Shifter.futureShift.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shifterShift(Shifter.futureShift.PlayerId);

                if (TaskMaster.isTaskMaster(oldShifter.PlayerId))
                {
                    byte clearTasks = 0;
                    for (int i = 0; i < oldShifter.Data.Tasks.Count; ++i)
                    {
                        if (oldShifter.Data.Tasks[i].Complete)
                            ++clearTasks;
                    }
                    bool allTasksCompleted = clearTasks == oldShifter.Data.Tasks.Count;
                    byte[] taskTypeIds = allTasksCompleted ? TaskMasterTaskHelper.GetTaskMasterTasks(oldShifter) : null;
                    MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TaskMasterSetExTasks, Hazel.SendOption.Reliable, -1);
                    writer2.Write(oldShifter.PlayerId);
                    writer2.Write(oldTaskMasterPlayerId);
                    if (taskTypeIds != null)
                        writer2.Write(taskTypeIds);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    RPCProcedure.taskMasterSetExTasks(oldShifter.PlayerId, oldTaskMasterPlayerId, taskTypeIds);
                }
            }
            Shifter.futureShift = null;

            // Eraser erase
            if (Eraser.eraser != null && AmongUsClient.Instance.AmHost && Eraser.futureErased != null) {  // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
                foreach (PlayerControl target in Eraser.futureErased) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ErasePlayerRoles, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.erasePlayerRoles(target.PlayerId);
                    Eraser.alreadyErased.Add(target.PlayerId);
                }
            }
            Eraser.futureErased = new List<PlayerControl>();

            // Trickster boxes
            if (Trickster.trickster != null && JackInTheBox.hasJackInTheBoxLimitReached()) {
                JackInTheBox.convertToVents();
                if (PlayerControl.LocalPlayer == Trickster.trickster)
                    _ = new StaticAchievementToken("trickster.common1");
            }

            // Activate portals.
            Portal.meetingEndsUpdate();            

            // Witch execute casted spells
            if (Witch.witch != null && Witch.futureSpelled != null && AmongUsClient.Instance.AmHost) {
                bool exiledIsWitch = init.networkedPlayer != null && init.networkedPlayer.PlayerId == Witch.witch.PlayerId;
                bool witchDiesWithExiledLover = init.networkedPlayer != null && init.networkedPlayer.Object.GetAllRelatedPlayers().Contains(Witch.witch);

                if ((witchDiesWithExiledLover || exiledIsWitch) && Witch.witchVoteSavesTargets) Witch.futureSpelled = new List<PlayerControl>();
                foreach (PlayerControl target in Witch.futureSpelled) {
                    if (target != null && !target.Data.IsDead && Helpers.checkMuderAttempt(Witch.witch, target, true) == MurderAttemptResult.PerformKill){
                        if (target == Lawyer.target && Lawyer.lawyer != null) {
                            MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);
                            RPCProcedure.lawyerPromotesToPursuer();
                        }
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedExilePlayer, Hazel.SendOption.Reliable, -1);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.uncheckedExilePlayer(target.PlayerId);

                        MessageWriter writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                        writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer3.Write((byte)RPCProcedure.GhostInfoTypes.DeathReasonAndKiller);
                        writer3.Write(target.PlayerId);
                        writer3.Write((byte)DeadPlayer.CustomDeathReason.WitchExile);
                        writer3.Write(Witch.witch.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer3);

                        GameHistory.overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.WitchExile, killer: Witch.witch);
                    }
                }
            }
            Witch.futureSpelled = new List<PlayerControl>();

            // SecurityGuard vents and cameras
            var allCameras = MapUtilities.CachedShipStatus.AllCameras.ToList();
            TORMapOptions.camerasToAdd.ForEach(camera => {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                allCameras.Add(camera);
            });
            MapUtilities.CachedShipStatus.AllCameras = allCameras.ToArray();
            TORMapOptions.camerasToAdd = new List<SurvCamera>();

            foreach (Vent vent in TORMapOptions.ventsToSeal) {
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
                rend.color = Color.white;
                vent.name = "SealedVent_" + vent.name;
            }
            TORMapOptions.ventsToSeal = new List<Vent>();

            EventUtility.meetingEndsUpdate();
        }        
    }

    [HarmonyPatch]
    class ExileControllerWrapUpPatch {

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        class BaseExileControllerPatch {
            static bool Prefix(ExileController __instance) {
                __instance.StartCoroutine(WrapUpPrefix(__instance).WrapToIl2Cpp());
                return false;
            }
            public static void Postfix(ExileController __instance) {
                NetworkedPlayerInfo networkedPlayer = __instance.initData.networkedPlayer;
                WrapUpPostfix((networkedPlayer != null) ? networkedPlayer.Object : null);
            }
        }

        [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
        class AirshipExileControllerPatch {
            static bool Prefix(AirshipExileController __instance, ref Il2CppSystem.Collections.IEnumerator __result) {
                __result = WrapUpPrefix(__instance).WrapToIl2Cpp();
                return false;
            }
            public static void Postfix(AirshipExileController __instance) {
                NetworkedPlayerInfo networkedPlayer = __instance.initData.networkedPlayer;
                WrapUpPostfix((networkedPlayer != null) ? networkedPlayer.Object : null);
            }
        }

        // Workaround to add a "postfix" to the destroying of the exile controller (i.e. cutscene) and SpwanInMinigame of submerged
        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj) {
            // Nightvision:
            if (obj != null && obj.name != null && obj.name.Contains("FungleSecurity"))
            {
                SurveillanceMinigamePatch.resetNightVision();
                return;
            }

            if (!SubmergedCompatibility.IsSubmerged) return;
            if (obj.name.Contains("ExileCutscene")) {
                WrapUpPostfix(obj.GetComponent<ExileController>().initData.networkedPlayer?.Object);
            } else if (obj.name.Contains("SpawnInMinigame")) {
                AntiTeleport.setPosition();
                Chameleon.lastMoved.Clear();
            }
        }

        static IEnumerator WrapUpPrefix(ExileController __instance)
        {
            PlayerControl @object = null;
            if (__instance.initData.networkedPlayer != null)
            {
                @object = __instance.initData.networkedPlayer.Object;
                if (@object) @object.Exiled();
                __instance.initData.networkedPlayer.IsDead = true;
            }
            if (ExileControllerBeginPatch.extraVictim && CustomOptionHolder.noticeExtraVictims.getBool())
            {
                string str = ModTranslation.getString("someoneDisappeared");
                int num = 0;
                var additionalText = UnityEngine.Object.Instantiate(__instance.Text, __instance.transform);
                additionalText.transform.localPosition = new Vector3(0, 0, -800f);
                additionalText.text = "";

                while (num < str.Length)
                {
                    num++;
                    additionalText.text = str[..num];
                    SoundManager.Instance.PlaySoundImmediate(__instance.TextSound, false, 0.8f, 0.92f);
                    yield return new WaitForSeconds(Mathf.Min(2.8f / str.Length, 0.28f));
                }
                yield return new WaitForSeconds(1.9f);

                float a = 1f;
                while (a > 0f)
                {
                    a -= Time.deltaTime * 1.5f;
                    additionalText.color = Color.white.AlphaMultiplied(a);
                    yield return null;
                }
                yield return new WaitForSeconds(0.3f);
            }
            if (DestroyableSingleton<TutorialManager>.InstanceExists || GameData.Instance || GameManager.Instance.LogicFlow.IsGameOverDueToDeath())
            {
                if (__instance is AirshipExileController airshipExileController) yield return ShipStatus.Instance.PrespawnStep();
                __instance.ReEnableGameplay();
            }
            UnityEngine.Object.Destroy(__instance.gameObject);
        }

        static void WrapUpPostfix(PlayerControl exiled)
        {
            GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.MeetingEnd, null, 0) { RelatedTag = EventDetail.MeetingEnd });
            if (exiled != null)
                GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Exile, null, 1 << exiled.PlayerId) { RelatedTag = EventDetail.Exiled });
            
            // Prosecutor win condition
            /*if (exiled != null && Lawyer.lawyer != null && Lawyer.target != null && Lawyer.isProsecutor && Lawyer.target.PlayerId == exiled.PlayerId && !Lawyer.lawyer.Data.IsDead)
                Lawyer.triggerProsecutorWin = true;*/

            // Mini exile lose condition
            if (exiled != null && Mini.mini != null && Mini.mini.PlayerId == exiled.PlayerId && !Mini.isGrownUp() && !Mini.mini.Data.Role.IsImpostor && !RoleInfo.getRoleInfoForPlayer(Mini.mini).Any(x => x.isNeutral)) {
                Mini.triggerMiniLose = true;
            }
            // Jester win condition
            else if (exiled != null && Jester.jester != null && Jester.jester.PlayerId == exiled.PlayerId) {
                Jester.triggerJesterWin = true;
            }


            // Reset custom button timers where necessary
            CustomButton.MeetingEndedUpdate();

            if (SchrodingersCat.schrodingersCat != null && PlayerControl.LocalPlayer == SchrodingersCat.schrodingersCat && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                SchrodingersCat.schrodingersCat.SetKillTimerUnchecked(SchrodingersCat.killCooldown);
            }

            // Mini set adapted cooldown
            if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini && Mini.mini.Data.Role.IsImpostor) {
                var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
                Mini.mini.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() * multiplier);
            }

            if (Mayor.mayor != null && PlayerControl.LocalPlayer == Mayor.mayor && exiled != null)
            {
                Mayor.acTokenChallenge.Value.cleared |= Mayor.acTokenChallenge.Value.votedFor == exiled.PlayerId && Mayor.acTokenChallenge.Value.doubleVote &&
                    ((Helpers.isEvil(ExileController.Instance.initData.networkedPlayer.Object) && Jester.jester != ExileController.Instance.initData.networkedPlayer.Object) || Madmate.madmate.Any(x => x.PlayerId == exiled.PlayerId)
                || CreatedMadmate.createdMadmate == ExileController.Instance.initData.networkedPlayer.Object);
            }

            if (Shifter.niceShifterAcTokenChallenge != null && PlayerControl.LocalPlayer.PlayerId == Shifter.niceShifterAcTokenChallenge.Value.oldShifterId && !Shifter.isNeutral &&
                exiled != null)
            {
                Shifter.niceShifterAcTokenChallenge.Value.cleared |= Shifter.niceShifterAcTokenChallenge.Value.shiftId == exiled.PlayerId;
            }

            if (Bait.bait != null && Bait.bait.Data.IsDead && PlayerControl.LocalPlayer == Bait.bait && exiled != null)
            {
                Bait.acTokenChallenge.Value.cleared |= Bait.acTokenChallenge.Value.killerId == exiled.PlayerId;
            }

            if (Detective.detective != null && !Detective.detective.Data.IsDead && PlayerControl.LocalPlayer == Detective.detective)
            {
                if (exiled != null)
                    Detective.acTokenChallenge.Value.cleared |= Detective.acTokenChallenge.Value.reported && Detective.acTokenChallenge.Value.votedFor == exiled.PlayerId;
                Detective.acTokenChallenge.Value.reported = false;
                Detective.acTokenChallenge.Value.killerId = byte.MaxValue;
            }

            if (Medic.medic != null && PlayerControl.LocalPlayer == Medic.medic)
            {
                if (exiled != null)
                    Medic.acTokenChallenge.Value.cleared |= Medic.acTokenChallenge.Value.killerId == exiled.PlayerId;
                Medic.acTokenChallenge.Value.killerId = byte.MaxValue;
            }

            if (Swapper.swapper != null && PlayerControl.LocalPlayer == Swapper.swapper && exiled != null)
            {
                if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                    Swapper.acTokenChallenge.Value.cleared |= (Swapper.acTokenChallenge.Value.swapped1 == exiled.PlayerId || Swapper.acTokenChallenge.Value.swapped2 == exiled.PlayerId)
                        && ExileController.Instance.initData.networkedPlayer.Object.Data.Role.IsImpostor;
                else
                {
                    bool swapped = Swapper.evilSwapperAcTokenChallenge.Value.swapped1 == exiled.PlayerId || Swapper.evilSwapperAcTokenChallenge.Value.swapped2 == exiled.PlayerId;
                    Swapper.evilSwapperAcTokenChallenge.Value.cleared |= swapped && !ExileController.Instance.initData.networkedPlayer.Object.Data.Role.IsImpostor && (Helpers.playerById(Swapper.evilSwapperAcTokenChallenge.Value.swapped1).Data.Role.IsImpostor || Helpers.playerById(
                        Swapper.evilSwapperAcTokenChallenge.Value.swapped2).Data.Role.IsImpostor);
                    if (swapped && Jester.jester == ExileController.Instance.initData.networkedPlayer.Object)
                        _ = new StaticAchievementToken("evilSwapper.another1");
                }
            }

            if (Yasuna.yasuna != null && PlayerControl.LocalPlayer == Yasuna.yasuna && exiled != null)
            {
                if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    if (Yasuna.yasunaAcTokenChallenge.Value.targetId == exiled.PlayerId)
                    {
                        PlayerControl exiledPlayer = Helpers.playerById(exiled.PlayerId);
                        Yasuna.yasunaAcTokenChallenge.Value.cleared |= Helpers.isEvil(exiledPlayer) && (((exiledPlayer == Lovers.lover1 ||
                                exiledPlayer == Lovers.lover2) && Lovers.lover1 && Lovers.lover2
                                && Helpers.isEvil(exiledPlayer == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1)) || ((exiledPlayer == Cupid.lovers1 || exiledPlayer == Cupid.lovers2) &&
                                Cupid.lovers1 && Cupid.lovers2 && Helpers.isEvil(exiledPlayer == Cupid.lovers1 ? Cupid.lovers2 : Cupid.lovers1)));
                        if (exiledPlayer == Jester.jester) _ = new StaticAchievementToken("niceYasuna.another1");
                        Yasuna.yasunaAcTokenChallenge.Value.targetId = byte.MaxValue;
                    }
                }
                else
                {
                    Yasuna.evilYasunaAcTokenChallenge.Value.cleared |= Yasuna.evilYasunaAcTokenChallenge.Value.targetId == exiled.PlayerId && !ExileController.Instance.initData.networkedPlayer.Object.Data.Role.IsImpostor;
                    Yasuna.evilYasunaAcTokenChallenge.Value.targetId = byte.MaxValue;
                }
            }

            // Seer spawn souls
            if (Seer.deadBodyPositions != null && Seer.seer != null && PlayerControl.LocalPlayer == Seer.seer && (Seer.mode == 0 || Seer.mode == 2)) {
                foreach (Vector3 pos in Seer.deadBodyPositions) {
                    GameObject soul = new();
                    //soul.transform.position = pos;
                    soul.transform.position = new Vector3(pos.x, pos.y, pos.y / 1000 - 1f);
                    soul.layer = 5;
                    var rend = soul.AddComponent<SpriteRenderer>();
                    soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    rend.sprite = Seer.getSoulSprite();
                    
                    if(Seer.limitSoulDuration) {
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Seer.soulDuration, new Action<float>((p) => {
                            if (rend != null) {
                                var tmp = rend.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                rend.color = tmp;
                            }    
                            if (p == 1f && rend != null && rend.gameObject != null) UnityEngine.Object.Destroy(rend.gameObject);
                        })));
                    }
                }
                Seer.deadBodyPositions = new List<Vector3>();
            }

            if (Morphling.morphling != null && PlayerControl.LocalPlayer == Morphling.morphling)
            {
                Morphling.acTokenChallenge.Value.cleared |= exiled != null && Morphling.acTokenChallenge.Value.kill && Morphling.acTokenChallenge.Value.playerId == exiled.PlayerId;
                Morphling.acTokenChallenge.Value.playerId = byte.MaxValue;
                Morphling.acTokenChallenge.Value.kill = false;
            }

            // Fortune Teller reset MeetingFlag
            if (FortuneTeller.fortuneTeller != null)
            {
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5.0f, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        FortuneTeller.meetingFlag = false;
                    }
                })));

                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    FortuneTeller.playerStatus[p.PlayerId] = !p.Data.IsDead;
                }

                if (PlayerControl.LocalPlayer == FortuneTeller.fortuneTeller)
                    FortuneTeller.acTokenImpostor.Value.cleared |= exiled != null && FortuneTeller.acTokenImpostor.Value.divined && FortuneTeller.divineTarget != null
                        && FortuneTeller.divineTarget.PlayerId == exiled.PlayerId;
            }

            if (PlagueDoctor.plagueDoctor != null)
            {
                PlagueDoctor.updateDead();

                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(PlagueDoctor.immunityTime, new Action<float>((p) =>
                { // 5秒後から感染開始
                    if (p == 1f)
                    {
                        PlagueDoctor.meetingFlag = false;
                    }
                })));
            }

            // Clear all traps
            Trap.clearAllTraps();
            Trapper.meetingFlag = false;

            // Reset Yasuna settings.
            if (Yasuna.yasuna != null)
                Yasuna.specialVoteTargetPlayerId = byte.MaxValue;

            // Tracker reset deadBodyPositions
            Tracker.deadBodyPositions = new List<Vector3>();

            // Blackmailer reset blackmail
            if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnblackmailPlayer, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unblackmailPlayer();
            }

            // Arsonist deactivate dead poolable players
            if (Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer) {
                var notDoused = new List<PlayerControl>();

                int visibleCounter = 0;
                Vector3 newBottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft;
                var BottomLeft = newBottomLeft + new Vector3(-0.25f, -0.25f, 0);
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    if (((!p.Data.IsDead && !p.Data.Disconnected) || (exiled != null && exiled.PlayerId == p.PlayerId)) && !Arsonist.dousedPlayers.Contains(p) && Arsonist.arsonist != p) notDoused.Add(p);
                    if (!TORMapOptions.playerIcons.ContainsKey(p.PlayerId) || Arsonist.arsonist == p) continue;
                    TORMapOptions.playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.2f;
                    if (p.Data.IsDead || p.Data.Disconnected) {
                        TORMapOptions.playerIcons[p.PlayerId].gameObject.SetActive(false);
                    } else {
                        TORMapOptions.playerIcons[p.PlayerId].transform.localPosition = BottomLeft + Vector3.right * visibleCounter * 0.35f;
                        TORMapOptions.playerIcons[p.PlayerId].gameObject.SetActive(true);
                        visibleCounter++;
                    }
                    if (!Arsonist.dousedPlayers.Contains(p)) TORMapOptions.playerIcons[p.PlayerId].setSemiTransparent(true);
                }

                if (notDoused.Count == 1 && exiled != null && notDoused[0].PlayerId == exiled.PlayerId)
                    _ = new StaticAchievementToken("arsonist.another1");
            }

            // Deputy check Promotion, see if the sheriff still exists. The promotion will be after the meeting.
            if (Deputy.deputy != null)
            {
                PlayerControlFixedUpdatePatch.deputyCheckPromotion(isMeeting: true);
            }

            // Force Bounty Hunter Bounty Update
            if (BountyHunter.bountyHunter != null && BountyHunter.bountyHunter == PlayerControl.LocalPlayer)
                BountyHunter.bountyUpdateTimer = 0f;

            // Medium spawn souls
            if (Medium.medium != null && PlayerControl.LocalPlayer == Medium.medium) {
                Medium.acTokenChallenge.Value.cleared |= exiled != null && Medium.acTokenChallenge.Value.additionals.Any(x => x == exiled.PlayerId);
                Medium.acTokenChallenge.Value.additionals = new();

                if (Medium.souls != null) {
                    foreach (SpriteRenderer sr in Medium.souls) UnityEngine.Object.Destroy(sr.gameObject);
                    Medium.souls = new List<SpriteRenderer>();
                }

                if (Medium.futureDeadBodies != null) {
                    foreach ((DeadPlayer db, Vector3 ps) in Medium.futureDeadBodies) {
                        GameObject s = new();
                        //s.transform.position = ps;
                        s.transform.position = new Vector3(ps.x, ps.y, ps.y / 1000 - 1f);
                        s.layer = 5;
                        var rend = s.AddComponent<SpriteRenderer>();
                        s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                        rend.sprite = Medium.getSoulSprite();
                        Medium.souls.Add(rend);
                    }
                    Medium.deadBodies = Medium.futureDeadBodies;
                    Medium.futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
                }
            }

            if (Lawyer.lawyer != null && PlayerControl.LocalPlayer == Lawyer.lawyer && !Lawyer.lawyer.Data.IsDead)
                Lawyer.meetings++;

            // AntiTeleport set position
            AntiTeleport.setPosition();

            // Remove DeadBodys
            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++)
            {
                UnityEngine.Object.Destroy(array[i].gameObject);
            }

            MapBehaviourPatch.resetRealTasks();

            if (CustomOptionHolder.randomGameStartPosition.getBool() && (AntiTeleport.antiTeleport.FindAll(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId).Count == 0))
            { //Random spawn on round start

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

            if (CustomOptionHolder.activateProps.getBool() && !CustomGameModes.FreePlayGM.isFreePlayGM) Props.placeProps();

            // Invert add meeting
            if (Invert.meetings > 0) Invert.meetings--;

            Chameleon.lastMoved?.Clear();

            /*foreach (Trap trap in Trap.traps) trap.triggerable = false;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2 + 2, new Action<float>((p) => {
            if (p == 1f) foreach (Trap trap in Trap.traps) trap.triggerable = true;
            })));*/

            if (!Yoyo.markStaysOverMeeting)
                Silhouette.clearSilhouettes();
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]  // Set position of AntiTp players AFTER they have selected a spawn.
    class AirshipSpawnInPatch {
        static void Postfix() {
            AntiTeleport.setPosition();
            Chameleon.lastMoved.Clear();
        }
    }

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    class ExileControllerMessagePatch {
        static void Postfix(ref string __result, [HarmonyArgument(0)]StringNames id) {
            try {
                if (ExileController.Instance != null && ExileController.Instance.initData != null)
                {
                    PlayerControl player = ExileController.Instance.initData.networkedPlayer.Object;
                    if (player == null) return;
                    // Exile role text
                    if (id is StringNames.ExileTextPN or StringNames.ExileTextSN or StringNames.ExileTextPP or StringNames.ExileTextSP) {
                        __result = player.Data.PlayerName + " was The " + String.Join(" ", RoleInfo.getRoleInfoForPlayer(player, false, includeHidden: true).Select(x => x.name).ToArray());
                    }
                    // Hide number of remaining impostors on Jester win
                    if (id is StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS) {
                        if (Jester.jester != null && player.PlayerId == Jester.jester.PlayerId) __result = "";
                    }
                    if (Yasuna.specialVoteTargetPlayerId != byte.MaxValue)
                    {
                        if (CustomOptionHolder.yasunaSpecificMessageMode.getBool()) __result += ModTranslation.getString("yasunaSpecialIndicator");
                        Tiebreaker.isTiebreak = false;
                        Yasuna.specialVoteTargetPlayerId = byte.MaxValue;
                    }
                    if (Tiebreaker.isTiebreak) __result += ModTranslation.getString("tiebreakerSpecialIndicator");
                    Tiebreaker.isTiebreak = false;
                }
            } catch {
                // pass - Hopefully prevent leaving while exiling to softlock game
            }
        }
    }
}
