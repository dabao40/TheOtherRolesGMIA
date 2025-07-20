using System.Collections.Generic;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Eraser : RoleBase<Eraser> {
        public static Color color = Palette.ImpostorRed;

        public Eraser()
        {
            RoleId = roleId = RoleId.Eraser;
            currentTarget = null;
            acTokenChallenge = null;
        }

        public static List<byte> alreadyErased = [];

        public static List<PlayerControl> futureErased = [];
        public PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static bool canEraseAnyone = false;
        public static float cooldownIncrease = 10f;
        public AchievementToken<int> acTokenChallenge;

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;

            List<PlayerControl> untargetables = [.. Spy.allPlayers];
            foreach (var jackal in Jackal.players)                 if (jackal.player != null && jackal.wasTeamRed) untargetables.Add(jackal.player);
            foreach (var sidekick in Sidekick.players)                 if (sidekick.player != null && sidekick.wasTeamRed) untargetables.Add(sidekick.player);
            currentTarget = setTarget(onlyCrewmates: !canEraseAnyone, untargetablePlayers: canEraseAnyone ? [] : untargetables);
            setPlayerOutline(currentTarget, color);
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("eraser.challenge", 0, (val, _) => val >= 3);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EraserButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            futureErased = [];
            cooldown = CustomOptionHolder.eraserCooldown.getFloat();
            canEraseAnyone = CustomOptionHolder.eraserCanEraseAnyone.getBool();
            cooldownIncrease = CustomOptionHolder.eraserCooldownIncrease.getFloat();
            alreadyErased = [];
            players = [];
        }
    }
}
