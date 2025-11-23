using System.Linq;
using Hazel;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Undertaker : RoleBase<Undertaker>
    {
        public static Color color = Palette.ImpostorRed;

        public Undertaker()
        {
            RoleId = roleId = RoleId.Undertaker;
        }

        static public readonly HelpSprite[] HelpSprites = [new(getDragButtonSprite(), "undertakerDragHint"), new(getDropButtonSprite(), "undertakerDropHint")];

        public static DeadBody DraggedBody;
        public static DeadBody TargetBody;
        public static Vent ventTarget = null;
        public static bool CanDropBody;
        public static float throwCooldown;
        public static float speedDecrease = -50f;
        public static bool disableVent = true;

        public static Sprite dragButtonSprite;
        public static Sprite dropButtonSprite;
        public static Sprite throwButtonSprite;

        public static RemoteProcess<Vector3> DropBody = RemotePrimitiveProcess.OfVector3("UndertakerDropBody", (message, _) =>
        {
            if (!DraggedBody) return;
            DraggedBody.transform.position = message;
            DraggedBody = null;
            TargetBody = null;
        });

        public override void FixedUpdate()
        {
            undertakerSetTarget();
            undertakerUpdate();
            undertakerCanDropTarget();
            setVentTarget();
        }

        public override void OnDeath(PlayerControl killer = null)
        {
            if (PlayerControl.LocalPlayer == player && DraggedBody != null && killer != null)
                _ = new StaticAchievementToken("undertaker.another1");
        }

        void undertakerCanDropTarget()
        {
            if (player != PlayerControl.LocalPlayer) return;

            CanDropBody = false;

            if (DraggedBody == null) return;

            if (DraggedBody.enabled && Vector2.Distance(player.GetTruePosition(), DraggedBody.TruePosition) <= player.MaxReportDistance && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), DraggedBody.TruePosition, Constants.ShipAndObjectsMask, false))                 CanDropBody = true;
        }

        private void setVentTarget()
        {
            if (player != PlayerControl.LocalPlayer || MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

            Vent target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            for (int i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
            {
                Vent vent = MapUtilities.CachedShipStatus.AllVents[i];
                if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 9) continue; // cannot seal submergeds exit only vent!
                float distance = Vector2.Distance(vent.transform.position, truePosition);
                if (distance <= vent.UsableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = vent;
                }
            }
            ventTarget = target;
        }
        void undertakerSetTarget()
        {
            if (player != PlayerControl.LocalPlayer) return;
            if (TargetBody != null)
                Helpers.SetDeadBodyOutline(TargetBody, null);

            if (DraggedBody == null)
            {
                TargetBody = Helpers.setDeadTarget(2f / 3f);
                Helpers.SetDeadBodyOutline(TargetBody, color);
            }
        }

        void undertakerUpdate()
        {
            var bodyComponent = DraggedBody;

            if (player == null || bodyComponent == null) return;

            var undertakerPos = player.transform.position;
            var bodyLastPos = bodyComponent.transform.position;

            var direction = player.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;

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
                    player.GetTruePosition(),
                    newBodyPos,
                    Constants.ShipAndObjectsMask,
                    false
                ))
                newBodyPos = new Vector3(undertakerPos.x, undertakerPos.y, bodyLastPos.z);

            if (player.Data.IsDead) {
                if (player.AmOwner)
                    DropBody.Invoke(newBodyPos);
                return;
            }

            bodyComponent.transform.position = newBodyPos;

            if (!player.AmOwner) return;

            Helpers.SetDeadBodyOutline(bodyComponent, Color.green);
        }

        public static RemoteProcess<byte> DragBody = RemotePrimitiveProcess.OfByte("UndertakerDragBody", (message, _) =>
        {
            if (allPlayers.Count == 0) return;
            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == message);
            if (body == null) return;
            DraggedBody = body;
        });

        public static Sprite getDragButtonSprite()
        {
            if (dragButtonSprite) return dragButtonSprite;
            dragButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DragButton.png", 115f);
            return dragButtonSprite;
        }

        public static Sprite getDropButtonSprite()
        {
            if (dropButtonSprite) return dropButtonSprite;
            dropButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.DropButton.png", 115f);
            return dropButtonSprite;
        }
        public static Sprite getThrowButtonSprite()
        {
            if (throwButtonSprite) return throwButtonSprite;
            throwButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ThrowButton.png", 115f);
            return throwButtonSprite;
        }
        
        public static void clearAndReload()
        {
            DraggedBody = null;
            TargetBody = null;
            ventTarget = null;
            speedDecrease = CustomOptionHolder.undertakerSpeedDecrease.getFloat();
            disableVent = CustomOptionHolder.undertakerDisableVent.getBool();
            throwCooldown = CustomOptionHolder.undertakerThrowVent.getFloat();
            players = [];
        }
    }
}
