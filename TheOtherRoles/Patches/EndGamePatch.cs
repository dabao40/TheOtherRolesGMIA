  
using HarmonyLib;
using static TheOtherRoles.TheOtherRoles;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;
using TheOtherRoles.MetaContext;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;

namespace TheOtherRoles.Patches {
    enum CustomGameOverReason {
        LoversWin = 10,
        TeamJackalWin = 11,
        MiniLose = 12,
        JesterWin = 13,
        ArsonistWin = 14,
        VultureWin = 15,
        MoriartyWin = 16,
        AkujoWin = 17,
        PlagueDoctorWin = 18,
        JekyllAndHydeWin = 19,
        CupidLoversWin = 20,
        LawyerSoloWin = 21,
        FoxWin = 22
        //ProsecutorWin = 16
    }

    enum WinCondition {
        Default,
        LoversTeamWin,
        LoversSoloWin,
        JesterWin,
        JackalWin,
        MiniLose,
        ArsonistWin,
        VultureWin,
        LawyerSoloWin,
        AdditionalLawyerBonusWin,
        AdditionalAlivePursuerWin,
        AdditionalLawyerStolenWin,
        OpportunistWin,
        CrewmateWin, 
        ImpostorWin,
        FoxWin,
        MoriartyWin,
        AkujoWin,
        PlagueDoctorWin,
        JekyllAndHydeWin,
        CupidLoversWin,
        EveryoneDied
        //ProsecutorWin
    }

    static class AdditionalTempData {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition winCondition = WinCondition.Default;
        public static List<WinCondition> additionalWinConditions = new();
        public static List<PlayerRoleInfo> playerRoles = new();
        public static float timer = 0;
        public static Dictionary<int, PlayerControl> plagueDoctorInfected = new();
        public static Dictionary<int, float> plagueDoctorProgress = new();

        public static void clear() {
            playerRoles.Clear();
            additionalWinConditions.Clear();
            winCondition = WinCondition.Default;
            timer = 0;
        }

