using System.Linq;
using Hazel;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Undertaker : RoleBase<Undertaker>
    {
        public static Color color = Palette.ImpostorRed;

        public Undertaker()
        {
            RoleId = roleId = RoleId.Undertaker;
        }

        static public HelpSprite[] helpSprite = [new(getDragButtonSprite(), "undertakerDragHint"), new(getDropButtonSprite(), "undertakerDropHint")];

        public static DeadBody DraggedBody;
        public static DeadBody TargetBody;
        public static bool CanDropBody;

        public static float speedDecrease = -50f;
        public static bool disableVent = true;

        public static Sprite dragButtonSprite;
        public static Sprite dropButtonSprite;

        public static void RpcDropBody(Vector3 position)
        {
            if (allPlayers.Count == 0) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndertakerDropBody, SendOption.Reliable, -1);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DropBody(position);
        }

        public static void RpcDragBody(byte playerId)
        {
            if (allPlayers.Count == 0) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndertakerDragBody, SendOption.Reliable, -1);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            DragBody(playerId);
        }

        public static void DropBody(Vector3 position)
        {
            if (!DraggedBody) return;
            DraggedBody.transform.position = position;
            DraggedBody = null;
            TargetBody = null;
        }

        public override void FixedUpdate()
        {
            undertakerSetTarget();
            undertakerUpdate();
            undertakerCanDropTarget();
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
                    RpcDropBody(newBodyPos);
                return;
            }

            bodyComponent.transform.position = newBodyPos;

            if (!player.AmOwner) return;

            Helpers.SetDeadBodyOutline(bodyComponent, Color.green);
        }

        public static void DragBody(byte playerId)
        {
            if (allPlayers.Count == 0) return;
            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == playerId);
            if (body == null) return;
            DraggedBody = body;
        }

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

        public static void clearAndReload()
        {
            DraggedBody = null;
            TargetBody = null;

            speedDecrease = CustomOptionHolder.undertakerSpeedDecrease.getFloat();
            disableVent = CustomOptionHolder.undertakerDisableVent.getBool();
            players = [];
        }
    }
}
