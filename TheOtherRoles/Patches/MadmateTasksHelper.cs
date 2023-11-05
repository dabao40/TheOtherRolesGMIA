using HarmonyLib;
using System;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Patches
{
    class MadmateTasksHelper
    {
        public static int madmateTasks { get; private set; }

        public static void Reset()
        {
            madmateTasks = 0;
        }

        public static void SetMadmateTasks()
        {
            PlayerControl me = CachedPlayer.LocalPlayer.PlayerControl;
            if (me == null)
                return;
            GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(me.PlayerId);
            if (playerById == null)
                return;
            me.clearAllTasks();
            List<byte> list = new List<byte>(10);
            SetTasksToList2(
                ref list,
                MapUtilities.CachedShipStatus.CommonTasks.ToList<NormalPlayerTask>(),
                Mathf.RoundToInt(CustomOptionHolder.madmateCommonTasks.getFloat()));
            SetTasksToList2(
                ref list,
                MapUtilities.CachedShipStatus.LongTasks.ToList<NormalPlayerTask>(),
                Mathf.RoundToInt(CustomOptionHolder.madmateLongTasks.getFloat()));
            SetTasksToList2(
                ref list,
                MapUtilities.CachedShipStatus.ShortTasks.ToList<NormalPlayerTask>(),
                Mathf.RoundToInt(CustomOptionHolder.madmateShortTasks.getFloat()));

            byte[] taskTypeIds = list.ToArray();
            playerById.Tasks = new Il2CppSystem.Collections.Generic.List<GameData.TaskInfo>(taskTypeIds.Length);
            for (int i = 0; i < taskTypeIds.Length; i++)
            {
                playerById.Tasks.Add(new GameData.TaskInfo(taskTypeIds[i], (uint)i));
                playerById.Tasks[i].Id = (uint)i;
            }
            for (int i = 0; i < playerById.Tasks.Count; i++)
            {
                GameData.TaskInfo taskInfo = playerById.Tasks[i];
                NormalPlayerTask normalPlayerTask = UnityEngine.Object.Instantiate<NormalPlayerTask>(MapUtilities.CachedShipStatus.GetTaskById(taskInfo.TypeId), me.transform);
                normalPlayerTask.Id = taskInfo.Id;
                normalPlayerTask.Owner = me;
                normalPlayerTask.Initialize();
                me.myTasks.Add(normalPlayerTask);
            }
            madmateTasks = playerById.Tasks.Count;
        }

        private static void SetTasksToList(
            ref List<byte> list,
            List<NormalPlayerTask> playerTasks,
            int numConfiguredTasks)
        {
            playerTasks.Shuffle(0);
            int numTasks = Math.Min(playerTasks.Count, numConfiguredTasks);
            for (int i = 0; i < numTasks; i++)
            {
                list.Add((byte)playerTasks[i].Index);
            }
        }


        private static void SetTasksToList2(
            ref List<byte> list,
            List<NormalPlayerTask> playerTasks,
            int numConfiguredTasks)
        {
            if (numConfiguredTasks == 0)
                return;
            List<TaskTypes> taskTypesList = new List<TaskTypes>();
            playerTasks.Shuffle();
            int count = 0;
            int numTasks = Math.Min(playerTasks.Count, numConfiguredTasks);
            for (int i = 0; i < playerTasks.Count; i++)
            {
                if (taskTypesList.Contains(playerTasks[i].TaskType))
                    continue;
                taskTypesList.Add(playerTasks[i].TaskType);
                ++count;
                list.Add((byte)playerTasks[i].Index);
                if (count >= numTasks)
                    break;
            }
        }
    }
}
