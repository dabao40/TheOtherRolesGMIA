using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static TheOtherRoles.MetaContext.GUIButtonGroupInstance;

namespace TheOtherRoles.MetaContext;

/// <summary>
/// 協調的に選択状態を切り替えるSelectable
/// </summary>
file class GUIHarmoniousSelectable : IGUISelectable
{
    private GUIButtonInstance singleSelectable;
    private GUIButtonGroupInstance myGroup;
    public GUIHarmoniousSelectable(GUIButtonInstance singleSelectable, GUIButtonGroupInstance myGroup)
    {
        this.singleSelectable = singleSelectable;
        this.myGroup = myGroup;
    }

    bool IGUISelectable.IsSelected => singleSelectable.IsSelected;

    void IGUISelectable.Select()
    {
        myGroup.Select(singleSelectable);
    }

    void IGUISelectable.Unselect()
    {
        singleSelectable.Unselect();
    }
}

/// <summary>
/// 実際にボタンをとりまとめるグループ
/// </summary>
file class GUIButtonGroupInstance
{
    internal class GUIButtonInstance(Action unselect, Action select) : IGUISelectable
    {
        public void Unselect()
        {
            IsSelected = false;
            unselect.Invoke();
        }
        public void Select()
        {
            IsSelected = true;
            select.Invoke();
        }
        public bool IsSelected { get; set; } = false;
    }
    static public GUIButtonGroupInstance CurrentInstance { get; private set; }

    private List<GUIButtonInstance> buttons = [];

    public GUIButtonGroupInstance(GUIButtonGroupProperty groupProp)
    {
        GroupProp = groupProp;
    }

    public IReadOnlyList<GUIButtonInstance> Buttons => buttons;

    public GUIButtonGroupProperty GroupProp { get; private init; }

    public void AddButton(GUIButtonInstance button) => buttons.Add(button);
    public void UnselectAll() => buttons.Do(b => b.Unselect());
    public void Select(GUIButtonInstance button)
    {
        UnselectAll();
        button.Select();
    }

    static public void Refresh()
    {
        CurrentInstance = null;
    }

    static public void Call(GUIButtonGroupProperty groupProp)
    {
        CurrentInstance = new(groupProp);
    }
}

internal record GUIButtonGroupProperty(bool AllowMultipleChoice)
{
    static public GUIButtonGroupProperty Default = new(false);
}


/// <summary>
/// ボタンをグループ化するウィジェットです。
/// </summary>
internal class GUIButtonGroup : GUIContext
{
    private GUIContext innerContext;
    internal GUIButtonGroup(GUIContext inner)
    {
        innerContext = inner;
    }
    public GUIButtonGroupProperty GroupProperty { get; private set; } = GUIButtonGroupProperty.Default;
    internal override GUIAlignment Alignment => innerContext.Alignment;
    internal override GameObject Instantiate(Size size, out Size actualSize)
    {
        try
        {
            Call(GroupProperty);
            return innerContext.Instantiate(size, out actualSize);
        }
        finally
        {
            Refresh();
        }
    }
    internal override GameObject Instantiate(Anchor anchor, Size size, out Size actualSize)
    {
        try
        {
            Call(GroupProperty);
            return innerContext.Instantiate(anchor, size, out actualSize);
        }
        finally
        {
            Refresh();
        }
    }
}

/// <summary>
/// モダンデザインのボタンです。グループ化できます。
/// </summary>
internal class GUIModernButton : TORGUIText
{
    static private MultiImage modernButtonSprite = DividedExpandableSpriteLoader.FromResource("TheOtherRoles.Resources.GUIButton.png", 150f, 12, 12, 3, 2);
    static private MultiImage modernCheckSprite = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.GUICheckmark.png", 150f, 2, 1);
    static internal Sprite ModernCheckBackSprite => modernCheckSprite.GetSprite(0);
    static internal Sprite ModernCheckSprite => modernCheckSprite.GetSprite(1);
    public GUIClickableAction OnClick { get; init; }
    public GUIClickableAction OnRightClick { get; init; }
    public GUIClickableAction OnMouseOver { get; init; }
    public GUIClickableAction OnMouseOut { get; init; }

    public string RawText { init { Text = new RawTextComponent(value); } }
    public string TranslationKey { init { Text = new TranslateTextComponent(value); } }
    public bool AsMaskedButton { get; init; }
    public float TextMargin { get; init; } = 0.05f;

    public bool SelectedDefault { get; init; } = false; //デフォルトで選択状態の場合はtrue
    public bool WithCheckMark { get; init; } = false;
    public bool EmphasizeOnSelected { get; init; } = true;
    public bool BlockSelectingOnClicked { get; init; } = false;

    override protected bool AllowGenerateCollider => false;

    public GUIModernButton(GUIAlignment alignment, TextAttributes attribute, TextComponent text) : base(alignment, attribute, text)
    {
        AsMaskedButton = attribute.Font.FontMaterial != null;
    }

    static private Color piledModernColor = new(41 * 0.5f, 235 * 0.5f, 198 * 0.5f);
    static private Color selectedModernColor = new Color32(16, 16, 16, 255);

