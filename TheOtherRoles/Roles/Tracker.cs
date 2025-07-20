using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Tracker : RoleBase<Tracker> {
        public static Color color = new Color32(100, 58, 220, byte.MaxValue);
        public List<Arrow> localArrows = [];

        public Tracker()
        {
            RoleId = roleId = RoleId.Tracker;
            timeUntilUpdate = 0f;
            corpsesTrackingTimer = 0f;
            acTokenChallenge = null;
            resetTracked();
        }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer != player) return;
            // Handle player tracking
            if (arrow?.arrow != null)
                if (tracked != null && !player.Data.IsDead)
                {
                    timeUntilUpdate -= Time.fixedDeltaTime;

                    if (timeUntilUpdate <= 0f)
                    {
                        bool trackedOnMap = !tracked.Data.IsDead;
                        Vector3 position = tracked.transform.position;
                        if (!trackedOnMap)
                        { // Check for dead body
                            DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == tracked.PlayerId);
                            if (body != null)
                            {
                                trackedOnMap = true;
                                position = body.transform.position;
                            }
                        }

                        if (trackingMode is 1 or 2) Arrow.UpdateProximity(position);
                        if (trackingMode is 0 or 2)
                        {
                            arrow.Update(position);
                            arrow.arrow.SetActive(trackedOnMap);
                        }
                        timeUntilUpdate = updateIntervall;
                    }
                    else
                        if (trackingMode is 0 or 2)
                            arrow.Update();

                    if (tracked.inVent && !acTokenChallenge.Value.inVent)
                    {
                        acTokenChallenge.Value.inVent = true;
                        acTokenChallenge.Value.ventTime = GameStatistics.currentTime;
                    }
                    else if (!tracked.inVent && acTokenChallenge.Value.inVent)
                        acTokenChallenge.Value.inVent = false;
                }
                else if (player.Data.IsDead)
                {
                    if (DangerMeterParent != null) DangerMeterParent?.SetActive(false);
                    if (Meter?.gameObject != null) Meter?.gameObject?.SetActive(false);
                    if (arrow?.arrow != null) arrow.arrow.SetActive(false);
                }

            // Handle corpses tracking
            if (corpsesTrackingTimer >= 0f && !player.Data.IsDead)
            {
                bool arrowsCountChanged = localArrows.Count != deadBodyPositions.Count;
                int index = 0;

                if (arrowsCountChanged)
                {
                    foreach (Arrow arrow in localArrows) Object.Destroy(arrow.arrow);
                    localArrows = [];
                }
                foreach (Vector3 position in deadBodyPositions)
                {
                    if (arrowsCountChanged)
                    {
                        localArrows.Add(new Arrow(color));
                        localArrows[index].arrow.SetActive(true);
                    }
                    if (localArrows[index] != null) localArrows[index].Update(position);
                    index++;
                }
            }
            else if (localArrows.Count > 0)
            {
                foreach (Arrow arrow in localArrows) Object.Destroy(arrow.arrow);
                localArrows = [];
            }

            currentTarget = setTarget();
            if (!usedTracker) setPlayerOutline(currentTarget, color);
        }

        public static float updateIntervall = 5f;
        public static bool resetTargetAfterMeeting = false;
        public static bool canTrackCorpses = false;
        public static float corpsesTrackingCooldown = 30f;
        public static float corpsesTrackingDuration = 5f;
        public float corpsesTrackingTimer = 0f;
        public static int trackingMode = 0;
        public static List<Vector3> deadBodyPositions = [];

        public PlayerControl currentTarget;
        public PlayerControl tracked;
        public bool usedTracker = false;
        public float timeUntilUpdate = 0f;
        public Arrow arrow = new(Color.blue);

        public GameObject DangerMeterParent;
        public DangerMeter Meter;

        public AchievementToken<(bool inVent, float ventTime, bool cleared)> acTokenChallenge = null;

        public void unlockAch(float ventTime)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockTrackerAcChallenge, SendOption.Reliable, -1);
            writer.Write(ventTime);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.unlockTrackerAcChallenge(ventTime, player.PlayerId);
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("tracker.challenge", (false, 0f, false), (val, _) => val.cleared);
        }

        private static Sprite trackCorpsesButtonSprite;
        public static Sprite getTrackCorpsesButtonSprite()
        {
            if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
            trackCorpsesButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PathfindButton.png", 115f);
            return trackCorpsesButtonSprite;
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrackerButton.png", 115f);
            return buttonSprite;
        }

        public void resetTracked() {
            currentTarget = tracked = null;
            timeUntilUpdate = 0f;
            usedTracker = false;
            if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
            if (DangerMeterParent)
            {
                Meter.gameObject.Destroy();
                DangerMeterParent.Destroy();
            }
        }

        public override void ResetRole(bool isShifted)
        {
            resetTracked();
            if (localArrows != null)                 foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            localArrows = [];
        }

        public static void clearAndReload() {
            updateIntervall = CustomOptionHolder.trackerUpdateIntervall.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.getBool();
            deadBodyPositions = [];
            corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.getFloat();
            corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.getFloat();
            canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.getBool();
            trackingMode = CustomOptionHolder.trackerTrackingMethod.getSelection();
            players = [];
        }
    }
}
