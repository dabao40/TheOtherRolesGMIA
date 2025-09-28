using System.Linq;
using TheOtherRoles.MetaContext;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Yoyo : RoleBase<Yoyo>
    {
        public static Color color = Palette.ImpostorRed;

        public Yoyo()
        {
            RoleId = roleId = RoleId.Yoyo;
            markedLocation = null;
        }

        static public readonly HelpSprite[] HelpSprites = [new(getMarkButtonSprite(), "yoyoMarkHint"), new(getBlinkButtonSprite(), "yoyoBlinkHint")];
        public static readonly Image Illustration = new TORSpriteLoader("Assets/Sprites/YoYo.png");

        public static float blinkDuration = 0;
        public static float markCooldown = 0;
        public static bool markStaysOverMeeting = false;
        public float SilhouetteVisibility => silhouetteVisibility == 0 && (PlayerControl.LocalPlayer == player || PlayerControl.LocalPlayer.Data.IsDead) ? 0.1f : silhouetteVisibility;
        public static float silhouetteVisibility = 0;

        public Vector3? markedLocation = null;

        private static Sprite markButtonSprite;

        public static RemoteProcess<(Vector3 pos, byte playerId)> MarkLocation = new("YoyoMarkLocation", (message, _) =>
        {
            PlayerControl player = Helpers.playerById(message.playerId);
            var yoyo = getRole(player);
            if (player == null || yoyo == null) return;
            yoyo.markLocation(message.pos);
            new Silhouette(message.pos, player, -1, false);
        });

        public static RemoteProcess<(bool isFirstJump, Vector3 pos, byte playerId)> Blink = new("YoyoBlink", (message, _) =>
        {
            PlayerControl player = Helpers.playerById(message.playerId);
            var yoyo = getRole(player);
            TheOtherRolesPlugin.Logger.LogMessage($"blink fistjumpo: {message.isFirstJump}");
            if (player == null || yoyo == null || yoyo.markedLocation == null) return;
            var markedPos = (Vector3)yoyo.markedLocation;
            player.NetTransform.SnapTo(markedPos);

            var markedSilhouette = Silhouette.silhouettes.FirstOrDefault(s => s.gameObject.transform.position.x == markedPos.x && s.gameObject.transform.position.y == markedPos.y);
            if (markedSilhouette != null)
                markedSilhouette.permanent = false;

            // Create Silhoutte At Start Position:
            if (message.isFirstJump)
            {
                yoyo.markLocation(message.pos);
                new Silhouette(message.pos, player, blinkDuration, true);
            }
            else
            {
                new Silhouette(message.pos, player, 5, true);
                yoyo.markedLocation = null;
            }
            if (Chameleon.chameleon.Any(x => x.PlayerId == message.playerId)) // Make the Yoyo visible if chameleon!
                Chameleon.lastMoved[message.playerId] = Time.time;
        });

        public static Sprite getMarkButtonSprite()
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.YoyoMarkButtonSprite.png", 115f);
            return markButtonSprite;
        }
        private static Sprite blinkButtonSprite;

        public static Sprite getBlinkButtonSprite()
        {
            if (blinkButtonSprite) return blinkButtonSprite;
            blinkButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.YoyoBlinkButtonSprite.png", 115f);
            return blinkButtonSprite;
        }

        public void markLocation(Vector3 position)
        {
            markedLocation = position;
        }

        public override void ResetRole(bool isShifted)
        {
            markedLocation = null;
            Silhouette.clearSilhouettes(player);
        }

        public static void clearAndReload()
        {
            blinkDuration = CustomOptionHolder.yoyoBlinkDuration.getFloat();
            markCooldown = CustomOptionHolder.yoyoMarkCooldown.getFloat();
            markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.getBool();
            silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.getSelection() / 10f;
            players = [];
        }
    }
}
