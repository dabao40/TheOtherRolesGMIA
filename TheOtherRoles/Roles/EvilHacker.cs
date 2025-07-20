using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class EvilHacker : RoleBase<EvilHacker>
    {
        public static Color color = Palette.ImpostorRed;
        public static bool canHasBetterAdmin = false;
        public bool canCreateMadmate = false;
        public static bool canSeeDoorStatus = true;
        public static bool canCreateMadmateFromJackal;
        public static bool canInheritAbility;
        public PlayerControl currentTarget;

        public EvilHacker()
        {
            RoleId = roleId = RoleId.EvilHacker;
            currentTarget = null;
            acTokenChallenge = null;
            canCreateMadmate = CustomOptionHolder.evilHackerCanCreateMadmate.getBool();
        }

        public AchievementToken<(bool admin, bool cleared)> acTokenChallenge = null;

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("evilHacker.challenge", (false, false), (val, _) => val.cleared);
        }

        private static Sprite madmateButtonSprite;

        public static Sprite getMadmateButtonSprite()
        {
            if (madmateButtonSprite) return madmateButtonSprite;
            madmateButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SidekickButton.png", 115f);
            return madmateButtonSprite;
        }

        public static bool isInherited()
        {
            return canInheritAbility && exists && livingPlayers.Count == 0 && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
        }

        public override void OnKill(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer == player)
            {
                acTokenChallenge.Value.cleared |= acTokenChallenge.Value.admin;
                acTokenChallenge.Value.admin = false;
            }
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            currentTarget = setTarget(true);
            setPlayerOutline(currentTarget, color);
        }

        public static void clearAndReload()
        {
            canHasBetterAdmin = CustomOptionHolder.evilHackerCanHasBetterAdmin.getBool();
            canCreateMadmateFromJackal = CustomOptionHolder.evilHackerCanCreateMadmateFromJackal.getBool();
            canInheritAbility = CustomOptionHolder.evilHackerCanInheritAbility.getBool();
            canSeeDoorStatus = CustomOptionHolder.evilHackerCanSeeDoorStatus.getBool();
            players = [];
        }
    }
}
