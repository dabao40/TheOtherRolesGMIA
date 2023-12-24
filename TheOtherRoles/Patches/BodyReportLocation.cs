using System;
using HarmonyLib;
using Hazel;
using TheOtherRoles.Players;

namespace TheOtherRoles.Patches
{
    public static class ReportBodyLocation
    {
        public static string LocationBody;

        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public class RecordLocation
        {
            [HarmonyPostfix]
            public static void postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.position.y != -3.25f)
                {
                    LocationBody =  __instance.text.text;
                }
                else 
                {
                    string name = PlayerControl.LocalPlayer.name;
                    LocationBody = $"a hallway/outside, {name} where is the body?";
                }
               
            }
         }
         [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
         public static class SendLocationBody
         {
           static DeadPlayer killer = null;
            public static DateTime dateTime;
            public static float KillAge {get;  set;}

            [HarmonyPostfix]
            public static void postfix([HarmonyArgument(0)] GameData.PlayerInfo target)
            {
                KillAge = (float)(DateTime.UtcNow - killer.timeOfDeath).TotalMilliseconds;

                string report =  $"Body reported in {LocationBody}, ";
                string additionInfo = $"Body Time Dead at : {Math.Round(KillAge / 1000)}s ago";
                string allinfo = report + additionInfo;

                if (target == null && CustomOptionHolder.LocationReport.getBool())
                
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, allinfo);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SendLocationBody, SendOption.Reliable);
                        writer.Write(allinfo);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
         }
 }

    