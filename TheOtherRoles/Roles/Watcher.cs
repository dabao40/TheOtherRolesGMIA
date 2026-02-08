using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Patches;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;

namespace TheOtherRoles.Roles
{
    public class NiceWatcher : RoleBase<NiceWatcher>
    {
        public bool canKill = false;
        public PlayerControl currentTarget = null;
        public NiceWatcher()
        {
            RoleId = roleId = RoleId.NiceWatcher;
            canKill = false;
            currentTarget = null;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            canKill = false;
        }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer != player || PlayerControl.LocalPlayer.Data.IsDead) return;
            currentTarget = setTarget();
            if (canKill) setPlayerOutline(currentTarget, Watcher.color);
        }
    }

    public class EvilWatcher : RoleBase<EvilWatcher>
    {
        public bool extraKill = false;
        public EvilWatcher()
        {
            RoleId = roleId = RoleId.EvilWatcher;
            extraKill = false;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            extraKill = false;
        }

        public override void OnKill(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer == player && extraKill)
            {
                extraKill = false;
                PlayerControl.LocalPlayer.SetKillTimerUnchecked(1f);
            }
        }
    }

    public static class Watcher
    {
        public static Color color = Palette.Purple;
        public static bool canSeeGuesses = false;
        public static bool canSeeYasunaVotes = false;

        public static void clearAndReload()
        {
            canSeeGuesses = CustomOptionHolder.watcherSeeGuesses.getBool();
            canSeeYasunaVotes = CustomOptionHolder.watcherSeeYasunaVotes.getBool();
            NiceWatcher.players = [];
            EvilWatcher.players = [];
        }
    }
}
