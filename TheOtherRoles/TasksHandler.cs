using HarmonyLib;
using Hazel;
using System;
using System.Linq;
using TheOtherRoles.Patches;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;

namespace TheOtherRoles {
    [HarmonyPatch]
    public static class TasksHandler {

        public static Tuple<int, int> taskInfo(GameData.PlayerInfo playerInfo, bool isResult = false) {
            int TotalTasks = 0;
            int CompletedTasks = 0;
            if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
                playerInfo.Object &&
                playerInfo.Role && playerInfo.Role.TasksCountTowardProgress &&
                !playerInfo.Object.hasFakeTasks() && !playerInfo.Role.IsImpostor
                ) {
                bool isOldTaskMasterEx = TaskMaster.taskMaster && TaskMaster.oldTaskMasterPlayerId == playerInfo.PlayerId;
                bool isTaskMasterEx = TaskMaster.taskMaster && TaskMaster.taskMaster == playerInfo.Object && TaskMaster.isTaskComplete;
                if (isOldTaskMasterEx || (!isResult && isTaskMasterEx))
                {
                    TotalTasks = CompletedTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks + 
                    GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks + 
                    GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
                }
                else
                {
                    foreach (var playerInfoTask in playerInfo.Tasks.GetFastEnumerator())
                    {
                        if (playerInfoTask.Complete) CompletedTasks++;
                        TotalTasks++;
                    }
                }
            }
            return Tuple.Create(CompletedTasks, TotalTasks);
        }

        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private static class GameDataRecomputeTaskCountsPatch {
            private static bool Prefix(GameData __instance) {
               

                var totalTasks = 0;
                var completedTasks = 0;
                
                foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
                {
                    if (playerInfo.Object
                        && playerInfo.Object.hasAliveKillingLover() // Tasks do not count if a Crewmate has an alive killing Lover
                        || playerInfo.PlayerId == Lawyer.lawyer?.PlayerId // Tasks of the Lawyer do not count
                        || (playerInfo.PlayerId == Pursuer.pursuer?.PlayerId && Pursuer.pursuer.Data.IsDead) // Tasks of the Pursuer only count, if he's alive
                        || (playerInfo.PlayerId == Shifter.shifter?.PlayerId && Shifter.isNeutral) // Chain-Shifter has tasks, but they don't count
                        || playerInfo.PlayerId == Thief.thief?.PlayerId // Thief's tasks only count after joining crew team as sheriff (and then the thief is not the thief anymore)
                        || (Madmate.hasTasks && Madmate.madmate.Any(x => x.PlayerId == playerInfo.PlayerId))
                        || (CreatedMadmate.hasTasks && playerInfo.PlayerId == CreatedMadmate.createdMadmate?.PlayerId)
                        || playerInfo.PlayerId == JekyllAndHyde.jekyllAndHyde?.PlayerId
                        || playerInfo.PlayerId == Fox.fox?.PlayerId
                        )
                        continue;
                    var (playerCompleted, playerTotal) = taskInfo(playerInfo);
                    totalTasks += playerTotal;
                    completedTasks += playerCompleted;
                }
                
                __instance.TotalTasks = totalTasks;
                __instance.CompletedTasks = completedTasks;
                return false;
            }
        }

        [HarmonyPatch(typeof(GameData), nameof(GameData.CompleteTask))]
        private static class GameDataCompleteTaskPatch
        {
            private static void Postfix(GameData __instance, [HarmonyArgument(0)] PlayerControl pc, [HarmonyArgument(1)] uint taskId)
            {
                if (AmongUsClient.Instance.AmHost && !pc.Data.IsDead && TaskMaster.isTaskMaster(pc.PlayerId))
                {
                    byte clearTasks = 0;
                    for (int i = 0; i < pc.Data.Tasks.Count; ++i)
                    {
                        if (pc.Data.Tasks[i].Complete)
                            ++clearTasks;
                    }
                    bool allTasksCompleted = clearTasks == pc.Data.Tasks.Count;
                    Action action = () => {
                        if (TaskMaster.isTaskComplete)
                        {
                            byte clearTasks = 0;
                            for (int i = 0; i < pc.Data.Tasks.Count; ++i)
                            {
                                if (pc.Data.Tasks[i].Complete)
                                    ++clearTasks;
                            }
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TaskMasterUpdateExTasks, Hazel.SendOption.Reliable, -1);
                            writer.Write(clearTasks);
                            writer.Write((byte)pc.Data.Tasks.Count);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.taskMasterUpdateExTasks(clearTasks, (byte)pc.Data.Tasks.Count);
                        }
                    };

                    if (allTasksCompleted)
                    {
                        if (!TaskMaster.isTaskComplete)
                        {
                            byte[] taskTypeIds = TaskMasterTaskHelper.GetTaskMasterTasks(pc);
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TaskMasterSetExTasks, Hazel.SendOption.Reliable, -1);
                            writer.Write(pc.PlayerId);
                            writer.Write(byte.MaxValue);
                            writer.Write(taskTypeIds);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.taskMasterSetExTasks(pc.PlayerId, byte.MaxValue, taskTypeIds);
                            action();
                        }
                        else if (!TaskMaster.triggerTaskMasterWin)
                        {
                            action();
                            TaskMaster.triggerTaskMasterWin = true;
                        }
                    }
                    else
                    {
                        action();
                    }
                }
            }
        }

    }
}
