using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class NiceWatcher : RoleBase<NiceWatcher>
    {
        public NiceWatcher()
        {
            RoleId = roleId = RoleId.NiceWatcher;
        }
    }

    public class EvilWatcher : RoleBase<EvilWatcher>
    {
        public EvilWatcher()
        {
            RoleId = roleId = RoleId.EvilWatcher;
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
