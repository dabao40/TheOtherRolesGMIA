using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Hazel;
using TheOtherRoles.MetaContext;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    class OptimizeMapPatch
    {
        static private SpriteLoader customMeetingSideSprite = SpriteLoader.FromResource("TheOtherRoles.Resources.ladder_bg.png", 100f);
        static private SpriteLoader customMeetingLadderSprite = SpriteLoader.FromResource("TheOtherRoles.Resources.ladder.png", 100f);

        public static void Postfix(ShipStatus __instance)
        {
            addLadder(GameOptionsManager.Instance.currentNormalGameOptions.MapId);
            optimizeMap(GameOptionsManager.Instance.currentNormalGameOptions.MapId);
        }

        public static void optimizeMap(int mapId)
        {
            if (!CustomOptionHolder.airshipOptimize.getBool()) return;
            if (mapId == 4)
            {
                var obj = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].gameObject;
                //昇降機右に影を追加
                OneWayShadows oneWayShadow = obj.transform.FindChild("Shadow").FindChild("LedgeShadow").GetComponent<OneWayShadows>();
                oneWayShadow.enabled = false;
                if (CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor) oneWayShadow.gameObject.SetActive(false);

                SpriteRenderer renderer;

                GameObject fance = new("ModFance")
                {
                    layer = LayerMask.NameToLayer("Ship")
                };
                fance.transform.SetParent(obj.transform);
                fance.transform.localPosition = new Vector3(4.2f, 0.15f, 0.5f);
                fance.transform.localScale = new Vector3(1f, 1f, 1f);
                fance.SetActive(true);
                var Collider = fance.AddComponent<EdgeCollider2D>();
                Collider.points = new Vector2[] { new Vector2(1.5f, -0.2f), new Vector2(-1.5f, -0.2f), new Vector2(-1.5f, 1.5f) };
                Collider.enabled = true;
                renderer = fance.AddComponent<SpriteRenderer>();
                renderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AirshipFence.png", 100f);

                var panel = obj.transform.FindChild("panel_data");
                panel.localPosition = new Vector3(4.52f, -3.95f, 0.1f);
            }

        }

        public static void addLadder(int mapId)
        {
            if (mapId == 4 && CustomOptionHolder.airshipLadder.getBool())
            {
                Transform meetingRoom = ShipStatus.Instance.FastRooms[SystemTypes.MeetingRoom].transform;
                Transform gapRoom = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].transform;

                float diffX = (meetingRoom.position.x - gapRoom.transform.position.x) / 0.7f;
                float[] shadowX = new float[2] { 0f, 0f };

                //画像を更新する
                GameObject customRendererObj = new("meeting_custom");
                customRendererObj.transform.SetParent(meetingRoom);
                customRendererObj.transform.localPosition = new Vector3(9.58f, -2.86f, 4.8f);
                customRendererObj.transform.localScale = new Vector3(1f, 1f, 1f);
                customRendererObj.AddComponent<SpriteRenderer>().sprite = customMeetingSideSprite.GetSprite(); ;
                customRendererObj.layer = LayerMask.NameToLayer("Ship");

                //はしごを生成
                GameObject originalLadderObj = meetingRoom.FindChild("ladder_meeting").gameObject;
                GameObject ladderObj = GameObject.Instantiate(meetingRoom.FindChild("ladder_meeting").gameObject, meetingRoom);
                ladderObj.name = "ladder_meeting_custom";
                ladderObj.transform.position += new Vector3(10.9f, 0);
                ladderObj.GetComponent<SpriteRenderer>().sprite = customMeetingLadderSprite.GetSprite();
                ladderObj.GetComponentsInChildren<Ladder>().Do(l => { l.Id = (byte)(l.Id + 8); ShipStatus.Instance.Ladders = new Il2CppReferenceArray<Ladder>(ShipStatus.Instance.Ladders.Append(l).ToArray()); });

                //MeetingRoomの当たり判定に手を加える
                var collider = meetingRoom.FindChild("Walls").GetComponents<EdgeCollider2D>().Where((c) => c.pointCount == 43).FirstOrDefault();
                Il2CppSystem.Collections.Generic.List<Vector2> colliderPosList = new();
                int index = 0;
                float tempX = 0f;
                float tempY = 0f;

                foreach (var p in collider!.points)
                {
                    if (index != 30) colliderPosList.Add(p);
                    if (index == 29) tempX = p.x;
                    if (index == 30)
                    {
                        tempX = (tempX + p.x) / 2f;
                        colliderPosList.Add(new Vector2(tempX, p.y));
                        colliderPosList.Add(new Vector2(tempX, -1.8067f));
                        colliderPosList.Add(new Vector2(p.x, -1.8067f));
                    }
                    index++;
                }
                collider.SetPoints(colliderPosList);

                //MeetingRoomの影に手を加える
                collider = meetingRoom.FindChild("Shadows").GetComponents<EdgeCollider2D>().Where((c) => c.pointCount == 46).FirstOrDefault();

                colliderPosList = new Il2CppSystem.Collections.Generic.List<Vector2>();
                index = 0;
                while (index <= 40)
                {
                    colliderPosList.Add(collider!.points[index]);
                    index++;
                }

                shadowX[0] = collider!.points[41].x;
                shadowX[1] = tempX = (collider.points[40].x + collider.points[41].x) / 2f;
                tempY = (collider.points[40].y + collider.points[41].y) / 2f;
                colliderPosList.Add(new Vector2(tempX, tempY));
                colliderPosList.Add(new Vector2(tempX, tempY - 2.56f));
                var newCollider = meetingRoom.FindChild("Shadows").gameObject.AddComponent<EdgeCollider2D>();
                newCollider.SetPoints(colliderPosList);

                colliderPosList = new Il2CppSystem.Collections.Generic.List<Vector2>();
                index = 41;
                while (index <= 45)
                {
                    if (index == 41) colliderPosList.Add(collider.points[41] - new Vector2(0, 2.56f));
                    colliderPosList.Add(collider.points[index]);
                    index++;
                }
                tempX = collider.points[41].x;
                collider.SetPoints(colliderPosList);

                //GapRoomの影に手を加える
                collider = gapRoom.FindChild("Shadow").GetComponents<EdgeCollider2D>().Where(x => Math.Abs(x.points[0].x + 6.2984f) < 0.1).FirstOrDefault();
                colliderPosList = new Il2CppSystem.Collections.Generic.List<Vector2>();
                index = 0;
                while (index <= 1)
                {
                    colliderPosList.Add(collider!.points[index]);
                    index++;
                }
                colliderPosList.Add(new Vector2(shadowX[0] + diffX, collider!.points[1].y));
                newCollider = gapRoom.FindChild("Shadow").gameObject.AddComponent<EdgeCollider2D>();
                newCollider.SetPoints(colliderPosList);
                colliderPosList = new Il2CppSystem.Collections.Generic.List<Vector2>();
                index = 2;
                colliderPosList.Add(new Vector2(shadowX[1] + diffX, collider.points[1].y));
                while (index <= 4)
                {
                    colliderPosList.Add(collider.points[index]);
                    index++;
                }
                collider.SetPoints(colliderPosList);

                AirshipStatus airship = ShipStatus.Instance.Cast<AirshipStatus>();
                airship.Ladders = new Il2CppReferenceArray<Ladder>(airship.GetComponentsInChildren<Ladder>());

                ladderObj.transform.GetChild(2).gameObject.SetActive(false);
                ladderObj.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }
}
