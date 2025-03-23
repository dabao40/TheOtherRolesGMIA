using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.GameHistory;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;
using TheOtherRoles.CustomGameModes;
using static UnityEngine.GraphicsBuffer;
using AmongUs.GameOptions;
using Sentry.Internal.Extensions;
using TheOtherRoles.Modules;
using TheOtherRoles.MetaContext;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdatePatch {
        // Helpers

        public static PlayerControl setTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null) {
            PlayerControl result = null;
            float num = AmongUs.GameOptions.GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 2)];
            if (!MapUtilities.CachedShipStatus) return result;
            if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
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
            if (Kataomoi.kataomoi != null && Kataomoi.target != null && Kataomoi.isStalking()) {
                untargetablePlayers.Add(Kataomoi.kataomoi);
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

        public static void setPlayerOutline(PlayerControl target, Color color) {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;
            color = color.SetAlpha(Chameleon.visibility(target.PlayerId));

            target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
            target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
        }

        // Update functions

        static void setBasePlayerOutlines() {
            foreach (PlayerControl target in PlayerControl.AllPlayerControls) {
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

                if (PlayerControl.LocalPlayer.Data.IsDead && Armored.armored != null && target == Armored.armored && !Armored.isBrokenArmor && !hasVisibleShield)
                {
                    hasVisibleShield = true;
                    color = Color.yellow;
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
                        if (PlayerControl.LocalPlayer.inVent) {
                            foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents) {
                                bool canUse;
                                bool couldUse;
                                vent.CanUse(PlayerControl.LocalPlayer.Data, out canUse, out couldUse);
                                if (canUse) {
                                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(vent.Id);
                                    vent.SetButtons(false);
                                }
                            }
                        }
                        // Set position
                        PlayerControl.LocalPlayer.transform.position = next.Item1;
                    }
                    else if (localPlayerPositions.Any(x => x.Item2 == true)) {
                        PlayerControl.LocalPlayer.transform.position = next.Item1;
                    }
                    if (SubmergedCompatibility.IsSubmerged) {
                        SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);
                    }

                    localPlayerPositions.RemoveAt(0);

                    if (localPlayerPositions.Count > 1) localPlayerPositions.RemoveAt(0); // Skip every second position to rewinde twice as fast, but never skip the last position
                }
                else {
                    TimeMaster.isRewinding = false;
                    PlayerControl.LocalPlayer.moveable = true;
                }
            }
            else {
                while (localPlayerPositions.Count >= Mathf.Round(TimeMaster.rewindTime / Time.fixedDeltaTime)) localPlayerPositions.RemoveAt(localPlayerPositions.Count - 1);
                localPlayerPositions.Insert(0, new Tuple<Vector3, bool>(PlayerControl.LocalPlayer.transform.position, PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
            }
        }

        static void medicSetTarget() {
            if (Medic.medic == null || Medic.medic != PlayerControl.LocalPlayer) return;
            Medic.currentTarget = setTarget();
            if (!Medic.usedShield) setPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
        }

        static void shifterSetTarget() {
            if (Shifter.shifter == null || Shifter.shifter != PlayerControl.LocalPlayer) return;
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
            if (Morphling.morphling == null || Morphling.morphling != PlayerControl.LocalPlayer) return;
            Morphling.currentTarget = setTarget();
            setPlayerOutline(Morphling.currentTarget, Morphling.color);
        }

        static void sheriffSetTarget() {
            if (Sheriff.sheriff == null || Sheriff.sheriff != PlayerControl.LocalPlayer) return;
            Sheriff.currentTarget = setTarget();
            setPlayerOutline(Sheriff.currentTarget, Sheriff.color);
        }

        static void deputySetTarget()
        {
            if (Deputy.deputy == null || Deputy.deputy != PlayerControl.LocalPlayer) return;
            Deputy.currentTarget = setTarget();
            setPlayerOutline(Deputy.currentTarget, Deputy.color);
        }

        public static void deputyCheckPromotion(bool isMeeting=false)
        {
            // If LocalPlayer is Deputy, the Sheriff is disconnected and Deputy promotion is enabled, then trigger promotion
            if (Deputy.deputy == null || Deputy.deputy != PlayerControl.LocalPlayer) return;
            if (Deputy.promotesToSheriff == 0 || Deputy.deputy.Data.IsDead == true || Deputy.promotesToSheriff == 2 && !isMeeting) return;
            if (Sheriff.sheriff == null || Sheriff.sheriff?.Data?.Disconnected == true || Sheriff.sheriff.Data.IsDead)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DeputyPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.deputyPromotes();
            }
        }

        static void trackerSetTarget() {
            if (Tracker.tracker == null || Tracker.tracker != PlayerControl.LocalPlayer) return;
            Tracker.currentTarget = setTarget();
            if (!Tracker.usedTracker) setPlayerOutline(Tracker.currentTarget, Tracker.color);
        }

        static void vampireSetTarget() {
            if (Vampire.vampire == null || Vampire.vampire != PlayerControl.LocalPlayer) return;

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
            if (Jackal.jackal == null || Jackal.jackal != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.canCreateSidekickFromImpostor) {
                // Only exclude sidekick from beeing targeted if the jackal can create sidekicks from impostors
                if (Sidekick.sidekick != null) untargetablePlayers.Add(Sidekick.sidekick);
            }
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal) untargetablePlayers.Add(SchrodingersCat.schrodingersCat);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
            if (Bait.showBaitFor != 2) untargetablePlayers.Add(Bait.bait);
            Jackal.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            setPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);
        }

        static void sidekickSetTarget() {
            if (Sidekick.sidekick == null || Sidekick.sidekick != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.jackal != null) untargetablePlayers.Add(Jackal.jackal);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Sidekick from targeting the Mini unless it has grown up
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal) untargetablePlayers.Add(SchrodingersCat.schrodingersCat);
            Sidekick.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            if (Sidekick.canKill) setPlayerOutline(Sidekick.currentTarget, Palette.ImpostorRed);
        }

        static void sidekickCheckPromotion() {
            // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
            if (Sidekick.sidekick == null || Sidekick.sidekick != PlayerControl.LocalPlayer) return;
            if (Sidekick.sidekick.Data.IsDead == true || !Sidekick.promotesToJackal) return;
            if (Jackal.jackal == null || Jackal.jackal?.Data?.Disconnected == true) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes();
            }
        }

        static void eraserSetTarget() {
            if (Eraser.eraser == null || Eraser.eraser != PlayerControl.LocalPlayer) return;

            List<PlayerControl> untargetables = new();
            if (Spy.spy != null) untargetables.Add(Spy.spy);
            if (Sidekick.wasTeamRed) untargetables.Add(Sidekick.sidekick);
            if (Jackal.wasTeamRed) untargetables.Add(Jackal.jackal);
            Eraser.currentTarget = setTarget(onlyCrewmates: !Eraser.canEraseAnyone, untargetablePlayers: Eraser.canEraseAnyone ? new List<PlayerControl>() : untargetables);
            setPlayerOutline(Eraser.currentTarget, Eraser.color);
        }

        static void UndertakerSetTarget()
        {
            if (Undertaker.undertaker == null || Undertaker.undertaker != PlayerControl.LocalPlayer) return;
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
            if (PlayerControl.LocalPlayer == null || !Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId)) return;
            
            if (Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] <= 0)
            {
                Deputy.handcuffedKnows.Remove(PlayerControl.LocalPlayer.PlayerId);
                // Resets the buttons
                Deputy.setHandcuffedKnows(false);
                
                // Ghost info
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffOver);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
        }

        static void impostorSetTarget() {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor ||!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead || (PlayerControl.LocalPlayer == Undertaker.undertaker && Undertaker.DraggedBody != null)) { // !isImpostor || !canMove || isDead
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

        static void schrodingersCatSetTarget()
        {
            var untargetables = new List<PlayerControl>();
            if (SchrodingersCat.schrodingersCat == PlayerControl.LocalPlayer && SchrodingersCat.team == SchrodingersCat.Team.Jackal)
            {
                if (!SchrodingersCat.isTeamJackalAlive() || !SchrodingersCat.cantKillUntilLastOne)
                {
                    if (Jackal.jackal != null) untargetables.Add(Jackal.jackal);
                    if (Sidekick.sidekick != null) untargetables.Add(Sidekick.sidekick);
                    SchrodingersCat.currentTarget = setTarget(untargetablePlayers: untargetables);
                    setPlayerOutline(SchrodingersCat.currentTarget, Jackal.color);
                }
            }
            if (SchrodingersCat.schrodingersCat == PlayerControl.LocalPlayer && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde)
            {
                if (JekyllAndHyde.jekyllAndHyde == null || JekyllAndHyde.jekyllAndHyde.Data.IsDead || !SchrodingersCat.cantKillUntilLastOne)
                {
                    if (JekyllAndHyde.jekyllAndHyde != null) untargetables.Add(JekyllAndHyde.jekyllAndHyde);
                    SchrodingersCat.currentTarget = setTarget(untargetablePlayers: untargetables);
                    setPlayerOutline(SchrodingersCat.currentTarget, JekyllAndHyde.color);
                }
            }
            if (SchrodingersCat.schrodingersCat == PlayerControl.LocalPlayer && SchrodingersCat.team == SchrodingersCat.Team.Moriarty)
            {
                if (Moriarty.moriarty == null || Moriarty.moriarty.Data.IsDead || !SchrodingersCat.cantKillUntilLastOne)
                {
                    if (Moriarty.moriarty != null) untargetables.Add(Moriarty.moriarty);
                    SchrodingersCat.currentTarget = setTarget(untargetablePlayers: untargetables);
                    setPlayerOutline(SchrodingersCat.currentTarget, Moriarty.color);
                }
            }
            if (SchrodingersCat.schrodingersCat == PlayerControl.LocalPlayer && SchrodingersCat.team == SchrodingersCat.Team.Impostor && !SchrodingersCat.isLastImpostor() && SchrodingersCat.cantKillUntilLastOne)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            }
        }

        public static void kataomoiSetTarget()
        {
            if (Kataomoi.kataomoi == null || Kataomoi.kataomoi != PlayerControl.LocalPlayer) return;
            if (Kataomoi.target == null) return;

            var untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != Kataomoi.target.PlayerId).ToList();
            Kataomoi.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Kataomoi.currentTarget != null) setPlayerOutline(Kataomoi.currentTarget, Kataomoi.color);
        }

        static void baitUpdate()
        {
            if (Bait.bait == null || Bait.bait != PlayerControl.LocalPlayer) return;

            // Bait report
            if (Bait.bait.Data.IsDead && !Bait.reported)
            {
                Bait.reportDelay -= Time.fixedDeltaTime;
                DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == Bait.bait.PlayerId)?.FirstOrDefault();
                if (deadPlayer.killerIfExisting != null && Bait.reportDelay <= 0f)
                {
                    _ = new StaticAchievementToken("bait.common1");

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

                    Bait.acTokenChallenge.Value.killerId = reporter;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
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
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
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
            if (Moriarty.moriarty == null || Moriarty.moriarty != PlayerControl.LocalPlayer) return;
            Moriarty.currentTarget = setTarget();
            var untargetablePlayers = new List<PlayerControl>();
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Moriarty) untargetablePlayers.Add(SchrodingersCat.schrodingersCat);
            if (Moriarty.target != null) Moriarty.killTarget = setTarget(untargetablePlayers: untargetablePlayers, targetingPlayer: Moriarty.target);
            else Moriarty.killTarget = null;
            setPlayerOutline(Moriarty.currentTarget, Moriarty.color);
        }

        static void cupidSetTarget()
        {
            if (Cupid.cupid == null || Cupid.cupid != PlayerControl.LocalPlayer) return;
            var untargetables = new List<PlayerControl>();
            if (Cupid.lovers1 != null) untargetables.Add(Cupid.lovers1);
            Cupid.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Cupid.isShieldOn) Cupid.shieldTarget = setTarget();
            if (Cupid.lovers1 == null || Cupid.lovers2 == null) setPlayerOutline(Cupid.currentTarget, Cupid.color);
            else if (Cupid.shielded == null && Cupid.isShieldOn) setPlayerOutline(Cupid.shieldTarget, Cupid.color);
        }

        static void warlockSetTarget() {
            if (Warlock.warlock == null || Warlock.warlock != PlayerControl.LocalPlayer) return;
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
            if (Prophet.prophet == null ||PlayerControl.LocalPlayer != Prophet.prophet) return;
            Prophet.currentTarget = setTarget();
            if (Prophet.examinesLeft > 0) setPlayerOutline(Prophet.currentTarget, Prophet.color);
        }

        static void plagueDoctorSetTarget()
        {
            if (PlagueDoctor.plagueDoctor == null || PlayerControl.LocalPlayer != PlagueDoctor.plagueDoctor) return;
            if (!PlagueDoctor.plagueDoctor.Data.IsDead && PlagueDoctor.numInfections > 0)
            {
                PlagueDoctor.currentTarget = setTarget(untargetablePlayers: PlagueDoctor.infected.Values.ToList());
                setPlayerOutline(PlagueDoctor.currentTarget, PlagueDoctor.color);
            }
        }

        static void doomsayerSetTarget()
        {
            if (Doomsayer.doomsayer == null || PlayerControl.LocalPlayer != Doomsayer.doomsayer || PlayerControl.LocalPlayer.Data.IsDead || Doomsayer.usesLeft <= 0) return;
            Doomsayer.currentTarget = setTarget();
            setPlayerOutline(Doomsayer.currentTarget, Doomsayer.color);
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

            if (PlagueDoctor.plagueDoctor != null && (PlayerControl.LocalPlayer == PlagueDoctor.plagueDoctor || (PlayerControl.LocalPlayer.Data.IsDead &&
                !(PlayerControl.LocalPlayer == Busker.busker && Busker.pseudocideFlag))))
            {
                if (PlagueDoctor.statusText == null)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance?.roomTracker.gameObject);
                    gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                    gameObject.SetActive(true);
                    UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                    PlagueDoctor.statusText = gameObject.GetComponent<TMPro.TMP_Text>();
                    gameObject.transform.localPosition = new Vector3(-2.7f, -0.1f - PlayerControl.AllPlayerControls.ToArray().Select(x => !PlagueDoctor.dead.ContainsKey(x.PlayerId)).Count() * 0.07f, gameObject.transform.localPosition.z);

                    PlagueDoctor.statusText.transform.localScale = new Vector3(1f, 1f, 1f);
                    PlagueDoctor.statusText.fontSize = 1.5f;
                    PlagueDoctor.statusText.fontSizeMin = 1.5f;
                    PlagueDoctor.statusText.fontSizeMax = 1.5f;
                    PlagueDoctor.statusText.alignment = TMPro.TextAlignmentOptions.BottomLeft;
                    PlagueDoctor.statusText.alpha = byte.MaxValue;
                }

                PlagueDoctor.statusText.gameObject.SetActive(true);
                string text = $"[{ModTranslation.getString("plagueDoctorProgress")}]\n";

                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
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

            if (PlagueDoctor.plagueDoctor != null && PlayerControl.LocalPlayer == PlagueDoctor.plagueDoctor)
            {
                if (!PlagueDoctor.meetingFlag && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead))
                {
                    List<PlayerControl> newInfected = new();
                    foreach (PlayerControl target in PlayerControl.AllPlayerControls)
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
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlagueDoctorUpdateProgress, Hazel.SendOption.Reliable, -1);
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
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlagueDoctorSetInfected, Hazel.SendOption.Reliable, -1);
                            writer.Write(targetId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.plagueDoctorInfected(targetId);
                        }

                        bool winFlag = true;
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
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
                            MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlagueDoctorWin, Hazel.SendOption.Reliable, -1);
                            AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                            RPCProcedure.plagueDoctorWin();
                        }
                    }
                }
            }
        }

        static void foxSetTarget()
        {
            if (Fox.fox == null || PlayerControl.LocalPlayer != Fox.fox || PlayerControl.LocalPlayer.Data.IsDead) return;
            List<PlayerControl> untargetablePlayers = new();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.Data.Role.IsImpostor || p == Jackal.jackal || p == JekyllAndHyde.jekyllAndHyde || p == Moriarty.moriarty
                    || (p == SchrodingersCat.schrodingersCat && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate)
                    || (p == Sidekick.sidekick && Sidekick.canKill))
                {
                    untargetablePlayers.Add(p);
                }
            }
            if (Bait.showBaitFor != 2) untargetablePlayers.Add(Bait.bait);
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

        public static void securityGuardSetTarget() {
            if (SecurityGuard.securityGuard == null || SecurityGuard.securityGuard != PlayerControl.LocalPlayer || MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

            Vent target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
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

        public static void arsonistSetTarget() {
            if (Arsonist.arsonist == null || Arsonist.arsonist != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables;
            if (Arsonist.douseTarget != null)
            {
                untargetables = new();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId != Arsonist.douseTarget.PlayerId)
                    {
                        untargetables.Add(player);
                    }
                }
            }
            else untargetables = Arsonist.dousedPlayers;
            Arsonist.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Arsonist.currentTarget != null) setPlayerOutline(Arsonist.currentTarget, Arsonist.color);
        }

        static void archaeologistSetTarget()
        {
            if (Archaeologist.archaeologist == null || PlayerControl.LocalPlayer != Archaeologist.archaeologist || PlayerControl.LocalPlayer.Data.IsDead || Archaeologist.revealed == null || MapUtilities.CachedShipStatus?.AllVents == null) return;
            Antique target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach (var antique in Archaeologist.revealed)
            {
                float distance = Vector2.Distance(antique.gameObject.transform.position, truePosition);
                if (distance <= usableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = antique;
                }
            }
            Archaeologist.target = target;
        }

        static void jekyllAndHydeSetTarget()
        {
            if (JekyllAndHyde.jekyllAndHyde == null || PlayerControl.LocalPlayer != JekyllAndHyde.jekyllAndHyde || JekyllAndHyde.jekyllAndHyde.Data.IsDead || JekyllAndHyde.isJekyll()) return;
            var untargetables = new List<PlayerControl>();
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde) untargetables.Add(SchrodingersCat.schrodingersCat);
            JekyllAndHyde.currentTarget = setTarget(untargetablePlayers : untargetables);
            setPlayerOutline(JekyllAndHyde.currentTarget, JekyllAndHyde.color);
        }

        public static void trapperUpdate()
        {
            try
            {
                if (PlayerControl.LocalPlayer == Trapper.trapper && Trap.traps.Count != 0 && !Trap.hasTrappedPlayer() && !Trapper.meetingFlag)
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
                                text = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                                text.transform.localScale = Vector3.one;
                                text.transform.localPosition = new Vector3(0, -1.8f, -69f);
                                text.enableWordWrapping = false;
                                text.text = string.Format(ModTranslation.getString("trapperGotTrapText"), p.Data.PlayerName);
                                text.gameObject.SetActive(true);
                                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
                                {
                                    if (p == 1f && text != null && text.gameObject != null)
                                    {
                                        UnityEngine.Object.Destroy(text.gameObject);
                                    }
                                })));
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ActivateTrap, Hazel.SendOption.Reliable, -1);
                                writer.Write(trap.Key);
                                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                                writer.Write(p.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.activateTrap(trap.Key, PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
                                break;
                            }
                        }
                    }
                }

                if (PlayerControl.LocalPlayer == Trapper.trapper && Trap.hasTrappedPlayer() && !Trapper.meetingFlag)
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
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DisableTrap, Hazel.SendOption.Reliable, -1);
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

        public static void mediumSetTarget() {
            if (Medium.medium == null || Medium.medium != PlayerControl.LocalPlayer || Medium.medium.Data.IsDead || Medium.deadBodies == null || MapUtilities.CachedShipStatus?.AllVents == null) return;

            DeadPlayer target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
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
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        player.setLook("", 6, "", "", "", "");
                }
            }

            // Morphling reset (only if camouflage is inactive)
            if (Camouflager.camouflageTimer <= 0f && oldMorphTimer > 0f && Morphling.morphTimer <= 0f && Morphling.morphling != null)
                Morphling.resetMorph();
            mushroomSaboWasActive = false;
        }

        static void evilHackerSetTarget()
        {
            if (EvilHacker.evilHacker == null || EvilHacker.evilHacker != PlayerControl.LocalPlayer) return;
            EvilHacker.currentTarget = setTarget(true);
            setPlayerOutline(EvilHacker.currentTarget, EvilHacker.color);
        }

        static void blackmailerSetTarget()
        {
            if (Blackmailer.blackmailer == null || Blackmailer.blackmailer != PlayerControl.LocalPlayer) return;
            Blackmailer.currentTarget = setTarget();
            setPlayerOutline(Blackmailer.currentTarget, Blackmailer.blackmailedColor);
        }

        public static void lawyerUpdate() {
            if (Lawyer.lawyer == null || Lawyer.lawyer != PlayerControl.LocalPlayer) return;

            // Meeting win
            if (Lawyer.winsAfterMeetings && Lawyer.neededMeetings == Lawyer.meetings && Lawyer.target != null && !Lawyer.target.Data.IsDead)
            {
                Lawyer.winsAfterMeetings = false; // Avoid sending mutliple RPCs until the host finshes the game
                MessageWriter winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerWin, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                RPCProcedure.lawyerWin();
                return;
            }

            // Promote to Pursuer
            if (Lawyer.target != null && Lawyer.target.Data.Disconnected && !Lawyer.lawyer.Data.IsDead) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer();
                return;
            }
        }

        public static void bomberASetTarget()
        {
            if (PlayerControl.LocalPlayer != BomberA.bomberA || BomberA.bomberA == null || BomberA.bomberA.Data.IsDead) return;
            List<PlayerControl> untargetables = new();
            if (BomberA.tmpTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != BomberA.tmpTarget.PlayerId).ToList();
            BomberA.currentTarget = setTarget(untargetablePlayers: untargetables);
            setPlayerOutline(BomberA.currentTarget, Palette.ImpostorRed);
        }

        public static void bomberBSetTarget()
        {
            if (PlayerControl.LocalPlayer != BomberB.bomberB || BomberB.bomberB == null || BomberB.bomberB.Data.IsDead) return;
            List<PlayerControl> untargetables = new();
            if (BomberB.tmpTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != BomberB.tmpTarget.PlayerId).ToList();
            BomberB.currentTarget = setTarget(untargetablePlayers: untargetables);
            setPlayerOutline(BomberB.currentTarget, Palette.ImpostorRed);
        }

        public static void akujoSetTarget()
        {
            if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || PlayerControl.LocalPlayer != Akujo.akujo) return;
            var untargetables = new List<PlayerControl>();
            if (Akujo.honmei != null) untargetables.Add(Akujo.honmei);
            if (Akujo.keeps != null) untargetables.AddRange(Akujo.keeps);
            Akujo.currentTarget = setTarget(untargetablePlayers: untargetables);
            if (Akujo.honmei == null || Akujo.keepsLeft > 0) setPlayerOutline(Akujo.currentTarget, Akujo.color);
        }

        static void evilTrackerSetTarget()
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer == EvilTracker.evilTracker)
            {
                EvilTracker.currentTarget = setTarget();
                setPlayerOutline(EvilTracker.currentTarget, Palette.ImpostorRed);
            }
        }

        static void noisemakerSetTarget()
        {
            if (Noisemaker.noisemaker == null || PlayerControl.LocalPlayer != Noisemaker.noisemaker) return;
            Noisemaker.currentTarget = setTarget();
            if (Noisemaker.target == null && Noisemaker.numSound > 0)
                setPlayerOutline(Noisemaker.currentTarget, Noisemaker.color);
        }

        static void pursuerSetTarget() {
            if (Pursuer.pursuer == null || Pursuer.pursuer != PlayerControl.LocalPlayer) return;
            Pursuer.target = setTarget();
            setPlayerOutline(Pursuer.target, Pursuer.color);
        }

        static void witchSetTarget() {
            if (Witch.witch == null || Witch.witch != PlayerControl.LocalPlayer) return;
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

        static void assassinSetTarget()
        {
            if (Assassin.assassin == null || Assassin.assassin != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables = new();
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
            if (Thief.thief == null || Thief.thief != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables = new();
            if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
            //if (Ninja.ninja != null && Ninja.stealthed && !Ninja.canBeTargeted) untargetables.Add(Ninja.ninja);
            //if (Sprinter.sprinter != null && Sprinter.sprinting) untargetables.Add(Sprinter.sprinter);
            Thief.currentTarget = setTarget(onlyCrewmates: false, untargetablePlayers: untargetables);
            setPlayerOutline(Thief.currentTarget, Thief.color);
        }

        static void bloodyUpdate() {
            if (!Bloody.active.Any()) return;
            foreach (KeyValuePair<byte, float> entry in new Dictionary<byte, float>(Bloody.active)) {
                PlayerControl player = Helpers.playerById(entry.Key);
                PlayerControl bloodyPlayer = Helpers.playerById(Bloody.bloodyKillerMap[player.PlayerId]);      

                Bloody.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
                if (entry.Value <= 0 || player.Data.IsDead) {
                    Bloody.bloodyKillerMap.Remove(player.PlayerId);
                    Bloody.active.Remove(entry.Key);
                    continue;  // Skip the creation of the next blood drop, if the killer is dead or the time is up
                }
                new Bloodytrail(player, bloodyPlayer);
            }
        }

        // Mini set adapted button cooldown for Vampire, Sheriff, Jackal, Sidekick, Warlock, Cleaner
        public static void miniCooldownUpdate() {
            if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini) {
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
                HudManagerStartPatch.serialKillerButton.MaxTimer = SerialKiller.suicideTimer * (Mini.isGrownUp() ? 2f : 1f);
                HudManagerStartPatch.schrodingersCatKillButton.MaxTimer = SchrodingersCat.killCooldown * multiplier;
            }
        }

        /*public static void trapperUpdate() {
            if (Trapper.trapper == null || PlayerControl.LocalPlayer != Trapper.trapper || Trapper.trapper.Data.IsDead) return;
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
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
                int numberOfTasks = playerTotal - playerCompleted;
                if (numberOfTasks == 0) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareTimer, Hazel.SendOption.Reliable, -1);
                    writer.Write(HideNSeek.taskPunish);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.shareTimer(HideNSeek.taskPunish);

                    Hunted.taskPunish = true;
                }
            }

            if (!HideNSeek.isHunter()) return;

            byte playerId = PlayerControl.LocalPlayer.PlayerId;
            foreach (Arrow arrow in Hunter.localArrows) arrow.arrow.SetActive(false);
            if (Hunter.arrowActive) {
                int arrowIndex = 0;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
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
            
            if (PlayerControl.LocalPlayer == __instance) {
                // Update player outlines
                setBasePlayerOutlines();

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
                // Tracker
                trackerSetTarget();
                // Vampire
                vampireSetTarget();
                //Trap.Update();
                // Eraser
                eraserSetTarget();
                // Jackal
                jackalSetTarget();
                // Sidekick
                sidekickSetTarget();
                // Impostor
                impostorSetTarget();
                // Warlock
                warlockSetTarget();
                // Check for deputy promotion on Sheriff disconnect
                deputyCheckPromotion();
                // Check for sidekick promotion on Jackal disconnect
                sidekickCheckPromotion();
                // SecurityGuard
                securityGuardSetTarget();
                // Arsonist
                arsonistSetTarget();
                // Medium
                mediumSetTarget();
                // Morphling and Camouflager
                morphlingAndCamouflagerUpdate();
                // Lawyer
                lawyerUpdate();
                // Kataomoi
                kataomoiSetTarget();
                // Pursuer
                pursuerSetTarget();
                // Bomber
                bomberASetTarget();
                bomberBSetTarget();
                // Bait
                baitUpdate();
                // Evil Tracker
                evilTrackerSetTarget();
                // Evil Hacker
                evilHackerSetTarget();
                // Undertaker
                UndertakerSetTarget();
                UndertakerUpdate();
                // Trapper
                trapperUpdate();
                // Moriarty
                moriartySetTarget();
                // Cupid
                cupidSetTarget();
                // Blackmailer
                blackmailerSetTarget();
                // Prophet
                prophetSetTarget();
                // Doomsayer
                doomsayerSetTarget();
                // Plague Doctor
                plagueDoctorSetTarget();
                plagueDoctorUpdate();
                // Fox
                foxSetTarget();
                // Jekyll and Hyde
                jekyllAndHydeSetTarget();
                // Akujo
                akujoSetTarget();
                // Archaeologist
                archaeologistSetTarget();
                // Noisemaker
                noisemakerSetTarget();
                // Schrodinger's Cat
                schrodingersCatSetTarget();
                // Witch                
                witchSetTarget();
                // Assassin
                assassinSetTarget();
                AssassinTrace.UpdateAll();
                // Thief
                thiefSetTarget();

                // -- MODIFIER--
                // Bloody
                bloodyUpdate();
                // mini (for the cooldowns)
                miniCooldownUpdate();
                //Bomb.update();
                // Bomber
                BombEffect.UpdateAll();
                // Yo-yo
                Silhouette.UpdateAll();

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

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.LocalPlayer.CmdReportDeadBody))]
    class BodyReportPatch
    {
        static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target)
        {
            if (Moriarty.brainwashed?.FindAll(x => x.PlayerId == __instance?.PlayerId).Count > 0)
            {
                return false;
            }
            return true;
        }

        static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]NetworkedPlayerInfo target)
        {
            // Medic or Detective report
            bool isMedicReport = Medic.medic != null && Medic.medic == PlayerControl.LocalPlayer && __instance.PlayerId == Medic.medic.PlayerId;
            bool isDetectiveReport = Detective.detective != null && Detective.detective == PlayerControl.LocalPlayer && __instance.PlayerId == Detective.detective.PlayerId;
            if (isMedicReport || isDetectiveReport)
            {
                DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == target?.PlayerId)?.FirstOrDefault();

                if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                    float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds);
                    string msg = "";

                    if (isMedicReport) {
                        msg = string.Format(ModTranslation.getString("medicReport"), Math.Round(timeSinceDeath / 1000));
                    } else if (isDetectiveReport) {
                        Detective.acTokenChallenge.Value.reported = true;
                        Detective.acTokenChallenge.Value.killerId = deadPlayer.killerIfExisting.PlayerId;
                        if (timeSinceDeath < Detective.reportNameDuration * 1000) {
                            msg = string.Format(ModTranslation.getString("detectiveReportName"), deadPlayer.killerIfExisting.Data.PlayerName);
                            Detective.acTokenCommon.Value = true;
                        } else if (timeSinceDeath < Detective.reportColorDuration * 1000) {
                            var typeOfColor = Helpers.isLighterColor(deadPlayer.killerIfExisting.Data.DefaultOutfit.ColorId) ? ModTranslation.getString("detectiveColorLight") : ModTranslation.getString("detectiveColorDark");
                            msg = string.Format(ModTranslation.getString("detectiveReportColor"), typeOfColor);
                        } else {
                            msg = ModTranslation.getString("detectiveReportNone");
                        }
                    }

                    MeetingOverlayHolder.RegisterOverlay(TORGUIContextEngine.API.VerticalHolder(GUIAlignment.Left,
                    new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.API.GetAttribute(AttributeAsset.OverlayTitle), new TranslateTextComponent(isDetectiveReport ? "detectiveReportInfo" : "medicReportInfo")),
                    new TORGUIText(GUIAlignment.Left, TORGUIContextEngine.API.GetAttribute(AttributeAsset.OverlayContent), new RawTextComponent($"{deadPlayer.player?.Data?.PlayerName}:\n" + msg)))
                    , MeetingOverlayHolder.IconsSprite[isMedicReport ? 3 : 1], isMedicReport ? Medic.color : Detective.color);

                    if (!string.IsNullOrWhiteSpace(msg))
                    {   
                        if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                        {
                            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg, false);

                            // Ghost Info
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
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

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.AssignRoleOnDeath))]
    public static class AssignRolePatch
    {
        public static bool Prefix([HarmonyArgument(0)] PlayerControl player, bool specialRolesAllowed)
        {
            if ((player == SchrodingersCat.schrodingersCat && !SchrodingersCat.hasTeam() && !SchrodingersCat.isExiled) || (player == Busker.busker && Busker.pseudocideFlag) || FreePlayGM.isFreePlayGM)
                return false;
            return true;
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
            DeadPlayer deadPlayer = new(target, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Kill, __instance);
            GameHistory.deadPlayers.Add(deadPlayer);

            // Reset killer to crewmate if resetToCrewmate
            if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            if (resetToDead) __instance.Data.IsDead = true;

            // Remove fake tasks when player dies
            if ((target.hasFakeTasks() || target == Lawyer.lawyer || target == Pursuer.pursuer || target == Thief.thief || (target == Shifter.shifter && Shifter.isNeutral) || Madmate.madmate.Any(x => x.PlayerId == target.PlayerId) || target == CreatedMadmate.createdMadmate || target == JekyllAndHyde.jekyllAndHyde || target == Fox.fox) && !FreePlayGM.isFreePlayGM)
                target.clearAllTasks();

            // First kill (set before lover suicide)
            if (TORMapOptions.firstKillName == "") TORMapOptions.firstKillName = target.Data.PlayerName;

            GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Kill, __instance.PlayerId, 1 << target.PlayerId) { RelatedTag = EventDetail.Kill});

            // Handle all suicides
            foreach (var p in target.GetAllRelatedPlayers())
            {
                if (p == null || p.Data.IsDead) continue;
                p.MurderPlayer(p, MurderResultFlags.Succeeded);
                if (p == Busker.busker && Busker.pseudocideFlag && PlayerControl.LocalPlayer == Busker.busker) Busker.dieBusker(true);
                bool isLoverSuicide = (p.isLover() && target == p.getPartner()) || (p.isCupidLover() && target == p.getCupidLover()) ||
                    (p == Akujo.akujo && target == Akujo.honmei) || (p == Akujo.honmei && target == Akujo.akujo);
                overrideDeathReasonAndKiller(p, isLoverSuicide ? DeadPlayer.CustomDeathReason.LoverSuicide : DeadPlayer.CustomDeathReason.Suicide);
            }

            // Neko-Kabocha kill murderer
            if (NekoKabocha.nekoKabocha != null && target == NekoKabocha.nekoKabocha && __instance != NekoKabocha.nekoKabocha)
            {
                if (!__instance.Data.IsDead)
                {
                    if ((__instance.Data.Role.IsImpostor && NekoKabocha.revengeImpostor)
                        || (Helpers.isNeutral(__instance) && NekoKabocha.revengeNeutral)
                        || (NekoKabocha.revengeCrew && !Helpers.isNeutral(__instance) && !__instance.Data.Role.IsImpostor))
                    {
                        if (PlayerControl.LocalPlayer == NekoKabocha.nekoKabocha) _ = new StaticAchievementToken("nekoKabocha.challenge");
                        NekoKabocha.nekoKabocha.MurderPlayer(__instance, MurderResultFlags.Succeeded);
                        GameHistory.overrideDeathReasonAndKiller(__instance, DeadPlayer.CustomDeathReason.Revenge, killer: NekoKabocha.nekoKabocha);
                    }
                }
            }

            // Sidekick promotion trigger on murder
            if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && target == Jackal.jackal && Jackal.jackal == PlayerControl.LocalPlayer) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes();
            }

            // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
            if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer();
            }

            Helpers.HandleRoleFlashOnDeath(target);

            // Medium add body
            if (Medium.deadBodies != null) {
                Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));
            }

            // Show flash on bait kill to the killer if enabled
            if (Bait.bait != null && target == Bait.bait)
            {
                Bait.reported = false;
                if (Bait.showKillFlash && __instance != Bait.bait && __instance == PlayerControl.LocalPlayer) {
                    Helpers.showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
                }
            }

            if (target.Data.Role.IsImpostor && AmongUsClient.Instance.AmHost)
                LastImpostor.promoteToLastImpostor();

            if (__instance == LastImpostor.lastImpostor && target != LastImpostor.lastImpostor) LastImpostor.killCounter++;

            // Ninja penalize
            if (Ninja.ninja != null && PlayerControl.LocalPlayer == Ninja.ninja && __instance == Ninja.ninja)
            {
                Ninja.penalized = Ninja.stealthed;
                float penalty = Ninja.penalized ? Ninja.killPenalty : 0f;
                PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + penalty);
            }

            if (SchrodingersCat.schrodingersCat != null && target == SchrodingersCat.schrodingersCat)
            {
                if (PlayerControl.LocalPlayer == target) _ = new StaticAchievementToken("schrodingersCat.common1");
                SchrodingersCat.schrodingersCat.clearAllTasks();
                if (!SchrodingersCat.hasTeam())
                {
                    if (__instance.Data.Role.IsImpostor) {
                        SchrodingersCat.setImpostorFlag();
                        if (SchrodingersCat.becomesImpostor) FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Impostor);
                    }
                    else if (__instance == Jackal.jackal || __instance == Sidekick.sidekick) {
                        SchrodingersCat.setJackalFlag();
                    }
                    else if (__instance == JekyllAndHyde.jekyllAndHyde) {
                        SchrodingersCat.setJekyllAndHydeFlag();
                    }
                    else if (__instance == Moriarty.moriarty) {
                        SchrodingersCat.setMoriartyFlag();
                    }
                    else {
                        if (!SchrodingersCat.justDieOnKilledByCrew)
                        SchrodingersCat.setCrewFlag();
                    }

                    target.ModRevive();
                    DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == target.PlayerId)
                        {
                            array[i].gameObject.active = false;
                        }
                    }
                    DeadPlayer deadPlayerEntry = deadPlayers.Where(x => x.player.PlayerId == target.PlayerId).FirstOrDefault();
                    if (deadPlayerEntry != null) deadPlayers.Remove(deadPlayerEntry);
                    GameStatistics.recordRoleHistory(target);
                    GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Revive, null, 1 << target.PlayerId) { RelatedTag = EventDetail.Revive });
                }
            }

            if (PlayerControl.LocalPlayer == SchrodingersCat.schrodingersCat &&  __instance == SchrodingersCat.schrodingersCat && SchrodingersCat.team == SchrodingersCat.Team.Impostor)
                SchrodingersCat.schrodingersCat.SetKillTimerUnchecked(SchrodingersCat.killCooldown);

            if (Sheriff.sheriff != null && PlayerControl.LocalPlayer == Sheriff.sheriff && __instance == Sheriff.sheriff)
            {
                Sheriff.acTokenChallenge.Value.isTriggeredFalse = false;

                if (Sheriff.acTokenChallenge.Value.cleared)
                {
                    foreach (var dp in deadPlayers)
                    {
                        if (dp.player == null || !Helpers.isEvil(dp.player)) continue;
                        if (dp.killerIfExisting != Sheriff.sheriff)
                        {
                            Sheriff.acTokenChallenge.Value.cleared = false;
                            break;
                        }
                    }

                    foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                    {
                        if (p == null || !Helpers.isEvil(p)) continue;
                        if (!p.Data.IsDead)
                        {
                            Sheriff.acTokenChallenge.Value.isTriggeredFalse = true;
                            break;
                        }
                    }
                }
            }

            if (Godfather.godfather != null && target == Godfather.godfather && Mafioso.mafioso != null && PlayerControl.LocalPlayer == Mafioso.mafioso && !PlayerControl.LocalPlayer.Data.IsDead)
                _ = new StaticAchievementToken("mafioso.another1");

            if (Morphling.morphling != null && PlayerControl.LocalPlayer == Morphling.morphling && Morphling.morphTimer > 0f)
            {
                if (target == Morphling.morphling)
                    _ = new StaticAchievementToken("morphling.another1");
                else
                    Morphling.acTokenChallenge.Value.kill = true;
            }

            if (Camouflager.camouflager != null && PlayerControl.LocalPlayer == Camouflager.camouflager && Camouflager.camouflageTimer > 0f)
            {
                Camouflager.acTokenChallenge.Value.kills++;
                Camouflager.acTokenChallenge.Value.cleared |= Camouflager.acTokenChallenge.Value.kills >= 3;
                if (target == Camouflager.camouflager)
                    _ = new StaticAchievementToken("camouflager.another1");
            }

            if (TaskMaster.taskMaster != null && PlayerControl.LocalPlayer == TaskMaster.taskMaster && target == TaskMaster.taskMaster)
            {
                var (taskComplete, total) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
                if (taskComplete == 0 && total > 0)
                    _ = new StaticAchievementToken("taskMaster.another1");
            }

            if (Teleporter.teleporter != null && PlayerControl.LocalPlayer == Teleporter.teleporter)
            {
                var teleportT = target.PlayerId == Teleporter.acTokenChallenge.Value.target1 ? Teleporter.acTokenChallenge.Value.target2 : Teleporter.acTokenChallenge.Value.target1;
                PlayerControl teleportTarget = null;
                if (teleportT != byte.MaxValue) teleportTarget = Helpers.playerById(teleportT);
                if (teleportTarget != null)
                {
                    if (target.PlayerId != teleportT && DateTime.UtcNow.Subtract(Teleporter.acTokenChallenge.Value.swapTime).TotalSeconds <= 4f)
                    {
                        if (Teleporter.acTokenChallenge.Value.target2 != byte.MaxValue && (teleportTarget == Snitch.snitch || teleportTarget == Mayor.mayor || teleportTarget == Guesser.niceGuesser || teleportTarget == FortuneTeller.fortuneTeller
                            || teleportTarget == Sheriff.sheriff))
                            Teleporter.acTokenChallenge.Value.cleared = true;
                    }
                }
            }

            if (Trickster.trickster != null && PlayerControl.LocalPlayer == Trickster.trickster && Trickster.lightsOutTimer > 0f)
            {
                Trickster.acTokenChallenge.Value.kills++;
                Trickster.acTokenChallenge.Value.cleared |= Trickster.acTokenChallenge.Value.kills >= 2;
            }

            // Serial Killer set suicide timer
            if (SerialKiller.serialKiller != null && PlayerControl.LocalPlayer == SerialKiller.serialKiller && __instance == SerialKiller.serialKiller && target != SerialKiller.serialKiller)
            {
                _ = new StaticAchievementToken("serialKiller.common1");
                SerialKiller.serialKiller.SetKillTimerUnchecked(SerialKiller.killCooldown);
                HudManagerStartPatch.serialKillerButton.Timer = SerialKiller.suicideTimer;
                SerialKiller.isCountDown = true;
            }

            if (EvilHacker.evilHacker != null && PlayerControl.LocalPlayer == EvilHacker.evilHacker && __instance == EvilHacker.evilHacker)
            {
                EvilHacker.acTokenChallenge.Value.cleared |= EvilHacker.acTokenChallenge.Value.admin;
                EvilHacker.acTokenChallenge.Value.admin = false;
            }

            if (JekyllAndHyde.jekyllAndHyde != null && __instance == JekyllAndHyde.jekyllAndHyde && target != JekyllAndHyde.jekyllAndHyde)
            {
                JekyllAndHyde.counter++;
                if (JekyllAndHyde.counter >= JekyllAndHyde.numberToWin) JekyllAndHyde.triggerWin = true;
                HudManagerStartPatch.jekyllAndHydeSuicideButton.Timer = JekyllAndHyde.suicideTimer;
            }
             
            // Trapper peforms kills
            if (Trapper.trapper != null && PlayerControl.LocalPlayer == Trapper.trapper && __instance == Trapper.trapper)
            {
                if (Trap.isTrapped(target) && !Trapper.isTrapKill)  // トラップにかかっている対象をキルした場合のボーナス
                {
                    Trapper.trapper.killTimer = Mathf.Max(1f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - Trapper.bonusTime);
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Mathf.Max(1f, Trapper.cooldown - Trapper.bonusTime);
                }
                else if (Trap.isTrapped(target) && Trapper.isTrapKill)  // トラップキルした場合のペナルティ
                {
                    PlayerControl.LocalPlayer.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Trapper.cooldown;
                    if (target.Data.Role.IsImpostor) _ = new StaticAchievementToken("trapper.another1");
                }
                else // トラップにかかっていない対象を通常キルした場合はペナルティーを受ける
                {
                    PlayerControl.LocalPlayer.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + Trapper.penaltyTime;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Trapper.cooldown + Trapper.penaltyTime;
                }
                if (!Trapper.isTrapKill)
                {
                    MessageWriter writer;
                    writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ClearTrap, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.clearTrap();
                }
                Trapper.isTrapKill = false;
            }

            // Evil Tracker see flash
            if (__instance.Data.Role.IsImpostor && __instance != EvilTracker.evilTracker && PlayerControl.LocalPlayer == EvilTracker.evilTracker && !PlayerControl.LocalPlayer.Data.IsDead && EvilTracker.canSeeDeathFlash)
            {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: ModTranslation.getString("evilTrackerInfo"));
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

            if (__instance.Data.Role.IsImpostor && Spy.spy != null && PlayerControl.LocalPlayer == Spy.spy)
            {
                if (target == Spy.spy)  _ = new StaticAchievementToken("spy.another1");
                else
                {
                    if (!Helpers.AnyNonTriggersBetween(PlayerControl.LocalPlayer.GetTruePosition(), target.GetTruePosition(), out var vec)
                        && vec.magnitude < ShipStatus.Instance.CalculateLightRadius(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId)) * 0.75f)
                        _ = new StaticAchievementToken("spy.challenge");
                }
            }

            if (Undertaker.undertaker != null && PlayerControl.LocalPlayer == Undertaker.undertaker && target == Undertaker.undertaker && Undertaker.DraggedBody != null)
            {
                _ = new StaticAchievementToken("undertaker.another1");
            }

            if (Lawyer.lawyer != null && PlayerControl.LocalPlayer == Lawyer.lawyer && target == Lawyer.lawyer && __instance == Lawyer.target)
                _ = new StaticAchievementToken("lawyer.another1");

            if (Blackmailer.blackmailer != null && PlayerControl.LocalPlayer == Blackmailer.blackmailer && __instance == Blackmailer.blackmailer)
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (PlayerControl.LocalPlayer == p) continue;
                    if (!Helpers.AnyNonTriggersBetween(target.GetTruePosition(), p.GetTruePosition(), out var vec)
                        && vec.magnitude < ShipStatus.Instance.CalculateLightRadius(GameData.Instance.GetPlayerById(p.PlayerId)) * 0.75f)
                    {
                        Blackmailer.acTokenChallenge.Value.witness.Add(p.PlayerId);
                    }
                }
                Blackmailer.acTokenChallenge.Value.cleared |= Blackmailer.blackmailed != null && Blackmailer.acTokenChallenge.Value.witness.Any(x => x == Blackmailer.blackmailed.PlayerId);
            }

            if (Assassin.assassin != null && PlayerControl.LocalPlayer == Assassin.assassin && __instance == Assassin.assassin)
            {
                bool clearFlag = false;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (PlayerControl.LocalPlayer == p) continue;
                    if (!Helpers.AnyNonTriggersBetween(target.GetTruePosition(), p.GetTruePosition(), out var vec)
                        && vec.magnitude < ShipStatus.Instance.CalculateLightRadius(GameData.Instance.GetPlayerById(p.PlayerId)) * 0.75f)
                    {
                        clearFlag = true;
                        break;
                    }
                }
                Assassin.acTokenChallenge.Value.cleared |= clearFlag && Assassin.acTokenChallenge.Value.markKill;
                Assassin.acTokenChallenge.Value.markKill = false;
            }

            // Mimic(Killer) morph into victim
            if (MimicK.mimicK != null && __instance == MimicK.mimicK && target != MimicK.mimicK)
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

                if (PlayerControl.LocalPlayer == __instance) MimicK.acTokenChallenge.Value++;
            }

            // Mimic morph and arrows
            if (MimicK.mimicK != null && target == MimicK.mimicK)
            {
                MimicK.mimicK.setDefaultLook();
                MimicK.victim = null;
            }

            if (MimicA.mimicA != null && target == MimicA.mimicA)
            {
                MimicA.mimicA.setDefaultLook();
                MimicA.isMorph = false;
            }

            if (__instance == PlayerControl.LocalPlayer && target == Shifter.shifter && Shifter.isNeutral && Shifter.pastShifters.Contains(PlayerControl.LocalPlayer.PlayerId))
                _ = new StaticAchievementToken("corruptedShifter.challenge");

            // Set the correct opacity to the Ninja and Sprinter
            if (Ninja.ninja != null && target == Ninja.ninja)
            {
                if (PlayerControl.LocalPlayer == Ninja.ninja && Ninja.stealthed)
                    _ = new StaticAchievementToken("ninja.another1");
                Ninja.stealthed = false;
                Ninja.setOpacity(Ninja.ninja, 1.0f);
            }

            if (PlayerControl.LocalPlayer == Ninja.ninja && __instance == Ninja.ninja)
            {
                if (Ninja.stealthed) Ninja.acTokenChallenge.Value++;
            }

            if (Sprinter.sprinter != null)
            {
                if (target == Sprinter.sprinter)
                {
                    Sprinter.sprinting = false;
                    Sprinter.setOpacity(Sprinter.sprinter, 1.0f);
                }
                else if (PlayerControl.LocalPlayer == Sprinter.sprinter && Sprinter.sprinting)
                {
                    if (!Helpers.AnyNonTriggersBetween(PlayerControl.LocalPlayer.GetTruePosition(), target.GetTruePosition(), out var vec) &&
                    vec.magnitude < ShipStatus.Instance.CalculateLightRadius(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId)) * 0.75f)
                        _ = new StaticAchievementToken("sprinter.challenge");
                }
            }

            // Mimic(Assistant) show flash
            if (MimicK.mimicK != null && MimicA.mimicA != null && __instance == MimicK.mimicK && PlayerControl.LocalPlayer == MimicA.mimicA && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: ModTranslation.getString("mimicAInfo"));
            }

            // Sherlock record log
            Sherlock.killLog.Add(Tuple.Create(__instance.PlayerId, Tuple.Create(target.PlayerId, target.transform.position + Vector3.zero)));

            // Set bountyHunter cooldown
            if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter && __instance == BountyHunter.bountyHunter) {
                if (BountyHunter.acTokenChallenge.Value.kills == 0)
                    BountyHunter.acTokenChallenge.Value.history = DateTime.UtcNow;
                BountyHunter.acTokenChallenge.Value.kills++;
                if (BountyHunter.acTokenChallenge.Value.kills >= 3) {
                    BountyHunter.acTokenChallenge.Value.cleared |= DateTime.UtcNow.Subtract(BountyHunter.acTokenChallenge.Value.history).TotalSeconds <= 30;
                    BountyHunter.acTokenChallenge.Value.kills = 0;
                }
                if (target == BountyHunter.bounty) {
                    _ = new StaticAchievementToken("bountyHunter.common1");
                    BountyHunter.bountyHunter.SetKillTimer(BountyHunter.bountyKillCooldown);
                    BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
                }
                else {
                    _ = new StaticAchievementToken("bountyHunter.another1");
                    BountyHunter.bountyHunter.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + BountyHunter.punishmentTime);
                }
            }

            // Mini Set Impostor Mini kill timer (Due to mini being a modifier, all "SetKillTimers" must have happened before this!)
            if (Mini.mini != null && __instance == Mini.mini && __instance == PlayerControl.LocalPlayer) {
                float multiplier = 1f;
                if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
                Mini.mini.SetKillTimer(__instance.killTimer * multiplier);
            }

            // Cleaner Button Sync
            if (Cleaner.cleaner != null && PlayerControl.LocalPlayer == Cleaner.cleaner && __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null)
                HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

            // Witch Button Sync
            if (Witch.triggerBothCooldowns && Witch.witch != null && PlayerControl.LocalPlayer == Witch.witch && __instance == Witch.witch && HudManagerStartPatch.witchSpellButton != null)
                HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

            // Warlock Button Sync
            if (Warlock.warlock != null && PlayerControl.LocalPlayer == Warlock.warlock && __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null) {
                if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer) {
                    HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
                }
            }
            // Assassin Button Sync
            if (Assassin.assassin != null && PlayerControl.LocalPlayer == Assassin.assassin && __instance == Assassin.assassin && HudManagerStartPatch.assassinButton != null)
                HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;

            // Bait
            /*if (Bait.bait.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) {
                float reportDelay = (float) rnd.Next((int)Bait.reportDelayMin, (int)Bait.reportDelayMax + 1);
                Bait.active.Add(deadPlayer, reportDelay);

                if (Bait.showKillFlash && __instance == PlayerControl.LocalPlayer) Helpers.showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
            }*/

            // Add Bloody Modifier
            if (Bloody.bloody.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Bloody, Hazel.SendOption.Reliable, -1);
                writer.Write(__instance.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.bloody(__instance.PlayerId, target.PlayerId);
            }

            // HideNSeek
            if (HideNSeek.isHideNSeekGM) {
                int visibleCounter = 0;
                Vector3 bottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft + new Vector3(-0.25f, -0.25f, 0);
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
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
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
    class RevivePatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            __instance.Data.IsDead = false;
            __instance.gameObject.layer = LayerMask.NameToLayer("Players");
            __instance.MyPhysics.ResetMoveState(true);
            __instance.clickKillCollider.enabled = true;
            __instance.cosmetics.SetPetSource(__instance);
            __instance.cosmetics.SetNameMask(true);
            if (__instance.AmOwner)
            {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(true);
                DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
                DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
                DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(false);
            }
            DeadPlayer deadPlayerEntry = deadPlayers.Where(x => x.player.PlayerId == __instance.PlayerId).FirstOrDefault();
            if (deadPlayerEntry != null) deadPlayers.Remove(deadPlayerEntry);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    static class PlayerControlSetCoolDownPatch {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)]float time) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;
            if (PlayerControl.LocalPlayer.GetKillCooldown() <= 0f) return false;
            float multiplier = 1f;
            float addition = 0f;
            if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter) addition = BountyHunter.punishmentTime;
            if (Ninja.ninja != null && PlayerControl.LocalPlayer == Ninja.ninja && Ninja.penalized) addition = Ninja.killPenalty;

            float max = Mathf.Max(PlayerControl.LocalPlayer.GetKillCooldown() * multiplier + addition, __instance.killTimer);
            __instance.SetKillTimerUnchecked(Mathf.Clamp(time, 0f, max), max);
            return false;
        }

        public static void SetKillTimerUnchecked(this PlayerControl player, float time, float max = float.NegativeInfinity)
        {
            if (max == float.NegativeInfinity) max = time;
            player.killTimer = time;
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
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
            if (TaskMaster.isTaskMaster(__instance.PlayerId) && __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId && TaskMaster.isTaskComplete && !FreePlayGM.isFreePlayGM)
                __instance.clearAllTasks();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class ExilePlayerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            // Collect dead player info
            DeadPlayer deadPlayer = new(__instance, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Exile, null);
            GameHistory.deadPlayers.Add(deadPlayer);


            // Remove fake tasks when player dies
            if ((__instance.hasFakeTasks() || __instance == Lawyer.lawyer || __instance == Pursuer.pursuer || __instance == Thief.thief || (__instance == Shifter.shifter && Shifter.isNeutral) || Madmate.madmate.Any(x => x.PlayerId == __instance.PlayerId) || __instance == CreatedMadmate.createdMadmate || __instance == JekyllAndHyde.jekyllAndHyde || __instance == Fox.fox) && !FreePlayGM.isFreePlayGM)
                __instance.clearAllTasks();

            // Neko-Kabocha revenge on exile
            if (NekoKabocha.nekoKabocha != null && __instance == NekoKabocha.nekoKabocha && NekoKabocha.meetingKiller == null && NekoKabocha.otherKiller == null && PlayerControl.LocalPlayer == NekoKabocha.nekoKabocha)
            {
                if (NekoKabocha.revengeExile)
                {
                    var candidates = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x != NekoKabocha.nekoKabocha && !x.Data.IsDead).ToList();
                    int targetID = rnd.Next(0, candidates.Count);
                    var target = candidates[targetID];
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NekoKabochaExile, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.nekoKabochaExile(target.PlayerId);
                }
            }

            if (__instance.Data.Role.IsImpostor && AmongUsClient.Instance.AmHost)
                LastImpostor.promoteToLastImpostor();

            // Handle all suicides
            foreach (var p in __instance.GetAllRelatedPlayers())
            {
                if (p == null || p.Data.IsDead) continue;
                p.Exiled();
                bool isLoverSuicide = (p.isLover() && __instance == p.getPartner()) || (p.isCupidLover() && __instance == p.getCupidLover()) ||
                    (p == Akujo.akujo && __instance == Akujo.honmei) || (p == Akujo.honmei && __instance == Akujo.akujo);
                RPCProcedure.updateMeeting(p.PlayerId);
                if (isLoverSuicide && p == NekoKabocha.nekoKabocha) NekoKabocha.otherKiller = p;
                overrideDeathReasonAndKiller(p, isLoverSuicide ? DeadPlayer.CustomDeathReason.LoverSuicide : DeadPlayer.CustomDeathReason.Suicide);
                if (MeetingHud.Instance)
                {
                    if (FastDestroyableSingleton<HudManager>.Instance != null && PlayerControl.LocalPlayer == p)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(p.Data, p.Data);
                        if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                    }
                    if ((GuesserGM.isGuesser(PlayerControl.LocalPlayer.PlayerId) || PlayerControl.LocalPlayer == Doomsayer.doomsayer) && !PlayerControl.LocalPlayer.Data.IsDead &&
                        (GuesserGM.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 || PlayerControl.LocalPlayer == Doomsayer.doomsayer))
                    {
                        MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == p.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                        if (MeetingHudPatch.guesserUI != null && MeetingHudPatch.guesserUIExitButton != null)
                        {
                            if (MeetingHudPatch.guesserCurrentTarget == p.PlayerId)
                                MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                        }
                    }
                }
            }

            // Assign the default ghost role to let the Schrodinger's Cat have the haunt button
            if (__instance == SchrodingersCat.schrodingersCat && !SchrodingersCat.hasTeam()) {
                SchrodingersCat.isExiled = true;
                if (AmongUsClient.Instance.AmHost) FastDestroyableSingleton<RoleManager>.Instance.AssignRoleOnDeath(__instance, false);
            }

            // Check Plague Doctor status
            if (PlagueDoctor.plagueDoctor != null && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead)) PlagueDoctor.checkWinStatus();

            // Sidekick promotion trigger on exile
            if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && __instance == Jackal.jackal && Jackal.jackal == PlayerControl.LocalPlayer) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.sidekickPromotes();
            }

            // Pursuer promotion trigger on exile & suicide (the host sends the call such that everyone recieves the update before a possible game End)
            if (Lawyer.lawyer != null && __instance == Lawyer.target) {
                PlayerControl lawyer = Lawyer.lawyer;
                // && !Lawyer.isProsecutor
                if (AmongUsClient.Instance.AmHost && ((Lawyer.target != Jester.jester) || Lawyer.targetWasGuessed)) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.lawyerPromotesToPursuer();
                }

                 //&& !Lawyer.isProsecutor
                /*if (!Lawyer.targetWasGuessed) {
                    if (Lawyer.lawyer != null) Lawyer.lawyer.Exiled();
                    if (Pursuer.pursuer != null) Pursuer.pursuer.Exiled();

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
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
            bool shouldInvert = Invert.invert.FindAll(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId).Count > 0 && Invert.meetings > 0;  // xor. if already invert, eventInvert will turn it off for 10s
            if (__instance.AmOwner &&
                AmongUsClient.Instance &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !PlayerControl.LocalPlayer.Data.IsDead && 
                shouldInvert && 
                GameData.Instance && 
                __instance.myPlayer.CanMove)  
                __instance.body.velocity *= -1;

            if (__instance.AmOwner &&
                AmongUsClient.Instance &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !PlayerControl.LocalPlayer.Data.IsDead &&
                GameData.Instance &&
                __instance.myPlayer.CanMove &&
                PlayerControl.LocalPlayer == Undertaker.undertaker &&
                Undertaker.DraggedBody != null)
            {
                __instance.body.velocity *= 1f + Undertaker.speedDecrease / 100f;
            }

            Kataomoi.fixedUpdate(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsFlashlightEnabled))]
    public static class IsFlashlightEnabledPatch {
        public static bool Prefix(ref bool __result) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return true;
            __result = false;
            if (!PlayerControl.LocalPlayer.Data.IsDead && Lighter.lighter != null && Lighter.lighter.PlayerId == PlayerControl.LocalPlayer.PlayerId) {
                __result = true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
    public static class AdjustLight {
        public static bool Prefix(PlayerControl __instance) {
            if (__instance == null || PlayerControl.LocalPlayer == null || Lighter.lighter == null) return true;

            bool hasFlashlight = !PlayerControl.LocalPlayer.Data.IsDead && Lighter.lighter.PlayerId == PlayerControl.LocalPlayer.PlayerId;
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
            if (AmongUsClient.Instance && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && GameStatistics.roleHistory != null) {
                GameStatistics.Event.GameStatistics.RecordEvent(new(GameStatistics.EventVariation.Disconnect, player.PlayerId, 0) { RelatedTag = EventDetail.Disconnect });
                if (PlayerControl.LocalPlayer == player) Props.clearProps();
            }
            if (RoleDraft.isEnabled && RoleDraft.isRunning) {
                if (RoleDraft.pickOrder != null && RoleDraft.pickOrder.Count > 0) {
                    RoleDraft.pickOrder.Remove(player.PlayerId);
                    if (player.PlayerId == RoleDraft.pickOrder[0]) {
                        RoleDraft.timer = 0;
                        RoleDraft.picked = true;
                    }
                }
            }
        }
        public static void Postfix(GameData __instance, PlayerControl player, DisconnectReasons reason)
        {
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                if (AmongUsClient.Instance.AmHost && player != null && player.Data?.Role?.IsImpostor == true) LastImpostor.promoteToLastImpostor();
            }
        }
    }
}
