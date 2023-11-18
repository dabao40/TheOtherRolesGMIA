using HarmonyLib;
using Hazel;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;


namespace TheOtherRoles.Patches {

	[HarmonyPatch(typeof(MapBehaviour))]
	class MapBehaviourPatch {
		public static Dictionary<PlayerControl, SpriteRenderer> herePoints = new Dictionary<PlayerControl, SpriteRenderer>();

        public static SpriteRenderer targetHerePoint;
        public static Dictionary<byte, SpriteRenderer> impostorHerePoint;
        public static Sprite doorClosedSprite;
        public static Dictionary<string, SpriteRenderer> doorMarks;
        public static Il2CppArrayBase<PlainDoor> plainDoors = null;

        public static void reset()
        {
            if (doorMarks != null)
            {
                foreach (var mark in doorMarks.Values)
                {
                    UnityEngine.Object.Destroy(mark.gameObject);
                }
                doorMarks.Clear();
                doorMarks = null;
            }
            if (plainDoors != null)
            {
                plainDoors = null;
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
			if (CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker && EvilTracker.canSeeTargetPosition)
			{
                if (EvilTracker.target != null && MeetingHud.Instance == null)
                {
                    if (targetHerePoint == null)
                    {
                        targetHerePoint = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                    }
                    targetHerePoint.gameObject.SetActive(!EvilTracker.target.Data.IsDead);
                    GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(EvilTracker.target.PlayerId);
                    PlayerMaterial.SetColors((playerById != null) ? playerById.DefaultOutfit.ColorId : 0, targetHerePoint);
                    Vector3 pos = new Vector3(EvilTracker.target.transform.position.x, EvilTracker.target.transform.position.y, EvilTracker.target.transform.position.z);
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
                    if ((p.Data.Role.IsImpostor && p != CachedPlayer.LocalPlayer.PlayerControl) || (Spy.spy != null && p == Spy.spy))
                    {
                        if (!impostorHerePoint.ContainsKey(p.PlayerId))
                        {
                            impostorHerePoint[p.PlayerId] = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        }
                        impostorHerePoint[p.PlayerId].gameObject.SetActive(!p.Data.IsDead && MeetingHud.Instance == null);
                        GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(p.PlayerId);
                        PlayerMaterial.SetColors(0, impostorHerePoint[p.PlayerId]);
                        Vector3 pos = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z);
                        pos /= MapUtilities.CachedShipStatus.MapScale;
                        pos.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                        pos.z = -10;
                        impostorHerePoint[p.PlayerId].transform.localPosition = pos;
                    }
                }
            }

            if (EvilHacker.canSeeDoorStatus && (CachedPlayer.LocalPlayer.PlayerControl == EvilHacker.evilHacker || EvilHacker.isInherited()))
            {
                //if (!EvilHacker.canSeeDoorStatus) return;
                if (doorClosedSprite == null)
                {
                    doorClosedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Cross.png", 500f);
                }
                if (doorMarks == null) doorMarks = new();
                plainDoors = GameObject.FindObjectsOfType<PlainDoor>();
                foreach (var door in plainDoors)
                {
                    Vector3 pos = door.gameObject.transform.position / MapUtilities.CachedShipStatus.MapScale;
                    pos.z = -10f;
                    String key = $"{pos.x},{pos.y}";
                    SpriteRenderer mark;
                    if (doorMarks.ContainsKey(key))
                    {
                        mark = doorMarks[key];
                        if (mark == null) mark = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                    }
                    else
                    {
                        mark = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        doorMarks.Add(key, mark);
                    }
                    if (mark != null)
                    {
                        if (!door.Open)
                        {
                            mark.gameObject.SetActive(true);
                            mark.sprite = doorClosedSprite;
                            PlayerMaterial.SetColors(0, mark);
                            mark.transform.localPosition = pos;
                            mark.gameObject.SetActive(true);
                        }
                        else
                        {
                            mark.gameObject.SetActive(false);
                        }
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
        }
    }
}
