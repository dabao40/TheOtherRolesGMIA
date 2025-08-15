using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Cupid : RoleBase<Cupid>
    {
        public Cupid()
        {
            RoleId = roleId = RoleId.Cupid;
            shielded = null;
            currentTarget = null;
            shieldTarget = null;
            startTime = DateTime.UtcNow;
            lovers1 = null;
            lovers2 = null;
        }

        static public HelpSprite[] helpSprite = [new(getArrowSprite(), "cupidArrowHint"), new(Medic.getButtonSprite(), "cupidShieldHint")];

        public static Color color = new Color32(246, 152, 150, byte.MaxValue);

        public PlayerControl lovers1;
        public PlayerControl lovers2;
        public PlayerControl shielded;
        public PlayerControl currentTarget;
        public PlayerControl shieldTarget;
        public DateTime startTime = DateTime.UtcNow;
        public static bool isShieldOn = false;
        public int timeLeft { get { return (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds); } }
        public static float timeLimit;

        public override void ResetRole(bool isShifted)
        {
            if (PlayerControl.LocalPlayer == player && HudManagerStartPatch.cupidTimeRemainingText != null)
                HudManagerStartPatch.cupidTimeRemainingText.enabled = false;
        }

        public override void FixedUpdate()
        {
            if (player != PlayerControl.LocalPlayer) return;
            var untargetables = new List<PlayerControl>();
            if (lovers1 != null) untargetables.Add(lovers1);
            currentTarget = setTarget(untargetablePlayers: untargetables);
            if (isShieldOn) shieldTarget = setTarget();
            if (lovers1 == null || lovers2 == null) setPlayerOutline(currentTarget, color);
            if (shielded == null && isShieldOn) setPlayerOutline(shieldTarget, color);

            if (HudManagerStartPatch.cupidTimeRemainingText != null)
                HudManagerStartPatch.cupidTimeRemainingText.enabled = false;

            if ((lovers1 == null || lovers2 == null) && !player.Data.IsDead)
            {
                if (HudManagerStartPatch.cupidTimeRemainingText != null)
                {
                    HudManagerStartPatch.cupidTimeRemainingText.text = TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss");
                    HudManagerStartPatch.cupidTimeRemainingText.enabled = Helpers.ShowButtons;
                }
                if (timeLeft <= 0)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CupidSuicide, SendOption.Reliable, -1);
                    writer.Write(player.PlayerId);
                    writer.Write(false);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.cupidSuicide(player.PlayerId, false, false);
                }
            }
        }

        public static bool checkShieldActive(PlayerControl target)
        {
            return players.Any(x => x.shielded == target && x.player && !x.player.Data.IsDead);
        }

        public static void killCupid(PlayerControl player, PlayerControl killer = null)
        {
            // Cupidを道連れにする
            foreach (var cupid in players)
            {
                if (cupid.player == null || cupid.player != PlayerControl.LocalPlayer || cupid.player.Data.IsDead) continue;
                if (cupid.lovers1 == player || cupid.lovers2 == player)
                {
                    if (killer != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CupidSuicide, SendOption.Reliable, -1);
                        writer.Write(cupid.player.PlayerId);
                        writer.Write(false);
                        writer.Write(false);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.cupidSuicide(cupid.player.PlayerId, false, false);
                    }
                    else
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CupidSuicide, SendOption.Reliable, -1);
                        writer.Write(cupid.player.PlayerId);
                        writer.Write(false);
                        writer.Write(true);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.cupidSuicide(cupid.player.PlayerId, false, true);
                    }
                    GameHistory.overrideDeathReasonAndKiller(cupid.player, DeadPlayer.CustomDeathReason.Suicide);
                }
            }
        }

        public static void scapeGoat(PlayerControl target)
        {
            var cupids = players.FindAll(x => x.player && x.shielded == target && !x.player.Data.IsDead);
            cupids.ForEach(x =>
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CupidSuicide, SendOption.Reliable, -1);
                writer.Write(x.player.PlayerId);
                writer.Write(true);
                writer.Write(false);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.cupidSuicide(x.player.PlayerId, true, false);
            });
        }

        private static Sprite arrowSprite;
        public static Sprite getArrowSprite()
        {
            if (arrowSprite) return arrowSprite;
            arrowSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CupidButton.png", 115f);
            return arrowSprite;
        }

        public static void breakLovers(PlayerControl p1, PlayerControl p2)
        {
            if (p1.isLovers())
            {
                PlayerControl otherLover = p1.getPartner();
                if (otherLover != null && otherLover != p2 && !otherLover.Data.IsDead)
                {
                    Lovers.eraseCouple(p1);
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
                }
                else if (otherLover == p2)
                    Lovers.eraseCouple(p1);
            }
            else if (p1.isRole(RoleId.Akujo) && !Akujo.isHonmei(p2))
            {
                var akujo = Akujo.getRole(p1);
                var honmei = akujo.honmei;
                if (honmei != null)
                {
                    akujo.honmei = null;
                    // 本命を自殺させる
                    honmei.MurderPlayer(honmei, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(honmei, DeadPlayer.CustomDeathReason.LoveStolen);
                }
                akujo.cupidHonmei = p2;
            }
            else if (p1.isRole(RoleId.Akujo) && Akujo.isHonmei(p2))
            {
                var akujo = Akujo.getRole(p1);
                akujo.honmei = null;
                akujo.cupidHonmei = p2;
            }
            else if (Akujo.isKeep(p1) && !p2.isRole(RoleId.Akujo))
                Akujo.players.ForEach(x => x.keeps.Remove(p1));
        }

        public static bool isCupidLovers(PlayerControl player)
        {
            return players.Any(x => x.lovers1 == player || x.lovers2 == player);
        }

        public static void createLovers()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetCupidLovers, SendOption.Reliable, -1);
            writer.Write(local.lovers1.PlayerId);
            writer.Write(local.lovers2.PlayerId);
            writer.Write(local.player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.setCupidLovers(local.lovers1.PlayerId, local.lovers2.PlayerId, local.player.PlayerId);
        }

        public static void clearAndReload()
        {
            timeLimit = CustomOptionHolder.cupidTimeLimit.getFloat() + 10f;
            isShieldOn = CustomOptionHolder.cupidShield.getBool();
            players = [];
        }
    }
}
