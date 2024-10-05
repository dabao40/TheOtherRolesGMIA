using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.TORMapOptions;
using TheOtherRoles.Objects;
using System;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;
using Innersloth.Assets;
using Reactor.Utilities;
using AmongUs.QuickChat;
using Epic.OnlineServices.Presence;
using TheOtherRoles.Modules;
using Il2CppSystem.Reflection;
using TheOtherRoles.MetaContext;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace TheOtherRoles.Patches {
    [HarmonyPatch]
    class MeetingHudPatch {
        static bool[] selections;
        static SpriteRenderer[] renderers;
        private static NetworkedPlayerInfo target = null;
        private const float scale = 0.65f;
        private static TMPro.TextMeshPro meetingExtraButtonText;
        private static PassiveButton[] swapperButtonList;
        private static TMPro.TextMeshPro meetingExtraButtonLabel;
        private static PlayerVoteArea swapped1 = null;
        private static PlayerVoteArea swapped2 = null;
        static TMPro.TextMeshPro[] meetingInfoText = new TMPro.TextMeshPro[4];
        static int meetingTextIndex = 0;

        static private float[] VotingAreaScale = { 1f, 0.95f, 0.76f };
        static private (int x, int y)[] VotingAreaSize = { (3, 5), (3, 6), (4, 6) };
        static private Vector3[] VotingAreaOffset = { Vector3.zero, new(0.1f, 0.145f, 0f), new(-0.355f, 0f, 0f) };
        static private (float x, float y)[] VotingAreaMultiplier = { (1f, 1f), (1f, 0.89f), (0.974f, 1f) };
        static private int GetVotingAreaType(int players) => players <= 15 ? 0 : players <= 18 ? 1 : 2;
        private static Vector3 ToVoteAreaPos(int index, int arrangeType)
        {
            int x = index % VotingAreaSize[arrangeType].x;
            int y = index / VotingAreaSize[arrangeType].x;
            return
                MeetingHud.Instance.VoteOrigin + VotingAreaOffset[arrangeType] +
                new Vector3(
                    MeetingHud.Instance.VoteButtonOffsets.x * VotingAreaScale[arrangeType] * VotingAreaMultiplier[arrangeType].x * (float)x,
                    MeetingHud.Instance.VoteButtonOffsets.y * VotingAreaScale[arrangeType] * VotingAreaMultiplier[arrangeType].y * (float)y,
                    -0.9f - (float)y * 0.01f);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
        class MeetingCalculateVotesPatch {
            private static readonly List<byte> NonPlayerVotes = new() { 252, 254, byte.MaxValue };
            private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance) 
            {
                var votes = new Dictionary<byte, int>();
                var hasSwapper = Swapper.swapper != null && !Swapper.swapper.Data.IsDead;
                if (hasSwapper)
                {
                    swapped1 = null;
                    swapped2 = null;
                }
                foreach (var playerState in __instance.playerStates)
                {
                    if (NonPlayerVotes.Contains(playerState.VotedFor)) continue;
                    if (Helpers.playerById((byte)playerState.TargetPlayerId) == MimicA.mimicA && MimicK.mimicK != null && MimicK.hasOneVote && !MimicK.mimicK.Data.IsDead) continue;
                    if (Helpers.playerById((byte)playerState.TargetPlayerId) == BomberB.bomberB && BomberA.bomberA != null && BomberA.hasOneVote && !BomberA.bomberA.Data.IsDead) continue;
                    var amMayorEnabled = Mayor.mayor != null && Mayor.mayor.PlayerId == playerState.TargetPlayerId &&
                                         Mayor.voteTwice;
                    if (amMayorEnabled)
                        Mayor.unlockAch(playerState.VotedFor);
                    if (Detective.detective != null && Detective.detective.PlayerId == playerState.TargetPlayerId)
                        Detective.unlockAch(playerState.VotedFor);
                    var voteCount = amMayorEnabled ? 2 : 1;
                    votes[playerState.VotedFor] =
                        !votes.TryGetValue(playerState.VotedFor, out var num) ? voteCount : num + voteCount;
                    if (hasSwapper)
                    {
                        if (playerState.TargetPlayerId == Swapper.playerId1)
                        {
                            swapped1 = playerState;
                        }
                        if (playerState.TargetPlayerId == Swapper.playerId2)
                        {
                            swapped2 = playerState;
                        }
                    }
                }
                if (swapped1 != null && swapped2 != null)
                {
                    var swapped1VotesCount = votes.TryGetValue(swapped1.TargetPlayerId, out var voteCount1) ? voteCount1 : 0;
                    var swapped2ValueVotesCount = votes.TryGetValue(swapped2.TargetPlayerId, out var voteCount2) ? voteCount2 : 0;
                    votes[swapped1.TargetPlayerId] = swapped2ValueVotesCount;
                    votes[swapped2.TargetPlayerId] = swapped1VotesCount;
                }

                return votes;
            }


            static bool Prefix(MeetingHud __instance) {
                if (__instance.playerStates.All((PlayerVoteArea ps) => ps.AmDead || ps.DidVote)) {
                    // If skipping is disabled, replace skipps/no-votes with self vote
                    if (target == null && blockSkippingInEmergencyMeetings && noVoteIsSelfVote) {
                        foreach (PlayerVoteArea playerVoteArea in __instance.playerStates) {
                            if (playerVoteArea.VotedFor == byte.MaxValue - 1) playerVoteArea.VotedFor = playerVoteArea.TargetPlayerId; // TargetPlayerId
                        }
                    }

			        Dictionary<byte, int> self = CalculateVotes(__instance);
                    bool tie;
			        KeyValuePair<byte, int> max = self.MaxPair(out tie);
                    NetworkedPlayerInfo exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key && !v.IsDead);

                    // TieBreaker 
                    List<NetworkedPlayerInfo> potentialExiled = new();
                    bool skipIsTie = false;
                    if (self.Count > 0) {
                        Tiebreaker.isTiebreak = false;
                        int maxVoteValue = self.Values.Max();
                        PlayerVoteArea tb = null;
                        if (Tiebreaker.tiebreaker != null)
                            tb = __instance.playerStates.ToArray().FirstOrDefault(x => x.TargetPlayerId == Tiebreaker.tiebreaker.PlayerId);
                        bool isTiebreakerSkip = tb == null || tb.VotedFor == 253;
                        if (tb != null && tb.AmDead) isTiebreakerSkip = true;

                        foreach (KeyValuePair<byte, int> pair in self) {
                            if (pair.Value != maxVoteValue || isTiebreakerSkip) continue;
                            if (pair.Key != 253)
                                potentialExiled.Add(GameData.Instance.AllPlayers.ToArray().FirstOrDefault(x => x.PlayerId == pair.Key));
                            else 
                                skipIsTie = true;
                        }
                    }

                    byte forceTargetPlayerId = Yasuna.yasuna != null && !Yasuna.yasuna.Data.IsDead && Yasuna.specialVoteTargetPlayerId != byte.MaxValue ? Yasuna.specialVoteTargetPlayerId : byte.MaxValue;
                    if (forceTargetPlayerId != byte.MaxValue)
                        tie = false;

                    MeetingHud.VoterState[] array = new MeetingHud.VoterState[__instance.playerStates.Length];
                    for (int i = 0; i < __instance.playerStates.Length; i++)
                    {
                        PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                        if (forceTargetPlayerId != byte.MaxValue)
                            playerVoteArea.VotedFor = forceTargetPlayerId;

                        array[i] = new MeetingHud.VoterState {
                            VoterId = playerVoteArea.TargetPlayerId,
                            VotedForId = playerVoteArea.VotedFor
                        };

                        if (Tiebreaker.tiebreaker == null || playerVoteArea.TargetPlayerId != Tiebreaker.tiebreaker.PlayerId) continue;

                        byte tiebreakerVote = playerVoteArea.VotedFor;
                        if (swapped1 != null && swapped2 != null) {
                            if (tiebreakerVote == swapped1.TargetPlayerId) tiebreakerVote = swapped2.TargetPlayerId;
                            else if (tiebreakerVote == swapped2.TargetPlayerId) tiebreakerVote = swapped1.TargetPlayerId;
                        }

                        if (potentialExiled.FindAll(x => x != null && x.PlayerId == tiebreakerVote).Count > 0 && (potentialExiled.Count > 1 || skipIsTie)) {
                            exiled = potentialExiled.ToArray().FirstOrDefault(v => v.PlayerId == tiebreakerVote);
                            tie = false;

                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetTiebreak, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.setTiebreak();
                        }
                    }

                    if (forceTargetPlayerId != byte.MaxValue)
                        exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => v.PlayerId == forceTargetPlayerId && !v.IsDead);

                    // RPCVotingComplete
                    __instance.RpcVotingComplete(array, exiled, tie);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
        class MeetingHudBloopAVoteIconPatch {
            public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)]NetworkedPlayerInfo voterPlayer, [HarmonyArgument(1)]int index, [HarmonyArgument(2)]Transform parent) {
                var spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);
                var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes() ||
                                     (CachedPlayer.LocalPlayer.Data.IsDead && TORMapOptions.ghostsSeeVotes) ||
                                     (Mayor.mayor != null && Mayor.mayor == CachedPlayer.LocalPlayer.PlayerControl && Mayor.canSeeVoteColors && TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data).Item1 >= Mayor.tasksNeededToSeeVoteColors ||
                                     CachedPlayer.LocalPlayer.PlayerControl == Watcher.nicewatcher ||
                                     CachedPlayer.LocalPlayer.PlayerControl == Watcher.evilwatcher);
                if (showVoteColors)
                {
                    PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                }
                else
                {
                    PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
                }

                var transform = spriteRenderer.transform;
                transform.SetParent(parent);
                transform.localScale = Vector3.zero;
                var component = parent.GetComponent<PlayerVoteArea>();
                if (component != null)
                {
                    spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
                }

                __instance.StartCoroutine(Effects.Bloop(index * 0.3f, transform));
                parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy))]
        class OnDestroyPatch
        {
            public static void Postfix()
            {
                Modules.AntiCheat.MeetingTimes = 0;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
        class MeetingHudPopulateVotesPatch {
            
            static bool Prefix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states) {
                // Swapper swap

                PlayerVoteArea swapped1 = null;
                PlayerVoteArea swapped2 = null;
                foreach (PlayerVoteArea playerVoteArea in __instance.playerStates) {
                    if (playerVoteArea.TargetPlayerId == Swapper.playerId1) swapped1 = playerVoteArea;
                    if (playerVoteArea.TargetPlayerId == Swapper.playerId2) swapped2 = playerVoteArea;
                }
                bool doSwap = swapped1 != null && swapped2 != null && Swapper.swapper != null && !Swapper.swapper.Data.IsDead && Yasuna.specialVoteTargetPlayerId != swapped1.TargetPlayerId && Yasuna.specialVoteTargetPlayerId != swapped2.TargetPlayerId;

                if (doSwap) {
                    __instance.StartCoroutine(Effects.Slide3D(swapped1.transform, swapped1.transform.localPosition, swapped2.transform.localPosition, 1.5f));
                    __instance.StartCoroutine(Effects.Slide3D(swapped2.transform, swapped2.transform.localPosition, swapped1.transform.localPosition, 1.5f));
                }

                //if (Yasuna.specialVoteTargetPlayerId == swapped1.TargetPlayerId || Yasuna.specialVoteTargetPlayerId == swapped2.TargetPlayerId) Swapper.charges++;

                    __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                int num = 0;
                for (int i = 0; i < __instance.playerStates.Length; i++) {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    byte targetPlayerId = playerVoteArea.TargetPlayerId;
                    // Swapper change playerVoteArea that gets the votes
                    if (doSwap && playerVoteArea.TargetPlayerId == swapped1.TargetPlayerId) playerVoteArea = swapped2;
                    else if (doSwap && playerVoteArea.TargetPlayerId == swapped2.TargetPlayerId) playerVoteArea = swapped1;

                    playerVoteArea.ClearForResults();
                    int num2 = 0;
                    bool mayorFirstVoteDisplayed = false;
                    for (int j = 0; j < states.Length; j++) {
                        MeetingHud.VoterState voterState = states[j];
                        NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                        if (playerById == null) 
                        {
                            Debug.LogError(string.Format("Couldn't find player info for voter: {0}", voterState.VoterId));
                        }
                        else if (i == 0 && voterState.SkippedVote && !playerById.IsDead) {
                            __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);
                            num++;
                        }
                        else if (voterState.VotedForId == targetPlayerId && !playerById.IsDead) {
                            __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);
                            num2++;
                        }

                        // Major vote, redo this iteration to place a second vote
                        if (Mayor.mayor != null && voterState.VoterId == (sbyte)Mayor.mayor.PlayerId && !mayorFirstVoteDisplayed && Mayor.voteTwice) {
                            mayorFirstVoteDisplayed = true;
                            j--;    
                        }
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        class MeetingHudVotingCompletedPatch {
            static void Postfix(MeetingHud __instance, [HarmonyArgument(0)]byte[] states, [HarmonyArgument(1)]NetworkedPlayerInfo exiled, [HarmonyArgument(2)]bool tie)
            {
                // Reset swapper values
                Swapper.playerId1 = Byte.MaxValue;
                Swapper.playerId2 = Byte.MaxValue;

                // Disable meeting info text
                if (meetingInfoText != null)
                    foreach (var text in meetingInfoText)
                        text.gameObject.SetActive(false);

                // Lovers, Lawyer & Pursuer save next to be exiled, because RPC of ending game comes before RPC of exiled
                Lovers.notAckedExiledIsLover = false;
                Pursuer.notAckedExiled = false;
                if (exiled != null) {
                    Lovers.notAckedExiledIsLover = ((Lovers.lover1 != null && Lovers.lover1.PlayerId == exiled.PlayerId) || (Lovers.lover2 != null && Lovers.lover2.PlayerId == exiled.PlayerId));
                    
                    // Changed this: The Lawyer doesn't die if the target was ejected
                    Pursuer.notAckedExiled = (Pursuer.pursuer != null && Pursuer.pursuer.PlayerId == exiled.PlayerId);  //|| (Lawyer.lawyer != null && Lawyer.target != null && Lawyer.target.PlayerId == exiled.PlayerId && Lawyer.target != Jester.jester); // && !Lawyer.isProsecutor
                }

                if (Mayor.mayor != null && CachedPlayer.LocalPlayer.PlayerControl == Mayor.mayor) { 
                    if (Mayor.acTokenDoubleVote != null) {
                        Mayor.acTokenDoubleVote.Value.cleared |= Mayor.acTokenDoubleVote.Value.doubleVote;
                    }
                }

                // Yasuna
                if (Yasuna.isYasuna(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) && Yasuna.specialVoteTargetPlayerId == byte.MaxValue)
                {
                    for (int i = 0; i < __instance.playerStates.Length; i++)
                    {
                        PlayerVoteArea voteArea = __instance.playerStates[i];
                        Transform t = voteArea.transform.FindChild("SpecialVoteButton");
                        if (t != null)
                            t.gameObject.SetActive(false);
                    }
                }

                // Mini
                if (!Mini.isGrowingUpInMeeting) Mini.timeOfGrowthStart = Mini.timeOfGrowthStart.Add(DateTime.UtcNow.Subtract(Mini.timeOfMeetingStart)).AddSeconds(10);

                // Snitch
                if (Snitch.snitch != null && !Snitch.needsUpdate && Snitch.snitch.Data.IsDead && Snitch.text != null) {
                    UnityEngine.Object.Destroy(Snitch.text);
                }
            }
        }

        public static void SortVotingArea(MeetingHud __instance, Func<NetworkedPlayerInfo, int> rank, float speed = 1f)
        {
            int length = __instance.playerStates.Length;
            int type = GetVotingAreaType(length);
            __instance.playerStates.Do(p => p.transform.localScale = new(VotingAreaScale[type], VotingAreaScale[type], 1f));

            var ordered = __instance.playerStates.OrderBy(p => p.TargetPlayerId + 32 * rank.Invoke(GameData.Instance.GetPlayerById(p.TargetPlayerId))).ToArray();

            for (int i = 0; i < ordered.Length; i++)
                __instance.StartCoroutine(ordered[i].transform.Smooth(ToVoteAreaPos(i, type), 1.6f / speed).WrapToIl2Cpp());
        }


        static void swapperOnClick(int i, MeetingHud __instance) {
            if (__instance.state == MeetingHud.VoteStates.Results || Swapper.charges <= 0) return;
            if (__instance.playerStates[i].AmDead) return;

            int selectedCount = selections.Where(b => b).Count();
            SpriteRenderer renderer = renderers[i];

            if (selectedCount == 0) {
                renderer.color = Color.yellow;
                selections[i] = true;
            } else if (selectedCount == 1) {
                if (selections[i]) {
                    renderer.color = Color.red;
                    selections[i] = false;
                } else {
                    selections[i] = true;
                    renderer.color = Color.yellow;
                    meetingExtraButtonLabel.text = Helpers.cs(Color.yellow, ModTranslation.getString("swapperConfirmSwap"));
                }
            } else if (selectedCount == 2) {
                if (selections[i]) {
                    renderer.color = Color.red;
                    selections[i] = false;
                    meetingExtraButtonLabel.text = Helpers.cs(Color.red, ModTranslation.getString("swapperConfirmSwap"));
                }
            }
        }

        static void swapperConfirm(MeetingHud __instance) {
            __instance.playerStates[0].Cancel();  // This will stop the underlying buttons of the template from showing up
            if (__instance.state == MeetingHud.VoteStates.Results) return;
            if (selections.Where(b => b).Count() != 2) return;
            if (Swapper.charges <= 0 || Swapper.playerId1 != Byte.MaxValue) return;

            PlayerVoteArea firstPlayer = null;
            PlayerVoteArea secondPlayer = null;
            for (int A = 0; A < selections.Length; A++) {
                if (selections[A]) {
                    if (firstPlayer == null) {
                        firstPlayer = __instance.playerStates[A];
                    } else {
                        secondPlayer = __instance.playerStates[A];
                    }
                    renderers[A].color = Color.green;
                } else if (renderers[A] != null) {
                    renderers[A].color = Color.gray;
                    }
                if (swapperButtonList[A] != null) swapperButtonList[A].OnClick.RemoveAllListeners();  // Swap buttons can't be clicked / changed anymore
            }
            if (firstPlayer != null && secondPlayer != null) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SwapperSwap, Hazel.SendOption.Reliable, -1);
                writer.Write((byte)firstPlayer.TargetPlayerId);
                writer.Write((byte)secondPlayer.TargetPlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                RPCProcedure.swapperSwap((byte)firstPlayer.TargetPlayerId, (byte)secondPlayer.TargetPlayerId);
                if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
                {
                    Swapper.acTokenChallenge.Value.swapped1 = firstPlayer.TargetPlayerId;
                    Swapper.acTokenChallenge.Value.swapped2 = secondPlayer.TargetPlayerId;
                }
                else
                {
                    Swapper.evilSwapperAcTokenChallenge.Value.swapped1 = firstPlayer.TargetPlayerId;
                    Swapper.evilSwapperAcTokenChallenge.Value.swapped2 = secondPlayer.TargetPlayerId;
                }
                meetingExtraButtonLabel.text = Helpers.cs(Color.green, ModTranslation.getString("swapperSwapping"));
                Swapper.charges--;
                meetingExtraButtonText.text = string.Format(ModTranslation.getString("swapperRemainingSwaps"), Swapper.charges);
            }
        }

        public static void swapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId) {
            // someone was guessed or dced in the meeting, check if this affects the swapper.
            if (Swapper.swapper == null || __instance.state == MeetingHud.VoteStates.Results) return;

            // reset swap.
            bool reset = false;
            if (dyingPlayerId == Swapper.playerId1 || dyingPlayerId == Swapper.playerId2) {
                reset = true;
                Swapper.playerId1 = Swapper.playerId2 = byte.MaxValue;
            }
            

            // Only for the swapper: Reset all the buttons and charges value to their original state.
            if (CachedPlayer.LocalPlayer.PlayerControl != Swapper.swapper) return;


            // check if dying player was a selected player (but not confirmed yet)
            for (int i = 0; i < __instance.playerStates.Count; i++) {
                reset = reset || selections[i] && __instance.playerStates[i].TargetPlayerId == dyingPlayerId;
                if (reset) break;
            }

            if (!reset) return;


            for (int i = 0; i < selections.Length; i++) {
                selections[i] = false;
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers)) continue;
                renderers[i].color = Color.red;
                Swapper.charges++;
                int copyI = i;
                swapperButtonList[i].OnClick.RemoveAllListeners();
                swapperButtonList[i].OnClick.AddListener((System.Action)(() => swapperOnClick(copyI, __instance)));
            }
            meetingExtraButtonText.text = string.Format(ModTranslation.getString("swapperRemainingSwaps"), Swapper.charges);
            meetingExtraButtonLabel.text = Helpers.cs(Color.red, ModTranslation.getString("swapperConfirmSwap"));

        }

        static void mayorToggleVoteTwice(MeetingHud __instance) {
            __instance.playerStates[0].Cancel();  // This will stop the underlying buttons of the template from showing up
            if (__instance.state == MeetingHud.VoteStates.Results || Mayor.mayor.Data.IsDead) return;
            if (Mayor.mayorChooseSingleVote == 1) { // Only accept changes until the mayor voted
                var mayorPVA = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Mayor.mayor.PlayerId);
                if (mayorPVA != null && mayorPVA.DidVote) {
                    SoundEffectsManager.play("fail");
                    return;
                }
            }

            Mayor.voteTwice = !Mayor.voteTwice;

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MayorSetVoteTwice, Hazel.SendOption.Reliable, -1);
            writer.Write(Mayor.voteTwice);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            meetingExtraButtonLabel.text = Helpers.cs(Mayor.color, ModTranslation.getString("mayorDoubleVote") + (Mayor.voteTwice ? Helpers.cs(Color.green, ModTranslation.getString("mayorDoubleVoteOn")) : Helpers.cs(Color.red, ModTranslation.getString("mayorDoubleVoteOff"))));
        }

        public static GameObject guesserUI;
        public static PassiveButton guesserUIExitButton;
        public static byte guesserCurrentTarget;
        static void guesserOnClick(int buttonTarget, MeetingHud __instance) {
            if (guesserUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

            Transform PhoneUI = UnityEngine.Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            Transform container = UnityEngine.Object.Instantiate(PhoneUI, __instance.transform);
            container.transform.localPosition = new Vector3(0, 0, -5f);
            guesserUI = container.gameObject;

            int i = 0;
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
            var textTemplate = __instance.playerStates[0].NameText;

            guesserCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

            Transform exitButtonParent = (new GameObject()).transform;
            exitButtonParent.SetParent(container);
            Transform exitButton = UnityEngine.Object.Instantiate(buttonTemplate.transform, exitButtonParent);
            Transform exitButtonMask = UnityEngine.Object.Instantiate(maskTemplate, exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
            exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
            exitButtonParent.transform.localScale = new Vector3(0.217f, 0.9f, 1);
            guesserUIExitButton = exitButton.GetComponent<PassiveButton>();
            guesserUIExitButton.OnClick.RemoveAllListeners();
            guesserUIExitButton.OnClick.AddListener((System.Action)(() => {
                __instance.playerStates.ToList().ForEach(x => {
                    x.gameObject.SetActive(true);
                    if (CachedPlayer.LocalPlayer.Data.IsDead && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                });
                UnityEngine.Object.Destroy(container.gameObject);
            }));

            List<Transform> buttons = new();
            Transform selectedButton = null;

            foreach (RoleInfo roleInfo in RoleInfo.allRoleInfos) {
                RoleId guesserRole = (Guesser.niceGuesser != null && CachedPlayer.LocalPlayer.PlayerId == Guesser.niceGuesser.PlayerId) ? RoleId.NiceGuesser :  RoleId.EvilGuesser;
                if (roleInfo.isModifier || roleInfo.roleId == guesserRole || (!HandleGuesser.evilGuesserCanGuessSpy && guesserRole == RoleId.EvilGuesser && roleInfo.roleId == RoleId.Spy && !HandleGuesser.isGuesserGm)) continue; // Not guessable roles & modifier
                if (HandleGuesser.isGuesserGm && (roleInfo.roleId == RoleId.NiceGuesser || roleInfo.roleId == RoleId.EvilGuesser)) continue; // remove Guesser for guesser game mode
                if (HandleGuesser.isGuesserGm && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor && !HandleGuesser.evilGuesserCanGuessSpy && roleInfo.roleId == RoleId.Spy) continue;
                // remove all roles that cannot spawn due to the settings from the ui.
                RoleManagerSelectRolesPatch.RoleAssignmentData roleData = RoleManagerSelectRolesPatch.getRoleAssignmentData();
                if (roleData.neutralSettings.ContainsKey((byte)roleInfo.roleId) && roleData.neutralSettings[(byte)roleInfo.roleId] == 0) continue;
                else if (roleData.impSettings.ContainsKey((byte)roleInfo.roleId) && roleData.impSettings[(byte)roleInfo.roleId] == 0) continue;
                else if (roleData.crewSettings.ContainsKey((byte)roleInfo.roleId) && roleData.crewSettings[(byte)roleInfo.roleId] == 0) continue;
                else if (new List<RoleId>() { RoleId.Janitor, RoleId.Godfather, RoleId.Mafioso }.Contains(roleInfo.roleId) && (CustomOptionHolder.mafiaSpawnRate.getSelection() == 0 || GameOptionsManager.Instance.currentGameOptions.NumImpostors < 3)) continue;
                else if (roleInfo.roleId == RoleId.Sidekick && (!CustomOptionHolder.jackalCanCreateSidekick.getBool() || CustomOptionHolder.jackalSpawnRate.getSelection() == 0)) continue;
                else if (new List<RoleId>() { RoleId.MimicA, RoleId.MimicK }.Contains(roleInfo.roleId) && CustomOptionHolder.mimicSpawnRate.getSelection() == 0) continue;
                else if (roleInfo.roleId == RoleId.BomberA && CustomOptionHolder.bomberSpawnRate.getSelection() == 0) continue;
                if (roleInfo.roleId == RoleId.Deputy && (CustomOptionHolder.deputySpawnRate.getSelection() == 0 || CustomOptionHolder.sheriffSpawnRate.getSelection() == 0)) continue;
                if (roleInfo.roleId == RoleId.Pursuer && CustomOptionHolder.lawyerSpawnRate.getSelection() == 0) continue;
                if (roleInfo.roleId == RoleId.Immoralist && CustomOptionHolder.foxSpawnRate.getSelection() == 0) continue;
                if (roleInfo.roleId == RoleId.Spy && roleData.impostors.Count <= 1) continue;
                if (roleInfo.roleId == RoleId.BomberB) continue;
                //if (roleInfo.roleId == RoleId.Prosecutor && (CustomOptionHolder.lawyerIsProsecutorChance.getSelection() == 0 || CustomOptionHolder.lawyerSpawnRate.getSelection() == 0)) continue;
                //if (roleInfo.roleId == RoleId.Lawyer && CustomOptionHolder.lawyerSpawnRate.getSelection() == 0) continue;
                if (Snitch.snitch != null && HandleGuesser.guesserCantGuessSnitch) {
                    var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                    int numberOfLeftTasks = playerTotal - playerCompleted;
                    if (numberOfLeftTasks <= 0 && roleInfo.roleId == RoleId.Snitch) continue;
                }

                Transform buttonParent = (new GameObject()).transform;
                buttonParent.SetParent(container);
                Transform button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
                Transform buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
                TMPro.TextMeshPro label = UnityEngine.Object.Instantiate(textTemplate, button);
                button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
                buttons.Add(button);
                int row = i/5, col = i%5;
                buttonParent.localPosition = new Vector3(-3.47f + 1.55f * col, 1.5f - 0.35f * row, -5);
                buttonParent.localScale = new Vector3(0.45f, 0.45f, 1f);
                label.text = Helpers.cs(roleInfo.color, roleInfo.name);
                label.alignment = TMPro.TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale *= 1.7f;
                int copiedIndex = i;

                button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
                if (!CachedPlayer.LocalPlayer.Data.IsDead && !Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId).Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((System.Action)(() => {
                    if (selectedButton != button) {
                        selectedButton = button;
                        buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    } else {
                        PlayerControl focusedTarget = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
                        if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted) || focusedTarget == null || HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) <= 0 ) return;

                        if (!HandleGuesser.killsThroughShield && focusedTarget == Medic.shielded) { // Depending on the options, shooting the shielded player will not allow the guess, notifiy everyone about the kill attempt and close the window
                            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true)); 
                            UnityEngine.Object.Destroy(container.gameObject);

                            MessageWriter murderAttemptWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(murderAttemptWriter);
                            RPCProcedure.shieldedMurderAttempt();
                            SoundEffectsManager.play("fail");
                            return;
                        }

                        var mainRoleInfo = RoleInfo.getRoleInfoForPlayer(focusedTarget, false, includeHidden: true).FirstOrDefault();
                        if (mainRoleInfo == null) return;

                        PlayerControl dyingTarget = ((mainRoleInfo == roleInfo) || (roleInfo == RoleInfo.bomberA && mainRoleInfo == RoleInfo.bomberB)) ? focusedTarget : CachedPlayer.LocalPlayer.PlayerControl;

                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        UnityEngine.Object.Destroy(container.gameObject);
                        if (HandleGuesser.hasMultipleShotsPerMeeting && HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) > 1 && dyingTarget != CachedPlayer.LocalPlayer.PlayerControl)
                            __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                        else
                            __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                        bool isSpecialRole = roleInfo == RoleInfo.niceshifter || roleInfo == RoleInfo.niceSwapper;
                        // Shoot player and send chat info if activated
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GuesserShoot, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write(dyingTarget.PlayerId);
                        writer.Write(focusedTarget.PlayerId);
                        writer.Write((byte)roleInfo.roleId);
                        writer.Write(isSpecialRole);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.guesserShoot(CachedPlayer.LocalPlayer.PlayerId, dyingTarget.PlayerId, focusedTarget.PlayerId, (byte)roleInfo.roleId, isSpecialRole);
                    }
                }));

                i++;
            }
            container.transform.localScale *= 0.75f;
        }

        static void yasunaOnClick(int buttonTarget, MeetingHud __instance)
        {
            if (Yasuna.yasuna != null && (Yasuna.yasuna.Data.IsDead || Yasuna.specialVoteTargetPlayerId != byte.MaxValue)) return;
            if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted || __instance.state == MeetingHud.VoteStates.Results)) return;
            if (__instance.playerStates[buttonTarget].AmDead) return;

            var yasunaPVA = __instance.playerStates.FirstOrDefault(t => t.TargetPlayerId == Yasuna.yasuna.PlayerId);
            if (yasunaPVA != null && yasunaPVA.DidVote)
            {
                SoundEffectsManager.play("fail");
                return;
            }

            byte targetId = __instance.playerStates[buttonTarget].TargetPlayerId;
            RPCProcedure.yasunaSpecialVote(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, targetId);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.YasunaSpecialVote, Hazel.SendOption.Reliable, -1);
            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
            writer.Write(targetId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor) Yasuna.yasunaAcTokenChallenge.Value.targetId = targetId;
            else Yasuna.evilYasunaAcTokenChallenge.Value.targetId = targetId;

            __instance.SkipVoteButton.gameObject.SetActive(false);
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea voteArea = __instance.playerStates[i];
                voteArea.ClearButtons();
                Transform t = voteArea.transform.FindChild("SpecialVoteButton");
                if (t != null && voteArea.TargetPlayerId != targetId)
                    t.gameObject.SetActive(false);
            }
            if (AmongUsClient.Instance.AmHost)
            {
                PlayerControl target = Helpers.playerById(targetId);
                if (target != null)
                    MeetingHud.Instance.CmdCastVote(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, target.PlayerId);
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
        class PlayerVoteAreaSelectPatch {
            static bool Prefix(MeetingHud __instance) {
                //return !(CachedPlayer.LocalPlayer != null && ((HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId) && guesserUI != null) || (Yasuna.isYasuna(CachedPlayer.LocalPlayer.PlayerId) && Yasuna.specialVoteTargetPlayerId != byte.MaxValue)));
                return !(CachedPlayer.LocalPlayer != null && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId) && guesserUI != null);
            }
        }

        static void populateButtonsPostfix(MeetingHud __instance) {
            // Add Swapper Buttons
            bool addSwapperButtons = Swapper.swapper != null && CachedPlayer.LocalPlayer.PlayerControl == Swapper.swapper && !Swapper.swapper.Data.IsDead;
            bool addMayorButton = Mayor.mayor != null && CachedPlayer.LocalPlayer.PlayerControl == Mayor.mayor && !Mayor.mayor.Data.IsDead && Mayor.mayorChooseSingleVote > 0;
            if (addSwapperButtons) {
                selections = new bool[__instance.playerStates.Length];
                renderers = new SpriteRenderer[__instance.playerStates.Length];
                swapperButtonList = new PassiveButton[__instance.playerStates.Length];

                for (int i = 0; i < __instance.playerStates.Length; i++) {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers)) continue;

                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject checkbox = UnityEngine.Object.Instantiate(template);
                    checkbox.transform.SetParent(playerVoteArea.transform);
                    checkbox.transform.position = template.transform.position;
                    checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId)) checkbox.transform.localPosition = new Vector3(-0.5f, 0.03f, -1.3f);
                    SpriteRenderer renderer = checkbox.GetComponent<SpriteRenderer>();
                    renderer.sprite = Swapper.getCheckSprite();
                    renderer.color = Color.red;

                    if (Swapper.charges <= 0) renderer.color = Color.gray;

                    PassiveButton button = checkbox.GetComponent<PassiveButton>();
                    swapperButtonList[i] = button;
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((System.Action)(() => swapperOnClick(copiedIndex, __instance)));
                    button.OnMouseOver.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(button, new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.Instance.GetAttribute(AttributeAsset.OverlayContent),
                        new RawTextComponent(string.Format(ModTranslation.getString("buttonLeftClick"), ModTranslation.getString("buttonSwap")))))));
                    button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));

                    selections[i] = false;
                    renderers[i] = renderer;
                }
            }

            // Add meeting extra button, i.e. Swapper Confirm Button or Mayor Toggle Double Vote Button. Swapper Button uses ExtraButtonText on the Left of the Button. (Future meeting buttons can easily be added here)
            if (addSwapperButtons || addMayorButton) {
                Transform meetingUI = UnityEngine.Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

                var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
                var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
                var textTemplate = __instance.playerStates[0].NameText;
                Transform meetingExtraButtonParent = (new GameObject()).transform;
                meetingExtraButtonParent.SetParent(meetingUI);
                Transform meetingExtraButton = UnityEngine.Object.Instantiate(buttonTemplate, meetingExtraButtonParent);

                Transform infoTransform = __instance.playerStates[0].NameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro meetingInfo = infoTransform != null ? infoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                meetingExtraButtonText = UnityEngine.Object.Instantiate(__instance.playerStates[0].NameText, meetingExtraButtonParent);
                meetingExtraButtonText.text = addSwapperButtons ? string.Format(ModTranslation.getString("swapperRemainingSwaps"), Swapper.charges) : "";
                meetingExtraButtonText.enableWordWrapping = false;
                meetingExtraButtonText.transform.localScale = Vector3.one * 1.7f;
                meetingExtraButtonText.transform.localPosition = new Vector3(-2.5f, 0f, 0f);

                Transform meetingExtraButtonMask = UnityEngine.Object.Instantiate(maskTemplate, meetingExtraButtonParent);
                meetingExtraButtonLabel = UnityEngine.Object.Instantiate(textTemplate, meetingExtraButton);
                meetingExtraButton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

                meetingExtraButtonParent.localPosition = new Vector3(0, -2.225f, -5);
                meetingExtraButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
                meetingExtraButtonLabel.alignment = TMPro.TextAlignmentOptions.Center;
                meetingExtraButtonLabel.transform.localPosition = new Vector3(0, 0, meetingExtraButtonLabel.transform.localPosition.z);
                if (addSwapperButtons) {
                    meetingExtraButtonLabel.transform.localScale *= 1.7f;
                    meetingExtraButtonLabel.text = Helpers.cs(Color.red, ModTranslation.getString("swapperConfirmSwap"));
                } else if (addMayorButton) {
                    meetingExtraButtonLabel.transform.localScale = new Vector3(meetingExtraButtonLabel.transform.localScale.x * 1.5f, meetingExtraButtonLabel.transform.localScale.x * 1.7f, meetingExtraButtonLabel.transform.localScale.x * 1.7f);
                    meetingExtraButtonLabel.text = Helpers.cs(Mayor.color, ModTranslation.getString("mayorDoubleVote") + (Mayor.voteTwice ? Helpers.cs(Color.green, ModTranslation.getString("mayorDoubleVoteOn")) : Helpers.cs(Color.red, ModTranslation.getString("mayorDoubleVoteOff"))));
                }
                PassiveButton passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                if (!CachedPlayer.LocalPlayer.Data.IsDead) {
                    if (addSwapperButtons)
                        passiveButton.OnClick.AddListener((Action)(() => swapperConfirm(__instance)));
                    else if (addMayorButton)
                        passiveButton.OnClick.AddListener((Action)(() => mayorToggleVoteTwice(__instance)));
                }
                meetingExtraButton.parent.gameObject.SetActive(false);
                __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>((p) => { // Button appears delayed, so that its visible in the voting screen only!
                    if (p == 1f) {
                        meetingExtraButton.parent.gameObject.SetActive(true);
                    }
                })));
            }

            //Fix visor in Meetings 
            /**
            foreach (PlayerVoteArea pva in __instance.playerStates) {
                if(pva.PlayerIcon != null && pva.PlayerIcon.VisorSlot != null){
                    pva.PlayerIcon.VisorSlot.transform.position += new Vector3(0, 0, -1f);
                }
            } */

            bool isGuesser = HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId);

            // Add overlay for spelled players
            if (Witch.witch != null && Witch.futureSpelled != null) {
                foreach (PlayerVoteArea pva in __instance.playerStates) {
                    if (Witch.futureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId)) {
                        SpriteRenderer rend = (new GameObject()).AddComponent<SpriteRenderer>();
                        rend.transform.SetParent(pva.transform);
                        rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                        rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
                        if (CachedPlayer.LocalPlayer.PlayerControl == Swapper.swapper && isGuesser) rend.transform.localPosition = new Vector3(-0.725f, -0.15f, -1f);
                        rend.sprite = Witch.getSpelledOverlaySprite();
                    }
                }
            }

            // Add Track Button for Evil Tracker
            bool isTrackerButton = EvilTracker.canSetTargetOnMeeting && (EvilTracker.target == null || EvilTracker.resetTargetAfterMeeting) && CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
            if (isTrackerButton)
            {
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) continue;
                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "EvilTrackerButton";
                    targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId)) targetBox.transform.localPosition = new Vector3(-0.5f, 0.03f, -1.3f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = EvilTracker.getArrowSprite();
                    renderer.color = Palette.CrewmateBlue;
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((System.Action)(() =>
                    {
                        PlayerControl focusedTarget = Helpers.playerById((byte)__instance.playerStates[copiedIndex].TargetPlayerId);
                        EvilTracker.futureTarget = EvilTracker.target = focusedTarget;
                        EvilTracker.acTokenCommon1 ??= new("evilTracker.common1");
                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("EvilTrackerButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("EvilTrackerButton").gameObject); });
                        GameObject targetMark = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                        targetMark.name = "EvilTrackerMark";
                        PassiveButton button = targetMark.GetComponent<PassiveButton>();
                        targetMark.transform.localPosition = new Vector3(1.1f, 0.03f, -20f);                        
                        GameObject.Destroy(button);
                        SpriteRenderer renderer = targetMark.GetComponent<SpriteRenderer>();
                        renderer.sprite = EvilTracker.getArrowSprite();
                        renderer.color = Palette.CrewmateBlue;
                    }));
                    button.OnMouseOver.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(button, new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.Instance.GetAttribute(AttributeAsset.OverlayContent),
                        new RawTextComponent(string.Format(ModTranslation.getString("buttonLeftClick"), ModTranslation.getString("buttonTrack")))))));
                    button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));
                }
            }

            // Add Guesser Buttons
            int remainingShots = HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId);

            if (isGuesser && !CachedPlayer.LocalPlayer.Data.IsDead && remainingShots > 0) {
                for (int i = 0; i < __instance.playerStates.Length; i++) {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerId) continue;
                    if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl == Eraser.eraser && Eraser.alreadyErased.Contains(playerVoteArea.TargetPlayerId)) continue;

                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "ShootButton";
                    targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = HandleGuesser.getTargetSprite();
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((System.Action)(() => guesserOnClick(copiedIndex, __instance)));
                    button.OnMouseOver.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(button, new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.Instance.GetAttribute(AttributeAsset.OverlayContent),
                        new RawTextComponent(string.Format(ModTranslation.getString("buttonLeftClick"), ModTranslation.getString("buttonGuess")))))));
                    button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));
                }
            }

            // Add Yasuna Special Buttons
            if (Yasuna.isYasuna(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) && !Yasuna.yasuna.Data.IsDead && Yasuna.remainingSpecialVotes() > 0)
            {
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) continue;

                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "SpecialVoteButton";
                    targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -2.5f);
                    if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId)) targetBox.transform.localPosition = new Vector3(-0.5f, 0.03f, -1.3f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = Yasuna.getTargetSprite(CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor);
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => yasunaOnClick(copiedIndex, __instance)));
                    button.OnMouseOver.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(button, new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.Instance.GetAttribute(AttributeAsset.OverlayContent),
                        new RawTextComponent(string.Format(ModTranslation.getString("buttonLeftClick"), ModTranslation.getString("buttonForceExile")))))));
                    button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));
                }
            }
        }

        public static void updateMeetingText(MeetingHud __instance)
        {
            // Uses remaining text for guesser/yasuna etc.
            if (meetingInfoText[0] == null)
            {
                for (int i = 0; i < meetingInfoText.Length; i++)
                {
                    meetingInfoText[i] = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                    meetingInfoText[i].alignment = TMPro.TextAlignmentOptions.BottomLeft;
                    meetingInfoText[i].transform.position = Vector3.zero;
                    meetingInfoText[i].transform.localPosition = new Vector3(-3.07f, 3.33f, -20f);
                    meetingInfoText[i].transform.localScale *= 1.1f;
                    meetingInfoText[i].color = Palette.White;
                    meetingInfoText[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < meetingInfoText.Length; i++)
            {
                meetingInfoText[i].text = "";
                meetingInfoText[i].gameObject.SetActive(false);
            }

            if (MeetingHud.Instance.state is not MeetingHud.VoteStates.Voted and
                not MeetingHud.VoteStates.NotVoted and
                not MeetingHud.VoteStates.Discussion)
                return;

            int numGuesses = HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) ? HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerControl.PlayerId) : 0;
            if (numGuesses > 0 && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
            {
                meetingInfoText.getFirst().text = string.Format(ModTranslation.getString("guesserGuessesLeft"), numGuesses);
            }

            int numSpecialVotes = Yasuna.isYasuna(CachedPlayer.LocalPlayer.PlayerId) ? Yasuna.remainingSpecialVotes() : 0;
            if (numSpecialVotes > 0 && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
            {
                meetingInfoText.getFirst().text = string.Format(ModTranslation.getString("yasunaSpecialVotes"), numSpecialVotes);
            }

            if (!CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && Akujo.timeLeft > 0)
            {
                meetingInfoText.getFirst().text = string.Format(ModTranslation.getString("akujoTimeRemaining"), $"{TimeSpan.FromSeconds(Akujo.timeLeft):mm\\:ss}");
            }

            if (!CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl == Cupid.cupid && Cupid.timeLeft > 0)
            {
                meetingInfoText.getFirst().text = string.Format(ModTranslation.getString("akujoTimeRemaining"), $"{TimeSpan.FromSeconds(Cupid.timeLeft):mm\\:ss}");
            }

            if (CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker && EvilTracker.target != null)
            {
                meetingInfoText.getFirst().text = string.Format(ModTranslation.getString("evilTrackerCurrentTarget"), EvilTracker.target?.Data?.PlayerName ?? "");
            }

            if (!CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && Lawyer.winsAfterMeetings)
            {
                meetingInfoText.getFirst().text = Lawyer.neededMeetings - Lawyer.meetings > 1 ? string.Format(ModTranslation.getString("lawyerMeetingInfo"), Lawyer.neededMeetings - Lawyer.meetings - 1) : ModTranslation.getString("lawyerAboutToWin");
            }

            meetingInfoText[meetingTextIndex].gameObject.SetActive(true);
            if (meetingInfoText.totalCounts() == 0) return;
            if (meetingTextIndex + 1 > meetingInfoText.totalCounts())
                meetingTextIndex = meetingInfoText.totalCounts() - 1;
        }

        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class MeetingTextUpdatePatch
        {
            public static void Postfix(KeyboardJoystick __instance)
            {
                if (meetingInfoText[0] == null || meetingInfoText.totalCounts() == 0) return;
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    meetingTextIndex = (meetingTextIndex + 1) % meetingInfoText.totalCounts();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                {
                    meetingTextIndex = 0;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
                    meetingTextIndex = 1 % meetingInfoText.totalCounts();
                }
                if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                {
                    meetingTextIndex = meetingInfoText.totalCounts() <= 3 ? meetingInfoText.totalCounts() - 1 : 2;
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
        class MeetingServerStartPatch {
            static void Postfix(MeetingHud __instance)
            {
                SortVotingArea(__instance, p => p.IsDead || p.Disconnected ? 2 : 1, 10f);
                populateButtonsPostfix(__instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
        class MeetingDeserializePatch {
            static void Postfix(MeetingHud __instance, [HarmonyArgument(0)]MessageReader reader, [HarmonyArgument(1)]bool initialState)
            {
                // Add swapper buttons
                if (initialState) {
                    populateButtonsPostfix(__instance);
                    SortVotingArea(__instance, p => p.IsDead || p.Disconnected ? 2 : 1, 10f);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]NetworkedPlayerInfo meetingTarget) {
                RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                byte roomId = Byte.MinValue;
                if (roomTracker != null && roomTracker.LastRoom != null) {
                    roomId = (byte)roomTracker.LastRoom?.RoomId;
                }
                if (Snitch.snitch != null && roomTracker != null) {
                    MessageWriter roomWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareRoom, Hazel.SendOption.Reliable, -1);
                    roomWriter.Write(CachedPlayer.LocalPlayer.PlayerId);
                    roomWriter.Write(roomId);
                    AmongUsClient.Instance.FinishRpcImmediately(roomWriter);
                }

                // Resett Bait list
                //Bait.active = new Dictionary<DeadPlayer, float>();
                // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
                if (CachedPlayer.LocalPlayer.PlayerPhysics.enabled && (CachedPlayer.LocalPlayer.PlayerControl.moveable || CachedPlayer.LocalPlayer.PlayerControl.inVent
                    || HudManagerStartPatch.hackerVitalsButton.isEffectActive || HudManagerStartPatch.hackerAdminTableButton.isEffectActive || HudManagerStartPatch.securityGuardCamButton.isEffectActive
                    || Portal.isTeleporting && Portal.teleportedPlayers.Last().playerId == CachedPlayer.LocalPlayer.PlayerId))
                {
                    if (!CachedPlayer.LocalPlayer.PlayerControl.inMovingPlat)
                        AntiTeleport.position = CachedPlayer.LocalPlayer.transform.position;
                }

                // Save real tasks
                MapBehaviourPatch.shareRealTasks();

                // Medium meeting start time
                Medium.meetingStartTime = DateTime.UtcNow;
                // Mini
                Mini.timeOfMeetingStart = DateTime.UtcNow;
                Mini.ageOnMeetingStart = Mathf.FloorToInt(Mini.growingProgress() * 18);
                // Reset vampire bitten
                Vampire.bitten = null;
                // Count meetings
                if (meetingTarget == null) meetingsCount++;
                // Save the meeting target
                target = meetingTarget;
                meetingTextIndex = 0;

                BomberA.bombTarget = null;
                BomberB.bombTarget = null;

                TranslatableTag tag = meetingTarget == null ? EventDetail.EmergencyButton : EventDetail.Report;
                if (meetingTarget != null)
                {
                    var player = Helpers.playerById(meetingTarget.PlayerId);
                    if (Bait.bait != null && player == Bait.bait && Bait.reportDelay <= 0f)
                        tag = EventDetail.BaitReport;
                }
                GameStatistics.Event.GameStatistics.RecordEvent(new GameStatistics.Event(
            meetingTarget == null ? GameStatistics.EventVariation.EmergencyButton : GameStatistics.EventVariation.Report, __instance.PlayerId,
                meetingTarget == null ? 0 : (1 << meetingTarget.PlayerId)) { RelatedTag = tag });

                if (CachedPlayer.LocalPlayer.PlayerControl == Mayor.mayor) {
                    if (Mayor.acTokenDoubleVote != null) {
                        Mayor.acTokenDoubleVote.Value.doubleVote = false;
                    }
                    if (Mayor.acTokenChallenge != null) {
                        Mayor.acTokenChallenge.Value.doubleVote = false;
                    }
                }

                if (CachedPlayer.LocalPlayer.PlayerControl == Swapper.swapper)
                {
                    if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
                    {
                        Swapper.acTokenChallenge.Value.swapped1 = byte.MaxValue;
                        Swapper.acTokenChallenge.Value.swapped2 = byte.MaxValue;
                    }
                    else
                    {
                        Swapper.evilSwapperAcTokenChallenge.Value.swapped1 = byte.MaxValue;
                        Swapper.evilSwapperAcTokenChallenge.Value.swapped2 = byte.MaxValue;
                    }
                }

                if (CachedPlayer.LocalPlayer.PlayerControl == Seer.seer)
                {
                    Seer.acTokenChallenge.Value.cleared |= Seer.acTokenChallenge.Value.flash >= 5;
                    Seer.acTokenChallenge.Value.flash = 0;
                }

                if (CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch)
                    Snitch.acTokenChallenge.Value.cleared |= Snitch.acTokenChallenge.Value.taskComplete && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;

                if (CachedPlayer.LocalPlayer.PlayerControl == Vampire.vampire)
                    Vampire.acTokenChallenge.Value.cleared |= DateTime.UtcNow.Subtract(Vampire.acTokenChallenge.Value.deathTime).TotalSeconds <= 3;

                // Fortune Teller set MeetingFlag
                FortuneTeller.meetingFlag = true;
                PlagueDoctor.meetingFlag = true;

                // Reset the victim for Mimic(Killer)
                MimicK.victim = null;
                MimicA.isMorph = false;

                if (Busker.busker != null && CachedPlayer.LocalPlayer.PlayerControl == Busker.busker)
                {
                    if (Busker.pseudocideFlag)
                        Busker.dieBusker();
                    else
                        Busker.acTokenChallenge.Value.cleared |= DateTime.UtcNow.Subtract(Busker.acTokenChallenge.Value.pseudocide).TotalSeconds <= 3f && __instance != Busker.busker;
                }

                // Blackmail target
                if (Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
                {
                    Coroutines.Start(Helpers.BlackmailShhh());
                }

                if (Moriarty.moriarty != null && Moriarty.hasKilled && FastDestroyableSingleton<HudManager>.Instance != null && AmongUsClient.Instance.AmClient && Moriarty.indicateKills)
                {
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, ModTranslation.getString("moriartyIndicator"));
                    Moriarty.hasKilled = false;
                }

                // Add Portal info into Portalmaker Chat:
                if (Portalmaker.portalmaker != null && (CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker || Helpers.shouldShowGhostInfo()) && !Portalmaker.portalmaker.Data.IsDead) {
                    if (Portal.teleportedPlayers.Count > 0) {
                        string msg = ModTranslation.getString("portalmakerLog");
                        foreach (var entry in Portal.teleportedPlayers) {
                            float timeBeforeMeeting = ((float)(DateTime.UtcNow - entry.time).TotalMilliseconds) / 1000;
                            msg += Portalmaker.logShowsTime ? string.Format(ModTranslation.getString("portalmakerLogTime"), (int)timeBeforeMeeting) : "";
                            msg = msg + string.Format(ModTranslation.getString("portalmakerLogName"), entry.name);
                        }
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Portalmaker.portalmaker, $"{msg}", false);
                    }
                }

                NekoKabocha.meetingKiller = null;

                if (Shifter.shifter != null && Shifter.isNeutral && Shifter.shifter.Data.IsDead && Shifter.futureShift != null && Shifter.futureShift.Data.Role.IsImpostor)
                {
                    var dp = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == Shifter.shifter.PlayerId);
                    Shifter.killer = dp.killerIfExisting;
                    Shifter.deathReason = dp.deathReason;
                }

                // Add trapped Info into Trapper chat
                /*if (Trapper.trapper != null && (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper || Helpers.shouldShowGhostInfo()) && !Trapper.trapper.Data.IsDead) {
                    if (Trap.traps.Any(x => x.revealed))
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, "Trap Logs:");
                    foreach (Trap trap in Trap.traps) {
                        if (!trap.revealed) continue;
                        string message = $"Trap {trap.instanceId}: \n";
                        trap.trappedPlayer = trap.trappedPlayer.OrderBy(x => rnd.Next()).ToList();
                        foreach (PlayerControl p in trap.trappedPlayer) {
                            if (Trapper.infoType == 0) message += RoleInfo.GetRolesString(p, false, false, true) + "\n";
                            else if (Trapper.infoType == 1) {
                                if (Helpers.isNeutral(p) || p.Data.Role.IsImpostor) message += "Evil Role \n";
                                else message += "Good Role \n";
                            }
                            else message += p.Data.PlayerName + "\n";
                        }
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, $"{message}");
                    }
                }*/

                // Add Snitch info
                string output = "";

                if (Snitch.snitch != null && Snitch.mode != Snitch.Mode.Map && (CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch || Helpers.shouldShowGhostInfo()) && !Snitch.snitch.Data.IsDead) {
                    var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                    int numberOfTasks = playerTotal - playerCompleted;
                    if (numberOfTasks == 0) {
                        output = ModTranslation.getString("snitchOutput");
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.4f, new Action<float>((x) => {
                            if (x == 1f) {
                                foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                                    if (Snitch.targets == Snitch.Targets.Killers && !Helpers.isKiller(p)) continue;
                                    else if (Snitch.targets == Snitch.Targets.EvilPlayers && !Helpers.isEvil(p)) continue;
                                    if (!Snitch.playerRoomMap.ContainsKey(p.PlayerId)) continue;
                                    if (p.Data.IsDead) continue;
                                    var room = Snitch.playerRoomMap[p.PlayerId];
                                    var roomName = DestroyableSingleton<TranslationController>.Instance.GetString(SystemTypes.Outside);
                                    if (room != byte.MinValue) {
                                        roomName = DestroyableSingleton<TranslationController>.Instance.GetString((SystemTypes)room);
                                    }
                                    output += "- " + string.Format(ModTranslation.getString("snitchLastSeen"), RoleInfo.GetRolesString(p, false, false, true), roomName) + "\n";
                                }
                                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Snitch.snitch, $"{output}", false);
                            }
                        })));
                    }
                }

                if (CachedPlayer.LocalPlayer.Data.IsDead && output != "") FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer, $"{output}", false);

                //Trapper.playersOnMap = new List<PlayerControl>();
                Snitch.playerRoomMap = new Dictionary<byte, byte>();

                // Remove revealed traps
                //Trap.clearRevealedTraps();

                // Clear props here else something will get wrong
                if (CustomOptionHolder.activateProps.getBool())
                {
                    Props.clearProps();
                }

                BombEffect.clearBombEffects();

                // Reset zoomed out ghosts
                Helpers.toggleZoom(reset: true);

                // Stop all playing sounds
                SoundEffectsManager.stopAll();

                // Close In-Game Settings Display if open
                HudManagerUpdate.CloseSettings();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch {
            public static Sprite Overlay => Blackmailer.getBlackmailOverlaySprite();
            static void Postfix(MeetingHud __instance) {
                // Deactivate skip Button if skipping on emergency meetings is disabled
                if (target == null && blockSkippingInEmergencyMeetings)
                    __instance.SkipVoteButton.gameObject.SetActive(false);

                if (__instance.state >= MeetingHud.VoteStates.Discussion)
                {
                    // Remove first kill shield
                    TORMapOptions.firstKillPlayer = null;
                }

                updateMeetingText(__instance);

                if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null)
                {
                    // Blackmailer show overlay
                    var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Blackmailer.blackmailed.PlayerId);
                    playerState.Overlay.gameObject.SetActive(true);
                    playerState.Overlay.sprite = Overlay;
                    if (__instance.state != MeetingHud.VoteStates.Animating && !Blackmailer.alreadyShook)
                    {
                        Blackmailer.alreadyShook = true;
                        __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(QuickChatMenu), nameof(QuickChatMenu.Open))]
        public class BlockQuickChatAbility
        {
            public static bool Prefix(QuickChatMenu __instance)
            {
                if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
        public class BlockChatBlackmailed
        {
            public static bool Prefix(TextBoxTMP __instance)
            {
                if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public static void MeetingHudIntroPrefix() {
            EventUtility.meetingStartsUpdate();
        }

    }
}
