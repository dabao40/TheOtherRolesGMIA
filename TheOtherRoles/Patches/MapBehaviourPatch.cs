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

        public static Sprite Vent = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Vent.png", 150f);

        public static List<List<Vent>> VentNetworks = new();

        public static Dictionary<string, GameObject> mapIcons = new();

        public static Sprite doorClosedSprite;
        public static Dictionary<string, SpriteRenderer> doorMarks;
        public static Il2CppArrayBase<OpenableDoor> plainDoors = null;

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
            foreach (var mapIcon in mapIcons.Values)
            {
                mapIcon.Destroy();
            }
            mapIcons = new();
            VentNetworks = new();
            if (doorMarks != null)
            {
                foreach (var mark in doorMarks.Values)
                {
                    if (mark != null) UnityEngine.Object.Destroy(mark?.gameObject);
                }
                doorMarks.Clear();
                doorMarks = null;
            }
            if (plainDoors != null)
            {
                plainDoors = null;
            }
        }

        private static bool evilTrackerShowTask(MapTaskOverlay __instance)
        {
            if (FreePlayGM.isFreePlayGM) return true;
            if (!MeetingHud.Instance) return true;  // Only run in meetings, and then set the Position of the HerePoint to the Position before the Meeting!
            if (CachedPlayer.LocalPlayer.PlayerControl != EvilTracker.evilTracker || !EvilTracker.canSeeTargetTasks) return true;
            if (EvilTracker.target == null) return true;
            if (realTasks[EvilTracker.target.PlayerId] == null) return false;
            _ = new StaticAchievementToken("evilTracker.common2");
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
            __instance.HerePoint.transform.SetLocalZ(-2.1f);
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
                    targetHerePoint.gameObject.SetActive(!EvilTracker.target.Data.IsDead && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead);
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
                impostorHerePoint ??= new();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if ((p.Data.Role.IsImpostor && p != CachedPlayer.LocalPlayer.PlayerControl) || (Spy.spy != null && p == Spy.spy) || (p == Sidekick.sidekick && Sidekick.wasTeamRed)
                        || (p == Jackal.jackal && Jackal.wasTeamRed))
                    {
                        if (!impostorHerePoint.ContainsKey(p.PlayerId))
                        {
                            impostorHerePoint[p.PlayerId] = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        }
                        impostorHerePoint[p.PlayerId].gameObject.SetActive(!p.Data.IsDead && MeetingHud.Instance == null && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead);
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

            if (EvilHacker.canSeeDoorStatus && ((EvilHacker.evilHacker != null && CachedPlayer.LocalPlayer.PlayerId == EvilHacker.evilHacker.PlayerId) || EvilHacker.isInherited()))
            {
                if (doorClosedSprite == null)
                {
                    doorClosedSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Cross.png", 500f);
                }
                doorMarks ??= new();
                plainDoors = GameObject.FindObjectsOfType<OpenableDoor>();
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
                        if (!door.IsOpen)
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

            foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
            {
                if (vent.name.StartsWith("JackInThe") && !(PlayerControl.LocalPlayer == Trickster.trickster || PlayerControl.LocalPlayer.Data.IsDead)) continue; //for trickster vents

                if (!TheOtherRolesPlugin.ShowVentsOnMap.Value)
                {
                    if (mapIcons.Count > 0)
                    {
                        mapIcons.Values.Do((x) => x.Destroy());
                        mapIcons.Clear();
                    }
                    break;
                }

                var Instance = DestroyableSingleton<MapTaskOverlay>.Instance;
                var task = PlayerControl.LocalPlayer.myTasks.ToArray().FirstOrDefault(x => x.TaskType == TaskTypes.VentCleaning);

                var location = vent.transform.position / MapUtilities.CachedShipStatus.MapScale;
                location.z = -2f; //show above sabotage buttons

                GameObject MapIcon;
                if (!mapIcons.ContainsKey($"vent {vent.Id} icon"))
                {
                    MapIcon = GameObject.Instantiate(__instance.HerePoint.gameObject, __instance.HerePoint.transform.parent);
                    mapIcons.Add($"vent {vent.Id} icon", MapIcon);
                }
                else
                {
                    MapIcon = mapIcons[$"vent {vent.Id} icon"];
                }

                MapIcon.GetComponent<SpriteRenderer>().sprite = Vent;

                MapIcon.name = $"vent {vent.Id} icon";
                MapIcon.transform.localPosition = location;

                if (task?.IsComplete == false && task.FindConsoles()[0].ConsoleId == vent.Id)
                {
                    MapIcon.transform.localScale *= 0.6f;
                }
                if (vent.name.StartsWith("JackInThe"))
                {
                    MapIcon.GetComponent<SpriteRenderer>().sprite = JackInTheBox.getBoxAnimationSprite(0);
                    MapIcon.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    MapIcon.GetComponent<SpriteRenderer>().color = vent.isActiveAndEnabled ? Color.yellow : Color.yellow.SetAlpha(0.5f);
                }

                if (AllVentsRegistered(Instance))
                {
                    var array = VentNetworks.ToArray();
                    foreach (var connectedgroup in VentNetworks)
                    {
                        var index = Array.IndexOf(array, connectedgroup);
                        if (connectedgroup[0].name.StartsWith("JackInThe"))
                            continue;
                        connectedgroup.Do(x => GetIcon(x).GetComponent<SpriteRenderer>().color = Palette.PlayerColors[index]);
                    }
                    continue;
                }

                HandleMiraOrSub();

                var network = GetNetworkFor(vent);
                if (network == null)
                {
                    VentNetworks.Add(new(vent.NearbyVents.Where(x => x != null)) { vent });
                }
                else
                {
                    if (!network.Any(x => x == vent)) network.Add(vent);
                }
            }

            HudManagerUpdate.CloseSettings();
            CustomOverlay.hideInfoOverlay();
        }

        public static List<Vent> GetNetworkFor(Vent vent)
        {
            return VentNetworks.FirstOrDefault(x => x.Any(y => y == vent || y == vent.Left || y == vent.Center || y == vent.Right));
        }

        public static bool AllVentsRegistered(MapTaskOverlay __instance)
        {
            foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
            {
                if (!vent.isActiveAndEnabled) continue;
                var network = GetNetworkFor(vent);
                if (network == null || !network.Any(x => x == vent)) return false;
                if (!mapIcons.ContainsKey($"vent {vent.Id} icon")) return false;
            }
            return true;
        }

        public static GameObject GetIcon(Vent vent)
        {
            var icon = mapIcons[$"vent {vent.Id} icon"];
            return icon;
        }

        public static void HandleMiraOrSub()
        {
            if (VentNetworks.Count != 0) return;

            if (Helpers.isMira())
            {
                var vents = MapUtilities.CachedShipStatus.AllVents.Where(x => !x.name.Contains("JackInTheBoxVent_"));
                VentNetworks.Add(vents.ToList());
                return;
            }
            if (MapUtilities.CachedShipStatus.Type == SubmergedCompatibility.SUBMERGED_MAP_TYPE)
            {
                var vents = MapUtilities.CachedShipStatus.AllVents.Where(x => x.Id is 12 or 13 or 15 or 16);
                VentNetworks.Add(vents.ToList());
            }
        }
    }
}
