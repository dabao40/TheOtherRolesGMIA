using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.MetaContext;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Vampire : RoleBase<Vampire> {
        public static Color color = Palette.ImpostorRed;

        public static readonly Image Illustration = new TORSpriteLoader("Assets/Vampire.png");

        public Vampire()
        {
            RoleId = roleId = RoleId.Vampire;
            acTokenChallenge = null;
            currentTarget = null;
            bitten = null;
            targetNearGarlic = false;
        }

        public static float delay = 10f;
        public static float cooldown = 30f;
        public static bool canKillNearGarlics = true;
        public static bool localPlacedGarlic = false;
        public static bool garlicsActive = true;

        public PlayerControl currentTarget;
        public PlayerControl bitten;
        public bool targetNearGarlic = false;

        public AchievementToken<(DateTime deathTime, bool cleared)> acTokenChallenge;

        public override void OnMeetingStart()
        {
            // Reset vampire bitten
            bitten = null;
            if (PlayerControl.LocalPlayer == player)
                acTokenChallenge.Value.cleared |= DateTime.UtcNow.Subtract(acTokenChallenge.Value.deathTime).TotalSeconds <= 3;
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("vampire.challenge", (DateTime.UtcNow, false), (val, _) => val.cleared);
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;

            PlayerControl target = null;
            if (Spy.exists)
            {
                if (Spy.impostorsCanKillAnyone) {
                    target = setTarget(false, true);
                }
                else
                {
                    var untargetables = new List<PlayerControl>();
                    untargetables.AddRange(Spy.allPlayers);
                    untargetables.AddRange(Jackal.players.Where(x => x.wasTeamRed).Select(x => x.player));
                    untargetables.AddRange(Sidekick.players.Where(x => x.wasTeamRed).Select(x => x.player));
                    target = setTarget(true, true, untargetables);
                }
            }
            else {
                var untargetables = new List<PlayerControl>();
                untargetables.AddRange(Jackal.players.Where(x => x.wasImpostor).Select(x => x.player));
                untargetables.AddRange(Sidekick.players.Where(x => x.wasImpostor).Select(x => x.player));
                target = setTarget(true, true, untargetables);
            }

            bool targetNearGarlic = false;
            if (target != null)
            {
                foreach (Garlic garlic in Garlic.garlics)
                {
                    if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.91f)
                        targetNearGarlic = true;
                }
            }
            this.targetNearGarlic = targetNearGarlic;
            currentTarget = target;
            setPlayerOutline(currentTarget, color);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.VampireButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite garlicButtonSprite;
        public static Sprite getGarlicButtonSprite() {
            if (garlicButtonSprite) return garlicButtonSprite;
            garlicButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.GarlicButton.png", 115f);
            return garlicButtonSprite;
        }

        public static void clearAndReload() {
            localPlacedGarlic = false;
            garlicsActive = CustomOptionHolder.vampireSpawnRate.getSelection() > 0;
            delay = CustomOptionHolder.vampireKillDelay.getFloat();
            cooldown = CustomOptionHolder.vampireCooldown.getFloat();
            canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.getBool();
            players = [];
        }
    }
}
