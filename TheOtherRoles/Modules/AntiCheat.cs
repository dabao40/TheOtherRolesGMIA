using System;
using Hazel;

namespace TheOtherRoles.Modules
{
    public static class GameStates //from TONX
    {
        public static bool IsLobby => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Joined;
        public static bool IsEnded => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Ended;
        public static bool IsNotJoined => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.NotJoined;
        public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
        public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
        public static bool IsExilling => ExileController.Instance != null;
        public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
        public static bool IsShip => ShipStatus.Instance != null;
        public static bool IsCanMove => PlayerControl.LocalPlayer?.CanMove is true;
        public static bool IsDead => PlayerControl.LocalPlayer?.Data?.IsDead is true;
    }
    public static class AntiCheat//参考了亿些EAC的代码而已（
    {
        public static int MeetingTimes = 0;
        public static bool RpcSafe(PlayerControl pc, byte callId, MessageReader reader)
        {
            if (!AmongUsClient.Instance.AmHost) return false;
            if (pc == null || reader == null) return false;
            try
            {
                MessageReader sr = MessageReader.Get(reader);
                var rpc = (RpcCalls)callId;
                switch (rpc)
                {
                    case RpcCalls.StartMeeting:
                        MeetingTimes++;
                        if ((MeetingHud.Instance && MeetingTimes > 3) || GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"The meeting has been initiated three times in a short period of time, which is impossible. Refusal,The Rpc requesters identified this time are:{pc.Data.PlayerName}");
                            return true;
                        }
                        break;
                    case RpcCalls.MurderPlayer:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"Received an unexpected kill request, which has been rejected. Sender:{pc.Data.PlayerName}");
                            return true;
                        }
                        break;

                }
                switch (callId)
                {
                    case 11:
                        MeetingTimes++;
                        if ((MeetingHud.Instance && MeetingTimes > 3) || GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"The meeting has been initiated three times in a short period of time, which is impossible. Refusal,The Rpc requesters identified this time are:{pc.Data.PlayerName}");
                            return true;
                        }
                        break;
                    case 47:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"Received an unexpected kill request, which has been rejected. Sender:{pc.Data.PlayerName}");
                            return true;
                        }
                        break;
                }

            }
            catch (Exception e)
            {
                TheOtherRolesPlugin.Logger.LogError("Error in " + e);
            }
            return false;
        }
    }
}