    internal override GameObject Instantiate(Size size, out Size actualSize)
    {
        var inner = base.Instantiate(size, out actualSize)!;

        var margin = TextMargin;

        var button = Helpers.CreateObject<SpriteRenderer>("Button", null, UnityEngine.Vector3.zero, LayerMask.NameToLayer("UI"));
        button.sprite = modernButtonSprite.GetSprite((SelectedDefault && EmphasizeOnSelected) ? 3 : 0);
        button.drawMode = SpriteDrawMode.Sliced;
        button.tileMode = SpriteTileMode.Continuous;
        button.size = actualSize.ToUnityVector() + new Vector2(margin * 0.84f, margin * 0.84f);
        if (AsMaskedButton) button.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        inner.transform.SetParent(button.transform);
        inner.transform.localPosition += new Vector3(0, 0, -0.05f);

        var collider = button.gameObject.AddComponent<BoxCollider2D>();
        collider.size = actualSize.ToUnityVector() + new Vector2(margin * 0.6f, margin * 0.6f);
        collider.isTrigger = true;

        var passiveButton = button.gameObject.SetUpButton(true);

        var innerText = inner.GetComponent<TextMeshPro>();
        innerText.outlineColor = UnityEngine.Color.clear;

        GameObject checkMark = null;
        if (WithCheckMark)
        {
            var checkMarkBack = Helpers.CreateObject<SpriteRenderer>("Checkmark", button.transform, (actualSize.ToUnityVector() * 0.5f).AsVector3(-1f) + new Vector3(-0.05f, -0.05f, 0f));
            checkMark = checkMarkBack.gameObject;
            checkMarkBack.sprite = modernCheckSprite.GetSprite(0);
            checkMarkBack.maskInteraction = button.maskInteraction;

            var checkMarkFront = Helpers.CreateObject<SpriteRenderer>("Checkmark", checkMarkBack.transform, new(0.02f, 0.01f, -0.1f));
            checkMarkFront.sprite = modernCheckSprite.GetSprite(1);
            checkMarkFront.maskInteraction = button.maskInteraction;
            checkMarkFront.gameObject.SetActive(true);

            checkMark.SetActive(SelectedDefault);

            button.gameObject.AddComponent<SortingGroup>();
        }

        var piledText = TORGUIContextEngine.API.ColorTextComponent(piledModernColor, Text);
        var selectedText = TORGUIContextEngine.API.ColorTextComponent(selectedModernColor, Text);

        var currentInstance = CurrentInstance;
        GUIButtonInstance buttonInstance = null;
        if (currentInstance != null)
        {
            buttonInstance = new(
                () =>
                {
                    var isPiled = passiveButton.IsPiled();
                    button.sprite = modernButtonSprite.GetSprite(isPiled ? 1 : 0);
                    if (Text != null) innerText.text = (isPiled ? piledText : Text)!.GetString();
                    if (checkMark) checkMark!.SetActive(false);
                },
                () =>
                {
                    var isPiled = passiveButton.IsPiled();
                    if (EmphasizeOnSelected) button.sprite = modernButtonSprite.GetSprite(isPiled ? 4 : 3);
                    if (Text != null) innerText.text = selectedText!.GetString();
                    if (checkMark) checkMark!.SetActive(true);
                }
                )
            { IsSelected = SelectedDefault };

            currentInstance.AddButton(buttonInstance);
            if (!BlockSelectingOnClicked) passiveButton.OnClick.AddListener((Action)(() => currentInstance.Select(buttonInstance)));
        }

        GUIClickable clickable = new(passiveButton, buttonInstance != null ? new GUIHarmoniousSelectable(buttonInstance, currentInstance!) : null);

        //マウスカーソルを合わせたとき
        passiveButton.OnMouseOver.AddListener((Action)(() =>
        {
            var selected = (buttonInstance?.IsSelected ?? false) && EmphasizeOnSelected;
            button.sprite = modernButtonSprite.GetSprite(selected ? 4 : 1);
            if (Text != null) innerText.text = (selected ? selectedText : piledText)!.GetString();
        }));
        passiveButton.OnMouseOut.AddListener((Action)(() =>
        {
            var selected = (buttonInstance?.IsSelected ?? false) && EmphasizeOnSelected;
            button.sprite = modernButtonSprite.GetSprite(selected ? 3 : 0);
            if (Text != null) innerText.text = (selected ? selectedText : Text)!.GetString();
        }));

        //テキスト色の変更
        if (Text != null && SelectedDefault) innerText.text = selectedText!.GetString();

        if (OnClick != null) passiveButton.OnClick.AddListener((Action)(() => OnClick(clickable)));
        if (OnMouseOut != null) passiveButton.OnMouseOut.AddListener((Action)(() => OnMouseOut(clickable)));
        if (OnMouseOver != null) passiveButton.OnMouseOver.AddListener((Action)(() => OnMouseOver(clickable)));

        if (OverlayContext != null)
        {
#if WINDOWS
            passiveButton.OnMouseOver.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(passiveButton, OverlayContext())));
            passiveButton.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(passiveButton)));
#else
            if (OnClickText == null) {
                passiveButton.OnClick.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(passiveButton, OverlayContext())));
            }
#endif
        }

        if (OnRightClick != null) passiveButton.gameObject.AddComponent<ExtraPassiveBehaviour>().OnRightClicked += () => OnRightClick(clickable);

        actualSize.Width += margin + 0.1f;
        actualSize.Height += margin + 0.1f;

        return button.gameObject;
    }
}
