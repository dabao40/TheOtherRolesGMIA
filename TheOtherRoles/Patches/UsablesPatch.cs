using HarmonyLib;
using System;
using Hazel;
using UnityEngine;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.TORMapOptions;
using System.Collections.Generic;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TheOtherRoles.Objects;
using TheOtherRoles.CustomGameModes;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using TheOtherRoles.Modules;

namespace TheOtherRoles.Patches {

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentCanUsePatch
    {
        public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] ref bool canUse, [HarmonyArgument(2)] ref bool couldUse) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;

            bool roleCouldUse = @object.roleCanUseVents();

            if (__instance.name.StartsWith("SealedVent_")) {
                canUse = couldUse = false;
                __result = num;
                return false;
            }

            // Submerged Compatability if needed:
            if (SubmergedCompatibility.IsSubmerged) {
                // as submerged does, only change stuff for vents 9 and 14 of submerged. Code partially provided by AlexejheroYTB
                if (SubmergedCompatibility.getInTransition()) {
                    __result = float.MaxValue;
                    return canUse = couldUse = false;
                }                
                switch (__instance.Id) {
                    case 9:  // Cannot enter vent 9 (Engine Room Exit Only Vent)!
                        if (CachedPlayer.LocalPlayer.PlayerControl.inVent) break;
                        __result = float.MaxValue;
                        return canUse = couldUse = false;                    
                    case 14: // Lower Central
                        __result = float.MaxValue;
                        couldUse = roleCouldUse && !pc.IsDead && (@object.CanMove || @object.inVent);
                        canUse = couldUse;
                        if (canUse) {
                            Vector3 center = @object.Collider.bounds.center;
                            Vector3 position = __instance.transform.position;
                            __result = Vector2.Distance(center, position);
                            canUse &= __result <= __instance.UsableDistance;
                        }
                        return false;
                }
            }

            var usableDistance = __instance.UsableDistance;
            if (__instance.name.StartsWith("JackInTheBoxVent_")) {
                if(Trickster.trickster != CachedPlayer.LocalPlayer.PlayerControl) {
                    // Only the Trickster can use the Jack-In-The-Boxes!
                    canUse = false;
                    couldUse = false;
                    __result = num;
                    return false; 
                } else {
                    // Reduce the usable distance to reduce the risk of gettings stuck while trying to jump into the box if it's placed near objects
                    usableDistance = 0.4f; 
                }
            }

            couldUse = (@object.inVent || roleCouldUse) && !pc.IsDead && (@object.CanMove || @object.inVent);
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance(center, position);
                canUse &= (num <= usableDistance && (!PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false) || __instance.name.StartsWith("JackInTheBoxVent_")));
            }
            __result = num;
            return false;
        }
    }

    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    class VentButtonDoClickPatch {
        static  bool Prefix(VentButton __instance) {
            // Manually modifying the VentButton to use Vent.Use again in order to trigger the Vent.Use prefix patch
		    if (__instance.currentTarget != null && !Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId)) __instance.currentTarget.Use();
            return false;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
    public static class VentUsePatch {
        public static bool Prefix(Vent __instance) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;
            // Deputy handcuff disables the vents
            if (Deputy.handcuffedPlayers.Contains(CachedPlayer.LocalPlayer.PlayerId)) {
                Deputy.setHandcuffedKnows();
                return false;
            }
            if (Trap.isTrapped(CachedPlayer.LocalPlayer.PlayerControl)) return false;
            if (CachedPlayer.LocalPlayer.PlayerControl == Undertaker.undertaker && Undertaker.disableVent && Undertaker.DraggedBody != null) return false;

            bool canUse;
            bool couldUse;
            __instance.CanUse(CachedPlayer.LocalPlayer.PlayerControl.Data, out canUse, out couldUse);
            bool canMoveInVents = CachedPlayer.LocalPlayer.PlayerControl != Spy.spy && CachedPlayer.LocalPlayer.PlayerControl != Jester.jester && !Madmate.madmate.Contains(CachedPlayer.LocalPlayer.PlayerControl) && CachedPlayer.LocalPlayer.PlayerControl != CreatedMadmate.createdMadmate; //&& !Trapper.playersOnMap.Contains(CachedPlayer.LocalPlayer.PlayerControl)
            if (CachedPlayer.LocalPlayer.PlayerControl == Engineer.engineer) canMoveInVents = true;
            if (!canUse) return false; // No need to execute the native method as using is disallowed anyways

            bool isEnter = !CachedPlayer.LocalPlayer.PlayerControl.inVent;
            
            if (__instance.name.StartsWith("JackInTheBoxVent_")) {
                __instance.SetButtons(isEnter && canMoveInVents);
                MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UseUncheckedVent, Hazel.SendOption.Reliable);
                writer.WritePacked(__instance.Id);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                writer.Write(isEnter ? byte.MaxValue : (byte)0);
                writer.EndMessage();
                RPCProcedure.useUncheckedVent(__instance.Id, CachedPlayer.LocalPlayer.PlayerId, isEnter ? byte.MaxValue : (byte)0);
                SoundEffectsManager.play("tricksterUseBoxVent");
                return false;
            }

            if(isEnter) {
                CachedPlayer.LocalPlayer.PlayerPhysics.RpcEnterVent(__instance.Id);
            } else {
                CachedPlayer.LocalPlayer.PlayerPhysics.RpcExitVent(__instance.Id);
            }
            __instance.SetButtons(isEnter && canMoveInVents);
            return false;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.TryMoveToVent))]
    public static class MoveToVentPatch {
        public static bool Prefix(Vent otherVent) {
            //return !Trapper.playersOnMap.Contains(CachedPlayer.LocalPlayer.PlayerControl);
            return true;
        }
        public static void Postfix(Vent otherVent)
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == Tracker.tracked && Tracker.tracked != null)
                Tracker.unlockAch(GameStatistics.currentTime);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class VentButtonVisibilityPatch {
        static void Postfix(PlayerControl __instance) {
            if (__instance.AmOwner && __instance.roleCanUseVents() && FastDestroyableSingleton<HudManager>.Instance.ReportButton.isActiveAndEnabled) {
                FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Show();
            }
        }
    }

    [HarmonyPatch(typeof(VentButton), nameof(VentButton.SetTarget))]
    class VentButtonSetTargetPatch {
        static Sprite defaultVentSprite = null;
        static void Postfix(VentButton __instance) {
            // Trickster render special vent button
            if (Trickster.trickster != null && Trickster.trickster == CachedPlayer.LocalPlayer.PlayerControl) {
                if (defaultVentSprite == null) defaultVentSprite = __instance.graphic.sprite;
                bool isSpecialVent = __instance.currentTarget != null && __instance.currentTarget.gameObject != null && __instance.currentTarget.gameObject.name.StartsWith("JackInTheBoxVent_");
                __instance.graphic.sprite = isSpecialVent ?  Trickster.getTricksterVentButtonSprite() : defaultVentSprite;
                __instance.buttonLabelText.enabled = !isSpecialVent;
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    class KillButtonDoClickPatch {
        public static bool Prefix(KillButton __instance) {
            if (__instance.isActiveAndEnabled && __instance.currentTarget && !__instance.isCoolingDown && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl.CanMove) {
                // Ninja doesn't get teleported to the body on stealth
                bool showAnimation = true;
                if (CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja && Ninja.stealthed)
                {
                    showAnimation = false;
                }
                
                // Deputy handcuff update.
                if (Deputy.handcuffedPlayers.Contains(CachedPlayer.LocalPlayer.PlayerId)) {
                    Deputy.setHandcuffedKnows();
                    return false;
                }
                
                // Use an unchecked kill command, to allow shorter kill cooldowns etc. without getting kicked
                MurderAttemptResult res = Helpers.checkMurderAttemptAndKill(CachedPlayer.LocalPlayer.PlayerControl, __instance.currentTarget, showAnimation: showAnimation);
                // Handle blank kill
                if (res == MurderAttemptResult.BlankKill) {
                    CachedPlayer.LocalPlayer.PlayerControl.killTimer = CachedPlayer.LocalPlayer.PlayerControl.GetKillCooldown();
                    if (CachedPlayer.LocalPlayer.PlayerControl == Cleaner.cleaner)
                        Cleaner.cleaner.killTimer = HudManagerStartPatch.cleanerCleanButton.Timer = HudManagerStartPatch.cleanerCleanButton.MaxTimer;
                    else if (CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock)
                        Warlock.warlock.killTimer = HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
                    else if (CachedPlayer.LocalPlayer.PlayerControl == Mini.mini && Mini.mini.Data.Role.IsImpostor)
                        Mini.mini.SetKillTimer(CachedPlayer.LocalPlayer.PlayerControl.GetKillCooldown() * (Mini.isGrownUp() ? 0.66f : 2f));
                    else if (CachedPlayer.LocalPlayer.PlayerControl == Witch.witch)
                        Witch.witch.killTimer = HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;
                    else if (CachedPlayer.LocalPlayer.PlayerControl == Assassin.assassin)
                        Assassin.assassin.killTimer = HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;
                }
                __instance.SetTarget(null);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
    public static class MapBehaviourShowPatch {
        public static void Prefix(MapBehaviour __instance, ref MapOptions opts) {
            bool blockSabotageJanitor = Janitor.janitor != null && Janitor.janitor == CachedPlayer.LocalPlayer.PlayerControl;
            bool blockSabotageMafioso = Mafioso.mafioso != null && Mafioso.mafioso == CachedPlayer.LocalPlayer.PlayerControl && Godfather.godfather != null && !Godfather.godfather.Data.IsDead;
            if (blockSabotageJanitor || blockSabotageMafioso) {
                if (opts.Mode == MapOptions.Modes.Sabotage) opts.Mode = MapOptions.Modes.Normal;
            }
        }
    }

    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public static class SabotageButtonDoClickPatch
    {
        public static bool Prefix(SabotageButton __instance)
        {
            // The sabotage button behaves just fine if it's a regular impostor
            if (CachedPlayer.LocalPlayer.PlayerControl.Data.Role.TeamType == RoleTeamTypes.Impostor) return true;

            DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new MapOptions
            {
                Mode = MapOptions.Modes.Sabotage
            });
            return false;
        }
    }

    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    class ReportButtonDoClickPatch {
        public static bool Prefix(ReportButton __instance) {
            if (__instance.isActiveAndEnabled && Deputy.handcuffedPlayers.Contains(CachedPlayer.LocalPlayer.PlayerId) && __instance.graphic.color == Palette.EnabledColor) Deputy.setHandcuffedKnows();
            return !Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId);
        }
    }

    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    class EmergencyMinigameUpdatePatch {
        static void Postfix(EmergencyMinigame __instance) {
            var roleCanCallEmergency = true;
            var statusText = "";

            // Deactivate emergency button for Swapper
            if (Swapper.swapper != null && Swapper.swapper == CachedPlayer.LocalPlayer.PlayerControl && !Swapper.canCallEmergency) {
                roleCanCallEmergency = false;
                statusText = ModTranslation.getString("swapperMeetingButton");
            }
            // Potentially deactivate emergency button for Jester
            if (Jester.jester != null && Jester.jester == CachedPlayer.LocalPlayer.PlayerControl && !Jester.canCallEmergency) {
                roleCanCallEmergency = false;
                statusText = ModTranslation.getString("jesterMeetingButton");
            }
            // Potentially deactivate emergency button for Lawyer/Prosecutor
            if (Lawyer.lawyer != null && Lawyer.lawyer == CachedPlayer.LocalPlayer.PlayerControl && Lawyer.winsAfterMeetings) {
                roleCanCallEmergency = false;
                statusText = string.Format(ModTranslation.getString("lawyerMeetingButton"), Lawyer.neededMeetings - Lawyer.meetings);
                //if (Lawyer.isProsecutor) statusText = "The Prosecutor can't start an emergency meeting";
            }
            // Potentially deactivate emergency button for Fortune Teller
            if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == CachedPlayer.LocalPlayer.PlayerControl && FortuneTeller.isCompletedNumTasks(CachedPlayer.LocalPlayer.PlayerControl))
            {
                roleCanCallEmergency = false;
                statusText = ModTranslation.getString("fortuneTellerMeetingButton");
            }
            // Potentially deactivate emergency button for Prophet
            if (Prophet.prophet != null && Prophet.prophet == CachedPlayer.LocalPlayer.PlayerControl && !Prophet.canCallEmergency)
            {
                roleCanCallEmergency = false;
                statusText = ModTranslation.getString("prophetMeetingButton");
            }

            if (!roleCanCallEmergency) {
                __instance.StatusText.text = statusText;
                __instance.NumberText.text = string.Empty;
                __instance.ClosedLid.gameObject.SetActive(true);
                __instance.OpenLid.gameObject.SetActive(false);
                __instance.ButtonActive = false;
                return;
            }

            // Handle max number of meetings
            if (__instance.state == 1) {
                int localRemaining = CachedPlayer.LocalPlayer.PlayerControl.RemainingEmergencies;
                int teamRemaining = Mathf.Max(0, maxNumberOfMeetings - meetingsCount);
                int remaining = Mathf.Min(localRemaining, (Mayor.mayor != null && Mayor.mayor == CachedPlayer.LocalPlayer.PlayerControl) ? 1 : teamRemaining);
                __instance.NumberText.text = String.Format(ModTranslation.getString("meetingCount"), localRemaining.ToString(), teamRemaining.ToString());
                __instance.ButtonActive = remaining > 0;
                __instance.ClosedLid.gameObject.SetActive(!__instance.ButtonActive);
                __instance.OpenLid.gameObject.SetActive(__instance.ButtonActive);
				return;
			}
        }
    }

    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static class ConsoleCanUsePatch {
        public static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse) {
            canUse = couldUse = false;
            if (Swapper.swapper != null && Swapper.swapper == CachedPlayer.LocalPlayer.PlayerControl)
                return !__instance.TaskTypes.Any(x => x == TaskTypes.FixLights || x == TaskTypes.FixComms);
            if (Madmate.madmate != null && Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId) && __instance.AllowImpostor)
                return !__instance.TaskTypes.Any(x => (!Madmate.canFixComm && x == TaskTypes.FixComms) || x == TaskTypes.FixLights);
            if (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == CachedPlayer.LocalPlayer.PlayerControl && __instance.AllowImpostor)
                return !__instance.TaskTypes.Any(x => (!CreatedMadmate.canFixComm && x == TaskTypes.FixComms) || x == TaskTypes.FixLights);
            if (JekyllAndHyde.jekyllAndHyde != null && JekyllAndHyde.jekyllAndHyde == CachedPlayer.LocalPlayer.PlayerControl && !JekyllAndHyde.isJekyll())
                return __instance.TaskTypes.Any(x => x == TaskTypes.FixComms || x == TaskTypes.FixLights || x == TaskTypes.RestoreOxy || x == TaskTypes.StopCharles || x == TaskTypes.ResetSeismic || x == TaskTypes.ResetReactor);
            if (Fox.fox != null && Fox.fox == CachedPlayer.LocalPlayer.PlayerControl)
                return !__instance.TaskTypes.Any(x => x == TaskTypes.FixComms || x == TaskTypes.FixLights || x == TaskTypes.RestoreOxy || x == TaskTypes.StopCharles || x == TaskTypes.ResetSeismic || x == TaskTypes.ResetReactor || (Fox.stealthed && x == TaskTypes.None));
            if (__instance.AllowImpostor) return true;
            if (!Helpers.hasFakeTasks(pc.Object)) return true;
            __result = float.MaxValue;
            return false;
        }
    }

    [HarmonyPatch(typeof(TuneRadioMinigame), nameof(TuneRadioMinigame.Begin))]
    class CommsMinigameBeginPatch {
        static void Postfix(TuneRadioMinigame __instance) {
            // Block Swapper from fixing comms. Still looking for a better way to do this, but deleting the task doesn't seem like a viable option since then the camera, admin table, ... work while comms are out
            if ((Swapper.swapper != null && Swapper.swapper == CachedPlayer.LocalPlayer.PlayerControl) ||
                (Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId) && !Madmate.canFixComm) ||
                (CreatedMadmate.createdMadmate != null && CachedPlayer.LocalPlayer.PlayerControl == CreatedMadmate.createdMadmate && !CreatedMadmate.canFixComm)) {
                __instance.Close();
            }
        }
    }

    [HarmonyPatch(typeof(SwitchMinigame), nameof(SwitchMinigame.Begin))]
    class LightsMinigameBeginPatch {
        static void Postfix(SwitchMinigame __instance) {
            // Block Swapper from fixing lights. One could also just delete the PlayerTask, but I wanted to do it the same way as with coms for now.
            if ((Swapper.swapper != null && Swapper.swapper == CachedPlayer.LocalPlayer.PlayerControl) ||
                Madmate.madmate.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId) || 
                (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == CachedPlayer.LocalPlayer.PlayerControl)) {
                __instance.Close();
            }
        }
    }

    [HarmonyPatch]
    class VitalsMinigamePatch {
        private static List<TMPro.TextMeshPro> hackerTexts = new();

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
        class VitalsMinigameStartPatch {
            private static float[] PanelAreaScale = { 1f, 0.95f, 0.76f };
            private static (int x, int y)[] PanelAreaSize = { (3, 5), (3, 6), (4, 6) };
            private static Vector3[] PanelAreaOffset = { new Vector3(0.0f, 0.0f, -1f), new Vector3(0.1f, 0.145f, -1f), new Vector3(-0.555f, 0.0f, -1f) };
            private static (float x, float y)[] PanelAreaMultiplier = { (1f, 1f), (1f, 0.89f), (275f * (float)Math.PI / 887f, 1f) };

            private static Vector3 ToVoteAreaPos(VitalsMinigame minigame, int index, int arrangeType) => Helpers.convertPos(index, arrangeType, PanelAreaSize, new Vector3(minigame.XStart, minigame.YStart, -1f), PanelAreaOffset, new Vector3(minigame.XOffset, minigame.YOffset), PanelAreaScale, PanelAreaMultiplier);

            static void Postfix(VitalsMinigame __instance) {
                __instance.BatteryText.gameObject.SetActive(false);
                VitalsMinigameUpdatePatch.UpdateVitals(__instance);
                int index = 0;
                int displayType = Helpers.GetDisplayType(__instance.vitals.Count);
                foreach (VitalsPanel vital in (Il2CppArrayBase<VitalsPanel>)__instance.vitals)
                {
                    var color = vital.PlayerInfo.IsDead && !Busker.buskerList.Any(x => x == vital.PlayerInfo.PlayerId) ? Palette.HalfWhite : Palette.White;
                    vital.PlayerIcon.cosmetics.SetSkin(vital.PlayerInfo.DefaultOutfit.SkinId, vital.PlayerInfo.DefaultOutfit.ColorId, (Action)null);
                    vital.PlayerIcon.cosmetics.SetHatColor(color);
                    vital.PlayerIcon.cosmetics.SetVisorAlpha(color.a);
                    vital.transform.localPosition = ToVoteAreaPos(__instance, index, displayType);
                    Transform transform = vital.transform;
                    transform.localScale *= PanelAreaScale[displayType];
                    ++index;
                }

                if (Hacker.hacker != null && CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker) {
                    hackerTexts = new List<TMPro.TextMeshPro>();
                    foreach (VitalsPanel panel in __instance.vitals) {
                        TMPro.TextMeshPro text = UnityEngine.Object.Instantiate(__instance.SabText, panel.transform);
                        hackerTexts.Add(text);
                        UnityEngine.Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                        text.gameObject.SetActive(false);
                        text.transform.localScale = Vector3.one * 0.75f * PanelAreaScale[displayType];
                        text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f) * PanelAreaScale[displayType];
                    }
                }

                //Fix Visor in Vitals
                foreach (VitalsPanel panel in __instance.vitals) {
                    if (panel.PlayerIcon != null && panel.PlayerIcon.cosmetics.skin != null) {
                         panel.PlayerIcon.cosmetics.skin.transform.position = new Vector3(0, 0, 0f);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        class VitalsMinigameUpdatePatch {

            public static void UpdateVitals(VitalsMinigame __instance)
            {
                if (__instance.SabText.isActiveAndEnabled && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer))
                {
                    __instance.SabText.gameObject.SetActive(false);
                    foreach (var v in __instance.vitals) v.gameObject.SetActive(true);
                }
                else if (!__instance.SabText.isActiveAndEnabled && PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer))
                {
                    __instance.SabText.gameObject.SetActive(true);
                    foreach (var v in __instance.vitals) v.gameObject.SetActive(false);
                }

                var vitals = VitalsStatePatch.VitalsFromActuals;
                foreach (var v in __instance.vitals)
                {
                    var myInfo = vitals.Players.FirstOrDefault(p => p.playerId == v.PlayerInfo.PlayerId);
                    if (myInfo.state == VitalsState.Disconnected)
                    {
                        if (!v.IsDiscon)
                        {
                            v.SetDisconnected();
                        }
                    }
                    else if (myInfo.state == VitalsState.Dead && Busker.buskerList.FindAll(x => x == v.PlayerInfo.PlayerId).Count <= 0)
                    {
                        if (!v.IsDead)
                        {
                            v.SetDead();
                            v.Cardio.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (v.IsDiscon || v.IsDead)
                        {
                            v.IsDiscon = false;
                            v.IsDead = false;
                            v.SetAlive();
                            v.Background.sprite = __instance.PanelPrefab.Background.sprite;
                            v.Cardio.gameObject.SetActive(true);
                        }
                    }
                }
            }

            static bool Prefix(VitalsMinigame __instance)
            {
                UpdateVitals(__instance);
                return false;
            }

            static void Postfix(VitalsMinigame __instance) {
                // Restrict information for Busker
                if (Busker.busker != null && CachedPlayer.LocalPlayer.PlayerControl == Busker.busker && Busker.pseudocideFlag && Busker.restrictInformation)
                {
                    __instance.SabText.gameObject.SetActive(true);
                    __instance.SabText.text = "[ I N F O  R E S T R I C T E D ]";
                    for (int k = 0; k < __instance.vitals.Length; k++)
                    {
                        VitalsPanel vitalsPanel = __instance.vitals[k];
                        vitalsPanel.gameObject.SetActive(false);
                    }
                    return;
                }

                // Hacker show time since death
                
                if (Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl && Hacker.hackerTimer > 0) {
                    for (int k = 0; k < __instance.vitals.Length; k++) {
                        VitalsPanel vitalsPanel = __instance.vitals[k];
                        NetworkedPlayerInfo player = vitalsPanel.PlayerInfo;

                        // Hacker update
                        if (vitalsPanel.IsDead) {
                            DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == player?.PlayerId)?.FirstOrDefault();
                            if (deadPlayer != null && k < hackerTexts.Count && hackerTexts[k] != null) {
                                float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds);
                                hackerTexts[k].gameObject.SetActive(true);
                                hackerTexts[k].text = string.Format(ModTranslation.getString("hackerTimer"), Math.Round(timeSinceDeath / 1000));
                            }
                        }
                    }
                } else {
                    foreach (TMPro.TextMeshPro text in hackerTexts)
                        if (text != null && text.gameObject != null)
                            text.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch]
    public static class MapBehaviourPatch2
    {
        public static void ResetIcons()
        {
            if (kataomoiMark != null)
            {
                GameObject.Destroy(kataomoiMark.gameObject);
                kataomoiMark = null;
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.GenericShow))]
        class GenericShowPatch
        {
            static void Postfix(MapBehaviour __instance)
            {
                if (Kataomoi.kataomoi == CachedPlayer.LocalPlayer.PlayerControl)
                {
                    if (kataomoiMark == null)
                    {
                        kataomoiMark = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        kataomoiMark.sprite = GetKataomoiMarkSprite();
                        kataomoiMark.transform.localScale = Vector3.one * 0.5f;
                        kataomoiMark.enabled = IsShowKataomoiMark();
                        PlayerMaterial.SetColors(7, kataomoiMark);
                        kataomoiMark.color = Kataomoi.color;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
        class FixedUpdatePatch
        {
            static void Postfix(MapBehaviour __instance)
            {

                if (Kataomoi.kataomoi == CachedPlayer.LocalPlayer.PlayerControl)
                {
                    bool isShowKataomoiMark = IsShowKataomoiMark();
                    kataomoiMark.enabled = isShowKataomoiMark;
                    if (isShowKataomoiMark)
                    {
                        Vector3 vector = Kataomoi.target.transform.position;
                        vector /= MapUtilities.CachedShipStatus.MapScale;
                        vector.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                        vector.z = -1f;
                        kataomoiMark.transform.localPosition = vector;
                    }
                }
            }
        }

        static bool IsShowKataomoiMark()
        {
            return Kataomoi.kataomoi == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Kataomoi.target != null && !Kataomoi.target.Data.IsDead && Kataomoi.isSearch;
        }

        static SpriteRenderer kataomoiMark;
        static Sprite kataomoiMarkSprite;

        static Sprite GetKataomoiMarkSprite()
        {
            if (kataomoiMarkSprite) return kataomoiMarkSprite;
            kataomoiMarkSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.KataomoiMark.png", 115f);
            return kataomoiMarkSprite;
        }
    }

    [HarmonyPatch]
    class AdminPanelPatch {
        static Dictionary<SystemTypes, List<Color>> players = new();
        static TMPro.TextMeshPro restrictInfo = null;
        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        class MapCountOverlayUpdatePatch {
            static bool Prefix(MapCountOverlay __instance) {
                // Save colors for the Hacker
                __instance.timer += Time.deltaTime;
                if (__instance.timer < 0.1f)
                {
                    return false;
                }
                __instance.timer = 0f;
                players = new Dictionary<SystemTypes, List<Color>>();
                bool commsActive = false;
                    foreach (PlayerTask task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                        if (task.TaskType == TaskTypes.FixComms) commsActive = true;


                // Restrict admin for Busker
                if (Busker.busker != null && CachedPlayer.LocalPlayer.PlayerControl == Busker.busker && Busker.restrictInformation && !commsActive)
                {
                    if (restrictInfo == null)
                    {
                        restrictInfo = UnityEngine.Object.Instantiate(__instance.SabotageText, __instance.SabotageText.transform.parent);
                        restrictInfo.text = "[ I N F O  R E S T R I C T E D ]";
                    }
                    if (Busker.pseudocideFlag)
                    {
                        __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                        foreach (CounterArea ca in __instance.CountAreas) ca.UpdateCount(0);
                        restrictInfo.gameObject.SetActive(true);
                        return false;
                    }
                    else
                        restrictInfo.gameObject.SetActive(false);
                }

                if (!__instance.isSab && commsActive)
                {
                    __instance.isSab = true;
                    __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                    __instance.SabotageText.gameObject.SetActive(true);
                    restrictInfo?.gameObject.SetActive(false);
                    return false;
                }
                if (__instance.isSab && !commsActive)
                {
                    __instance.isSab = false;
                    __instance.BackgroundColor.SetColor(Color.green);
                    __instance.SabotageText.gameObject.SetActive(false);
                    restrictInfo?.gameObject.SetActive(false);
                }

                for (int i = 0; i < __instance.CountAreas.Length; i++)
                {
                    CounterArea counterArea = __instance.CountAreas[i];
                    List<Color> roomColors = new();
                    players.Add(counterArea.RoomType, roomColors);

                    if (!commsActive)
                    {
                        MapUtilities.CachedShipStatus.FastRooms.TryGetValue(counterArea.RoomType, out var plainShipRoom);

                        if (plainShipRoom != null && plainShipRoom.roomArea) {


                            HashSet<int> hashSet = new();
                            int num = plainShipRoom.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                            int num2 = 0;
                            for (int j = 0; j < num; j++) {
                                Collider2D collider2D = __instance.buffer[j];
                                if (collider2D.CompareTag("DeadBody") && __instance.includeDeadBodies) {
                                    num2++;
                                    DeadBody bodyComponent = collider2D.GetComponent<DeadBody>();
                                    if (bodyComponent) {
                                        NetworkedPlayerInfo playerInfo = GameData.Instance.GetPlayerById(bodyComponent.ParentId);
                                        if (playerInfo != null) {
                                            var color = Palette.PlayerColors[playerInfo.DefaultOutfit.ColorId];
                                            if (Hacker.onlyColorType)
                                                color = Helpers.isLighterColor(playerInfo.DefaultOutfit.ColorId) ? Palette.PlayerColors[7] : Palette.PlayerColors[6];
                                            roomColors.Add(color);
                                        }
                                    }
                                } else {
                                    PlayerControl component = collider2D.GetComponent<PlayerControl>();
                                    if (component && component.Data != null && !component.Data.Disconnected && !component.Data.IsDead && (__instance.showLivePlayerPosition || !component.AmOwner) && hashSet.Add((int)component.PlayerId)) {
                                        num2++;
                                        if (component?.cosmetics?.currentBodySprite?.BodySprite?.material != null) {
                                            Color color = component.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
                                            if (Hacker.onlyColorType) {
                                                var id = Mathf.Max(0, Palette.PlayerColors.IndexOf(color));
                                                color = Helpers.isLighterColor((byte)id) ? Palette.PlayerColors[7] : Palette.PlayerColors[6];
                                            }
                                            roomColors.Add(color);
                                        }
                                    }
                                }
                            }

                            counterArea.UpdateCount(num2);
                        }
                        else
                        {
                            Debug.LogWarning("Couldn't find counter for:" + counterArea.RoomType);
                        }
                    }
                    else
                    {
                        counterArea.UpdateCount(0);
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(CounterArea), nameof(CounterArea.UpdateCount))]
        class CounterAreaUpdateCountPatch {
            private static Material defaultMat;
            private static Material newMat;
            static void Postfix(CounterArea __instance) {
                // Hacker display saved colors on the admin panel
                bool showHackerInfo = Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl && Hacker.hackerTimer > 0;
                if (players.ContainsKey(__instance.RoomType)) {
                    List<Color> colors = players[__instance.RoomType];

                    // Save colors for the Mimic(Assistant)
                    List<Color> impostorColors = new();
                    List<Color> mimicKColors = new();
                    List<Color> deadBodyColors = new();
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        var color = Palette.PlayerColors[p.Data.DefaultOutfit.ColorId];
                        if (p.Data.Role.IsImpostor || p == Spy.spy)
                        {
                            impostorColors.Add(color);
                            if (p == MimicK.mimicK)
                            {
                                mimicKColors.Add(color);
                            }
                        }
                        else if (p.Data.IsDead) deadBodyColors.Add(color);
                    }

                    int i = -1;
                    foreach (var icon in __instance.myIcons.GetFastEnumerator())
                    {
                        i += 1;
                        SpriteRenderer renderer = icon.GetComponent<SpriteRenderer>();

                        if (renderer != null) {
                            if (defaultMat == null) defaultMat = renderer.material;
                            if (newMat == null) newMat = UnityEngine.Object.Instantiate<Material>(defaultMat);
                            if (showHackerInfo && colors.Count > i) {
                                renderer.material = newMat;
                                var color = colors[i];
                                renderer.material.SetColor("_BodyColor", color);
                                var id = Palette.PlayerColors.IndexOf(color);
                                if (id < 0) {
                                    renderer.material.SetColor("_BackColor", color);
                                } else {
                                    renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                                }
                                renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                            }

                            // Set up the Mimic(Assistant)
                            else if ((CachedPlayer.LocalPlayer.PlayerControl == MimicA.mimicA && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead) || ((CachedPlayer.LocalPlayer.PlayerControl == EvilHacker.evilHacker || EvilHacker.isInherited()) && EvilHacker.canHasBetterAdmin))
                            {
                                renderer.material = newMat;
                                var color = colors[i];
                                if (impostorColors.Contains(color))
                                {
                                    if (mimicKColors.Contains(color)) color = Palette.PlayerColors[3];
                                    else color = Palette.ImpostorRed;
                                    renderer.material.SetColor("_BodyColor", color);
                                    var id = Palette.PlayerColors.IndexOf(color);
                                    if (id < 0)
                                    {
                                        renderer.material.SetColor("_BackColor", color);
                                    }
                                    else
                                    {
                                        renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                                    }
                                    renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                                }
                                else if (deadBodyColors.Contains(color))
                                {
                                    color = Palette.Black;
                                    renderer.material.SetColor("_BodyColor", color);
                                    var id = Palette.PlayerColors.IndexOf(color);
                                    if (id < 0)
                                    {
                                        renderer.material.SetColor("_BackColor", color);
                                    }
                                    else
                                    {
                                        renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                                    }
                                    renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                                }
                                else
                                {
                                    renderer.material = defaultMat;
                                }
                            }

                            else {
                                renderer.material = defaultMat;
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch]
    class SurveillanceMinigamePatch {
        private static int page = 0;
        private static float timer = 0f;

        public static List<GameObject> nightVisionOverlays = null;
        private static Sprite overlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NightVisionOverlay.png", 350f);
        public static bool nightVisionIsActive = false;
        private static bool isLightsOut;

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
        class SurveillanceMinigameBeginPatch {
            public static void Postfix(SurveillanceMinigame __instance) {
                // Add securityGuard cameras
                page = 0;
                timer = 0;
                if (MapUtilities.CachedShipStatus.AllCameras.Length > 4 && __instance.FilteredRooms.Length > 0) {
                    __instance.textures = __instance.textures.ToList().Concat(new RenderTexture[MapUtilities.CachedShipStatus.AllCameras.Length - 4]).ToArray();
                    for (int i = 4; i < MapUtilities.CachedShipStatus.AllCameras.Length; i++) {
                        SurvCamera surv = MapUtilities.CachedShipStatus.AllCameras[i];
                        Camera camera = UnityEngine.Object.Instantiate<Camera>(__instance.CameraPrefab);
                        camera.transform.SetParent(__instance.transform);
                        camera.transform.position = new Vector3(surv.transform.position.x, surv.transform.position.y, 8f);
                        camera.orthographicSize = 2.35f;
                        RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 16, (RenderTextureFormat)0);
                        __instance.textures[i] = temporary;
                        camera.targetTexture = temporary;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        class SurveillanceMinigameUpdatePatch {
            public static bool Prefix(SurveillanceMinigame __instance) {
                // Restrict Information
                if (Busker.busker != null && CachedPlayer.LocalPlayer.PlayerControl == Busker.busker && Busker.pseudocideFlag && Busker.restrictInformation)
                {
                    for (int i = 0; i < __instance.SabText.Length; i++)
                    {
                        __instance.SabText[i].text = "[ I N F O  R E S T R I C T E D ]";
                        __instance.SabText[i].gameObject.SetActive(true);
                    }
                    for (int i = 0; i < __instance.ViewPorts.Length; i++)
                        __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
                }

                // Update normal and securityGuard cameras
                timer += Time.deltaTime;
                int numberOfPages = Mathf.CeilToInt(MapUtilities.CachedShipStatus.AllCameras.Length / 4f);

                bool update = false;

                if (timer > 3f || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                    update = true;
                    timer = 0f;
                    page = (page + 1) % numberOfPages;
                } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                    page = (page + numberOfPages - 1) % numberOfPages;
                    update = true;
                    timer = 0f;
                }

                if ((__instance.isStatic || update) && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl)) {
                    __instance.isStatic = false;
                    for (int i = 0; i < __instance.ViewPorts.Length; i++) {
                        __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                        __instance.SabText[i].gameObject.SetActive(false);
                        if (page * 4 + i < __instance.textures.Length)
                            __instance.ViewPorts[i].material.SetTexture("_MainTex", __instance.textures[page * 4 + i]);
                        else
                            __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
                    }
                } else if (!__instance.isStatic && PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl)) {
                    __instance.isStatic = true;
                    for (int j = 0; j < __instance.ViewPorts.Length; j++) {
                        __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                        __instance.SabText[j].gameObject.SetActive(true);
                    }
                }

                nightVisionUpdate(SkeldCamsMinigame: __instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
        class PlanetSurveillanceMinigameUpdatePatch {
            public static bool Prefix(PlanetSurveillanceMinigame __instance)
            {
                bool commsActive = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl);
                if (commsActive)
                {
                    __instance.SabText.gameObject.SetActive(true);
                    __instance.ViewPort.sharedMaterial = __instance.StaticMaterial;
                }
                if (!commsActive && CachedPlayer.LocalPlayer.PlayerControl == Busker.busker && Busker.pseudocideFlag && Busker.restrictInformation)
                {
                    __instance.SabText.text = "[ I N F O  R E S T R I C T E D ]";
                    __instance.SabText.gameObject.SetActive(true);
                    __instance.ViewPort.sharedMaterial = __instance.StaticMaterial;
                }
                return false;
            }

            public static void Postfix(PlanetSurveillanceMinigame __instance) {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    __instance.NextCamera(1);
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    __instance.NextCamera(-1);

                nightVisionUpdate(SwitchCamsMinigame: __instance);
            }
        }

        [HarmonyPatch(typeof(FungleSurveillanceMinigame), nameof(FungleSurveillanceMinigame.Update))]
        class FungleSurveillanceMinigameUpdatePatch
        {
            public static void Postfix(FungleSurveillanceMinigame __instance)
            {
                nightVisionUpdate(FungleCamMinigame: __instance);
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.OnDestroy))]
        class SurveillanceMinigameDestroyPatch {
            public static void Prefix() {
                resetNightVision();
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.OnDestroy))]
        class PlanetSurveillanceMinigameDestroyPatch {
            public static void Prefix() {
                resetNightVision();
            }
        }


        private static void nightVisionUpdate(SurveillanceMinigame SkeldCamsMinigame = null, PlanetSurveillanceMinigame SwitchCamsMinigame = null, FungleSurveillanceMinigame FungleCamMinigame = null)
        {
            GameObject closeButton = null;
            if (nightVisionOverlays == null) {
                List<MeshRenderer> viewPorts = new();
                Transform viewablesTransform = null;
                if (SkeldCamsMinigame != null)
                {
                    closeButton = SkeldCamsMinigame.Viewables.transform.Find("CloseButton").gameObject;
                    foreach (var rend in SkeldCamsMinigame.ViewPorts) viewPorts.Add(rend);
                    viewablesTransform = SkeldCamsMinigame.Viewables.transform;
                }
                else if (SwitchCamsMinigame != null)
                {
                    closeButton = SwitchCamsMinigame.Viewables.transform.Find("CloseButton").gameObject;
                    viewPorts.Add(SwitchCamsMinigame.ViewPort);
                    viewablesTransform = SwitchCamsMinigame.Viewables.transform;
                }
                else if (FungleCamMinigame != null)
                {
                    closeButton = FungleCamMinigame.transform.Find("CloseButton").gameObject;
                    viewPorts.Add(FungleCamMinigame.viewport);
                    viewablesTransform = FungleCamMinigame.viewport.transform;
                }
                else return;

                nightVisionOverlays = new List<GameObject>();

                foreach (var renderer in viewPorts) {
                    GameObject overlayObject;
                    float zPosition;
                    if (FungleCamMinigame != null)
                    {
                        overlayObject = GameObject.Instantiate(closeButton, renderer.transform);
                        overlayObject.layer = renderer.gameObject.layer;
                        zPosition = -0.5f;
                        overlayObject.transform.localPosition = new Vector3(0, 0, zPosition);
                    }
                    else
                    {
                        overlayObject = GameObject.Instantiate(closeButton, viewablesTransform);
                        zPosition = overlayObject.transform.position.z;
                        overlayObject.layer = closeButton.layer;
                        overlayObject.transform.position = new Vector3(renderer.transform.position.x, renderer.transform.position.y, zPosition);
                    }
                    Vector3 localScale = (SkeldCamsMinigame != null) ? new Vector3(0.91f, 0.612f, 1f) : new Vector3(2.124f, 1.356f, 1f);
                    localScale = (FungleCamMinigame != null) ? new Vector3(10f, 10f, 1f) : localScale;
                    overlayObject.transform.localScale = localScale;
                    var overlayRenderer = overlayObject.GetComponent<SpriteRenderer>();
                    overlayRenderer.sprite = overlaySprite;
                    overlayObject.SetActive(false);
                    GameObject.Destroy(overlayObject.GetComponent<CircleCollider2D>());
                    nightVisionOverlays.Add(overlayObject);
                }
            }


            isLightsOut = CachedPlayer.LocalPlayer.PlayerControl.myTasks.ToArray().Any(x => x.name.Contains("FixLightsTask")) || Trickster.lightsOutTimer > 0;
            bool ignoreNightVision = CustomOptionHolder.camsNoNightVisionIfImpVision.getBool() && Helpers.hasImpVision(GameData.Instance.GetPlayerById(CachedPlayer.LocalPlayer.PlayerId)) || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
            bool nightVisionEnabled = CustomOptionHolder.camsNightVision.getBool();

            if (isLightsOut && !nightVisionIsActive && nightVisionEnabled && !ignoreNightVision) {  // only update when something changed!
                foreach (PlayerControl pc in CachedPlayer.AllPlayers) {
                    pc.setLook("", 11, "", "", "", "", false);
                }
                foreach (var overlayObject in nightVisionOverlays) {
                    overlayObject.SetActive(true);
                }
                // Dead Bodies
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>()) {
                    SpriteRenderer component = deadBody.bodyRenderers.FirstOrDefault();
                    component.material.SetColor("_BackColor", Palette.ShadowColors[11]);
                    component.material.SetColor("_BodyColor", Palette.PlayerColors[11]);
                }
                nightVisionIsActive = true;
            } else if (!isLightsOut && nightVisionIsActive) {
                resetNightVision();
            }
        }

        public static void resetNightVision() {
            foreach (var go in nightVisionOverlays) {
                go.Destroy();
            }
            nightVisionOverlays = null;

            if (nightVisionIsActive) {
                nightVisionIsActive = false;
                foreach (PlayerControl pc in CachedPlayer.AllPlayers) {
                    if (Camouflager.camouflageTimer > 0) {
                        pc.setLook("", 6, "", "", "", "", false);
                    } else if (pc == Morphling.morphling && Morphling.morphTimer > 0) {
                        PlayerControl target = Morphling.morphTarget;
                        Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId, false);
                    }
                    else if (pc == MimicK.mimicK && MimicK.victim != null)
                    {
                        var victim = MimicK.victim;
                        MimicK.mimicK.setLook(victim.Data.PlayerName, victim.Data.DefaultOutfit.ColorId, victim.Data.DefaultOutfit.HatId, victim.Data.DefaultOutfit.VisorId, victim.Data.DefaultOutfit.SkinId, victim.Data.DefaultOutfit.PetId, false);
                    }
                    else if (pc == MimicA.mimicA && MimicK.mimicK != null && MimicA.isMorph)
                    {
                        var victim = MimicK.mimicK;
                        MimicA.mimicA.setLook(victim.Data.PlayerName, victim.Data.DefaultOutfit.ColorId, victim.Data.DefaultOutfit.HatId, victim.Data.DefaultOutfit.VisorId, victim.Data.DefaultOutfit.SkinId, victim.Data.DefaultOutfit.PetId, false);
                    }
                    else {
                        Helpers.setDefaultLook(pc, false);
                    }
                    // Dead Bodies
                    foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>()) {
                        var colorId = GameData.Instance.GetPlayerById(deadBody.ParentId).Object.Data.DefaultOutfit.ColorId;
                        SpriteRenderer component = deadBody.bodyRenderers.FirstOrDefault();
                        component.material.SetColor("_BackColor", Palette.ShadowColors[colorId]);
                        component.material.SetColor("_BodyColor", Palette.PlayerColors[colorId]);
                    }
                }
            }

        }

        public static void enforceNightVision(PlayerControl player) {
            if (isLightsOut && nightVisionOverlays != null && nightVisionIsActive) {
                player.setLook("", 11, "", "", "", "", false);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetPlayerMaterialColors))]
        public static void Postfix(PlayerControl __instance, SpriteRenderer rend) {
            if (!nightVisionIsActive) return;
            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>()) {
                foreach (SpriteRenderer component in new SpriteRenderer[2] { deadBody.bodyRenderers.FirstOrDefault(), deadBody.bloodSplatter }) { 
                    component.material.SetColor("_BackColor", Palette.ShadowColors[11]);
                    component.material.SetColor("_BodyColor", Palette.PlayerColors[11]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    class MedScanMinigameFixedUpdatePatch {
        static void Prefix(MedScanMinigame __instance) {
            if (TORMapOptions.allowParallelMedBayScans) {
                __instance.medscan.CurrentUser = CachedPlayer.LocalPlayer.PlayerId;
                __instance.medscan.UsersList.Clear();
            }
        }
    }
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    class ShowSabotageMapPatch {
        static bool Prefix(MapBehaviour __instance) {
            if (HideNSeek.isHideNSeekGM)
                return HideNSeek.canSabotage;
            return true;
        }
    }

}
