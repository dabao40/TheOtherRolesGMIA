using System.Collections.Generic;
using TheOtherRoles.CustomGameModes;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Pursuer : RoleBase<Pursuer> {
        public PlayerControl target;
        public static Color color = Lawyer.color;
        public static List<PlayerControl> blankedList = [];
        public int blanks = 0;
        public static Sprite blank;
        public bool notAckedExiled = false;

        public Pursuer()
        {
            RoleId = roleId = RoleId.Pursuer;
            blanks = 0;
            notAckedExiled = false;
            target = null;
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            target = setTarget();
            setPlayerOutline(target, color);
        }

        public override void OnDeath(PlayerControl killer = null)
        {
            if (!FreePlayGM.isFreePlayGM) player.clearAllTasks();
        }

        public static float cooldown = 30f;
        public static int blanksNumber = 5;

        public static Sprite getTargetSprite() {
            if (blank) return blank;
            blank = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PursuerButton.png", 115f);
            return blank;
        }

        public static void clearAndReload() {
            blankedList = [];

            cooldown = CustomOptionHolder.pursuerCooldown.getFloat();
            blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.getFloat());
            players = [];
        }
    }
}
