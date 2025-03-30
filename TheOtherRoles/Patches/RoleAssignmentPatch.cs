using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using AmongUs.GameOptions;
using TheOtherRoles.Utilities;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(RoleOptionsCollectionV08), nameof(RoleOptionsCollectionV08.GetNumPerGame))]
    class RoleOptionsDataGetNumPerGamePatch{
        public static void Postfix(ref int __result) {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) __result = 0; // Deactivate Vanilla Roles if the mod roles are active
        }
    }

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    class GameOptionsDataGetAdjustedNumImpostorsPatch {
        public static void Postfix(ref int __result) {
            if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek) {
                int impCount = Mathf.RoundToInt(CustomOptionHolder.hideNSeekHunterCount.getFloat());
                __result = impCount; ; // Set Imp Num
            }
            else if (TORMapOptions.gameMode == CustomGamemodes.FreePlay) {
                __result = 0; // No imps for freeplay
            }
            else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) {  // Ignore Vanilla impostor limits in TOR Games.
                __result = Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.NumImpostors, 1, 6);
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Validate))]
    class GameOptionsDataValidatePatch {
        public static void Postfix(GameOptionsData __instance) {
            if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek || GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.Normal) return;
            __instance.NumImpostors = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
        }
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    class RoleManagerSelectRolesPatch {
        private static int crewValues;
        private static int impValues;
        private static bool isEvilGuesser;
        private static bool isEvilWatcher;
        private static List<Tuple<byte, byte>> playerRoleMap = new();
        public static bool isGuesserGamemode { get { return TORMapOptions.gameMode == CustomGamemodes.Guesser; } }
        public static void Postfix() {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ResetVaribles, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.resetVariables();
            if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek || TORMapOptions.gameMode == CustomGamemodes.FreePlay || RoleDraft.isEnabled) return; // Don't assign Roles in Hide N Seek
            assignRoles();
            MessageWriter acWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ResetAchievement, SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(acWriter);
            RPCProcedure.resetAchievement();
        }

        private static void assignRoles() {
            var data = getRoleAssignmentData();
            assignSpecialRoles(data); // Assign special roles like mafia and lovers first as they assign a role to multiple players and the chances are independent of the ticket system
            selectFactionForFactionIndependentRoles(data);
            assignEnsuredRoles(data); // Assign roles that should always be in the game next
            assignDependentRoles(data); // Assign roles that may have a dependent role
            assignChanceRoles(data); // Assign roles that may or may not be in the game last
            assignRoleTargets(data); // Assign targets for Lawyer & Prosecutor
            if (isGuesserGamemode) assignGuesserGamemode();
            assignModifiers(); // Assign modifier
            //setRolesAgain();
        }

        public static RoleAssignmentData getRoleAssignmentData() {
            // Get the players that we want to assign the roles to. Crewmate and Neutral roles are assigned to natural crewmates. Impostor roles to impostors.
            List<PlayerControl> crewmates = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            crewmates.RemoveAll(x => x.Data.Role.IsImpostor);
            List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            impostors.RemoveAll(x => !x.Data.Role.IsImpostor);

            var crewmateMin = CustomOptionHolder.crewmateRolesCountMin.getSelection();
            var crewmateMax = CustomOptionHolder.crewmateRolesCountMax.getSelection();
            var neutralMin = CustomOptionHolder.neutralRolesCountMin.getSelection();
            var neutralMax = CustomOptionHolder.neutralRolesCountMax.getSelection();
            var impostorMin = CustomOptionHolder.impostorRolesCountMin.getSelection();
            var impostorMax = CustomOptionHolder.impostorRolesCountMax.getSelection();

            // Make sure min is less or equal to max
            if (crewmateMin > crewmateMax) crewmateMin = crewmateMax;
            if (neutralMin > neutralMax) neutralMin = neutralMax;
            if (impostorMin > impostorMax) impostorMin = impostorMax;

            // Automatically force everyone to get a role by setting crew Min / Max according to Neutral Settings
            if (CustomOptionHolder.crewmateRolesFill.getBool()) {
                crewmateMax = crewmates.Count - neutralMin;
                crewmateMin = crewmates.Count - neutralMax;
            }

            // Get the maximum allowed count of each role type based on the minimum and maximum option
            int crewCountSettings = rnd.Next(crewmateMin, crewmateMax + 1);
            int neutralCountSettings = rnd.Next(neutralMin, neutralMax + 1);
            int impCountSettings = rnd.Next(impostorMin, impostorMax + 1);
            // If fill crewmates is enabled, make sure crew + neutral >= crewmates s.t. everyone has a role!
            while (crewCountSettings + neutralCountSettings < crewmates.Count && CustomOptionHolder.crewmateRolesFill.getBool())
                crewCountSettings++;

            // Potentially lower the actual maximum to the assignable players
            int maxCrewmateRoles = Mathf.Min(crewmates.Count, crewCountSettings);
            int maxNeutralRoles = Mathf.Min(crewmates.Count, neutralCountSettings);
            int maxImpostorRoles = Mathf.Min(impostors.Count, impCountSettings);

            // Fill in the lists with the roles that should be assigned to players. Note that the special roles (like Mafia or Lovers) are NOT included in these lists
            Dictionary<byte, int> impSettings = new();
            Dictionary<byte, int> neutralSettings = new();
            Dictionary<byte, int> crewSettings = new();

            impSettings.Add((byte)RoleId.Morphling, CustomOptionHolder.morphlingSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Camouflager, CustomOptionHolder.camouflagerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Vampire, CustomOptionHolder.vampireSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Eraser, CustomOptionHolder.eraserSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Trickster, CustomOptionHolder.tricksterSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Cleaner, CustomOptionHolder.cleanerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Warlock, CustomOptionHolder.warlockSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Witch, CustomOptionHolder.witchSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.SerialKiller, CustomOptionHolder.serialKillerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Assassin, CustomOptionHolder.assassinSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Ninja, CustomOptionHolder.ninjaSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.NekoKabocha, CustomOptionHolder.nekoKabochaSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.EvilTracker, CustomOptionHolder.evilTrackerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Undertaker, CustomOptionHolder.undertakerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.EvilHacker, CustomOptionHolder.evilHackerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Trapper, CustomOptionHolder.trapperSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Blackmailer, CustomOptionHolder.blackmailerSpawnRate.getSelection());
            impSettings.Add((byte)RoleId.Yoyo, CustomOptionHolder.yoyoSpawnRate.getSelection());
            //impSettings.Add((byte)RoleId.Bomber, CustomOptionHolder.bomberSpawnRate.getSelection());

            neutralSettings.Add((byte)RoleId.Jester, CustomOptionHolder.jesterSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Arsonist, CustomOptionHolder.arsonistSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Jackal, CustomOptionHolder.jackalSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Opportunist, CustomOptionHolder.opportunistSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Vulture, CustomOptionHolder.vultureSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Thief, CustomOptionHolder.thiefSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Moriarty, CustomOptionHolder.moriartySpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Akujo, CustomOptionHolder.akujoSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.PlagueDoctor, CustomOptionHolder.plagueDoctorSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.JekyllAndHyde, CustomOptionHolder.jekyllAndHydeSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Cupid, CustomOptionHolder.cupidSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Fox, CustomOptionHolder.foxSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.SchrodingersCat, CustomOptionHolder.schrodingersCatSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Kataomoi, CustomOptionHolder.kataomoiSpawnRate.getSelection());
            neutralSettings.Add((byte)RoleId.Doomsayer, CustomOptionHolder.doomsayerSpawnRate.getSelection());

            /*if ((rnd.Next(1, 101) <= CustomOptionHolder.lawyerIsProsecutorChance.getSelection() * 10)) // Lawyer or Prosecutor
                neutralSettings.Add((byte)RoleId.Prosecutor, CustomOptionHolder.lawyerSpawnRate.getSelection());
            else
                neutralSettings.Add((byte)RoleId.Lawyer, CustomOptionHolder.lawyerSpawnRate.getSelection());*/
            neutralSettings.Add((byte)RoleId.Lawyer, CustomOptionHolder.lawyerSpawnRate.getSelection());

            crewSettings.Add((byte)RoleId.Mayor, CustomOptionHolder.mayorSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Portalmaker, CustomOptionHolder.portalmakerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Engineer, CustomOptionHolder.engineerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Lighter, CustomOptionHolder.lighterSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Detective, CustomOptionHolder.detectiveSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.TimeMaster, CustomOptionHolder.timeMasterSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Medic, CustomOptionHolder.medicSpawnRate.getSelection());
            //crewSettings.Add((byte)RoleId.Swapper,CustomOptionHolder.swapperSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Seer, CustomOptionHolder.seerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Hacker, CustomOptionHolder.hackerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Tracker, CustomOptionHolder.trackerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Snitch, CustomOptionHolder.snitchSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Medium, CustomOptionHolder.mediumSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Sprinter, CustomOptionHolder.sprinterSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.FortuneTeller, CustomOptionHolder.fortuneTellerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Bait, CustomOptionHolder.baitSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Veteran, CustomOptionHolder.veteranSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Sherlock, CustomOptionHolder.sherlockSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Noisemaker, CustomOptionHolder.noisemakerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.TaskMaster, CustomOptionHolder.taskMasterSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Teleporter, CustomOptionHolder.teleporterSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Prophet, CustomOptionHolder.prophetSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Busker, CustomOptionHolder.buskerSpawnRate.getSelection());
            crewSettings.Add((byte)RoleId.Archaeologist, CustomOptionHolder.archaeologistSpawnRate.getSelection());
            //crewSettings.Add((byte)RoleId.Shifter, CustomOptionHolder.shifterSpawnRate.getSelection());
            //crewSettings.Add((byte)RoleId.Trapper, CustomOptionHolder.trapperSpawnRate.getSelection());
            if (impostors.Count > 1) {
                // Only add Spy if more than 1 impostor as the spy role is otherwise useless
                crewSettings.Add((byte)RoleId.Spy, CustomOptionHolder.spySpawnRate.getSelection());
            }
            crewSettings.Add((byte)RoleId.SecurityGuard, CustomOptionHolder.securityGuardSpawnRate.getSelection());

            return new RoleAssignmentData {
                crewmates = crewmates,
                impostors = impostors,
                crewSettings = crewSettings,
                neutralSettings = neutralSettings,
                impSettings = impSettings,
                maxCrewmateRoles = maxCrewmateRoles,
                maxNeutralRoles = maxNeutralRoles,
                maxImpostorRoles = maxImpostorRoles
            };
        }

        private static void assignSpecialRoles(RoleAssignmentData data) {
            // Assign Mafia
            if (data.impostors.Count >= 3 && data.maxImpostorRoles >= 3 && (rnd.Next(1, 101) <= CustomOptionHolder.mafiaSpawnRate.getSelection() * 10)) {
                setRoleToRandomPlayer((byte)RoleId.Godfather, data.impostors);
                setRoleToRandomPlayer((byte)RoleId.Janitor, data.impostors);
                setRoleToRandomPlayer((byte)RoleId.Mafioso, data.impostors);
                data.maxImpostorRoles -= 3;
            }

            // Assign Bomber
            if (data.impostors.Count >= 2 && data.maxImpostorRoles >= 2 && (rnd.Next(1, 101) <= CustomOptionHolder.bomberSpawnRate.getSelection() * 10))
            {
                setRoleToRandomPlayer((byte)RoleId.BomberA, data.impostors);
                setRoleToRandomPlayer((byte)RoleId.BomberB, data.impostors);
                data.maxImpostorRoles -= 2;
            }

            // Assign Mimic
            if (data.impostors.Count >= 2 && data.maxImpostorRoles >= 2 && (rnd.Next(1, 101) <= CustomOptionHolder.mimicSpawnRate.getSelection() * 10))
            {
                setRoleToRandomPlayer((byte)RoleId.MimicK, data.impostors);
                setRoleToRandomPlayer((byte)RoleId.MimicA, data.impostors);
                data.maxImpostorRoles -= 2;
            }
        }

        private static void selectFactionForFactionIndependentRoles(RoleAssignmentData data) {
            if (!isGuesserGamemode) {
                // Assign Guesser (chance to be impostor based on setting)
                isEvilGuesser = rnd.Next(1, 101) <= CustomOptionHolder.guesserIsImpGuesserRate.getSelection() * 10;
                if ((CustomOptionHolder.guesserSpawnBothRate.getSelection() > 0 &&
                    CustomOptionHolder.guesserSpawnRate.getSelection() == 10) ||
                    CustomOptionHolder.guesserSpawnBothRate.getSelection() == 0) {
                    if (isEvilGuesser) data.impSettings.Add((byte)RoleId.EvilGuesser, CustomOptionHolder.guesserSpawnRate.getSelection());
                    else data.crewSettings.Add((byte)RoleId.NiceGuesser, CustomOptionHolder.guesserSpawnRate.getSelection());

                }
            }

            // Assign Swapper (chance to be impostor based on setting)
            if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && rnd.Next(1, 101) <= CustomOptionHolder.swapperIsImpRate.getSelection() * 10)
            {
                data.impSettings.Add((byte)RoleId.Swapper, CustomOptionHolder.swapperSpawnRate.getSelection());
            }
            else if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0)
            {
                data.crewSettings.Add((byte)RoleId.Swapper, CustomOptionHolder.swapperSpawnRate.getSelection());
            }

            // Assign Shifter (chance to be neutral based on setting)
            bool shifterIsNeutral = false;
            if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && rnd.Next(1, 101) <= CustomOptionHolder.shifterIsNeutralRate.getSelection() * 10)
            {
                data.neutralSettings.Add((byte)RoleId.Shifter, CustomOptionHolder.shifterSpawnRate.getSelection());
                shifterIsNeutral = true;
            }
            else if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0)
            {
                data.crewSettings.Add((byte)RoleId.Shifter, CustomOptionHolder.shifterSpawnRate.getSelection());
                shifterIsNeutral = false;
            }

            // Assign Watcher
            isEvilWatcher = rnd.Next(1, 101) <= CustomOptionHolder.watcherisImpWatcherRate.getSelection() * 10;
            if ((CustomOptionHolder.watcherSpawnBothRate.getSelection() > 0 && CustomOptionHolder.watcherSpawnRate.getSelection() == 10) ||
                CustomOptionHolder.watcherSpawnBothRate.getSelection() == 0)
            {
                if (isEvilWatcher) data.impSettings.Add((byte)RoleId.EvilWatcher, CustomOptionHolder.watcherSpawnRate.getSelection());
                else data.crewSettings.Add((byte)RoleId.NiceWatcher, CustomOptionHolder.watcherSpawnRate.getSelection());
            }

            // Assign Sheriff
            if ((CustomOptionHolder.deputySpawnRate.getSelection() > 0 &&
                CustomOptionHolder.sheriffSpawnRate.getSelection() == 10) ||
                CustomOptionHolder.deputySpawnRate.getSelection() == 0)
                    data.crewSettings.Add((byte)RoleId.Sheriff, CustomOptionHolder.sheriffSpawnRate.getSelection());


            // Assign Yasuna (chance to be impostor based on setting)
            bool isEvilYasuna = (rnd.Next(1, 101) <= CustomOptionHolder.yasunaIsImpYasunaRate.getSelection() * 10);
            if (isEvilYasuna) data.impSettings.Add((byte)RoleId.EvilYasuna, CustomOptionHolder.yasunaSpawnRate.getSelection());
            else data.crewSettings.Add((byte)RoleId.Yasuna, CustomOptionHolder.yasunaSpawnRate.getSelection());

            crewValues = data.crewSettings.Values.ToList().Sum();
            impValues = data.impSettings.Values.ToList().Sum();

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetShifterType, Hazel.SendOption.Reliable, -1);
            writer.Write(shifterIsNeutral);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setShifterType(shifterIsNeutral);
        }

        private static void assignEnsuredRoles(RoleAssignmentData data) {
            // Get all roles where the chance to occur is set to 100%
            List<byte> ensuredCrewmateRoles = data.crewSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<byte> ensuredNeutralRoles = data.neutralSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<byte> ensuredImpostorRoles = data.impSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (
                (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) ||
                (data.crewmates.Count > 0 && (
                    (data.maxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) ||
                    (data.maxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
                ))) {

                Dictionary<RoleType, List<byte>> rolesToAssign = new();
                if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) rolesToAssign.Add(RoleType.Crewmate, ensuredCrewmateRoles);
                if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0) rolesToAssign.Add(RoleType.Neutral, ensuredNeutralRoles);
                if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) rolesToAssign.Add(RoleType.Impostor, ensuredImpostorRoles);

                // Randomly select a pool of roles to assign a role from next (Crewmate role, Neutral role or Impostor role)
                // then select one of the roles from the selected pool to a player
                // and remove the role (and any potentially blocked role pairings) from the pool(s)
                var roleType = rolesToAssign.Keys.ElementAt(rnd.Next(0, rolesToAssign.Keys.Count()));
                var players = roleType == RoleType.Crewmate || roleType == RoleType.Neutral ? data.crewmates : data.impostors;
                var index = rnd.Next(0, rolesToAssign[roleType].Count);
                var roleId = rolesToAssign[roleType][index];
                setRoleToRandomPlayer(rolesToAssign[roleType][index], players);
                rolesToAssign[roleType].RemoveAt(index);

                if (CustomOptionHolder.blockedRolePairings.ContainsKey(roleId)) {
                    foreach(var blockedRoleId in CustomOptionHolder.blockedRolePairings[roleId]) {
                        // Set chance for the blocked roles to 0 for chances less than 100%
                        if (data.impSettings.ContainsKey(blockedRoleId)) data.impSettings[blockedRoleId] = 0;
                        if (data.neutralSettings.ContainsKey(blockedRoleId)) data.neutralSettings[blockedRoleId] = 0;
                        if (data.crewSettings.ContainsKey(blockedRoleId)) data.crewSettings[blockedRoleId] = 0;
                        // Remove blocked roles even if the chance was 100%
                        foreach(var ensuredRolesList in rolesToAssign.Values) {
                            ensuredRolesList.RemoveAll(x => x == blockedRoleId);
                        }
                    }
                }

                // Adjust the role limit
                switch (roleType) {
                    case RoleType.Crewmate: data.maxCrewmateRoles--; crewValues -= 10; break;
                    case RoleType.Neutral: data.maxNeutralRoles--; break;
                    case RoleType.Impostor: data.maxImpostorRoles--; impValues -= 10;  break;
                }
            }
        }

        private static void assignDependentRoles(RoleAssignmentData data) {
            // Roles that prob have a dependent role
            bool guesserFlag = CustomOptionHolder.guesserSpawnBothRate.getSelection() > 0
                && CustomOptionHolder.guesserSpawnRate.getSelection() > 0;
            bool sheriffFlag = CustomOptionHolder.deputySpawnRate.getSelection() > 0
                && CustomOptionHolder.sheriffSpawnRate.getSelection() > 0;
            bool watcherFlag = CustomOptionHolder.watcherSpawnBothRate.getSelection() > 0
                && CustomOptionHolder.watcherSpawnRate.getSelection() > 0;

            if (isGuesserGamemode) guesserFlag = false;
            if (!guesserFlag && !sheriffFlag && !watcherFlag) return; // assignDependentRoles is not needed

            int crew = data.crewmates.Count < data.maxCrewmateRoles ? data.crewmates.Count : data.maxCrewmateRoles; // Max number of crew loops
            int imp = data.impostors.Count < data.maxImpostorRoles ? data.impostors.Count : data.maxImpostorRoles; // Max number of imp loops
            int crewSteps = crew / data.crewSettings.Keys.Count(); // Avarage crewvalues deducted after each loop
            int impSteps = imp / data.impSettings.Keys.Count(); // Avarage impvalues deducted after each loop

            // set to false if needed, otherwise we can skip the loop
            bool isSheriff = !sheriffFlag;
            bool isGuesser = !guesserFlag;
            bool isWatcher = !watcherFlag;

            // --- Simulate Crew & Imp ticket system ---
            while (crew > 0 && (!isSheriff || (!isEvilGuesser && !isGuesser) || (!isEvilWatcher && !isWatcher))) {
                if (!isSheriff && rnd.Next(crewValues) < CustomOptionHolder.sheriffSpawnRate.getSelection()) isSheriff = true;
                if (!isEvilGuesser && !isGuesser && rnd.Next(crewValues) < CustomOptionHolder.guesserSpawnRate.getSelection()) isGuesser = true;
                if (!isEvilWatcher && !isWatcher && rnd.Next(crewValues) < CustomOptionHolder.watcherSpawnRate.getSelection()) isWatcher = true;
                crew--;
                crewValues -= crewSteps;
            }
            while (imp > 0 && ((isEvilGuesser && !isGuesser) || (isEvilWatcher && !isWatcher))) {
                if (rnd.Next(impValues) < CustomOptionHolder.guesserSpawnRate.getSelection()) isGuesser = true;
                if (rnd.Next(impValues) < CustomOptionHolder.watcherSpawnRate.getSelection()) isWatcher = true;
                imp--;
                impValues -= impSteps;
            }

            // --- Assign Main Roles if they won the lottery ---
            if (isSheriff && Sheriff.sheriff == null && data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && sheriffFlag) { // Set Sheriff cause he won the lottery
                byte sheriff = setRoleToRandomPlayer((byte)RoleId.Sheriff, data.crewmates);
                data.crewmates.ToList().RemoveAll(x => x.PlayerId == sheriff);
                data.maxCrewmateRoles--;
            }
            if (!isGuesserGamemode) {
                if (!isEvilGuesser && isGuesser && Guesser.niceGuesser == null && data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && guesserFlag) { // Set Nice Guesser cause he won the lottery
                    byte niceGuesser = setRoleToRandomPlayer((byte)RoleId.NiceGuesser, data.crewmates);
                    data.crewmates.ToList().RemoveAll(x => x.PlayerId == niceGuesser);
                    data.maxCrewmateRoles--;
                }
                else if (isEvilGuesser && isGuesser && Guesser.evilGuesser == null && data.impostors.Count > 0 && data.maxImpostorRoles > 0 && guesserFlag) { // Set Evil Guesser cause he won the lottery
                    byte evilGuesser = setRoleToRandomPlayer((byte)RoleId.EvilGuesser, data.impostors);
                    data.impostors.ToList().RemoveAll(x => x.PlayerId == evilGuesser);
                    data.maxImpostorRoles--;
                }
            }
            if (!isEvilWatcher && isWatcher && Watcher.nicewatcher == null && data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && watcherFlag)
            {
                byte niceWatcher = setRoleToRandomPlayer((byte)RoleId.NiceWatcher, data.crewmates);
                data.crewmates.ToList().RemoveAll(x => x.PlayerId == niceWatcher);
                data.maxCrewmateRoles--;
            }
            else if (isEvilWatcher && isWatcher && Watcher.evilwatcher == null && data.impostors.Count > 0 && data.maxImpostorRoles > 0 && watcherFlag)
            {
                byte evilWatcher = setRoleToRandomPlayer((byte)RoleId.EvilWatcher, data.impostors);
                data.impostors.ToList().RemoveAll(x => x.PlayerId == evilWatcher);
                data.maxImpostorRoles--;
            }

            // --- Assign Dependent Roles if main role exists ---
            if (Sheriff.sheriff != null) { // Deputy
                if (CustomOptionHolder.deputySpawnRate.getSelection() == 10 && data.crewmates.Count > 0 && data.maxCrewmateRoles > 0) { // Force Deputy
                    byte deputy = setRoleToRandomPlayer((byte)RoleId.Deputy, data.crewmates);
                    data.crewmates.ToList().RemoveAll(x => x.PlayerId == deputy);
                    data.maxCrewmateRoles--;
                } else if (CustomOptionHolder.deputySpawnRate.getSelection() < 10) // Dont force, add Deputy to the ticket system
                    data.crewSettings.Add((byte)RoleId.Deputy, CustomOptionHolder.deputySpawnRate.getSelection());
            }

            if (!data.crewSettings.ContainsKey((byte)RoleId.Sheriff)) data.crewSettings.Add((byte)RoleId.Sheriff, 0);

            if (!isGuesserGamemode) {
                if (!isEvilGuesser && Guesser.niceGuesser != null) { // Other Guesser (evil)
                    if (CustomOptionHolder.guesserSpawnBothRate.getSelection() == 10 && data.impostors.Count > 0 && data.maxImpostorRoles > 0) { // Force other guesser (evil)
                        byte bothGuesser = setRoleToRandomPlayer((byte)RoleId.EvilGuesser, data.impostors);
                        data.impostors.ToList().RemoveAll(x => x.PlayerId == bothGuesser);
                        data.maxImpostorRoles--;
                    }
                    else if (CustomOptionHolder.guesserSpawnBothRate.getSelection() < 10) // Dont force, add Guesser (evil) to the ticket system
                        data.impSettings.Add((byte)RoleId.EvilGuesser, CustomOptionHolder.guesserSpawnBothRate.getSelection());
                }
                else if (isEvilGuesser && Guesser.evilGuesser != null) { // ELSE other Guesser (nice)
                    if (CustomOptionHolder.guesserSpawnBothRate.getSelection() == 10 && data.crewmates.Count > 0 && data.maxCrewmateRoles > 0) { // Force other guesser (nice)
                        byte bothGuesser = setRoleToRandomPlayer((byte)RoleId.NiceGuesser, data.crewmates);
                        data.crewmates.ToList().RemoveAll(x => x.PlayerId == bothGuesser);
                        data.maxCrewmateRoles--;
                    }
                    else if (CustomOptionHolder.guesserSpawnBothRate.getSelection() < 10) // Dont force, add Guesser (nice) to the ticket system
                        data.crewSettings.Add((byte)RoleId.NiceGuesser, CustomOptionHolder.guesserSpawnBothRate.getSelection());
                }
            }

            if (!isEvilWatcher && Watcher.nicewatcher != null)
            {
                if (CustomOptionHolder.watcherSpawnBothRate.getSelection() == 10 && data.impostors.Count > 0 && data.maxImpostorRoles > 0)
                {
                    byte bothWatcher = setRoleToRandomPlayer((byte)RoleId.EvilWatcher, data.impostors);
                    data.impostors.ToList().RemoveAll(x => x.PlayerId == bothWatcher);
                    data.maxImpostorRoles--;
                }
                else if (CustomOptionHolder.watcherSpawnBothRate.getSelection() < 10)
                    data.impSettings.Add((byte)RoleId.EvilWatcher, CustomOptionHolder.watcherSpawnBothRate.getSelection());
            }
            else if (isEvilWatcher && Watcher.evilwatcher != null)
            {
                if (CustomOptionHolder.watcherSpawnBothRate.getSelection() == 10 && data.crewmates.Count > 0 && data.maxCrewmateRoles > 0)
                {
                    byte bothWatcher = setRoleToRandomPlayer((byte)RoleId.NiceWatcher, data.crewmates);
                    data.crewmates.ToList().RemoveAll(x => x.PlayerId == bothWatcher);
                    data.maxCrewmateRoles--;
                }
                else if (CustomOptionHolder.watcherSpawnBothRate.getSelection() < 10)
                    data.crewSettings.Add((byte)RoleId.NiceWatcher, CustomOptionHolder.watcherSpawnBothRate.getSelection());
            }
        }
        private static void assignChanceRoles(RoleAssignmentData data) {
            // Get all roles where the chance to occur is set grater than 0% but not 100% and build a ticket pool based on their weight
            List<byte> crewmateTickets = data.crewSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();
            List<byte> neutralTickets = data.neutralSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();
            List<byte> impostorTickets = data.impSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (
                (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && impostorTickets.Count > 0) ||
                (data.crewmates.Count > 0 && (
                    (data.maxCrewmateRoles > 0 && crewmateTickets.Count > 0) ||
                    (data.maxNeutralRoles > 0 && neutralTickets.Count > 0)
                ))) {

                Dictionary<RoleType, List<byte>> rolesToAssign = new();
                if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && crewmateTickets.Count > 0) rolesToAssign.Add(RoleType.Crewmate, crewmateTickets);
                if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && neutralTickets.Count > 0) rolesToAssign.Add(RoleType.Neutral, neutralTickets);
                if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && impostorTickets.Count > 0) rolesToAssign.Add(RoleType.Impostor, impostorTickets);

                // Randomly select a pool of role tickets to assign a role from next (Crewmate role, Neutral role or Impostor role)
                // then select one of the roles from the selected pool to a player
                // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)
                var roleType = rolesToAssign.Keys.ElementAt(rnd.Next(0, rolesToAssign.Keys.Count()));
                var players = roleType == RoleType.Crewmate || roleType == RoleType.Neutral ? data.crewmates : data.impostors;
                var index = rnd.Next(0, rolesToAssign[roleType].Count);
                var roleId = rolesToAssign[roleType][index];
                setRoleToRandomPlayer(roleId, players);
                rolesToAssign[roleType].RemoveAll(x => x == roleId);

                if (CustomOptionHolder.blockedRolePairings.ContainsKey(roleId)) {
                    foreach(var blockedRoleId in CustomOptionHolder.blockedRolePairings[roleId]) {
                        // Remove tickets of blocked roles from all pools
                        crewmateTickets.RemoveAll(x => x == blockedRoleId);
                        neutralTickets.RemoveAll(x => x == blockedRoleId);
                        impostorTickets.RemoveAll(x => x == blockedRoleId);
                    }
                }

                // Adjust the role limit
                switch (roleType) {
                    case RoleType.Crewmate: data.maxCrewmateRoles--; break;
                    case RoleType.Neutral: data.maxNeutralRoles--;break;
                    case RoleType.Impostor: data.maxImpostorRoles--;break;
                }
            }
        }

        public static void assignRoleTargets(RoleAssignmentData data) {
            // Set Lawyer or Prosecutor Target
            if (Lawyer.lawyer != null) {
                var possibleTargets = new List<PlayerControl>();
                //if (!Lawyer.isProsecutor) { // Lawyer
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    if (!p.Data.IsDead && !p.Data.Disconnected && p != Lovers.lover1 && p != Lovers.lover2 && (p.Data.Role.IsImpostor || p == Jackal.jackal || (Lawyer.targetCanBeJester && p == Jester.jester)))
                        possibleTargets.Add(p);
                }
            //}
                /*else { // Prosecutor
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                        if (!p.Data.IsDead && !p.Data.Disconnected && p != Lovers.lover1 && p != Lovers.lover2 && p != Mini.mini && !p.Data.Role.IsImpostor && !Helpers.isNeutral(p) && p != Swapper.swapper)
                            possibleTargets.Add(p);
                    }
                }*/

                if (possibleTargets.Count == 0) {
                    MessageWriter w = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(w);
                    RPCProcedure.lawyerPromotesToPursuer();
                } else {
                    var target = possibleTargets[TheOtherRoles.rnd.Next(0, possibleTargets.Count)];
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerSetTarget, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerSetTarget(target.PlayerId);
                }
            }

            // Set Kataomoi target
            if (Kataomoi.kataomoi != null)
            {
                var possibleTargets = new List<PlayerControl>();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (!p.Data.IsDead && p != Kataomoi.kataomoi)
                        possibleTargets.Add(p);
                }
                if (possibleTargets.Count > 0)
                {
                    var target = possibleTargets[TheOtherRoles.rnd.Next(0, possibleTargets.Count)];
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.KataomoiSetTarget, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.kataomoiSetTarget(target.PlayerId);
                }
            }
        }

        public static void assignModifiers() {
            var modifierMin = CustomOptionHolder.modifiersCountMin.getSelection();
            var modifierMax = CustomOptionHolder.modifiersCountMax.getSelection();
            if (modifierMin > modifierMax) modifierMin = modifierMax;
            int modifierCountSettings = rnd.Next(modifierMin, modifierMax + 1);
            List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList();
            if (isGuesserGamemode && !CustomOptionHolder.guesserGamemodeHaveModifier.getBool())
                players.RemoveAll(x => GuesserGM.isGuesser(x.PlayerId));
            int modifierCount = Mathf.Min(players.Count, modifierCountSettings);

            if (modifierCount == 0) return;

            List<RoleId> allModifiers = new();
            List<RoleId> ensuredModifiers = new();
            List<RoleId> chanceModifiers = new();
            allModifiers.AddRange(new List<RoleId> {
                RoleId.Tiebreaker,
                RoleId.Mini,
                //RoleId.Bait,
                RoleId.Bloody,
                RoleId.AntiTeleport,
                RoleId.Sunglasses,
                RoleId.Vip,
                RoleId.Invert,
                RoleId.Chameleon,
                RoleId.Armored,
                RoleId.TimeEater
                //RoleId.Shifter
            });
            List<PlayerControl> impostorPlayers = players.Where(x => x.Data.Role.IsImpostor).ToList();
            if (allModifiers.Contains(RoleId.TimeEater)) {
                assignModifierToImpostors(RoleId.TimeEater, impostorPlayers, ref modifierCount);
                allModifiers.Remove(RoleId.TimeEater);
            }




            var crewPlayerMadmate = new List<PlayerControl>(players);
            crewPlayerMadmate.RemoveAll(x => x.Data.Role.IsImpostor || Helpers.isNeutral(x) || x == Spy.spy || x == FortuneTeller.fortuneTeller || x == Sprinter.sprinter || x == Veteran.veteran
            || x == Deputy.deputy || x == Portalmaker.portalmaker || x == TaskMaster.taskMaster || x == Sherlock.sherlock || x == Snitch.snitch || x == Teleporter.teleporter || x == Prophet.prophet);

            // Always remember to remove the Mad Sheriff if Deputy is assigned
            if (Deputy.deputy != null && Sheriff.sheriff != null) crewPlayerMadmate.RemoveAll(x => x == Sheriff.sheriff);

            byte playerId;
            bool isFixedMadmateAssigned = !crewPlayerMadmate.Any(x => RoleInfo.getRoleInfoForPlayer(x, includeHidden: true).Any(y => y.roleId == Madmate.fixedRole));
            for (int i = 0; i < CustomOptionHolder.madmateQuantity.getQuantity(); i++)
            {
                if (crewPlayerMadmate.Count <= 0) break;
                if (rnd.Next(1, 101) <= CustomOptionHolder.madmateSpawnRate.getSelection() * 10)
                {
                    if (Madmate.fixedRole != RoleId.Jester && !isFixedMadmateAssigned)
                    {
                        playerId = setModifierToRandomPlayer((byte)RoleId.Madmate, crewPlayerMadmate.Where(x => RoleInfo.getRoleInfoForPlayer(x, false, true).Any(y =>
                        y.roleId == Madmate.fixedRole)).ToList());
                    }
                    else playerId = setModifierToRandomPlayer((byte)RoleId.Madmate, crewPlayerMadmate);
                    crewPlayerMadmate.RemoveAll(x => x.PlayerId == playerId);
                    modifierCount--;
                }
            }

            if (rnd.Next(1, 101) <= CustomOptionHolder.modifierLover.getSelection() * 10) { // Assign lover
                bool isEvilLover = rnd.Next(1, 101) <= CustomOptionHolder.modifierLoverImpLoverRate.getSelection() * 10;
                byte firstLoverId;
                List<PlayerControl> impPlayer = new(players);
                List<PlayerControl> crewPlayer = new(players);
                impPlayer.RemoveAll(x => !x.Data.Role.IsImpostor || x == NekoKabocha.nekoKabocha);
                if (MimicK.ifOneDiesBothDie) impPlayer.RemoveAll(y => y == MimicK.mimicK || y == MimicA.mimicA);
                if (BomberA.ifOneDiesBothDie) impPlayer.RemoveAll(z => z == BomberA.bomberA || z == BomberB.bomberB);
                crewPlayer.RemoveAll(x => x.Data.Role.IsImpostor || x == Lawyer.lawyer || x == FortuneTeller.fortuneTeller || x == Akujo.akujo || x == Cupid.cupid || x == Fox.fox
                || x == Kataomoi.kataomoi);

                if (isEvilLover) firstLoverId = setModifierToRandomPlayer((byte)RoleId.Lover, impPlayer);
                else firstLoverId = setModifierToRandomPlayer((byte)RoleId.Lover, crewPlayer);
                byte secondLoverId = setModifierToRandomPlayer((byte)RoleId.Lover, crewPlayer, 1);

                players.RemoveAll(x => x.PlayerId == firstLoverId || x.PlayerId == secondLoverId);
                modifierCount--;
            }

            foreach (RoleId m in allModifiers) {
                if (getSelectionForRoleId(m) == 10) ensuredModifiers.AddRange(Enumerable.Repeat(m, getSelectionForRoleId(m, true) / 10));
                else chanceModifiers.AddRange(Enumerable.Repeat(m, getSelectionForRoleId(m, true)));
            }

            assignModifiersToPlayers(ensuredModifiers, players, modifierCount); // Assign ensured modifier

            modifierCount -= ensuredModifiers.Count;
            if (modifierCount <= 0) return;
            int chanceModifierCount = Mathf.Min(modifierCount, chanceModifiers.Count);
            List<RoleId> chanceModifierToAssign = new();
            while (chanceModifierCount > 0 && chanceModifiers.Count > 0) {
                var index = rnd.Next(0, chanceModifiers.Count);
                RoleId modifierId = chanceModifiers[index];
                chanceModifierToAssign.Add(modifierId);

                int modifierSelection = getSelectionForRoleId(modifierId);
                while (modifierSelection > 0) {
                    chanceModifiers.Remove(modifierId);
                    modifierSelection--;
                }
                chanceModifierCount--;
            }

            assignModifiersToPlayers(chanceModifierToAssign, players, modifierCount); // Assign chance modifier
        }
        private static void assignModifierToImpostors(RoleId modifierId, List<PlayerControl> impostorPlayers, ref int modifierCount) {
            if (impostorPlayers.Count == 0 || modifierCount == 0) return;

            // 这里可以实现具体的分配逻辑，例如随机分配

            // 调用设置modifier的方法
            setModifierToRandomPlayer((byte)modifierId, impostorPlayers);
            modifierCount--;
        }





        public static void assignGuesserGamemode() {
            List<PlayerControl> impPlayer = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            List<PlayerControl> neutralPlayer = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            List<PlayerControl> crewPlayer = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            impPlayer.RemoveAll(x => !x.Data.Role.IsImpostor);
            neutralPlayer.RemoveAll(x => !Helpers.isNeutral(x) || x == Doomsayer.doomsayer); // Remove Doomsayer from Guesser List
            crewPlayer.RemoveAll(x => x.Data.Role.IsImpostor || Helpers.isNeutral(x));
            assignGuesserGamemodeToPlayers(crewPlayer, Mathf.RoundToInt(CustomOptionHolder.guesserGamemodeCrewNumber.getFloat()));
            assignGuesserGamemodeToPlayers(neutralPlayer, Mathf.RoundToInt(CustomOptionHolder.guesserGamemodeNeutralNumber.getFloat()), CustomOptionHolder.guesserForceJackalGuesser.getBool(), CustomOptionHolder.guesserForceThiefGuesser.getBool());
            assignGuesserGamemodeToPlayers(impPlayer, Mathf.RoundToInt(CustomOptionHolder.guesserGamemodeImpNumber.getFloat()));
        }

        private static void assignGuesserGamemodeToPlayers(List<PlayerControl> playerList, int count, bool forceJackal = false, bool forceThief = false) {
            for (int i = 0; i < count && playerList.Count > 0; i++) {
                var index = rnd.Next(0, playerList.Count);
                if (forceThief && !forceJackal) {
                    if (Thief.thief != null)
                        index = playerList.FindIndex(x => x == Thief.thief);
                    forceThief = false;
                }
                if (forceJackal) {
                    if (Jackal.jackal != null)
                        index = playerList.FindIndex(x => x == Jackal.jackal);
                    forceJackal = false;
                }
                byte playerId = playerList[index].PlayerId;
                playerList.RemoveAt(index);

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGuesserGm, Hazel.SendOption.Reliable, -1);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setGuesserGm(playerId);
            }
        }

        private static byte setRoleToRandomPlayer(byte roleId, List<PlayerControl> playerList, bool removePlayer = true) {
            var index = rnd.Next(0, playerList.Count);
            byte playerId = playerList[index].PlayerId;
            if (removePlayer) playerList.RemoveAt(index);

            playerRoleMap.Add(new Tuple<byte, byte>(playerId, roleId));

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRole, Hazel.SendOption.Reliable, -1);
            writer.Write(roleId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setRole(roleId, playerId);
            return playerId;
        }

        private static byte setModifierToRandomPlayer(byte modifierId, List<PlayerControl> playerList, byte flag = 0) {
            if (playerList.Count == 0) return Byte.MaxValue;
            var index = rnd.Next(0, playerList.Count);
            byte playerId = playerList[index].PlayerId;
            playerList.RemoveAt(index);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetModifier, Hazel.SendOption.Reliable, -1);
            writer.Write(modifierId);
            writer.Write(playerId);
            writer.Write(flag);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setModifier(modifierId, playerId, flag);
            return playerId;
        }

        private static void assignModifiersToPlayers(List<RoleId> modifiers, List<PlayerControl> playerList, int modifierCount) {
            modifiers = modifiers.OrderBy(x => rnd.Next()).ToList(); // randomize list

            while (modifierCount < modifiers.Count) {
                var index = rnd.Next(0, modifiers.Count);
                modifiers.RemoveAt(index);
            }

            byte playerId;

            List<PlayerControl> crewPlayer = new(playerList);
            crewPlayer.RemoveAll(x => x.Data.Role.IsImpostor || RoleInfo.getRoleInfoForPlayer(x).Any(r => r.isNeutral));
            /*if (modifiers.Contains(RoleId.Shifter)) {
                var crewPlayerShifter = new List<PlayerControl>(crewPlayer);
                crewPlayerShifter.RemoveAll(x => x == Spy.spy);
                playerId = setModifierToRandomPlayer((byte)RoleId.Shifter, crewPlayerShifter);
                crewPlayer.RemoveAll(x => x.PlayerId == playerId);
                playerList.RemoveAll(x => x.PlayerId == playerId);
                modifiers.RemoveAll(x => x == RoleId.Shifter);
            }*/
            if (modifiers.Contains(RoleId.Sunglasses)) {
                int sunglassesCount = 0;
                while (sunglassesCount < modifiers.FindAll(x => x == RoleId.Sunglasses).Count) {
                    playerId = setModifierToRandomPlayer((byte)RoleId.Sunglasses, crewPlayer);
                    crewPlayer.RemoveAll(x => x.PlayerId == playerId);
                    playerList.RemoveAll(x => x.PlayerId == playerId);
                    sunglassesCount++;
                }
                modifiers.RemoveAll(x => x == RoleId.Sunglasses);
            }

            foreach (RoleId modifier in modifiers) {
                if (playerList.Count == 0) break;
                playerId = setModifierToRandomPlayer((byte)modifier, playerList);
                playerList.RemoveAll(x => x.PlayerId == playerId);
            }
        }

        private static int getSelectionForRoleId(RoleId roleId, bool multiplyQuantity = false) {
            int selection = 0;
            switch (roleId) {
                case RoleId.Lover:
                    selection = CustomOptionHolder.modifierLover.getSelection(); break;
                case RoleId.Tiebreaker:
                    selection = CustomOptionHolder.modifierTieBreaker.getSelection(); break;
                case RoleId.Mini:
                    selection = CustomOptionHolder.modifierMini.getSelection();
                    if (EventUtility.isEnabled)
                    {
                        selection = 10;
                        if (CustomOptionHolder.modifierMini.getSelection() == 0 && CustomOptionHolder.eventReallyNoMini.getBool())
                            selection = 0;
                    }
                    break;
                //case RoleId.Bait:
                //selection = CustomOptionHolder.modifierBait.getSelection();
                //if (multiplyQuantity) selection *= CustomOptionHolder.modifierBaitQuantity.getQuantity();
                //break;
                case RoleId.Bloody:
                    selection = CustomOptionHolder.modifierBloody.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierBloodyQuantity.getQuantity();
                    break;
                case RoleId.AntiTeleport:
                    selection = CustomOptionHolder.modifierAntiTeleport.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierAntiTeleportQuantity.getQuantity();
                    break;
                case RoleId.Sunglasses:
                    selection = CustomOptionHolder.modifierSunglasses.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierSunglassesQuantity.getQuantity();
                    break;
                case RoleId.Madmate:
                    selection = CustomOptionHolder.madmateSpawnRate.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.madmateQuantity.getQuantity();
                    break;
                case RoleId.Vip:
                    selection = CustomOptionHolder.modifierVip.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierVipQuantity.getQuantity();
                    break;
                case RoleId.Invert:
                    selection = CustomOptionHolder.modifierInvert.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierInvertQuantity.getQuantity();
                    break;
                case RoleId.Chameleon:
                    selection = CustomOptionHolder.modifierChameleon.getSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierChameleonQuantity.getQuantity();
                    break;
                case RoleId.Armored:
                    selection = CustomOptionHolder.modifierArmored.getSelection();
                    break;
                    //case RoleId.Shifter:
                    //selection = CustomOptionHolder.modifierShifter.getSelection();
                    //break;
            }

            return selection;
        }

        private static void setRolesAgain()
        {

            while (playerRoleMap.Any())
            {
                byte amount = (byte)Math.Min(playerRoleMap.Count, 20);
                var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WorkaroundSetRoles, SendOption.Reliable, -1);
                writer.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    var option = playerRoleMap[0];
                    playerRoleMap.RemoveAt(0);
                    writer.WritePacked((uint)option.Item1);
                    writer.WritePacked((uint)option.Item2);
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public class RoleAssignmentData {
            public List<PlayerControl> crewmates {get;set;}
            public List<PlayerControl> impostors {get;set;}
            public Dictionary<byte, int> impSettings = new();
            public Dictionary<byte, int> neutralSettings = new();
            public Dictionary<byte, int> crewSettings = new();
            public int maxCrewmateRoles {get;set;}
            public int maxNeutralRoles {get;set;}
            public int maxImpostorRoles {get;set;}
        }

        private enum RoleType {
            Crewmate = 0,
            Neutral = 1,
            Impostor = 2
        }

    }
}
