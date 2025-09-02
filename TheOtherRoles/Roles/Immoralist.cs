using System.Collections.Generic;
using HarmonyLib;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Immoralist : RoleBase<Immoralist>
    {
        public static Color color = Fox.color;

        public Immoralist()
        {
            RoleId = roleId = RoleId.Immoralist;
        }

        static public readonly HelpSprite[] HelpSprites = [new(getButtonSprite(), "immoralistSuicideHint")];

        public static List<Arrow> arrows = [];
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 1f;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.SuicideButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAllArrow()
        {
            if (arrows?.Count > 0)
            {
                foreach (var arrow in arrows)
                    if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            }
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (arrows?.Count > 0)
                {
                    foreach (var arrow in arrows)
                        if (arrow != null && arrow.arrow != null) Object.Destroy(arrow.arrow);
                }
                return;
            }

            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {
                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        Object.Destroy(arrow.arrow);
                    }
                }

                // Arrow一覧
                arrows = [];

                // 狐の位置を示すArrowを描画
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    if (p.isRole(RoleId.Fox))
                    {
                        arrow = new Arrow(Fox.color);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
            else
                arrows.Do(x => x.Update());
        }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer == player) arrowUpdate();
        }

        public override void ResetRole(bool isShifted)
        {
            clearAllArrow();
        }

        public static void clearAndReload()
        {
            foreach (Arrow arrow in arrows)
            {
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    Object.Destroy(arrow.arrow);
                }
            }
            arrows = [];
            players = [];
        }
    }
}
