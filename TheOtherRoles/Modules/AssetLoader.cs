using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using TheOtherRoles.Objects;
using UnityEngine;
using UnityEngine.UI;
using Reactor.Utilities.Extensions;
using Il2CppSystem;

namespace TheOtherRoles.Modules
{
    public static class AssetLoader
    {
        private static readonly Assembly dll = Assembly.GetExecutingAssembly();
        private static bool flag = false;

        public static void LoadAssets()
        {
            if (flag) return;
            flag = true;
            LoadAudioAssets();
            LoadSpriteAssets();
            LoadShaderAssets();
        }

        private static void LoadAudioAssets()
        {
            var resourceAudioAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.audiobundle");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceAudioAssetBundleStream.ReadFully());
            Trap.activate = assetBundleBundle.LoadAsset<AudioClip>("TrapperActivate.mp3").DontUnload();
            Trap.countdown = assetBundleBundle.LoadAsset<AudioClip>("TrapperCountdown.mp3").DontUnload();
            Trap.disable = assetBundleBundle.LoadAsset<AudioClip>("TrapperDisable.mp3").DontUnload();
            Trap.kill = assetBundleBundle.LoadAsset<AudioClip>("TrapperKill.mp3").DontUnload();
            Trap.place = assetBundleBundle.LoadAsset<AudioClip>("TrapperPlace.mp3").DontUnload();

            SoundEffects.arsonistDouse = assetBundleBundle.LoadAsset<AudioClip>("arsonistDouse.mp3").DontUnload();
            SoundEffects.blackmailerSilence = assetBundleBundle.LoadAsset<AudioClip>("blackmailerSilence.mp3").DontUnload();
            SoundEffects.bombDefused = assetBundleBundle.LoadAsset<AudioClip>("bombDefused.mp3").DontUnload();
            SoundEffects.bomberPlantBomb = assetBundleBundle.LoadAsset<AudioClip>("bomberPlantBomb.mp3").DontUnload();
            SoundEffects.bombExplosion = assetBundleBundle.LoadAsset<AudioClip>("bombExplosion.mp3").DontUnload();
            SoundEffects.bombFuseBurning = assetBundleBundle.LoadAsset<AudioClip>("bombFuseBurning.mp3").DontUnload();
            SoundEffects.bombTick = assetBundleBundle.LoadAsset<AudioClip>("bombTick.mp3").DontUnload();
            SoundEffects.cleanerClean = assetBundleBundle.LoadAsset<AudioClip>("cleanerClean.mp3").DontUnload();
            SoundEffects.deputyHandcuff = assetBundleBundle.LoadAsset<AudioClip>("deputyHandcuff.mp3").DontUnload();
            SoundEffects.engineerRepair = assetBundleBundle.LoadAsset<AudioClip>("engineerRepair.mp3").DontUnload();
            SoundEffects.eraserErase = assetBundleBundle.LoadAsset<AudioClip>("eraserErase.mp3").DontUnload();
            SoundEffects.fail = assetBundleBundle.LoadAsset<AudioClip>("fail.mp3").DontUnload();
            SoundEffects.garlic = assetBundleBundle.LoadAsset<AudioClip>("garlic.mp3").DontUnload();
            SoundEffects.hackerHack = assetBundleBundle.LoadAsset<AudioClip>("hackerHack.mp3").DontUnload();
            SoundEffects.jackalSidekick = assetBundleBundle.LoadAsset<AudioClip>("jackalSidekick.mp3").DontUnload();
            SoundEffects.jekyllAndHydeDrug = assetBundleBundle.LoadAsset<AudioClip>("jekyllAndHydeDrug.mp3").DontUnload();
            SoundEffects.knockKnock = assetBundleBundle.LoadAsset<AudioClip>("knockKnock.mp3").DontUnload();
            SoundEffects.lighterLight = assetBundleBundle.LoadAsset<AudioClip>("lighterLight.mp3").DontUnload();
            SoundEffects.medicShield = assetBundleBundle.LoadAsset<AudioClip>("medicShield.mp3").DontUnload();
            SoundEffects.mediumAsk = assetBundleBundle.LoadAsset<AudioClip>("mediumAsk.mp3").DontUnload();
            SoundEffects.moriartyBrainwash = assetBundleBundle.LoadAsset<AudioClip>("moriartyBrainwash.mp3").DontUnload();
            SoundEffects.morphlingMorph = assetBundleBundle.LoadAsset<AudioClip>("morphlingMorph.mp3").DontUnload();
            SoundEffects.morphlingSample = assetBundleBundle.LoadAsset<AudioClip>("morphlingSample.mp3").DontUnload();
            SoundEffects.ninjaStealth = assetBundleBundle.LoadAsset<AudioClip>("ninjaStealth.mp3").DontUnload();
            SoundEffects.plagueDoctorSyringe = assetBundleBundle.LoadAsset<AudioClip>("plagueDoctorSyringe.mp3").DontUnload();
            SoundEffects.portalUse = assetBundleBundle.LoadAsset<AudioClip>("portalUse.mp3").DontUnload();
            SoundEffects.pursuerBlank = assetBundleBundle.LoadAsset<AudioClip>("pursuerBlank.mp3").DontUnload();
            SoundEffects.securityGuardPlaceCam = assetBundleBundle.LoadAsset<AudioClip>("securityGuardPlaceCam.mp3").DontUnload();
            SoundEffects.sherlockInvestigate = assetBundleBundle.LoadAsset<AudioClip>("sherlockInvestigate.mp3").DontUnload();
            SoundEffects.shifterShift = assetBundleBundle.LoadAsset<AudioClip>("shifterShift.mp3").DontUnload();
            SoundEffects.teleporterSample = assetBundleBundle.LoadAsset<AudioClip>("teleporterSample.mp3").DontUnload();
            SoundEffects.teleporterTeleport = assetBundleBundle.LoadAsset<AudioClip>("teleporterTeleport.mp3").DontUnload();
            SoundEffects.timemasterShield = assetBundleBundle.LoadAsset<AudioClip>("timemasterShield.mp3").DontUnload();
            SoundEffects.trackerTrackCorpses = assetBundleBundle.LoadAsset<AudioClip>("trackerTrackCorpses.mp3").DontUnload();
            SoundEffects.trackerTrackPlayer = assetBundleBundle.LoadAsset<AudioClip>("trackerTrackPlayer.mp3").DontUnload();
            SoundEffects.trapperTrap = assetBundleBundle.LoadAsset<AudioClip>("trapperTrap.mp3").DontUnload();
            SoundEffects.tricksterPlaceBox = assetBundleBundle.LoadAsset<AudioClip>("tricksterPlaceBox.mp3").DontUnload();
            SoundEffects.tricksterUseBoxVent = assetBundleBundle.LoadAsset<AudioClip>("tricksterUseBoxVent.mp3").DontUnload();
            SoundEffects.triggerDeceleration = assetBundleBundle.LoadAsset<AudioClip>("triggerDeceleration.mp3").DontUnload();
            SoundEffects.untriggerDeceleration = assetBundleBundle.LoadAsset<AudioClip>("untriggerDeceleration.mp3").DontUnload();
            SoundEffects.vampireBite = assetBundleBundle.LoadAsset<AudioClip>("vampireBite.mp3").DontUnload();
            SoundEffects.veteranAlert = assetBundleBundle.LoadAsset<AudioClip>("veteranAlert.mp3").DontUnload();
            SoundEffects.vultureEat = assetBundleBundle.LoadAsset<AudioClip>("vultureEat.mp3").DontUnload();
            SoundEffects.warlockCurse = assetBundleBundle.LoadAsset<AudioClip>("warlockCurse.mp3").DontUnload();
            SoundEffects.witchSpell = assetBundleBundle.LoadAsset<AudioClip>("witchSpell.mp3").DontUnload();
        }

