
using System;
using Hazel;
using Il2CppSystem.Runtime.Remoting.Messaging;

namespace TheOtherRoles.Modules
{
    public static class GMIAAntiCheat//参考了亿些EAC的代码而已（
    {
        public static int MeetingTimes = 0;
        public static bool RpcSafe(PlayerControl pc, byte callId, MessageReader reader)
        {
           
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
                    case RpcCalls.Shapeshift:
                    case RpcCalls.RejectShapeshift:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"Received a shapechange request that should not exist and has been rejected. Sender:{pc.Data.PlayerName}");
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
                    case RpcCalls.SnapTo:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"SnapTo request should not have been received in the lobby, rejected. Sender:{pc.Data.PlayerName}");
                            return true;

                        }
                    break;
                    case RpcCalls.SetScanner:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"Scan request that should not exist has been rejected. Sender:{pc.Data.PlayerName}");
                            return true;
                        }
                
                        bool hasthistask = false;
                        foreach ( var task in pc.Data.Tasks)
                        {
                            if(task.TypeId == (byte)TaskTypes.SubmitScan)
                            {
                                hasthistask = true;
                            }

                        }
                        if (!hasthistask)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"This player has no scanning task and the request has been rejected. Sender:{pc.Data.PlayerName}");
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
                    case 46:
                    case 56:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"Received a shapechange request that should not exist and has been rejected. Sender:{pc.Data.PlayerName}");
                            return true;
                        }
                    break;
                    case 21:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"SnapTo request should not have been received in the lobby, rejected Sender:{pc.Data.PlayerName}");
                            return true;

                        }
                    break;
                    case 15:
                        if (GameStates.IsLobby)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"Scan request that should not exist has been rejected. Sender:{pc.Data.PlayerName}");
                            return true;
                        }

                        bool hasthistask = false;
                        foreach (var task in pc.Data.Tasks)
                        {
                            if (task.TypeId == (byte)TaskTypes.SubmitScan)
                            {
                                hasthistask = true;
                            }

                        }
                        if (!hasthistask)
                        {
                            TheOtherRolesPlugin.Logger.LogError($"This player has no scanning task and the request has been rejected. Sender:{pc.Data.PlayerName}");
                            return true;
                        }

                        break;
                }

                }
            catch(Exception e)
            {
               TheOtherRolesPlugin.Logger.LogError( "Error in "+e);
            }
                return false;
        }
    }
}
