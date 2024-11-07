using System.Linq;
using System;
using System.Collections.Generic;
using TheOtherRoles.Players;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using Reactor.Networking;

namespace TheOtherRoles
{
    public class RoleInfo {
        public Color color;
        public string name { get { return ModTranslation.getString(nameKey); } }
        public string introDescription { get { return ModTranslation.getString(nameKey + "IntroDesc"); } }
        public string shortDescription { get { return ModTranslation.getString(nameKey + "ShortDesc"); } }
        public string fullDescription { get { return ModTranslation.getString(nameKey + "FullDesc"); } }
        public RoleId roleId;
        public bool isNeutral;
        public bool isModifier;
        public string nameKey;

        public RoleInfo(string name, Color color, RoleId roleId, bool isNeutral = false, bool isModifier = false)
        {
            nameKey = name;
            this.color = color;
            this.roleId = roleId;
            this.isNeutral = isNeutral;
            this.isModifier = isModifier;
        }

        public static RoleInfo jester = new("jester", Jester.color, RoleId.Jester, true);
        public static RoleInfo mayor = new("mayor", Mayor.color, RoleId.Mayor);
        public static RoleInfo portalmaker = new("portalmaker", Portalmaker.color, RoleId.Portalmaker);
        public static RoleInfo engineer = new("engineer", Engineer.color, RoleId.Engineer);
        public static RoleInfo sheriff = new("sheriff", Sheriff.color, RoleId.Sheriff);
        public static RoleInfo deputy = new("deputy", Sheriff.color, RoleId.Deputy);
        public static RoleInfo lighter = new("lighter", Lighter.color, RoleId.Lighter);
        public static RoleInfo godfather = new("godfather", Godfather.color, RoleId.Godfather);
        public static RoleInfo mafioso = new("mafioso", Mafioso.color, RoleId.Mafioso);
        public static RoleInfo janitor = new("janitor", Janitor.color, RoleId.Janitor);
        public static RoleInfo morphling = new("morphling", Morphling.color, RoleId.Morphling);
        public static RoleInfo camouflager = new("camouflager", Camouflager.color, RoleId.Camouflager);
        public static RoleInfo vampire = new("vampire", Vampire.color, RoleId.Vampire);
        public static RoleInfo eraser = new("eraser", Eraser.color, RoleId.Eraser);
        public static RoleInfo trickster = new("trickster", Trickster.color, RoleId.Trickster);
        public static RoleInfo cleaner = new("cleaner", Cleaner.color, RoleId.Cleaner);
        public static RoleInfo warlock = new("warlock", Warlock.color, RoleId.Warlock);
        public static RoleInfo bountyHunter = new("bountyHunter", BountyHunter.color, RoleId.BountyHunter);
        public static RoleInfo detective = new("detective", Detective.color, RoleId.Detective);
        public static RoleInfo bait = new("bait", Bait.color, RoleId.Bait);
        public static RoleInfo timeMaster = new("timeMaster", TimeMaster.color, RoleId.TimeMaster);
        public static RoleInfo medic = new("medic", Medic.color, RoleId.Medic);
        public static RoleInfo niceSwapper = new("niceSwapper", Swapper.color, RoleId.Swapper);
        public static RoleInfo seer = new("seer", Seer.color, RoleId.Seer);
        public static RoleInfo hacker = new("hacker", Hacker.color, RoleId.Hacker);
        public static RoleInfo niceshifter = new("niceShifter", Shifter.color, RoleId.Shifter);
        public static RoleInfo tracker = new("tracker", Tracker.color, RoleId.Tracker);
        public static RoleInfo snitch = new("snitch", Snitch.color, RoleId.Snitch);
        public static RoleInfo jackal = new("jackal", Jackal.color, RoleId.Jackal, true);
        public static RoleInfo sidekick = new("sidekick", Sidekick.color, RoleId.Sidekick, true);
        public static RoleInfo spy = new("spy", Spy.color, RoleId.Spy);
        public static RoleInfo securityGuard = new("securityGuard", SecurityGuard.color, RoleId.SecurityGuard);
        public static RoleInfo arsonist = new("arsonist", Arsonist.color, RoleId.Arsonist, true);
        public static RoleInfo goodGuesser = new("niceGuesser", Guesser.color, RoleId.NiceGuesser);
        public static RoleInfo badGuesser = new("evilGuesser", Palette.ImpostorRed, RoleId.EvilGuesser);
        public static RoleInfo niceWatcher = new("niceWatcher", Watcher.color, RoleId.NiceWatcher);
        public static RoleInfo evilWatcher = new("evilWatcher", Palette.ImpostorRed, RoleId.EvilWatcher);
        public static RoleInfo vulture = new("vulture", Vulture.color, RoleId.Vulture, true);
        public static RoleInfo medium = new("medium", Medium.color, RoleId.Medium);
        //public static RoleInfo trapper = new RoleInfo("Trapper", Trapper.color, "Place traps to find the Impostors", "Place traps", RoleId.Trapper);
        public static RoleInfo lawyer = new("lawyer", Lawyer.color, RoleId.Lawyer, true);
        //public static RoleInfo prosecutor = new RoleInfo("Prosecutor", Lawyer.color, "Vote out your target", "Vote out your target", RoleId.Prosecutor, true);
        public static RoleInfo pursuer = new("pursuer", Pursuer.color, RoleId.Pursuer, true);
        public static RoleInfo impostor = new("impostor", Palette.ImpostorRed, RoleId.Impostor);
        public static RoleInfo crewmate = new("crewmate", Color.white, RoleId.Crewmate);
        public static RoleInfo witch = new("witch", Witch.color, RoleId.Witch);
        public static RoleInfo assassin = new("assassin", Assassin.color, RoleId.Assassin);
        public static RoleInfo thief = new("thief", Thief.color, RoleId.Thief, true);
        //public static RoleInfo bomber = new RoleInfo("Bomber", Bomber.color, "Bomb all Crewmates", "Bomb all Crewmates", RoleId.Bomber);

