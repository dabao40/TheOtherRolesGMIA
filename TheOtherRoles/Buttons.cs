using HarmonyLib;
using Hazel;
using System;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using System.Linq;
using System.Collections.Generic;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using JetBrains.Annotations;
using AsmResolver.PE.DotNet.StrongName;
using TheOtherRoles.Patches;
using static UnityEngine.GraphicsBuffer;
using System.Diagnostics.Metrics;
using UnityEngine.SocialPlatforms;

namespace TheOtherRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    static class HudManagerStartPatch
    {
        private static bool initialized = false;

        private static CustomButton engineerRepairButton;
        private static CustomButton janitorCleanButton;
        public static CustomButton sheriffKillButton;
        private static CustomButton deputyHandcuffButton;
        private static CustomButton timeMasterShieldButton;
        private static CustomButton medicShieldButton;
        private static CustomButton shifterShiftButton;
        private static CustomButton morphlingButton;
        private static CustomButton camouflagerButton;
        private static CustomButton portalmakerPlacePortalButton;
        private static CustomButton usePortalButton;
        private static CustomButton portalmakerMoveToPortalButton;
        private static CustomButton hackerButton;
        public static CustomButton hackerVitalsButton;
        public static CustomButton hackerAdminTableButton;
        private static CustomButton trackerTrackPlayerButton;
        private static CustomButton trackerTrackCorpsesButton;
        public static CustomButton vampireKillButton;
        public static CustomButton garlicButton;
        public static CustomButton jackalKillButton;
        public static CustomButton sidekickKillButton;
        private static CustomButton jackalSidekickButton;
        private static CustomButton eraserButton;
        private static CustomButton placeJackInTheBoxButton;        
        private static CustomButton lightsOutButton;
        public static CustomButton cleanerCleanButton;
        public static CustomButton warlockCurseButton;
        public static CustomButton securityGuardButton;
        public static CustomButton securityGuardCamButton;
        public static CustomButton arsonistButton;
        public static CustomButton serialKillerButton;
        public static CustomButton vultureEatButton;
        public static CustomButton mediumButton;
        public static CustomButton pursuerButton;
        public static CustomButton witchSpellButton;
        public static CustomButton assassinButton;
        public static CustomButton mayorMeetingButton;
        public static CustomButton thiefKillButton;
        public static CustomButton ninjaButton;
        public static CustomButton evilTrackerButton;
        public static CustomButton sprintButton;
        public static List<CustomButton> fortuneTellerButtons;
        public static CustomButton veteranAlertButton;
        public static CustomButton undertakerDragButton;
        public static CustomButton mimicAAdminButton;
        public static CustomButton mimicAMorphButton;
        public static CustomButton sherlockInvestigateButton;
        public static CustomButton sherlockWatchButton;
        public static CustomButton bomberAPlantBombButton;
        public static CustomButton bomberAReleaseBombButton;
        public static CustomButton bomberBPlantBombButton;
        public static CustomButton bomberBReleaseBombButton;
        public static CustomButton evilHackerButton;
        public static CustomButton evilHackerCreatesMadmateButton;
        public static CustomButton trapperSetTrapButton;
        public static CustomButton moriartyBrainwashButton;
        public static CustomButton moriartyKillButton;
        public static CustomButton akujoHonmeiButton;
        public static CustomButton akujoBackupButton;
        //public static CustomButton trapperButton;
        //public static CustomButton bomberButton;
        //public static CustomButton defuseButton;
        public static CustomButton zoomOutButton;
        private static CustomButton hunterLighterButton;
        private static CustomButton hunterAdminTableButton;
        private static CustomButton hunterArrowButton;
        private static CustomButton huntedShieldButton;

        public static Dictionary<byte, List<CustomButton>> deputyHandcuffedButtons = null;
        public static PoolablePlayer targetDisplay;

        public static TMPro.TMP_Text securityGuardButtonScrewsText;
        public static TMPro.TMP_Text securityGuardChargesText;
        public static TMPro.TMP_Text deputyButtonHandcuffsText;
        public static TMPro.TMP_Text pursuerButtonBlanksText;
        public static TMPro.TMP_Text veteranButtonAlertText;
        public static TMPro.TMP_Text hackerAdminTableChargesText;
        public static TMPro.TMP_Text hackerVitalsChargesText;
        public static TMPro.TMP_Text sherlockNumInvestigateText;
        public static TMPro.TMP_Text sherlockNumKillTimerText;
        public static TMPro.TMP_Text moriartyKillCounterText;
        public static TMPro.TMP_Text akujoTimeRemainingText;
        public static TMPro.TMP_Text akujoBackupLeftText;
        //public static TMPro.TMP_Text trapperChargesText;
        public static TMPro.TMP_Text portalmakerButtonText1;
        public static TMPro.TMP_Text portalmakerButtonText2;
        public static TMPro.TMP_Text huntedShieldCountText;

        public static void setCustomButtonCooldowns() {
            if (!initialized) {
                try {
                    createButtonsPostfix(HudManager.Instance);
                } 
                catch {
                    TheOtherRolesPlugin.Logger.LogWarning("Button cooldowns not set, either the gamemode does not require them or there's something wrong.");
                    return;
                }
            }
            engineerRepairButton.MaxTimer = 0f;
            janitorCleanButton.MaxTimer = Janitor.cooldown;
            sheriffKillButton.MaxTimer = Sheriff.cooldown;
            deputyHandcuffButton.MaxTimer = Deputy.handcuffCooldown;
            timeMasterShieldButton.MaxTimer = TimeMaster.cooldown;
            medicShieldButton.MaxTimer = 0f;
            shifterShiftButton.MaxTimer = 0f;
            morphlingButton.MaxTimer = Morphling.cooldown;
            camouflagerButton.MaxTimer = Camouflager.cooldown;
            portalmakerPlacePortalButton.MaxTimer = Portalmaker.cooldown;
            usePortalButton.MaxTimer = Portalmaker.usePortalCooldown;
            portalmakerMoveToPortalButton.MaxTimer = Portalmaker.usePortalCooldown;
            hackerButton.MaxTimer = Hacker.cooldown;
            hackerVitalsButton.MaxTimer = Hacker.cooldown;
            hackerAdminTableButton.MaxTimer = Hacker.cooldown;
            vampireKillButton.MaxTimer = Vampire.cooldown;
            trackerTrackPlayerButton.MaxTimer = 0f;
            garlicButton.MaxTimer = 0f;
            jackalKillButton.MaxTimer = Jackal.cooldown;
            sidekickKillButton.MaxTimer = Sidekick.cooldown;
            jackalSidekickButton.MaxTimer = Jackal.createSidekickCooldown;
            eraserButton.MaxTimer = Eraser.cooldown;
            placeJackInTheBoxButton.MaxTimer = Trickster.placeBoxCooldown;
            lightsOutButton.MaxTimer = Trickster.lightsOutCooldown;
            cleanerCleanButton.MaxTimer = Cleaner.cooldown;
            warlockCurseButton.MaxTimer = Warlock.cooldown;
            securityGuardButton.MaxTimer = SecurityGuard.cooldown;
            securityGuardCamButton.MaxTimer = SecurityGuard.cooldown;
            arsonistButton.MaxTimer = Arsonist.cooldown;
            vultureEatButton.MaxTimer = Vulture.cooldown;
            mediumButton.MaxTimer = Medium.cooldown;
            pursuerButton.MaxTimer = Pursuer.cooldown;
            trackerTrackCorpsesButton.MaxTimer = Tracker.corpsesTrackingCooldown;
            witchSpellButton.MaxTimer = Witch.cooldown;
            assassinButton.MaxTimer = Assassin.cooldown;
            thiefKillButton.MaxTimer = Thief.cooldown;
            mayorMeetingButton.MaxTimer = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
            ninjaButton.MaxTimer = Ninja.stealthCooldown;
            serialKillerButton.MaxTimer = SerialKiller.suicideTimer;
            //serialKillerButton.MaxTimer = 0f;
            evilTrackerButton.MaxTimer = EvilTracker.cooldown;
            trapperSetTrapButton.MaxTimer = Trapper.cooldown;
            sprintButton.MaxTimer = Sprinter.sprintCooldown;
            veteranAlertButton.MaxTimer = Veteran.cooldown;
            undertakerDragButton.MaxTimer = 0f;
            mimicAAdminButton.MaxTimer = 0f;
            mimicAMorphButton.MaxTimer = 0f;
            sherlockInvestigateButton.MaxTimer = Sherlock.cooldown;
            sherlockWatchButton.MaxTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
            bomberAPlantBombButton.MaxTimer = BomberA.cooldown;
            bomberBPlantBombButton.MaxTimer = BomberA.cooldown;
            bomberAReleaseBombButton.MaxTimer = 0f;
            bomberBReleaseBombButton.MaxTimer = 0f;
            evilHackerButton.MaxTimer = 0f;
            evilHackerCreatesMadmateButton.MaxTimer = 0f;
            moriartyBrainwashButton.MaxTimer = Moriarty.brainwashCooldown;
            moriartyKillButton.MaxTimer = 0f;
            akujoHonmeiButton.MaxTimer = 0f;
            akujoBackupButton.MaxTimer = 0f;
            //trapperButton.MaxTimer = Trapper.cooldown;
            //bomberButton.MaxTimer = Bomber.bombCooldown;
            hunterLighterButton.MaxTimer = Hunter.lightCooldown;
            hunterAdminTableButton.MaxTimer = Hunter.AdminCooldown;
            hunterArrowButton.MaxTimer = Hunter.ArrowCooldown;
            huntedShieldButton.MaxTimer = Hunted.shieldCooldown;
            //defuseButton.MaxTimer = 0f;
            //defuseButton.Timer = 0f;

            timeMasterShieldButton.EffectDuration = TimeMaster.shieldDuration;
            hackerButton.EffectDuration = Hacker.duration;
            hackerVitalsButton.EffectDuration = Hacker.duration;
            hackerAdminTableButton.EffectDuration = Hacker.duration;
            vampireKillButton.EffectDuration = Vampire.delay;
            camouflagerButton.EffectDuration = Camouflager.duration;
            morphlingButton.EffectDuration = Morphling.duration;
            lightsOutButton.EffectDuration = Trickster.lightsOutDuration;
            arsonistButton.EffectDuration = Arsonist.duration;
            mediumButton.EffectDuration = Medium.duration;
            trackerTrackCorpsesButton.EffectDuration = Tracker.corpsesTrackingDuration;
            witchSpellButton.EffectDuration = Witch.spellCastingDuration;
            securityGuardCamButton.EffectDuration = SecurityGuard.duration;
            ninjaButton.EffectDuration = Ninja.stealthDuration;
            bomberAPlantBombButton.EffectDuration = BomberA.duration;
            bomberBPlantBombButton.EffectDuration = BomberA.duration;
            //serialKillerButton.EffectDuration = SerialKiller.suicideTimer;
            veteranAlertButton.EffectDuration = Veteran.alertDuration;
            hunterLighterButton.EffectDuration = Hunter.lightDuration;
            hunterArrowButton.EffectDuration = Hunter.ArrowDuration;
            huntedShieldButton.EffectDuration = Hunted.shieldDuration;
            //defuseButton.EffectDuration = Bomber.defuseDuration;
            //bomberButton.EffectDuration = Bomber.destructionTime + Bomber.bombActiveAfter;
            // Already set the timer to the max, as the button is enabled during the game and not available at the start
            lightsOutButton.Timer = lightsOutButton.MaxTimer;
            zoomOutButton.MaxTimer = 0f;
        }

        public static void resetTimeMasterButton() {
            timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
            timeMasterShieldButton.isEffectActive = false;
            timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            SoundEffectsManager.stop("timemasterShield");
        }

        public static void resetHuntedRewindButton() {
            huntedShieldButton.Timer = huntedShieldButton.MaxTimer;
            huntedShieldButton.isEffectActive = false;
            huntedShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            SoundEffectsManager.stop("timemasterShield");
        }

        private static void addReplacementHandcuffedButton(CustomButton button, Vector3? positionOffset = null, Func<bool> couldUse = null)
        {
            Vector3 positionOffsetValue = positionOffset ?? button.PositionOffset;  // For non custom buttons, we can set these manually.
            positionOffsetValue.z = -0.1f;
            couldUse = couldUse ?? button.CouldUse;
            CustomButton replacementHandcuffedButton = new CustomButton(() => { }, () => { return true; }, couldUse, () => { }, Deputy.getHandcuffedButtonSprite(), positionOffsetValue, button.hudManager, button.hotkey,
                true, Deputy.handcuffDuration, () => { }, button.mirror);
            replacementHandcuffedButton.Timer = replacementHandcuffedButton.EffectDuration;
            replacementHandcuffedButton.actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
            replacementHandcuffedButton.isEffectActive = true;
            if (deputyHandcuffedButtons.ContainsKey(CachedPlayer.LocalPlayer.PlayerId))
                deputyHandcuffedButtons[CachedPlayer.LocalPlayer.PlayerId].Add(replacementHandcuffedButton);
            else
                deputyHandcuffedButtons.Add(CachedPlayer.LocalPlayer.PlayerId, new List<CustomButton> { replacementHandcuffedButton });
        }
        
        // Disables / Enables all Buttons (except the ones disabled in the Deputy class), and replaces them with new buttons.
        public static void setAllButtonsHandcuffedStatus(bool handcuffed, bool reset = false)
        {
            if (reset) {
                deputyHandcuffedButtons = new Dictionary<byte, List<CustomButton>>();
                return;
            }
            if (handcuffed && !deputyHandcuffedButtons.ContainsKey(CachedPlayer.LocalPlayer.PlayerId))
            {
                int maxI = CustomButton.buttons.Count;
                for (int i = 0; i < maxI; i++)
                {
                    try
                    {
                        if (CustomButton.buttons[i].HasButton() && CustomButton.buttons[i] != serialKillerButton)  // For each custombutton the player has
                        {
                            addReplacementHandcuffedButton(CustomButton.buttons[i]);  // The new buttons are the only non-handcuffed buttons now!
                        }
                        if (CustomButton.buttons[i] != serialKillerButton) CustomButton.buttons[i].isHandcuffed = true;
                    }
                    catch (NullReferenceException)
                    {
                        System.Console.WriteLine("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");  // Note: idk what this is good for, but i copied it from above /gendelo
                    }
                }
                // Non Custom (Vanilla) Buttons. The Originals are disabled / hidden in UpdatePatch.cs already, just need to replace them. Can use any button, as we replace onclick etc anyways.
                // Kill Button if enabled for the Role
                if (FastDestroyableSingleton<HudManager>.Instance.KillButton.isActiveAndEnabled) addReplacementHandcuffedButton(arsonistButton, CustomButton.ButtonPositions.upperRowRight, couldUse: () => { return FastDestroyableSingleton<HudManager>.Instance.KillButton.currentTarget != null; });
                // Vent Button if enabled
                if (CachedPlayer.LocalPlayer.PlayerControl.roleCanUseVents()) addReplacementHandcuffedButton(arsonistButton, CustomButton.ButtonPositions.upperRowCenter, couldUse: () => { return FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.currentTarget != null; });
                // Report Button
                addReplacementHandcuffedButton(arsonistButton, (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor) ? new Vector3(-1f, -0.06f, 0): CustomButton.ButtonPositions.lowerRowRight, () => { return FastDestroyableSingleton<HudManager>.Instance.ReportButton.graphic.color == Palette.EnabledColor; });
            }
            else if (!handcuffed && deputyHandcuffedButtons.ContainsKey(CachedPlayer.LocalPlayer.PlayerId))  // Reset to original. Disables the replacements, enables the original buttons.
            {
                foreach (CustomButton replacementButton in deputyHandcuffedButtons[CachedPlayer.LocalPlayer.PlayerId])
                {
                    replacementButton.HasButton = () => { return false; };
                    replacementButton.Update(); // To make it disappear properly.
                    CustomButton.buttons.Remove(replacementButton);
                }
                deputyHandcuffedButtons.Remove(CachedPlayer.LocalPlayer.PlayerId);

                foreach (CustomButton button in CustomButton.buttons)
                {
                    button.isHandcuffed = false;
                }
            }
        }

        private static void setButtonTargetDisplay(PlayerControl target, CustomButton button = null, Vector3? offset=null) {
            if (target == null || button == null) {
                if (targetDisplay != null) {  // Reset the poolable player
                    targetDisplay.gameObject.SetActive(false);
                    GameObject.Destroy(targetDisplay.gameObject);
                    targetDisplay = null;
                }
                return;
            }
            // Add poolable player to the button so that the target outfit is shown
            button.actionButton.cooldownTimerText.transform.localPosition = new Vector3(0, 0, -1f);  // Before the poolable player
            targetDisplay = UnityEngine.Object.Instantiate<PoolablePlayer>(Patches.IntroCutsceneOnDestroyPatch.playerPrefab, button.actionButton.transform);
            GameData.PlayerInfo data = target.Data;
            target.SetPlayerMaterialColors(targetDisplay.cosmetics.currentBodySprite.BodySprite);
            targetDisplay.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
            targetDisplay.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
            targetDisplay.cosmetics.nameText.text = "";  // Hide the name!
            targetDisplay.transform.localPosition = new Vector3(0f, 0.22f, -0.01f);
            if (offset != null) targetDisplay.transform.localPosition += (Vector3)offset;
            targetDisplay.transform.localScale = Vector3.one * 0.33f;
            targetDisplay.setSemiTransparent(false);
            targetDisplay.gameObject.SetActive(true);
        }

        public static void Postfix(HudManager __instance) {
            initialized = false;

            try {
                createButtonsPostfix(__instance);
            } catch { }
        }
         
        public static void createButtonsPostfix(HudManager __instance) {
            // get map id, or raise error to wait...
            var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            // Engineer Repair
            engineerRepairButton = new CustomButton(
                () => {
                    engineerRepairButton.Timer = 0f;
                    MessageWriter usedRepairWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EngineerUsedRepair, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(usedRepairWriter);
                    RPCProcedure.engineerUsedRepair();
                    SoundEffectsManager.play("engineerRepair");
                    foreach (PlayerTask task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator()) {
                        if (task.TaskType == TaskTypes.FixLights) {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EngineerFixLights, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.engineerFixLights();
                        } else if (task.TaskType == TaskTypes.RestoreOxy) {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 0 | 64);
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 1 | 64);
                        } else if (task.TaskType == TaskTypes.ResetReactor) {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 16);
                        } else if (task.TaskType == TaskTypes.ResetSeismic) {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Laboratory, 16);
                        } else if (task.TaskType == TaskTypes.FixComms) {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                        } else if (task.TaskType == TaskTypes.StopCharles) {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 0 | 16);
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 1 | 16);
                        } else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask) {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EngineerFixSubmergedOxygen, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.engineerFixSubmergedOxygen();
                        }

                    }
                },
                () => { return Engineer.engineer != null && Engineer.engineer == CachedPlayer.LocalPlayer.PlayerControl && Engineer.remainingFixes > 0 && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    bool sabotageActive = false;
                    foreach (PlayerTask task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                        if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles
                            || SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                            sabotageActive = true;
                    return sabotageActive && Engineer.remainingFixes > 0 && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => {},
                Engineer.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.F
            );

            // Janitor Clean
            janitorCleanButton = new CustomButton(
                () => {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask)) {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance && CachedPlayer.LocalPlayer.PlayerControl.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    writer.Write(Janitor.janitor.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.cleanBody(playerInfo.PlayerId, Janitor.janitor.PlayerId);
                                    janitorCleanButton.Timer = janitorCleanButton.MaxTimer;
                                    SoundEffectsManager.play("cleanerClean");

                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return Janitor.janitor != null && Janitor.janitor == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { janitorCleanButton.Timer = janitorCleanButton.MaxTimer; },
                Janitor.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            // Sheriff Kill
            sheriffKillButton = new CustomButton(
                () => {
                    if (Sheriff.currentTarget == Veteran.veteran && Veteran.alertActive)
                    {
                        Helpers.checkMurderAttemptAndKill(Sheriff.sheriff, Sheriff.sheriff);
                        return;
                    }

                    MurderAttemptResult murderAttemptResult = Helpers.checkMuderAttempt(Sheriff.sheriff, Sheriff.currentTarget);
                    if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;

                    if (murderAttemptResult == MurderAttemptResult.PerformKill) {
                        byte targetId = 0;
                        if (((Sheriff.currentTarget.Data.Role.IsImpostor && (Sheriff.currentTarget != Mini.mini || Mini.isGrownUp())) ||
                            (Sheriff.spyCanDieToSheriff && Spy.spy == Sheriff.currentTarget) ||
                            (Sheriff.canKillNeutrals && Helpers.isNeutral(Sheriff.currentTarget)) ||
                            (Jackal.jackal == Sheriff.currentTarget || Sidekick.sidekick == Sheriff.currentTarget) ||
                            (CreatedMadmate.createdMadmate == Sheriff.currentTarget && CreatedMadmate.canDieToSheriff) ||
                            (Madmate.canDieToSheriff && Madmate.madmate.Any(x => x.PlayerId == Sheriff.currentTarget.PlayerId))) &&
                            !Madmate.madmate.Any(y => y.PlayerId == Sheriff.sheriff.PlayerId)) 
                        {
                            targetId = Sheriff.currentTarget.PlayerId;
                        }
                        else {
                            targetId = CachedPlayer.LocalPlayer.PlayerId;
                        }

                        MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                        killWriter.Write(Sheriff.sheriff.Data.PlayerId);
                        killWriter.Write(targetId);
                        killWriter.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                        RPCProcedure.uncheckedMurderPlayer(Sheriff.sheriff.Data.PlayerId, targetId, Byte.MaxValue);
                    }

                    sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                    Sheriff.currentTarget = null;
                },
                () => { return Sheriff.sheriff != null && Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return Sheriff.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer;},
                __instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.Q
            );

            // Deputy Handcuff
            deputyHandcuffButton = new CustomButton(
                () => {
                    byte targetId = 0;
                    targetId = Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl ? Sheriff.currentTarget.PlayerId : Deputy.currentTarget.PlayerId;  // If the deputy is now the sheriff, sheriffs target, else deputies target

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DeputyUsedHandcuffs, Hazel.SendOption.Reliable, -1);
                    writer.Write(targetId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.deputyUsedHandcuffs(targetId);
                    Deputy.currentTarget = null;
                    deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer;

                    SoundEffectsManager.play("deputyHandcuff");
                },
                () => { return (Deputy.deputy != null && Deputy.deputy == CachedPlayer.LocalPlayer.PlayerControl || Sheriff.sheriff != null && Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl && Sheriff.sheriff == Sheriff.formerDeputy && Deputy.keepsHandcuffsOnPromotion) && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (deputyButtonHandcuffsText != null) deputyButtonHandcuffsText.text = $"{Deputy.remainingHandcuffs}";
                    return ((Deputy.deputy != null && Deputy.deputy == CachedPlayer.LocalPlayer.PlayerControl && Deputy.currentTarget || Sheriff.sheriff != null && Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl && Sheriff.sheriff == Sheriff.formerDeputy && Sheriff.currentTarget) && Deputy.remainingHandcuffs > 0 && CachedPlayer.LocalPlayer.PlayerControl.CanMove);
                },
                () => { deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer; },
                Deputy.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );
            // Deputy Handcuff button handcuff counter
            deputyButtonHandcuffsText = GameObject.Instantiate(deputyHandcuffButton.actionButton.cooldownTimerText, deputyHandcuffButton.actionButton.cooldownTimerText.transform.parent);
            deputyButtonHandcuffsText.text = "";
            deputyButtonHandcuffsText.enableWordWrapping = false;
            deputyButtonHandcuffsText.transform.localScale = Vector3.one * 0.5f;
            deputyButtonHandcuffsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Time Master Rewind Time
            timeMasterShieldButton = new CustomButton(
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TimeMasterShield, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.timeMasterShield();
                    SoundEffectsManager.play("timemasterShield");
                },
                () => { return TimeMaster.timeMaster != null && TimeMaster.timeMaster == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => {
                    timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                    timeMasterShieldButton.isEffectActive = false;
                    timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                TimeMaster.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F, 
                true,
                TimeMaster.shieldDuration,
                () => {
                    timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                    SoundEffectsManager.stop("timemasterShield");

                }
            );

            // Medic Shield
            medicShieldButton = new CustomButton(
                () => {
                    medicShieldButton.Timer = 0f;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, Medic.setShieldAfterMeeting ? (byte)CustomRPC.SetFutureShielded : (byte)CustomRPC.MedicSetShielded, Hazel.SendOption.Reliable, -1);
                    writer.Write(Medic.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    if (Medic.setShieldAfterMeeting)
                        RPCProcedure.setFutureShielded(Medic.currentTarget.PlayerId);
                    else
                        RPCProcedure.medicSetShielded(Medic.currentTarget.PlayerId);
                    Medic.meetingAfterShielding = false;

                    SoundEffectsManager.play("medicShield");
                    },
                () => { return Medic.medic != null && Medic.medic == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return !Medic.usedShield && Medic.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => {},
                Medic.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );


            // Shifter shift
            shifterShiftButton = new CustomButton(
                () => {
                    if (Veteran.veteran != null && Shifter.currentTarget == Veteran.veteran && Veteran.alertActive && Shifter.isNeutral)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, Shifter.shifter);
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureShifted, Hazel.SendOption.Reliable, -1);
                    writer.Write(Shifter.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
                    SoundEffectsManager.play("shifterShift");
                },
                () => { return Shifter.shifter != null && Shifter.shifter == CachedPlayer.LocalPlayer.PlayerControl && Shifter.futureShift == null && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return Shifter.currentTarget && Shifter.futureShift == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { },
                Shifter.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );

            // Morphling morph

            morphlingButton = new CustomButton(
                () => {
                    if (Morphling.sampledTarget != null) {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MorphlingMorph, Hazel.SendOption.Reliable, -1);
                        writer.Write(Morphling.sampledTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.morphlingMorph(Morphling.sampledTarget.PlayerId);
                        Morphling.sampledTarget = null;
                        morphlingButton.EffectDuration = Morphling.duration;
                        SoundEffectsManager.play("morphlingMorph");
                    } else if (Morphling.currentTarget != null) {
                        if (Morphling.currentTarget == Veteran.veteran && Veteran.alertActive && Veteran.veteran != null)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Morphling.morphling);
                            return;
                        }
                        Morphling.sampledTarget = Morphling.currentTarget;
                        morphlingButton.Sprite = Morphling.getMorphSprite();
                        morphlingButton.EffectDuration = 1f;
                        SoundEffectsManager.play("morphlingSample");

                        // Add poolable player to the button so that the target outfit is shown
                        setButtonTargetDisplay(Morphling.sampledTarget, morphlingButton);
                    }
                },
                () => { return Morphling.morphling != null && Morphling.morphling == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return (Morphling.currentTarget || Morphling.sampledTarget) && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { 
                    morphlingButton.Timer = morphlingButton.MaxTimer;
                    morphlingButton.Sprite = Morphling.getSampleSprite();
                    morphlingButton.isEffectActive = false;
                    morphlingButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                    Morphling.sampledTarget = null;
                    setButtonTargetDisplay(null);
                },
                Morphling.getSampleSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                true,
                Morphling.duration,
                () => {
                    if (Morphling.sampledTarget == null) {
                        morphlingButton.Timer = morphlingButton.MaxTimer;
                        morphlingButton.Sprite = Morphling.getSampleSprite();
                        SoundEffectsManager.play("morphlingMorph");

                        // Reset the poolable player
                        setButtonTargetDisplay(null);
                    }
                }
            );

            // Camouflager camouflage
            camouflagerButton = new CustomButton(
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CamouflagerCamouflage, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.camouflagerCamouflage();
                    SoundEffectsManager.play("morphlingMorph");
                },
                () => { return Camouflager.camouflager != null && Camouflager.camouflager == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => {
                    camouflagerButton.Timer = camouflagerButton.MaxTimer;
                    camouflagerButton.isEffectActive = false;
                    camouflagerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Camouflager.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                true,
                Camouflager.duration,
                () => {
                    camouflagerButton.Timer = camouflagerButton.MaxTimer;
                    SoundEffectsManager.play("morphlingMorph");
                }
            );

            // Hacker button
            hackerButton = new CustomButton(
                () => {
                    Hacker.hackerTimer = Hacker.duration;
                    SoundEffectsManager.play("hackerHack");
                },
                () => { return Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return true; },
                () => {
                    hackerButton.Timer = hackerButton.MaxTimer;
                    hackerButton.isEffectActive = false;
                    hackerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Hacker.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.F,
                true,
                0f,
                () => { hackerButton.Timer = hackerButton.MaxTimer;}
            );

            hackerAdminTableButton = new CustomButton(
               () => {
                   if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled) {
                       HudManager __instance = FastDestroyableSingleton<HudManager>.Instance;
                       __instance.InitMap();
                       MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
                   }
                   if (Hacker.cantMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                   CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                   Hacker.chargesAdminTable--;
               },
               () => { return Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead;},
               () => {
                   if (hackerAdminTableChargesText != null) hackerAdminTableChargesText.text = $"{Hacker.chargesAdminTable} / {Hacker.toolsNumber}";
                   return Hacker.chargesAdminTable > 0; 
               },
               () => {
                   hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                   hackerAdminTableButton.isEffectActive = false;
                   hackerAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
               },
               Hacker.getAdminSprite(),
               CustomButton.ButtonPositions.lowerRowRight,
               __instance,
               KeyCode.Q,
               true,
               0f,
               () => { 
                   hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                   if (!hackerVitalsButton.isEffectActive) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                   if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
               },
               GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3,
               FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
           );

            // Hacker Admin Table Charges
            hackerAdminTableChargesText = GameObject.Instantiate(hackerAdminTableButton.actionButton.cooldownTimerText, hackerAdminTableButton.actionButton.cooldownTimerText.transform.parent);
            hackerAdminTableChargesText.text = "";
            hackerAdminTableChargesText.enableWordWrapping = false;
            hackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
            hackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            hackerVitalsButton = new CustomButton(
               () => {
                   if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1) {
                       if (Hacker.vitals == null) {
                           var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                           if (e == null || Camera.main == null) return;
                           Hacker.vitals = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                       }
                       Hacker.vitals.transform.SetParent(Camera.main.transform, false);
                       Hacker.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                       Hacker.vitals.Begin(null);
                   } else {
                       if (Hacker.doorLog == null) {
                           var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                           if (e == null || Camera.main == null) return;
                           Hacker.doorLog = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                       }
                       Hacker.doorLog.transform.SetParent(Camera.main.transform, false);
                       Hacker.doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                       Hacker.doorLog.Begin(null);
                   }

                   if (Hacker.cantMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                   CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 

                   Hacker.chargesVitals--;
               },
               () => { return Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 0 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 3; },
               () => {
                   if (hackerVitalsChargesText != null) hackerVitalsChargesText.text = $"{Hacker.chargesVitals} / {Hacker.toolsNumber}";
                   hackerVitalsButton.actionButton.graphic.sprite = GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 ? Hacker.getLogSprite() : Hacker.getVitalsSprite();
                   hackerVitalsButton.actionButton.OverrideText(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 ?
                       TranslationController.Instance.GetString(StringNames.DoorlogLabel) :
                       TranslationController.Instance.GetString(StringNames.VitalsLabel));
                   return Hacker.chargesVitals > 0;
               },
               () => {
                   hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                   hackerVitalsButton.isEffectActive = false;
                   hackerVitalsButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
               },
               Hacker.getVitalsSprite(),
               CustomButton.ButtonPositions.lowerRowCenter,
               __instance,
               KeyCode.Q,
               true,
               0f,
               () => { 
                   hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                   if(!hackerAdminTableButton.isEffectActive) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                   if (Minigame.Instance) {
                       if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1) Hacker.doorLog.ForceClose();
                       else Hacker.vitals.ForceClose();
                   }
               },
               false,
              GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 ?
              TranslationController.Instance.GetString(StringNames.DoorlogLabel) :
              TranslationController.Instance.GetString(StringNames.VitalsLabel)
           );

            // Hacker Vitals Charges
            hackerVitalsChargesText = GameObject.Instantiate(hackerVitalsButton.actionButton.cooldownTimerText, hackerVitalsButton.actionButton.cooldownTimerText.transform.parent);
            hackerVitalsChargesText.text = "";
            hackerVitalsChargesText.enableWordWrapping = false;
            hackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
            hackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Tracker button
            trackerTrackPlayerButton = new CustomButton(
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.TrackerUsedTracker, Hazel.SendOption.Reliable, -1);
                    writer.Write(Tracker.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.trackerUsedTracker(Tracker.currentTarget.PlayerId);
                    SoundEffectsManager.play("trackerTrackPlayer");
                },
                () => { return Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Tracker.currentTarget != null && !Tracker.usedTracker; },
                () => { if(Tracker.resetTargetAfterMeeting) Tracker.resetTracked(); },
                Tracker.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );

            trackerTrackCorpsesButton = new CustomButton(
                () => { Tracker.corpsesTrackingTimer = Tracker.corpsesTrackingDuration;
                            SoundEffectsManager.play("trackerTrackCorpses"); },
                () => { return Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && Tracker.canTrackCorpses; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => {
                    trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                    trackerTrackCorpsesButton.isEffectActive = false;
                    trackerTrackCorpsesButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Tracker.getTrackCorpsesButtonSprite(),
                CustomButton.ButtonPositions.lowerRowCenter,
                __instance,
                KeyCode.Q,
                true,
                Tracker.corpsesTrackingDuration,
                () => {
                    trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                }
            );
    
            vampireKillButton = new CustomButton(
                () => {
                    if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == Vampire.currentTarget)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, Vampire.vampire);
                        return;
                    }
                    MurderAttemptResult murder = Helpers.checkMuderAttempt(Vampire.vampire, Vampire.currentTarget);
                    if (murder == MurderAttemptResult.PerformKill) {
                        if (Vampire.targetNearGarlic) {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                            writer.Write(Vampire.vampire.PlayerId);
                            writer.Write(Vampire.currentTarget.PlayerId);
                            writer.Write(Byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.uncheckedMurderPlayer(Vampire.vampire.PlayerId, Vampire.currentTarget.PlayerId, Byte.MaxValue);

                            vampireKillButton.HasEffect = false; // Block effect on this click
                            vampireKillButton.Timer = vampireKillButton.MaxTimer;
                        } else {
                            Vampire.bitten = Vampire.currentTarget;
                            // Notify players about bitten
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                            writer.Write(Vampire.bitten.PlayerId);
                            writer.Write((byte)0);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.vampireSetBitten(Vampire.bitten.PlayerId, 0);

                            byte lastTimer = (byte)Vampire.delay;
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.delay, new Action<float>((p) => { // Delayed action
                                if (p <= 1f) {
                                    byte timer = (byte)vampireKillButton.Timer;
                                    if (timer != lastTimer) {
                                        lastTimer = timer;
                                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                                        writer.Write((byte)RPCProcedure.GhostInfoTypes.VampireTimer);
                                        writer.Write(timer);
                                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    }
                                }
                                if (p == 1f) {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    Helpers.checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten, showAnimation: false);
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                                    writer.Write(byte.MaxValue);
                                    writer.Write(byte.MaxValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                                }
                            })));
                            SoundEffectsManager.play("vampireBite");

                            vampireKillButton.HasEffect = true; // Trigger effect on this click
                        }
                    } else if (murder == MurderAttemptResult.BlankKill) {
                        vampireKillButton.Timer = vampireKillButton.MaxTimer;
                        vampireKillButton.HasEffect = false;
                    } else {
                        vampireKillButton.HasEffect = false;
                    }
                },
                () => { return Vampire.vampire != null && Vampire.vampire == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (Vampire.targetNearGarlic && Vampire.canKillNearGarlics) {
                        vampireKillButton.actionButton.graphic.sprite = __instance.KillButton.graphic.sprite;
                        vampireKillButton.showButtonText = true;
                    }
                    else {
                        vampireKillButton.actionButton.graphic.sprite = Vampire.getButtonSprite();
                        vampireKillButton.showButtonText = false;
                    }
                    return Vampire.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove && (!Vampire.targetNearGarlic || Vampire.canKillNearGarlics);
                },
                () => {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    vampireKillButton.isEffectActive = false;
                    vampireKillButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Vampire.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.Q,
                false,
                0f,
                () => {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                }
            );

            garlicButton = new CustomButton(
                () => {
                    Vampire.localPlacedGarlic = true;
                    var pos = CachedPlayer.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0*sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1*sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceGarlic, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeGarlic(buff);
                    SoundEffectsManager.play("garlic");
                },
                () => { return !Vampire.localPlacedGarlic && !CachedPlayer.LocalPlayer.Data.IsDead && Vampire.garlicsActive && !HideNSeek.isHideNSeekGM; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Vampire.localPlacedGarlic; },
                () => { },
                Vampire.getGarlicButtonSprite(),
                new Vector3(0, -0.06f, 0),
                __instance,
                null,
                true
            );

            portalmakerPlacePortalButton = new CustomButton(
                () => {
                    portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer;

                    var pos = CachedPlayer.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlacePortal, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placePortal(buff);
                    SoundEffectsManager.play("tricksterPlaceBox");
                },
                () => { return Portalmaker.portalmaker != null && Portalmaker.portalmaker == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && Portal.secondPortal == null; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Portal.secondPortal == null; },
                () => { portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer; },
                Portalmaker.getPlacePortalButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );

            usePortalButton = new CustomButton(
                () => {
                    bool didTeleport = false;
                    Vector3 exit = Portal.findExit(CachedPlayer.LocalPlayer.transform.position);
                    Vector3 entry = Portal.findEntry(CachedPlayer.LocalPlayer.transform.position);

                    bool portalMakerSoloTeleport = !Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position);
                    if (portalMakerSoloTeleport) {
                        exit = Portal.firstPortal.portalGameObject.transform.position;
                        entry = CachedPlayer.LocalPlayer.transform.position;
                    }

                    CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(entry);

                    if (!CachedPlayer.LocalPlayer.Data.IsDead) {  // Ghosts can portal too, but non-blocking and only with a local animation
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UsePortal, Hazel.SendOption.Reliable, -1);
                        writer.Write((byte)CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write(portalMakerSoloTeleport ? (byte)1 : (byte)0);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    RPCProcedure.usePortal(CachedPlayer.LocalPlayer.PlayerId, portalMakerSoloTeleport ? (byte)1 : (byte)0);
                    usePortalButton.Timer = usePortalButton.MaxTimer;
                    portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
                    SoundEffectsManager.play("portalUse");
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration, new Action<float>((p) => { // Delayed action
                        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                        CachedPlayer.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance) {
                            if (SubmergedCompatibility.IsSubmerged) {
                                SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            }
                            CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }
                        if (p == 1f) {
                            CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                        }
                    })));
                    },
                () => {
                    if (CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker && Portal.bothPlacedAndEnabled)
                        portalmakerButtonText1.text = Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position) || !Portalmaker.canPortalFromAnywhere ? "" : "1. " + Portal.firstPortal.room;
                    return Portal.bothPlacedAndEnabled; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && (Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position) || Portalmaker.canPortalFromAnywhere && CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker) && !Portal.isTeleporting; },
                () => { usePortalButton.Timer = usePortalButton.MaxTimer; },
                Portalmaker.getUsePortalButtonSprite(),
                new Vector3(0.9f, -0.06f, 0),
                __instance,
                KeyCode.H,
                mirror: true
            );

            portalmakerMoveToPortalButton = new CustomButton(
                () => {
                    bool didTeleport = false;
                    Vector3 exit = Portal.secondPortal.portalGameObject.transform.position;

                    if (!CachedPlayer.LocalPlayer.Data.IsDead) {  // Ghosts can portal too, but non-blocking and only with a local animation
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UsePortal, Hazel.SendOption.Reliable, -1);
                        writer.Write((byte)CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write((byte)2);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    RPCProcedure.usePortal(CachedPlayer.LocalPlayer.PlayerId, 2);
                    usePortalButton.Timer = usePortalButton.MaxTimer;
                    portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
                    SoundEffectsManager.play("portalUse");
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration, new Action<float>((p) => { // Delayed action
                        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                        CachedPlayer.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance) {
                            if (SubmergedCompatibility.IsSubmerged) {
                                SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            }
                            CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }
                        if (p == 1f) {
                            CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                        }
                    })));
                },
                () => { return Portalmaker.canPortalFromAnywhere && Portal.bothPlacedAndEnabled && CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position) && !Portal.isTeleporting; },
                () => { portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer; },
                Portalmaker.getUsePortalButtonSprite(),
                new Vector3(0.9f, 1f, 0),
                __instance,
                KeyCode.J,
                mirror: true
            );


            portalmakerButtonText1 = GameObject.Instantiate(usePortalButton.actionButton.cooldownTimerText, usePortalButton.actionButton.cooldownTimerText.transform.parent);
            portalmakerButtonText1.text = "";
            portalmakerButtonText1.enableWordWrapping = false;
            portalmakerButtonText1.transform.localScale = Vector3.one * 0.5f;
            portalmakerButtonText1.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);

            portalmakerButtonText2 = GameObject.Instantiate(portalmakerMoveToPortalButton.actionButton.cooldownTimerText, portalmakerMoveToPortalButton.actionButton.cooldownTimerText.transform.parent);
            portalmakerButtonText2.text = "";
            portalmakerButtonText2.enableWordWrapping = false;
            portalmakerButtonText2.transform.localScale = Vector3.one * 0.5f;
            portalmakerButtonText2.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);



            // Jackal Sidekick Button
            jackalSidekickButton = new CustomButton(
                () => {
                    if (Jackal.currentTarget == Veteran.veteran && Veteran.alertActive && Veteran.veteran != null)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, Jackal.jackal);
                        return;
                    }
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.JackalCreatesSidekick, Hazel.SendOption.Reliable, -1);
                    writer.Write(Jackal.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.jackalCreatesSidekick(Jackal.currentTarget.PlayerId);
                    SoundEffectsManager.play("jackalSidekick");
                },
                () => { return Jackal.canCreateSidekick && Jackal.jackal != null && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return Jackal.canCreateSidekick && Jackal.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { jackalSidekickButton.Timer = jackalSidekickButton.MaxTimer;},
                Jackal.getSidekickButtonSprite(),
                CustomButton.ButtonPositions.lowerRowCenter,
                __instance,
                KeyCode.F
            );

            // Jackal Kill
            jackalKillButton = new CustomButton(
                () => {
                    if (Helpers.checkMurderAttemptAndKill(Jackal.jackal, Jackal.currentTarget) == MurderAttemptResult.SuppressKill) return;

                    jackalKillButton.Timer = jackalKillButton.MaxTimer; 
                    Jackal.currentTarget = null;
                },
                () => { return Jackal.jackal != null && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return Jackal.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { jackalKillButton.Timer = jackalKillButton.MaxTimer;},
                __instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.Q
            );

            // EvilHacker creates madmate button
            evilHackerCreatesMadmateButton = new CustomButton(
                () =>
                {
                    /*
                     * creates madmate
                     */
                    if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == EvilHacker.currentTarget)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, EvilHacker.evilHacker);
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EvilHackerCreatesMadmate, Hazel.SendOption.Reliable, -1);
                    writer.Write(EvilHacker.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.evilHackerCreatesMadmate(EvilHacker.currentTarget.PlayerId);
                },
                () =>
                {
                    return EvilHacker.evilHacker != null &&
                      EvilHacker.evilHacker == CachedPlayer.LocalPlayer.PlayerControl &&
                      EvilHacker.canCreateMadmate &&
                      !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
                },
                () => { return EvilHacker.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { },
                EvilHacker.getMadmateButtonSprite(),
                CustomButton.ButtonPositions.lowerRowCenter,
                __instance,
                KeyCode.F
            );

            // Sidekick Kill
            sidekickKillButton = new CustomButton(
                () => {
                    if (Helpers.checkMurderAttemptAndKill(Sidekick.sidekick, Sidekick.currentTarget) == MurderAttemptResult.SuppressKill) return;
                    sidekickKillButton.Timer = sidekickKillButton.MaxTimer; 
                    Sidekick.currentTarget = null;
                },
                () => { return Sidekick.canKill && Sidekick.sidekick != null && Sidekick.sidekick == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return Sidekick.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { sidekickKillButton.Timer = sidekickKillButton.MaxTimer;},
                __instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.Q
            );

            // Eraser erase button
            eraserButton = new CustomButton(
                () => {
                    if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == Eraser.currentTarget)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, Eraser.eraser);
                        return;
                    }
                    eraserButton.MaxTimer += 10;
                    eraserButton.Timer = eraserButton.MaxTimer;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureErased, Hazel.SendOption.Reliable, -1);
                    writer.Write(Eraser.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureErased(Eraser.currentTarget.PlayerId);
                    SoundEffectsManager.play("eraserErase");
                },
                () => { return Eraser.eraser != null && Eraser.eraser == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Eraser.currentTarget != null; },
                () => { eraserButton.Timer = eraserButton.MaxTimer;},
                Eraser.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            trapperSetTrapButton = new CustomButton(
                () =>
                { // ボタンが押された時に実行
                    if (!CachedPlayer.LocalPlayer.PlayerControl.CanMove || Trap.hasTrappedPlayer()) return;
                    Trapper.setTrap();
                    trapperSetTrapButton.Timer = trapperSetTrapButton.MaxTimer;
                },
                () =>
                { /*ボタン有効になる条件*/
                    return CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
                },
                () =>
                { /*ボタンが使える条件*/
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Trap.hasTrappedPlayer();
                },
                () =>
                { /*ミーティング終了時*/
                    trapperSetTrapButton.Timer = trapperSetTrapButton.MaxTimer;
                },
                Trapper.getTrapButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            moriartyBrainwashButton = new CustomButton(
                () =>
                {
                    if (Moriarty.currentTarget != null)
                    {
                        if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == Moriarty.currentTarget)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Moriarty.moriarty);
                            return;
                        }

                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBrainwash, Hazel.SendOption.Reliable, -1);
                        writer.Write(Moriarty.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.setBrainwash(Moriarty.currentTarget.PlayerId);

                        // 洗脳終了までのカウントダウン
                        TMPro.TMP_Text text;
                        RoomTracker roomTracker = HudManager.Instance?.roomTracker;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(HudManager.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -1.3f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 3f;
                        text = gameObject.GetComponent<TMPro.TMP_Text>();
                        PlayerControl tmpP = Moriarty.target;
                        bool done = false;
                        HudManager.Instance.StartCoroutine(Effects.Lerp(Moriarty.brainwashTime, new Action<float>((p) =>
                        {
                            if (done)
                            {
                                return;
                            }
                            if (Moriarty.target == null || MeetingHud.Instance != null || p == 1f)
                            {
                                if (text != null && text.gameObject) UnityEngine.Object.Destroy(text.gameObject);
                                if (Moriarty.target == tmpP) Moriarty.target = null;
                                done = true;
                                return;
                            }
                            else
                            {
                                string message = (Moriarty.brainwashTime - (p * Moriarty.brainwashTime)).ToString("0");
                                bool even = ((int)(p * Moriarty.brainwashTime / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds
                                // string prefix = even ? "<color=#555555FF>" : "<color=#FFFFFFFF>";
                                string prefix = "<color=#555555FF>";
                                text.text = prefix + message + "</color>";
                                if (text != null) text.color = even ? Color.yellow : Color.red;

                            }
                        })));
                    }
                    Moriarty.tmpTarget = null;
                    moriartyBrainwashButton.Timer = moriartyBrainwashButton.MaxTimer;
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Moriarty.moriarty && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Moriarty.target == null; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Moriarty.currentTarget != null; },
                () => { moriartyBrainwashButton.Timer = moriartyBrainwashButton.MaxTimer; },
                Moriarty.getBrainwashIcon(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            moriartyKillButton = new CustomButton(
                () =>
                {

                    MurderAttemptResult murder = Helpers.checkMurderAttemptAndKill(CachedPlayer.LocalPlayer.PlayerControl, Moriarty.killTarget, showAnimation: false);
                    if (murder != MurderAttemptResult.BlankKill)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MoriartyKill, Hazel.SendOption.Reliable, -1);
                        writer.Write(Moriarty.killTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.moriartyKill(Moriarty.killTarget.PlayerId);
                        Moriarty.target = null;
                        moriartyBrainwashButton.Timer = moriartyBrainwashButton.MaxTimer;
                    }
                },
                // HasButton
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Moriarty.moriarty && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead; },
                // CouldUse
                () =>
                {
                    if (moriartyKillCounterText != null)
                    {
                        moriartyKillCounterText.text = $"{Moriarty.counter}/{Moriarty.numberToWin}";
                    }
                    return Moriarty.killTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                // OnMeetingEnds
                () =>
                {
                    moriartyKillButton.Timer = moriartyKillButton.MaxTimer;
                    Moriarty.brainwashed.Clear();
                    Moriarty.target = null;
                },
                __instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.Q
            );
            moriartyKillCounterText = GameObject.Instantiate(moriartyKillButton.actionButton.cooldownTimerText, moriartyKillButton.actionButton.cooldownTimerText.transform.parent);
            moriartyKillCounterText.text = "";
            moriartyKillCounterText.enableWordWrapping = false;
            moriartyKillCounterText.transform.localScale = Vector3.one * 0.5f;
            moriartyKillCounterText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            placeJackInTheBoxButton = new CustomButton(
                () => {
                    placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                    var pos = CachedPlayer.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0*sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1*sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceJackInTheBox, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeJackInTheBox(buff);
                    SoundEffectsManager.play("tricksterPlaceBox");
                },
                () => { return Trickster.trickster != null && Trickster.trickster == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && !JackInTheBox.hasJackInTheBoxLimitReached(); },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !JackInTheBox.hasJackInTheBoxLimitReached(); },
                () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;},
                Trickster.getPlaceBoxButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            lightsOutButton = new CustomButton(
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LightsOut, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lightsOut();
                    SoundEffectsManager.play("lighterLight");
                },
                () => { return Trickster.trickster != null && Trickster.trickster == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents; },
                () => { 
                    lightsOutButton.Timer = lightsOutButton.MaxTimer;
                    lightsOutButton.isEffectActive = false;
                    lightsOutButton.actionButton.graphic.color = Palette.EnabledColor;
                },
                Trickster.getLightsOutButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                true,
                Trickster.lightsOutDuration,
                () => {
                    lightsOutButton.Timer = lightsOutButton.MaxTimer;
                    SoundEffectsManager.play("lighterLight");
                }
            );

            // Cleaner Clean
            cleanerCleanButton = new CustomButton(
                () => {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask)) {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance && CachedPlayer.LocalPlayer.PlayerControl.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);
                                    
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    writer.Write(Cleaner.cleaner.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.cleanBody(playerInfo.PlayerId, Cleaner.cleaner.PlayerId);

                                    Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                                    SoundEffectsManager.play("cleanerClean");
                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return Cleaner.cleaner != null && Cleaner.cleaner == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
                Cleaner.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            // Warlock curse
            warlockCurseButton = new CustomButton(
                () => {
                    if (Warlock.curseVictim == null) {
                        if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == Warlock.currentTarget)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Warlock.warlock);
                            return;
                        }

                        // Apply Curse
                        Warlock.curseVictim = Warlock.currentTarget;
                        warlockCurseButton.Sprite = Warlock.getCurseKillButtonSprite();
                        warlockCurseButton.Timer = 1f;
                        SoundEffectsManager.play("warlockCurse");

                        // Ghost Info
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                        writer.Write(Warlock.curseVictim.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                    } else if (Warlock.curseVictim != null && Warlock.curseVictimTarget != null) {
                        MurderAttemptResult murder = Helpers.checkMurderAttemptAndKill(Warlock.warlock, Warlock.curseVictimTarget, showAnimation: false);
                        if (murder == MurderAttemptResult.SuppressKill) return; 

                        // If blanked or killed
                        if(Warlock.rootTime > 0) {
                            AntiTeleport.position = CachedPlayer.LocalPlayer.transform.position;
                            CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                            CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.rootTime, new Action<float>((p) => { // Delayed action
                                if (p == 1f) {
                                    CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                                }
                            })));
                        }
                        
                        Warlock.curseVictim = null;
                        Warlock.curseVictimTarget = null;
                        warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                        Warlock.warlock.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;

                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                        writer.Write(Byte.MaxValue); // This will set it to null!
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                    }
                },
                () => { return Warlock.warlock != null && Warlock.warlock == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return ((Warlock.curseVictim == null && Warlock.currentTarget != null) || (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)) && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { 
                    warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                    warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                    Warlock.curseVictim = null;
                    Warlock.curseVictimTarget = null;
                },
                Warlock.getCurseButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            // Security Guard button
            securityGuardButton = new CustomButton(
                () => {
                    if (SecurityGuard.ventTarget != null) { // Seal vent
                        MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SealVent, Hazel.SendOption.Reliable);
                        writer.WritePacked(SecurityGuard.ventTarget.Id);
                        writer.EndMessage();
                        RPCProcedure.sealVent(SecurityGuard.ventTarget.Id);
                        SecurityGuard.ventTarget = null;
                        
                    } else if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1 && !SubmergedCompatibility.IsSubmerged) { // Place camera if there's no vent and it's not MiraHQ or Submerged
                        var pos = CachedPlayer.LocalPlayer.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0*sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1*sizeof(float), sizeof(float));

                        MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceCamera, Hazel.SendOption.Reliable);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        RPCProcedure.placeCamera(buff); 
                    }
                    SoundEffectsManager.play("securityGuardPlaceCam");  // Same sound used for both types (cam or vent)!
                    securityGuardButton.Timer = securityGuardButton.MaxTimer;
                },
                () => { return SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews >= Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice); },
                () => {
                    securityGuardButton.actionButton.graphic.sprite = (SecurityGuard.ventTarget == null && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1 && !SubmergedCompatibility.IsSubmerged) ? SecurityGuard.getPlaceCameraButtonSprite() : SecurityGuard.getCloseVentButtonSprite(); 
                    if (securityGuardButtonScrewsText != null) securityGuardButtonScrewsText.text = $"{SecurityGuard.remainingScrews}/{SecurityGuard.totalScrews}";

                    if (SecurityGuard.ventTarget != null)
                        return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                    return GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1 && !SubmergedCompatibility.IsSubmerged && SecurityGuard.remainingScrews >= SecurityGuard.camPrice && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
                SecurityGuard.getPlaceCameraButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );
            
            // Security Guard button screws counter
            securityGuardButtonScrewsText = GameObject.Instantiate(securityGuardButton.actionButton.cooldownTimerText, securityGuardButton.actionButton.cooldownTimerText.transform.parent);
            securityGuardButtonScrewsText.text = "";
            securityGuardButtonScrewsText.enableWordWrapping = false;
            securityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
            securityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            securityGuardCamButton = new CustomButton(
                () => {
                    if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1) {
                        if (SecurityGuard.minigame == null) {
                            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                            UnityEngine.Object.FindObjectsOfType<SystemConsole>().ToList().ForEach(x => TheOtherRolesPlugin.Logger.LogMessage($"{x.name} {x.GetType()}, {x.MinigamePrefab.TaskType}"));
                            var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam"));
                            if (mapId == 0 || mapId == 3) e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                            else if (mapId == 4) e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                            if (e == null || Camera.main == null) return;
                            SecurityGuard.minigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                        }
                        SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                        SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                        SecurityGuard.minigame.Begin(null);
                    } else {
                        if (SecurityGuard.minigame == null) {
                            var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                            if (e == null || Camera.main == null) return;
                            SecurityGuard.minigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                        }
                        SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                        SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                        SecurityGuard.minigame.Begin(null);
                    }
                    SecurityGuard.charges--;

                    if (SecurityGuard.cantMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                    CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                },
                () => { return SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews < Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice)
                               && !SubmergedCompatibility.IsSubmerged; },
                () => {
                    if (securityGuardChargesText != null) securityGuardChargesText.text = $"{SecurityGuard.charges} / {SecurityGuard.maxCharges}";
                    securityGuardCamButton.actionButton.graphic.sprite = GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 ? SecurityGuard.getLogSprite() : SecurityGuard.getCamSprite();
                    securityGuardCamButton.actionButton.OverrideText(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 ?
                        TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) :
                        TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && SecurityGuard.charges > 0;
                },
                () => {
                    securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                    securityGuardCamButton.isEffectActive = false;
                    securityGuardCamButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                SecurityGuard.getCamSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.Q,
                true,
                0f,
                () => {
                    securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                    if (Minigame.Instance) {
                        SecurityGuard.minigame.ForceClose();
                    }
                    CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                },
                false,
                GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 ?
                TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) :
                TranslationController.Instance.GetString(StringNames.SecurityCamsSystem)
            );

            // Security Guard cam button charges
            securityGuardChargesText = GameObject.Instantiate(securityGuardCamButton.actionButton.cooldownTimerText, securityGuardCamButton.actionButton.cooldownTimerText.transform.parent);
            securityGuardChargesText.text = "";
            securityGuardChargesText.enableWordWrapping = false;
            securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
            securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Arsonist button
            arsonistButton = new CustomButton(
                () => {
                    bool dousedEveryoneAlive = Arsonist.dousedEveryoneAlive();
                    if (dousedEveryoneAlive) {
                        MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ArsonistWin, Hazel.SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.arsonistWin();
                        arsonistButton.HasEffect = false;
                    } else if (Arsonist.currentTarget != null) {
                        if (Veteran.veteran != null && Veteran.veteran == Arsonist.currentTarget && Veteran.alertActive)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Arsonist.arsonist);
                            return;
                        }
                        Arsonist.douseTarget = Arsonist.currentTarget;
                        arsonistButton.HasEffect = true;
                        SoundEffectsManager.play("arsonistDouse");
                    }
                },
                () => { return Arsonist.arsonist != null && Arsonist.arsonist == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    bool dousedEveryoneAlive = Arsonist.dousedEveryoneAlive();
                    if (dousedEveryoneAlive) arsonistButton.actionButton.graphic.sprite = Arsonist.getIgniteSprite();
                    
                    if (arsonistButton.isEffectActive && Arsonist.douseTarget != Arsonist.currentTarget) {
                        Arsonist.douseTarget = null;
                        arsonistButton.Timer = 0f;
                        arsonistButton.isEffectActive = false;
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && (dousedEveryoneAlive || Arsonist.currentTarget != null);
                },
                () => {
                    arsonistButton.Timer = arsonistButton.MaxTimer;
                    arsonistButton.isEffectActive = false;
                    Arsonist.douseTarget = null;
                },
                Arsonist.getDouseSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F,
                true,
                Arsonist.duration,
                () => {
                    if (Arsonist.douseTarget != null) Arsonist.dousedPlayers.Add(Arsonist.douseTarget);
                    
                    arsonistButton.Timer = Arsonist.dousedEveryoneAlive() ? 0 : arsonistButton.MaxTimer;

                    foreach (PlayerControl p in Arsonist.dousedPlayers) {
                        if (TORMapOptions.playerIcons.ContainsKey(p.PlayerId)) {
                            TORMapOptions.playerIcons[p.PlayerId].setSemiTransparent(false);
                        }
                    }

                    // Ghost Info
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.ArsonistDouse);
                    writer.Write(Arsonist.douseTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    Arsonist.douseTarget = null;
                }
            );

            // Veteran Alert
            veteranAlertButton = new CustomButton(
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VeteranAlert, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.veteranAlert();

                    Veteran.remainingAlerts--;
                },
                () => { return Veteran.veteran != null && Veteran.veteran == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (veteranButtonAlertText != null) veteranButtonAlertText.text = $"{Veteran.remainingAlerts}";
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Veteran.remainingAlerts > 0; 
                },
                () => {
                    veteranAlertButton.Timer = veteranAlertButton.MaxTimer;
                    veteranAlertButton.isEffectActive = false;
                    veteranAlertButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Veteran.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight, //brb
                __instance,
                KeyCode.F,
                true,
                Veteran.alertDuration,
                () => { veteranAlertButton.Timer = veteranAlertButton.MaxTimer; }
            );

            veteranButtonAlertText = GameObject.Instantiate(veteranAlertButton.actionButton.cooldownTimerText, veteranAlertButton.actionButton.cooldownTimerText.transform.parent);
            veteranButtonAlertText.text = "";
            veteranButtonAlertText.enableWordWrapping = false;
            veteranButtonAlertText.transform.localScale = Vector3.one * 0.5f;
            veteranButtonAlertText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            bomberAPlantBombButton = new CustomButton(
                // OnClick
                () =>
                {
                    if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == BomberA.currentTarget)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, BomberA.bomberA);
                        return;
                    }

                    if (BomberA.currentTarget != null)
                    {
                        BomberA.tmpTarget = BomberA.currentTarget;
                        bomberAPlantBombButton.HasEffect = true;
                    }
                },
                // HasButton
                () => { return CachedPlayer.LocalPlayer.PlayerControl == BomberA.bomberA && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead; },
                // CouldUse
                () =>
                {
                    if (bomberAPlantBombButton.isEffectActive && BomberA.tmpTarget != BomberA.currentTarget)
                    {
                        BomberA.tmpTarget = null;
                        bomberAPlantBombButton.Timer = 0f;
                        bomberAPlantBombButton.isEffectActive = false;
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && BomberA.currentTarget != null;
                },
                // OnMeetingEnds
                () =>
                {
                    bomberAPlantBombButton.Timer = bomberAPlantBombButton.MaxTimer;
                    bomberAPlantBombButton.isEffectActive = false;
                    BomberA.tmpTarget = null;                    
                },
                BomberA.getBomberButtonSprite(),
                CustomButton.ButtonPositions.upperRowCenter,
                __instance,
                KeyCode.F,
                true,
                BomberA.duration,
                // OnEffectsEnd
                () =>
                {
                    if ((BomberA.tmpTarget == Mini.mini && !Mini.isGrownUp()) || (BomberB.bombTarget != null && BomberA.tmpTarget == BomberB.bombTarget))
                    {
                        bomberAPlantBombButton.Timer = 0f;
                    }
                    else
                    {
                        if (BomberA.tmpTarget != null)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlantBomb, Hazel.SendOption.Reliable, -1);
                            writer.Write(BomberA.tmpTarget.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            BomberA.bombTarget = BomberA.tmpTarget;
                        }

                        BomberA.tmpTarget = null;
                        bomberAPlantBombButton.Timer = bomberAPlantBombButton.MaxTimer;
                    }
                }
            );

            bomberBPlantBombButton = new CustomButton(
                // OnClick
                () =>
                {
                    if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == BomberB.currentTarget)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, BomberB.bomberB);
                        return;
                    }

                    if (BomberB.currentTarget != null)
                    {
                        BomberB.tmpTarget = BomberB.currentTarget;
                        bomberBPlantBombButton.HasEffect = true;
                    }
                },
                // HasButton
                () => { return CachedPlayer.LocalPlayer.PlayerControl == BomberB.bomberB && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && BomberA.bomberA != null && !BomberA.bomberA.Data.IsDead; },
                // CouldUse
                () =>
                {
                    if (bomberBPlantBombButton.isEffectActive && BomberB.tmpTarget != BomberB.currentTarget)
                    {
                        BomberB.tmpTarget = null;
                        bomberBPlantBombButton.Timer = 0f;
                        bomberBPlantBombButton.isEffectActive = false;
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && BomberB.currentTarget != null;
                },
                // OnMeetingEnds
                () =>
                {
                    bomberBPlantBombButton.Timer = bomberBPlantBombButton.MaxTimer;
                    bomberBPlantBombButton.isEffectActive = false;
                    BomberB.tmpTarget = null;                    
                },
                BomberB.getBomberButtonSprite(),
                CustomButton.ButtonPositions.upperRowCenter,
                __instance,
                KeyCode.F,
                true,
                BomberA.duration,
                // OnEffectsEnd
                () =>
                {
                    if ((BomberB.tmpTarget == Mini.mini && !Mini.isGrownUp()) || (BomberA.bombTarget != null && BomberB.tmpTarget == BomberA.bombTarget)) bomberBPlantBombButton.Timer = 0f;
                    else
                    {
                        if (BomberB.tmpTarget != null)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlantBomb, Hazel.SendOption.Reliable, -1);
                            writer.Write(BomberB.tmpTarget.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            BomberB.bombTarget = BomberB.tmpTarget;
                        }

                        BomberB.tmpTarget = null;
                        bomberBPlantBombButton.Timer = bomberBPlantBombButton.MaxTimer;
                    }
                }
            );

            bomberAReleaseBombButton = new CustomButton(
                // OnClick
                () =>
                {
                    if (BomberA.bombTarget == Veteran.veteran && Veteran.veteran != null && Veteran.alertActive)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, BomberA.bomberA);
                        return;
                    }

                    // Use MurderAttempt to exclude eg.Medic shielded
                    MurderAttemptResult attempt = Helpers.checkMuderAttempt(BomberA.bomberA, BomberA.bombTarget);

                    var bomberB = BomberB.bomberB;
                    float distance = Vector2.Distance(CachedPlayer.LocalPlayer.PlayerControl.transform.localPosition, bomberB.transform.localPosition);

                    if (attempt == MurderAttemptResult.PerformKill)
                    {
                        if (CachedPlayer.LocalPlayer.PlayerControl.CanMove && BomberA.bombTarget != null && BomberB.bombTarget != null && bomberB != null && !bomberB.Data.IsDead && distance < 1)
                        {
                            var target = BomberA.bombTarget;
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ReleaseBomb, Hazel.SendOption.Reliable, -1);
                            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.releaseBomb(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, target.PlayerId);
                        }
                    }
                    else if (attempt == MurderAttemptResult.BlankKill)
                    {
                        bomberAPlantBombButton.Timer = bomberAPlantBombButton.MaxTimer;
                        return;
                    }
                    else if (attempt == MurderAttemptResult.SuppressKill) return;
                },
                // HasButton
                () => { return CachedPlayer.LocalPlayer.PlayerControl == BomberA.bomberA && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead; },
                // CouldUse
                () =>
                {
                    var bomberB = BomberB.bomberB;
                    float distance = Vector2.Distance(CachedPlayer.LocalPlayer.PlayerControl.transform.localPosition, bomberB.transform.localPosition);

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && BomberA.bombTarget != null && BomberB.bombTarget != null && bomberB != null && !bomberB.Data.IsDead && distance < 1;
                },
                // OnMeetingEnds
                () =>
                {
                    bomberAReleaseBombButton.Timer = bomberAReleaseBombButton.MaxTimer;
                },
                BomberA.getReleaseButtonSprite(),
                CustomButton.ButtonPositions.lowerRowCenter,
                __instance,
                KeyCode.Q,
                false
            );

            bomberBReleaseBombButton = new CustomButton(
                // OnClick
                () =>
                {
                    if (BomberB.bombTarget == Veteran.veteran && Veteran.veteran != null && Veteran.alertActive)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, BomberB.bomberB);
                        return;
                    }

                    var bomberA = BomberA.bomberA;
                    float distance = Vector2.Distance(CachedPlayer.LocalPlayer.PlayerControl.transform.localPosition, bomberA.transform.localPosition);

                    MurderAttemptResult attempt = Helpers.checkMuderAttempt(BomberB.bomberB, BomberB.bombTarget);

                    if (attempt == MurderAttemptResult.PerformKill)
                    {
                        if (CachedPlayer.LocalPlayer.PlayerControl.CanMove && BomberA.bombTarget != null && BomberB.bombTarget != null && bomberA != null && !bomberA.Data.IsDead && distance < 1)
                        {
                            var target = BomberB.bombTarget;
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ReleaseBomb, Hazel.SendOption.Reliable, -1);
                            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.releaseBomb(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, target.PlayerId);
                        }
                    }
                    else if (attempt == MurderAttemptResult.BlankKill)
                    {
                        bomberBPlantBombButton.Timer = bomberBPlantBombButton.MaxTimer;
                        return;
                    }
                    else if (attempt == MurderAttemptResult.SuppressKill) return;
                },
                // HasButton
                () => { return CachedPlayer.LocalPlayer.PlayerControl == BomberB.bomberB && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && BomberA.bomberA != null && !BomberA.bomberA.Data.IsDead; },
                // CouldUse
                () =>
                {
                    var bomberA = BomberA.bomberA;
                    float distance = Vector2.Distance(CachedPlayer.LocalPlayer.PlayerControl.transform.localPosition, bomberA.transform.localPosition);

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && BomberA.bombTarget != null && BomberB.bombTarget != null && !bomberA.Data.IsDead && bomberA != null && distance < 1;
                },
                // OnMeetingEnds
                () =>
                {
                    bomberBReleaseBombButton.Timer = bomberBReleaseBombButton.MaxTimer;
                },
                BomberB.getReleaseButtonSprite(),
                CustomButton.ButtonPositions.lowerRowCenter,
                __instance,
                KeyCode.Q,
                false
            );

            undertakerDragButton = new CustomButton(
                () =>
                {
                    var bodyComponent = Undertaker.TargetBody;
                    if (Undertaker.DraggedBody == null && bodyComponent != null)
                    {
                        Undertaker.RpcDragBody(bodyComponent.ParentId);
                    }
                    else if (Undertaker.DraggedBody != null)
                    {
                        var position = Undertaker.DraggedBody.transform.position;
                        Undertaker.RpcDropBody(position);
                    }
                }, // Action OnClick
                () =>
                {
                    return Undertaker.undertaker != null &&
                           Undertaker.undertaker == CachedPlayer.LocalPlayer.PlayerControl &&
                           !CachedPlayer.LocalPlayer.Data.IsDead;
                }, // Bool HasButton
                () =>
                {
                    if (Undertaker.DraggedBody != null)
                    {
                        undertakerDragButton.Sprite = Undertaker.getDropButtonSprite();
                    }
                    else
                    {
                        undertakerDragButton.Sprite = Undertaker.getDragButtonSprite();
                    }
                    return ((Undertaker.TargetBody != null && Undertaker.DraggedBody == null)
                            || (Undertaker.DraggedBody != null && Undertaker.CanDropBody))
                           && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                }, // Bool CouldUse
                () => { }, // Action OnMeetingEnds
                Undertaker.getDragButtonSprite(), // Sprite sprite,
                CustomButton.ButtonPositions.upperRowLeft, // Vector3 PositionOffset
                __instance, // HudManager hudManager
                null, // String actionName,
                false, // bool HasEffect
                0f, // Float EffectDuration
                () => { }, // Action OnEffectEnds
                false, // Bool mirror = false
                "" // String buttonText = ""
            );

            // Sherlock Investigate
            sherlockInvestigateButton = new CustomButton(
                () =>
                {
                    string message = "";
                    foreach (var item in Sherlock.killLog)
                    {
                        float distance = Vector3.Distance(item.Item2.Item2, CachedPlayer.LocalPlayer.PlayerControl.transform.position);
                        if (distance < Sherlock.investigateDistance)
                        {
                            PlayerControl killer = Helpers.playerById(item.Item1);
                            PlayerControl target = Helpers.playerById(item.Item2.Item1);
                            string killerTeam = RoleInfo.GetRolesString(killer, useColors: true, showModifier: false, includeHidden: true);
                            message += string.Format(ModTranslation.getString("sherlockMessage2"), target.Data.PlayerName, killerTeam) + "\n";
                        }
                    }
                    if (message == "")
                    {
                        message = ModTranslation.getString("sherlockMessage1");
                    }
                    Sherlock.investigateMessage(message, 7f, Color.white);
                    Sherlock.numUsed += 1;
                    sherlockInvestigateButton.Timer = sherlockInvestigateButton.MaxTimer;
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Sherlock.sherlock && !Sherlock.sherlock.Data.IsDead; },
                () =>
                {
                    if (sherlockNumInvestigateText != null)
                    {
                        sherlockNumInvestigateText.text = $"{Sherlock.numUsed} / {Sherlock.getNumInvestigate()}";
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Sherlock.numUsed < Sherlock.getNumInvestigate();
                },
                () => { sherlockInvestigateButton.Timer = sherlockInvestigateButton.MaxTimer; },
                Sherlock.getInvestigateIcon(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );
            sherlockNumInvestigateText = GameObject.Instantiate(sherlockInvestigateButton.actionButton.cooldownTimerText, sherlockInvestigateButton.actionButton.cooldownTimerText.transform.parent);
            sherlockNumInvestigateText.text = "";
            sherlockNumInvestigateText.enableWordWrapping = false;
            sherlockNumInvestigateText.transform.localScale = Vector3.one * 0.5f;
            sherlockNumInvestigateText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Sherlock Watch
            sherlockWatchButton = new CustomButton(
                () => { },
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Sherlock.sherlock && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead; },
                () =>
                {
                    if (sherlockNumKillTimerText != null)
                    {
                        sherlockNumKillTimerText.text = $"{Sherlock.killTimerCounter}";
                    }
                    if (sherlockWatchButton.Timer <= 0)
                    {
                        Sherlock.killTimerCounter += 1;
                        sherlockWatchButton.Timer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Sherlock.numUsed < Sherlock.getNumInvestigate();
                },
                () => {
                    sherlockWatchButton.Timer = sherlockWatchButton.MaxTimer;
                    Sherlock.killTimerCounter = 0;
                },
                Sherlock.getWatchIcon(),
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.H
            );
            sherlockNumKillTimerText = GameObject.Instantiate(sherlockWatchButton.actionButton.cooldownTimerText, sherlockWatchButton.actionButton.cooldownTimerText.transform.parent);
            sherlockNumKillTimerText.text = "";
            sherlockNumKillTimerText.enableWordWrapping = false;
            sherlockNumKillTimerText.transform.localScale = Vector3.one * 0.5f;
            sherlockNumKillTimerText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Akujo Honmei
            akujoHonmeiButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSetHonmei, Hazel.SendOption.Reliable, -1);
                    writer.Write(Akujo.akujo.PlayerId);
                    writer.Write(Akujo.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.akujoSetHonmei(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, Akujo.currentTarget.PlayerId);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.honmei == null && Akujo.timeLeft > 0; },
                () =>
                {                    
                    return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.currentTarget != null && Akujo.honmei == null && Akujo.timeLeft > 0;
                },
                () => { akujoHonmeiButton.Timer = akujoHonmeiButton.MaxTimer; },
                Akujo.getHonmeiSprite(),
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.F
            );
            akujoTimeRemainingText = GameObject.Instantiate(akujoHonmeiButton.actionButton.cooldownTimerText, __instance.transform);
            akujoTimeRemainingText.text = "";
            akujoTimeRemainingText.enableWordWrapping = false;
            akujoTimeRemainingText.transform.localScale = Vector3.one * 0.45f;
            akujoTimeRemainingText.transform.localPosition = akujoHonmeiButton.actionButton.cooldownTimerText.transform.parent.localPosition + new Vector3(-0.1f, 0.35f, 0f);

            // Akujo Keep
            akujoBackupButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSetKeep, Hazel.SendOption.Reliable, -1);
                    writer.Write(Akujo.akujo.PlayerId);
                    writer.Write(Akujo.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.akujoSetKeep(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, Akujo.currentTarget.PlayerId);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.keepsLeft > 0 && Akujo.timeLeft > 0; },
                () =>
                {
                    if (akujoBackupLeftText != null)
                    {
                        if (Akujo.keepsLeft > 0)
                            akujoBackupLeftText.text = Akujo.keepsLeft.ToString();
                        else
                            akujoBackupLeftText.text = "";
                    }
                    return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.currentTarget != null && Akujo.keepsLeft > 0 && Akujo.timeLeft > 0;
                },
                () => { akujoBackupButton.Timer = akujoBackupButton.MaxTimer; },
                Akujo.getKeepSprite(),
                CustomButton.ButtonPositions.upperRowCenter,
                __instance,
                KeyCode.K
            );
            akujoBackupLeftText = GameObject.Instantiate(akujoBackupButton.actionButton.cooldownTimerText, akujoBackupButton.actionButton.cooldownTimerText.transform.parent);
            akujoBackupLeftText.text = "";
            akujoBackupLeftText.enableWordWrapping = false;
            akujoBackupLeftText.transform.localScale = Vector3.one * 0.5f;
            akujoBackupLeftText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Mimic(Assistant) Morph
            mimicAMorphButton = new CustomButton(
                () =>
                {
                    if (!MimicA.isMorph)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MimicMorph, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        writer.Write(MimicK.mimicK.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.mimicMorph(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, MimicK.mimicK.PlayerId);
                    }
                    else
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MimicResetMorph, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.mimicResetMorph(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    }
                },
                () => { return MimicA.mimicA != null && CachedPlayer.LocalPlayer.PlayerControl == MimicA.mimicA && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead; },
                () =>
                {
                    MimicA.isMorph = false;
                    MimicA.mimicA.setDefaultLook();
                },
                MimicA.getMorphSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                false
            );

            // Mimic(Assistant) Admin
            mimicAAdminButton = new CustomButton(
                () =>
                {
                    if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                    {
                        HudManager __instance = FastDestroyableSingleton<HudManager>.Instance;
                        __instance.InitMap();
                        MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
                    }
                    CachedPlayer.LocalPlayer.NetTransform.Halt();
                },
                () => {
                    return MimicA.mimicA != null && CachedPlayer.LocalPlayer.PlayerControl == MimicA.mimicA && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead
                    && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead;
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead; },
                () => { },
                MimicA.getAdminSprite(),
                CustomButton.ButtonPositions.upperRowCenter,
                __instance,
                KeyCode.H,
                false,
                FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
            );

            // Ninja Stealth
            ninjaButton = new CustomButton(
                () =>
                {
                    if (ninjaButton.isEffectActive)
                    {
                        ninjaButton.Timer = 0;
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.NinjaStealth, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.ninjaStealth(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, true);
                },
                () => { return Ninja.ninja != null && Ninja.ninja == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => {
                    ninjaButton.Timer = ninjaButton.MaxTimer = Ninja.stealthCooldown;
                    ninjaButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                    ninjaButton.isEffectActive = false;
                    Ninja.stealthed = false;
                    Ninja.ninja.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Ninja.addition);
                },
                Ninja.getButtonSprite(),
                //new Vector3(-1.8f, -0.06f, 0),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F, 
                true, 
                Ninja.stealthDuration,
                () =>
                {
                    ninjaButton.Timer = ninjaButton.MaxTimer = Ninja.stealthCooldown;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.NinjaStealth, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.ninjaStealth(CachedPlayer.LocalPlayer.PlayerId, false);

                    CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(Math.Max(CachedPlayer.LocalPlayer.PlayerControl.killTimer, Ninja.killPenalty));
                }
            );
            ninjaButton.effectCancellable = true;            

            // Serial Killer Suicide Countdown
            serialKillerButton = new CustomButton(
                () => { },
                () => { return SerialKiller.serialKiller != null && CachedPlayer.LocalPlayer.PlayerControl == SerialKiller.serialKiller && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && SerialKiller.isCountDown; },
                () => { return true; },
                () =>
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == SerialKiller.serialKiller)
                    {
                        SerialKiller.serialKiller.SetKillTimer(SerialKiller.killCooldown);
                        if (SerialKiller.resetTimer)
                        {
                            serialKillerButton.Timer = SerialKiller.suicideTimer;
                        }
                    }
                },
                SerialKiller.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                true,
                SerialKiller.suicideTimer,
                () =>
                {
                    byte targetId = SerialKiller.serialKiller.PlayerId;
                    MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SerialKillerSuicide, Hazel.SendOption.Reliable, -1); killWriter.Write(targetId);
                    killWriter.Write(targetId);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCProcedure.serialKillerSuicide(targetId);
                }
            );
            //UnityEngine.Object.Destroy(serialKillerButton.actionButton.buttonLabelText);
            //serialKillerButton.actionButton.buttonLabelText = UnityEngine.Object.Instantiate(__instance.AbilityButton.buttonLabelText, serialKillerButton.actionButton.transform);
            serialKillerButton.showButtonText = true;            
            serialKillerButton.buttonText = ModTranslation.getString("serialKillerSuicideText");
            serialKillerButton.isEffectActive = true;

            // Evil Tracker track
            evilTrackerButton = new CustomButton(
                () => {
                    if (Veteran.veteran != null && EvilTracker.currentTarget == Veteran.veteran && Veteran.alertActive)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, EvilTracker.evilTracker);
                        return;
                    }
                    EvilTracker.target = EvilTracker.currentTarget;
                },
                () => { return EvilTracker.target == null && CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead; },
                () => { return EvilTracker.currentTarget != null && EvilTracker.target == null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { evilTrackerButton.Timer = evilTrackerButton.MaxTimer; },
                EvilTracker.getEvilTrackerButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F
            );

            // Vulture Eat
            vultureEatButton = new CustomButton(
                () => {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask)) {
                        if (collider2D.tag == "DeadBody") {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported) {
                                Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance && CachedPlayer.LocalPlayer.PlayerControl.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false)) {
                                    GameData.PlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CleanBody, Hazel.SendOption.Reliable, -1);
                                    writer.Write(playerInfo.PlayerId);
                                    writer.Write(Vulture.vulture.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.cleanBody(playerInfo.PlayerId, Vulture.vulture.PlayerId);

                                    Vulture.cooldown = vultureEatButton.Timer = vultureEatButton.MaxTimer;
                                    SoundEffectsManager.play("vultureEat");
                                    break;
                                }
                            }
                        }
                    }
                },
                () => { return Vulture.vulture != null && Vulture.vulture == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return __instance.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
                Vulture.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowCenter,
                __instance,
                KeyCode.F
            );

            // EvilHacker button
            evilHackerButton = new CustomButton(
                () => {
                    CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt();
                    if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                    {
                        HudManager __instance = FastDestroyableSingleton<HudManager>.Instance;
                        __instance.InitMap();
                        MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
                    }
                },
                () => {
                    return EvilHacker.evilHacker != null &&
                      EvilHacker.evilHacker == CachedPlayer.LocalPlayer.PlayerControl &&
                      !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () => { },
                EvilHacker.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.H,
                false,
                FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
            );

            // Medium button
            mediumButton = new CustomButton(
                () => {
                    if (Medium.target != null) {
                        Medium.soulTarget = Medium.target;
                        mediumButton.HasEffect = true;
                        SoundEffectsManager.play("mediumAsk");
                    }
                },
                () => { return Medium.medium != null && Medium.medium == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (mediumButton.isEffectActive && Medium.target != Medium.soulTarget) {
                        Medium.soulTarget = null;
                        mediumButton.Timer = 0f;
                        mediumButton.isEffectActive = false;
                    }
                    return Medium.target != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => {
                    mediumButton.Timer = mediumButton.MaxTimer;
                    mediumButton.isEffectActive = false;
                    Medium.soulTarget = null;
                },
                Medium.getQuestionSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F,
                true,
                Medium.duration,
                () => {
                    mediumButton.Timer = mediumButton.MaxTimer;
                    if (Medium.target == null || Medium.target.player == null) return;
                    string msg = Medium.getInfo(Medium.target.player, Medium.target.killerIfExisting);
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, msg);

                    // Ghost Info
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(Medium.target.player.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.MediumInfo);
                    writer.Write(msg);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    // Remove soul
                    if (Medium.oneTimeUse) {
                        float closestDistance = float.MaxValue;
                        SpriteRenderer target = null;

                        foreach ((DeadPlayer db, Vector3 ps) in Medium.deadBodies) {
                            if (db == Medium.target) {
                                Tuple<DeadPlayer, Vector3> deadBody = Tuple.Create(db, ps);
                                Medium.deadBodies.Remove(deadBody);
                                break;
                            }

                        }
                        foreach (SpriteRenderer rend in Medium.souls) {
                            float distance = Vector2.Distance(rend.transform.position, CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition());
                            if (distance < closestDistance) {
                                closestDistance = distance;
                                target = rend;
                            }
                        }

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>((p) => {
                            if (target != null) {
                                var tmp = target.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                target.color = tmp;
                            }
                            if (p == 1f && target != null && target.gameObject != null) UnityEngine.Object.Destroy(target.gameObject);
                        })));

                        Medium.souls.Remove(target);
                    }
                    SoundEffectsManager.stop("mediumAsk");
                }
            );

            // Fortune Teller button
            fortuneTellerButtons = new List<CustomButton>();
            //HudManager hm = FastDestroyableSingleton<HudManager>.Instance;
            Vector3 fortuneTellerCalcPos(byte index)
            {
                int adjIndex = index < CachedPlayer.LocalPlayer.PlayerControl.PlayerId ? index : index - 1;
                return new Vector3(-0.25f, -0.15f, 0) + Vector3.right * adjIndex * 0.55f;
            }

            Action fortuneTellerButtonOnClick(byte index)
            {
                return () =>
                {
                    if (PlayerControl.LocalPlayer.CanMove && FortuneTeller.numUsed < 1 && FortuneTeller.canDivine(index))
                    {
                        PlayerControl p = Helpers.playerById(index);
                        FortuneTeller.divine(p);
                    }
                };
            };

            Func<bool> fortuneTellerHasButton(byte index)
            {
                return () =>
                {
                    return CachedPlayer.LocalPlayer.PlayerControl == FortuneTeller.fortuneTeller;
                };
            }

            void setButtonPos(byte index)
            {
                Vector3 pos = fortuneTellerCalcPos(index);
                Vector3 scale = new Vector3(0.4f, 0.5f, 1.0f);

                Vector3 iconBase = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;

                fortuneTellerButtons[index].PositionOffset = pos;
                //fortuneTellerButtons[index].LocalScale = scale;
                fortuneTellerButtons[index].actionButton.transform.localScale = scale;
                TORMapOptions.playerIcons[index].transform.localPosition = pos + iconBase;
            }

            void setIconPos(byte index, bool transparent)
            {
                TORMapOptions.playerIcons[index].transform.localScale = Vector3.one * 0.25f;
                TORMapOptions.playerIcons[index].gameObject.SetActive(CachedPlayer.LocalPlayer.PlayerControl.CanMove);
                TORMapOptions.playerIcons[index].setSemiTransparent(transparent);
            }

            Func<bool> fortuneTellerCouldUse(byte index)
            {
                return () =>
                {
                    //　占い師以外の場合、リソースがない場合はボタンを表示しない
                    if (!TORMapOptions.playerIcons.ContainsKey(index) ||
                        !CachedPlayer.LocalPlayer.PlayerControl == FortuneTeller.fortuneTeller ||
                        CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead ||
                        CachedPlayer.LocalPlayer.PlayerControl.PlayerId == index ||
                        !FortuneTeller.isCompletedNumTasks(FortuneTeller.fortuneTeller) ||
                        FortuneTeller.numUsed >= 1)
                    {
                        if (TORMapOptions.playerIcons.ContainsKey(index)) TORMapOptions.playerIcons[index].gameObject.SetActive(false);
                        if (fortuneTellerButtons.Count > index) fortuneTellerButtons[index].setActive(false);
                        return false;
                    }

                    // ボタンの位置を変更
                    setButtonPos(index);

                    // ボタンにテキストを設定
                    bool status = true;
                    if (FortuneTeller.playerStatus.ContainsKey(index))
                    {
                        status = FortuneTeller.playerStatus[index];
                    }

                    fortuneTellerButtons[index].showButtonText = true;
                    if (status)
                    {
                        var progress = FortuneTeller.progress.ContainsKey(index) ? FortuneTeller.progress[index] : 0f;
                        fortuneTellerButtons[index].buttonText = $"{progress:0.0}/{FortuneTeller.duration:0.0}";
                    }
                    else
                    {
                        fortuneTellerButtons[index].buttonText = ModTranslation.getString("fortuneTellerDead");
                    }

                    // アイコンの位置と透明度を変更
                    setIconPos(index, !FortuneTeller.canDivine(index));

                    TORMapOptions.playerIcons[index].gameObject.SetActive(!(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                      !MeetingHud.Instance &&
                      !ExileController.Instance && CachedPlayer.LocalPlayer.PlayerControl.CanMove);
                    fortuneTellerButtons[index].setActive(!(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                      !MeetingHud.Instance &&
                      !ExileController.Instance && CachedPlayer.LocalPlayer.PlayerControl.CanMove);
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && FortuneTeller.numUsed < 1 && FortuneTeller.canDivine(index);
                };
            }

            for (byte i = 0; i < 15; i++)
            {
                CustomButton fortuneTellerButton = new(
                    // Action OnClick
                    fortuneTellerButtonOnClick(i),
                    // bool HasButton
                    fortuneTellerHasButton(i),
                    // bool CouldUse
                    fortuneTellerCouldUse(i),
                    // Action OnMeetingEnds
                    () => { },
                    // sprite
                    null,
                    // position
                    Vector3.zero,
                    __instance,
                    // keyboard shortcut
                    KeyCode.None,
                    true
                )
                {
                    Timer = 0.0f,
                    MaxTimer = 0.0f
                };

                fortuneTellerButtons.Add(fortuneTellerButton);
            }

            // Pursuer button
            pursuerButton = new CustomButton(
                () => {
                    if (Pursuer.target != null) {
                        if (Pursuer.target == Veteran.veteran && Veteran.alertActive && Veteran.veteran != null)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Pursuer.pursuer);
                            return;
                        }
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetBlanked, Hazel.SendOption.Reliable, -1);
                        writer.Write(Pursuer.target.PlayerId);
                        writer.Write(Byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.setBlanked(Pursuer.target.PlayerId, Byte.MaxValue);

                        Pursuer.target = null;

                        Pursuer.blanks++;
                        pursuerButton.Timer = pursuerButton.MaxTimer;
                        SoundEffectsManager.play("pursuerBlank");
                    }

                },
                () => { return Pursuer.pursuer != null && Pursuer.pursuer == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && Pursuer.blanks < Pursuer.blanksNumber; },
                () => {
                    if (pursuerButtonBlanksText != null) pursuerButtonBlanksText.text = $"{Pursuer.blanksNumber - Pursuer.blanks}";

                    return Pursuer.blanksNumber > Pursuer.blanks && CachedPlayer.LocalPlayer.PlayerControl.CanMove && Pursuer.target != null;
                },
                () => { pursuerButton.Timer = pursuerButton.MaxTimer; },
                Pursuer.getTargetSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );

            // Pursuer button blanks left
            pursuerButtonBlanksText = GameObject.Instantiate(pursuerButton.actionButton.cooldownTimerText, pursuerButton.actionButton.cooldownTimerText.transform.parent);
            pursuerButtonBlanksText.text = "";
            pursuerButtonBlanksText.enableWordWrapping = false;
            pursuerButtonBlanksText.transform.localScale = Vector3.one * 0.5f;
            pursuerButtonBlanksText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);


            // Witch Spell button
            witchSpellButton = new CustomButton(
                () => {
                    if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == Witch.currentTarget)
                    {
                        Helpers.checkMurderAttemptAndKill(Veteran.veteran, Witch.witch);
                        return;
                    }
                    if (Witch.currentTarget != null) {
                        Witch.spellCastingTarget = Witch.currentTarget;
                        SoundEffectsManager.play("witchSpell");
                    }
                },
                () => { return Witch.witch != null && Witch.witch == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (witchSpellButton.isEffectActive && Witch.spellCastingTarget != Witch.currentTarget) {
                        Witch.spellCastingTarget = null;
                        witchSpellButton.Timer = 0f;
                        witchSpellButton.isEffectActive = false;
                    }
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Witch.currentTarget != null;
                },
                () => {
                    witchSpellButton.Timer = witchSpellButton.MaxTimer;
                    witchSpellButton.isEffectActive = false;
                    Witch.spellCastingTarget = null;
                },
                Witch.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                true,
                Witch.spellCastingDuration,
                () => {
                    if (Witch.spellCastingTarget == null) return;
                    MurderAttemptResult attempt = Helpers.checkMuderAttempt(Witch.witch, Witch.spellCastingTarget);
                    if (attempt == MurderAttemptResult.PerformKill) {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureSpelled, Hazel.SendOption.Reliable, -1);
                        writer.Write(Witch.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.setFutureSpelled(Witch.currentTarget.PlayerId);
                    }
                    if (attempt == MurderAttemptResult.BlankKill || attempt == MurderAttemptResult.PerformKill) {
                        Witch.currentCooldownAddition += Witch.cooldownAddition;
                        witchSpellButton.MaxTimer = Witch.cooldown + Witch.currentCooldownAddition;
                        Patches.PlayerControlFixedUpdatePatch.miniCooldownUpdate();  // Modifies the MaxTimer if the witch is the mini
                        witchSpellButton.Timer = witchSpellButton.MaxTimer;
                        if (Witch.triggerBothCooldowns) {
                            float multiplier = (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini) ? (Mini.isGrownUp() ? 0.66f : 2f) : 1f;
                            Witch.witch.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier;
                        }
                    } else {
                        witchSpellButton.Timer = 0f;
                    }
                    Witch.spellCastingTarget = null;
                }
            );

            sprintButton = new CustomButton(
                () => {
                    if (sprintButton.isEffectActive)
                    {
                        sprintButton.Timer = 0;
                        return;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SprinterSprint, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.sprinterSprint(PlayerControl.LocalPlayer.PlayerId, true);
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl == Sprinter.sprinter && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () => { sprintButton.Timer = sprintButton.MaxTimer = Sprinter.sprintCooldown; },
                Sprinter.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F,
                true,
                Sprinter.sprintDuration,
                () => {
                    sprintButton.Timer = sprintButton.MaxTimer = Sprinter.sprintCooldown;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SprinterSprint, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.sprinterSprint(PlayerControl.LocalPlayer.PlayerId, false);
                }
            );
            sprintButton.effectCancellable = true;

            // Assassin mark and assassinate button 
            assassinButton = new CustomButton(
                () => {
                    MessageWriter writer;
                    if (Assassin.assassinMarked != null) {
                        // Murder attempt with teleport
                        MurderAttemptResult attempt = Helpers.checkMuderAttempt(Assassin.assassin, Assassin.assassinMarked);
                        if (attempt == MurderAttemptResult.ReverseKill)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Assassin.assassin);
                            return;
                        }
                        if (attempt == MurderAttemptResult.PerformKill) {
                            // Create first trace before killing
                            var pos = CachedPlayer.LocalPlayer.transform.position;
                            byte[] buff = new byte[sizeof(float) * 2];
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                            writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceAssassinTrace, Hazel.SendOption.Reliable);
                            writer.WriteBytesAndSize(buff);
                            writer.EndMessage();
                            RPCProcedure.placeAssassinTrace(buff);

                            /*MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetInvisible, Hazel.SendOption.Reliable, -1);
                            invisibleWriter.Write(Assassin.assassin.PlayerId);
                            invisibleWriter.Write(byte.MinValue);
                            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                            RPCProcedure.setInvisible(Assassin.assassin.PlayerId, byte.MinValue);*/

                            // Perform Kill
                            MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                            writer2.Write(CachedPlayer.LocalPlayer.PlayerId);
                            writer2.Write(Assassin.assassinMarked.PlayerId);
                            writer2.Write(byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);
                            if (SubmergedCompatibility.IsSubmerged)
                            {
                                SubmergedCompatibility.ChangeFloor(Assassin.assassinMarked.transform.localPosition.y > -7);
                            }
                            RPCProcedure.uncheckedMurderPlayer(CachedPlayer.LocalPlayer.PlayerId, Assassin.assassinMarked.PlayerId, byte.MaxValue);

                            // Create Second trace after killing
                            pos = Assassin.assassinMarked.transform.position;
                            buff = new byte[sizeof(float) * 2];
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                            MessageWriter writer3 = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceAssassinTrace, Hazel.SendOption.Reliable);
                            writer3.WriteBytesAndSize(buff);
                            writer3.EndMessage();
                            RPCProcedure.placeAssassinTrace(buff);
                        }

                        if (attempt == MurderAttemptResult.BlankKill || attempt == MurderAttemptResult.PerformKill) {
                            assassinButton.Timer = assassinButton.MaxTimer;
                            Assassin.assassin.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        } else if (attempt == MurderAttemptResult.SuppressKill) {
                            assassinButton.Timer = 0f;
                        }
                        Assassin.assassinMarked = null;
                        return;
                    } 
                    if (Assassin.currentTarget != null) {
                        if (Assassin.currentTarget == Veteran.veteran && Veteran.alertActive && Veteran.veteran != null)
                        {
                            Helpers.checkMurderAttemptAndKill(Veteran.veteran, Assassin.assassin);
                            return;
                        }
                        Assassin.assassinMarked = Assassin.currentTarget;
                        assassinButton.Timer = 5f;
                        SoundEffectsManager.play("warlockCurse");

                        // Ghost Info
                        writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write((byte)RPCProcedure.GhostInfoTypes.AssassinMarked);
                        writer.Write(Assassin.assassinMarked.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                },
                () => { return Assassin.assassin != null && Assassin.assassin == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {  // CouldUse
                    assassinButton.Sprite = Assassin.assassinMarked != null ? Assassin.getKillButtonSprite() : Assassin.getMarkButtonSprite(); 
                    return (Assassin.currentTarget != null || Assassin.assassinMarked != null) && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => {  // on meeting ends
                    assassinButton.Timer = assassinButton.MaxTimer;
                    Assassin.assassinMarked = null;
                },
                Assassin.getMarkButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F                   
            );

            mayorMeetingButton = new CustomButton(
               () => {
                   CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                   Mayor.remoteMeetingsLeft--;
	               Helpers.handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                   RPCProcedure.uncheckedCmdReportDeadBody(CachedPlayer.LocalPlayer.PlayerId, Byte.MaxValue);

                   MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
                   writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                   writer.Write(Byte.MaxValue);
                   AmongUsClient.Instance.FinishRpcImmediately(writer);
                   mayorMeetingButton.Timer = 1f;
               },
               () => { return Mayor.mayor != null && Mayor.mayor == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead && Mayor.meetingButton; },
               () => {
                   mayorMeetingButton.actionButton.OverrideText(ModTranslation.getString("mayorEmergencyLeftText") + " (" + Mayor.remoteMeetingsLeft + ")");
                   bool sabotageActive = false;
                   foreach (PlayerTask task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                       if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles
                           || SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                           sabotageActive = true;
                   return !sabotageActive && CachedPlayer.LocalPlayer.PlayerControl.CanMove && (Mayor.remoteMeetingsLeft > 0);
               },
               () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
               Mayor.getMeetingSprite(),
               CustomButton.ButtonPositions.lowerRowRight,
               __instance,
               KeyCode.F,
               true,
               0f,
               () => {},
               false,
               ModTranslation.getString("mayorEmergencyMeetingText")
           );

            // Trapper button
            //trapperButton = new CustomButton(
                /*() => {


                    var pos = CachedPlayer.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetTrap, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.setTrap(buff);

                    SoundEffectsManager.play("trapperTrap");
                    trapperButton.Timer = trapperButton.MaxTimer;
                },
                () => { return Trapper.trapper != null && Trapper.trapper == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (trapperChargesText != null) trapperChargesText.text = $"{Trapper.charges} / {Trapper.maxCharges}";
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Trapper.charges > 0;
                },
                () => { trapperButton.Timer = trapperButton.MaxTimer; },
                Trapper.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F
            );*/

            // Bomber button
            /*bomberButton = new CustomButton(
                () => {
                    if (Helpers.checkMuderAttempt(Bomber.bomber, Bomber.bomber) != MurderAttemptResult.BlankKill) {
                        var pos = CachedPlayer.LocalPlayer.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceBomb, Hazel.SendOption.Reliable);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        RPCProcedure.placeBomb(buff);

                        SoundEffectsManager.play("trapperTrap");
                    }

                    bomberButton.Timer = bomberButton.MaxTimer;
                    Bomber.isPlanted = true;
                },
                () => { return Bomber.bomber != null && Bomber.bomber == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Bomber.isPlanted; },
                () => { bomberButton.Timer = bomberButton.MaxTimer; },
                Bomber.getButtonSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.F,
                true,
                Bomber.destructionTime,
                () => {
                    bomberButton.Timer = bomberButton.MaxTimer;
                    bomberButton.isEffectActive = false;
                    bomberButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                }
            );

            defuseButton = new CustomButton(
                () => {
                    defuseButton.HasEffect = true;
                },
                () => {
                    if (shifterShiftButton.HasButton())
                        defuseButton.PositionOffset = new Vector3(0f, 2f, 0f);
                    else
                        defuseButton.PositionOffset = new Vector3(0f, 1f, 0f);
                    return Bomber.bomb != null && Bomb.canDefuse && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (defuseButton.isEffectActive && !Bomb.canDefuse) {
                        defuseButton.Timer = 0f;
                        defuseButton.isEffectActive = false;
                    }
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove; 
                },
                () => {
                    defuseButton.Timer = 0f;
                    defuseButton.isEffectActive = false;
                },
                Bomb.getDefuseSprite(),
                new Vector3(0f, 1f, 0),
                __instance,
                null,
                true,
                Bomber.defuseDuration,
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DefuseBomb, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.defuseBomb();

                    defuseButton.Timer = 0f;
                    Bomb.canDefuse = false;
                },
                true
            );*/

            thiefKillButton = new CustomButton(
                () => {
                    PlayerControl thief = Thief.thief;
                    PlayerControl target = Thief.currentTarget;
                    var result = Helpers.checkMuderAttempt(thief, target);
                    if (result == MurderAttemptResult.BlankKill) {
                        thiefKillButton.Timer = thiefKillButton.MaxTimer;
                        return;
                    }

                    if (Thief.suicideFlag) {
                        // Suicide
                        MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                        writer2.Write(thief.PlayerId);
                        writer2.Write(thief.PlayerId);
                        writer2.Write(0);
                        RPCProcedure.uncheckedMurderPlayer(thief.PlayerId, thief.PlayerId, 0);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        Thief.thief.clearAllTasks();
                    }

                    // Steal role if survived.
                    if (!Thief.thief.Data.IsDead && result == MurderAttemptResult.PerformKill) {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ThiefStealsRole, Hazel.SendOption.Reliable, -1);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.thiefStealsRole(target.PlayerId);
                    }
                    // Kill the victim (after becoming their role - so that no win is triggered for other teams)
                    if (result == MurderAttemptResult.PerformKill) {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                        writer.Write(thief.PlayerId);
                        writer.Write(target.PlayerId);
                        writer.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.uncheckedMurderPlayer(thief.PlayerId, target.PlayerId, byte.MaxValue);
                    }
                },
               () => { return Thief.thief != null && CachedPlayer.LocalPlayer.PlayerControl == Thief.thief && !CachedPlayer.LocalPlayer.Data.IsDead; },
               () => { return Thief.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
               () => { thiefKillButton.Timer = thiefKillButton.MaxTimer; },
               __instance.KillButton.graphic.sprite,
               CustomButton.ButtonPositions.upperRowRight,
               __instance,
               KeyCode.Q
               );

            // Trapper Charges
            //trapperChargesText = GameObject.Instantiate(trapperButton.actionButton.cooldownTimerText, trapperButton.actionButton.cooldownTimerText.transform.parent);
            //trapperChargesText.text = "";
            //trapperChargesText.enableWordWrapping = false;
            //trapperChargesText.transform.localScale = Vector3.one * 0.5f;
            //trapperChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            zoomOutButton = new CustomButton(
                () => { Helpers.toggleZoom();
                },
                () => { if (CachedPlayer.LocalPlayer.PlayerControl == null || !CachedPlayer.LocalPlayer.Data.IsDead || CachedPlayer.LocalPlayer.Data.Role.IsImpostor) return false;
                    var (playerCompleted, playerTotal) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data);
                    int numberOfLeftTasks = playerTotal - playerCompleted;
                    return numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.getBool();
                },
                () => { return true; },
                () => { return; },
                Helpers.loadSpriteFromResources("TheOtherRoles.Resources.MinusButton.png", 150f),  // Invisible button!
                new Vector3(0.4f, 2.8f, 0),
                __instance,
                KeyCode.KeypadPlus
                );
            zoomOutButton.Timer = 0f;


            hunterLighterButton = new CustomButton(
                () => {
                    Hunter.lightActive.Add(CachedPlayer.LocalPlayer.PlayerId);
                    SoundEffectsManager.play("lighterLight");

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareTimer, Hazel.SendOption.Reliable, -1);
                    writer.Write(Hunter.lightPunish);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.shareTimer(Hunter.lightPunish);
                },
                () => { return HideNSeek.isHunter() && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return true; },
                () => {
                    hunterLighterButton.Timer = 30f;
                    hunterLighterButton.isEffectActive = false;
                    hunterLighterButton.actionButton.graphic.color = Palette.EnabledColor;
                },
                Hunter.getLightSprite(),
                CustomButton.ButtonPositions.upperRowFarLeft,
                __instance,
                KeyCode.F,
                true,
                Hunter.lightDuration,
                () => {
                    Hunter.lightActive.Remove(CachedPlayer.LocalPlayer.PlayerId);
                    hunterLighterButton.Timer = hunterLighterButton.MaxTimer;
                    SoundEffectsManager.play("lighterLight");
                }
            );

            hunterAdminTableButton = new CustomButton(
               () => {
                   if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled) {
                       HudManager __instance = FastDestroyableSingleton<HudManager>.Instance;
                       __instance.InitMap();
                       MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: false);
                   }

                   CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 

                   MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareTimer, Hazel.SendOption.Reliable, -1);
                   writer.Write(Hunter.AdminPunish); 
                   AmongUsClient.Instance.FinishRpcImmediately(writer);
                   RPCProcedure.shareTimer(Hunter.AdminPunish);
               },
               () => { return HideNSeek.isHunter() && !CachedPlayer.LocalPlayer.Data.IsDead; },
               () => { return true; },
               () => {
                   hunterAdminTableButton.Timer = hunterAdminTableButton.MaxTimer;
                   hunterAdminTableButton.isEffectActive = false;
                   hunterAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
               },
               Hacker.getAdminSprite(),
               CustomButton.ButtonPositions.lowerRowCenter,
               __instance,
               KeyCode.G,
               true,
               Hunter.AdminDuration,
               () => {
                   hunterAdminTableButton.Timer = hunterAdminTableButton.MaxTimer;
                   if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
               },
               false,
               FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
            );

            hunterArrowButton = new CustomButton(
                () => {
                    Hunter.arrowActive = true;
                    SoundEffectsManager.play("trackerTrackPlayer");

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareTimer, Hazel.SendOption.Reliable, -1);
                    writer.Write(Hunter.ArrowPunish);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.shareTimer(Hunter.ArrowPunish);
                },
                () => { return HideNSeek.isHunter() && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => { return true; },
                () => {
                    hunterArrowButton.Timer = 30f;
                    hunterArrowButton.isEffectActive = false;
                    hunterArrowButton.actionButton.graphic.color = Palette.EnabledColor;
                },
                Hunter.getArrowSprite(),
                CustomButton.ButtonPositions.upperRowLeft,
                __instance,
                KeyCode.R,
                true,
                Hunter.ArrowDuration,
                () => {
                    Hunter.arrowActive = false;
                    hunterArrowButton.Timer = hunterArrowButton.MaxTimer;
                    SoundEffectsManager.play("trackerTrackPlayer");
                }
            );

            huntedShieldButton = new CustomButton(
                () => {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.HuntedShield, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.huntedShield(CachedPlayer.LocalPlayer.PlayerId);
                    SoundEffectsManager.play("timemasterShield");

                    Hunted.shieldCount--;
                },
                () => { return HideNSeek.isHunted() && !CachedPlayer.LocalPlayer.Data.IsDead; },
                () => {
                    if (huntedShieldCountText != null) huntedShieldCountText.text = $"{Hunted.shieldCount}";
                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Hunted.shieldCount > 0;
                },
                () => {
                    huntedShieldButton.Timer = huntedShieldButton.MaxTimer;
                    huntedShieldButton.isEffectActive = false;
                    huntedShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                TimeMaster.getButtonSprite(),
                CustomButton.ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F,
                true,
                Hunted.shieldDuration,
                () => {
                    huntedShieldButton.Timer = huntedShieldButton.MaxTimer;
                    SoundEffectsManager.stop("timemasterShield");

                }
            );

            huntedShieldCountText = GameObject.Instantiate(huntedShieldButton.actionButton.cooldownTimerText, huntedShieldButton.actionButton.cooldownTimerText.transform.parent);
            huntedShieldCountText.text = "";
            huntedShieldCountText.enableWordWrapping = false;
            huntedShieldCountText.transform.localScale = Vector3.one * 0.5f;
            huntedShieldCountText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

            // Set the default (or settings from the previous game) timers / durations when spawning the buttons
            initialized = true;
            setCustomButtonCooldowns();
            deputyHandcuffedButtons = new Dictionary<byte, List<CustomButton>>();
            
        }
    }
}
