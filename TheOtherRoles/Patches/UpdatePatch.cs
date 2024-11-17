using HarmonyLib;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using AmongUs.GameOptions;
using TheOtherRoles.Modules;
using TheOtherRoles.MetaContext;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        private static Dictionary<byte, (string name, Color color)> TagColorDict = new();
        static void resetNameTagsAndColors() {
            var localPlayer = CachedPlayer.LocalPlayer.PlayerControl;
            var myData = CachedPlayer.LocalPlayer.PlayerControl.Data;
            var amImpostor = myData.Role.IsImpostor;
            var morphTimerNotUp = Morphling.morphTimer > 0f;
            var morphTargetNotNull = Morphling.morphTarget != null;

            var dict = TagColorDict;
            dict.Clear();
            
            foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                var player = data.Object;
                string text = data.PlayerName;
                Color color;
                if (player)
                {
                    var playerName = text;
                    if (morphTimerNotUp && morphTargetNotNull && Morphling.morphling == player) playerName = Morphling.morphTarget.Data.PlayerName;
                    if (MimicA.isMorph && MimicA.mimicA == player && MimicA.mimicA != null && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead) playerName = MimicK.mimicK.Data.PlayerName;
                    if (MimicK.mimicK != null && MimicK.victim != null && MimicK.mimicK == player) playerName = MimicK.victim.Data.PlayerName;
                    var nameText = player.cosmetics.nameText;
                
                    nameText.text = Helpers.hidePlayerName(localPlayer, player) ? "" : playerName;
                    nameText.color = color = amImpostor && data.Role.IsImpostor ? Palette.ImpostorRed : Color.white;
                    nameText.color = nameText.color.SetAlpha(Chameleon.visibility(player.PlayerId));
                }
                else
                {
                    color = Color.white;
                }
                
                
                dict.Add(data.PlayerId, (text, color));
            }
            
            if (MeetingHud.Instance != null) 
            {
                foreach (PlayerVoteArea playerVoteArea in MeetingHud.Instance.playerStates)
                {
                    var data = dict[playerVoteArea.TargetPlayerId];
                    var text = playerVoteArea.NameText;
                    text.text = data.name;
                    text.color = data.color;
                }
            }
        }

        static void setPlayerNameColor(PlayerControl p, Color color) {
            p.cosmetics.nameText.color = color.SetAlpha(Chameleon.visibility(p.PlayerId));
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = color;
        }

        static void setNameColors() {
            var localPlayer = CachedPlayer.LocalPlayer.PlayerControl;
            var localRole = RoleInfo.getRoleInfoForPlayer(localPlayer, false).FirstOrDefault();
            setPlayerNameColor(localPlayer, localRole.color);

            /*if (Jester.jester != null && Jester.jester == localPlayer)
                setPlayerNameColor(Jester.jester, Jester.color);
            else if (Mayor.mayor != null && Mayor.mayor == localPlayer)
                setPlayerNameColor(Mayor.mayor, Mayor.color);
            else if (Engineer.engineer != null && Engineer.engineer == localPlayer)
                setPlayerNameColor(Engineer.engineer, Engineer.color);
            else if (Sheriff.sheriff != null && Sheriff.sheriff == localPlayer) {
                setPlayerNameColor(Sheriff.sheriff, Sheriff.color);
                if (Deputy.deputy != null && Deputy.knowsSheriff) {
                    setPlayerNameColor(Deputy.deputy, Deputy.color);
                }
            } else*/
            if (Deputy.deputy != null && Deputy.deputy == localPlayer) 
            {
                setPlayerNameColor(Deputy.deputy, Deputy.color);
                if (Sheriff.sheriff != null && Deputy.knowsSheriff) {
                    setPlayerNameColor(Sheriff.sheriff, Sheriff.color);
                }
            }
            else if (Sheriff.sheriff != null && Sheriff.sheriff == localPlayer)
            {
                setPlayerNameColor(Sheriff.sheriff, Sheriff.color);
                if (Deputy.deputy != null && Deputy.knowsSheriff) setPlayerNameColor(Deputy.deputy, Sheriff.color);
            }
            /*else if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == localPlayer)
                setPlayerNameColor(Portalmaker.portalmaker, Portalmaker.color);
            else if (Lighter.lighter != null && Lighter.lighter == localPlayer)
                setPlayerNameColor(Lighter.lighter, Lighter.color);
            else if (Detective.detective != null && Detective.detective == localPlayer)
                setPlayerNameColor(Detective.detective, Detective.color);
            else if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == localPlayer)
                setPlayerNameColor(TimeMaster.timeMaster, TimeMaster.color);
            else if (Medic.medic != null && Medic.medic == localPlayer)
                setPlayerNameColor(Medic.medic, Medic.color);
            else if (Shifter.shifter != null && Shifter.shifter == localPlayer)
                setPlayerNameColor(Shifter.shifter, Shifter.color);
            else if (Swapper.swapper != null && Swapper.swapper == localPlayer)
                setPlayerNameColor(Swapper.swapper, Swapper.color);
            else if (Seer.seer != null && Seer.seer == localPlayer)
                setPlayerNameColor(Seer.seer, Seer.color);
            else if (Hacker.hacker != null && Hacker.hacker == localPlayer)
                setPlayerNameColor(Hacker.hacker, Hacker.color);
            else if (Tracker.tracker != null && Tracker.tracker == localPlayer)
                setPlayerNameColor(Tracker.tracker, Tracker.color);
            else if (Snitch.snitch != null && Snitch.snitch == localPlayer)
                setPlayerNameColor(Snitch.snitch, Snitch.color);*/
            else if (Jackal.jackal != null && Jackal.jackal == localPlayer) {
                // Jackal can see his sidekick
                setPlayerNameColor(Jackal.jackal, Jackal.color);
                if (Sidekick.sidekick != null) {
                    setPlayerNameColor(Sidekick.sidekick, Jackal.color);
                }
                if (Jackal.fakeSidekick != null) {
                    setPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
                }
                if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, Jackal.color);
            }
            else if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == localPlayer && (FortuneTeller.isCompletedNumTasks(PlayerControl.LocalPlayer) || PlayerControl.LocalPlayer.Data.IsDead))
            {
                setPlayerNameColor(FortuneTeller.fortuneTeller, FortuneTeller.color);
            }
            else if (TaskMaster.taskMaster != null && TaskMaster.taskMaster == localPlayer)
            {
                setPlayerNameColor(TaskMaster.taskMaster, !TaskMaster.becomeATaskMasterWhenCompleteAllTasks || TaskMaster.isTaskComplete ? TaskMaster.color : RoleInfo.crewmate.color);
            }
            else if (Swapper.swapper != null && Swapper.swapper == localPlayer)
            {
                setPlayerNameColor(Swapper.swapper, Swapper.swapper.Data.Role.IsImpostor ? Palette.ImpostorRed : Swapper.color);
            }
            else if (Yasuna.yasuna != null && Yasuna.yasuna == localPlayer)
            {
                setPlayerNameColor(Yasuna.yasuna, localPlayer.Data.Role.IsImpostor ? Palette.ImpostorRed : Yasuna.color);
            }
            else if (Prophet.prophet != null && Prophet.prophet == localPlayer)
            {
                setPlayerNameColor(Prophet.prophet, Prophet.color);
                if (Prophet.examined != null && !localPlayer.Data.IsDead) // Reset the name tags when Prophet is dead
                {
                    foreach (var p in Prophet.examined)
                    {
                        setPlayerNameColor(p.Key, p.Value ? Palette.ImpostorRed : Color.green);
                    }
                }
            }
            else if (Kataomoi.kataomoi != null && Kataomoi.kataomoi == localPlayer)
            {
                setPlayerNameColor(Kataomoi.kataomoi, Kataomoi.color);
                if (Kataomoi.target != null)
                    setPlayerNameColor(Kataomoi.target, Kataomoi.color);
            }
            /*else if (Spy.spy != null && Spy.spy == localPlayer) {
                setPlayerNameColor(Spy.spy, Spy.color);
            } else if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == localPlayer) {
                setPlayerNameColor(SecurityGuard.securityGuard, SecurityGuard.color);
            } else if (Arsonist.arsonist != null && Arsonist.arsonist == localPlayer) {
                setPlayerNameColor(Arsonist.arsonist, Arsonist.color);
            } else if (Guesser.niceGuesser != null && Guesser.niceGuesser == localPlayer) {
                setPlayerNameColor(Guesser.niceGuesser, Guesser.color);
            } else if (Guesser.evilGuesser != null && Guesser.evilGuesser == localPlayer) {
                setPlayerNameColor(Guesser.evilGuesser, Palette.ImpostorRed);
            } else if (Vulture.vulture != null && Vulture.vulture == localPlayer) {
                setPlayerNameColor(Vulture.vulture, Vulture.color);
            } else if (Medium.medium != null && Medium.medium == localPlayer) {
                setPlayerNameColor(Medium.medium, Medium.color);
            } else if (Trapper.trapper != null && Trapper.trapper == localPlayer) {
                setPlayerNameColor(Trapper.trapper, Trapper.color);
            } else if (Lawyer.lawyer != null && Lawyer.lawyer == localPlayer) {
                setPlayerNameColor(Lawyer.lawyer, Lawyer.color);
            } else if (Pursuer.pursuer != null && Pursuer.pursuer == localPlayer) {
                setPlayerNameColor(Pursuer.pursuer, Pursuer.color);
            }*/

            // No else if here, as a Lover of team Jackal needs the colors
            if (Sidekick.sidekick != null && Sidekick.sidekick == localPlayer) {
                // Sidekick can see the jackal
                setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
                if (Jackal.jackal != null) {
                    setPlayerNameColor(Jackal.jackal, Jackal.color);
                }
                if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, Jackal.color);
            }

            if (SchrodingersCat.schrodingersCat != null && localPlayer == SchrodingersCat.schrodingersCat)
            {
                if (SchrodingersCat.team == SchrodingersCat.Team.Impostor)
                {
                    foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p.Data.Role.IsImpostor) setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
                else if (SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                {
                    if (Jackal.jackal != null) setPlayerNameColor(Jackal.jackal, Jackal.color);
                    if (Sidekick.sidekick != null) setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
                }
                else if (SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde && JekyllAndHyde.jekyllAndHyde != null)
                    setPlayerNameColor(JekyllAndHyde.jekyllAndHyde, JekyllAndHyde.color);
                else if (SchrodingersCat.team == SchrodingersCat.Team.Moriarty && Moriarty.moriarty != null)
                    setPlayerNameColor(Moriarty.moriarty, Moriarty.color);
            }

            if (SchrodingersCat.schrodingersCat != null)
            {
                if (localPlayer == JekyllAndHyde.jekyllAndHyde && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, JekyllAndHyde.color);
                if (localPlayer == Moriarty.moriarty && SchrodingersCat.team == SchrodingersCat.Team.Moriarty)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, Moriarty.color);
            }

            // No else if here, as the Impostors need the Spy name to be colored
            if (Spy.spy != null && localPlayer.Data.Role.IsImpostor) {
                setPlayerNameColor(Spy.spy, Spy.color);
            }
            if (Sidekick.sidekick != null && Sidekick.wasTeamRed && localPlayer.Data.Role.IsImpostor) {
                setPlayerNameColor(Sidekick.sidekick, Spy.color);
            }
            if (Jackal.jackal != null && Jackal.wasTeamRed && localPlayer.Data.Role.IsImpostor) {
                setPlayerNameColor(Jackal.jackal, Spy.color);
            }
            if (Fox.fox != null && localPlayer == Fox.fox) { 
                setPlayerNameColor(localPlayer, Fox.color);
                if (Immoralist.immoralist != null) { 
                    setPlayerNameColor(Immoralist.immoralist, Immoralist.color);
                }
            }
            if (Immoralist.immoralist != null && localPlayer == Immoralist.immoralist) {
                setPlayerNameColor(localPlayer, Immoralist.color);
                if (Fox.fox != null) { 
                    setPlayerNameColor(Fox.fox, Immoralist.color);
                }
            }
            if (Madmate.madmate.Contains(localPlayer))
            {
                setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Madmate.color);
                if (Madmate.tasksComplete(localPlayer))
                {
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p == Spy.spy || p.Data.Role.IsImpostor || (p == Jackal.jackal && Jackal.wasTeamRed) || (p == Sidekick.sidekick && Sidekick.wasTeamRed))
                        {
                            setPlayerNameColor(p, Palette.ImpostorRed);
                        }
                    }
                }
            }
            if (localPlayer == CreatedMadmate.createdMadmate)
            {
                setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Madmate.color);
                if (CreatedMadmate.tasksComplete(localPlayer))
                {
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p == Spy.spy || p.Data.Role.IsImpostor || (p == Jackal.jackal && Jackal.wasTeamRed) || (p == Sidekick.sidekick && Sidekick.wasTeamRed))
                        {
                            setPlayerNameColor(p, Palette.ImpostorRed);
                        }
                    }
                }
            }
            // Crewmate roles with no changes: Mini
            // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
        }

        static void setNameTags() {
            // Mafia
            if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor) {
                foreach (PlayerControl player in CachedPlayer.AllPlayers)
                    if (Godfather.godfather != null && Godfather.godfather == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + $" ({ModTranslation.getString("mafiaG")})";
                    else if (Mafioso.mafioso != null && Mafioso.mafioso == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + $" ({ModTranslation.getString("mafiaM")})";
                    else if (Janitor.janitor != null && Janitor.janitor == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + $" ({ModTranslation.getString("mafiaJ")})";
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Godfather.godfather != null && Godfather.godfather.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Godfather.godfather.Data.PlayerName + $" ({ModTranslation.getString("mafiaG")})";
                        else if (Mafioso.mafioso != null && Mafioso.mafioso.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Mafioso.mafioso.Data.PlayerName + $" ({ModTranslation.getString("mafiaM")})";
                        else if (Janitor.janitor != null && Janitor.janitor.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Janitor.janitor.Data.PlayerName + $" ({ModTranslation.getString("mafiaJ")})";
            }

            // Lovers
            if (Lovers.lover1 != null && Lovers.lover2 != null && (Lovers.lover1 == CachedPlayer.LocalPlayer.PlayerControl || Lovers.lover2 == CachedPlayer.LocalPlayer.PlayerControl)) {
                string suffix = Helpers.cs(Lovers.color, " ♥");
                Lovers.lover1.cosmetics.nameText.text += suffix;
                Lovers.lover2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Lovers.lover1.PlayerId == player.TargetPlayerId || Lovers.lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Cupid
            if (Cupid.lovers1 != null && Cupid.lovers2 != null && (Cupid.lovers1 == CachedPlayer.LocalPlayer.PlayerControl || Cupid.lovers2 == CachedPlayer.LocalPlayer.PlayerControl || (Cupid.cupid != null && CachedPlayer.LocalPlayer.PlayerControl == Cupid.cupid)))
            {
                string suffix = Helpers.cs(Cupid.color, " ♥");
                Cupid.lovers1.cosmetics.nameText.text += suffix;
                Cupid.lovers2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Cupid.lovers1.PlayerId == player.TargetPlayerId || Cupid.lovers2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Kataomoi
            if (Kataomoi.kataomoi != null && Kataomoi.kataomoi == CachedPlayer.LocalPlayer.PlayerControl && Kataomoi.target != null)
            {
                string suffix = Helpers.cs(Kataomoi.color, " ♥");
                Kataomoi.target.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Kataomoi.target.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Akujo
            if (Akujo.akujo != null && (Akujo.keeps != null || Akujo.honmei != null))
            {
                if (Akujo.keeps != null)
                {
                    foreach (PlayerControl p in Akujo.keeps)
                    {
                        if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo) p.cosmetics.nameText.text += Helpers.cs(Color.gray, " ♥");
                        if (CachedPlayer.LocalPlayer.PlayerControl == p)
                        {
                            Akujo.akujo.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                            p.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                        }
                    }
                }
                if (Akujo.honmei != null)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo) Akujo.honmei.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                    if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.honmei)
                    {
                        Akujo.akujo.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                        Akujo.honmei.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                    }
                }

                if (MeetingHud.Instance != null)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (player.TargetPlayerId == Akujo.akujo.PlayerId && ((Akujo.honmei != null && Akujo.honmei == CachedPlayer.LocalPlayer.PlayerControl) || (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId))))
                            player.NameText.text += Helpers.cs(Akujo.color, " ♥");
                        if (CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo)
                        {
                            if (player.TargetPlayerId == Akujo.honmei?.PlayerId) player.NameText.text += Helpers.cs(Akujo.color, " ♥");
                            if (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == player.TargetPlayerId)) player.NameText.text += Helpers.cs(Color.gray, " ♥");
                        }
                    }
                }
            }

            // Lawyer or Prosecutor
            bool localIsLawyer = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.lawyer == CachedPlayer.LocalPlayer.PlayerControl;
            bool localIsKnowingTarget = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == CachedPlayer.LocalPlayer.PlayerControl;

            if (localIsLawyer || (localIsKnowingTarget && !Lawyer.lawyer.Data.IsDead)) {
                Color color = Lawyer.color;
                PlayerControl target = Lawyer.target;
                string suffix = Helpers.cs(color, " §");
                target.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == target.PlayerId)
                            player.NameText.text += suffix;
            }

            // Former Thief
            if (Thief.formerThief != null && (Thief.formerThief == CachedPlayer.LocalPlayer.PlayerControl || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)) {
                string suffix = Helpers.cs(Thief.color, " $");
                Thief.formerThief.cosmetics.nameText.text += suffix;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == Thief.formerThief.PlayerId)
                            player.NameText.text += suffix;
            }

            // Display lighter / darker color for all alive players
            if (CachedPlayer.LocalPlayer != null && MeetingHud.Instance != null && TORMapOptions.showLighterDarker) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                    var target = Helpers.playerById(player.TargetPlayerId);
                    if (target != null)  player.NameText.text += $" ({(Helpers.isLighterColor(target.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("detectiveLightLabel") : ModTranslation.getString("detectiveDarkLabel"))})";
                }
            }

            // Add medic shield info:
            if (MeetingHud.Instance != null && Medic.medic != null && Medic.shielded != null && Medic.shieldVisible(Medic.shielded))
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Medic.shielded.PlayerId)
                    {
                        player.NameText.text = Helpers.cs(Medic.color, "[") + player.NameText.text + Helpers.cs(Medic.color, "]");
                        // player.HighlightedFX.color = Medic.color;
                        // player.HighlightedFX.enabled = true;
                    }
            }
        }

        static void updateShielded() {
            if (Medic.shielded == null) return;

            if (Medic.shielded.Data.IsDead || Medic.medic == null || Medic.medic.Data.IsDead) {
                Medic.shielded = null;
            }
        }

        static void timerUpdate() {
            var dt = Time.deltaTime;
            Hacker.hackerTimer -= dt;
            Trickster.lightsOutTimer -= dt;
            Tracker.corpsesTrackingTimer -= dt;
            HideNSeek.timer -= dt;
            foreach (byte key in Deputy.handcuffedKnows.Keys)
                Deputy.handcuffedKnows[key] -= dt;
        }

        public static void miniUpdate() {
            //  || Mini.mini == MimicK.mimicK && MimicK.victim != null
            // the above line deleted in 2024.3.9, specified the MimicK instead
            if (Mini.mini == null || Camouflager.camouflageTimer > 0f || Helpers.MushroomSabotageActive() || (Mini.mini == MimicA.mimicA && MimicA.isMorph) || (Mini.mini == Morphling.morphling && Morphling.morphTimer > 0f) || (Mini.mini == Ninja.ninja && Ninja.stealthed)
                || (Mini.mini == Fox.fox && Fox.stealthed) || (Mini.mini == Sprinter.sprinter && Sprinter.sprinting) || (Mini.mini == Kataomoi.kataomoi && Kataomoi.isStalking()) || SurveillanceMinigamePatch.nightVisionIsActive) return;
                
            float growingProgress = Mini.growingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            string suffix = "";
            if (growingProgress != 1f)
                suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growingProgress * 18) + ")</color>"; 
            if (!Mini.isGrowingUpInMeeting && MeetingHud.Instance != null && Mini.ageOnMeetingStart != 0 && !(Mini.ageOnMeetingStart >= 18))
                suffix = " <color=#FAD934FF>(" + Mini.ageOnMeetingStart + ")</color>";

            if (!(Mini.mini == MimicK.mimicK && MimicK.victim != null)) Mini.mini.cosmetics.nameText.text += suffix;
            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Mini.mini.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
            }

            if (Morphling.morphling != null && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f)
                Morphling.morphling.cosmetics.nameText.text += suffix;
            if (MimicK.mimicK != null && MimicA.mimicA != null && MimicA.isMorph && MimicK.mimicK == Mini.mini)
                MimicA.mimicA.cosmetics.nameText.text += suffix;
        }

        static void updateImpostorKillButton(HudManager __instance) {
            if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor) return;
            if (MeetingHud.Instance) {
                __instance.KillButton.Hide();
                return;
            }

            /*if (Undertaker.undertaker != null && CachedPlayer.LocalPlayer.PlayerControl == Undertaker.undertaker && Undertaker.DraggedBody != null)
            {
                __instance.KillButton.graphic.color = Palette.DisabledClear;
                __instance.KillButton.buttonLabelText.color = Palette.DisabledClear;
                __instance.KillButton.cooldownTimerText.color = Palette.DisabledClear;
                __instance.KillButton.graphic.material.SetFloat(Shader.PropertyToID("_Desat"), 1f);
                return;
            }*/

            bool enabled = true;
            if (Vampire.vampire != null && Vampire.vampire == CachedPlayer.LocalPlayer.PlayerControl)
                enabled = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == CachedPlayer.LocalPlayer.PlayerControl && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                enabled = false;
            else if (Janitor.janitor != null && Janitor.janitor == CachedPlayer.LocalPlayer.PlayerControl)
                enabled = false;
            else if (MimicA.mimicA != null && MimicA.mimicA == CachedPlayer.LocalPlayer.PlayerControl && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead)
                enabled = false;
            else if (BomberA.bomberA != null && BomberA.bomberA == CachedPlayer.LocalPlayer.PlayerControl && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead)
                enabled = false;
            else if (BomberB.bomberB != null && BomberB.bomberB == CachedPlayer.LocalPlayer.PlayerControl && BomberA.bomberA != null && !BomberA.bomberA.Data.IsDead)
                enabled = false;

            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();

            if (Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
        }

        static void updateReportButton(HudManager __instance) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0 || MeetingHud.Instance) __instance.ReportButton.Hide();
            else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
        }
         
        static void updateVentButton(HudManager __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if ((Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] > 0) || MeetingHud.Instance || CachedPlayer.LocalPlayer.PlayerControl.roleCanUseVents() == false) __instance.ImpostorVentButton.Hide();
            else if (CachedPlayer.LocalPlayer.PlayerControl.roleCanUseVents() && !__instance.ImpostorVentButton.isActiveAndEnabled) __instance.ImpostorVentButton.Show();

            if (Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId)) {
                if (CachedPlayer.LocalPlayer.PlayerControl != Engineer.engineer) __instance.ImpostorVentButton.transform.localPosition = __instance.UseButton.transform.localPosition + CustomButton.ButtonPositions.upperRowLeft;
                else if (Madmate.canVent && CachedPlayer.LocalPlayer.PlayerControl == Engineer.engineer) __instance.ImpostorVentButton.transform.localPosition = __instance.UseButton.transform.localPosition + CustomButton.ButtonPositions.lowerRowRight;
            }
            if (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == CachedPlayer.LocalPlayer.PlayerControl && CreatedMadmate.canEnterVents) __instance.ImpostorVentButton.transform.localPosition = __instance.UseButton.transform.localPosition + CustomButton.ButtonPositions.lowerRowRight;
        }

        static void updateUseButton(HudManager __instance) {
            if (MeetingHud.Instance) __instance.UseButton.Hide();
        }

        static void updateSabotageButton(HudManager __instance) {
            if (MeetingHud.Instance || TORMapOptions.gameMode == CustomGamemodes.HideNSeek || !CachedPlayer.LocalPlayer.PlayerControl.roleCanUseSabotage()) __instance.SabotageButton.Hide();
            else if (CachedPlayer.LocalPlayer.PlayerControl.roleCanUseSabotage() && !__instance.SabotageButton.isActiveAndEnabled) __instance.SabotageButton.Show();

            if (Helpers.ShowButtons) {
                if ((CreatedMadmate.createdMadmate != null && CachedPlayer.LocalPlayer.PlayerControl == CreatedMadmate.createdMadmate && CreatedMadmate.canSabotage)
                    || (Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) && Madmate.canSabotage))
                    __instance.SabotageButton.transform.localPosition = CustomButton.ButtonPositions.upperRowCenter + __instance.UseButton.transform.localPosition;
            }
        }

        static void updateMapButton(HudManager __instance) {
            //Trapper.trapper == null || !(CachedPlayer.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) ||
            if ( __instance == null || __instance.MapButton.HeldButtonSprite == null) return;
            //__instance.MapButton.HeldButtonSprite.color = Trapper.playersOnMap.Any() ? Trapper.color : Color.white;
        }

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            
            EventUtility.Update();

            CustomButton.HudUpdate();
            resetNameTagsAndColors();
            setNameColors();
            updateShielded();
            setNameTags();

            // Impostors
            updateImpostorKillButton(__instance);
            // Timer updates
            timerUpdate();
            // Mini
            miniUpdate();

            // Deputy Sabotage, Use and Vent Button Disabling
            updateReportButton(__instance);
            updateVentButton(__instance);
            // Meeting hide buttons if needed (used for the map usage, because closing the map would show buttons)
            updateSabotageButton(__instance);
            updateUseButton(__instance);
            updateMapButton(__instance);
            if (!MeetingHud.Instance) __instance.AbilityButton?.Update();
            GameStatistics.updateTimer();

            // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
            foreach (PlayerControl target in CachedPlayer.AllPlayers) {
                var pet = target.GetPet();
                if (pet != null) {
                    pet.Visible = (PlayerControl.LocalPlayer.Data.IsDead && target.Data.IsDead || !target.Data.IsDead) && !target.inVent;
                }
            }
        }
    }
}