        // GMIA functional
        public static RoleInfo ninja = new("ninja", Ninja.color, RoleId.Ninja);
        public static RoleInfo nekoKabocha = new("nekoKabocha", NekoKabocha.color, RoleId.NekoKabocha);
        public static RoleInfo serialKiller = new("serialKiller", SerialKiller.color, RoleId.SerialKiller);
        public static RoleInfo evilTracker = new("evilTracker", EvilTracker.color, RoleId.EvilTracker);
        public static RoleInfo undertaker = new("undertaker", Undertaker.color, RoleId.Undertaker);
        public static RoleInfo mimicK = new("mimicK", MimicK.color, RoleId.MimicK);
        public static RoleInfo mimicA = new("mimicA", MimicA.color, RoleId.MimicA);
        public static RoleInfo bomberA = new("bomber", BomberA.color, RoleId.BomberA);
        public static RoleInfo bomberB = new("bomber", BomberB.color, RoleId.BomberB);
        public static RoleInfo evilSwapper = new("evilSwapper", Palette.ImpostorRed, RoleId.Swapper);
        public static RoleInfo evilHacker = new("evilHacker", EvilHacker.color, RoleId.EvilHacker);
        public static RoleInfo trapper = new("trapper", Trapper.color, RoleId.Trapper);
        public static RoleInfo blackmailer = new("blackmailer", Blackmailer.color, RoleId.Blackmailer);
        public static RoleInfo fortuneTeller = new("fortuneTeller", FortuneTeller.color, RoleId.FortuneTeller);
        public static RoleInfo veteran = new("veteran", Veteran.color, RoleId.Veteran);
        public static RoleInfo sprinter = new("sprinter", Sprinter.color, RoleId.Sprinter);
        public static RoleInfo sherlock = new("sherlock", Sherlock.color, RoleId.Sherlock);
        public static RoleInfo yasuna = new("niceYasuna", Yasuna.color, RoleId.Yasuna);
        public static RoleInfo taskMaster = new("taskMaster", TaskMaster.color, RoleId.TaskMaster);
        public static RoleInfo teleporter = new("teleporter", Teleporter.color, RoleId.Teleporter);
        public static RoleInfo prophet = new("prophet", Prophet.color, RoleId.Prophet);
        public static RoleInfo busker = new("busker", Busker.color, RoleId.Busker);
        public static RoleInfo noisemaker = new("noisemaker", Noisemaker.color, RoleId.Noisemaker);
        public static RoleInfo evilYasuna = new("evilYasuna", Palette.ImpostorRed, RoleId.EvilYasuna);
        public static RoleInfo opportunist = new("opportunist", Opportunist.color, RoleId.Opportunist, true);
        public static RoleInfo chainshifter = new("corruptedShifter", Shifter.color, RoleId.Shifter, true);
        public static RoleInfo moriarty = new("moriarty", Moriarty.color, RoleId.Moriarty, true);
        public static RoleInfo akujo = new("akujo", Akujo.color, RoleId.Akujo, true);
        public static RoleInfo plagueDoctor = new("plagueDoctor", PlagueDoctor.color, RoleId.PlagueDoctor, true);
        public static RoleInfo jekyllAndHyde = new("jekyllAndHyde", JekyllAndHyde.color, RoleId.JekyllAndHyde, true);
        public static RoleInfo cupid = new("cupid", Cupid.color, RoleId.Cupid, true);
        public static RoleInfo cupidLover = new("lover", Cupid.color, RoleId.Lover, false, true);
        public static RoleInfo fox = new("fox", Fox.color, RoleId.Fox, true);
        public static RoleInfo immoralist = new("immoralist", Immoralist.color, RoleId.Immoralist, true);
        public static RoleInfo schrodingersCat = new("schrodingersCat", SchrodingersCat.color, RoleId.SchrodingersCat, true);
        public static RoleInfo kataomoi = new("kataomoi", Kataomoi.color, RoleId.Kataomoi, true);

