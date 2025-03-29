using HarmonyLib;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using AmongUs.GameOptions;
using TheOtherRoles.Modules;
using static TheOtherRoles.GameHistory;
using Hazel;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        private static Dictionary<byte, (string name, Color color)> TagColorDict = new();
        static void resetNameTagsAndColors() {
            var localPlayer = PlayerControl.LocalPlayer;
            var myData = PlayerControl.LocalPlayer.Data;
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
            var localPlayer = PlayerControl.LocalPlayer;
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
                setPlayerNameColor(PlayerControl.LocalPlayer, Madmate.color);
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
                setPlayerNameColor(PlayerControl.LocalPlayer, Madmate.color);
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
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
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
            if (Lovers.lover1 != null && Lovers.lover2 != null && (Lovers.lover1 == PlayerControl.LocalPlayer || Lovers.lover2 == PlayerControl.LocalPlayer)) {
                string suffix = Helpers.cs(Lovers.color, " ♥");
                Lovers.lover1.cosmetics.nameText.text += suffix;
                Lovers.lover2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Lovers.lover1.PlayerId == player.TargetPlayerId || Lovers.lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Cupid
            if (Cupid.lovers1 != null && Cupid.lovers2 != null && (Cupid.lovers1 == PlayerControl.LocalPlayer || Cupid.lovers2 == PlayerControl.LocalPlayer || (Cupid.cupid != null && PlayerControl.LocalPlayer == Cupid.cupid)))
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
            if (Kataomoi.kataomoi != null && Kataomoi.kataomoi == PlayerControl.LocalPlayer && Kataomoi.target != null)
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
                        if (PlayerControl.LocalPlayer == Akujo.akujo) p.cosmetics.nameText.text += Helpers.cs(Color.gray, " ♥");
                        if (PlayerControl.LocalPlayer == p)
                        {
                            Akujo.akujo.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                            p.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                        }
                    }
                }
                if (Akujo.honmei != null)
                {
                    if (PlayerControl.LocalPlayer == Akujo.akujo) Akujo.honmei.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                    if (PlayerControl.LocalPlayer == Akujo.honmei)
                    {
                        Akujo.akujo.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                        Akujo.honmei.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                    }
                }

                if (MeetingHud.Instance != null)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (player.TargetPlayerId == Akujo.akujo.PlayerId && ((Akujo.honmei != null && Akujo.honmei == PlayerControl.LocalPlayer) || (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))))
                            player.NameText.text += Helpers.cs(Akujo.color, " ♥");
                        if (PlayerControl.LocalPlayer == Akujo.akujo)
                        {
                            if (player.TargetPlayerId == Akujo.honmei?.PlayerId) player.NameText.text += Helpers.cs(Akujo.color, " ♥");
                            if (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == player.TargetPlayerId)) player.NameText.text += Helpers.cs(Color.gray, " ♥");
                        }
                    }
                }
            }

            // Lawyer or Prosecutor
            bool localIsLawyer = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.lawyer == PlayerControl.LocalPlayer;
            bool localIsKnowingTarget = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == PlayerControl.LocalPlayer;

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
            if (Thief.formerThief != null && (Thief.formerThief == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)) {
                string suffix = Helpers.cs(Thief.color, " $");
                Thief.formerThief.cosmetics.nameText.text += suffix;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == Thief.formerThief.PlayerId)
                            player.NameText.text += suffix;
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

        public static void updatePlayerInfo() {
            bool commsActive = false;
            foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
            {
                if (t.TaskType == TaskTypes.FixComms)
                {
                    commsActive = true;
                    break;
                }
            }

            Vector3 colorBlindTextMeetingInitialLocalPos = new(0.3384f, -0.16666f, -0.01f);
            Vector3 colorBlindTextMeetingInitialLocalScale = new(0.9f, 1f, 1f);
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                
                // Colorblind Text in Meeting
                PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active) {
                    playerVoteArea.ColorBlindName.transform.localPosition = colorBlindTextMeetingInitialLocalPos + new Vector3(0f, 0.4f, 0f);
                    playerVoteArea.ColorBlindName.transform.localScale = colorBlindTextMeetingInitialLocalScale * 0.8f;
                }

                // Colorblind Text During the round
                if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active) {
                    p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1f, 0f);
                }

                p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);  // This moves both the name AND the colorblindtext behind objects (if the player is behind the object), like the rock on polus

                if ((Lawyer.lawyerKnowsRole && PlayerControl.LocalPlayer == Lawyer.lawyer && p == Lawyer.target) || (Akujo.knowsRoles && PlayerControl.LocalPlayer == Akujo.akujo && (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))) || p == PlayerControl.LocalPlayer || (PlayerControl.LocalPlayer.Data.IsDead
                    && !(PlayerControl.LocalPlayer == Busker.busker && Busker.pseudocideFlag)) || FreePlayGM.isFreePlayGM) {
                    Transform playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo == null) {
                        playerInfo = UnityEngine.Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                        playerInfo.transform.localPosition += Vector3.up * 0.225f;
                        playerInfo.fontSize *= 0.75f;
                        playerInfo.gameObject.name = "Info";
                        playerInfo.color = playerInfo.color.SetAlpha(1f);
                    }
    
                    Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                    TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (meetingInfo == null && playerVoteArea != null) {
                        meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                        meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                        meetingInfo.fontSize *= 0.60f;
                        meetingInfo.gameObject.name = "Info";
                    }

                    // Set player name higher to align in middle
                    if (meetingInfo != null && playerVoteArea != null) {
                        var playerName = playerVoteArea.NameText;
                        playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                    }

                    bool isTaskMasterExTask = TaskMaster.isTaskMaster(p.PlayerId) && TaskMaster.isTaskComplete;
                    var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                    var (exTasksCompleted, exTasksTotal) = TasksHandler.taskInfo(p.Data, true);
                    string roleNames = RoleInfo.GetRolesString(p, true, false);
                    string roleText = RoleInfo.GetRolesString(p, true, TORMapOptions.ghostsSeeModifier, false, true);
                    string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({(commsActive ? "?" : tasksCompleted)}/{tasksTotal})</color>" : "";
                    string exTaskInfo = exTasksTotal > 0 ? $"<color=#E1564BFF>({exTasksCompleted}/{exTasksTotal})</color>" : "";

                    string playerInfoText = "";
                    string meetingInfoText = "";                    
                    if (p == PlayerControl.LocalPlayer) {
                        if (p.Data.IsDead) roleNames = roleText;
                        playerInfoText = $"{roleNames}";
                        if (p == Swapper.swapper) playerInfoText = $"{roleNames}" + Helpers.cs((p.Data.Role.IsImpostor || Madmate.madmate.Any(x => x.PlayerId == p.PlayerId)) ? Palette.ImpostorRed : Swapper.color, $" ({Swapper.charges})");
                        if (HudManager.Instance.TaskPanel != null) {
                            TMPro.TextMeshPro tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                            //tabText.SetText($"Tasks {taskInfo}");
                            tabText.SetText(String.Format("{0} {1}", isTaskMasterExTask ? ModTranslation.getString("taskMasterExTasks") : TranslationController.Instance.GetString(StringNames.Tasks), isTaskMasterExTask ? exTaskInfo : taskInfo));
                        }
                        //meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                        if (!isTaskMasterExTask)
                            meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                        else
                            meetingInfoText = $"{roleNames} {exTaskInfo}".Trim();
                    }
                    else if (!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer == Akujo.akujo && (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId)) && Akujo.knowsRoles) {
                        playerInfoText = roleText;
                        meetingInfoText = roleText;
                    }
                    else if (TORMapOptions.ghostsSeeRoles && TORMapOptions.ghostsSeeInformation) {
                        if (!isTaskMasterExTask)
                            playerInfoText = $"{roleText} {taskInfo}".Trim();
                        else
                            playerInfoText = $"{roleText} {exTaskInfo}".Trim();
                        //playerInfoText = $"{roleText} {taskInfo}".Trim();
                        meetingInfoText = playerInfoText;
                    }
                    else if (TORMapOptions.ghostsSeeInformation) {
                        playerInfoText = $"{taskInfo}".Trim();
                        meetingInfoText = playerInfoText;
                    }
                    else if (TORMapOptions.ghostsSeeRoles || (Lawyer.lawyerKnowsRole && PlayerControl.LocalPlayer == Lawyer.lawyer && p == Lawyer.target)) {
                        playerInfoText = $"{roleText}";
                        meetingInfoText = playerInfoText;
                    }

                    playerInfo.text = playerInfoText;
                    playerInfo.gameObject.SetActive(p.Visible);
                    if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
                }                
            }
        }

        public static void moriartyUpdate()
        {
            if (Moriarty.moriarty == null || PlayerControl.LocalPlayer != Moriarty.moriarty) return;
            Moriarty.arrowUpdate();
        }

        static void assassinUpdate()
        {
            if (Assassin.arrow?.arrow != null)
            {
                if (Assassin.assassin == null || Assassin.assassin != PlayerControl.LocalPlayer || !Assassin.knowsTargetLocation)
                {
                    Assassin.arrow.arrow.SetActive(false);
                    return;
                }
                if (Assassin.assassinMarked != null && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    bool trackedOnMap = !Assassin.assassinMarked.Data.IsDead;
                    Vector3 position = Assassin.assassinMarked.transform.position;
                    if (!trackedOnMap)
                    { // Check for dead body
                        DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Assassin.assassinMarked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }
                    Assassin.arrow.Update(position);
                    Assassin.arrow.arrow.SetActive(trackedOnMap);
                }
                else
                {
                    Assassin.arrow.arrow.SetActive(false);
                }
            }
        }

        static void detectiveUpdateFootPrints()
        {
            if (Detective.detective == null || Detective.detective != PlayerControl.LocalPlayer) return;

            Detective.timer -= Time.deltaTime;
            if (Detective.timer <= 0f) {
                Detective.timer = Detective.footprintIntervall;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                    if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent && !((player == Ninja.ninja && Ninja.stealthed) || (player == Sprinter.sprinter && Sprinter.sprinting)
                        || (player == Fox.fox && Fox.stealthed) || (player == Kataomoi.kataomoi && Kataomoi.isStalking()))) {
                        FootprintHolder.Instance.MakeFootprint(player);
                    }
                }
            }
        }
        
        static void trackerUpdate() {
            // Handle player tracking
            if (Tracker.arrow?.arrow != null) {
                if (Tracker.tracker == null || PlayerControl.LocalPlayer != Tracker.tracker) {
                    Tracker.arrow.arrow.SetActive(false);
                    if (Tracker.DangerMeterParent) Tracker.DangerMeterParent.SetActive(false);
                    return;
                }

                if (Tracker.tracked != null && !Tracker.tracker.Data.IsDead) {
                    Tracker.timeUntilUpdate -= Time.deltaTime;

                    if (Tracker.timeUntilUpdate <= 0f) {
                        bool trackedOnMap = !Tracker.tracked.Data.IsDead;
                        Vector3 position = Tracker.tracked.transform.position;
                        if (!trackedOnMap) { // Check for dead body
                            DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Tracker.tracked.PlayerId);
                            if (body != null) {
                                trackedOnMap = true;
                                position = body.transform.position;
                            }
                        }

                        if (Tracker.trackingMode is 1 or 2) Arrow.UpdateProximity(position);
                        if (Tracker.trackingMode is 0 or 2)
                        {
                            Tracker.arrow.Update(position);
                            Tracker.arrow.arrow.SetActive(trackedOnMap);
                        }
                        Tracker.timeUntilUpdate = Tracker.updateIntervall;
                    } else {
                        if (Tracker.trackingMode is 0 or 2)
                            Tracker.arrow.Update();
                    }

                    if (Tracker.tracked.inVent && !Tracker.acTokenChallenge.Value.inVent)
                    {
                        Tracker.acTokenChallenge.Value.inVent = true;
                        Tracker.acTokenChallenge.Value.ventTime = GameStatistics.currentTime;
                    }
                    else if (!Tracker.tracked.inVent && Tracker.acTokenChallenge.Value.inVent)
                    {
                        Tracker.acTokenChallenge.Value.inVent = false;
                    }
                }
                else if (Tracker.tracker.Data.IsDead)
                {
                    if (Tracker.DangerMeterParent != null) Tracker.DangerMeterParent?.SetActive(false);
                    if (Tracker.Meter?.gameObject != null) Tracker.Meter?.gameObject?.SetActive(false);
                    if (Tracker.arrow?.arrow != null) Tracker.arrow.arrow.SetActive(false);
                }
            }

            // Handle corpses tracking
            if (Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer && Tracker.corpsesTrackingTimer >= 0f && !Tracker.tracker.Data.IsDead) {
                bool arrowsCountChanged = Tracker.localArrows.Count != Tracker.deadBodyPositions.Count;
                int index = 0;

                if (arrowsCountChanged) {
                    foreach (Arrow arrow in Tracker.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                    Tracker.localArrows = new List<Arrow>();
                }
                foreach (Vector3 position in Tracker.deadBodyPositions) {
                    if (arrowsCountChanged) {
                        Tracker.localArrows.Add(new Arrow(Tracker.color));
                        Tracker.localArrows[index].arrow.SetActive(true);
                    }
                    if (Tracker.localArrows[index] != null) Tracker.localArrows[index].Update(position);
                    index++;
                }
            } else if (Tracker.localArrows.Count > 0) { 
                foreach (Arrow arrow in Tracker.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Tracker.localArrows = new List<Arrow>();
            }
        }

        static void snitchUpdate() {
            if (Snitch.localArrows == null) return;

            foreach (Arrow arrow in Snitch.localArrows) arrow.arrow.SetActive(false);

            if (Snitch.snitch == null || Snitch.snitch.Data.IsDead) return;
            var local = PlayerControl.LocalPlayer;

            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
            int numberOfTasks = playerTotal - playerCompleted;

            if (numberOfTasks <= Snitch.taskCountForReveal && !local.Data.IsDead && (local.Data.Role.IsImpostor || (Snitch.includeTeamEvil && (local == Jackal.jackal || local == Sidekick.sidekick
                || local == Moriarty.moriarty || local == JekyllAndHyde.jekyllAndHyde || local == Fox.fox || local == Immoralist.immoralist
                || (local == SchrodingersCat.schrodingersCat && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate)))))
            {
                if (Snitch.localArrows.Count == 0) Snitch.localArrows.Add(new Arrow(Color.blue));
                if (Snitch.localArrows.Count != 0 && Snitch.localArrows[0] != null)
                {
                    Snitch.localArrows[0].arrow.SetActive(true);
                    Snitch.localArrows[0].Update(Snitch.snitch.transform.position);
                }
            }
            else if (local == Snitch.snitch && numberOfTasks == 0)
            {
                bool includeEvil = Snitch.includeTeamEvil;
                int arrowIndex = 0;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    bool arrowForImp = p.Data.Role.IsImpostor;
                    bool arrowForTeamJackal = includeEvil && (p == Jackal.jackal || p == Sidekick.sidekick || (p == SchrodingersCat.schrodingersCat && SchrodingersCat.team == SchrodingersCat.Team.Jackal));
                    bool arrowForFox = includeEvil && (p == Fox.fox || p == Immoralist.immoralist);
                    bool arrowForJekyll = includeEvil && (p == JekyllAndHyde.jekyllAndHyde || (p == SchrodingersCat.schrodingersCat && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde));
                    bool arrowForMoriarty = includeEvil && (p == Moriarty.moriarty || (p == SchrodingersCat.schrodingersCat && SchrodingersCat.team == SchrodingersCat.Team.Moriarty));

                    Color color = Palette.ImpostorRed;
                    if (Snitch.teamEvilUseDifferentArrowColor)
                    {
                        if (arrowForTeamJackal) color = Jackal.color;
                        else if (arrowForFox) color = Fox.color;
                        else if (arrowForJekyll) color = JekyllAndHyde.color;
                        else if (arrowForMoriarty) color = Moriarty.color;
                    }

                    if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal || arrowForFox || arrowForJekyll || arrowForMoriarty))
                    {
                        if (arrowIndex >= Snitch.localArrows.Count)
                        {
                            Snitch.localArrows.Add(new Arrow(color));
                        }
                        if (arrowIndex < Snitch.localArrows.Count && Snitch.localArrows[arrowIndex] != null)
                        {
                            Snitch.localArrows[arrowIndex].arrow.SetActive(true);
                            Snitch.localArrows[arrowIndex].Update(p.transform.position, color);
                        }
                        arrowIndex++;
                    }
                }
            }
        }

        static void prophetUpdate()
        {
            if (Prophet.arrows == null) return;

            foreach (var arrow in Prophet.arrows) arrow.arrow.SetActive(false);

            if (Prophet.prophet == null || Prophet.prophet.Data.IsDead) return;

            if (Prophet.isRevealed && Helpers.isKiller(PlayerControl.LocalPlayer) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (Prophet.arrows.Count == 0) Prophet.arrows.Add(new Arrow(Prophet.color));
                if (Prophet.arrows.Count != 0 && Prophet.arrows[0] != null)
                {
                    Prophet.arrows[0].arrow.SetActive(true);
                    Prophet.arrows[0].Update(Prophet.prophet.transform.position);
                }
            }
        }

        public static void bomberAUpdate()
        {
            if (PlayerControl.LocalPlayer == BomberA.bomberA)
            {
                BomberA.arrowUpdate();
                BomberA.playerIconsUpdate();
            }
        }

        public static void bomberBUpdate()
        {
            if (PlayerControl.LocalPlayer == BomberB.bomberB)
            {
                BomberB.arrowUpdate();
                BomberB.playerIconsUpdate();
            }
        }

        public static void impostorArrowUpdate()
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (FortuneTeller.arrows != null && FortuneTeller.arrows.Count > 0)
                {
                    foreach (var arrow in FortuneTeller.arrows)
                    {
                        if (arrow != null && arrow?.arrow != null) arrow.arrow.SetActive(false);
                    }
                }
                return;
            }
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                // 前フレームからの経過時間をマイナスする
                FortuneTeller.updateTimer -= Time.deltaTime;

                // 1秒経過したらArrowを更新
                if (FortuneTeller.updateTimer <= 0.0f)
                {
                    // 前回のArrowをすべて破棄する
                    foreach (Arrow arrow1 in FortuneTeller.arrows)
                    {
                        if (arrow1?.arrow != null)
                        {
                            arrow1.arrow.SetActive(false);
                            UnityEngine.Object.Destroy(arrow1.arrow);
                        }
                    }

                    // Arrow一覧
                    FortuneTeller.arrows = new List<Arrow>();

                    if (FortuneTeller.fortuneTeller == null || !FortuneTeller.divinedFlag || FortuneTeller.fortuneTeller.Data.IsDead) return;

                    Arrow arrow = new(FortuneTeller.color);
                    arrow.arrow.SetActive(true);
                    arrow.Update(FortuneTeller.fortuneTeller.transform.position);
                    FortuneTeller.arrows.Add(arrow);

                    // タイマーに時間をセット
                    FortuneTeller.updateTimer = 1f;
                }
                else
                {
                    FortuneTeller.arrows.Do(x => x.Update());
                }
            }
        }

        static void engineerUpdate() {
            bool jackalHighlight = Engineer.highlightForTeamJackal && (PlayerControl.LocalPlayer == Jackal.jackal || PlayerControl.LocalPlayer == Sidekick.sidekick);
            bool impostorHighlight = Engineer.highlightForImpostors && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            if ((jackalHighlight || impostorHighlight) && MapUtilities.CachedShipStatus?.AllVents != null) {
                foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents) {
                    try {
                        if (vent?.myRend?.material != null) {
                            if (Engineer.engineer != null && Engineer.engineer.inVent) {
                                vent.myRend.material.SetFloat("_Outline", 1f);
                                vent.myRend.material.SetColor("_OutlineColor", Engineer.color);
                            }
                            else if (vent.myRend.material.GetColor("_AddColor") != Color.red) {
                                vent.myRend.material.SetFloat("_Outline", 0);
                            }
                        }
                    }
                    catch { }
                }
            }

            if (Engineer.engineer != null && PlayerControl.LocalPlayer == Engineer.engineer && MapUtilities.CachedShipStatus?.AllVents != null && Engineer.acTokenChallenge != null)
            {
                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    if (PlayerControl.LocalPlayer.inVent && !Engineer.acTokenChallenge.Value.inVent)
                        Engineer.acTokenChallenge.Value.inVent = true;
                    else if (!PlayerControl.LocalPlayer.inVent && Engineer.acTokenChallenge.Value.inVent)
                        Engineer.acTokenChallenge.Value.inVent = false;
                }
                else
                {
                    if (Engineer.acTokenChallenge.Value.inVent && !Engineer.acTokenChallenge.Value.cleared)
                        Engineer.acTokenChallenge.Value.cleared = true;
                }
            }
        }

        static void vultureUpdate() {
            if (Vulture.vulture == null || PlayerControl.LocalPlayer != Vulture.vulture || Vulture.localArrows == null || !Vulture.showArrows) return;
            if (Vulture.vulture.Data.IsDead) {
                foreach (Arrow arrow in Vulture.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Vulture.localArrows = new List<Arrow>();
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = Vulture.localArrows.Count != deadBodies.Count();
            int index = 0;

            if (arrowUpdate) {
                foreach (Arrow arrow in Vulture.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Vulture.localArrows = new List<Arrow>();
            }

            foreach (DeadBody db in deadBodies) {
                if (arrowUpdate) {
                    Vulture.localArrows.Add(new Arrow(Color.blue));
                    Vulture.localArrows[index].arrow.SetActive(true);
                }
                if (Vulture.localArrows[index] != null) Vulture.localArrows[index].Update(db.transform.position);
                index++;
            }
        }

        public static void fortuneTellerUpdate()
        {
            if (FortuneTeller.fortuneTeller == null) return;
            if (FortuneTeller.fortuneTeller == PlayerControl.LocalPlayer && !FortuneTeller.meetingFlag)
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (!FortuneTeller.progress.ContainsKey(p.PlayerId)) FortuneTeller.progress[p.PlayerId] = 0f;
                    if (p.Data.IsDead) continue;
                    var fortuneTeller = PlayerControl.LocalPlayer;
                    float distance = Vector3.Distance(p.transform.position, fortuneTeller.transform.position);
                    // 障害物判定
                    bool anythingBetween = PhysicsHelpers.AnythingBetween(p.GetTruePosition(), fortuneTeller.GetTruePosition(), Constants.ShipAndObjectsMask, false);
                    if (!anythingBetween && distance <= FortuneTeller.distance && FortuneTeller.progress[p.PlayerId] < FortuneTeller.duration)
                    {
                        FortuneTeller.progress[p.PlayerId] += Time.deltaTime;
                    }
                }
            }
        }

        static void schrodingersCatUpdate()
        {
            if (SchrodingersCat.schrodingersCat == null || PlayerControl.LocalPlayer != SchrodingersCat.schrodingersCat) return;
            if (SchrodingersCat.schrodingersCat.Data.IsDead || SchrodingersCat.hasTeam() || MeetingHud.Instance || ExileController.Instance) {
                if (SchrodingersCat.shownMenu) SchrodingersCat.showMenu();
                return;
            }
        }

        static void archaeologistUpdate()
        {
            if (Archaeologist.arrow?.arrow != null)
            {
                if (Archaeologist.archaeologist == null || PlayerControl.LocalPlayer != Archaeologist.archaeologist)
                {
                    Archaeologist.arrow.arrow.SetActive(false);
                    return;
                }
                if (Antique.antiques != null && Antique.antiques.Any(x => x.isDetected) && !PlayerControl.LocalPlayer.Data.IsDead && Archaeologist.arrowActive)
                {
                    var antique = Antique.antiques.FirstOrDefault(x => x.isDetected);
                    Archaeologist.arrow.Update(ShipStatus.Instance.FastRooms[antique.room].roomArea.ClosestPoint(PlayerControl.LocalPlayer.transform.position));
                    Archaeologist.arrow.arrow.SetActive(true);
                }
                else
                {
                    Archaeologist.arrow.arrow.SetActive(false);
                }
            }

            if (PlayerControl.LocalPlayer == Archaeologist.archaeologist && !PlayerControl.LocalPlayer.Data.IsDead && Antique.antiques != null && Antique.antiques.Any(x => x.isDetected))
            {
                var antique = Antique.antiques.FirstOrDefault(x => x.isDetected);
                if (ShipStatus.Instance.FastRooms[antique.room].roomArea.OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition())) {
                    antique.gameObject.SetActive(true);
                    Archaeologist.revealed.Add(antique);
                }
            }
        }

        static void foxUpdate()
        {
            if (Fox.fox == null || PlayerControl.LocalPlayer != Fox.fox) return;
            Fox.arrowUpdate();
        }

        static void immoralistUpdate()
        {
            if (Immoralist.immoralist == null || PlayerControl.LocalPlayer != Immoralist.immoralist) return;
            Immoralist.arrowUpdate();
        }

        static void kataomoiUpdate()
        {
            if (Kataomoi.kataomoi != PlayerControl.LocalPlayer || Kataomoi.target == null) return;
            TORMapOptions.resetPoolables();
            if (Kataomoi.kataomoi.Data.IsDead)
            {
                if (Kataomoi.arrow != null) UnityEngine.Object.Destroy(Kataomoi.arrow.arrow);
                Kataomoi.arrow = null;
                if (Kataomoi.stareText != null && Kataomoi.stareText.gameObject != null) UnityEngine.Object.Destroy(Kataomoi.stareText.gameObject);
                Kataomoi.stareText = null;
                for (int i = 0; i < Kataomoi.gaugeRenderer.Length; ++i) {
                    if (Kataomoi.gaugeRenderer[i] != null)
                    {
                        UnityEngine.Object.Destroy(Kataomoi.gaugeRenderer[i].gameObject);
                        Kataomoi.gaugeRenderer[i] = null;
                    }
                }
                return;
            }

            // Update Stare Count Text
            if (Kataomoi.stareText != null)
            {
                Kataomoi.stareText.gameObject.SetActive(!MeetingHud.Instance);
                if (Kataomoi.stareCount > 0)
                    Kataomoi.stareText.text = $"{Kataomoi.stareCount}";
                else
                    Kataomoi.stareText.text = "";
            }

            if (Kataomoi.target != null && TORMapOptions.playerIcons.ContainsKey(Kataomoi.target?.PlayerId ?? byte.MaxValue)) {
                TORMapOptions.playerIcons[Kataomoi.target.PlayerId].gameObject.SetActive(!MeetingHud.Instance);
            }
            for (int i = 0; i < Kataomoi.gaugeRenderer.Length; ++i)
            {
                if (Kataomoi.gaugeRenderer[i] != null)
                {
                    Kataomoi.gaugeRenderer[i].gameObject?.SetActive(!MeetingHud.Instance);
                }
            }

            // Update Arrow
            Kataomoi.arrow ??= new Arrow(Kataomoi.color);
            Kataomoi.arrow.arrow.SetActive(Kataomoi.isSearch);
            if (Kataomoi.isSearch && Kataomoi.target != null)
            {
                Kataomoi.arrow.Update(Kataomoi.target.transform.position);
            }
        }

        public static void securityGuardUpdate()
        {
            if (SecurityGuard.securityGuard == null || PlayerControl.LocalPlayer != SecurityGuard.securityGuard || SecurityGuard.securityGuard.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(SecurityGuard.securityGuard.Data);
            if (playerCompleted == SecurityGuard.rechargedTasks)
            {
                SecurityGuard.rechargedTasks += SecurityGuard.rechargeTasksNumber;
                if (SecurityGuard.maxCharges > SecurityGuard.charges) SecurityGuard.charges++;
            }
        }

        // For swapper swap charges        
        public static void swapperUpdate()
        {
            if (Swapper.swapper == null || PlayerControl.LocalPlayer != Swapper.swapper || PlayerControl.LocalPlayer.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
            if (playerCompleted == Swapper.rechargedTasks)
            {
                Swapper.rechargedTasks += Swapper.rechargeTasksNumber;
                Swapper.charges++;
                _ = new StaticAchievementToken("niceSwapper.common1");
            }
        }

        public static void hackerUpdate()
        {
            if (Hacker.hacker == null || PlayerControl.LocalPlayer != Hacker.hacker || Hacker.hacker.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(Hacker.hacker.Data);
            if (playerCompleted == Hacker.rechargedTasks)
            {
                Hacker.rechargedTasks += Hacker.rechargeTasksNumber;
                if (Hacker.toolsNumber > Hacker.chargesVitals) Hacker.chargesVitals++;
                if (Hacker.toolsNumber > Hacker.chargesAdminTable) Hacker.chargesAdminTable++;
            }
        }

        static void evilTrackerUpdate()
        {
            if (EvilTracker.evilTracker == null) return;
            if (PlayerControl.LocalPlayer == EvilTracker.evilTracker) EvilTracker.arrowUpdate();
        }

        public static void cupidUpdate()
        {
            if (HudManagerStartPatch.cupidTimeRemainingText != null && HudManagerStartPatch.cupidTimeRemainingText.isActiveAndEnabled)
                HudManagerStartPatch.cupidTimeRemainingText.enabled = false;

            if (Cupid.cupid == null || Cupid.cupid.Data.IsDead || PlayerControl.LocalPlayer != Cupid.cupid) return;
            Cupid.timeLeft = (int)Math.Ceiling(Cupid.timeLimit - (DateTime.UtcNow - Cupid.startTime).TotalSeconds);

            if (Cupid.timeLeft > 0)
            {
                if (Cupid.lovers1 == null || Cupid.lovers2 == null)
                {
                    if (HudManagerStartPatch.cupidTimeRemainingText != null) {
                        HudManagerStartPatch.cupidTimeRemainingText.text = TimeSpan.FromSeconds(Cupid.timeLeft).ToString(@"mm\:ss");
                    }
                    HudManagerStartPatch.cupidTimeRemainingText.enabled = Helpers.ShowButtons;
                }
                else HudManagerStartPatch.cupidTimeRemainingText.enabled = false;
            }
            else if (Cupid.timeLeft <= 0)
            {
                if (Cupid.lovers1 == null || Cupid.lovers2 == null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CupidSuicide, Hazel.SendOption.Reliable, -1);
                    writer.Write(Cupid.cupid.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.cupidSuicide(Cupid.cupid.PlayerId, false);
                }
            }
        }

        public static void akujoUpdate()
        {
            if (HudManagerStartPatch.akujoTimeRemainingText != null && HudManagerStartPatch.akujoTimeRemainingText.isActiveAndEnabled)
                HudManagerStartPatch.akujoTimeRemainingText.enabled = false;
            if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || PlayerControl.LocalPlayer != Akujo.akujo) return;
            Akujo.timeLeft = (int)Math.Ceiling(Akujo.timeLimit - (DateTime.UtcNow - Akujo.startTime).TotalSeconds);
            if (Akujo.timeLeft > 0)
            {
                if (Akujo.honmei == null)
                {
                    if (HudManagerStartPatch.akujoTimeRemainingText != null)
                    {
                        HudManagerStartPatch.akujoTimeRemainingText.text = TimeSpan.FromSeconds(Akujo.timeLeft).ToString(@"mm\:ss");
                    }
                    HudManagerStartPatch.akujoTimeRemainingText.enabled = Helpers.ShowButtons;
                }
                else HudManagerStartPatch.akujoTimeRemainingText.enabled = false;
            }
            else if (Akujo.timeLeft <= 0)
            {
                if (Akujo.honmei == null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AkujoSuicide, Hazel.SendOption.Reliable, -1);
                    writer.Write(Akujo.akujo.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.akujoSuicide(Akujo.akujo.PlayerId);
                }
            }
        }

        static void bountyHunterUpdate() {
            if (BountyHunter.bountyHunter == null || PlayerControl.LocalPlayer != BountyHunter.bountyHunter) return;

            if (BountyHunter.bountyHunter.Data.IsDead) {
                if (BountyHunter.arrow != null) UnityEngine.Object.Destroy(BountyHunter.arrow.arrow);
                BountyHunter.arrow = null;
                if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null) UnityEngine.Object.Destroy(BountyHunter.cooldownText.gameObject);
                BountyHunter.cooldownText = null;
                BountyHunter.bounty = null;
                TORMapOptions.resetPoolables();
                return;
            }

            BountyHunter.arrowUpdateTimer -= Time.deltaTime;
            BountyHunter.bountyUpdateTimer -= Time.deltaTime;

            if (BountyHunter.bounty == null || BountyHunter.bountyUpdateTimer <= 0f) {
                // Set new bounty
                BountyHunter.bounty = null;
                BountyHunter.arrowUpdateTimer = 0f; // Force arrow to update
                BountyHunter.bountyUpdateTimer = BountyHunter.bountyDuration;
                var possibleTargets = new List<PlayerControl>();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    if (!p.Data.IsDead && !p.Data.Disconnected && p != p.Data.Role.IsImpostor && p != Spy.spy && (p != Sidekick.sidekick || !Sidekick.wasTeamRed) && (p != Jackal.jackal || !Jackal.wasTeamRed) && (p != Mini.mini || Mini.isGrownUp()) &&
                        !BountyHunter.bountyHunter.GetAllRelatedPlayers().Contains(p) && !(p == Akujo.akujo && Akujo.keeps.Contains(BountyHunter.bountyHunter))) possibleTargets.Add(p);
                }
                if (possibleTargets.Count == 0) return;
                BountyHunter.bounty = possibleTargets[TheOtherRoles.rnd.Next(0, possibleTargets.Count)];
                if (BountyHunter.bounty == null) return;

                // Ghost Info
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.BountyTarget);
                writer.Write(BountyHunter.bounty.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null) {
                    TORMapOptions.resetPoolables();
                    if (TORMapOptions.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) && TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                        TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(true);
                }
            }

            // Hide in meeting
            if (MeetingHud.Instance && TORMapOptions.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) && TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(false);

            // Update Cooldown Text
            if (BountyHunter.cooldownText != null) {
                BountyHunter.cooldownText.text = Mathf.CeilToInt(Mathf.Clamp(BountyHunter.bountyUpdateTimer, 0, BountyHunter.bountyDuration)).ToString();
                BountyHunter.cooldownText.gameObject.SetActive(!MeetingHud.Instance);  // Show if not in meeting
            }

            // Update Arrow
            if (BountyHunter.showArrow && BountyHunter.bounty != null) {
                BountyHunter.arrow ??= new Arrow(Color.red);
                if (BountyHunter.arrowUpdateTimer <= 0f) {
                    BountyHunter.arrow.Update(BountyHunter.bounty.transform.position);
                    BountyHunter.arrowUpdateTimer = BountyHunter.arrowUpdateIntervall;
                }
                BountyHunter.arrow.Update();
            }
        }

        static void UndertakerCanDropTarget()
        {
            if (Undertaker.undertaker == null || Undertaker.undertaker != PlayerControl.LocalPlayer) return;
            var component = Undertaker.DraggedBody;

            Undertaker.CanDropBody = false;

            if (component == null) return;

            if (component.enabled && Vector2.Distance(Undertaker.undertaker.GetTruePosition(), component.TruePosition) <= Undertaker.undertaker.MaxReportDistance && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), component.TruePosition, Constants.ShipAndObjectsMask, false))
            {
                Undertaker.CanDropBody = true;
            }
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
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
            if (MeetingHud.Instance) {
                __instance.KillButton.Hide();
                return;
            }
            bool enabled = true;
            if (Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer)
                enabled = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                enabled = false;
            else if (Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer)
                enabled = false;
            else if (MimicA.mimicA != null && MimicA.mimicA == PlayerControl.LocalPlayer && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead)
                enabled = false;
            else if (BomberA.bomberA != null && BomberA.bomberA == PlayerControl.LocalPlayer && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead)
                enabled = false;
            else if (BomberB.bomberB != null && BomberB.bomberB == PlayerControl.LocalPlayer && BomberA.bomberA != null && !BomberA.bomberA.Data.IsDead)
                enabled = false;

            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();

            if (Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
        }

        static void updateReportButton(HudManager __instance) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0 || MeetingHud.Instance) __instance.ReportButton.Hide();
            else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
        }
         
        static void updateVentButton(HudManager __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if ((Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) || MeetingHud.Instance || PlayerControl.LocalPlayer.roleCanUseVents() == false) __instance.ImpostorVentButton.Hide();
            else if (PlayerControl.LocalPlayer.roleCanUseVents() && !__instance.ImpostorVentButton.isActiveAndEnabled) __instance.ImpostorVentButton.Show();

            if (Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
                __instance.ImpostorVentButton.transform.localPosition = PlayerControl.LocalPlayer == Engineer.engineer ? CustomButton.ButtonPositions.lowerRowRight : CustomButton.ButtonPositions.upperRowLeft;
            if (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == PlayerControl.LocalPlayer && CreatedMadmate.canEnterVents) __instance.ImpostorVentButton.transform.localPosition = CustomButton.ButtonPositions.lowerRowRight;

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(RewiredConsts.Action.UseVent) && !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.roleCanUseVents()) {
                __instance.ImpostorVentButton.DoClick();
            }
        }

        static void updateUseButton(HudManager __instance) {
            if (MeetingHud.Instance) __instance.UseButton.Hide();
        }

        static void updateSabotageButton(HudManager __instance) {
            if (MeetingHud.Instance || TORMapOptions.gameMode == CustomGamemodes.HideNSeek || !PlayerControl.LocalPlayer.roleCanUseSabotage()) __instance.SabotageButton.Hide();
            else if (PlayerControl.LocalPlayer.roleCanUseSabotage() && !__instance.SabotageButton.isActiveAndEnabled) __instance.SabotageButton.Show();

            if (Helpers.ShowButtons) {
                if ((CreatedMadmate.createdMadmate != null && PlayerControl.LocalPlayer == CreatedMadmate.createdMadmate && CreatedMadmate.canSabotage)
                    || (Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) && Madmate.canSabotage))
                    __instance.SabotageButton.transform.localPosition = CustomButton.ButtonPositions.upperRowCenter + __instance.UseButton.transform.localPosition;
            }
        }

        static void updateMapButton(HudManager __instance) {
            //Trapper.trapper == null || !(PlayerControl.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) ||
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

            // Update Player Info
            updatePlayerInfo();
            // Update Role Description
            Helpers.refreshRoleDescription(PlayerControl.LocalPlayer);
            // Tracker
            trackerUpdate();
            // Fortune Teller
            fortuneTellerUpdate();
            impostorArrowUpdate();  // If the Camouflager/Thief is having problem, please delete this line
            // Snitch
            snitchUpdate();
            // Detective
            detectiveUpdateFootPrints();
            // Bomber
            bomberAUpdate();
            bomberBUpdate();
            // Prophet
            prophetUpdate();
            // Moriarty
            moriartyUpdate();
            // Engineer
            engineerUpdate();
            // Evil Tracker
            evilTrackerUpdate();
            // Schrodinger's Cat
            schrodingersCatUpdate();
            // Undertaker
            UndertakerCanDropTarget();
            // Assassin
            assassinUpdate();
            // Kataomoi
            kataomoiUpdate();
            // Mimic(Killer)
            MimicK.arrowUpdate();
            // Mimic(Assistant)
            MimicA.arrowUpdate();
            // Akujo
            akujoUpdate();
            // Cupid
            cupidUpdate();
            // BountyHunter
            bountyHunterUpdate();
            // Fox
            foxUpdate();
            // Immorailst
            immoralistUpdate();
            // Archaeologist
            archaeologistUpdate();
            // Security Guard
            securityGuardUpdate();
            // Swapper
            swapperUpdate();
            // Hacker
            hackerUpdate();
            // Vulture
            vultureUpdate();

            // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
            foreach (PlayerControl target in PlayerControl.AllPlayerControls) {
                var pet = target.GetPet();
                if (pet != null) {
                    pet.Visible = ((PlayerControl.LocalPlayer.Data.IsDead && target.Data.IsDead) || !target.Data.IsDead) && !target.inVent;
                }
            }
        }
    }
}
