using AmongUs.GameOptions;
using Hazel;
using Il2CppSystem.Runtime.ExceptionServices;
using Rewired;
using System;
using System.Collections.Generic;
using TheOtherRoles.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Objects {
    public class CustomButton {
        public static List<CustomButton> buttons = new();
        public static KeyCode Action2Keycode = KeyCode.G;
        public static KeyCode Action3Keycode = KeyCode.H;

        public ActionButton actionButton;
        public Vector3 LocalScale = Vector3.one;
        public GameObject actionButtonGameObject;
        public SpriteRenderer actionButtonRenderer;
        public Material actionButtonMat;
        public TextMeshPro actionButtonLabelText;
        public Vector3 PositionOffset;
        public float MaxTimer = float.MaxValue;
        public float Timer = 0f;
        public float DeputyTimer = 0f;
        private Action OnClick;
        private Action InitialOnClick;
        private Action OnMeetingEnds;
        public Func<bool> HasButton;
        public Func<bool> CouldUse;
        private Action OnEffectEnds;
        public bool HasEffect;
        public bool effectCancellable = false;
        public bool isEffectActive = false;
        public bool showButtonText = false;
        public float EffectDuration;
        public Sprite Sprite;
        public HudManager hudManager;
        public bool mirror;
        public KeyCode? hotkey;
        public KeyCode? originalHotkey;
        public string buttonText = "";
        public string actionName = null;
        public bool shakeOnEnd = true;
        public bool isSuicide = false;
        public bool isHandcuffed = false;
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        public static class ButtonPositions {
            public static readonly Vector3 lowerRowRight = new(-2f, -0.06f, 0);  // Not usable for imps beacuse of new button positions!
            public static readonly Vector3 lowerRowCenter = new(-3f, -0.06f, 0);
            public static readonly Vector3 lowerRowLeft = new(-4f, -0.06f, 0);
            public static readonly Vector3 upperRowRight = new(0f, 1f, 0f);  // Not usable for imps beacuse of new button positions!
            public static readonly Vector3 upperRowCenter = new(-1f, 1f, 0f);  // Not usable for imps beacuse of new button positions!
            public static readonly Vector3 upperRowLeft = new(-2f, 1f, 0f);
            public static readonly Vector3 upperRowFarLeft = new(-3f, 1f, 0f);
            public static readonly Vector3 highRowRight = new(0f, 2.06f, 0f);
        }

        public enum ButtonLabelType
        {
            UseButton,
            AdminButton,
            KillButton,
            HauntButton
        }

        public CustomButton(Action OnClick, Func<bool> HasButton, Func<bool> CouldUse, Action OnMeetingEnds, Sprite Sprite, Vector3 PositionOffset, HudManager hudManager, KeyCode? hotkey, bool HasEffect, float EffectDuration, Action OnEffectEnds, bool mirror = false, string buttonText = "", ButtonLabelType abilityTexture = ButtonLabelType.KillButton, string actionName = null, bool shakeOnEnd = true,
            bool isSuicide = false)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.InitialOnClick = OnClick;
            this.HasButton = HasButton;
            this.CouldUse = CouldUse;
            this.PositionOffset = PositionOffset;
            this.OnMeetingEnds = OnMeetingEnds;
            this.HasEffect = HasEffect;
            this.EffectDuration = EffectDuration;
            this.OnEffectEnds = OnEffectEnds;
            this.Sprite = Sprite;
            this.mirror = mirror;
            this.hotkey = hotkey;
            this.buttonText = buttonText;
            this.actionName = actionName;
            this.shakeOnEnd = shakeOnEnd;
            this.isSuicide = isSuicide;
            originalHotkey = hotkey;
            Timer = 16.2f;
            buttons.Add(this);
            actionButton = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
            actionButtonGameObject = actionButton.gameObject;
            actionButtonRenderer = actionButton.graphic;
            actionButtonMat = actionButtonRenderer.material;
            setLabelType(abilityTexture);
            actionButtonLabelText = actionButton.buttonLabelText;
            PassiveButton button = actionButton.GetComponent<PassiveButton>();
            showButtonText = actionButtonRenderer.sprite == Sprite || buttonText != "";
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)onClickEvent);
            setKeyBind();
            setActive(false);
        }

        public CustomButton(Action OnClick, Func<bool> HasButton, Func<bool> CouldUse, Action OnMeetingEnds, Sprite Sprite, Vector3 PositionOffset, HudManager hudManager, KeyCode? hotkey, bool mirror = false, string buttonText = "", ButtonLabelType abilityTexture = ButtonLabelType.KillButton, string actionName = null, bool shakeOnEnd = true, bool isSuicide = false)
        : this(OnClick, HasButton, CouldUse, OnMeetingEnds, Sprite, PositionOffset, hudManager, hotkey, false, 0f, () => {}, mirror, buttonText, abilityTexture, actionName, shakeOnEnd, isSuicide) { }

        public void onClickEvent()
        {
            if ((this.Timer < 0f && HasButton() && CouldUse()) || (this.HasEffect && this.isEffectActive && this.effectCancellable && CouldUse()))
            {
                actionButtonRenderer.color = new Color(1f, 1f, 1f, 0.3f);
                this.OnClick();

                // Deputy skip onClickEvent if handcuffed
                if (Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0f) return;

                if (this.HasEffect && !this.isEffectActive) {
                    this.DeputyTimer = this.EffectDuration;
                    this.Timer = this.EffectDuration;
                    actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                    this.isEffectActive = true;
                }

                if (Sherlock.sherlock != null && !Sherlock.sherlock.Data.IsDead && PlayerControl.LocalPlayer.Data.Role.IsImpostor
                    && actionButton != HudManagerStartPatch.garlicButton.actionButton) {
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                    MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SherlockReceiveDetect, Hazel.SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.sherlockReceiveDetect(buff);
                }
            }
        }

        public static void HudUpdate()
        {
            buttons.RemoveAll(item => item.actionButton == null);
        
            for (int i = 0; i < buttons.Count; i++)
            {
                try
                {
                    buttons[i].Update();
                }
                catch (NullReferenceException)
                {
                    System.Console.WriteLine("[WARNING] NullReferenceException from HudUpdate().HasButton(), if theres only one warning its fine");
                }
            }
        }

        public static void MeetingEndedUpdate() {
            buttons.RemoveAll(item => item.actionButton == null);
            for (int i = 0; i < buttons.Count; i++)
            {
                try
                {
                    buttons[i].OnMeetingEnds();
                    buttons[i].Update();
                }
                catch (NullReferenceException)
                {
                    System.Console.WriteLine("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
                }
            }
        }

        public static void ResetAllCooldowns() {
            for (int i = 0; i < buttons.Count; i++)
            {
                try
                {
                    buttons[i].Timer = buttons[i].MaxTimer;
                    buttons[i].DeputyTimer = buttons[i].MaxTimer;
                    buttons[i].Update();
                }
                catch (NullReferenceException)
                {
                    System.Console.WriteLine("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
                }
            }
        }

        // Reload the rebound hotkeys from the among us settings.
        public static void ReloadHotkeys()
        {
            foreach (var button in buttons)
            {
                // Q button is used only for killing! This rebinds every button that would use Q to use the currently set killing button in among us.
                if (button.originalHotkey == KeyCode.Q)
                {
                    Player player = Rewired.ReInput.players.GetPlayer(0);
                    string keycode = player.controllers.maps.GetFirstButtonMapWithAction(8, true).elementIdentifierName;
                    button.hotkey = (KeyCode)Enum.Parse(typeof(KeyCode), keycode);
                }
                // F is the default ability button. All buttons that would use F now use the ability button.
                if (button.originalHotkey == KeyCode.F)
                {
                    Player player = Rewired.ReInput.players.GetPlayer(0);
                    string keycode = player.controllers.maps.GetFirstButtonMapWithAction(49, true).elementIdentifierName;
                    button.hotkey = (KeyCode)Enum.Parse(typeof(KeyCode), keycode);
                }

                if (button.originalHotkey == KeyCode.G)
                {
                    button.hotkey = Action2Keycode;
                }
                if (button.originalHotkey == KeyCode.H)
                {
                    button.hotkey = Action3Keycode;
                }
            }

        }

        public void setActive(bool isActive) {
            if (isActive) {
                actionButtonGameObject.SetActive(true);
                actionButtonRenderer.enabled = true;
            } else {
                actionButtonGameObject.SetActive(false);
                actionButtonRenderer.enabled = false;
            }
        }

        public void Update()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var moveable = localPlayer.moveable;
            
            if (localPlayer.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton()) {
                setActive(false);
                return;
            }
            setActive(hudManager.UseButton.isActiveAndEnabled || hudManager.PetButton.isActiveAndEnabled);

            if (DeputyTimer >= 0) { // This had to be reordered, so that the handcuffs do not stop the underlying timers from running
                if (HasEffect && isEffectActive)
                    DeputyTimer -= Time.deltaTime;
                else if (!localPlayer.inVent && moveable)
                    DeputyTimer -= Time.deltaTime;
            }

            if (DeputyTimer <= 0 && HasEffect && isEffectActive && !isSuicide) { // Here we have to specify that the Serial Killer button will not be affected
                isEffectActive = false;
                actionButton.cooldownTimerText.color = Palette.EnabledColor;
                OnEffectEnds();
            }

            if (isHandcuffed) {
                setActive(false);
                return;
            }

            actionButtonRenderer.sprite = Sprite;
            if (showButtonText && buttonText != ""){
                actionButton.OverrideText(buttonText);
            }
            actionButtonLabelText.enabled = showButtonText; // Only show the text if it's a kill button
            if (hudManager.UseButton != null) {
                Vector3 pos = hudManager.UseButton.transform.localPosition;
                if (mirror) {
                    float aspect = Camera.main.aspect;
                    float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                    float xpos = 0.05f - safeOrthographicSize * aspect * 1.70f;
                    pos = new Vector3(xpos, pos.y, pos.z);
                }
                actionButton.transform.localPosition = pos + PositionOffset;
            }
            if (CouldUse()) {
                actionButtonRenderer.color = actionButtonLabelText.color = Palette.EnabledColor;
                actionButtonMat.SetFloat(Desat, 0f);
            } else {
                actionButtonRenderer.color = actionButtonLabelText.color = Palette.DisabledClear;
                actionButtonMat.SetFloat(Desat, 1f);
            }
        
            if (Timer >= 0 && !RoleDraft.isRunning) {
                if (HasEffect && isEffectActive) {
                    Timer -= Time.deltaTime;
                    if (Timer <= 3f && Timer > 0f && shakeOnEnd)
                        actionButton.graphic.transform.localPosition = actionButton.transform.localPosition + (Vector3)UnityEngine.Random.insideUnitCircle * 0.05f;
                }
                else if (!localPlayer.inVent && moveable)
                    Timer -= Time.deltaTime;
            }
            
            if (Timer <= 0 && HasEffect && isEffectActive) {
                isEffectActive = false;
                actionButton.cooldownTimerText.color = Palette.EnabledColor;
                OnEffectEnds();
            }
        
            actionButton.SetCoolDown(Timer, (HasEffect && isEffectActive) ? EffectDuration : MaxTimer);

            // Trigger OnClickEvent if the hotkey is being pressed down
            if (hotkey.HasValue && Input.GetKeyDown(hotkey.Value)) onClickEvent();

            // Deputy disable the button and display Handcuffs instead...
            if (Deputy.handcuffedPlayers.Contains(localPlayer.PlayerId)) {
                OnClick = () => {
                    Deputy.setHandcuffedKnows();
                };
            } else // Reset.
            {
                OnClick = InitialOnClick;
            }
        }

        internal GameObject UsesIcon = null!;
        public TMPro.TextMeshPro ShowUsesIcon(int iconVariation)
        {
            if (UsesIcon) GameObject.Destroy(UsesIcon);
            UsesIcon = ButtonEffect.ShowUsesIcon(actionButton, iconVariation, out var text);
            return text;
        }

        public void setLabelType(ButtonLabelType labelType)
        {
            Material mat = null;
            switch (labelType)
            {
                case ButtonLabelType.UseButton:
                    mat = hudManager.UseButton.fastUseSettings[ImageNames.UseButton].FontMaterial; break;
                case ButtonLabelType.AdminButton:
                    mat = hudManager.UseButton.fastUseSettings[ImageNames.PolusAdminButton].FontMaterial; break;
                case ButtonLabelType.KillButton:
                    mat = RoleManager.Instance.GetRole(RoleTypes.Shapeshifter).Ability.FontMaterial; break;
                case ButtonLabelType.HauntButton:
                    mat = RoleManager.Instance.GetRole(RoleTypes.Engineer).Ability.FontMaterial; break;
            }
            if (mat != null) actionButton.buttonLabelText.SetSharedMaterial(mat);
        }

        public void setKeyBind()
        {
            if (hotkey is not null and not KeyCode.None)
            {
                actionButtonGameObject.ForEachChild((Il2CppSystem.Action<GameObject>)((c) => { if (c.name.Equals("HotKeyGuide")) GameObject.Destroy(c); }));
                ButtonEffect.SetKeyGuide(actionButtonGameObject, (KeyCode)hotkey, action: actionName);
            }
        }
    }
}
