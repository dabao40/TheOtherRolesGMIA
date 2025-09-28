using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Trickster : RoleBase<Trickster> {
        public static Color color = Palette.ImpostorRed;
        public static float placeBoxCooldown = 30f;
        public static float boxKillPenalty = 2.5f;
        public static float lightsOutCooldown = 30f;
        public static float lightsOutDuration = 10f;
        public static float lightsOutTimer = 0f;
        public static AchievementToken<(int kills, bool cleared)> acTokenChallenge;

        static public readonly HelpSprite[] HelpSprites = [new(getPlaceBoxButtonSprite(), "tricksterBoxHint"), new(getLightsOutButtonSprite(), "tricksterLightsOutHint")];

        public static bool isInTricksterVent
        {
            get
            {
                return PlayerControl.LocalPlayer.inVent && Vent.currentVent != null && Vent.currentVent?.name.StartsWith("JackInTheBoxVent_") == true;
            }
        }

        public static RemoteProcess<Vector2> PlaceBox = RemotePrimitiveProcess.OfVector2("PlaceJackInTheBox", (message, _) => new JackInTheBox(message));

        public static RemoteProcess LightsOut = new("LightsOut", (_) =>
        {
            lightsOutTimer = lightsOutDuration;
            // If the local player is impostor indicate lights out
            if (Helpers.hasImpVision(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId)))
            {
                new CustomMessage(ModTranslation.getString("tricksterLightsOutText"), lightsOutDuration);
            }
        });

        public Trickster()
        {
            RoleId = roleId = RoleId.Trickster;
        }

        private static Sprite placeBoxButtonSprite;
        private static Sprite lightOutButtonSprite;
        private static Sprite tricksterVentButtonSprite;

        public static Sprite getPlaceBoxButtonSprite() {
            if (placeBoxButtonSprite) return placeBoxButtonSprite;
            placeBoxButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlaceJackInTheBoxButton.png", 115f);
            return placeBoxButtonSprite;
        }

        public static Sprite getLightsOutButtonSprite() {
            if (lightOutButtonSprite) return lightOutButtonSprite;
            lightOutButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.LightsOutButton.png", 115f);
            return lightOutButtonSprite;
        }

        public static Sprite getTricksterVentButtonSprite() {
            if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
            tricksterVentButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TricksterVentButton.png", 115f);
            return tricksterVentButtonSprite;
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("trickster.challenge", (0, false), (val, _) => val.cleared || val.kills >= 2);
        }

        public override void ResetRole(bool isShifted)
        {
            if (!isShifted)
                JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
        }

        public override void OnKill(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer == player)
            {
                if (isInTricksterVent)
                    player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + boxKillPenalty);
                else player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }
        }

        public static void clearAndReload() {
            lightsOutTimer = 0f;
            placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.getFloat();
            boxKillPenalty = CustomOptionHolder.tricksterBoxKillPenalty.getFloat();
            lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.getFloat();
            lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.getFloat();
            acTokenChallenge = null;
            players = [];
        }
    }
}
