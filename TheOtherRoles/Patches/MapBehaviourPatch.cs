using HarmonyLib;
using Hazel;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;


namespace TheOtherRoles.Patches {

	[HarmonyPatch(typeof(MapBehaviour))]
	class MapBehaviourPatch {
		public static Dictionary<PlayerControl, SpriteRenderer> herePoints = new();

        public static SpriteRenderer targetHerePoint;
        public static Dictionary<byte, SpriteRenderer> impostorHerePoint;

        public static Dictionary<byte, Il2CppSystem.Collections.Generic.List<Vector2>> realTasks = new();
        public static void resetRealTasks()
        {
            realTasks.Clear();
        }

        public static void shareRealTasks()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareRealTasks, Hazel.SendOption.Reliable, -1);
            int count = 0;
            foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks)
            {
                if (!task.IsComplete && task.HasLocation && !PlayerTask.TaskIsEmergency(task))
                {
                    foreach (var loc in task.Locations)
                    {
                        count++;
                    }
                }
            }
            writer.Write((byte)count);
            foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks)
            {
                if (!task.IsComplete && task.HasLocation && !PlayerTask.TaskIsEmergency(task))
                {
                    foreach (var loc in task.Locations)
                    {
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        writer.Write(loc.x);
                        writer.Write(loc.y);
                    }
                }
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void reset()
        {
            herePoints = new();
            impostorHerePoint = new();
        }

        private static bool evilTrackerShowTask(MapTaskOverlay __instance)
        {
            if (FreePlayGM.isFreePlayGM) return true;
            if (!MeetingHud.Instance) return true;  // Only run in meetings, and then set the Position of the HerePoint to the Position before the Meeting!
            if (CachedPlayer.LocalPlayer.PlayerControl != EvilTracker.evilTracker || !EvilTracker.canSeeTargetTasks) return true;
            if (EvilTracker.target == null) return true;
            if (realTasks[EvilTracker.target.PlayerId] == null) return false;
            EvilTracker.acTokenCommon2 ??= new("evilTracker.common2");
            __instance.gameObject.SetActive(true);
            __instance.data.Clear();
            for (int i = 0; i < realTasks[EvilTracker.target.PlayerId].Count; i++)
            {
                try
                {
                    Vector2 pos = realTasks[EvilTracker.target.PlayerId][i];

                    Vector3 localPosition = pos / MapUtilities.CachedShipStatus.MapScale;
                    localPosition.z = -1f;
                    PooledMapIcon pooledMapIcon = __instance.icons.Get<PooledMapIcon>();
                    pooledMapIcon.transform.localScale = new Vector3(pooledMapIcon.NormalSize, pooledMapIcon.NormalSize, pooledMapIcon.NormalSize);
                    pooledMapIcon.rend.color = Palette.CrewmateBlue;
                    pooledMapIcon.name = $"{i}";
                    pooledMapIcon.lastMapTaskStep = 0;
                    pooledMapIcon.transform.localPosition = localPosition;
                    string text = $"{i}";
                    __instance.data.Add(text, pooledMapIcon);
                }
                catch (Exception ex)
                {
                    TheOtherRolesPlugin.Logger.LogError(ex.Message);
                }
            }
            return false;
        }

        [HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.Show))]
        class MapTaskOverlayShow
        {
            static bool Prefix(MapTaskOverlay __instance)
            {
                if (EvilTracker.evilTracker != null && CachedPlayer.LocalPlayer.PlayerId == EvilTracker.evilTracker.PlayerId)
                {
                    return evilTrackerShowTask(__instance);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
		static void Postfix(MapBehaviour __instance) {
			/*if (Trapper.trapper != null && CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) {
				foreach (PlayerControl player in Trapper.playersOnMap) {
					if (herePoints.ContainsKey(player)) continue;
					Vector3 v = Trap.trapPlayerIdMap[player.PlayerId].trap.transform.position;
					v /= MapUtilities.CachedShipStatus.MapScale;
					v.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
					v.z = -1f;
					var herePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
					herePoint.transform.localPosition = v;
					herePoint.enabled = true;
					int colorId = player.CurrentOutfit.ColorId;
					if (Trapper.anonymousMap) player.CurrentOutfit.ColorId = 6;
					player.SetPlayerMaterialColors(herePoint);
					player.CurrentOutfit.ColorId = colorId;
					herePoints.Add(player, herePoint);
				}
				foreach (var s in herePoints.Where(x => !Trapper.playersOnMap.Contains(x.Key)).ToList()) {
					UnityEngine.Object.Destroy(s.Value);
					herePoints.Remove(s.Key);
				}
			}*/
			if (EvilTracker.evilTracker != null && CachedPlayer.LocalPlayer.PlayerId == EvilTracker.evilTracker.PlayerId && EvilTracker.canSeeTargetPosition)
			{
                if (EvilTracker.target != null && MeetingHud.Instance == null)
                {
                    if (targetHerePoint == null)
                    {
                        targetHerePoint = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                    }
                    targetHerePoint.gameObject.SetActive(!EvilTracker.target.Data.IsDead);
                    NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(EvilTracker.target.PlayerId);
                    PlayerMaterial.SetColors((playerById != null) ? playerById.DefaultOutfit.ColorId : 0, targetHerePoint);
                    Vector3 pos = new(EvilTracker.target.transform.position.x, EvilTracker.target.transform.position.y, EvilTracker.target.transform.position.z);
                    pos /= MapUtilities.CachedShipStatus.MapScale;
                    pos.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                    pos.z = -10;
                    targetHerePoint.transform.localPosition = pos;
                }
                else UnityEngine.Object.Destroy(targetHerePoint);

                // Use the red icons to indicate the Impostors' positions
                if (impostorHerePoint == null) impostorHerePoint = new();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if ((p.Data.Role.IsImpostor && p != CachedPlayer.LocalPlayer.PlayerControl) || (Spy.spy != null && p == Spy.spy) || (p == Sidekick.sidekick && Sidekick.wasTeamRed)
                        || (p == Jackal.jackal && Jackal.wasTeamRed))
                    {
                        if (!impostorHerePoint.ContainsKey(p.PlayerId))
                        {
                            impostorHerePoint[p.PlayerId] = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        }
                        impostorHerePoint[p.PlayerId].gameObject.SetActive(!p.Data.IsDead && MeetingHud.Instance == null);
                        NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(p.PlayerId);
                        PlayerMaterial.SetColors(0, impostorHerePoint[p.PlayerId]);
                        Vector3 pos = new(p.transform.position.x, p.transform.position.y, p.transform.position.z);
                        pos /= MapUtilities.CachedShipStatus.MapScale;
                        pos.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                        pos.z = -10;
                        impostorHerePoint[p.PlayerId].transform.localPosition = pos;
                    }
                }
            }

            if (Snitch.snitch != null && CachedPlayer.LocalPlayer.PlayerId == Snitch.snitch.PlayerId && !Snitch.snitch.Data.IsDead && Snitch.mode != Snitch.Mode.Chat) {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                int numberOfTasks = playerTotal - playerCompleted;

                if (numberOfTasks == 0) {
					if (MeetingHud.Instance == null) {
                        foreach (PlayerControl player in CachedPlayer.AllPlayers) {
                            if (Snitch.targets == Snitch.Targets.EvilPlayers && !Helpers.isEvil(player)) continue;
                            else if (Snitch.targets == Snitch.Targets.Killers && !Helpers.isKiller(player)) continue;
							if (player.Data.IsDead) continue;
                            Vector3 v = player.transform.position;
                            v /= MapUtilities.CachedShipStatus.MapScale;
                            v.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                            v.z = -1f;
							if (herePoints.ContainsKey(player)) {
								herePoints[player].transform.localPosition = v;
								continue;
							}
                            var herePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
                            herePoint.transform.localPosition = v;
                            herePoint.enabled = true;
                            int colorId = player.CurrentOutfit.ColorId;
                            player.CurrentOutfit.ColorId = 6;
                            player.SetPlayerMaterialColors(herePoint);
                            player.CurrentOutfit.ColorId = colorId;
                            herePoints.Add(player, herePoint);
                        }
                    } else {
                        foreach (var s in herePoints) {
                            UnityEngine.Object.Destroy(s.Value);
                            herePoints.Remove(s.Key);
                        }
                    }
                }
			}
            HudManagerUpdate.CloseSettings();
            CustomOverlay.hideInfoOverlay();
        }
    }
}
