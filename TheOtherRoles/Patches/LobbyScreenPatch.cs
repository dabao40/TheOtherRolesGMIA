using System.Text.RegularExpressions;
using AmongUs.Data;
using HarmonyLib;
using InnerNet;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Modules;
using TMPro;
using UnityEngine;
namespace TheOtherRoles.Patches
{
    [HarmonyPatch]
    public static class LobbyJoin
    {
        private static int GameId;

        private static GameObject LobbyText;

        private static TextMeshPro Text;
        private static bool JoiningAttempted;

        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
        [HarmonyPostfix]
        public static void Postfix(InnerNetClient __instance)
        {
            GameId = __instance.GameId;
        }

        [HarmonyPatch(typeof(EnterCodeManager), nameof(EnterCodeManager.OnEnable))]
        [HarmonyPostfix]
        public static void OnEnable(EnterCodeManager __instance)
        {
            if (LobbyText)
            {
                LobbyText.SetActive(GameId != 0);
                return;
            }

            LobbyText = Object.Instantiate(__instance.transform.FindChild("Header").gameObject, __instance.transform);
            LobbyText.name = "LobbyText";
            Text = LobbyText.transform.GetChild(1).GetComponent<TextMeshPro>();
            Text.fontSizeMin = 3.35f;
            Text.fontSizeMax = 3.35f;
            Text.fontSize = 3.35f;
            Text.text = string.Empty;
            Text.alignment = TextAlignmentOptions.Center;
            LobbyText.transform.localPosition = new Vector3(1f, 0f, 0f);
            LobbyText.transform.GetChild(0).gameObject.Destroy();
            LobbyText.SetActive(GameId != 0);
        }

        [HarmonyPatch(typeof(EnterCodeManager), nameof(EnterCodeManager.OnDisable))]
        [HarmonyPostfix]
        public static void OnDisable()
        {
            if (LobbyText)
            {
                LobbyText.SetActive(false);
                JoiningAttempted = false;
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        [HarmonyPostfix]
        public static void Update()
        {
            if (GameId == 0 || !LobbyText || !LobbyText.active)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && !JoiningAttempted)
            {
                AmongUsClient.Instance.StartCoroutine(AmongUsClient.Instance.CoFindGameInfoFromCodeAndJoin(GameId));
                JoiningAttempted = true;
            }

            if (LobbyText && Text)
            {
                var code = GameCode.IntToGameName(GameId);
                if (DataManager.Settings.Gameplay.StreamerMode)
                {
                    code = "******";
                }

                Text.text = $"<size=110%>{ModTranslation.getString("lobbyJoinBindText1")}</size>"
                            + $"\n<size=4.6f>({code})</size>"
                            + $"[LShift]";
            }
        }
    }
}
