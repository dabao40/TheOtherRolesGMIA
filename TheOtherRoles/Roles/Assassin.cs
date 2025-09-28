using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    [TORRPCHolder]
    public class Assassin : RoleBase<Assassin> {
        public Assassin()
        {
            RoleId = roleId = RoleId.Assassin;
            currentTarget = assassinMarked = null;
            acTokenChallenge = null;
            clearArrows();
        }

        static public readonly HelpSprite[] HelpSprites = [new(getMarkButtonSprite(), "assassinMarkHint"), new(getKillButtonSprite(), "assassinKillHint")];

        public static Color color = Palette.ImpostorRed;

        public static RemoteProcess<(byte playerId, Vector2 pos)> PlaceTrace = new("PlaceAssassinTrace", (message, _) =>
        {
            PlayerControl player = Helpers.playerById(message.playerId);
            var assassin = getRole(player);
            if (player == null || assassin == null) return;
            new AssassinTrace(message.pos, player, traceTime);
            if (PlayerControl.LocalPlayer != player)
                assassin.assassinMarked = null;
        });

        public PlayerControl assassinMarked;
        public PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static float traceTime = 1f;
        public static bool knowsTargetLocation = false;

        private static Sprite markButtonSprite;
        private static Sprite killButtonSprite;
        public Arrow arrow = new(Color.black);

        public AchievementToken<(bool markKill, bool cleared)> acTokenChallenge = null;

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("assassin.challenge", (false, false), (val, _) => val.cleared);
        }

        public static Sprite getMarkButtonSprite() {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AssassinMarkButton.png", 115f);
            return markButtonSprite;
        }

        public static Sprite getKillButtonSprite() {
            if (killButtonSprite) return killButtonSprite;
            killButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.AssassinAssassinateButton.png", 115f);
            return killButtonSprite;
        }

        public void clearArrows()
        {
            if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.black);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }

        public override void FixedUpdate()
        {
            assassinSetTarget();
            assassinUpdate();
        }

        public void assassinSetTarget()
        {
            if (player != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables = [];
            if (!Spy.impostorsCanKillAnyone) untargetables.AddRange(Spy.allPlayers);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
            foreach (var jackal in Jackal.players)
                if (jackal.player != null && jackal.wasTeamRed) untargetables.Add(jackal.player);
            foreach (var sidekick in Sidekick.players)
                if (sidekick.player != null && sidekick.wasTeamRed) untargetables.Add(sidekick.player);
            currentTarget = setTarget(onlyCrewmates: Spy.allPlayers.Count == 0 || !Spy.impostorsCanKillAnyone, untargetablePlayers: untargetables);
            setPlayerOutline(currentTarget, color);
        }

        public override void OnKill(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer == player)
            {
                if (HudManagerStartPatch.assassinButton != null)
                    HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;

                bool clearFlag = false;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (PlayerControl.LocalPlayer == p) continue;
                    if (!Helpers.AnyNonTriggersBetween(target.GetTruePosition(), p.GetTruePosition(), out var vec)
                        && vec.magnitude < ShipStatus.Instance.CalculateLightRadius(GameData.Instance.GetPlayerById(p.PlayerId)) * 0.75f)
                    {
                        clearFlag = true;
                        break;
                    }
                }
                acTokenChallenge.Value.cleared |= clearFlag && acTokenChallenge.Value.markKill;
                acTokenChallenge.Value.markKill = false;
            }
        }

        public void assassinUpdate()
        {
            if (PlayerControl.LocalPlayer != player) return;
            if (arrow?.arrow != null)
            {
                if (!knowsTargetLocation)
                {
                    arrow.arrow.SetActive(false);
                    return;
                }
                if (assassinMarked != null && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    bool trackedOnMap = !assassinMarked.Data.IsDead;
                    Vector3 position = assassinMarked.transform.position;
                    if (!trackedOnMap)
                    { // Check for dead body
                        DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == assassinMarked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }
                    arrow.Update(position);
                    arrow.arrow.SetActive(trackedOnMap);
                }
                else
                    arrow.arrow.SetActive(false);
            }
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            assassinMarked = null;
        }

        public override void ResetRole(bool isShifted)
        {
            clearArrows();
            assassinMarked = null;
        }

        public static void clearAndReload() {
            cooldown = CustomOptionHolder.assassinCooldown.getFloat();
            knowsTargetLocation = CustomOptionHolder.assassinKnowsTargetLocation.getBool();
            traceTime = CustomOptionHolder.assassinTraceTime.getFloat();
            players = [];
        }
    }
}
