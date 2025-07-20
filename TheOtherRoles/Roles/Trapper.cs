using System;
using System.Collections.Generic;
using Hazel;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Trapper : RoleBase<Trapper>
    {
        public Trapper()
        {
            RoleId = roleId = RoleId.Trapper;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            Trap.clearAllTraps();
            meetingFlag = false;
        }

        public override void OnMeetingStart()
        {
            meetingFlag = true;
        }

        public override void ResetRole(bool isShifted)
        {
            Trap.clearAllTraps();
        }

        public static Color color = Palette.ImpostorRed;

        public static float minDistance = 0f;
        public static float maxDistance;
        public static int numTrap;
        public static float extensionTime;
        public static float killTimer;
        public static float cooldown;
        public static float trapRange;
        public static float penaltyTime;
        public static float bonusTime;
        public static bool isTrapKill = false;
        public static bool meetingFlag;

        public static Sprite trapButtonSprite;
        public static DateTime placedTime;

        public static AchievementToken<int> acTokenChallenge;

        public override void FixedUpdate()
        {
            try
            {
                if (PlayerControl.LocalPlayer == player && Trap.traps.Count != 0 && !Trap.hasTrappedPlayer() && !meetingFlag)
                    foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        foreach (var trap in Trap.traps)
                        {
                            if (DateTime.UtcNow.Subtract(trap.Value.placedTime).TotalSeconds < extensionTime) continue;
                            if (trap.Value.isActive || p.Data.IsDead || p.inVent || meetingFlag) continue;
                            var p1 = p.transform.localPosition;
                            Dictionary<GameObject, byte> listActivate = [];
                            var p2 = trap.Value.trap.transform.localPosition;
                            var distance = Vector3.Distance(p1, p2);
                            if (distance < trapRange)
                            {
                                TMPro.TMP_Text text;
                                text = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                                text.transform.localScale = Vector3.one;
                                text.transform.localPosition = new Vector3(0, -1.8f, -69f);
                                text.enableWordWrapping = false;
                                text.text = string.Format(ModTranslation.getString("trapperGotTrapText"), p.Data.PlayerName);
                                text.gameObject.SetActive(true);
                                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
                                {
                                    if (p == 1f && text != null && text.gameObject != null)
                                        UnityEngine.Object.Destroy(text.gameObject);
                                })));
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ActivateTrap, SendOption.Reliable, -1);
                                writer.Write(trap.Key);
                                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                                writer.Write(p.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.activateTrap(trap.Key, PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
                                break;
                            }
                        }

                if (PlayerControl.LocalPlayer == player && Trap.hasTrappedPlayer() && !meetingFlag)
                    // トラップにかかっているプレイヤーを救出する
                    foreach (var trap in Trap.traps)
                    {
                        if (trap.Value.trap == null || !trap.Value.isActive) return;
                        Vector3 p1 = trap.Value.trap.transform.position;
                        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        {
                            if (player.PlayerId == trap.Value.target.PlayerId || player.Data.IsDead || player.inVent || player == this.player) continue;
                            Vector3 p2 = player.transform.position;
                            float distance = Vector3.Distance(p1, p2);
                            if (distance < 0.5)
                            {
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DisableTrap, SendOption.Reliable, -1);
                                writer.Write(trap.Key);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.disableTrap(trap.Key);
                            }
                        }

                    }
            }
            catch (NullReferenceException e)
            {
                TheOtherRolesPlugin.Logger.LogWarning(e.Message);
            }
        }

        public override void OnKill(PlayerControl target)
        {
            //　キルクールダウン設定
            if (PlayerControl.LocalPlayer == player)
            {
                if (Trap.isTrapped(target) && !isTrapKill)  // トラップにかかっている対象をキルした場合のボーナス
                {
                    player.killTimer = Mathf.Max(1f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - bonusTime);
                    HudManagerStartPatch.trapperSetTrapButton.Timer = Mathf.Max(1f, cooldown - bonusTime);
                }
                else if (Trap.isTrapped(target) && isTrapKill)  // トラップキルした場合のペナルティ
                {
                    PlayerControl.LocalPlayer.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = cooldown;
                    if (target.Data.Role.IsImpostor) _ = new StaticAchievementToken("trapper.another1");
                }
                else // トラップにかかっていない対象を通常キルした場合はペナルティーを受ける
                {
                    PlayerControl.LocalPlayer.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + penaltyTime;
                    HudManagerStartPatch.trapperSetTrapButton.Timer = cooldown + penaltyTime;
                }
                if (!isTrapKill)
                {
                    MessageWriter writer;
                    writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ClearTrap, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.clearTrap();
                }
                isTrapKill = false;
            }
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("trapper.challenge", 0, (val, _) => val >= 3);
        }

        public static Sprite getTrapButtonSprite()
        {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.TrapperButton.png", 115f);
            return trapButtonSprite;
        }

        public static void setTrap()
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceTrap, SendOption.Reliable);
            writer.WriteBytesAndSize(buff);
            writer.EndMessage();
            RPCProcedure.placeTrap(buff);
            placedTime = DateTime.UtcNow;
        }

        public static void clearAndReload()
        {
            numTrap = (int)CustomOptionHolder.trapperNumTrap.getFloat();
            extensionTime = CustomOptionHolder.trapperExtensionTime.getFloat();
            killTimer = CustomOptionHolder.trapperKillTimer.getFloat();
            cooldown = CustomOptionHolder.trapperCooldown.getFloat();
            trapRange = CustomOptionHolder.trapperTrapRange.getFloat();
            penaltyTime = CustomOptionHolder.trapperPenaltyTime.getFloat();
            bonusTime = CustomOptionHolder.trapperBonusTime.getFloat();
            maxDistance = CustomOptionHolder.trapperMaxDistance.getFloat();
            meetingFlag = false;
            Trap.clearAllTraps();
            acTokenChallenge = null;
            players = [];
        }
    }
}
