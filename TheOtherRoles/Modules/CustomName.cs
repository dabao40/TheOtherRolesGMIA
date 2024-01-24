using AmongUs.Data;

namespace TheOtherRoles.Modules
{
    public static class CustomName
    {

        public static void ApplySuffix()
        {

            if (!GameStates.IsLobby) return;
            string name = DataManager.player.Customization.Name;
            if (EOSManager.Instance.FriendCode.GetDevUser().HasTag())
            {
                name = EOSManager.Instance.FriendCode.GetDevUser().GetTag() + name;
            }
            PlayerControl.LocalPlayer.RpcSetName(name);



        }
    }
}