        internal class PlayerRoleInfo {
            public string PlayerName { get; set; }
            public List<RoleInfo> Roles {get;set;}
            public List<Color> RoleColors { get;set;}
            public string RoleNames { get; set; }
            public int TasksCompleted  {get;set;}
            public int TasksTotal  {get;set;}
            public int ExTasksCompleted { get; set; }
            public int ExTasksTotal { get; set; }
            public bool IsGuesser {get; set;}
            public int? Kills {get; set;}
            public byte PlayerId { get; set;}
            public bool IsAlive { get; set; }
            public bool IsMadmate { get; set;}
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch {
        private static GameOverReason gameOverReason;
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) {
            gameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;

            // Reset zoomed out ghosts
            Helpers.toggleZoom(reset: true);

            GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.GameEnd, null, 0) { RelatedTag = EventDetail.GameEnd });
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) {
            bool foxTaskComplete = Fox.tasksComplete();
            if (foxTaskComplete)
            {
                if (gameOverReason == GameOverReason.HumansByTask && !Fox.crewWinsByTasks)
                {
                    gameOverReason = (GameOverReason)CustomGameOverReason.FoxWin;
                }
                else if (gameOverReason == GameOverReason.ImpostorBySabotage && !Fox.impostorWinsBySabotage)
                {
                    gameOverReason = (GameOverReason)CustomGameOverReason.FoxWin;
                }
                else if (gameOverReason is GameOverReason.HumansByVote or GameOverReason.ImpostorByKill or GameOverReason.ImpostorByVote)
                {
                    gameOverReason = (GameOverReason)CustomGameOverReason.FoxWin;
                }
            }

            AdditionalTempData.clear();

            foreach(var playerControl in CachedPlayer.AllPlayers) {
                var roles = RoleInfo.getRoleInfoForPlayer(playerControl, true, true);
                var colors = roles.Select(x => x.color).ToList();
                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(playerControl.Data);
                bool isGuesser = HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(playerControl.PlayerId);
                bool isMadmate = Madmate.madmate.Any(x => x.PlayerId ==  playerControl.PlayerId) || playerControl.PlayerId == CreatedMadmate.createdMadmate?.PlayerId;
                int? killCount = GameHistory.deadPlayers.FindAll(x => x.killerIfExisting != null && x.killerIfExisting.PlayerId == playerControl.PlayerId).Count;
                if (killCount == 0 && !(new List<RoleInfo>() { RoleInfo.sheriff, RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.thief, RoleInfo.moriarty, RoleInfo.jekyllAndHyde }.Contains(RoleInfo.getRoleInfoForPlayer(playerControl, false).FirstOrDefault()) || playerControl.Data.Role.IsImpostor)) {
                    killCount = null;
                    }
                byte playerId = playerControl.PlayerId;
                bool isTaskMaster = TaskMaster.isTaskMaster(playerControl.PlayerId);
                bool isTaskMasterExTasks = isTaskMaster && TaskMaster.isTaskComplete;
                string roleString = RoleInfo.GetRolesString(playerControl, true, true, false, true);
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Roles = roles, RoleColors = colors, RoleNames = roleString ,TasksTotal = tasksTotal, TasksCompleted = tasksCompleted,
                    ExTasksTotal = isTaskMasterExTasks ? TaskMaster.allExTasks : isTaskMaster ? TaskMasterTaskHelper.GetTaskMasterTasks() : 0,
                    ExTasksCompleted = isTaskMasterExTasks ? TaskMaster.clearExTasks : 0,
                    IsGuesser = isGuesser, Kills = killCount, PlayerId = playerId, IsAlive = !playerControl.Data.IsDead, IsMadmate = isMadmate });
            }
            AdditionalTempData.plagueDoctorInfected = PlagueDoctor.infected;
            AdditionalTempData.plagueDoctorProgress = PlagueDoctor.progress;

            // Remove Jester, Opportunist, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new();
            if (Jester.jester != null) notWinners.Add(Jester.jester);
            if (Sidekick.sidekick != null) notWinners.Add(Sidekick.sidekick);
            if (Jackal.jackal != null) notWinners.Add(Jackal.jackal);
            if (Arsonist.arsonist != null) notWinners.Add(Arsonist.arsonist);
            if (Vulture.vulture != null) notWinners.Add(Vulture.vulture);
            if (Lawyer.lawyer != null) notWinners.Add(Lawyer.lawyer);
            if (Opportunist.opportunist != null) notWinners.Add(Opportunist.opportunist);
            if (Pursuer.pursuer != null) notWinners.Add(Pursuer.pursuer);
            if (Thief.thief != null) notWinners.Add(Thief.thief);
            if (Shifter.shifter != null && Shifter.isNeutral) notWinners.Add(Shifter.shifter);
            if (Moriarty.moriarty != null) notWinners.Add(Moriarty.moriarty);
            if (Moriarty.formerMoriarty != null) notWinners.Add(Moriarty.formerMoriarty);
            if (Akujo.akujo != null) notWinners.Add(Akujo.akujo);
            if (PlagueDoctor.plagueDoctor != null) notWinners.Add(PlagueDoctor.plagueDoctor);
            if (JekyllAndHyde.jekyllAndHyde != null) notWinners.Add(JekyllAndHyde.jekyllAndHyde);
            if (JekyllAndHyde.formerJekyllAndHyde != null) notWinners.Add(JekyllAndHyde.formerJekyllAndHyde);
            if (Cupid.cupid != null) notWinners.Add(Cupid.cupid);
            if (Fox.fox != null) notWinners.Add(Fox.fox);
            if (Immoralist.immoralist != null) notWinners.Add(Immoralist.immoralist);
            if (SchrodingersCat.schrodingersCat != null) notWinners.Add(SchrodingersCat.schrodingersCat);
            if (SchrodingersCat.formerSchrodingersCat != null) notWinners.Add(SchrodingersCat.formerSchrodingersCat);

            notWinners.AddRange(Jackal.formerJackals);

            List<CachedPlayerData> winnersToRemove = new();
            foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) EndGameResult.CachedWinners.Remove(winner);

            // Putting them all in one doesn't work
            bool saboWin = gameOverReason == GameOverReason.ImpostorBySabotage;
            bool impostorkillWin = gameOverReason == GameOverReason.ImpostorByKill;
            bool impostorvoteWin = gameOverReason == GameOverReason.ImpostorByVote;
            bool impostorWin = saboWin || impostorkillWin || impostorvoteWin;

            bool taskWin = gameOverReason == GameOverReason.HumansByTask;
            bool crewmatevoteWin = gameOverReason == GameOverReason.HumansByVote;
            bool crewmateWin = taskWin || crewmatevoteWin;

            bool jesterWin = Jester.jester != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            bool arsonistWin = Arsonist.arsonist != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            bool miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
            bool loversWin = Lovers.existingAndAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin || (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.existingWithKiller())); // Either they win if they are among the last 3 players, or they win if they are both Crewmates and both alive and the Crew wins (Team Imp/Jackal Lovers can only win solo wins)
            bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin && ((Jackal.jackal != null && !Jackal.jackal.Data.IsDead) || (Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead));
            bool vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
            bool lawyerSoloWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.LawyerSoloWin;
            bool moriartyWin = Moriarty.moriarty != null && gameOverReason == (GameOverReason)CustomGameOverReason.MoriartyWin;
            bool akujoWin = Akujo.akujo != null && gameOverReason == (GameOverReason)CustomGameOverReason.AkujoWin && (Akujo.honmei != null && !Akujo.honmei.Data.IsDead && !Akujo.akujo.Data.IsDead);
            bool plagueDoctorWin = PlagueDoctor.plagueDoctor != null && gameOverReason == (GameOverReason)CustomGameOverReason.PlagueDoctorWin;
            bool foxWin = Fox.fox != null && gameOverReason == (GameOverReason)CustomGameOverReason.FoxWin;
            bool jekyllAndHydeWin = JekyllAndHyde.jekyllAndHyde != null && gameOverReason == (GameOverReason)CustomGameOverReason.JekyllAndHydeWin;
            bool cupidLoversWin = Cupid.lovers1 != null && Cupid.lovers2 != null && !Cupid.lovers1.Data.IsDead && !Cupid.lovers2.Data.IsDead && gameOverReason == (GameOverReason)CustomGameOverReason.CupidLoversWin;
            bool everyoneDead = AdditionalTempData.playerRoles.All(x => !x.IsAlive);
            //bool prosecutorWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.ProsecutorWin;

            // Here we changed this to: The Pursuer wins no matter who wins except for sabotage
            //bool isPursurerLose = jesterWin || arsonistWin || miniLose || vultureWin || teamJackalWin;

            // Crewmates Win
            if (crewmateWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (!p.Data.Role.IsImpostor && !Helpers.isNeutral(p) && !Madmate.madmate.Contains(p) && CreatedMadmate.createdMadmate != p)
                    {
                        CachedPlayerData wpd = new(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                }
                AdditionalTempData.winCondition = WinCondition.CrewmateWin;
            }

            // Impostors Win
            if (impostorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p.Data.Role.IsImpostor)
                    {
                        CachedPlayerData wpd = new(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                    else if (p == SchrodingersCat.schrodingersCat)
                    {
                        if (SchrodingersCat.team == SchrodingersCat.Team.Impostor)
                        {
                            CachedPlayerData wpd = new(p.Data);
                            EndGameResult.CachedWinners.Add(wpd);
                        }
                    }
                    else if (p == SchrodingersCat.formerSchrodingersCat)
                    {
                        if (SchrodingersCat.team == SchrodingersCat.Team.Impostor)
                        {
                            CachedPlayerData wpd = new(p.Data);
                            EndGameResult.CachedWinners.Add(wpd);
                        }
                    }
                }
                AdditionalTempData.winCondition = WinCondition.ImpostorWin;
            }

            // Mini lose
            if (miniLose) {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Mini.mini.Data)
                {
                    IsYou = false // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
                };
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.MiniLose;  
            }

            // Jester win
            else if (jesterWin) {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Jester.jester.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.JesterWin;
            }

            // Arsonist win
            else if (arsonistWin) {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Arsonist.arsonist.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            }

            else if (plagueDoctorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(PlagueDoctor.plagueDoctor.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.PlagueDoctorWin;
            }

            // Everyone Died
            else if (everyoneDead)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                AdditionalTempData.winCondition = WinCondition.EveryoneDied;
            }

            else if (jekyllAndHydeWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(JekyllAndHyde.jekyllAndHyde.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.JekyllAndHydeWin;

                if (JekyllAndHyde.formerJekyllAndHyde != null)
                {
                    CachedPlayerData wpdFormerJekyllAndHyde = new(JekyllAndHyde.formerJekyllAndHyde.Data);
                    EndGameResult.CachedWinners.Add(wpdFormerJekyllAndHyde);
                }

                if (SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde)
                {
                    if (SchrodingersCat.schrodingersCat != null)
                    {
                        CachedPlayerData wpdSchrodingersCat = new(SchrodingersCat.schrodingersCat.Data);
                        EndGameResult.CachedWinners.Add(wpdSchrodingersCat);
                    }
                    if (SchrodingersCat.formerSchrodingersCat != null)
                    {
                        CachedPlayerData wpdSchrodingersCat = new(SchrodingersCat.formerSchrodingersCat.Data);
                        EndGameResult.CachedWinners.Add(wpdSchrodingersCat);
                    }
                }
            }

            else if (moriartyWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Moriarty.moriarty.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.MoriartyWin;

                if (Moriarty.formerMoriarty != null)
                {
                    CachedPlayerData wpdFormerMoriarty = new(Moriarty.formerMoriarty.Data);
                    EndGameResult.CachedWinners.Add(wpdFormerMoriarty);
                }

                if (SchrodingersCat.team == SchrodingersCat.Team.Moriarty)
                {
                    if (SchrodingersCat.schrodingersCat != null)
                    {
                        CachedPlayerData wpdSchrodingersCat = new(SchrodingersCat.schrodingersCat.Data);
                        EndGameResult.CachedWinners.Add(wpdSchrodingersCat);
                    }
                    if (SchrodingersCat.formerSchrodingersCat != null)
                    {
                        CachedPlayerData wpdSchrodingersCat = new(SchrodingersCat.formerSchrodingersCat.Data);
                        EndGameResult.CachedWinners.Add(wpdSchrodingersCat);
                    }
                }
            }

            // Vulture win
            else if (vultureWin) {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Vulture.vulture.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.VultureWin;
            }

            // Akujo win
            else if (akujoWin)
            {
                AdditionalTempData.winCondition = WinCondition.AkujoWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Akujo.akujo.Data));
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Akujo.honmei.Data));
            }            

            // Jester win
            /*else if (prosecutorWin) {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Lawyer.lawyer.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ProsecutorWin;
            }*/

            // Lovers win conditions
            else if (loversWin) {
                // Double win for lovers, crewmates also win
                if (!Lovers.existingWithKiller()) {
                    AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                        if (p == null) continue;
                        if (p == Lovers.lover1 || p == Lovers.lover2)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p == Pursuer.pursuer && !Pursuer.pursuer.Data.IsDead)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p == Cupid.lovers1 || p == Cupid.lovers2)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p != Jester.jester && p != Jackal.jackal && p != Sidekick.sidekick && p != Arsonist.arsonist && p != Vulture.vulture && !Jackal.formerJackals.Contains(p) && !p.Data.Role.IsImpostor && p != Moriarty.moriarty && p != Akujo.akujo && p != JekyllAndHyde.jekyllAndHyde
                            && p != Moriarty.formerMoriarty && p != JekyllAndHyde.formerJekyllAndHyde && p != PlagueDoctor.plagueDoctor && !Madmate.madmate.Contains(p) && p != CreatedMadmate.createdMadmate && p != Fox.fox && p != Immoralist.immoralist)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                    }
                }
                // Lovers solo win
                else {
                    AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.lover1.Data));
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.lover2.Data));
                }
            }

            else if (cupidLoversWin)
            {
                AdditionalTempData.winCondition = WinCondition.CupidLoversWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Cupid.lovers1.Data));
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Cupid.lovers2.Data));
            }
            
            // Jackal win condition (should be implemented using a proper GameOverReason in the future)
            else if (teamJackalWin) {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.JackalWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Jackal.jackal.Data)
                {
                    IsImpostor = false
                };
                EndGameResult.CachedWinners.Add(wpd);
                // If there is a sidekick. The sidekick also wins
                if (Sidekick.sidekick != null) {
                    CachedPlayerData wpdSidekick = new(Sidekick.sidekick.Data)
                    {
                        IsImpostor = false
                    };
                    EndGameResult.CachedWinners.Add(wpdSidekick);
                }
                foreach(var player in Jackal.formerJackals) {
                    CachedPlayerData wpdFormerJackal = new(player.Data)
                    {
                        IsImpostor = false
                    };
                    EndGameResult.CachedWinners.Add(wpdFormerJackal);
                }
                if (SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                {
                    if (SchrodingersCat.schrodingersCat != null)
                    {
                        CachedPlayerData wpdSchrodingersCat = new(SchrodingersCat.schrodingersCat.Data) {
                            IsImpostor = false
                        };
                        EndGameResult.CachedWinners.Add(wpdSchrodingersCat);
                    }
                    if (SchrodingersCat.formerSchrodingersCat != null)
                    {
                        CachedPlayerData wpdSchrodingersCat = new(SchrodingersCat.formerSchrodingersCat.Data) {
                            IsImpostor = false
                        };
                        EndGameResult.CachedWinners.Add(wpdSchrodingersCat);
                    }
                }
            }

            else if (lawyerSoloWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Lawyer.lawyer.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.LawyerSoloWin;
            }

            else if (foxWin)
            {
                AdditionalTempData.winCondition = WinCondition.FoxWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Fox.fox.Data);
                EndGameResult.CachedWinners.Add(wpd);

                if (Immoralist.immoralist != null)
                {
                    CachedPlayerData wpdI = new(Immoralist.immoralist.Data);
                    EndGameResult.CachedWinners.Add(wpdI);
                }
            }

            // Madmate win with impostors
            if (EndGameResult.CachedWinners.ToArray().Any(x => x.IsImpostor))
            {
                if (Madmate.madmate != null)
                {
                    foreach (var p in Madmate.madmate)
                    {
                        CachedPlayerData wpd = new(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                }
                if (CreatedMadmate.createdMadmate != null)
                {
                    CachedPlayerData wpd = new(CreatedMadmate.createdMadmate.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                }
            }

            // Possible Additional winner: Lawyer
            // && !Lawyer.isProsecutor
            if (Lawyer.lawyer != null && Lawyer.target != null && (!Lawyer.target.Data.IsDead || Lawyer.target == Jester.jester) && !Pursuer.notAckedExiled) {
                CachedPlayerData winningClient = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) {
                    if (winner.PlayerName == Lawyer.target.Data.PlayerName)
                        winningClient = winner;
                }
                if (winningClient != null) { // The Lawyer wins if the client is winning (and alive, but if he wasn't the Lawyer shouldn't exist anymore)
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Lawyer.lawyer.Data.PlayerName))
                    {
                        if (!Lawyer.lawyer.Data.IsDead)
                        {
                            EndGameResult.CachedWinners.Remove(winningClient);
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(Lawyer.lawyer.Data));
                            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerStolenWin); // The Lawyer replaces the client's victory
                        }
                        else
                        {
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(Lawyer.lawyer.Data));
                            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerBonusWin); // The Lawyer wins with the client
                        }                       
                    }                    
                } 
            }

            // Cupid wins with both cupid Loverse
            if (Cupid.cupid != null && Cupid.lovers1 != null && Cupid.lovers2 != null)
            {
                if (EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Cupid.lovers1.Data.PlayerName) &&
                    EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Cupid.lovers2.Data.PlayerName) && 
                    !EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Cupid.cupid.Data.PlayerName)) {
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Cupid.cupid.Data));
                }
            }

            // Possible Additional winner: Pursuer
            if (Pursuer.pursuer != null && !Pursuer.pursuer.Data.IsDead && !Pursuer.notAckedExiled && !saboWin && !arsonistWin && !miniLose)
            {
                if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Pursuer.pursuer.Data.PlayerName))
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Pursuer.pursuer.Data));
                AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAlivePursuerWin);
            }

            // Possible Additional winner: Opportunist
            if (Opportunist.opportunist != null && !Opportunist.opportunist.Data.IsDead && !saboWin && !arsonistWin && !miniLose)
            {
                if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Opportunist.opportunist.Data.PlayerName))
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Opportunist.opportunist.Data));
                AdditionalTempData.additionalWinConditions.Add(WinCondition.OpportunistWin);
            }

            AdditionalTempData.timer = ((float)(DateTime.UtcNow - HideNSeek.startTime).TotalMilliseconds) / 1000;

            // Reset Settings
            if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek) ShipStatusPatch.resetVanillaSettings();
            RPCProcedure.resetVariables();
            EventUtility.gameEndsUpdate();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch {
        public static void Postfix(EndGameManager __instance) {
            // Delete and readd PoolablePlayers always showing the name and role of the player
            foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>()) {
                UnityEngine.Object.Destroy(pb.gameObject);
            }
            int num = Mathf.CeilToInt(7.5f);
            List<CachedPlayerData> list = EndGameResult.CachedWinners.ToArray().ToList().OrderBy(delegate(CachedPlayerData b)
            {
                if (!b.IsYou)
                {
                    return 0;
                }
                return -1;
            }).ToList();
            for (int i = 0; i < list.Count; i++) {
                CachedPlayerData CachedPlayerData2 = list[i];
                int num2 = (i % 2 == 0) ? -1 : 1;
                int num3 = (i + 1) / 2;
                float num4 = (float)num3 / (float)num;
                float num5 = Mathf.Lerp(1f, 0.75f, num4);
                float num6 = (float)((i == 0) ? -8 : -1);
                PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
                float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
                Vector3 vector = new(num7, num7, 1f);
                poolablePlayer.transform.localScale = vector;
                if (CachedPlayerData2.IsDead)
                {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                }
                else {
                    poolablePlayer.SetFlipX(i % 2 == 0);
                }

                poolablePlayer.UpdateFromPlayerOutfit(CachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, CachedPlayerData2.IsDead, true);

                poolablePlayer.cosmetics.nameText.color = Color.white;
                poolablePlayer.cosmetics.nameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
                poolablePlayer.cosmetics.nameText.transform.localPosition = new Vector3(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y, -15f);
                poolablePlayer.cosmetics.nameText.text = CachedPlayerData2.PlayerName;

                foreach(var data in AdditionalTempData.playerRoles) {
                    if (data.PlayerName != CachedPlayerData2.PlayerName) continue;
                    var roles =
                    //poolablePlayer.cosmetics.nameText.text += $"\n{string.Join("\n", data.Roles.Select(x => Helpers.cs(x.color, x.name)))}";
                    poolablePlayer.cosmetics.nameText.text += $"\n{string.Join("\n", data.Roles.Select(x => Helpers.cs((data.IsMadmate && !x.isModifier) ? Madmate.color : data.RoleColors[data.Roles.IndexOf(x)], 
                    (data.IsMadmate && !x.isModifier) ? (x == RoleInfo.crewmate ? Madmate.fullName : (Madmate.prefix + x.name)) : x.name)))}";
                }
            }

            TORGUIManager.Instance.StartCoroutine(Achievement.CoShowAchievements(TORGUIManager.Instance, Achievement.UniteAll()).WrapToIl2Cpp());
            RPCProcedure.resetAchievement();
            IEnumerator CoShowStatistics()
            {
                yield return new WaitForSeconds(0.4f);
                var viewer = Helpers.CreateObject<GameStatisticsViewer>("Statistics", __instance.transform, new Vector3(0f, 2.5f, -20f), LayerMask.NameToLayer("UI"));
                viewer.PlayerPrefab = __instance.PlayerPrefab;
                viewer.GameEndText = __instance.WinText;
            }
            __instance.StartCoroutine(CoShowStatistics().WrapToIl2Cpp());

            // Additional code
            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";
            string nonModTranslationText = "";

            if (AdditionalTempData.winCondition == WinCondition.JesterWin)
            {
                nonModTranslationText = "jesterWin";
                textRenderer.text = ModTranslation.getString("jesterWin");
                textRenderer.color = Jester.color;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.ArsonistWin)
            {
                nonModTranslationText = "arsonistWin";
                textRenderer.text = ModTranslation.getString("arsonistWin");
                textRenderer.color = Arsonist.color;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.VultureWin)
            {
                nonModTranslationText = "vultureWin";
                textRenderer.text = ModTranslation.getString("vultureWin");
                textRenderer.color = Vulture.color;
                __instance.BackgroundBar.material.SetColor("_Color", Vulture.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.LawyerSoloWin)
            {
                nonModTranslationText = "lawyerWin";
                textRenderer.text = ModTranslation.getString("lawyerWin");
                textRenderer.color = Lawyer.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lawyer.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.PlagueDoctorWin)
            {
                nonModTranslationText = "plagueDoctorWin";
                textRenderer.text = ModTranslation.getString("plagueDoctorWin");
                textRenderer.color = PlagueDoctor.color;
                __instance.BackgroundBar.material.SetColor("_Color", PlagueDoctor.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.FoxWin)
            {
                nonModTranslationText = "foxWin";
                textRenderer.text = ModTranslation.getString("foxWin");
                textRenderer.color = Fox.color;
                __instance.BackgroundBar.material.SetColor("_Color", Fox.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.JekyllAndHydeWin)
            {
                nonModTranslationText = "jekyllAndHydeWin";
                textRenderer.text = ModTranslation.getString("jekyllAndHydeWin");
                textRenderer.color = JekyllAndHyde.color;
                __instance.BackgroundBar.material.SetColor("_Color", JekyllAndHyde.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.LoversTeamWin)
            {
                nonModTranslationText = "crewLoverWin";
                textRenderer.text = ModTranslation.getString("crewLoverWin");
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);                
            }
            else if (AdditionalTempData.winCondition == WinCondition.LoversSoloWin || AdditionalTempData.winCondition == WinCondition.CupidLoversWin)
            {
                nonModTranslationText = "loversWin";
                textRenderer.text = ModTranslation.getString("loversWin");
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.JackalWin)
            {
                nonModTranslationText = "jackalWin";
                textRenderer.text = ModTranslation.getString("jackalWin");
                textRenderer.color = Jackal.color;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.EveryoneDied)
            {
                textRenderer.text = ModTranslation.getString("everyoneDied");
                textRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
            }
            else if (AdditionalTempData.winCondition == WinCondition.MiniLose)
            {
                nonModTranslationText = "miniDied";
                textRenderer.text = ModTranslation.getString("miniDied");
                textRenderer.color = Mini.color;
            }
            else if (AdditionalTempData.winCondition == WinCondition.AkujoWin)
            {
                nonModTranslationText = "akujoWin";
                textRenderer.text = ModTranslation.getString("akujoWin");
                textRenderer.color = Akujo.color;
                __instance.BackgroundBar.material.SetColor("_Color", Akujo.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.MoriartyWin)
            {
                nonModTranslationText = "moriartyWin";
                textRenderer.text = ModTranslation.getString("moriartyWin");
                textRenderer.color = Moriarty.color;
                __instance.BackgroundBar.material.SetColor("_Color", Moriarty.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.CrewmateWin)
            {
                nonModTranslationText = "crewWin";
                textRenderer.text = ModTranslation.getString("crewWin");
                textRenderer.color = Palette.White;
            }
            else if (AdditionalTempData.winCondition == WinCondition.ImpostorWin)
            {
                nonModTranslationText = "impostorWin";
                textRenderer.text = ModTranslation.getString("impostorWin");
                textRenderer.color = Palette.ImpostorRed;
            }
            foreach (WinCondition cond in AdditionalTempData.additionalWinConditions)
            {
                if (cond == WinCondition.OpportunistWin)
                {
                    textRenderer.text = string.Format(ModTranslation.getString(nonModTranslationText + "Extra"), ModTranslation.getString("opportunistExtra"));
                }
            }

            foreach (WinCondition cond in AdditionalTempData.additionalWinConditions) {
                if (cond == WinCondition.AdditionalLawyerBonusWin) {
                    textRenderer.text += $"\n{Helpers.cs(Lawyer.color, ModTranslation.getString("lawyerExtraBonus"))}";
                } else if (cond == WinCondition.AdditionalAlivePursuerWin) {
                    textRenderer.text += $"\n{Helpers.cs(Pursuer.color, ModTranslation.getString("pursuerExtraBonus"))}";
                }
                else if (cond == WinCondition.AdditionalLawyerStolenWin)
                {
                    textRenderer.text += $"\n{Helpers.cs(Lawyer.color, ModTranslation.getString("lawyerExtraStolen"))}";
                }
            }

            if (TORMapOptions.showRoleSummary || HideNSeek.isHideNSeekGM) {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -214f); 
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                if (HideNSeek.isHideNSeekGM) {
                    int minutes = (int)AdditionalTempData.timer / 60;
                    int seconds = (int)AdditionalTempData.timer % 60;
                    roleSummaryText.AppendLine($"<color=#FAD934FF>Time: {minutes:00}:{seconds:00}</color> \n");
                }
                roleSummaryText.AppendLine(ModTranslation.getString("roleSummaryText"));
                bool plagueExists = AdditionalTempData.playerRoles.Any(x => x.Roles.Contains(RoleInfo.plagueDoctor));
                foreach (var data in AdditionalTempData.playerRoles) {
                    //var roles = string.Join(" ", data.Roles.Select(x => Helpers.cs(x.color, x.name)));
                    string roles = data.RoleNames;
                    //if (data.IsGuesser) roles += " (Guesser)";
                    var taskInfo = data.TasksTotal > 0 ? $" - <color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>" : "";
                    var exTaskInfo = data.ExTasksTotal > 0 ? $" - Ex <color=#E1564BFF>({data.ExTasksCompleted}/{data.ExTasksTotal})</color>" : "";
                    if (data.Kills != null) taskInfo += String.Format(ModTranslation.getString("roleSummaryKillsInfo"), data.Kills);
                    string infectionInfo = "";
                    if (plagueExists && !data.Roles.Contains(RoleInfo.plagueDoctor))
                    {                        
                        if (AdditionalTempData.plagueDoctorInfected.ContainsKey(data.PlayerId))
                        {
                            infectionInfo += " - " + Helpers.cs(Color.red, ModTranslation.getString("plagueDoctorInfectedText"));
                        }
                        else
                        {
                            float progress = AdditionalTempData.plagueDoctorProgress.ContainsKey(data.PlayerId) ? AdditionalTempData.plagueDoctorProgress[data.PlayerId] : 0f;
                            infectionInfo += " - " + PlagueDoctor.getProgressString(progress);
                        }
                    }
                    roleSummaryText.AppendLine($"{Helpers.cs(data.IsAlive ? Color.white : new Color(.7f,.7f,.7f), data.PlayerName)} - {roles}{taskInfo}{exTaskInfo}{infectionInfo}"); 
                }
                TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
                roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1.5f;
                roleSummaryTextMesh.fontSizeMax = 1.5f;
                roleSummaryTextMesh.fontSize = 1.5f;
                
                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
            AdditionalTempData.clear();
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))] 
    class CheckEndCriteriaPatch {
        public static bool Prefix(ShipStatus __instance) {
            if (!GameData.Instance) return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
                return true;
            if (FreePlayGM.isFreePlayGM) return false;
            var statistics = new PlayerStatistics(__instance);
            if (CheckAndEndGameForMiniLose(__instance)) return false;
            if (CheckAndEndGameForJesterWin(__instance)) return false;
            if (CheckAndEndGameForLawyerMeetingWin(__instance)) return false;
            if (CheckAndEndGameForArsonistWin(__instance)) return false;
            if (CheckAndEndGameForVultureWin(__instance)) return false;
            if (CheckAndEndGameForPlagueDoctorWin(__instance)) return false;
            if (CheckAndEndGameForJekyllAndHydeWin(__instance, statistics)) return false;
            if (CheckAndEndGameForMoriartyWin(__instance, statistics)) return false;
            if (CheckAndEndGameForSabotageWin(__instance)) return false;
            if (CheckAndEndGameForTaskWin(__instance)) return false;
            //if (CheckAndEndGameForProsecutorWin(__instance)) return false;
            if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCupidLoversWin(__instance, statistics)) return false;
            if (CheckAndEndGameForAkujoWin(__instance, statistics)) return false;
            if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
            if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
            return false;
        }

        private static bool CheckAndEndGameForMiniLose(ShipStatus __instance) {
            if (Mini.triggerMiniLose) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.MiniLose, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJesterWin(ShipStatus __instance) {
            if (Jester.triggerJesterWin) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance) {
            if (Arsonist.triggerArsonistWin) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForVultureWin(ShipStatus __instance) {
            if (Vulture.triggerVultureWin) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.VultureWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForLawyerMeetingWin(ShipStatus __instance)
        {
            if (Lawyer.triggerLawyerWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.LawyerSoloWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForPlagueDoctorWin(ShipStatus __instance)
        {
            if (PlagueDoctor.triggerPlagueDoctorWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PlagueDoctorWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJekyllAndHydeWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (JekyllAndHyde.triggerWin || (statistics.TeamJekyllAndHydeAlive >= statistics.TotalAlive - statistics.TeamJekyllAndHydeAlive &&
                        statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0 && statistics.TeamMoriartyAlive == 0 && statistics.TeamSheriffAlive == 0))
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JekyllAndHydeWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance) {
            if (MapUtilities.Systems == null) return false;
            var systemType = MapUtilities.Systems.ContainsKey(SystemTypes.LifeSupp) ? MapUtilities.Systems[SystemTypes.LifeSupp] : null;
            if (systemType != null) {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f) {
                    EndGameForSabotage(__instance);
                    lifeSuppSystemType.Countdown = 10000f;
                    return true;
                }
            }
            var systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Reactor) ? MapUtilities.Systems[SystemTypes.Reactor] : null;
            if (systemType2 == null) {
                systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Laboratory) ? MapUtilities.Systems[SystemTypes.Laboratory] : null;
            }
            if (systemType2 != null) {
                ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSystem != null && criticalSystem.Countdown < 0f) {
                    EndGameForSabotage(__instance);
                    criticalSystem.ClearSabotage();
                    return true;
                }
            }
            return false;
        }

        private static bool CheckAndEndGameForTaskWin(ShipStatus __instance) {
            if (HideNSeek.isHideNSeekGM && !HideNSeek.taskWinPossible) return false;
            if ((GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) || (TaskMaster.triggerTaskMasterWin && TaskMaster.taskMaster != null)) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
                return true;
            }
            return false;
        }

        /*private static bool CheckAndEndGameForProsecutorWin(ShipStatus __instance) {
            if (Lawyer.triggerProsecutorWin) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ProsecutorWin, false);
                return true;
            }
            return false;
        }*/

        private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.TeamLoversAlive == 2 && statistics.TotalAlive <= 3) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.LoversWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForCupidLoversWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (statistics.TeamCupidLoversAlive == 2 && statistics.TotalAlive <= 3)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.CupidLoversWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForAkujoWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (statistics.TeamAkujoAlive == 2 && statistics.TotalAlive <= 3)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.AkujoWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJackalWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive && statistics.TeamImpostorsAlive == 0 && statistics.TeamSheriffAlive == 0 && statistics.TeamMoriartyAlive == 0 && statistics.TeamJekyllAndHydeAlive == 0 && !(statistics.TeamJackalHasAliveLover && (statistics.TeamLoversAlive == 2 || statistics.TeamCupidLoversAlive == 2 || statistics.TeamAkujoAlive == 2))) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForMoriartyWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if ((statistics.TeamMoriartyAlive >= statistics.TotalAlive - statistics.TeamMoriartyAlive && statistics.TeamImpostorsAlive == 0 && statistics.TeamSheriffAlive == 0 && statistics.TeamJackalAlive == 0 && statistics.TeamJekyllAndHydeAlive == 0) || Moriarty.triggerMoriartyWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.MoriartyWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (HideNSeek.isHideNSeekGM) 
                if ((0 != statistics.TotalAlive - statistics.TeamImpostorsAlive)) return false;

            if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive && statistics.TeamJackalAlive == 0 && statistics.TeamSheriffAlive == 0 && statistics.TeamMoriartyAlive == 0 && statistics.TeamJekyllAndHydeAlive == 0 && !(statistics.TeamImpostorHasAliveLover && (statistics.TeamLoversAlive == 2 || statistics.TeamCupidLoversAlive == 2 || statistics.TeamAkujoAlive == 2))) {
                //__instance.enabled = false;
                GameOverReason endReason;
                switch (GameData.LastDeathReason) {
                    case DeathReason.Exile:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                    case DeathReason.Kill:
                        endReason = GameOverReason.ImpostorByKill;
                        break;
                    default:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                }
                GameManager.Instance.RpcEndGame(endReason, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForCrewmateWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (HideNSeek.isHideNSeekGM && HideNSeek.timer <= 0 && !HideNSeek.isWaitingTimer) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            if (statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0 && statistics.TeamMoriartyAlive == 0 && statistics.TeamJekyllAndHydeAlive == 0 && statistics.TeamAkujoAlive <= 1) {
                //__instance.enabled = false;
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            return false;
        }

        private static void EndGameForSabotage(ShipStatus __instance) {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
            return;
        }

    }

    internal class PlayerStatistics {
        public int TeamImpostorsAlive {get;set;}
        public int TeamJackalAlive {get;set;}
        public int TeamSheriffAlive { get; set; }
        public int TeamMoriartyAlive { get; set; }
        public int TeamJekyllAndHydeAlive { get;set; }
        public int TeamLoversAlive {get;set;}
        public int TeamCupidLoversAlive { get; set; }
        public int TeamAkujoAlive { get;set; }
        public int TotalAlive {get;set;}
        public bool TeamImpostorHasAliveLover {get;set;}
        public bool TeamJackalHasAliveLover {get;set;}

        public PlayerStatistics(ShipStatus __instance) {
            GetPlayerCounts();
        }

        private bool isLover(NetworkedPlayerInfo p) {
            return (Lovers.lover1 != null && Lovers.lover1.PlayerId == p.PlayerId) || (Lovers.lover2 != null && Lovers.lover2.PlayerId == p.PlayerId);
        }

        private bool isCupidLover(NetworkedPlayerInfo p)
        {
            return (Cupid.lovers1 != null && Cupid.lovers1.PlayerId == p.PlayerId) || (Cupid.lovers2 != null && Cupid.lovers2.PlayerId == p.PlayerId);
        }

        private bool isAkujoHonmei(NetworkedPlayerInfo p) { 
            return Akujo.akujo != null && Akujo.honmei != null && Akujo.honmei.PlayerId == p.PlayerId;
        }

        private void GetPlayerCounts() {
            int numJackalAlive = 0;
            int numImpostorsAlive = 0;
            int numSheriffAlive = 0;
            int numMoriartyAlive = 0;
            int numJekyllAndHydeAlive = 0;
            int numLoversAlive = 0;
            int numCupidLoversAlive = 0;
            int numAkujoAlive = 0;
            int numTotalAlive = 0;
            bool impLover = false;
            bool jackalLover = false;

            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (!playerInfo.Disconnected)
                {
                    if (!playerInfo.IsDead)
                    {
                        numTotalAlive++;

                        bool lover = isLover(playerInfo);
                        bool cupidLover = isCupidLover(playerInfo);
                        bool akujoHonmei = isAkujoHonmei(playerInfo);
                        if (lover) numLoversAlive++;
                        if (cupidLover) numCupidLoversAlive++;

                        if (playerInfo.Role.IsImpostor) {
                            numImpostorsAlive++;
                            if (lover || cupidLover || akujoHonmei) impLover = true;
                        }
                        if (Jackal.jackal != null && Jackal.jackal.PlayerId == playerInfo.PlayerId) {
                            numJackalAlive++;
                            if (lover || cupidLover || akujoHonmei) jackalLover = true;
                        }
                        if (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == playerInfo.PlayerId) {
                            numJackalAlive++;
                            if (lover || cupidLover || akujoHonmei) jackalLover = true;
                        }
                        if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal && SchrodingersCat.schrodingersCat.PlayerId == playerInfo.PlayerId) {
                            numJackalAlive++;
                            if (lover || cupidLover || akujoHonmei) jackalLover = true;
                        }
                        if (Sheriff.sheriff != null && Sheriff.sheriff.PlayerId == playerInfo.PlayerId && !Madmate.madmate.Contains(Sheriff.sheriff)) {
                            numSheriffAlive++;
                        }
                        if (Deputy.deputy != null && Deputy.deputy.PlayerId == playerInfo.PlayerId && Deputy.stopsGameEnd && !Madmate.madmate.Contains(Deputy.deputy)) {
                            numSheriffAlive++;
                        }
                        if (Moriarty.moriarty != null && Moriarty.moriarty.PlayerId == playerInfo.PlayerId) {
                            numMoriartyAlive++;
                        }
                        if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Moriarty && SchrodingersCat.schrodingersCat.PlayerId == playerInfo.PlayerId) {
                            numMoriartyAlive++;
                        }
                        if (JekyllAndHyde.jekyllAndHyde != null && JekyllAndHyde.jekyllAndHyde.PlayerId == playerInfo.PlayerId) { 
                            numJekyllAndHydeAlive++;
                        }
                        if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde && SchrodingersCat.schrodingersCat.PlayerId == playerInfo.PlayerId) {
                            numJekyllAndHydeAlive++;
                        }
                        if (Akujo.akujo != null && Akujo.akujo.PlayerId == playerInfo.PlayerId) { 
                            numAkujoAlive++;
                        }
                        if (Akujo.honmei != null && Akujo.honmei.PlayerId == playerInfo.PlayerId) {
                            numAkujoAlive++;
                        }
                    }
                }
            }

            // Count the Mimic as one if enabled
            if (MimicK.mimicK != null && MimicA.mimicA != null && !MimicK.mimicK.Data.IsDead && !MimicA.mimicA.Data.IsDead && MimicK.countAsOne)
            {
                numImpostorsAlive--;
                numTotalAlive--;
            }

            if (BomberA.bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && BomberA.countAsOne)
            {
                numImpostorsAlive--;
                numTotalAlive--;
            }

            TeamJackalAlive = numJackalAlive;
            TeamImpostorsAlive = numImpostorsAlive;
            TeamLoversAlive = numLoversAlive;
            TeamCupidLoversAlive = numCupidLoversAlive;
            TeamAkujoAlive = numAkujoAlive;
            TeamSheriffAlive = numSheriffAlive;
            TeamMoriartyAlive = numMoriartyAlive;
            TeamJekyllAndHydeAlive = numJekyllAndHydeAlive;
            TotalAlive = numTotalAlive;
            TeamImpostorHasAliveLover = impLover;
            TeamJackalHasAliveLover = jackalLover;
        }
    }
}
