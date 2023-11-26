using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;
using System.IO;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace TheOtherRoles.Modules
{
    public static class Utils
    {
        //Get ShapeshifterMenu prefab to instantiate it
        //Found here: https://github.com/AlchlcDvl/TownOfUsReworked/blob/9f3cede9d30bab2c11eb7c960007ab3979f09156/TownOfUsReworked/Custom/Menu.cs
        public static ShapeshifterMinigame getShapeshifterMenu()
        {
            var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Shapeshifter);
            return UnityEngine.Object.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
        }
        //public static void getplayer()
        //{

        //}

    }
    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class ShapeShifterMenu
    {
        public static ShapeshifterMinigame playerpickMenu;
        public static bool IsActive;
        public static PlayerControl targetPlayer;
        public static Action customAction;
        public static List<PlayerControl> customPlayerList;

        //Open a custom menu to pick a player as a target
        public static void openPlayerPickMenu(List<PlayerControl> playerList, Action action)
        {
            IsActive = true;
            customPlayerList = playerList;
            customAction = action;

            //The menu is based off the shapeshifting menu
            playerpickMenu = UnityEngine.Object.Instantiate<ShapeshifterMinigame>(Utils.getShapeshifterMenu());

            playerpickMenu.transform.SetParent(Camera.main.transform, false);
            playerpickMenu.transform.localPosition = new Vector3(0f, 0f, -50f);
            playerpickMenu.Begin(null);
        }

        //Prefix patch of ShapeshifterMinigame.Begin to implement player pick menu logic
        public static bool Prefix(PlayerTask task, ShapeshifterMinigame __instance)
        {
            if (IsActive)
            { //Player pick menu logic

                //Custom player list set by openPlayerPickMenu
                List<PlayerControl> list = customPlayerList;

                __instance.potentialVictims = new List<ShapeshifterPanel>();
                List<UiElement> list2 = new List<UiElement>();

                for (int i = 0; i < list.Count; i++)
                {
                    PlayerControl player = list[i];
                    int num = i % 3;
                    int num2 = i / 3;
                    ShapeshifterPanel shapeshifterPanel = UnityEngine.Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, __instance.transform);
                    shapeshifterPanel.transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);

                    shapeshifterPanel.SetPlayer(i, player.Data, (Action)(() =>
                    {
                        targetPlayer = player; //Save targeted player

                        customAction.Invoke(); //Custom action set by openPlayerPickMenu

                        __instance.Close();
                    }));

                    shapeshifterPanel.NameText.color = new Color(1,1,1);
                    __instance.potentialVictims.Add(shapeshifterPanel);
                    list2.Add(shapeshifterPanel.Button);
                }

                ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2, false);

                IsActive = false;

                return false; //Skip original method when active

            }

            return true; //Open normal shapeshifter menu if not active
        }
    }
}
