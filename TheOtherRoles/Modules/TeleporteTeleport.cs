using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;

namespace TheOtherRoles.Modules
{
    public static  class TeleporteTeleport
    {
        public static PlayerControl firsttarget;
        public static PlayerControl secondtarget;
        public static  void Teleport()
        {
            if (ShapeShifterMenu.playerpickMenu != null)
            {
                ShapeShifterMenu.playerpickMenu.Close();
            }
            List<PlayerControl> playerList = new List<PlayerControl>();

            //All players are saved to playerList apart from LocalPlayer
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.AmOwner)
                {
                    playerList.Add(player);
                }
            }
            ShapeShifterMenu.openPlayerPickMenu(playerList, (Action)(() =>
            {
                firsttarget = ShapeShifterMenu.targetPlayer;
            }));
         
            if (firsttarget = null) return;
            ShapeShifterMenu.openPlayerPickMenu(playerList, (Action)(() =>
            {
                secondtarget = ShapeShifterMenu.targetPlayer;
            }));
      
            if (secondtarget = null) return;
            Vector3 firstplayerpo = firsttarget.transform.position;
            Vector3 secondplayerpo = secondtarget.transform.position;
            firsttarget.NetTransform.RpcSnapTo(secondplayerpo);
            secondtarget.NetTransform.RpcSnapTo(firstplayerpo);
            firsttarget = null;
            secondtarget = null;



        }
    }
}
