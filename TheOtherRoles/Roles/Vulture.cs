using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Vulture : RoleBase<Vulture> {

        public Vulture()
        {
            RoleId = roleId = RoleId.Vulture;
            localArrows = [];
            eatenBodies = 0;
        }

        public static Color color = new Color32(139, 69, 19, byte.MaxValue);
        public List<Arrow> localArrows = [];
        public static float cooldown = 30f;
        public static int vultureNumberToWin = 4;
        public int eatenBodies = 0;
        public bool triggerVultureWin = false;
        public static bool canUseVents = true;
        public static bool showArrows = true;
        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VultureButton.png", 115f);
            return buttonSprite;
        }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer != player || localArrows == null || !showArrows) return;
            if (player.Data.IsDead) {
                foreach (Arrow arrow in localArrows) Object.Destroy(arrow.arrow);
                localArrows = [];
                return;
            }

            DeadBody[] deadBodies = Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = localArrows.Count != deadBodies.Count();
            int index = 0;

            if (arrowUpdate) {
                foreach (Arrow arrow in localArrows) Object.Destroy(arrow.arrow);
                localArrows = [];
            }

            foreach (DeadBody db in deadBodies) {
                if (arrowUpdate) {
                    localArrows.Add(new Arrow(Color.blue));
                    localArrows[index].arrow.SetActive(true);
                }
                if (localArrows[index] != null) localArrows[index].Update(db.transform.position);
                index++;
            }
        }

        public override void ResetRole(bool isShifted)
        {
            if (localArrows != null)
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
        }

        public static void clearAndReload() {
            if (players != null) players.Do(x => x.triggerVultureWin = false);
            vultureNumberToWin = Mathf.RoundToInt(CustomOptionHolder.vultureNumberToWin.getFloat());
            cooldown = CustomOptionHolder.vultureCooldown.getFloat();
            canUseVents = CustomOptionHolder.vultureCanUseVents.getBool();
            showArrows = CustomOptionHolder.vultureShowArrows.getBool();
            players = [];
        }
    }
}
