using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Hazel;
using InnerNet;
using TheOtherRoles.MetaContext;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static Il2CppMono.Security.X509.X520;

namespace TheOtherRoles.Modules
{
    public class TitleShower : MonoBehaviour
    {
        static TitleShower() => ClassInjector.RegisterTypeInIl2Cpp<TitleShower>();

        private TMPro.TextMeshPro text = null!;
        private Achievement achievement = null;
        private BoxCollider2D collider = null!;

        private PlayerControl player = null!;

        private TMPro.TextMeshPro OrigText => player.cosmetics.nameText;
        private bool AmOwner => player ? player.AmOwner : false;
        private byte PlayerId => player ? player.PlayerId : byte.MaxValue;

        public void Awake()
        {
            TryGetComponent(out player);

            text = Instantiate(OrigText, OrigText.transform.parent);
            text.transform.localPosition = new Vector3(0, player ? 0.245f : -1f, -0.01f);
            text.fontSize = player ? 1.7f : 1.2f;

            text.text = "";

            collider = Helpers.CreateObject<BoxCollider2D>("Button", text.transform, new(0f, 0f, -10f));
            collider.isTrigger = true;
            var button = collider.gameObject.SetUpButton(false);
            button.OnMouseOver.AddListener((Action)(() =>
            {
                if (achievement != null) TORGUIManager.Instance.SetHelpContext(button, achievement.GetOverlayContext(false, true, false, true));
                else if (AmOwner) TORGUIManager.Instance.SetHelpContext(button, TORGUIContextEngine.API.LocalizedText(GUIAlignment.Left,
                    new TextAttributes(TORGUIContextEngine.API.GetAttribute(AttributeParams.StandardBaredBoldLeft)) { FontSize = new(1.8f) }, "achievementUnselectedDetail"));
                else return;
                VanillaAsset.PlayHoverSE();
            }));
            button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));
            button.OnClick.AddListener((Action)(() => {
                if (AmOwner) HelpMenu.TryOpenHelpScreen(HelpMenu.HelpTab.Achievements);
            }));
        }

        public Achievement SetAchievement(string achievement)
        {
            if (Achievement.GetAchievement(achievement, out var ach))
            {
                text.text = ModTranslation.getString(ach.TranslationKey);
                text.ForceMeshUpdate();
                this.achievement = ach;
                collider.size = (Vector2)text.bounds.size + new Vector2(0.1f, 0.1f);
            }
            else if (AmOwner)
            {
                text.text = ModTranslation.getString("achievementUnselected");
                text.ForceMeshUpdate();
                collider.size = (Vector2)text.bounds.size + new Vector2(0.1f, 0.1f);
                text.color = Color.gray;
                this.achievement = null;
                this.time = -1f;
            }
            else
            {
                text.text = "";
                this.achievement = null;
                collider.size = Vector2.zero;
            }

            return this.achievement;
        }

        float time = 1f;
        public void Update()
        {
            if (this.achievement != null)
            {
                time -= Time.deltaTime;
                if (time < 0f)
                {
                    text.color = Color.Lerp(Color.white, Palette.PlayerColors[player?.Data.DefaultOutfit.ColorId ?? PlayerId], 0.25f);
                    time = 1f;
                }
            }

            if (player && ShipStatus.Instance)
            {
                Destroy(text.gameObject);
                HelpMenu.TryCloseHelpScreen();
                Destroy(this);
            }
        }
    }

    public class TitleShowPlayer : MonoBehaviour
    {
        static TitleShowPlayer() => ClassInjector.RegisterTypeInIl2Cpp<TitleShowPlayer>();

        public static void RequireShare()
        {
            static IEnumerator CoShareAchievement()
            {
                yield return new WaitForSeconds(0.5f);
                if (PlayerControl.LocalPlayer?.AmOwner == true)
                {
                    var ach = Achievement.MyTitle?.Id ?? "-";
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareAchievement, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(ach);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.shareAchievement(PlayerControl.LocalPlayer.PlayerId, ach);
                }
            }
            AmongUsClient.Instance.StartCoroutine(CoShareAchievement().WrapToIl2Cpp());
        }

        public void Start()
        {
            static IEnumerator CoWaitAndUpdate()
            {
                RequireShare();
                yield return new WaitForSeconds(0.5f);
            }
            StartCoroutine(CoWaitAndUpdate().WrapToIl2Cpp());
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    public class EnableTitleShowerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (AmongUsClient.Instance && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) return;
            __instance.gameObject.AddComponent<TitleShowPlayer>();
        }
    }
}
