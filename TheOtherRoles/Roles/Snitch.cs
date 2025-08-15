using System;
using System.Collections.Generic;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Snitch : RoleBase<Snitch> {
        public static Color color = new Color32(184, 251, 79, byte.MaxValue);

        public static List<Arrow> localArrows = [];
        public List<Arrow> snitchArrows = [];
        public static int taskCountForReveal = 1;
        public static bool includeTeamEvil = false;
        public static bool teamEvilUseDifferentArrowColor = true;

        public Snitch()
        {
            RoleId = roleId = RoleId.Snitch;
            snitchArrows = [];
        }

        public override void ResetRole(bool isShifted)
        {
            if (snitchArrows != null)
                foreach (Arrow arrow in snitchArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            snitchArrows = [];
        }

        public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
        {
            if (player == this.player)
            {
                if (localArrows != null)
                    foreach (Arrow arrow in localArrows)
                        if (arrow?.arrow != null)
                            UnityEngine.Object.Destroy(arrow.arrow);
                localArrows = [];
            }
        }

        public override void OnMeetingStart()
        {
            if (PlayerControl.LocalPlayer == player)
            {
                var (taskComplete, taskTotal) = TasksHandler.taskInfo(player.Data);
                if (!PlayerControl.LocalPlayer.Data.IsDead && taskTotal > 0 && taskComplete == taskTotal) _ = new StaticAchievementToken("snitch.challenge");
            }
        }

        public override void FixedUpdate()
        {
            if (localArrows == null || snitchArrows == null) return;
            foreach (Arrow arrow in localArrows) arrow.arrow.SetActive(false);
            foreach (Arrow arrow in snitchArrows) arrow.arrow.SetActive(false);

            if (!hasAlivePlayers) return;
            var local = PlayerControl.LocalPlayer;

            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(player.Data);
            int numberOfTasks = playerTotal - playerCompleted;

            if (!local.Data.IsDead && (local.Data.Role.IsImpostor || (includeTeamEvil && (local.isRole(RoleId.Jackal) || local.isRole(RoleId.Sidekick)
                || local.isRole(RoleId.Moriarty) || local.isRole(RoleId.JekyllAndHyde) || local.isRole(RoleId.Fox)) || local.isRole(RoleId.Immoralist)
                || (local.isRole(RoleId.SchrodingersCat) && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate))))
            {
                int arrowIndex = 0;
                foreach (var snitch in allPlayers) {
                    var (complete, total) = TasksHandler.taskInfo(snitch.Data);
                    int remaining = total - complete;
                    if (remaining > taskCountForReveal || snitch.Data.IsDead) continue;
                    if (arrowIndex >= localArrows.Count)
                        localArrows.Add(new Arrow(Color.blue));
                    if (arrowIndex < localArrows.Count && localArrows[arrowIndex] != null)
                    {
                        localArrows[arrowIndex].arrow.SetActive(true);
                        localArrows[arrowIndex].Update(snitch.transform.position, Color.blue);
                    }
                    arrowIndex++;
                }
            }
            else if (local == player && numberOfTasks == 0)
            {
                int arrowIndex = 0;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    bool arrowForImp = p.Data.Role.IsImpostor;
                    bool arrowForTeamJackal = includeTeamEvil && (p.isRole(RoleId.Jackal) || p.isRole(RoleId.Sidekick) || (p.isRole(RoleId.SchrodingersCat) && SchrodingersCat.team == SchrodingersCat.Team.Jackal));
                    bool arrowForFox = includeTeamEvil && (p.isRole(RoleId.Fox) || p.isRole(RoleId.Immoralist));
                    bool arrowForJekyll = includeTeamEvil && (p.isRole(RoleId.JekyllAndHyde) || (p.isRole(RoleId.SchrodingersCat) && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde));
                    bool arrowForMoriarty = includeTeamEvil && (p.isRole(RoleId.Moriarty) || (p.isRole(RoleId.SchrodingersCat) && SchrodingersCat.team == SchrodingersCat.Team.Moriarty));

                    Color color = Palette.ImpostorRed;
                    if (teamEvilUseDifferentArrowColor) {
                        if (arrowForTeamJackal) color = Jackal.color;
                        else if (arrowForFox) color = Fox.color;
                        else if (arrowForJekyll) color = JekyllAndHyde.color;
                        else if (arrowForMoriarty) color = Moriarty.color;
                    }

                    if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal || arrowForFox || arrowForJekyll || arrowForMoriarty))
                    {
                        if (arrowIndex >= snitchArrows.Count)
                            snitchArrows.Add(new Arrow(color));
                        if (arrowIndex < snitchArrows.Count && snitchArrows[arrowIndex] != null)
                        {
                            snitchArrows[arrowIndex].arrow.SetActive(true);
                            snitchArrows[arrowIndex].Update(p.transform.position, color);
                        }
                        arrowIndex++;
                    }
                }
            }
        }

        public static void clearAndReload() {
            localArrows = [];
            taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
            includeTeamEvil = CustomOptionHolder.snitchIncludeTeamEvil.getBool();
            teamEvilUseDifferentArrowColor = CustomOptionHolder.snitchTeamEvilUseDifferentArrowColor.getBool();
            players = [];
        }
    }
}
