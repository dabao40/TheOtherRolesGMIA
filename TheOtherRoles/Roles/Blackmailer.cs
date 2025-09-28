using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Blackmailer : RoleBase<Blackmailer>
    {
        public static PlayerControl blackmailer;
        public static Color color = Palette.ImpostorRed;
        public static Color blackmailedColor = Palette.White;

        public Blackmailer()
        {
            RoleId = roleId = RoleId.Blackmailer;
            currentTarget = null;
            blackmailed = null;
        }

        public static bool alreadyShook = false;
        public PlayerControl blackmailed;
        public PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static bool blockTargetVote;
        public static bool blockTargetAbility;

        public AchievementToken<(List<byte> witness, bool cleared)> acTokenChallenge;
        private static Sprite blackmailButtonSprite;
        private static Sprite overlaySprite;

        public static RemoteProcess<(byte playerId, byte blackmailerId)> Blackmail = new("BlackmailerBlackmail", (message, _) =>
        {
            PlayerControl target = Helpers.playerById(message.playerId);
            PlayerControl blackmailer = Helpers.playerById(message.blackmailerId);
            var blackmailerRole = getRole(blackmailer);
            if (blackmailerRole == null) return;
            blackmailerRole.blackmailed = target;
        });

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("blackmailer.challenge", (new List<byte>(), false), (val, _) => val.cleared);
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            currentTarget = setTarget();
            setPlayerOutline(currentTarget, blackmailedColor);
        }

        public override void OnKill(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer == player)
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (PlayerControl.LocalPlayer == p) continue;
                    if (!Helpers.AnyNonTriggersBetween(target.GetTruePosition(), p.GetTruePosition(), out var vec)
                        && vec.magnitude < ShipStatus.Instance.CalculateLightRadius(GameData.Instance.GetPlayerById(p.PlayerId)) * 0.75f)                         acTokenChallenge.Value.witness.Add(p.PlayerId);
                }
                acTokenChallenge.Value.cleared |= blackmailed != null && acTokenChallenge.Value.witness.Any(x => x == blackmailed.PlayerId);
            }
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            blackmailed = null;
            alreadyShook = false;
            if (PlayerControl.LocalPlayer == player) acTokenChallenge.Value.witness = [];
        }

        public static Sprite getBlackmailOverlaySprite()
        {
            if (overlaySprite) return overlaySprite;
            overlaySprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerOverlay.png", 100f);
            return overlaySprite;
        }

        public static Sprite getBlackmailButtonSprite()
        {
            if (blackmailButtonSprite) return blackmailButtonSprite;
            blackmailButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.BlackmailerBlackmailButton.png", 115f);
            return blackmailButtonSprite;
        }

        public static void clearAndReload()
        {
            alreadyShook = false;
            cooldown = CustomOptionHolder.blackmailerCooldown.getFloat();
            blockTargetVote = CustomOptionHolder.blackmailerBlockTargetVote.getBool();
            blockTargetAbility = CustomOptionHolder.blackmailerBlockTargetAbility.getBool();
            players = [];
        }
    }
}
