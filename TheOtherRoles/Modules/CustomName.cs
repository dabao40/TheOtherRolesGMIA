using AmongUs.Data;

namespace TheOtherRoles.Modules
{
    public static class CustomName
    {

        public static void ApplySuffix()
        {
            string name = DataManager.player.Customization.Name;
            string fname = DataManager.player.Customization.Name;
            if (name == null) return;
            if(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                if (EOSManager.Instance.FriendCode == null) return;
                if (EOSManager.Instance.FriendCode.GetDevUser().HasTag())
                {
                    name = EOSManager.Instance.FriendCode.GetDevUser().GetTag() + name;
                }
                if(name != PlayerControl.LocalPlayer.name)PlayerControl.LocalPlayer.RpcSetName(name);
            }
            else
            {
                PlayerControl.LocalPlayer.RpcSetName(fname);
            }

            
        }
    }
}