        public static RoleInfo hunter = new("hunter", Palette.ImpostorRed, RoleId.Impostor);
        public static RoleInfo hunted = new("hunted", Color.white, RoleId.Crewmate);



        // Modifier
        public static RoleInfo bloody = new("bloody", Color.yellow, RoleId.Bloody, false, true);
        public static RoleInfo antiTeleport = new("antiTeleportPostfix", Color.yellow, RoleId.AntiTeleport, false, true);
        public static RoleInfo tiebreaker = new("tiebreaker", Color.yellow, RoleId.Tiebreaker, false, true);
        //public static RoleInfo bait = new RoleInfo("Bait", Color.yellow, "Bait your enemies", "Bait your enemies", RoleId.Bait, false, true);
        public static RoleInfo sunglasses = new("sunglasses", Color.yellow, RoleId.Sunglasses, false, true);
        public static RoleInfo lover = new("lover", Lovers.color, RoleId.Lover, false, true);
        public static RoleInfo mini = new("mini", Color.yellow, RoleId.Mini, false, true);
        public static RoleInfo vip = new("vip", Color.yellow, RoleId.Vip, false, true);
        public static RoleInfo invert = new("invert", Color.yellow, RoleId.Invert, false, true);
        public static RoleInfo chameleon = new("chameleon", Color.yellow, RoleId.Chameleon, false, true);
        //public static RoleInfo shifter = new RoleInfo("Shifter", Color.yellow, "Shift your role", "Shift your role", RoleId.Shifter, false, true);


