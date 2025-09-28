using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Witch : RoleBase<Witch> {
        public static Color color = Palette.ImpostorRed;

        public Witch()
        {
            RoleId = roleId = RoleId.Witch;
            currentTarget = spellCastingTarget = null;
            currentCooldownAddition = 0f;
            acTokenChallenge = null;
            futureSpelled = [];
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables;
            if (spellCastingTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != spellCastingTarget.PlayerId).ToList(); // Don't switch the target from the the one you're currently casting a spell on
            else {
                untargetables = []; // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
                if (!canSpellAnyone) {
                    untargetables.AddRange(Spy.allPlayers);
                    foreach (var jackal in Jackal.players)
                        if (jackal.player != null && jackal.wasTeamRed) untargetables.Add(jackal.player);
                    foreach (var sidekick in Sidekick.players)
                        if (sidekick.player != null && sidekick.wasTeamRed) untargetables.Add(sidekick.player);
                }
            }
            currentTarget = setTarget(onlyCrewmates: !canSpellAnyone, untargetablePlayers: untargetables);
            setPlayerOutline(currentTarget, color);
        }

        public static RemoteProcess<(byte playerId, byte witchId)> SetFutureSpelled = new("SetFutureSpelled", (message, _) =>
        {
            PlayerControl player = Helpers.playerById(message.playerId);
            PlayerControl witch = Helpers.playerById(message.witchId);
            var witchRole = getRole(witch);
            if (witch == null || witchRole == null) return;
            witchRole.futureSpelled ??= new List<PlayerControl>();
            if (player != null)
            {
                witchRole.futureSpelled.Add(player);
            }
        });

        public override void OnKill(PlayerControl target)
        {
            if (triggerBothCooldowns && PlayerControl.LocalPlayer == player  && HudManagerStartPatch.witchSpellButton != null)
                HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            spellCastingTarget = null;
        }

        public List<PlayerControl> futureSpelled = [];
        public PlayerControl currentTarget;
        public PlayerControl spellCastingTarget;
        public static float cooldown = 30f;
        public static float spellCastingDuration = 2f;
        public static float cooldownAddition = 10f;
        public float currentCooldownAddition = 0f;
        public static bool canSpellAnyone = false;
        public static bool triggerBothCooldowns = true;
        public static bool witchVoteSavesTargets = true;

        public AchievementToken<int> acTokenChallenge = null;

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("witch.challenge", 0, (val, _) => val >= 3);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite spelledOverlaySprite;
        public static Sprite getSpelledOverlaySprite() {
            if (spelledOverlaySprite) return spelledOverlaySprite;
            spelledOverlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SpellButtonMeeting.png", 225f);
            return spelledOverlaySprite;
        }

        public static void clearAndReload() {
            cooldown = CustomOptionHolder.witchCooldown.getFloat();
            cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.getFloat();
            canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.getBool();
            spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.getFloat();
            triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.getBool();
            witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.getBool();
            players = [];
        }
    }
}
