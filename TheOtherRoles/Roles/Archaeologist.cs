using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Archaeologist : RoleBase<Archaeologist>
    {
        public Archaeologist()
        {
            RoleId = roleId = RoleId.Archaeologist;
        }

        static public HelpSprite[] helpSprite = [new(getDetectSprite(), "archaeologistDetectHint"), new(getExcavateSprite(), "archaeologistExcavateHint")];

        public static Color color = new(71f / 255f, 93f / 255f, 206f / 255f);

        public enum RevealAntique
        {
            Never,
            Immediately,
            AfterMeeting
        }

        public override void FixedUpdate()
        {
            setTarget();
            fixedUpdate();
        }

        public void setTarget()
        {
            if (PlayerControl.LocalPlayer != player || PlayerControl.LocalPlayer.Data.IsDead || revealed == null || MapUtilities.CachedShipStatus?.AllVents == null) return;
            Antique target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach (var antique in revealed)
            {
                float distance = Vector2.Distance(antique.gameObject.transform.position, truePosition);
                if (distance <= usableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = antique;
                }
            }
            Archaeologist.target = target;
        }

        public void fixedUpdate()
        {
            if (PlayerControl.LocalPlayer == player && Antique.antiques != null && Antique.antiques.Any(x => x.isDetected))
            {
                if (!PlayerControl.LocalPlayer.Data.IsDead) {
                    var antique = Antique.antiques.FirstOrDefault(x => x.isDetected);
                    if (ShipStatus.Instance.FastRooms[antique.room].roomArea.OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition()))
                    {
                        antique.gameObject.SetActive(true);
                        revealed.Add(antique);
                    }
                }
                if (arrow?.arrow != null)
                {
                    if (Antique.antiques != null && arrowActive && !PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        var antique = Antique.antiques.FirstOrDefault(x => x.isDetected);
                        arrow.Update(ShipStatus.Instance.FastRooms[antique.room].roomArea.ClosestPoint(PlayerControl.LocalPlayer.transform.position));
                        arrow.arrow.SetActive(true);
                    }
                    else {
                        arrow.arrow.SetActive(false);
                    }
                }
            }
        }

        public static Arrow arrow = new(color);
        public static Antique target;
        public static Antique antiqueTarget;
        public static List<Antique> revealed = [];
        public static int numCandidates = 3;

        private static Sprite excavateSprite;
        public static Sprite getExcavateSprite()
        {
            if (excavateSprite) return excavateSprite;
            excavateSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ExcavateButton.png", 115f);
            return excavateSprite;
        }

        private static Sprite detectSprite;
        public static Sprite getDetectSprite()
        {
            if (detectSprite) return detectSprite;
            detectSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ArchaeologistDetectButton.png", 115f);
            return detectSprite;
        }
        public static bool arrowActive;
        public static float cooldown;
        /// <summary>
        /// The time the Archaeologist has to wait when detecting the antique
        /// </summary>
        public static float detectDuration;
        /// <summary>
        /// The duration of the discovery arrow
        /// </summary>
        public static float arrowDuration;
        public static RevealAntique revealAntique;

        public static bool hasRemainingAntique()
        {
            if (!exists || Antique.antiques == null || Antique.antiques.Count == 0) return false;
            var remainingList = Antique.antiques.Where(x => !x.isBroken).ToList();
            return remainingList.Count > 0;
        }

        public static (string, RoleInfo) getRoleInfo()
        {
            var list = new List<PlayerControl>(PlayerControl.AllPlayerControls.ToArray().ToList());
            list.Shuffle();
            while (list.Count > numCandidates) list.Remove(list.LastOrDefault());
            var role = RoleInfo.getRoleInfoForPlayer(list[rnd.Next(list.Count)], false, true).FirstOrDefault();
            string playerNames = string.Join(",", list.Select(p => p.Data.PlayerName));
            return (playerNames, role);
        }

        public static void clearArrows()
        {
            if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
            arrow = new Arrow(color);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }

        public override void ResetRole(bool isShifted)
        {
            clearArrows();
        }

        public static void clearAndReload()
        {
            arrowActive = false;
            target = null;
            antiqueTarget = null;
            cooldown = CustomOptionHolder.archaeologistCooldown.getFloat();
            detectDuration = CustomOptionHolder.archaeologistExploreDuration.getFloat();
            arrowDuration = CustomOptionHolder.archaeologistArrowDuration.getFloat();
            numCandidates = Mathf.RoundToInt(CustomOptionHolder.archaeologistNumCandidates.getFloat());
            revealAntique = (RevealAntique)CustomOptionHolder.archaeologistRevealAntiqueMode.getSelection();
            revealed = [];
            clearArrows();
            players = [];
        }
    }
}