        public static List<RoleInfo> allRoleInfos = new() {
            impostor,
            godfather,
            mafioso,
            janitor,
            morphling,
            camouflager,
            vampire,
            eraser,
            trickster,
            cleaner,
            warlock,
            bountyHunter,
            witch,
            assassin,
            ninja,
            nekoKabocha,
            serialKiller,
            evilTracker,
            evilHacker,
            evilSwapper,
            trapper,
            undertaker,
            blackmailer,
            mimicK,
            mimicA,
            bomberA,
            bomberB,
            evilYasuna,
            //bomber,
            goodGuesser,
            badGuesser,
            evilWatcher,
            lover,
            jester,
            arsonist,
            jackal,
            sidekick,
            vulture,
            pursuer,
            lawyer,
            thief,
            opportunist,
            chainshifter,
            plagueDoctor,
            fox,
            immoralist,
            akujo,
            jekyllAndHyde,
            moriarty,
            cupid,
            schrodingersCat,
            kataomoi,
            //prosecutor,
            crewmate,
            mayor,
            portalmaker,
            engineer,
            sheriff,
            deputy,
            niceshifter, 
            bait,
            lighter,
            detective,
            timeMaster,
            medic,
            niceSwapper,
            niceWatcher,
            seer,
            hacker,
            tracker,
            snitch,
            spy,
            securityGuard,
            medium,
            fortuneTeller,
            sprinter,
            veteran,
            sherlock,
            yasuna,
            taskMaster,
            teleporter,
            prophet,
            busker,
            noisemaker,
            //trapper,
            bloody,
            antiTeleport,
            tiebreaker,
            sunglasses,
            mini,
            vip,
            invert,
            chameleon,
            cupidLover,
            //shifter, 
        };

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true, bool includeHidden = false) {
            List<RoleInfo> infos = new();
            if (p == null) return infos;

            // Modifier
            if (showModifier) {
                // after dead modifier
                if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended)
                {
                    //if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                    if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                    if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                }
                if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
                if (Cupid.lovers1 != null && Cupid.lovers2 != null && (p == Cupid.lovers2 || p == Cupid.lovers1)) infos.Add(cupidLover);
                if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
                if (p == Mini.mini) infos.Add(mini);
                if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
                if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
                //if (p == Shifter.shifter) infos.Add(shifter);
            }

            int count = infos.Count;  // Save count after modifiers are added so that the role count can be checked

            // Special roles
            if (p == Jester.jester) infos.Add(jester);
            if (p == Mayor.mayor) infos.Add(mayor);
            if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
            if (p == Engineer.engineer) infos.Add(engineer);
            if (p == Sheriff.sheriff || p == Sheriff.formerSheriff) infos.Add(sheriff);
            if (p == Deputy.deputy) infos.Add(deputy);
            if (p == Lighter.lighter) infos.Add(lighter);
            if (p == Godfather.godfather) infos.Add(godfather);
            if (p == Mafioso.mafioso) infos.Add(mafioso);
            if (p == Janitor.janitor) infos.Add(janitor);
            if (p == Morphling.morphling) infos.Add(morphling);
            if (p == Camouflager.camouflager) infos.Add(camouflager);
            if (p == Vampire.vampire) infos.Add(vampire);
            if (p == Eraser.eraser) infos.Add(eraser);
            if (p == Trickster.trickster) infos.Add(trickster);
            if (p == Cleaner.cleaner) infos.Add(cleaner);
            if (p == Warlock.warlock) infos.Add(warlock);
            if (p == Witch.witch) infos.Add(witch);
            if (p == Assassin.assassin) infos.Add(assassin);
            //if (p == Bomber.bomber) infos.Add(bomber);
            if (p == Detective.detective) infos.Add(detective);
            if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
            if (p == Medic.medic) infos.Add(medic);
            if (p == Swapper.swapper) infos.Add(p.Data.Role.IsImpostor ? evilSwapper : niceSwapper);
            if (p == Seer.seer) infos.Add(seer);
            if (p == Hacker.hacker) infos.Add(hacker);
            if (p == Tracker.tracker) infos.Add(tracker);
            if (p == Snitch.snitch) infos.Add(snitch);
            if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
            if (p == Sidekick.sidekick) infos.Add(sidekick);
            if (p == Spy.spy) infos.Add(spy);
            if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
            if (p == Bait.bait) infos.Add(bait);
            if (p == Veteran.veteran) infos.Add(veteran);
            if (p == Sherlock.sherlock) infos.Add(sherlock);
            if (p == Sprinter.sprinter) infos.Add(sprinter);
            if (p == Yasuna.yasuna) infos.Add(p.Data.Role.IsImpostor ? evilYasuna : yasuna);
            if (p == Moriarty.moriarty || p == Moriarty.formerMoriarty) infos.Add(moriarty);
            if (p == JekyllAndHyde.jekyllAndHyde || p == JekyllAndHyde.formerJekyllAndHyde) infos.Add(jekyllAndHyde);
            if (p == Akujo.akujo) infos.Add(akujo);
            if (p == Teleporter.teleporter) infos.Add(teleporter);
            if (p == Cupid.cupid) infos.Add(cupid);
            if (p == Blackmailer.blackmailer) infos.Add(blackmailer);
            if (p == Prophet.prophet) infos.Add(prophet);
            if (p == Fox.fox) infos.Add(fox);
            if (p == Kataomoi.kataomoi) infos.Add(kataomoi);
            if (p == Immoralist.immoralist) infos.Add(immoralist);
            if (p == Busker.busker) infos.Add(busker);
            if (p == Noisemaker.noisemaker) infos.Add(noisemaker);
            if (p == FortuneTeller.fortuneTeller)
            {
                if (PlayerControl.LocalPlayer.Data.IsDead || includeHidden)
                {
                    infos.Add(fortuneTeller);
                }
                else
                {
                    var info = FortuneTeller.isCompletedNumTasks(p) ? fortuneTeller : crewmate;
                    infos.Add(info);
                }
            }
            if (p == TaskMaster.taskMaster)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || includeHidden || !TaskMaster.becomeATaskMasterWhenCompleteAllTasks) infos.Add(taskMaster);
                else infos.Add(TaskMaster.isTaskComplete ? taskMaster : crewmate);
            }
            if (p == PlagueDoctor.plagueDoctor) infos.Add(plagueDoctor);
            if (p == SchrodingersCat.schrodingersCat || p == SchrodingersCat.formerSchrodingersCat) infos.Add(!SchrodingersCat.hideRole || includeHidden || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead
                || SchrodingersCat.hasTeam() || SchrodingersCat.tasksComplete(CachedPlayer.LocalPlayer.PlayerControl) ? schrodingersCat : crewmate);
            if (p == Opportunist.opportunist) infos.Add(opportunist);
            if (p == Shifter.shifter) infos.Add(Shifter.isNeutral ? chainshifter : niceshifter);
            if (p == Arsonist.arsonist) infos.Add(arsonist);
            if (p == Guesser.niceGuesser) infos.Add(goodGuesser);
            if (p == Guesser.evilGuesser) infos.Add(badGuesser);
            if (p == Watcher.nicewatcher) infos.Add(niceWatcher);
            if (p == Watcher.evilwatcher) infos.Add(evilWatcher);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Ninja.ninja) infos.Add(ninja);
            if (p == NekoKabocha.nekoKabocha) infos.Add(nekoKabocha);
            if (p == SerialKiller.serialKiller) infos.Add(serialKiller);
            if (p == EvilTracker.evilTracker) infos.Add(evilTracker);
            if (p == EvilHacker.evilHacker) infos.Add(evilHacker);
            if (p == Undertaker.undertaker) infos.Add(undertaker);
            if (p == Trapper.trapper) infos.Add(trapper);
            if (p == MimicK.mimicK) infos.Add(mimicK);
            if (p == MimicA.mimicA) infos.Add(mimicA);
            if (p == BomberA.bomberA) infos.Add(bomberA);
            if (p == BomberB.bomberB) infos.Add(bomberB);
            if (p == Vulture.vulture) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);
            if (p == Lawyer.lawyer) infos.Add(lawyer); // && !Lawyer.isProsecutor
            //if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
            //if (p == Trapper.trapper) infos.Add(trapper);
            if (p == Pursuer.pursuer) infos.Add(pursuer);
            if (p == Thief.thief) infos.Add(thief);

            // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek
            if (infos.Count == count) {
                if (p.Data.Role.IsImpostor)
                    infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ? RoleInfo.hunter : RoleInfo.impostor);
                else
                    infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ? RoleInfo.hunted : RoleInfo.crewmate);
            }

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, bool suppressGhostInfo = false, bool includeHidden = false) {
            string roleName;
            roleName = String.Join(" ", getRoleInfoForPlayer(p, showModifier, includeHidden).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());

            if (Madmate.madmate.Any(x => x.PlayerId == p.PlayerId) || CreatedMadmate.createdMadmate == p)
            {
                if (getRoleInfoForPlayer(p, true, includeHidden).Contains(crewmate))
                {
                    roleName = useColors ? Helpers.cs(Madmate.color, Madmate.fullName) : Madmate.fullName;
                    if (showModifier && getRoleInfoForPlayer(p, true, includeHidden).Where(x => x.isModifier).FirstOrDefault() != null) roleName = string.Join(" ", getRoleInfoForPlayer(p, true, includeHidden).Where(x => x.isModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray()) + " " + roleName;
                }
                else
                {
                    string prefix = useColors ? Helpers.cs(Madmate.color, Madmate.prefix) : Madmate.prefix;
                    roleName = string.Join(" ", getRoleInfoForPlayer(p, false, includeHidden).Select(x => useColors ? Helpers.cs(Madmate.color, x.name) : x.name).ToArray());
                    roleName = prefix + roleName;
                    if (showModifier && getRoleInfoForPlayer(p, true, includeHidden).Where(x => x.isModifier).FirstOrDefault() != null) roleName = string.Join(" ", getRoleInfoForPlayer(p, true, includeHidden).Where(x => x.isModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray()) + " " + roleName;
                }
            }

            if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target) 
                roleName += useColors ? Helpers.cs(Pursuer.color, " §") : " §";
            if (Husk.husk.Any(x => x.PlayerId == p.PlayerId)) roleName += $" ({ModTranslation.getString("husk")})";
            if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += ModTranslation.getString("guesserModifier");            

            if (!suppressGhostInfo && p != null) {
                if (p == Shifter.shifter && (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || Helpers.shouldShowGhostInfo()) && Shifter.futureShift != null)
                    roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
                if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                    roleName = roleName + Helpers.cs(Vulture.color, $" ({Vulture.vultureNumberToWin - Vulture.eatenBodies} {ModTranslation.getString("roleInfoRemaining")})");
                if (Helpers.shouldShowGhostInfo()) {
                    if (Eraser.futureErased.Contains(p))
                        roleName = Helpers.cs(Color.gray, ModTranslation.getString("roleInfoErased")) + roleName;
                    if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                        roleName = Helpers.cs(Vampire.color, $"({ModTranslation.getString("roleInfoBitten")} {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                    if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.cs(Color.gray, ModTranslation.getString("roleInfoCuffed")) + roleName;
                    if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId))  // Active cuff
                        roleName = Helpers.cs(Deputy.color, ModTranslation.getString("roleInfoCuffed")) + roleName;
                    if (p == Warlock.curseVictim)
                        roleName = Helpers.cs(Warlock.color, ModTranslation.getString("roleInfoCursed")) + roleName;
                    if (p == Assassin.assassinMarked)
                        roleName = Helpers.cs(Assassin.color, ModTranslation.getString("roleInfoMarked")) + roleName;
                    if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                        roleName = Helpers.cs(Pursuer.color, ModTranslation.getString("roleInfoBlanked")) + roleName;
                    if (p == FortuneTeller.divineTarget)
                        roleName = Helpers.cs(FortuneTeller.color, ModTranslation.getString("roleInfoDivined")) + roleName;
                    if (Prophet.examined.ContainsKey(p))
                        roleName = Helpers.cs(Prophet.color, ModTranslation.getString("roleInfoExamined")) + roleName;
                    if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                        roleName = Helpers.cs(Witch.color, "☆ ") + roleName;
                    if (BountyHunter.bounty == p)
                        roleName = Helpers.cs(BountyHunter.color, ModTranslation.getString("roleInfoBounty")) + roleName;
                    if (Arsonist.dousedPlayers.Contains(p))
                        roleName = Helpers.cs(Arsonist.color, "♨ ") + roleName;
                    if (p == Arsonist.arsonist)
                        roleName = roleName + Helpers.cs(Arsonist.color, $" ({CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} {ModTranslation.getString("roleInfoRemaining")})");
                    if (p == Jackal.fakeSidekick)
                        roleName = Helpers.cs(Sidekick.color, ModTranslation.getString("roleInfoFakeSD")) + roleName;
                    if (Akujo.keeps.Contains(p))
                        roleName = Helpers.cs(Color.gray, ModTranslation.getString("roleInfoBackup")) + roleName;
                    if (p == Akujo.honmei)
                        roleName = Helpers.cs(Akujo.color, ModTranslation.getString("roleInfoHonmei")) + roleName;
                    if (p == Kataomoi.target)
                        roleName = Helpers.cs(Kataomoi.color, ModTranslation.getString("roleInfoKataomoiTarget")) + roleName;

                    // Death Reason on Ghosts
                    if (p.Data.IsDead) {
                        string deathReasonString = "";
                        var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                        Color killerColor = new();
                        if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                            killerColor = RoleInfo.getRoleInfoForPlayer(deadPlayer.killerIfExisting, false, true).FirstOrDefault().color;
                            if (Madmate.madmate.Any(x => x.PlayerId == deadPlayer.killerIfExisting.PlayerId)) killerColor = Palette.ImpostorRed;
                        }

                        if (deadPlayer != null) {
                            switch (deadPlayer.deathReason) {
                                case DeadPlayer.CustomDeathReason.Disconnect:
                                    deathReasonString = ModTranslation.getString("roleSummaryDisconnected");
                                    break;
                                case DeadPlayer.CustomDeathReason.Exile:
                                    deathReasonString = ModTranslation.getString("roleSummaryExiled");
                                    break;
                                case DeadPlayer.CustomDeathReason.Kill:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryKilled"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Guess:
                                    if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                        deathReasonString = ModTranslation.getString("roleSummaryFailedGuess");
                                    else
                                        deathReasonString = string.Format(ModTranslation.getString("roleSummaryGuess"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Shift:
                                    deathReasonString = $" - {Helpers.cs(Color.yellow, ModTranslation.getString("roleSummaryShift"))} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.WitchExile:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummarySpelled"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.LoverSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lovers.color, ModTranslation.getString("roleSummaryLoverDied"))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Revenge:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryRevenge"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Suicide:
                                    deathReasonString = ModTranslation.getString("roleSummarySuicide");
                                    break;
                                case DeadPlayer.CustomDeathReason.KataomoiStare:
                                    deathReasonString = $" - {Helpers.cs(Kataomoi.color, ModTranslation.getString("roleSummaryKataomoiStare"))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.BrainwashedKilled:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryBrainwashedKilled"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.LoveStolen:
                                    deathReasonString = $" - {Helpers.cs(Lovers.color, ModTranslation.getString("roleSummaryLoveStolen"))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Loneliness:
                                    deathReasonString = $" - {Helpers.cs(Akujo.color, ModTranslation.getString("roleSummaryLoneliness"))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Disease:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryDisease"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Scapegoat:
                                    deathReasonString = $" - {Helpers.cs(Cupid.color, ModTranslation.getString("roleSummaryScapegoat"))}";
                                    break;
                                //case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                //deathReasonString = $" - {Helpers.cs(Lawyer.color, "bad Lawyer")}";
                                //break;
                                case DeadPlayer.CustomDeathReason.Bomb:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryBombed"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Divined:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryDivined"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                                case DeadPlayer.CustomDeathReason.Pseudocide:
                                    deathReasonString = $" - {Helpers.cs(Busker.color, ModTranslation.getString("roleSummaryPseudocide"))}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Arson:
                                    deathReasonString = string.Format(ModTranslation.getString("roleSummaryTorched"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                    break;
                            }
                            roleName = roleName + deathReasonString;
                        }
                    }
                }
            }
            return roleName;
        }
    }
}
