using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Objects;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;
using TheOtherRoles.CustomGameModes;
using static UnityEngine.GraphicsBuffer;
using AmongUs.GameOptions;
using Sentry.Internal.Extensions;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdatePatch {
        // Helpers

        static PlayerControl setTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null) {
            PlayerControl result = null;
            float num = AmongUs.GameOptions.GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 2)];
            if (!MapUtilities.CachedShipStatus) return result;
            if (targetingPlayer == null) targetingPlayer = CachedPlayer.LocalPlayer.PlayerControl;
            if (targetingPlayer.Data.IsDead) return result;

            untargetablePlayers ??= new List<PlayerControl>();

            if (!Ninja.canBeTargeted && Ninja.ninja != null && Ninja.stealthed) {
                untargetablePlayers.Add(Ninja.ninja);
            }
            if (Sprinter.sprinter != null && Sprinter.sprinting) {
                untargetablePlayers.Add(Sprinter.sprinter);
            }
            if (Fox.fox != null && Fox.stealthed) {
                untargetablePlayers.Add(Fox.fox);
            }

            Vector2 truePosition = targetingPlayer.GetTruePosition();
            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Role.IsImpostor)) {
                    PlayerControl @object = playerInfo.Object;
                    if (untargetablePlayers != null && untargetablePlayers.Any(x => x == @object)) {
                        // if that player is not targetable: skip check
                        continue;
                    }                    

                    if (@object && (!@object.inVent || targetPlayersInVents)) {
                        Vector2 vector = @object.GetTruePosition() - truePosition;
                        float magnitude = vector.magnitude;
                        if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask)) {
                            result = @object;
                            num = magnitude;
                        }
                    }
                }
            }
            return result;
        }

        static void setPlayerOutline(PlayerControl target, Color color) {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

            color = color.SetAlpha(Chameleon.visibility(target.PlayerId));

            target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
            target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
        }

        // Update functions

        static void setBasePlayerOutlines() {
            foreach (PlayerControl target in CachedPlayer.AllPlayers) {
                if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

                bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
                bool isMimicKShield = target == MimicK.mimicK && MimicK.victim != null;
                bool isMimicAMorph = target == MimicA.mimicA && MimicA.isMorph;
                bool hasVisibleShield = false;
                Color color = Medic.shieldedColor;
                if (Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && Medic.shieldVisible(target))
                    hasVisibleShield = true;

                if (Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && TORMapOptions.firstKillPlayer != null && TORMapOptions.shieldFirstKill && ((target == TORMapOptions.firstKillPlayer && !isMorphedMorphling && !isMimicKShield && !isMimicAMorph) || (isMorphedMorphling && Morphling.morphTarget == TORMapOptions.firstKillPlayer)
                    || (isMimicAMorph && MimicK.mimicK == TORMapOptions.firstKillPlayer))) {
                    hasVisibleShield = true;
                    color = Color.blue;
                }

                if (hasVisibleShield) {
                target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
                }
                else {
                    target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                }
            }
        }

        public static void bendTimeUpdate() {
            if (TimeMaster.isRewinding) {
                if (localPlayerPositions.Count > 0) {
                    // Set position
                    var next = localPlayerPositions[0];
                    if (next.Item2 == true) {
                        // Exit current vent if necessary
                        if (CachedPlayer.LocalPlayer.PlayerControl.inVent) {
                            foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents) {
                                bool canUse;
                                bool couldUse;
                                vent.CanUse(CachedPlayer.LocalPlayer.Data, out canUse, out couldUse);
                                if (canUse) {
                                    CachedPlayer.LocalPlayer.PlayerPhysics.RpcExitVent(vent.Id);
                                    vent.SetButtons(false);
                                }
                            }
                        }
                        // Set position
                        CachedPlayer.LocalPlayer.transform.position = next.Item1;
                    }
                    else if (localPlayerPositions.Any(x => x.Item2 == true)) {
                        CachedPlayer.LocalPlayer.transform.position = next.Item1;
                    }
                    if (SubmergedCompatibility.IsSubmerged) {
                        SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);
                    }

                    localPlayerPositions.RemoveAt(0);

                    if (localPlayerPositions.Count > 1) localPlayerPositions.RemoveAt(0); // Skip every second position to rewinde twice as fast, but never skip the last position
                }
                else {
                    TimeMaster.isRewinding = false;
                    CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                }
            }
            else {
                while (localPlayerPositions.Count >= Mathf.Round(TimeMaster.rewindTime / Time.fixedDeltaTime)) localPlayerPositions.RemoveAt(localPlayerPositions.Count - 1);
                localPlayerPositions.Insert(0, new Tuple<Vector3, bool>(CachedPlayer.LocalPlayer.transform.position, CachedPlayer.LocalPlayer.PlayerControl.CanMove)); // CanMove = CanMove
            }
        }

        static void medicSetTarget() {
            if (Medic.medic == null || Medic.medic != CachedPlayer.LocalPlayer.PlayerControl) return;
            Medic.currentTarget = setTarget();
            if (!Medic.usedShield) setPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
        }

        static void shifterSetTarget() {
            if (Shifter.shifter == null || Shifter.shifter != CachedPlayer.LocalPlayer.PlayerControl) return;
            List<PlayerControl> blockShift = null;
            if (Shifter.isNeutral && !Shifter.shiftPastShifters)
            {
                blockShift = new List<PlayerControl>();
                foreach (var playerId in Shifter.pastShifters)
                {
                    blockShift.Add(Helpers.playerById((byte)playerId));
                }
            }

            Shifter.currentTarget = setTarget(untargetablePlayers: blockShift);
            if (Shifter.futureShift == null) setPlayerOutline(Shifter.currentTarget, Shifter.color);
        }


        static void morphlingSetTarget() {
            if (Morphling.morphling == null || Morphling.morphling != CachedPlayer.LocalPlayer.PlayerControl) return;
            Morphling.currentTarget = setTarget();
            setPlayerOutline(Morphling.currentTarget, Morphling.color);
        }

        static void sheriffSetTarget() {
            if (Sheriff.sheriff == null || Sheriff.sheriff != CachedPlayer.LocalPlayer.PlayerControl) return;
            Sheriff.currentTarget = setTarget();
            setPlayerOutline(Sheriff.currentTarget, Sheriff.color);
        }

        static void deputySetTarget()
        {
            if (Deputy.deputy == null || Deputy.deputy != CachedPlayer.LocalPlayer.PlayerControl) return;
            Deputy.currentTarget = setTarget();
            setPlayerOutline(Deputy.currentTarget, Deputy.color);
        }

        public static void deputyCheckPromotion(bool isMeeting=false)
        {
            // If LocalPlayer is Deputy, the Sheriff is disconnected and Deputy promotion is enabled, then trigger promotion
            if (Deputy.deputy == null || Deputy.deputy != CachedPlayer.LocalPlayer.PlayerControl) return;
            if (Deputy.promotesToSheriff == 0 || Deputy.deputy.Data.IsDead == true || Deputy.promotesToSheriff == 2 && !isMeeting) return;
            if (Sheriff.sheriff == null || Sheriff.sheriff?.Data?.Disconnected == true || Sheriff.sheriff.Data.IsDead)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DeputyPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.deputyPromotes();
            }
        }

        static void trackerSetTarget() {
            if (Tracker.tracker == null || Tracker.tracker != CachedPlayer.LocalPlayer.PlayerControl) return;
            Tracker.currentTarget = setTarget();
            if (!Tracker.usedTracker) setPlayerOutline(Tracker.currentTarget, Tracker.color);
        }

        static void detectiveUpdateFootPrints() {
            if (Detective.detective == null || Detective.detective != CachedPlayer.LocalPlayer.PlayerControl) return;

            Detective.timer -= Time.fixedDeltaTime;
            if (Detective.timer <= 0f) {
                Detective.timer = Detective.footprintIntervall;
                foreach (PlayerControl player in CachedPlayer.AllPlayers) {
                    if (player != null && player != CachedPlayer.LocalPlayer.PlayerControl && !player.Data.IsDead && !player.inVent && !((player == Ninja.ninja && Ninja.stealthed) || (player == Sprinter.sprinter && Sprinter.sprinting))) 
                    {
                        FootprintHolder.Instance.MakeFootprint(player);
                    }
                }
            }
        }

        static void vampireSetTarget() {
            if (Vampire.vampire == null || Vampire.vampire != CachedPlayer.LocalPlayer.PlayerControl) return;

            PlayerControl target = null;
            if (Spy.spy != null || Sidekick.wasSpy || Jackal.wasSpy) {
                if (Spy.impostorsCanKillAnyone) {
                    target = setTarget(false, true);
                }
                else {
                    target = setTarget(true, true, new List<PlayerControl>() { Spy.spy, Sidekick.wasTeamRed ? Sidekick.sidekick : null, Jackal.wasTeamRed ? Jackal.jackal : null });
                }
            }
            else {
                target = setTarget(true, true, new List<PlayerControl>() { Sidekick.wasImpostor ? Sidekick.sidekick : null, Jackal.wasImpostor ? Jackal.jackal : null });
            }

            bool targetNearGarlic = false;
            if (target != null) {
                foreach (Garlic garlic in Garlic.garlics) {
                    if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.91f) {
                        targetNearGarlic = true;
                    }
                }
            }
            Vampire.targetNearGarlic = targetNearGarlic;
            Vampire.currentTarget = target;
            setPlayerOutline(Vampire.currentTarget, Vampire.color);
        }

        static void jackalSetTarget() {
            if (Jackal.jackal == null || Jackal.jackal != CachedPlayer.LocalPlayer.PlayerControl) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.canCreateSidekickFromImpostor) {
                // Only exclude sidekick from beeing targeted if the jackal can create sidekicks from impostors
                if (Sidekick.sidekick != null) untargetablePlayers.Add(Sidekick.sidekick);
            }
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
            Jackal.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            setPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);
        }

        static void sidekickSetTarget() {
            if (Sidekick.sidekick == null || Sidekick.sidekick != CachedPlayer.LocalPlayer.PlayerControl) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.jackal != null) untargetablePlayers.Add(Jackal.jackal);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Sidekick from targeting the Mini unless it has grown up
            Sidekick.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            if (Sidekick.canKill) setPlayerOutline(Sidekick.currentTarget, Palette.ImpostorRed);
        }

        static void sidekickCheckPromotion() {
            // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
            if (Sidekick.sidekick == null || Sidekick.sidekick != CachedPlayer.LocalPlayer.PlayerControl) return;
            if (Sidekick.sidekick.Data.IsDead == true || !Sidekick.promotesToJackal) return;
            if (Jackal.jackal == null || Jackal.jackal?.Data?.Disconnected == true) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes();
            }
        }

        static void eraserSetTarget() {
            if (Eraser.eraser == null || Eraser.eraser != CachedPlayer.LocalPlayer.PlayerControl) return;

            List<PlayerControl> untargetables = new List<PlayerControl>();
            if (Spy.spy != null) untargetables.Add(Spy.spy);
            if (Sidekick.wasTeamRed) untargetables.Add(Sidekick.sidekick);
            if (Jackal.wasTeamRed) untargetables.Add(Jackal.jackal);
            Eraser.currentTarget = setTarget(onlyCrewmates: !Eraser.canEraseAnyone, untargetablePlayers: Eraser.canEraseAnyone ? new List<PlayerControl>() : untargetables);
            setPlayerOutline(Eraser.currentTarget, Eraser.color);
        }

        static void UndertakerSetTarget()
        {
            if (Undertaker.undertaker == null || Undertaker.undertaker != CachedPlayer.LocalPlayer.PlayerControl) return;
            if (Undertaker.TargetBody != null)
            {
                Helpers.SetDeadBodyOutline(Undertaker.TargetBody, null);
            }

            if (Undertaker.DraggedBody == null)
            {
                Undertaker.TargetBody = Helpers.setDeadTarget(2f / 3f);
                Helpers.SetDeadBodyOutline(Undertaker.TargetBody, Undertaker.color);
            }
        }

        static void UndertakerCanDropTarget()
        {
            if (Undertaker.undertaker == null || Undertaker.undertaker != CachedPlayer.LocalPlayer.PlayerControl) return;
            var component = Undertaker.DraggedBody;

            Undertaker.CanDropBody = false;

            if (component == null) return;

            if (component.enabled && Vector2.Distance(Undertaker.undertaker.GetTruePosition(), component.TruePosition) <= Undertaker.undertaker.MaxReportDistance && !PhysicsHelpers.AnythingBetween(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), component.TruePosition, Constants.ShipAndObjectsMask, false))
            {
                Undertaker.CanDropBody = true;
            }
        }

        static void UndertakerUpdate()
        {
            var undertakerPlayer = Undertaker.undertaker;
            var bodyComponent = Undertaker.DraggedBody;

            if (undertakerPlayer == null || bodyComponent == null) return;

            var undertakerPos = undertakerPlayer.transform.position;
            var bodyLastPos = bodyComponent.transform.position;

            var direction = undertakerPlayer.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;

            var newBodyPos = direction == Vector2.zero
                ? bodyLastPos
                : undertakerPos - (Vector3)(direction * (2f / 3f)) + (Vector3)bodyComponent.myCollider.offset;
            newBodyPos.z = undertakerPos.z + 0.005f;

            bodyComponent.transform.position.Set(newBodyPos.x, newBodyPos.y, newBodyPos.z);

            if (direction == Direction.right) newBodyPos += new Vector3(0.3f, 0, 0);
            if (direction == Direction.up) newBodyPos += new Vector3(0.15f, 0.2f, 0);
            if (direction == Direction.down) newBodyPos += new Vector3(0.15f, -0.2f, 0);
            if (direction == Direction.upleft) newBodyPos += new Vector3(0, 0.1f, 0);
            if (direction == Direction.upright) newBodyPos += new Vector3(0.3f, 0.1f, 0);
            if (direction == Direction.downright) newBodyPos += new Vector3(0.3f, -0.2f, 0);
            if (direction == Direction.downleft) newBodyPos += new Vector3(0f, -0.2f, 0);

            if (PhysicsHelpers.AnythingBetween(
                    undertakerPlayer.GetTruePosition(),
                    newBodyPos,
                    Constants.ShipAndObjectsMask,
                    false
                ))
            {
                newBodyPos = new Vector3(undertakerPos.x, undertakerPos.y, bodyLastPos.z);
            }

            if (undertakerPlayer.Data.IsDead)
            {
                if (undertakerPlayer.AmOwner)
                {
                    Undertaker.RpcDropBody(newBodyPos);
                }

                return;
            }

            bodyComponent.transform.position = newBodyPos;

            if (!undertakerPlayer.AmOwner) return;

            Helpers.SetDeadBodyOutline(bodyComponent, Color.green);
        }

        static void deputyUpdate()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == null || !Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId)) return;
            
            if (Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] <= 0)
            {
                Deputy.handcuffedKnows.Remove(CachedPlayer.LocalPlayer.PlayerId);
                // Resets the buttons
                Deputy.setHandcuffedKnows(false);
                
                // Ghost info
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffOver);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
        }

        static void engineerUpdate() {
            bool jackalHighlight = Engineer.highlightForTeamJackal && (CachedPlayer.LocalPlayer.PlayerControl == Jackal.jackal || CachedPlayer.LocalPlayer.PlayerControl == Sidekick.sidekick);
            bool impostorHighlight = Engineer.highlightForImpostors && CachedPlayer.LocalPlayer.Data.Role.IsImpostor;
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
        }

        static void impostorSetTarget() {
            if (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor ||!CachedPlayer.LocalPlayer.PlayerControl.CanMove || CachedPlayer.LocalPlayer.Data.IsDead || (CachedPlayer.LocalPlayer.PlayerControl == Undertaker.undertaker && Undertaker.DraggedBody != null)) { // !isImpostor || !canMove || isDead
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                return;
            }

            PlayerControl target = null;
            if (Spy.spy != null || Sidekick.wasSpy || Jackal.wasSpy) {
                if (Spy.impostorsCanKillAnyone) {
                    target = setTarget(false, true);
                }
                else {
                    target = setTarget(true, true, new List<PlayerControl>() { Spy.spy, Sidekick.wasTeamRed ? Sidekick.sidekick : null, Jackal.wasTeamRed ? Jackal.jackal : null });
                }
            }
            else {
                target = setTarget(true, true, new List<PlayerControl>() { Sidekick.wasImpostor ? Sidekick.sidekick : null, Jackal.wasImpostor ? Jackal.jackal : null });
            }

            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
        }

        static void baitUpdate()
        {
            if (Bait.bait == null || Bait.bait != CachedPlayer.LocalPlayer.PlayerControl) return;

            // Bait report
            if (Bait.bait.Data.IsDead && !Bait.reported)
            {
                Bait.reportDelay -= Time.fixedDeltaTime;
                DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == Bait.bait.PlayerId)?.FirstOrDefault();
                if (deadPlayer.killerIfExisting != null && Bait.reportDelay <= 0f)
                {
                    Helpers.handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                    Helpers.HandleUndertakerDropOnBodyReport();
                    Helpers.handleTrapperTrapOnBodyReport();

                    byte reporter = deadPlayer.killerIfExisting.PlayerId;

                    if (Madmate.madmate.Any(x => x.PlayerId == Bait.bait.PlayerId))
                    {
                        var candidates = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => !x.Data.IsDead && !x.Data.Role.IsImpostor).ToList();
                        int i = rnd.Next(0, candidates.Count);
                        reporter = candidates.Count > 0 ? candidates[i].PlayerId : deadPlayer.killerIfExisting.PlayerId;
                    }

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
                    writer.Write(reporter);
                    writer.Write(Bait.bait.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.uncheckedCmdReportDeadBody(reporter, Bait.bait.PlayerId);
                    Bait.reported = true;
                }
            }

            // Bait Vents
            if (MapUtilities.CachedShipStatus?.AllVents != null)
            {
                var ventsWithPlayers = new List<int>();
                foreach (PlayerControl player in CachedPlayer.AllPlayers)
                {
                    if (player == null) continue;

                    if (player.inVent)
                    {
                        Vent target = MapUtilities.CachedShipStatus.AllVents.OrderBy(x => Vector2.Distance(x.transform.position, player.GetTruePosition())).FirstOrDefault();
                        if (target != null) ventsWithPlayers.Add(target.Id);
                    }
                }

                foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                {
                    if (vent.myRend == null || vent.myRend.material == null) continue;
                    if (ventsWithPlayers.Contains(vent.Id) || (ventsWithPlayers.Count > 0 && Bait.highlightAllVents))
                    {
                        vent.myRend.material.SetFloat("_Outline", 1f);
                        vent.myRend.material.SetColor("_OutlineColor", Color.yellow);
                    }
                    else
                    {
                        vent.myRend.material.SetFloat("_Outline", 0);
                    }
                }
            }
        }

        static void moriartySetTarget()
        {
            if (Moriarty.moriarty == null || Moriarty.moriarty != CachedPlayer.LocalPlayer.PlayerControl) return;
            Moriarty.currentTarget = setTarget();
            if (Moriarty.target != null) Moriarty.killTarget = setTarget(targetingPlayer: Moriarty.target);
            else Moriarty.killTarget = null;
            setPlayerOutline(Moriarty.currentTarget, Moriarty.color);
        }

        static void cupidSetTarget()
        {
            if (Cupid.cupid == null || Cupid.cupid != CachedPlayer.LocalPlayer.PlayerControl) return;
            var untargetables = new List<PlayerControl>();
            if (Cupid.lovers1 != null) untargetables.Add(Cupid.lovers1);
            Cupid.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Cupid.isShieldOn) Cupid.shieldTarget = setTarget();
            if (Cupid.lovers1 == null || Cupid.lovers2 == null) setPlayerOutline(Cupid.currentTarget, Cupid.color);
            else if (Cupid.shielded == null && Cupid.isShieldOn) setPlayerOutline(Cupid.shieldTarget, Cupid.color);
        }

        static void warlockSetTarget() {
            if (Warlock.warlock == null || Warlock.warlock != CachedPlayer.LocalPlayer.PlayerControl) return;
            if (Warlock.curseVictim != null && (Warlock.curseVictim.Data.Disconnected || Warlock.curseVictim.Data.IsDead)) {
                // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                Warlock.resetCurse();
            }
            if (Warlock.curseVictim == null) {
                Warlock.currentTarget = setTarget();
                setPlayerOutline(Warlock.currentTarget, Warlock.color);
            }
            else {
                Warlock.curseVictimTarget = setTarget(targetingPlayer: Warlock.curseVictim);
                setPlayerOutline(Warlock.curseVictimTarget, Warlock.color);
            }
        }

        static void prophetSetTarget()
        {
            if (Prophet.prophet == null ||CachedPlayer.LocalPlayer.PlayerControl != Prophet.prophet) return;
            Prophet.currentTarget = setTarget();
            if (Prophet.examinesLeft > 0) setPlayerOutline(Prophet.currentTarget, Prophet.color);
        }

        static void assassinUpdate()
        {
            /*if (Assassin.isInvisble && Assassin.invisibleTimer <= 0 && Assassin.assassin == CachedPlayer.LocalPlayer.PlayerControl)
            {
                MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetInvisible, Hazel.SendOption.Reliable, -1);
                invisibleWriter.Write(Assassin.assassin.PlayerId);
                invisibleWriter.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.setInvisible(Assassin.assassin.PlayerId, byte.MaxValue);
            }*/
            if (Assassin.arrow?.arrow != null)
            {
                if (Assassin.assassin == null || Assassin.assassin != CachedPlayer.LocalPlayer.PlayerControl || !Assassin.knowsTargetLocation) {
                    Assassin.arrow.arrow.SetActive(false);
                    return;
                }
                if (Assassin.assassinMarked != null && !CachedPlayer.LocalPlayer.Data.IsDead)
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
                } else
                {
                    Assassin.arrow.arrow.SetActive(false);
                }
            }
        }

        static void plagueDoctorSetTarget()
        {
            if (PlagueDoctor.plagueDoctor == null || CachedPlayer.LocalPlayer.PlayerControl != PlagueDoctor.plagueDoctor) return;
            if (!PlagueDoctor.plagueDoctor.Data.IsDead && PlagueDoctor.numInfections > 0)
            {
                PlagueDoctor.currentTarget = setTarget(untargetablePlayers: PlagueDoctor.infected.Values.ToList());
                setPlayerOutline(PlagueDoctor.currentTarget, PlagueDoctor.color);
            }
        }

        static void teleporterSetTarget()
        {
            if (Teleporter.teleporter == null || Teleporter.teleporter != CachedPlayer.LocalPlayer.PlayerControl) return;
            var untargetables = new List<PlayerControl>();
            if (Teleporter.target1 != null) untargetables.Add(Teleporter.target1);
            if (Teleporter.target2 != null) untargetables.Add(Teleporter.target2);
            Teleporter.currentTarget = setTarget(untargetablePlayers: untargetables);
            if ((Teleporter.target1 == null || Teleporter.target2 == null) && Teleporter.teleportNumber > 0) setPlayerOutline(Teleporter.currentTarget, Teleporter.color);
        }


        static void CatcherSetTarget()
        {
            if (Catcher.catcher == null || Catcher.catcher != CachedPlayer.LocalPlayer.PlayerControl) return;
            Catcher.neartarget = setTarget();
            setPlayerOutline(Catcher.neartarget,Catcher.color);
        }

        public static void plagueDoctorUpdate()
        {
            if (MeetingHud.Instance != null)
            {
                if (PlagueDoctor.statusText != null)
                {
                    PlagueDoctor.statusText.gameObject.SetActive(false);
                }
            }

            if (PlagueDoctor.plagueDoctor != null && (CachedPlayer.LocalPlayer.PlayerControl == PlagueDoctor.plagueDoctor || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead))
            {
                if (PlagueDoctor.statusText == null)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance?.roomTracker.gameObject);
                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    gameObject.SetActive(true);
                    UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    PlagueDoctor.statusText = gameObject.GetComponent<TMPro.TMP_Text>();
                    gameObject.transform.localPosition = new Vector3(-2.7f, -0.1f, gameObject.transform.localPosition.z);

                    PlagueDoctor.statusText.transform.localScale = new Vector3(1f, 1f, 1f);
                    PlagueDoctor.statusText.fontSize = 1.5f;
                    PlagueDoctor.statusText.fontSizeMin = 1.5f;
                    PlagueDoctor.statusText.fontSizeMax = 1.5f;
                    PlagueDoctor.statusText.alignment = TMPro.TextAlignmentOptions.BottomLeft;
                    PlagueDoctor.statusText.alpha = byte.MaxValue;
                }

                PlagueDoctor.statusText.gameObject.SetActive(true);
                string text = $"[{ModTranslation.getString("plagueDoctorProgress")}]\n";

                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p == PlagueDoctor.plagueDoctor) continue;
                    if (PlagueDoctor.dead.ContainsKey(p.PlayerId) && PlagueDoctor.dead[p.PlayerId]) continue;
                    text += $"{p.Data.PlayerName}: ";
                    if (PlagueDoctor.infected.ContainsKey(p.PlayerId))
                    {
                        text += Helpers.cs(Color.red, ModTranslation.getString("plagueDoctorInfectedText"));
                    }
                    else
                    {
                        // データが無い場合は作成する
                        if (!PlagueDoctor.progress.ContainsKey(p.PlayerId))
                        {
                            PlagueDoctor.progress[p.PlayerId] = 0f;
                        }
                        text += PlagueDoctor.getProgressString(PlagueDoctor.progress[p.PlayerId]);
                    }
                    text += "\n";
                }

                PlagueDoctor.statusText.text = text;
            }

            if (PlagueDoctor.plagueDoctor != null && CachedPlayer.LocalPlayer.PlayerControl == PlagueDoctor.plagueDoctor)
            {
                if (!PlagueDoctor.meetingFlag && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead))
                {
                    List<PlayerControl> newInfected = new List<PlayerControl>();
                    foreach (PlayerControl target in CachedPlayer.AllPlayers)
                    {
                        if (target == PlagueDoctor.plagueDoctor || target.Data.IsDead || PlagueDoctor.infected.ContainsKey(target.PlayerId) || target.inVent) continue;
                        if (!PlagueDoctor.progress.ContainsKey(target.PlayerId)) PlagueDoctor.progress[target.PlayerId] = 0f;

                        foreach (PlayerControl source in PlagueDoctor.infected.Values.ToList())
                        {
                            if (source.Data.IsDead) continue;
                            float distance = Vector3.Distance(source.transform.position, target.transform.position);
                            bool anythingBetween = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShipAndObjectsMask, false);

                            if (distance <= PlagueDoctor.infectDistance && !anythingBetween)
                            {
                                PlagueDoctor.progress[target.PlayerId] += Time.fixedDeltaTime;

                                // 他のクライアントに進行状況を通知する
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlagueDoctorUpdateProgress, Hazel.SendOption.Reliable, -1);
                                writer.Write(target.PlayerId);
                                writer.Write(PlagueDoctor.progress[target.PlayerId]);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);

                                // Only update a player's infection once per FixedUpdate
                                break;
                            }
                        }

                        if (PlagueDoctor.progress[target.PlayerId] > PlagueDoctor.infectDuration)
                        {
                            newInfected.Add(target);
                        }

                        foreach (PlayerControl p in newInfected)
                        {
                            byte targetId = p.PlayerId;
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlagueDoctorSetInfected, Hazel.SendOption.Reliable, -1);
                            writer.Write(targetId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.plagueDoctorInfected(targetId);
                        }

                        bool winFlag = true;
                        foreach (PlayerControl p in CachedPlayer.AllPlayers)
                        {
                            if (p.Data.IsDead) continue;
                            if (p == PlagueDoctor.plagueDoctor) continue;
                            if (!PlagueDoctor.infected.ContainsKey(p.PlayerId))
                            {
                                winFlag = false;
                                break;
                            }
                        }

                        if (winFlag)
                        {
                            MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlagueDoctorWin, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                            RPCProcedure.plagueDoctorWin();
                        }
                    }
                }
            }
        }

        static void trackerUpdate() {
            // Handle player tracking
            if (Tracker.arrow?.arrow != null) {
                if (Tracker.tracker == null || CachedPlayer.LocalPlayer.PlayerControl != Tracker.tracker) {
                    Tracker.arrow.arrow.SetActive(false);
                    if (Tracker.DangerMeterParent) Tracker.DangerMeterParent.SetActive(false);
                    return;
                }

                if (Tracker.tracked != null && !Tracker.tracker.Data.IsDead) {
                    Tracker.timeUntilUpdate -= Time.fixedDeltaTime;

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

                        if (Tracker.trackingMode == 1 || Tracker.trackingMode == 2) Arrow.UpdateProximity(position);
                        if (Tracker.trackingMode == 0 || Tracker.trackingMode == 2)
                        {
                            Tracker.arrow.Update(position);
                            Tracker.arrow.arrow.SetActive(trackedOnMap);
                        }
                        Tracker.timeUntilUpdate = Tracker.updateIntervall;
                    } else {
                        if (Tracker.trackingMode == 0 || Tracker.trackingMode == 2)
                            Tracker.arrow.Update();
                    }
                }
                else if (Tracker.tracker.Data.IsDead)
                {
                    Tracker.DangerMeterParent?.SetActive(false);
                    Tracker.Meter?.gameObject.SetActive(false);
                }
            }

            // Handle corpses tracking
            if (Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl && Tracker.corpsesTrackingTimer >= 0f && !Tracker.tracker.Data.IsDead) {
                bool arrowsCountChanged = Tracker.localArrows.Count != Tracker.deadBodyPositions.Count();
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

        static void foxUpdate()
        {
            if (Fox.fox == null || CachedPlayer.LocalPlayer.PlayerControl != Fox.fox) return;
            Fox.arrowUpdate();
        }

        static void immoralistUpdate()
        {
            if (Immoralist.immoralist == null || CachedPlayer.LocalPlayer.PlayerControl != Immoralist.immoralist) return;
            Immoralist.arrowUpdate();
        }

        static void foxSetTarget()
        {
            if (Fox.fox == null || CachedPlayer.LocalPlayer.PlayerControl != Fox.fox || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead) return;
            List<PlayerControl> untargetablePlayers = new();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.Data.Role.IsImpostor || p == Jackal.jackal || p == JekyllAndHyde.jekyllAndHyde || p == Moriarty.moriarty)
                {
                    untargetablePlayers.Add(p);
                }
            }
            Fox.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            if (Fox.canCreateImmoralist) setPlayerOutline(Fox.currentTarget, Fox.color);
        }

        public static void playerSizeUpdate(PlayerControl p) {
            // Set default player size
            CircleCollider2D collider = p.Collider.CastFast<CircleCollider2D>();

            p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            collider.radius = Mini.defaultColliderRadius;
            collider.offset = Mini.defaultColliderOffset * Vector2.down;

            // Set adapted player size to Mini and Morphling
            if (Mini.mini == null || Camouflager.camouflageTimer > 0f || Helpers.MushroomSabotageActive() || (Mini.mini == Morphling.morphling && Morphling.morphTimer > 0) || (Mini.mini == MimicA.mimicA && MimicA.isMorph)) return;

            float growingProgress = Mini.growingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            float correctedColliderRadius = Mini.defaultColliderRadius * 0.7f / scale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

            if (p == Mini.mini && !(Mini.mini == MimicK.mimicK && MimicK.victim != null)) {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
            if ((Morphling.morphling != null && p == Morphling.morphling && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f) ||
                (MimicA.mimicA != null && p == MimicA.mimicA && MimicA.isMorph && MimicK.mimicK != null && MimicK.mimicK == Mini.mini)) {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
        }

        public static void updatePlayerInfo() {
            Vector3 colorBlindTextMeetingInitialLocalPos = new Vector3(0.3384f, -0.16666f, -0.01f);
            Vector3 colorBlindTextMeetingInitialLocalScale = new Vector3(0.9f, 1f, 1f);
            foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                
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

                if ((Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target) || (Akujo.knowsRoles && CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))) || p == CachedPlayer.LocalPlayer.PlayerControl || CachedPlayer.LocalPlayer.Data.IsDead) {
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
                    string roleText = RoleInfo.GetRolesString(p, true, TORMapOptions.ghostsSeeModifier);
                    string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";
                    string exTaskInfo = exTasksTotal > 0 ? $"<color=#E1564BFF>({exTasksCompleted}/{exTasksTotal})</color>" : "";

                    string playerInfoText = "";
                    string meetingInfoText = "";                    
                    if (p == CachedPlayer.LocalPlayer.PlayerControl) {
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
                    else if (TORMapOptions.ghostsSeeRoles || (Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target)) {
                        playerInfoText = $"{roleText}";
                        meetingInfoText = playerInfoText;
                    }

                    playerInfo.text = playerInfoText;
                    playerInfo.gameObject.SetActive(p.Visible);
                    if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
                }                
            }
        }

        public static void securityGuardSetTarget() {
            if (SecurityGuard.securityGuard == null || SecurityGuard.securityGuard != CachedPlayer.LocalPlayer.PlayerControl || MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

            Vent target = null;
            Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
            float closestDistance = float.MaxValue;
            for (int i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++) {
                Vent vent = MapUtilities.CachedShipStatus.AllVents[i];
                if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 9) continue; // cannot seal submergeds exit only vent!
                float distance = Vector2.Distance(vent.transform.position, truePosition);
                if (distance <= vent.UsableDistance && distance < closestDistance) {
                    closestDistance = distance;
                    target = vent;
                }
            }
            SecurityGuard.ventTarget = target;
        }

        public static void securityGuardUpdate() {
            if (SecurityGuard.securityGuard == null || CachedPlayer.LocalPlayer.PlayerControl != SecurityGuard.securityGuard || SecurityGuard.securityGuard.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(SecurityGuard.securityGuard.Data);
            if (playerCompleted == SecurityGuard.rechargedTasks) {
                SecurityGuard.rechargedTasks += SecurityGuard.rechargeTasksNumber;
                if (SecurityGuard.maxCharges > SecurityGuard.charges) SecurityGuard.charges++;
            }
        }

        public static void arsonistSetTarget() {
            if (Arsonist.arsonist == null || Arsonist.arsonist != CachedPlayer.LocalPlayer.PlayerControl) return;
            List<PlayerControl> untargetables;
            if (Arsonist.douseTarget != null)
            {
                untargetables = new();
                foreach (CachedPlayer cachedPlayer in CachedPlayer.AllPlayers)
                {
                    if (cachedPlayer.PlayerId != Arsonist.douseTarget.PlayerId)
                    {
                        untargetables.Add(cachedPlayer);
                    }
                }
            }
            else untargetables = Arsonist.dousedPlayers;
            Arsonist.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Arsonist.currentTarget != null) setPlayerOutline(Arsonist.currentTarget, Arsonist.color);
        }

        static void snitchUpdate() {
            if (Snitch.snitch == null) return;
            if (!Snitch.needsUpdate) return;

            bool snitchIsDead = Snitch.snitch.Data.IsDead;
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);

            if (playerTotal == 0) return;
            PlayerControl local = CachedPlayer.LocalPlayer.PlayerControl;

            int numberOfTasks = playerTotal - playerCompleted;

            if (Snitch.isRevealed && ((Snitch.targets == Snitch.Targets.EvilPlayers && Helpers.isEvil(local)) || (Snitch.targets == Snitch.Targets.Killers && Helpers.isKiller(local)))) {
                if (Snitch.text == null) {
                    Snitch.text = GameObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                    Snitch.text.enableWordWrapping = false;
                    Snitch.text.transform.localScale = Vector3.one * 0.75f;
                    Snitch.text.transform.localPosition += new Vector3(0f, 1.8f, -69f);
                    Snitch.text.gameObject.SetActive(true);
                } else {
                    Snitch.text.text = ModTranslation.getString("snitchAlive") + playerCompleted + "/" + playerTotal;
                    if (snitchIsDead) Snitch.text.text = ModTranslation.getString("snitchDead");
                }
            }

            if (snitchIsDead) {
                if (MeetingHud.Instance == null) Snitch.needsUpdate = false;
                return;
            }
            if (numberOfTasks <= Snitch.taskCountForReveal) Snitch.isRevealed = true;
        }

        static void bountyHunterUpdate() {
            if (BountyHunter.bountyHunter == null || CachedPlayer.LocalPlayer.PlayerControl != BountyHunter.bountyHunter) return;

            if (BountyHunter.bountyHunter.Data.IsDead) {
                if (BountyHunter.arrow != null || BountyHunter.arrow.arrow != null) UnityEngine.Object.Destroy(BountyHunter.arrow.arrow);
                BountyHunter.arrow = null;
                if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null) UnityEngine.Object.Destroy(BountyHunter.cooldownText.gameObject);
                BountyHunter.cooldownText = null;
                BountyHunter.bounty = null;
                foreach (PoolablePlayer p in TORMapOptions.playerIcons.Values) {
                    if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                }
                return;
            }

            BountyHunter.arrowUpdateTimer -= Time.fixedDeltaTime;
            BountyHunter.bountyUpdateTimer -= Time.fixedDeltaTime;

            if (BountyHunter.bounty == null || BountyHunter.bountyUpdateTimer <= 0f) {
                // Set new bounty
                BountyHunter.bounty = null;
                BountyHunter.arrowUpdateTimer = 0f; // Force arrow to update
                BountyHunter.bountyUpdateTimer = BountyHunter.bountyDuration;
                var possibleTargets = new List<PlayerControl>();
                foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                    if (!p.Data.IsDead && !p.Data.Disconnected && p != p.Data.Role.IsImpostor && p != Spy.spy && (p != Sidekick.sidekick || !Sidekick.wasTeamRed) && (p != Jackal.jackal || !Jackal.wasTeamRed) && (p != Mini.mini || Mini.isGrownUp()) && (Lovers.getPartner(BountyHunter.bountyHunter) == null || p != Lovers.getPartner(BountyHunter.bountyHunter))) possibleTargets.Add(p);
                }
                BountyHunter.bounty = possibleTargets[TheOtherRoles.rnd.Next(0, possibleTargets.Count)];
                if (BountyHunter.bounty == null) return;

                // Ghost Info
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.BountyTarget);
                writer.Write(BountyHunter.bounty.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null) {
                    foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
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
                if (BountyHunter.arrow == null) BountyHunter.arrow = new Arrow(Color.red);
                if (BountyHunter.arrowUpdateTimer <= 0f) {
                    BountyHunter.arrow.Update(BountyHunter.bounty.transform.position);
                    BountyHunter.arrowUpdateTimer = BountyHunter.arrowUpdateIntervall;
                }
                BountyHunter.arrow.Update();
            }
        }

        static void jekyllAndHydeSetTarget()
        {
            if (JekyllAndHyde.jekyllAndHyde == null || CachedPlayer.LocalPlayer.PlayerControl != JekyllAndHyde.jekyllAndHyde || JekyllAndHyde.jekyllAndHyde.Data.IsDead || JekyllAndHyde.isJekyll()) return;
            JekyllAndHyde.currentTarget = setTarget();
            setPlayerOutline(JekyllAndHyde.currentTarget, JekyllAndHyde.color);
        }

        public static void trapperUpdate()
        {
            try
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper && Trap.traps.Count != 0 && !Trap.hasTrappedPlayer() && !Trapper.meetingFlag)
                {
                    foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        foreach (var trap in Trap.traps)
                        {
                            if (DateTime.UtcNow.Subtract(trap.Value.placedTime).TotalSeconds < Trapper.extensionTime) continue;
                            if (trap.Value.isActive || p.Data.IsDead || p.inVent || Trapper.meetingFlag) continue;
                            var p1 = p.transform.localPosition;
                            Dictionary<GameObject, byte> listActivate = new();
                            var p2 = trap.Value.trap.transform.localPosition;
                            var distance = Vector3.Distance(p1, p2);
                            if (distance < Trapper.trapRange)
                            {
                                TMPro.TMP_Text text;
                                RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                                gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                                gameObject.transform.localScale = Vector3.one * 2f;
                                text = gameObject.GetComponent<TMPro.TMP_Text>();
                                text.text = string.Format(ModTranslation.getString("trapperGotTrapText"), p.Data.PlayerName);
                                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
                                {
                                    if (p == 1f && text != null && text.gameObject != null)
                                    {
                                        UnityEngine.Object.Destroy(text.gameObject);
                                    }
                                })));
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ActivateTrap, Hazel.SendOption.Reliable, -1);
                                writer.Write(trap.Key);
                                writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                                writer.Write(p.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.activateTrap(trap.Key, CachedPlayer.LocalPlayer.PlayerControl.PlayerId, p.PlayerId);
                                break;
                            }
                        }
                    }
                }

                if (CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper && Trap.hasTrappedPlayer() && !Trapper.meetingFlag)
                {
                    // トラップにかかっているプレイヤーを救出する
                    foreach (var trap in Trap.traps)
                    {
                        if (trap.Value.trap == null || !trap.Value.isActive) return;
                        Vector3 p1 = trap.Value.trap.transform.position;
                        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        {
                            if (player.PlayerId == trap.Value.target.PlayerId || player.Data.IsDead || player.inVent || player == Trapper.trapper) continue;
                            Vector3 p2 = player.transform.position;
                            float distance = Vector3.Distance(p1, p2);
                            if (distance < 0.5)
                            {
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DisableTrap, Hazel.SendOption.Reliable, -1);
                                writer.Write(trap.Key);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.disableTrap(trap.Key);
                            }
                        }

                    }
                }
            }
            catch (NullReferenceException e)
            {
                TheOtherRolesPlugin.Logger.LogWarning(e.Message);
            }
        }

        static void accelTrapUpdate()
        {
            if (!CustomOptionHolder.activateProps.getBool()) return;
            if (Props.AccelTrap.accels == null || Props.AccelTrap.accels.Count == 0 || MeetingHud.Instance) return;
            try
            {
                foreach (var acce in Props.AccelTrap.accels)
                {
                    if (!CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && acce.accelTrap.transform != null
                        && Vector3.Distance(CachedPlayer.LocalPlayer.PlayerControl.transform.position, acce.accelTrap.transform.position) < 0.25f &&
                        !CachedPlayer.LocalPlayer.PlayerControl.inVent)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ActivateAccel, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.activateAccel(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    }
                }
                if (Props.AccelTrap.acceled.ContainsKey(CachedPlayer.LocalPlayer.PlayerControl) && DateTime.UtcNow.Subtract(Props.AccelTrap.acceled[CachedPlayer.LocalPlayer.PlayerControl]).TotalSeconds >
                        CustomOptionHolder.accelerationDuration.getFloat())
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DeactivateAccel, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.deactivateAccel(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                }
            }
            catch (NullReferenceException e)
            {
                TheOtherRolesPlugin.Logger.LogWarning(e.Message);
            }
        }

        static void decelTrapUpdate()
        {
            if (!CustomOptionHolder.activateProps.getBool()) return;
            if (Props.DecelTrap.decels == null || Props.DecelTrap.decels.Count == 0 || MeetingHud.Instance) return;
            try
            {
                foreach (var decel in Props.DecelTrap.decels)
                {
                    if (!CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && decel.decelTrap.transform != null
                        && Vector3.Distance(CachedPlayer.LocalPlayer.PlayerControl.transform.position, decel.decelTrap.transform.position) < 0.6f &&
                        !CachedPlayer.LocalPlayer.PlayerControl.inVent && !decel.isTriggered)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ActivateDecel, Hazel.SendOption.Reliable, -1);
                        writer.Write(Props.DecelTrap.getId(decel));
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.activateDecel(Props.DecelTrap.getId(decel), CachedPlayer.LocalPlayer.PlayerId);
                    }

                    if (decel.isTriggered && DateTime.UtcNow.Subtract(decel.activateTime).TotalSeconds >= CustomOptionHolder.decelUpdateInterval.getFloat())
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UntriggerDecel, Hazel.SendOption.Reliable, -1);
                        writer.Write(Props.DecelTrap.getId(decel));
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.untriggerDecel(Props.DecelTrap.getId(decel));
                    }

                    if (Props.DecelTrap.deceled.ContainsKey(CachedPlayer.LocalPlayer.PlayerControl) && DateTime.UtcNow.Subtract(Props.DecelTrap.deceled[CachedPlayer.LocalPlayer.PlayerControl]).TotalSeconds >
                        CustomOptionHolder.decelerationDuration.getFloat())
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DeactivateDecel, Hazel.SendOption.Reliable, -1);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.deactivateDecel(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    }
                }
            }
            catch (NullReferenceException e)
            {
                TheOtherRolesPlugin.Logger.LogWarning(e.Message);
            }
        }

        static void vultureUpdate() {
            if (Vulture.vulture == null || CachedPlayer.LocalPlayer.PlayerControl != Vulture.vulture || Vulture.localArrows == null || !Vulture.showArrows) return;
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

        public static void mediumSetTarget() {
            if (Medium.medium == null || Medium.medium != CachedPlayer.LocalPlayer.PlayerControl || Medium.medium.Data.IsDead || Medium.deadBodies == null || MapUtilities.CachedShipStatus?.AllVents == null) return;

            DeadPlayer target = null;
            Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach ((DeadPlayer dp, Vector3 ps) in Medium.deadBodies) {
                float distance = Vector2.Distance(ps, truePosition);
                if (distance <= usableDistance && distance < closestDistance) {
                    closestDistance = distance;
                    target = dp;
                }
            }
            Medium.target = target;
        }

        static bool mushroomSaboWasActive = false;
        static void morphlingAndCamouflagerUpdate() {
            bool mushRoomSaboIsActive = Helpers.MushroomSabotageActive();
            if (!mushroomSaboWasActive) mushroomSaboWasActive = mushRoomSaboIsActive;

            float oldCamouflageTimer = Camouflager.camouflageTimer;
            float oldMorphTimer = Morphling.morphTimer;
            Camouflager.camouflageTimer = Mathf.Max(0f, Camouflager.camouflageTimer - Time.fixedDeltaTime);
            Morphling.morphTimer = Mathf.Max(0f, Morphling.morphTimer - Time.fixedDeltaTime);

            if (mushRoomSaboIsActive) return;

            // Camouflage reset and set Morphling look if necessary
            if (oldCamouflageTimer > 0f && Camouflager.camouflageTimer <= 0f) {
                Camouflager.resetCamouflage();
                if (Morphling.morphTimer > 0f && Morphling.morphling != null && Morphling.morphTarget != null) {
                    PlayerControl target = Morphling.morphTarget;
                    Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (MimicK.mimicK != null && MimicK.victim != null)
                {
                    PlayerControl target = MimicK.victim;
                    MimicK.mimicK.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (MimicK.mimicK != null && MimicA.mimicA != null && MimicA.isMorph)
                {
                    PlayerControl target = MimicK.mimicK;
                    MimicA.mimicA.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
            }

            // If the MushRoomSabotage ends while Morph is still active set the Morphlings look to the target's look
            if (mushroomSaboWasActive)
            {
                if (Morphling.morphTimer > 0f && Morphling.morphling != null && Morphling.morphTarget != null)
                {
                    PlayerControl target = Morphling.morphTarget;
                    Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (MimicK.mimicK != null && MimicK.victim != null)
                {
                    PlayerControl target = MimicK.victim;
                    MimicK.mimicK.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (MimicK.mimicK != null && MimicA.mimicA != null && MimicA.isMorph)
                {
                    PlayerControl target = MimicK.mimicK;
                    MimicA.mimicA.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (Camouflager.camouflageTimer > 0)
                {
                    foreach (PlayerControl player in CachedPlayer.AllPlayers)
                        player.setLook("", 6, "", "", "", "");
                }
            }

            // Morphling reset (only if camouflage is inactive)
            if (Camouflager.camouflageTimer <= 0f && oldMorphTimer > 0f && Morphling.morphTimer <= 0f && Morphling.morphling != null)
                Morphling.resetMorph();
            mushroomSaboWasActive = false;
        }

        /*public static void mimicAUpdate()
        {
            if (MimicA.mimicA == null) return;
            if (MimicA.mimicA.Data.IsDead) return;
            if (MimicK.mimicK == null || MimicK.mimicK.Data.IsDead)
            {
                MimicA.isMorph = false;
                MimicA.mimicA.setDefaultLook();
                return;
            }
            if (MimicA.isMorph && !MimicK.mimicK.Data.IsDead && MimicK.mimicK != null) MimicA.mimicA.setLook(MimicK.name, MimicK.mimicK.Data.DefaultOutfit.ColorId, MimicK.mimicK.Data.DefaultOutfit.HatId, MimicK.mimicK.Data.DefaultOutfit.VisorId, MimicK.mimicK.Data.DefaultOutfit.SkinId, MimicK.mimicK.Data.DefaultOutfit.PetId);
            else MimicA.mimicA.setDefaultLook();
        }*/

        static void evilHackerSetTarget()
        {
            if (EvilHacker.evilHacker == null || EvilHacker.evilHacker != CachedPlayer.LocalPlayer.PlayerControl) return;
            EvilHacker.currentTarget = setTarget(true);
            setPlayerOutline(EvilHacker.currentTarget, EvilHacker.color);
        }

        static void blackmailerSetTarget()
        {
            if (Blackmailer.blackmailer == null || Blackmailer.blackmailer != CachedPlayer.LocalPlayer.PlayerControl) return;
            Blackmailer.currentTarget = setTarget();
            setPlayerOutline(Blackmailer.currentTarget, Blackmailer.blackmailedColor);
        }

        public static void lawyerUpdate() {
            if (Lawyer.lawyer == null || Lawyer.lawyer != CachedPlayer.LocalPlayer.PlayerControl) return;

            // Meeting win
            if (Lawyer.winsAfterMeetings && Lawyer.neededMeetings == Lawyer.meetings && Lawyer.target != null && !Lawyer.target.Data.IsDead)
            {
                Lawyer.winsAfterMeetings = false; // Avoid sending mutliple RPCs until the host finshes the game
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerWin, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                RPCProcedure.lawyerWin();
                return;
            }

            // Promote to Pursuer
            if (Lawyer.target != null && Lawyer.target.Data.Disconnected && !Lawyer.lawyer.Data.IsDead) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer();
                return;
            }
        }

        public static void bomberASetTarget()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl != BomberA.bomberA || BomberA.bomberA == null || BomberA.bomberA.Data.IsDead) return;
            List<PlayerControl> untargetables;
            if (BomberA.tmpTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != BomberA.tmpTarget.PlayerId).ToList();
            else
            {
                untargetables = new List<PlayerControl>();
                if (BomberA.bombTarget != null) untargetables.Add(BomberA.bombTarget);
            }
            BomberA.currentTarget = setTarget(untargetablePlayers: untargetables);
            setPlayerOutline(BomberA.currentTarget, Palette.ImpostorRed);
        }

        public static void bomberBSetTarget()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl != BomberB.bomberB || BomberB.bomberB == null || BomberB.bomberB.Data.IsDead) return;
            List<PlayerControl> untargetables;
            if (BomberB.tmpTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != BomberB.tmpTarget.PlayerId).ToList();
            else
            {
                untargetables = new List<PlayerControl>();
                if (BomberB.bombTarget != null) untargetables.Add(BomberB.bombTarget);
            }
            BomberB.currentTarget = setTarget(untargetablePlayers: untargetables);
            setPlayerOutline(BomberB.currentTarget, Palette.ImpostorRed);
        }

        public static void akujoSetTarget()
        {
            if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl != Akujo.akujo) return;
            var untargetables = new List<PlayerControl>();
            if (Akujo.honmei != null) untargetables.Add(Akujo.honmei);
            if (Akujo.keeps != null) untargetables.AddRange(Akujo.keeps);
            Akujo.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Akujo.honmei == null || Akujo.keepsLeft > 0) setPlayerOutline(Akujo.currentTarget, Akujo.color);
        }

        static void prophetUpdate()
        {
            if (Prophet.arrows == null) return;

            foreach (var arrow in Prophet.arrows) arrow.arrow.SetActive(false);

            if (Prophet.prophet == null || Prophet.prophet.Data.IsDead) return;

            if (Prophet.isRevealed && Helpers.isKiller(CachedPlayer.LocalPlayer.PlayerControl))
            {
                if (Prophet.arrows.Count == 0) Prophet.arrows.Add(new Arrow(Prophet.color));
                if (Prophet.arrows.Count != 0 && Prophet.arrows[0] != null)
                {
                    Prophet.arrows[0].arrow.SetActive(true);
                    Prophet.arrows[0].Update(Prophet.prophet.transform.position);
                }
            }
        }

        public static void cupidUpdate()
        {
            if (Cupid.cupid == null || Cupid.cupid.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl != Cupid.cupid) return;
            Cupid.timeLeft = (int)Math.Ceiling(Cupid.timeLimit - (DateTime.UtcNow - Cupid.startTime).TotalSeconds);

            if (Cupid.timeLeft > 0)
            {
                if (Cupid.lovers1 == null || Cupid.lovers2 == null)
                {
                    if (HudManagerStartPatch.cupidTimeRemainingText != null)
                    {
                        HudManagerStartPatch.cupidTimeRemainingText.text = TimeSpan.FromSeconds(Cupid.timeLeft).ToString(@"mm\:ss");
                    }
                    HudManagerStartPatch.cupidTimeRemainingText.enabled = !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                      !MeetingHud.Instance &&
                      !ExileController.Instance;
                }
                else HudManagerStartPatch.cupidTimeRemainingText.enabled = false;
            }
            else if (Cupid.timeLeft <= 0)
            {
                if (Cupid.lovers1 == null || Cupid.lovers2 == null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CupidSuicide, Hazel.SendOption.Reliable, -1);
                    writer.Write(Cupid.cupid.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.cupidSuicide(Cupid.cupid.PlayerId, false);
                }
            }
        }

        public static void akujoUpdate()
        {
            if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl != Akujo.akujo) return;
            Akujo.timeLeft = (int)Math.Ceiling(Akujo.timeLimit - (DateTime.UtcNow - Akujo.startTime).TotalSeconds);
            if (Akujo.timeLeft > 0)
            {
                if (Akujo.honmei == null)
                {
                    if (HudManagerStartPatch.akujoTimeRemainingText != null)
                    {
                        HudManagerStartPatch.akujoTimeRemainingText.text = TimeSpan.FromSeconds(Akujo.timeLeft).ToString(@"mm\:ss");
                    }
                    HudManagerStartPatch.akujoTimeRemainingText.enabled = !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                      !MeetingHud.Instance &&
                      !ExileController.Instance;
                }
                else HudManagerStartPatch.akujoTimeRemainingText.enabled = false;
            }
            else if (Akujo.timeLeft <= 0)
            {
                if (Akujo.honmei == null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSuicide, Hazel.SendOption.Reliable, -1);
                    writer.Write(Akujo.akujo.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.akujoSuicide(Akujo.akujo.PlayerId);
                }
            }
        }

        public static void bomberAUpdate()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == BomberA.bomberA)
            {
                BomberA.arrowUpdate();
                BomberA.playerIconsUpdate();
            }
        }

        public static void bomberBUpdate()
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == BomberB.bomberB)
            {
                BomberB.arrowUpdate();
                BomberB.playerIconsUpdate();
            }
        }

        /*public static void ninjaUpdate()
        {
            if (Ninja.ninja == null) return;
            if (Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive()) Ninja.ninja.setDefaultLook();
            if (Ninja.stealthed && Ninja.invisibleTimer <= 0 && Camouflager.camouflageTimer <= 0f && Ninja.ninja == CachedPlayer.LocalPlayer.PlayerControl)
            {
                MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.NinjaStealth, Hazel.SendOption.Reliable, -1);
                invisibleWriter.Write(Ninja.ninja.PlayerId);
                invisibleWriter.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.ninjaStealth(Ninja.ninja.PlayerId, false);
            }
        }

        public static void sprinterUpdate()
        {
            if (Sprinter.sprinter == null) return;
            if (Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive()) Sprinter.sprinter.setDefaultLook();
            if (Sprinter.sprinting && Sprinter.invisibleTimer <= 0 && Sprinter.sprinter == CachedPlayer.LocalPlayer.PlayerControl)
            {
                MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SprinterSprint, Hazel.SendOption.Reliable, -1);
                invisibleWriter.Write(Sprinter.sprinter.PlayerId);
                invisibleWriter.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.sprinterSprint(Sprinter.sprinter.PlayerId, false);
            }
        }*/

        public static void fortuneTellerUpdate()
        {
            if (FortuneTeller.fortuneTeller == null) return;
            if (FortuneTeller.fortuneTeller == CachedPlayer.LocalPlayer.PlayerControl && !FortuneTeller.meetingFlag)
            {
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (!FortuneTeller.progress.ContainsKey(p.PlayerId)) FortuneTeller.progress[p.PlayerId] = 0f;
                    if (p.Data.IsDead) continue;
                    var fortuneTeller = CachedPlayer.LocalPlayer.PlayerControl;
                    float distance = Vector3.Distance(p.transform.position, fortuneTeller.transform.position);
                    // 障害物判定
                    bool anythingBetween = PhysicsHelpers.AnythingBetween(p.GetTruePosition(), fortuneTeller.GetTruePosition(), Constants.ShipAndObjectsMask, false);
                    if (!anythingBetween && distance <= FortuneTeller.distance && FortuneTeller.progress[p.PlayerId] < FortuneTeller.duration)
                    {
                        FortuneTeller.progress[p.PlayerId] += Time.fixedDeltaTime;
                    }
                }
            }
        }

        /*public static void serialKillerUpdate()
        {
            if (SerialKiller.serialKiller == null || CachedPlayer.LocalPlayer.PlayerControl != SerialKiller.serialKiller) return;
            if (SerialKiller.isCountDown) HudManagerStartPatch.serialKillerButton.isEffectActive = true;
        }*/

        public static void evilTrackerUpdate()
        {
            if (EvilTracker.evilTracker == null) return;
            if (CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker) EvilTracker.arrowUpdate();
            if (!CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker)
            {
                EvilTracker.currentTarget = setTarget();
                setPlayerOutline(EvilTracker.currentTarget, Palette.ImpostorRed);
            }
        }

        public static void moriartyUpdate()
        {
            if (Moriarty.moriarty == null || CachedPlayer.LocalPlayer.PlayerControl != Moriarty.moriarty || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead) return;
            Moriarty.arrowUpdate();
        }

        public static void impostorArrowUpdate()
        {
            if (FortuneTeller.arrows.FirstOrDefault()?.arrow != null)
            {
                if (FortuneTeller.fortuneTeller == null || FortuneTeller.fortuneTeller.Data.IsDead)
                {
                    foreach (Arrow arrows in FortuneTeller.arrows) arrows.arrow.SetActive(false);
                    return;
                }
            }
            if (CachedPlayer.LocalPlayer.Data.Role.IsImpostor)
            {
                // 前フレームからの経過時間をマイナスする
                FortuneTeller.updateTimer -= Time.fixedDeltaTime;

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

                    if (FortuneTeller.fortuneTeller == null || !FortuneTeller.divinedFlag || !FortuneTeller.isCompletedNumTasks(FortuneTeller.fortuneTeller) || FortuneTeller.fortuneTeller.Data.IsDead)
                    {
                        return;
                    }

                    Arrow arrow = new Arrow(FortuneTeller.color);
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

        public static void hackerUpdate() {
            if (Hacker.hacker == null || CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker || Hacker.hacker.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(Hacker.hacker.Data);
            if (playerCompleted == Hacker.rechargedTasks) {
                Hacker.rechargedTasks += Hacker.rechargeTasksNumber;
                if (Hacker.toolsNumber > Hacker.chargesVitals) Hacker.chargesVitals++;
                if (Hacker.toolsNumber > Hacker.chargesAdminTable) Hacker.chargesAdminTable++;
            }
        }

        // For swapper swap charges        
        public static void swapperUpdate() {
            if (Swapper.swapper == null || CachedPlayer.LocalPlayer.PlayerControl != Swapper.swapper || CachedPlayer.LocalPlayer.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data);
            if (playerCompleted == Swapper.rechargedTasks) {
                Swapper.rechargedTasks += Swapper.rechargeTasksNumber;
                Swapper.charges++;
            }
        }

        static void pursuerSetTarget() {
            if (Pursuer.pursuer == null || Pursuer.pursuer != CachedPlayer.LocalPlayer.PlayerControl) return;
            Pursuer.target = setTarget();
            setPlayerOutline(Pursuer.target, Pursuer.color);
        }

        static void witchSetTarget() {
            if (Witch.witch == null || Witch.witch != CachedPlayer.LocalPlayer.PlayerControl) return;
            List<PlayerControl> untargetables;
            if (Witch.spellCastingTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != Witch.spellCastingTarget.PlayerId).ToList(); // Don't switch the target from the the one you're currently casting a spell on
            else {
                untargetables = new List<PlayerControl>(); // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
                if (Spy.spy != null && !Witch.canSpellAnyone) untargetables.Add(Spy.spy);
                if (Sidekick.wasTeamRed && !Witch.canSpellAnyone) untargetables.Add(Sidekick.sidekick);
                if (Jackal.wasTeamRed && !Witch.canSpellAnyone) untargetables.Add(Jackal.jackal);
                //if (Ninja.stealthed && !Ninja.canBeTargeted) untargetables.Add(Ninja.ninja);
                //if (Sprinter.sprinting) untargetables.Add(Sprinter.sprinter);
            }
            Witch.currentTarget = setTarget(onlyCrewmates: !Witch.canSpellAnyone, untargetablePlayers: untargetables);
            setPlayerOutline(Witch.currentTarget, Witch.color);
        }

        static void madmateUpdate(PlayerControl player)
        {
            if (!Madmate.madmate.Any(x => x.PlayerId == player.PlayerId)) return;
            if (player.AmOwner && 
                !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                !MeetingHud.Instance &&
                !ExileController.Instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();
                if (!(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                !MeetingHud.Instance &&
                !ExileController.Instance)
                {
                    if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) && Madmate.canSabotage)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.SabotageButton.transform.localPosition = CustomButton.ButtonPositions.upperRowCenter + FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition;
                        FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
                        FastDestroyableSingleton<HudManager>.Instance.SabotageButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        static void createdMadmateUpdate(PlayerControl player)
        {
            if (CreatedMadmate.createdMadmate == null || CachedPlayer.LocalPlayer.PlayerControl != CreatedMadmate.createdMadmate) return;
            if (player.AmOwner &&
                !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                !MeetingHud.Instance &&
                !ExileController.Instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();
                if (!(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                !MeetingHud.Instance &&
                !ExileController.Instance)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == CreatedMadmate.createdMadmate && CreatedMadmate.canSabotage)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.SabotageButton.transform.localPosition = CustomButton.ButtonPositions.upperRowCenter + FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition;
                        FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
                        FastDestroyableSingleton<HudManager>.Instance.SabotageButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        static void assassinSetTarget()
        {
            if (Assassin.assassin == null || Assassin.assassin != CachedPlayer.LocalPlayer.PlayerControl) return;
            List<PlayerControl> untargetables = new List<PlayerControl>();
            if (Spy.spy != null && !Spy.impostorsCanKillAnyone) untargetables.Add(Spy.spy);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
            if (Sidekick.wasTeamRed && !Spy.impostorsCanKillAnyone) untargetables.Add(Sidekick.sidekick);
            if (Jackal.wasTeamRed && !Spy.impostorsCanKillAnyone) untargetables.Add(Jackal.jackal);
            //if (Ninja.stealthed && !Ninja.canBeTargeted) untargetables.Add(Ninja.ninja);
            //if (Sprinter.sprinting) untargetables.Add(Sprinter.sprinter);
            Assassin.currentTarget = setTarget(onlyCrewmates: Spy.spy == null || !Spy.impostorsCanKillAnyone, untargetablePlayers: untargetables);
            setPlayerOutline(Assassin.currentTarget, Assassin.color);
        }

        static void thiefSetTarget() {
            if (Thief.thief == null || Thief.thief != CachedPlayer.LocalPlayer.PlayerControl) return;
            List<PlayerControl> untargetables = new List<PlayerControl>();
            if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
            //if (Ninja.ninja != null && Ninja.stealthed && !Ninja.canBeTargeted) untargetables.Add(Ninja.ninja);
            //if (Sprinter.sprinter != null && Sprinter.sprinting) untargetables.Add(Sprinter.sprinter);
            Thief.currentTarget = setTarget(onlyCrewmates: false, untargetablePlayers: untargetables);
            setPlayerOutline(Thief.currentTarget, Thief.color);
        }




        /*static void baitUpdate() {
            if (!Bait.active.Any()) return;

            // Bait report
            foreach (KeyValuePair<DeadPlayer, float> entry in new Dictionary<DeadPlayer, float>(Bait.active)) {
                Bait.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
                if (entry.Value <= 0) {
                    Bait.active.Remove(entry.Key);
                    if (entry.Key.killerIfExisting != null && entry.Key.killerIfExisting.PlayerId == CachedPlayer.LocalPlayer.PlayerId) {
                        Helpers.handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                        RPCProcedure.uncheckedCmdReportDeadBody(entry.Key.killerIfExisting.PlayerId, entry.Key.player.PlayerId);

                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
                        writer.Write(entry.Key.killerIfExisting.PlayerId);
                        writer.Write(entry.Key.player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
        }*/

        static void bloodyUpdate() {
            if (!Bloody.active.Any()) return;
            foreach (KeyValuePair<byte, float> entry in new Dictionary<byte, float>(Bloody.active)) {
                PlayerControl player = Helpers.playerById(entry.Key);
                PlayerControl bloodyPlayer = Helpers.playerById(Bloody.bloodyKillerMap[player.PlayerId]);      

                Bloody.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
                if (entry.Value <= 0 || player.Data.IsDead) {
                    Bloody.active.Remove(entry.Key);
                    continue;  // Skip the creation of the next blood drop, if the killer is dead or the time is up
                }
                new Bloodytrail(player, bloodyPlayer);
            }
        }

        // Mini set adapted button cooldown for Vampire, Sheriff, Jackal, Sidekick, Warlock, Cleaner
        public static void miniCooldownUpdate() {
            if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini) {
                var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
                HudManagerStartPatch.sheriffKillButton.MaxTimer = Sheriff.cooldown * multiplier;
                HudManagerStartPatch.vampireKillButton.MaxTimer = Vampire.cooldown * multiplier;
                HudManagerStartPatch.jackalKillButton.MaxTimer = Jackal.cooldown * multiplier;
                HudManagerStartPatch.sidekickKillButton.MaxTimer = Sidekick.cooldown * multiplier;
                HudManagerStartPatch.warlockCurseButton.MaxTimer = Warlock.cooldown * multiplier;
                HudManagerStartPatch.cleanerCleanButton.MaxTimer = Cleaner.cooldown * multiplier;
                HudManagerStartPatch.witchSpellButton.MaxTimer = (Witch.cooldown + Witch.currentCooldownAddition) * multiplier;
                HudManagerStartPatch.assassinButton.MaxTimer = Assassin.cooldown * multiplier;
                HudManagerStartPatch.thiefKillButton.MaxTimer = Thief.cooldown * multiplier;
            }
        }

        /*public static void trapperUpdate() {
            if (Trapper.trapper == null || CachedPlayer.LocalPlayer.PlayerControl != Trapper.trapper || Trapper.trapper.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.taskInfo(Trapper.trapper.Data);
            if (playerCompleted == Trapper.rechargedTasks) {
                Trapper.rechargedTasks += Trapper.rechargeTasksNumber;
                if (Trapper.maxCharges > Trapper.charges) Trapper.charges++;
            }
        }*/

        static void hunterUpdate() {
            if (!HideNSeek.isHideNSeekGM) return;
            int minutes = (int)HideNSeek.timer / 60;
            int seconds = (int)HideNSeek.timer % 60;
            string suffix = $" {minutes:00}:{seconds:00}";

            if (HideNSeek.timerText == null) {
                RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                if (roomTracker != null) {
                    GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    HideNSeek.timerText = gameObject.GetComponent<TMPro.TMP_Text>();

                    // Use local position to place it in the player's view instead of the world location
                    gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                    if (AmongUs.Data.DataManager.Settings.Gameplay.StreamerMode) gameObject.transform.localPosition = new Vector3(0, 2f, gameObject.transform.localPosition.z);
                }
            } else {
                if (HideNSeek.isWaitingTimer) {
                    HideNSeek.timerText.text = "<color=#0000cc>" + suffix + "</color>";
                    HideNSeek.timerText.color = Color.blue;
                } else {
                    HideNSeek.timerText.text = "<color=#FF0000FF>" + suffix + "</color>";
                    HideNSeek.timerText.color = Color.red;
                }
            }
            if (HideNSeek.isHunted() && !Hunted.taskPunish && !HideNSeek.isWaitingTimer) {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data);
                int numberOfTasks = playerTotal - playerCompleted;
                if (numberOfTasks == 0) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareTimer, Hazel.SendOption.Reliable, -1);
                    writer.Write(HideNSeek.taskPunish);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.shareTimer(HideNSeek.taskPunish);

                    Hunted.taskPunish = true;
                }
            }

            if (!HideNSeek.isHunter()) return;

            byte playerId = CachedPlayer.LocalPlayer.PlayerId;
            foreach (Arrow arrow in Hunter.localArrows) arrow.arrow.SetActive(false);
            if (Hunter.arrowActive) {
                int arrowIndex = 0;
                foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                    if (!p.Data.IsDead && !p.Data.Role.IsImpostor) {
                        if (arrowIndex >= Hunter.localArrows.Count) {
                            Hunter.localArrows.Add(new Arrow(Color.blue));
                        }
                        if (arrowIndex < Hunter.localArrows.Count && Hunter.localArrows[arrowIndex] != null) {
                            Hunter.localArrows[arrowIndex].arrow.SetActive(true);
                            Hunter.localArrows[arrowIndex].Update(p.transform.position, Color.blue);
                        }
                        arrowIndex++;
                    }
                }
            }
        }

        public static void Postfix(PlayerControl __instance) {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

            // Mini and Morphling shrink
            playerSizeUpdate(__instance);
            
            // set position of colorblind text
            foreach (var pc in PlayerControl.AllPlayerControls) {
                //pc.cosmetics.colorBlindText.gameObject.transform.localPosition = new Vector3(0, 0, -0.0001f);
            }
            
            if (CachedPlayer.LocalPlayer.PlayerControl == __instance) {
                // Update player outlines
                setBasePlayerOutlines();

                // Update Role Description
                Helpers.refreshRoleDescription(__instance);

                // Update Player Info
                updatePlayerInfo();

                // Time Master
                bendTimeUpdate();
                // Morphling
                morphlingSetTarget();
                // Medic
                medicSetTarget();
                // Shifter
                shifterSetTarget();
                // Sheriff
                sheriffSetTarget();
                // Deputy
                deputySetTarget();
                deputyUpdate();
                // Detective
                detectiveUpdateFootPrints();
                // Tracker
                trackerSetTarget();
                // Vampire
                vampireSetTarget();
                Garlic.UpdateAll();
                //Catcher
                CatcherSetTarget();
                //Trap.Update();
                // Eraser
                eraserSetTarget();
                // Engineer
                engineerUpdate();
                // Tracker
                trackerUpdate();
                // Fortune Teller
                fortuneTellerUpdate();
                // Jackal
                jackalSetTarget();
                // Sidekick
                sidekickSetTarget();
                // Impostor
                impostorSetTarget();
                impostorArrowUpdate();  // If the Camouflager/Thief is having problem, please delete this line
                // Warlock
                warlockSetTarget();
                // Check for deputy promotion on Sheriff disconnect
                deputyCheckPromotion();
                // Check for sidekick promotion on Jackal disconnect
                sidekickCheckPromotion();
                // SecurityGuard
                securityGuardSetTarget();
                securityGuardUpdate();
                // Arsonist
                arsonistSetTarget();
                // Snitch
                snitchUpdate();                
                // BountyHunter
                bountyHunterUpdate();
                // Vulture
                vultureUpdate();
                // Medium
                mediumSetTarget();
                // Morphling and Camouflager
                morphlingAndCamouflagerUpdate();
                // Lawyer
                lawyerUpdate();
                // Teleporter
                teleporterSetTarget();
                // Pursuer
                pursuerSetTarget();
                // Bomber
                bomberASetTarget();
                bomberBSetTarget();
                bomberAUpdate();
                bomberBUpdate();
                // Bait
                baitUpdate();
                // Ninja
                //ninjaUpdate();
                // Sprinter
                //sprinterUpdate();
                // Serial Killer
                //serialKillerUpdate();
                // Evil Tracker
                evilTrackerUpdate();
                // Evil Hacker
                evilHackerSetTarget();
                // Undertaker
                UndertakerSetTarget();
                UndertakerCanDropTarget();
                UndertakerUpdate();
                // Mimic(Killer)
                MimicK.arrowUpdate();
                // Mimic(Assistant)
                MimicA.arrowUpdate();
                // Trapper
                trapperUpdate();
                // Madmate
                madmateUpdate(__instance);
                createdMadmateUpdate(__instance);
                // Moriarty
                moriartySetTarget();
                moriartyUpdate();
                // Cupid
                cupidSetTarget();
                cupidUpdate();
                // Blackmailer
                blackmailerSetTarget();
                // Prophet
                prophetSetTarget();
                prophetUpdate();
                // Plague Doctor
                plagueDoctorSetTarget();
                plagueDoctorUpdate();
                // Fox
                foxSetTarget();
                foxUpdate();
                // Immorailst
                immoralistUpdate();
                // Jekyll and Hyde
                jekyllAndHydeSetTarget();
                // Akujo
                akujoSetTarget();
                akujoUpdate();
                //mimicAUpdate();
                // Witch                
                witchSetTarget();
                // Assassin
                assassinSetTarget();
                AssassinTrace.UpdateAll();
                assassinUpdate();
                // Thief
                thiefSetTarget();

                hackerUpdate();
                swapperUpdate();
                // Hacker
                hackerUpdate();
                // Trapper
                //trapperUpdate();                                

                // -- MODIFIER--
                // Bait
                //baitUpdate();
                // Bloody
                bloodyUpdate();
                // mini (for the cooldowns)
                miniCooldownUpdate();
                // Chameleon (invis stuff, timers)
                Chameleon.update();
                //Bomb.update();
                // Bomber
                BombEffect.UpdateAll();
                // Props
                accelTrapUpdate();
                decelTrapUpdate();

                // -- GAME MODE --
                hunterUpdate();
            }            
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    class PlayerPhysicsWalkPlayerToPatch {
        private static Vector2 offset = Vector2.zero;
        public static void Prefix(PlayerPhysics __instance) {
            bool correctOffset = Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (__instance.myPlayer == Mini.mini ||  (Morphling.morphling != null && __instance.myPlayer == Morphling.morphling && Morphling.morphTarget == Mini.mini && Morphling.morphTimer > 0f)
                 || (MimicA.mimicA != null && __instance.myPlayer == MimicA.mimicA && MimicA.isMorph && MimicK.mimicK != null && MimicK.mimicK == Mini.mini));
            correctOffset = correctOffset && !(Mini.mini == Morphling.morphling && Morphling.morphTimer > 0f) && !(Mini.mini == MimicA.mimicA && MimicA.isMorph) && !(Mini.mini == MimicK.mimicK && MimicK.victim != null);
            if (correctOffset) {
                float currentScaling = (Mini.growingProgress() + 1) * 0.5f;
                __instance.myPlayer.Collider.offset = currentScaling * Mini.defaultColliderOffset * Vector2.down;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    class PlayerControlCmdReportDeadBodyPatch {
        public static bool Prefix(PlayerControl __instance) {
            if (HideNSeek.isHideNSeekGM) return false;
            Helpers.handleVampireBiteOnBodyReport();
            Helpers.HandleUndertakerDropOnBodyReport();
            Helpers.handleTrapperTrapOnBodyReport();
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(CachedPlayer.LocalPlayer.PlayerControl.CmdReportDeadBody))]
    class BodyReportPatch
    {
        static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
        {
            if (Moriarty.brainwashed.FindAll(x => x.PlayerId == __instance.PlayerId).Count > 0)
            {
                return false;
            }
            return true;
        }

        static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo target)
        {
            // Medic or Detective report
            bool isMedicReport = Medic.medic != null && Medic.medic == CachedPlayer.LocalPlayer.PlayerControl && __instance.PlayerId == Medic.medic.PlayerId;
            bool isDetectiveReport = Detective.detective != null && Detective.detective == CachedPlayer.LocalPlayer.PlayerControl && __instance.PlayerId == Detective.detective.PlayerId;
            if (isMedicReport || isDetectiveReport)
            {
                DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == target?.PlayerId)?.FirstOrDefault();

                if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                    float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds);
                    string msg = "";

                    if (isMedicReport) {
                        msg = string.Format(ModTranslation.getString("medicReport"), Math.Round(timeSinceDeath / 1000));
                    } else if (isDetectiveReport) {
                        if (timeSinceDeath < Detective.reportNameDuration * 1000) {
                            msg = string.Format(ModTranslation.getString("detectiveReportName"), deadPlayer.killerIfExisting.Data.PlayerName);
                        } else if (timeSinceDeath < Detective.reportColorDuration * 1000) {
                            var typeOfColor = Helpers.isLighterColor(deadPlayer.killerIfExisting.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("detectiveColorLight") : ModTranslation.getString("detectiveColorDark");
                            msg = string.Format(ModTranslation.getString("detectiveReportColor"), typeOfColor);
                        } else {
                            msg = ModTranslation.getString("detectiveReportNone");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(msg))
                    {   
                        if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                        {
                            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, msg);

                            // Ghost Info
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                            writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                            writer.Write((byte)RPCProcedure.GhostInfoTypes.DetectiveOrMedicInfo);
                            writer.Write(msg);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                        /*if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            FastDestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
                        }*/
                    }
                }
            }  
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class MurderPlayerPatch
    {
        public static bool resetToCrewmate = false;
        public static bool resetToDead = false;

        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target)
        {
            // Allow everyone to murder players
            resetToCrewmate = !__instance.Data.Role.IsImpostor;
            resetToDead = __instance.Data.IsDead;
            __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
            __instance.Data.IsDead = false;
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target)
        {
            // Collect dead player info
            DeadPlayer deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Kill, __instance);
            GameHistory.deadPlayers.Add(deadPlayer);

            // Reset killer to crewmate if resetToCrewmate
            if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            if (resetToDead) __instance.Data.IsDead = true;

            // Remove fake tasks when player dies
            if (target.hasFakeTasks() || target == Lawyer.lawyer || target == Pursuer.pursuer || target == Thief.thief || (target == Shifter.shifter && Shifter.isNeutral) || Madmate.madmate.Any(x => x.PlayerId == target.PlayerId) || target == CreatedMadmate.createdMadmate || target == JekyllAndHyde.jekyllAndHyde || target == Fox.fox)
                target.clearAllTasks();

            // First kill (set before lover suicide)
            if (TORMapOptions.firstKillName == "") TORMapOptions.firstKillName = target.Data.PlayerName;

            // Lover suicide trigger on murder
            if ((Lovers.lover1 != null && target == Lovers.lover1) || (Lovers.lover2 != null && target == Lovers.lover2)) {
                PlayerControl otherLover = target == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie) {
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }
            }

            // Cupid and Cupid Lovers suicide
            if (Cupid.lovers1 != null && Cupid.lovers2 != null && (target == Cupid.lovers1 || target == Cupid.lovers2))
            {
                PlayerControl otherLover = target == Cupid.lovers1 ? Cupid.lovers2 : Cupid.lovers1;
                if (otherLover != null && !otherLover.Data.IsDead)
                {
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }
                if (Cupid.cupid != null && !Cupid.cupid.Data.IsDead)
                {
                    Cupid.cupid.MurderPlayer(Cupid.cupid, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(Cupid.cupid, DeadPlayer.CustomDeathReason.Suicide);
                }
            }

            // Sidekick promotion trigger on murder
            if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && target == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes();
            }

            // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
            if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer();
            }

            // Seer show flash and add dead player position
            if (Seer.seer != null && (CachedPlayer.LocalPlayer.PlayerControl == Seer.seer || Helpers.shouldShowGhostInfo()) && !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1) {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message : ModTranslation.getString("seerInfo"));
            }
            if (Seer.deadBodyPositions != null) Seer.deadBodyPositions.Add(target.transform.position);

            // Tracker store body positions
            if (Tracker.deadBodyPositions != null) Tracker.deadBodyPositions.Add(target.transform.position);

            // Medium add body
            if (Medium.deadBodies != null) {
                Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));
            }

            // Show flash on bait kill to the killer if enabled
            if (Bait.bait != null && target == Bait.bait && Bait.showKillFlash && __instance != Bait.bait && __instance == CachedPlayer.LocalPlayer.PlayerControl)
            {
                Helpers.showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
            }

            // Ninja penalize
            if (Ninja.ninja != null && CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja && __instance == Ninja.ninja)
            {
                Ninja.OnKill(target);
            }

            // Serial Killer set suicide timer
            if (SerialKiller.serialKiller != null && CachedPlayer.LocalPlayer.PlayerControl == SerialKiller.serialKiller && __instance == SerialKiller.serialKiller && target != SerialKiller.serialKiller)
            {
                //HudManagerStartPatch.serialKillerButton.isEffectActive = false;
                SerialKiller.serialKiller.SetKillTimer(SerialKiller.killCooldown);
                HudManagerStartPatch.serialKillerButton.Timer = SerialKiller.suicideTimer;
                SerialKiller.isCountDown = true;
                //HudManagerStartPatch.serialKillerButton.isEffectActive = true;
            }

            if (JekyllAndHyde.jekyllAndHyde != null && CachedPlayer.LocalPlayer.PlayerControl == JekyllAndHyde.jekyllAndHyde && __instance == JekyllAndHyde.jekyllAndHyde && target != JekyllAndHyde.jekyllAndHyde)
            {
                JekyllAndHyde.counter++;
                if (JekyllAndHyde.counter >= JekyllAndHyde.numberToWin) JekyllAndHyde.triggerWin = true;
                HudManagerStartPatch.jekyllAndHydeSuicideButton.Timer = JekyllAndHyde.suicideTimer;
            }

            // Trapper peforms kills
            if (Trapper.trapper != null && CachedPlayer.LocalPlayer.PlayerControl == Trapper.trapper && __instance == Trapper.trapper)
            {
                if (Trap.isTrapped(target) && !Trapper.isTrapKill)  // トラップにかかっている対象をキルした場合のボーナス
                {
                    Trapper.trapper.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - Trapper.bonusTime;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Trapper.cooldown - Trapper.bonusTime;
                }
                else if (Trap.isTrapped(target) && Trapper.isTrapKill)  // トラップキルした場合のペナルティ
                {
                    Trapper.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Trapper.cooldown;
                }
                else // トラップにかかっていない対象を通常キルした場合はペナルティーを受ける
                {
                    Trapper.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Trapper.penaltyTime;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Trapper.cooldown + Trapper.penaltyTime;
                }
                if (!Trapper.isTrapKill)
                {
                    MessageWriter writer;
                    writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ClearTrap, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.clearTrap();
                }
                Trapper.isTrapKill = false;
            }

            // Neko-Kabocha kill murderer
            if (NekoKabocha.nekoKabocha != null && target == NekoKabocha.nekoKabocha && __instance != NekoKabocha.nekoKabocha)
            {
                if (!__instance.Data.IsDead)
                {
                    if ((__instance.Data.Role.IsImpostor && NekoKabocha.revengeImpostor)
                        || (Helpers.isNeutral(__instance) && NekoKabocha.revengeNeutral)
                        || ((NekoKabocha.revengeCrew && (!Helpers.isNeutral(__instance) && !__instance.Data.Role.IsImpostor))))
                    {
                        NekoKabocha.nekoKabocha.MurderPlayer(__instance, MurderResultFlags.Succeeded);
                        GameHistory.overrideDeathReasonAndKiller(__instance, DeadPlayer.CustomDeathReason.Revenge, killer: NekoKabocha.nekoKabocha);
                    }
                }
            }

            // Other Bomber trigger suicide
            if ((BomberA.bomberA != null && target == BomberA.bomberA) || (BomberB.bomberB != null && target == BomberB.bomberB))
            {
                var bomberPartner = target == BomberA.bomberA ? BomberB.bomberB : BomberA.bomberA;
                if (bomberPartner != null && BomberA.ifOneDiesBothDie && !bomberPartner.Data.IsDead)
                {
                    bomberPartner.MurderPlayer(bomberPartner, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(bomberPartner, DeadPlayer.CustomDeathReason.Suicide);
                }
            }

            // Other Mimic trigger suicide
            if ((MimicK.mimicK != null && target == MimicK.mimicK) || (MimicA.mimicA != null && target == MimicA.mimicA))
            {
                var mimicPartner = target == MimicK.mimicK ? MimicA.mimicA : MimicK.mimicK;
                if (mimicPartner != null && MimicK.ifOneDiesBothDie && !mimicPartner.Data.IsDead)
                {
                    mimicPartner.MurderPlayer(mimicPartner, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(mimicPartner, DeadPlayer.CustomDeathReason.Suicide);
                }
            }

            // Akujo Lovers trigger suicide
            if ((Akujo.akujo != null && target ==  Akujo.akujo) || (Akujo.honmei != null && target == Akujo.honmei))
            {
                PlayerControl akujoPartner = target == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
                if (akujoPartner != null && !akujoPartner.Data.IsDead)
                {
                    akujoPartner.MurderPlayer(akujoPartner, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(akujoPartner, DeadPlayer.CustomDeathReason.LoverSuicide);
                }
            }

            // Evil Tracker see flash
            if (__instance.Data.Role.IsImpostor && __instance != EvilTracker.evilTracker && CachedPlayer.LocalPlayer.PlayerControl == EvilTracker.evilTracker && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && EvilTracker.canSeeDeathFlash)
            {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: ModTranslation.getString("evilTrackerInfo"));
            }

            if (Immoralist.immoralist != null && CachedPlayer.LocalPlayer.PlayerControl == Immoralist.immoralist && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));

            if (Fox.fox != null && target == Fox.fox)
            {
                if (Immoralist.immoralist != null && !Immoralist.immoralist.Data.IsDead)
                {
                    Immoralist.immoralist.MurderPlayer(Immoralist.immoralist, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(Immoralist.immoralist, DeadPlayer.CustomDeathReason.Suicide);
                }
            }

            // Plague Doctor infect killer
            if (PlagueDoctor.plagueDoctor != null && target == PlagueDoctor.plagueDoctor && PlagueDoctor.infectKiller)
            {
                byte targetId = __instance.PlayerId;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlagueDoctorSetInfected, Hazel.SendOption.Reliable, -1);
                writer.Write(targetId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.plagueDoctorInfected(targetId);
            }

            if (PlagueDoctor.plagueDoctor != null && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead)) PlagueDoctor.checkWinStatus();

            // Mimic(Killer) morph into victim
            if (MimicK.mimicK != null && __instance == MimicK.mimicK)
            {
                // Delete the dead body
                DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == target.PlayerId)
                    {
                        array[i].gameObject.active = false;
                    }
                }

                // Block Mimic(Killer) from morphing if camo or mushroom is active
                if (Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive())
                    MimicK.mimicK.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                MimicK.victim = target;
            }

            // Mimic morph and arrows
            if (MimicK.mimicK != null && target == MimicK.mimicK)
            {
                MimicK.mimicK.setDefaultLook();
                MimicK.victim = null;
                if (MimicA.mimicA != null)
                {
                    MimicA.mimicA.setDefaultLook();
                    MimicA.isMorph = false;
                }
            }

            if (MimicA.mimicA != null && target == MimicA.mimicA)
            {
                MimicA.mimicA.setDefaultLook();
                MimicA.isMorph = false;
            }

            // Set the correct opacity to the Ninja and Sprinter
            if (Ninja.ninja != null && target == Ninja.ninja)
            {
                Ninja.stealthed = false;
                Ninja.setOpacity(Ninja.ninja, 1.0f);
            }

            if (Sprinter.sprinter != null && target == Sprinter.sprinter)
            {
                Sprinter.sprinting = false;
                Sprinter.setOpacity(Sprinter.sprinter, 1.0f);
            }

            // Mimic(Assistant) show flash
            if (MimicK.mimicK != null && MimicA.mimicA != null && __instance == MimicK.mimicK && CachedPlayer.LocalPlayer.PlayerControl == MimicA.mimicA && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
            {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: ModTranslation.getString("mimicAInfo"));
            }

            // Sherlock record log
            Sherlock.killLog.Add(Tuple.Create(__instance.PlayerId, Tuple.Create(target.PlayerId, target.transform.position + Vector3.zero)));

            // Set bountyHunter cooldown
            if (BountyHunter.bountyHunter != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter && __instance == BountyHunter.bountyHunter) {
                if (target == BountyHunter.bounty) {
                    BountyHunter.bountyHunter.SetKillTimer(BountyHunter.bountyKillCooldown);
                    BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
                }
                else
                    BountyHunter.bountyHunter.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + BountyHunter.punishmentTime); 
            }

            // Mini Set Impostor Mini kill timer (Due to mini being a modifier, all "SetKillTimers" must have happened before this!)
            if (Mini.mini != null && __instance == Mini.mini && __instance == CachedPlayer.LocalPlayer.PlayerControl) {
                float multiplier = 1f;
                if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
                Mini.mini.SetKillTimer(__instance.killTimer * multiplier);
            }

            // Cleaner Button Sync
            if (Cleaner.cleaner != null && CachedPlayer.LocalPlayer.PlayerControl == Cleaner.cleaner && __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null)
                HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

            // Witch Button Sync
            if (Witch.triggerBothCooldowns && Witch.witch != null && CachedPlayer.LocalPlayer.PlayerControl == Witch.witch && __instance == Witch.witch && HudManagerStartPatch.witchSpellButton != null)
                HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

            // Warlock Button Sync
            if (Warlock.warlock != null && CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock && __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null) {
                if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer) {
                    HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
                }
            }
            // Assassin Button Sync
            if (Assassin.assassin != null && CachedPlayer.LocalPlayer.PlayerControl == Assassin.assassin && __instance == Assassin.assassin && HudManagerStartPatch.assassinButton != null)
                HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;

            // Bait
            /*if (Bait.bait.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) {
                float reportDelay = (float) rnd.Next((int)Bait.reportDelayMin, (int)Bait.reportDelayMax + 1);
                Bait.active.Add(deadPlayer, reportDelay);

                if (Bait.showKillFlash && __instance == CachedPlayer.LocalPlayer.PlayerControl) Helpers.showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
            }*/

            // Add Bloody Modifier
            if (Bloody.bloody.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.Bloody, Hazel.SendOption.Reliable, -1);
                writer.Write(__instance.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.bloody(__instance.PlayerId, target.PlayerId);
            }

            // VIP Modifier
            if (Vip.vip.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) {
                Color color = Color.yellow;
                if (Vip.showColor) {
                    color = Color.white;
                    if (target.Data.Role.IsImpostor) color = Color.red;
                    else if (RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault().isNeutral) color = Color.blue;
                }
                Helpers.showFlash(color, 1.5f);
            }

            // HideNSeek
            if (HideNSeek.isHideNSeekGM) {
                int visibleCounter = 0;
                Vector3 bottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft + new Vector3(-0.25f, -0.25f, 0);
                foreach (PlayerControl p in CachedPlayer.AllPlayers) {
                    if (!TORMapOptions.playerIcons.ContainsKey(p.PlayerId) || p.Data.Role.IsImpostor) continue;
                    if (p.Data.IsDead || p.Data.Disconnected) {
                        TORMapOptions.playerIcons[p.PlayerId].gameObject.SetActive(false);
                    }
                    else {
                        TORMapOptions.playerIcons[p.PlayerId].transform.localPosition = bottomLeft + Vector3.right * visibleCounter * 0.35f;
                        visibleCounter++;
                    }
                }
            }

            // Snitch
            if (Snitch.snitch != null && CachedPlayer.LocalPlayer.PlayerId == Snitch.snitch.PlayerId && MapBehaviourPatch.herePoints.Keys.Any(x => x.PlayerId == target.PlayerId)) {
                foreach (var a in MapBehaviourPatch.herePoints.Where(x => x.Key.PlayerId == target.PlayerId)) {
                    UnityEngine.Object.Destroy(a.Value);
                    MapBehaviourPatch.herePoints.Remove(a.Key);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    class PlayerControlSetCoolDownPatch {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)]float time) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;
            if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f) return false;
            float multiplier = 1f;
            float addition = 0f;
            if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            if (BountyHunter.bountyHunter != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter) addition = BountyHunter.punishmentTime;
            if (Ninja.ninja != null && CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja && Ninja.penalized) addition += Ninja.killPenalty;

            __instance.killTimer = Mathf.Clamp(time, 0f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier + addition);
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier + addition);
            return false;
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
    class KillAnimationCoPerformKillPatch {
        public static bool hideNextAnimation = false;
        public static void Prefix(KillAnimation __instance, [HarmonyArgument(0)]ref PlayerControl source, [HarmonyArgument(1)]ref PlayerControl target) {
            if (hideNextAnimation)
                source = target;
            hideNextAnimation = false;
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement))]
    class KillAnimationSetMovementPatch {
        private static int? colorId = null;
        public static void Prefix(PlayerControl source, bool canMove) {
            Color color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
            if (Morphling.morphling != null && source.Data.PlayerId == Morphling.morphling.PlayerId) {
                var index = Palette.PlayerColors.IndexOf(color);
                if (index != -1) colorId = index;
            }
        }

        public static void Postfix(PlayerControl source, bool canMove) {
            if (colorId.HasValue) source.RawSetColor(colorId.Value);
            colorId = null;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public static class PlayerControlDiePatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            if (TaskMaster.isTaskMaster(__instance.PlayerId) && __instance.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId && TaskMaster.isTaskComplete)
                __instance.clearAllTasks();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class ExilePlayerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            // Collect dead player info
            DeadPlayer deadPlayer = new DeadPlayer(__instance, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Exile, null);
            GameHistory.deadPlayers.Add(deadPlayer);


            // Remove fake tasks when player dies
            if (__instance.hasFakeTasks() || __instance == Lawyer.lawyer || __instance == Pursuer.pursuer || __instance == Thief.thief || (__instance == Shifter.shifter && Shifter.isNeutral) || Madmate.madmate.Any(x => x.PlayerId == __instance.PlayerId) || __instance == CreatedMadmate.createdMadmate || __instance == JekyllAndHyde.jekyllAndHyde || __instance == Fox.fox)
                __instance.clearAllTasks();

            // Neko-Kabocha revenge on exile
            if (NekoKabocha.nekoKabocha != null && __instance == NekoKabocha.nekoKabocha && NekoKabocha.meetingKiller == null && NekoKabocha.otherKiller == null && CachedPlayer.LocalPlayer.PlayerControl == NekoKabocha.nekoKabocha)
            {
                if (NekoKabocha.revengeExile)
                {
                    var candidates = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x != NekoKabocha.nekoKabocha && !x.Data.IsDead).ToList();
                    int targetID = rnd.Next(0, candidates.Count);
                    var target = candidates[targetID];
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.NekoKabochaExile, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    //writer.Write((byte)DeadPlayer.CustomDeathReason.Revenge);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.nekoKabochaExile(target.PlayerId);
                }
            }

            // Neko-Kabocha set dead to meeting killer
            if (NekoKabocha.nekoKabocha != null && __instance == NekoKabocha.nekoKabocha && NekoKabocha.meetingKiller != null)
            {
                PlayerControl killer = NekoKabocha.meetingKiller;
                bool revengeFlag = (NekoKabocha.revengeCrew && (!Helpers.isNeutral(killer) && !killer.Data.Role.IsImpostor)) ||
                (NekoKabocha.revengeNeutral && Helpers.isNeutral(killer)) ||
                    (NekoKabocha.revengeImpostor && killer.Data.Role.IsImpostor);

                if (MeetingHud.Instance && revengeFlag)
                {
                    foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva.VotedFor != killer.PlayerId) continue;
                        pva.UnsetVote();
                        var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                        if (!voteAreaPlayer.AmOwner) continue;
                        MeetingHud.Instance.ClearVote();
                    }

                    if (AmongUsClient.Instance.AmHost)
                        MeetingHud.Instance.CheckForEndVoting();
                }
            }

            // Lover suicide trigger on exile
            if ((Lovers.lover1 != null && __instance == Lovers.lover1) || (Lovers.lover2 != null && __instance == Lovers.lover2)) {
                PlayerControl otherLover = __instance == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie) {
                    if (NekoKabocha.nekoKabocha != null && otherLover == NekoKabocha.nekoKabocha) NekoKabocha.otherKiller = otherLover; // Can put other non-null values here
                    otherLover.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }

            }

            // Cupid and Cupid Lovers suicide
            if (Cupid.lovers1 != null && Cupid.lovers2 != null && (__instance == Cupid.lovers1 || __instance == Cupid.lovers2))
            {
                PlayerControl otherLover = __instance == Cupid.lovers1 ? Cupid.lovers2 : Cupid.lovers1;
                if (otherLover != null && !otherLover.Data.IsDead)
                {
                    if (NekoKabocha.nekoKabocha != null && otherLover == NekoKabocha.nekoKabocha) NekoKabocha.otherKiller = otherLover;
                    otherLover.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }

                if (MeetingHud.Instance && otherLover != null)
                {
                    foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva.VotedFor != otherLover.PlayerId) continue;
                        pva.UnsetVote();
                        var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                        if (!voteAreaPlayer.AmOwner) continue;
                        MeetingHud.Instance.ClearVote();
                    }

                    if (Cupid.cupid != null && !Cupid.cupid.Data.IsDead)
                    {
                        foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                        {
                            if (pva.VotedFor != Cupid.cupid.PlayerId) continue;
                            pva.UnsetVote();
                            var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                            if (!voteAreaPlayer.AmOwner) continue;
                            MeetingHud.Instance.ClearVote();
                        }
                    }

                    if (AmongUsClient.Instance.AmHost)
                        MeetingHud.Instance.CheckForEndVoting();
                }

                if (Cupid.cupid != null && !Cupid.cupid.Data.IsDead)
                {
                    Cupid.cupid.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(Cupid.cupid, DeadPlayer.CustomDeathReason.Suicide);
                }
            }

            // Other Mimic suicide
            if (MimicK.mimicK != null && MimicA.mimicA != null && (__instance == MimicA.mimicA || __instance == MimicK.mimicK))
            {
                PlayerControl otherMimic = __instance == MimicK.mimicK ? MimicA.mimicA : MimicK.mimicK;
                if (MimicK.ifOneDiesBothDie && !otherMimic.Data.IsDead)
                {
                    otherMimic.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(otherMimic, DeadPlayer.CustomDeathReason.Suicide);
                }

                // Going to reset the votes here
                if (MeetingHud.Instance && MimicK.ifOneDiesBothDie && otherMimic != null)
                {
                    foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva.VotedFor != otherMimic.PlayerId) continue;
                        pva.UnsetVote();
                        var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                        if (!voteAreaPlayer.AmOwner) continue;
                        MeetingHud.Instance.ClearVote();
                    }

                    if (AmongUsClient.Instance.AmHost)
                        MeetingHud.Instance.CheckForEndVoting();
                }
            }

            // Other Bomber trigger suicide
            if ((BomberA.bomberA != null && __instance == BomberA.bomberA) || (BomberB.bomberB != null && __instance == BomberB.bomberB))
            {
                var otherBomber = __instance == BomberA.bomberA ? BomberB.bomberB : BomberA.bomberA;
                if (otherBomber != null && BomberA.ifOneDiesBothDie && !otherBomber.Data.IsDead)
                {
                    otherBomber.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(otherBomber, DeadPlayer.CustomDeathReason.Suicide);
                }

                if (MeetingHud.Instance && BomberA.ifOneDiesBothDie && otherBomber != null)
                {
                    foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva.VotedFor != otherBomber.PlayerId) continue;
                        pva.UnsetVote();
                        var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                        if (!voteAreaPlayer.AmOwner) continue;
                        MeetingHud.Instance.ClearVote();
                    }

                    if (AmongUsClient.Instance.AmHost)
                        MeetingHud.Instance.CheckForEndVoting();
                }
            }

            // Akujo Partner suicide
            if ((Akujo.akujo != null && Akujo.akujo == __instance) || (Akujo.honmei != null && Akujo.honmei == __instance))
            {
                PlayerControl akujoPartner = __instance == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
                if (akujoPartner != null && !akujoPartner.Data.IsDead)
                {
                    akujoPartner.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(akujoPartner, DeadPlayer.CustomDeathReason.Suicide);
                }

                if (MeetingHud.Instance && akujoPartner != null)
                {
                    foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva.VotedFor != akujoPartner.PlayerId) continue;
                        pva.UnsetVote();
                        var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                        if (!voteAreaPlayer.AmOwner) continue;
                        MeetingHud.Instance.ClearVote();
                    }

                    if (AmongUsClient.Instance.AmHost)
                        MeetingHud.Instance.CheckForEndVoting();
                }
            }

            if (Fox.fox != null && __instance == Fox.fox)
            {
                if (Immoralist.immoralist != null)
                {
                    Immoralist.immoralist.Exiled();
                    GameHistory.overrideDeathReasonAndKiller(Immoralist.immoralist, DeadPlayer.CustomDeathReason.Suicide);
                    if (MeetingHud.Instance) RPCProcedure.updateMeeting(Immoralist.immoralist.PlayerId);
                }
            }

            // Check Plague Doctor status
            if (PlagueDoctor.plagueDoctor != null && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead)) PlagueDoctor.checkWinStatus();

            // Sidekick promotion trigger on exile
            if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && __instance == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes();
            }

            // Pursuer promotion trigger on exile & suicide (the host sends the call such that everyone recieves the update before a possible game End)
            if (Lawyer.lawyer != null && __instance == Lawyer.target) {
                PlayerControl lawyer = Lawyer.lawyer;
                // && !Lawyer.isProsecutor
                if (AmongUsClient.Instance.AmHost && ((Lawyer.target != Jester.jester) || Lawyer.targetWasGuessed)) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }

                 //&& !Lawyer.isProsecutor
                /*if (!Lawyer.targetWasGuessed) {
                    if (Lawyer.lawyer != null) Lawyer.lawyer.Exiled();
                    if (Pursuer.pursuer != null) Pursuer.pursuer.Exiled();

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.DeathReasonAndKiller);
                    writer.Write(lawyer.PlayerId);
                    //writer.Write((byte)DeadPlayer.CustomDeathReason.LawyerSuicide);
                    writer.Write(lawyer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    //GameHistory.overrideDeathReasonAndKiller(lawyer, DeadPlayer.CustomDeathReason.LawyerSuicide, lawyer);  // TODO: only executed on host?!
                }*/
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysicsFixedUpdate {
        public static void Postfix(PlayerPhysics __instance)
        {
            bool shouldInvert = Invert.invert.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0 && Invert.meetings > 0;  // xor. if already invert, eventInvert will turn it off for 10s
            if (__instance.AmOwner &&
                AmongUsClient.Instance &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !CachedPlayer.LocalPlayer.Data.IsDead && 
                shouldInvert && 
                GameData.Instance && 
                __instance.myPlayer.CanMove)  
                __instance.body.velocity *= -1;

            if (__instance.AmOwner &&
                AmongUsClient.Instance &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !CachedPlayer.LocalPlayer.Data.IsDead &&
                GameData.Instance &&
                __instance.myPlayer.CanMove &&
                CachedPlayer.LocalPlayer.PlayerControl == Undertaker.undertaker &&
                Undertaker.DraggedBody != null)
            {
                __instance.body.velocity *= 1f + Undertaker.speedDecrease / 100f;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsFlashlightEnabled))]
    public static class IsFlashlightEnabledPatch {
        public static bool Prefix(ref bool __result) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return true;
            __result = false;
            if (!CachedPlayer.LocalPlayer.Data.IsDead && Lighter.lighter != null && Lighter.lighter.PlayerId == CachedPlayer.LocalPlayer.PlayerId) {
                __result = true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
    public static class AdjustLight {
        public static bool Prefix(PlayerControl __instance) {
            if (__instance == null || CachedPlayer.LocalPlayer == null || Lighter.lighter == null) return true;

            bool hasFlashlight = !CachedPlayer.LocalPlayer.Data.IsDead && Lighter.lighter.PlayerId == CachedPlayer.LocalPlayer.PlayerId;
            __instance.SetFlashlightInputMethod();
            __instance.lightSource.SetupLightingForGameplay(hasFlashlight, Lighter.flashlightWidth, __instance.TargetFlashlight.transform);

            return false;
        }
    }
    
    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), new[] {typeof(PlayerControl), typeof(DisconnectReasons) })]
    public static class GameDataHandleDisconnectPatch {
        public static void Prefix(GameData __instance, PlayerControl player, DisconnectReasons reason) {
            if (MeetingHud.Instance) {
                MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, player.PlayerId);
            }
        }
    }
}