        private static void LoadSpriteAssets()
        {
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.spriteassets");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceTestAssetBundleStream.ReadFully());
            FoxTask.prefab = assetBundleBundle.LoadAsset<GameObject>("FoxTask.prefab").DontUnload();
            Shrine.sprite = assetBundleBundle.LoadAsset<Sprite>("shrine2.png").DontUnload();
        }

        private static void LoadShaderAssets()
        {
            var resourceTestAssetBundleStream = dll.GetManifestResourceStream("TheOtherRoles.Resources.AssetsBundle.shaderassets");
            var assetBundleBundle = AssetBundle.LoadFromMemory(resourceTestAssetBundleStream.ReadFully());
            Achievement.materialShader = assetBundleBundle.LoadAsset<Shader>("Sprites-White").DontUnload();
            Helpers.HSVShader = assetBundleBundle.LoadAsset<Shader>("Sprites-HSV").DontUnload();
        }
    }

    public static class SoundEffects
    {
        public static AudioClip arsonistDouse;
        public static AudioClip blackmailerSilence;
        public static AudioClip bombDefused;
        public static AudioClip bomberPlantBomb;
        public static AudioClip bombExplosion;
        public static AudioClip bombFuseBurning;
        public static AudioClip bombTick;
        public static AudioClip cleanerClean;
        public static AudioClip deputyHandcuff;
        public static AudioClip engineerRepair;
        public static AudioClip eraserErase;
        public static AudioClip fail;
        public static AudioClip garlic;
        public static AudioClip hackerHack;
        public static AudioClip jackalSidekick;
        public static AudioClip jekyllAndHydeDrug;
        public static AudioClip knockKnock;
        public static AudioClip lighterLight;
        public static AudioClip medicShield;
        public static AudioClip mediumAsk;
        public static AudioClip moriartyBrainwash;
        public static AudioClip morphlingMorph;
        public static AudioClip morphlingSample;
        public static AudioClip ninjaStealth;
        public static AudioClip plagueDoctorSyringe;
        public static AudioClip portalUse;
        public static AudioClip pursuerBlank;
        public static AudioClip securityGuardPlaceCam;
        public static AudioClip sherlockInvestigate;
        public static AudioClip shifterShift;
        public static AudioClip teleporterSample;
        public static AudioClip teleporterTeleport;
        public static AudioClip timemasterShield;
        public static AudioClip trackerTrackCorpses;
        public static AudioClip trackerTrackPlayer;
        public static AudioClip trapperTrap;
        public static AudioClip tricksterPlaceBox;
        public static AudioClip tricksterUseBoxVent;
        public static AudioClip triggerDeceleration;
        public static AudioClip untriggerDeceleration;
        public static AudioClip vampireBite;
        public static AudioClip veteranAlert;
        public static AudioClip vultureEat;
        public static AudioClip warlockCurse;
        public static AudioClip witchSpell;
    }
}
