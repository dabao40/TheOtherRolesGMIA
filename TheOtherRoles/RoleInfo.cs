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
        public string name;
        public string introDescription;
        public string shortDescription;
        public RoleId roleId;
        public bool isNeutral;
        public bool isModifier;

        public RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId, bool isNeutral = false, bool isModifier = false) {
            this.color = color;
            this.name = name;
            this.introDescription = introDescription;
            this.shortDescription = shortDescription;
            this.roleId = roleId;
            this.isNeutral = isNeutral;
            this.isModifier = isModifier;
        }

        public static RoleInfo jester = new RoleInfo(ModTranslation.getString("jester"), Jester.color, ModTranslation.getString("jesterIntroDesc"), ModTranslation.getString("jesterShortDesc"), RoleId.Jester, true);
        public static RoleInfo mayor = new RoleInfo(ModTranslation.getString("mayor"), Mayor.color, ModTranslation.getString("mayorIntroDesc"), ModTranslation.getString("mayorShortDesc"), RoleId.Mayor);
        public static RoleInfo portalmaker = new RoleInfo(ModTranslation.getString("portalmaker"), Portalmaker.color, ModTranslation.getString("portalmakerIntroDesc"), ModTranslation.getString("portalmakerShortDesc"), RoleId.Portalmaker);
        public static RoleInfo engineer = new RoleInfo(ModTranslation.getString("engineer"),  Engineer.color, ModTranslation.getString("engineerIntroDesc"), ModTranslation.getString("engineerShortDesc"), RoleId.Engineer);
        public static RoleInfo sheriff = new RoleInfo(ModTranslation.getString("sheriff"), Sheriff.color, ModTranslation.getString("sheriffIntroDesc"), ModTranslation.getString("sheriffShortDesc"), RoleId.Sheriff);
        public static RoleInfo deputy = new RoleInfo(ModTranslation.getString("deputy"), Sheriff.color, ModTranslation.getString("deputyIntroDesc"), ModTranslation.getString("deputyShortDesc"), RoleId.Deputy);
        public static RoleInfo lighter = new RoleInfo(ModTranslation.getString("lighter"), Lighter.color, ModTranslation.getString("lighterIntroDesc"), ModTranslation.getString("lighterShortDesc"), RoleId.Lighter);
        public static RoleInfo godfather = new RoleInfo(ModTranslation.getString("godfather"), Godfather.color, ModTranslation.getString("godfatherIntroDesc"), ModTranslation.getString("godfatherShortDesc"), RoleId.Godfather);
        public static RoleInfo mafioso = new RoleInfo(ModTranslation.getString("mafioso"), Mafioso.color, ModTranslation.getString("mafiosoIntroDesc"), ModTranslation.getString("mafiosoShortDesc"), RoleId.Mafioso);
        public static RoleInfo janitor = new RoleInfo(ModTranslation.getString("janitor"), Janitor.color, ModTranslation.getString("janitorIntroDesc"), ModTranslation.getString("janitorShortDesc"), RoleId.Janitor);
        public static RoleInfo morphling = new RoleInfo(ModTranslation.getString("morphling"), Morphling.color, ModTranslation.getString("morphlingIntroDesc"), ModTranslation.getString("morphlingShortDesc"), RoleId.Morphling);
        public static RoleInfo camouflager = new RoleInfo(ModTranslation.getString("camouflager"), Camouflager.color, ModTranslation.getString("camouflagerIntroDesc"), ModTranslation.getString("camouflagerShortDesc"), RoleId.Camouflager);
        public static RoleInfo vampire = new RoleInfo(ModTranslation.getString("vampire"), Vampire.color, ModTranslation.getString("vampireIntroDesc"), ModTranslation.getString("vampireShortDesc"), RoleId.Vampire);
        public static RoleInfo eraser = new RoleInfo(ModTranslation.getString("eraser"), Eraser.color, ModTranslation.getString("eraserIntroDesc"), ModTranslation.getString("eraserShortDesc"), RoleId.Eraser);
        public static RoleInfo trickster = new RoleInfo(ModTranslation.getString("trickster"), Trickster.color, ModTranslation.getString("tricksterIntroDesc"), ModTranslation.getString("tricksterShortDesc"), RoleId.Trickster);
        public static RoleInfo cleaner = new RoleInfo(ModTranslation.getString("cleaner"), Cleaner.color, ModTranslation.getString("cleanerIntroDesc"), ModTranslation.getString("cleanerShortDesc"), RoleId.Cleaner);
        public static RoleInfo warlock = new RoleInfo(ModTranslation.getString("warlock"), Warlock.color, ModTranslation.getString("warlockIntroDesc"), ModTranslation.getString("warlockShortDesc"), RoleId.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo(ModTranslation.getString("bountyHunter"), BountyHunter.color, ModTranslation.getString("bountyHunterIntroDesc"), ModTranslation.getString("bountyHunterShortDesc"), RoleId.BountyHunter);
        public static RoleInfo detective = new RoleInfo(ModTranslation.getString("detective"), Detective.color, ModTranslation.getString("detectiveIntroDesc"), ModTranslation.getString("detectiveShortDesc"), RoleId.Detective);
        public static RoleInfo bait = new RoleInfo(ModTranslation.getString("bait"), Bait.color, ModTranslation.getString("baitIntroDesc"), ModTranslation.getString("baitShortDesc"), RoleId.Bait);
        public static RoleInfo timeMaster = new RoleInfo(ModTranslation.getString("timeMaster"), TimeMaster.color, ModTranslation.getString("timeMasterIntroDesc"), ModTranslation.getString("timeMasterShortDesc"), RoleId.TimeMaster);
        public static RoleInfo medic = new RoleInfo(ModTranslation.getString("medic"), Medic.color, ModTranslation.getString("medicIntroDesc"), ModTranslation.getString("medicShortDesc"), RoleId.Medic);
        public static RoleInfo niceSwapper = new RoleInfo(ModTranslation.getString("niceSwapper"), Swapper.color, ModTranslation.getString("niceSwapperIntroDesc"), ModTranslation.getString("niceSwapperShortDesc"), RoleId.Swapper);
        public static RoleInfo seer = new RoleInfo(ModTranslation.getString("seer"), Seer.color, ModTranslation.getString("seerIntroDesc"), ModTranslation.getString("seerShortDesc"), RoleId.Seer);
        public static RoleInfo hacker = new RoleInfo(ModTranslation.getString("hacker"), Hacker.color, ModTranslation.getString("hackerIntroDesc"), ModTranslation.getString("hackerShortDesc"), RoleId.Hacker);
        public static RoleInfo niceshifter = new RoleInfo(ModTranslation.getString("niceShifter"), Shifter.color, ModTranslation.getString("shifterIntroDesc"), ModTranslation.getString("shifterShortDesc"), RoleId.Shifter);
        public static RoleInfo tracker = new RoleInfo(ModTranslation.getString("tracker"), Tracker.color, ModTranslation.getString("trackerIntroDesc"), ModTranslation.getString("trackerShortDesc"), RoleId.Tracker);
        public static RoleInfo snitch = new RoleInfo(ModTranslation.getString("snitch"), Snitch.color, ModTranslation.getString("snitchIntroDesc"), ModTranslation.getString("snitchShortDesc"), RoleId.Snitch);
        public static RoleInfo jackal = new RoleInfo(ModTranslation.getString("jackal"), Jackal.color, ModTranslation.getString("jackalIntroDesc"), ModTranslation.getString("jackalShortDesc"), RoleId.Jackal, true);
        public static RoleInfo sidekick = new RoleInfo(ModTranslation.getString("sidekick"), Sidekick.color, ModTranslation.getString("sidekickIntroDesc"), ModTranslation.getString("sidekickShortDesc"), RoleId.Sidekick, true);
        public static RoleInfo spy = new RoleInfo(ModTranslation.getString("spy"), Spy.color, ModTranslation.getString("spyIntroDesc"), ModTranslation.getString("spyShortDesc"), RoleId.Spy);
        public static RoleInfo securityGuard = new RoleInfo(ModTranslation.getString("securityGuard"), SecurityGuard.color, ModTranslation.getString("securityGuardIntroDesc"), ModTranslation.getString("securityGuardShortDesc"), RoleId.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo(ModTranslation.getString("arsonist"), Arsonist.color, ModTranslation.getString("arsonistIntroDesc"), ModTranslation.getString("arsonistShortDesc"), RoleId.Arsonist, true);
        public static RoleInfo goodGuesser = new RoleInfo(ModTranslation.getString("niceGuesser"), Guesser.color, ModTranslation.getString("niceGuesserIntroDesc"), ModTranslation.getString("niceGuesserShortDesc"), RoleId.NiceGuesser);
        public static RoleInfo badGuesser = new RoleInfo(ModTranslation.getString("evilGuesser"), Palette.ImpostorRed, ModTranslation.getString("evilGuesserIntroDesc"), ModTranslation.getString("evilGuesserShortDesc"), RoleId.EvilGuesser);
        public static RoleInfo niceWatcher = new RoleInfo(ModTranslation.getString("niceWatcher"), Watcher.color, ModTranslation.getString("niceWatcherIntroDesc"), ModTranslation.getString("niceWatcherShortDesc"), RoleId.NiceWatcher);
        public static RoleInfo evilWatcher = new RoleInfo(ModTranslation.getString("evilWatcher"), Palette.ImpostorRed, ModTranslation.getString("evilWatcherIntroDesc"), ModTranslation.getString("evilWatcherShortDesc"), RoleId.EvilWatcher);
        public static RoleInfo vulture = new RoleInfo(ModTranslation.getString("vulture"), Vulture.color, ModTranslation.getString("vultureIntroDesc"), ModTranslation.getString("vultureShortDesc"), RoleId.Vulture, true);
        public static RoleInfo medium = new RoleInfo(ModTranslation.getString("medium"), Medium.color, ModTranslation.getString("mediumIntroDesc"), ModTranslation.getString("mediumShortDesc"), RoleId.Medium);
        //public static RoleInfo trapper = new RoleInfo("Trapper", Trapper.color, "Place traps to find the Impostors", "Place traps", RoleId.Trapper);
        public static RoleInfo lawyer = new RoleInfo(ModTranslation.getString("lawyer"), Lawyer.color, ModTranslation.getString("lawyerIntroDesc"), ModTranslation.getString("lawyerShortDesc"), RoleId.Lawyer, true);
        //public static RoleInfo prosecutor = new RoleInfo("Prosecutor", Lawyer.color, "Vote out your target", "Vote out your target", RoleId.Prosecutor, true);
        public static RoleInfo pursuer = new RoleInfo(ModTranslation.getString("pursuer"), Pursuer.color, ModTranslation.getString("pursuerIntroDesc"), ModTranslation.getString("pursuerShortDesc"), RoleId.Pursuer, true);
        public static RoleInfo impostor = new RoleInfo(ModTranslation.getString("impostor"), Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, ModTranslation.getString("impostorIntroDesc")), ModTranslation.getString("impostorShortDesc"), RoleId.Impostor);
        public static RoleInfo crewmate = new RoleInfo(ModTranslation.getString("crewmate"), Color.white, ModTranslation.getString("crewmateIntroDesc"), ModTranslation.getString("crewmateShortDesc"), RoleId.Crewmate);
        public static RoleInfo witch = new RoleInfo(ModTranslation.getString("witch"), Witch.color, ModTranslation.getString("witchIntroDesc"), ModTranslation.getString("witchShortDesc"), RoleId.Witch);
        public static RoleInfo assassin = new RoleInfo(ModTranslation.getString("assassin"), Assassin.color, ModTranslation.getString("assassinIntroDesc"), ModTranslation.getString("assassinShortDesc"), RoleId.Assassin);
        public static RoleInfo thief = new RoleInfo(ModTranslation.getString("thief"), Thief.color, ModTranslation.getString("thiefIntroDesc"), ModTranslation.getString("thiefShortDesc"), RoleId.Thief, true);
        //public static RoleInfo bomber = new RoleInfo("Bomber", Bomber.color, "Bomb all Crewmates", "Bomb all Crewmates", RoleId.Bomber);

        // GMIA functional
        public static RoleInfo ninja = new RoleInfo(ModTranslation.getString("ninja"), Ninja.color, ModTranslation.getString("ninjaIntroDesc"), ModTranslation.getString("ninjaShortDesc"), RoleId.Ninja);
        public static RoleInfo nekoKabocha = new RoleInfo(ModTranslation.getString("nekoKabocha"), NekoKabocha.color, ModTranslation.getString("nekoKabochaIntroDesc"), ModTranslation.getString("nekoKabochaShortDesc"), RoleId.NekoKabocha);
        public static RoleInfo serialKiller = new RoleInfo(ModTranslation.getString("serialKiller"), SerialKiller.color, ModTranslation.getString("serialKillerIntroDesc"), ModTranslation.getString("serialKillerShortDesc"), RoleId.SerialKiller);
        public static RoleInfo evilTracker = new RoleInfo(ModTranslation.getString("evilTracker"), EvilTracker.color, ModTranslation.getString("evilTrackerIntroDesc"), ModTranslation.getString("evilTrackerShortDesc"), RoleId.EvilTracker);
        public static RoleInfo undertaker = new RoleInfo(ModTranslation.getString("undertaker"), Undertaker.color, ModTranslation.getString("undertakerIntroDesc"), ModTranslation.getString("undertakerShortDesc"), RoleId.Undertaker);
        public static RoleInfo mimicK = new RoleInfo(ModTranslation.getString("mimicK"), MimicK.color, ModTranslation.getString("mimicKIntroDesc"), ModTranslation.getString("mimicKShortDesc"), RoleId.MimicK);
        public static RoleInfo mimicA = new RoleInfo(ModTranslation.getString("mimicA"), MimicA.color, ModTranslation.getString("mimicAIntroDesc"), ModTranslation.getString("mimicAShortDesc"), RoleId.MimicA);
        public static RoleInfo bomberA = new RoleInfo(ModTranslation.getString("bomber"), BomberA.color, ModTranslation.getString("bomberIntroDesc"), ModTranslation.getString("bomberShortDesc"), RoleId.BomberA);
        public static RoleInfo bomberB = new RoleInfo(ModTranslation.getString("bomber"), BomberB.color, ModTranslation.getString("bomberIntroDesc"), ModTranslation.getString("bomberShortDesc"), RoleId.BomberB);
        public static RoleInfo evilSwapper = new RoleInfo(ModTranslation.getString("evilSwapper"), Palette.ImpostorRed, ModTranslation.getString("evilSwapperIntroDesc"), ModTranslation.getString("evilSwapperShortDesc"), RoleId.Swapper);
        public static RoleInfo evilHacker = new RoleInfo(ModTranslation.getString("evilHacker"), EvilHacker.color, ModTranslation.getString("evilHackerIntroDesc"), ModTranslation.getString("evilHackerShortDesc"), RoleId.EvilHacker);
        public static RoleInfo trapper = new RoleInfo(ModTranslation.getString("trapper"), Trapper.color, ModTranslation.getString("trapperIntroDesc"), ModTranslation.getString("trapperShortDesc"), RoleId.Trapper);
        public static RoleInfo blackmailer = new RoleInfo(ModTranslation.getString("blackmailer"), Blackmailer.color, ModTranslation.getString("blackmailerIntroDesc"), ModTranslation.getString("blackmailerShortDesc"), RoleId.Blackmailer);
        public static RoleInfo fortuneTeller = new RoleInfo(ModTranslation.getString("fortuneTeller"), FortuneTeller.color, ModTranslation.getString("fortuneTellerIntroDesc"), ModTranslation.getString("fortuneTellerShortDesc"), RoleId.FortuneTeller);
        public static RoleInfo veteran = new RoleInfo(ModTranslation.getString("veteran"), Veteran.color, ModTranslation.getString("veteranIntroDesc"), ModTranslation.getString("veteranShortDesc"), RoleId.Veteran);
        public static RoleInfo sprinter = new RoleInfo(ModTranslation.getString("sprinter"), Sprinter.color, ModTranslation.getString("sprinterIntroDesc"), ModTranslation.getString("sprinterShortDesc"), RoleId.Sprinter);
        public static RoleInfo sherlock = new RoleInfo(ModTranslation.getString("sherlock"), Sherlock.color, ModTranslation.getString("sherlockIntroDesc"), ModTranslation.getString("sherlockShortDesc"), RoleId.Sherlock);
        public static RoleInfo yasuna = new RoleInfo(ModTranslation.getString("niceYasuna"), Yasuna.color, ModTranslation.getString("niceYasunaIntroDesc"), ModTranslation.getString("niceYasunaShortDesc"), RoleId.Yasuna);
        public static RoleInfo taskMaster = new RoleInfo(ModTranslation.getString("taskMaster"), TaskMaster.color, ModTranslation.getString("taskMasterIntroDesc"), ModTranslation.getString("taskMasterShortDesc"), RoleId.TaskMaster);
        public static RoleInfo teleporter = new RoleInfo(ModTranslation.getString("teleporter"), Teleporter.color, ModTranslation.getString("teleporterIntroDesc"), ModTranslation.getString("teleporterShortDesc"), RoleId.Teleporter);
        public static RoleInfo prophet = new RoleInfo(ModTranslation.getString("prophet"), Prophet.color, ModTranslation.getString("prophetIntroDesc"), ModTranslation.getString("prophetShortDesc"), RoleId.Prophet);
        public static RoleInfo evilYasuna = new RoleInfo(ModTranslation.getString("evilYasuna"), Palette.ImpostorRed, ModTranslation.getString("evilYasunaIntroDesc"), ModTranslation.getString("evilYasunaShortDesc"), RoleId.EvilYasuna);
        public static RoleInfo opportunist = new RoleInfo(ModTranslation.getString("opportunist"), Opportunist.color, ModTranslation.getString("opportunistIntroDesc"), ModTranslation.getString("opportunistShortDesc"), RoleId.Opportunist, true);
        public static RoleInfo chainshifter = new RoleInfo(ModTranslation.getString("corruptedShifter"), Shifter.color, ModTranslation.getString("corruptedShifterIntroDesc"), ModTranslation.getString("corruptedShifterShortDesc"), RoleId.Shifter, true);
        public static RoleInfo moriarty = new RoleInfo(ModTranslation.getString("moriarty"), Moriarty.color, ModTranslation.getString("moriartyIntroDesc"), ModTranslation.getString("moriartyShortDesc"), RoleId.Moriarty, true);
        public static RoleInfo akujo = new RoleInfo(ModTranslation.getString("akujo"), Akujo.color, ModTranslation.getString("akujoIntroDesc"), ModTranslation.getString("akujoShortDesc"), RoleId.Akujo, true);
        public static RoleInfo plagueDoctor = new RoleInfo(ModTranslation.getString("plagueDoctor"), PlagueDoctor.color, ModTranslation.getString("plagueDoctorIntroDesc"), ModTranslation.getString("plagueDoctorShortDesc"), RoleId.PlagueDoctor, true);
        public static RoleInfo jekyllAndHyde = new RoleInfo(ModTranslation.getString("jekyllAndHyde"), JekyllAndHyde.color, ModTranslation.getString("jekyllAndHydeIntroDesc"), ModTranslation.getString("jekyllAndHydeShortDesc"), RoleId.JekyllAndHyde, true);
        public static RoleInfo cupid = new RoleInfo(ModTranslation.getString("cupid"), Cupid.color, ModTranslation.getString("cupidIntroDesc"), ModTranslation.getString("cupidShortDesc"), RoleId.Cupid, true);
        public static RoleInfo cupidLover = new RoleInfo(ModTranslation.getString("lover"), Cupid.color, ModTranslation.getString("loversIntroDesc"), ModTranslation.getString("loversShortDesc"), RoleId.Lover, false, true);

        public static RoleInfo hunter = new RoleInfo(ModTranslation.getString("hunter"), Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, ModTranslation.getString("hunterIntroDesc")), ModTranslation.getString("hunterShortDesc"), RoleId.Impostor);
        public static RoleInfo hunted = new RoleInfo(ModTranslation.getString("hunted"), Color.white, ModTranslation.getString("huntedIntroDesc"), ModTranslation.getString("huntedShortDesc"), RoleId.Crewmate);



        // Modifier
        public static RoleInfo bloody = new RoleInfo(ModTranslation.getString("bloody"), Color.yellow, ModTranslation.getString("bloodyIntroDesc"), ModTranslation.getString("bloodyShortDesc"), RoleId.Bloody, false, true);
        public static RoleInfo antiTeleport = new RoleInfo(ModTranslation.getString("antiTeleportPostfix"), Color.yellow, ModTranslation.getString("antiTeleportIntroDesc"), ModTranslation.getString("antiTeleportShortDesc"), RoleId.AntiTeleport, false, true);
        public static RoleInfo tiebreaker = new RoleInfo(ModTranslation.getString("tiebreaker"), Color.yellow, ModTranslation.getString("tiebreakerIntroDesc"), ModTranslation.getString("tiebreakerShortDesc"), RoleId.Tiebreaker, false, true);
        //public static RoleInfo bait = new RoleInfo("Bait", Color.yellow, "Bait your enemies", "Bait your enemies", RoleId.Bait, false, true);
        public static RoleInfo sunglasses = new RoleInfo(ModTranslation.getString("sunglasses"), Color.yellow, ModTranslation.getString("sunglassesIntroDesc"), ModTranslation.getString("sunglassesShortDesc"), RoleId.Sunglasses, false, true);
        public static RoleInfo lover = new RoleInfo(ModTranslation.getString("lover"), Lovers.color, ModTranslation.getString("loversIntroDesc"), ModTranslation.getString("loversShortDesc"), RoleId.Lover, false, true);
        public static RoleInfo mini = new RoleInfo(ModTranslation.getString("mini"), Color.yellow, ModTranslation.getString("miniIntroDesc"), ModTranslation.getString("miniShortDesc"), RoleId.Mini, false, true);
        public static RoleInfo vip = new RoleInfo(ModTranslation.getString("vip"), Color.yellow, ModTranslation.getString("vipIntroDesc"), ModTranslation.getString("vipShortDesc"), RoleId.Vip, false, true);
        public static RoleInfo invert = new RoleInfo(ModTranslation.getString("invert"), Color.yellow, ModTranslation.getString("invertIntroDesc"), ModTranslation.getString("invertShortDesc"), RoleId.Invert, false, true);
        public static RoleInfo chameleon = new RoleInfo(ModTranslation.getString("chameleon"), Color.yellow, ModTranslation.getString("chameleonIntroDesc"), ModTranslation.getString("chameleonShortDesc"), RoleId.Chameleon, false, true);
        //public static RoleInfo shifter = new RoleInfo("Shifter", Color.yellow, "Shift your role", "Shift your role", RoleId.Shifter, false, true);


        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
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
            akujo,
            jekyllAndHyde,
            moriarty,
            cupid,
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
            List<RoleInfo> infos = new List<RoleInfo>();
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
                roleName += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
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
