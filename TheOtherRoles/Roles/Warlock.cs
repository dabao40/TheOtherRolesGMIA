using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Warlock: RoleBase<Warlock> {
        public static Color color = Palette.ImpostorRed;

        public PlayerControl currentTarget;
        public PlayerControl curseVictim;
        public PlayerControl curseVictimTarget;
        public AchievementToken<int> acTokenChallenge;

        public static float cooldown = 30f;
        public static float rootTime = 5f;

        private static Sprite curseButtonSprite;
        private static Sprite curseKillButtonSprite;

        public Warlock()
        {
            RoleId = roleId = RoleId.Warlock;
            acTokenChallenge = null;
            currentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            curseVictim = null;
            curseVictimTarget = null;
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("warlock.challenge", 0, (val, _) => val >= 3);
        }

        public static Sprite getCurseButtonSprite() {
            if (curseButtonSprite) return curseButtonSprite;
            curseButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseButton.png", 115f);
            return curseButtonSprite;
        }

        public static Sprite getCurseKillButtonSprite() {
            if (curseKillButtonSprite) return curseKillButtonSprite;
            curseKillButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurseKillButton.png", 115f);
            return curseKillButtonSprite;
        }

        public static void clearAndReload() {
            cooldown = CustomOptionHolder.warlockCooldown.getFloat();
            rootTime = CustomOptionHolder.warlockRootTime.getFloat();
            players = [];
        }

        public override void OnKill(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer == player && HudManagerStartPatch.warlockCurseButton != null)
                if (player.killTimer > HudManagerStartPatch.warlockCurseButton.Timer)                     HudManagerStartPatch.warlockCurseButton.Timer = player.killTimer;
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            if (curseVictim != null && (curseVictim.Data.Disconnected || curseVictim.Data.IsDead))                 // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                resetCurse();
            if (curseVictim == null) {
                currentTarget = setTarget();
                setPlayerOutline(currentTarget, color);
            }
            else {
                curseVictimTarget = setTarget(targetingPlayer: curseVictim);
                setPlayerOutline(curseVictimTarget, color);
            }
        }

        public void resetCurse() {
            HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
            HudManagerStartPatch.warlockCurseButton.Sprite = getCurseButtonSprite();
            HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            currentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
        }
    }
}
