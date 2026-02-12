using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheOtherRoles.MetaContext;

public class GUIScrollDynamicInnerContent
{
    /// <summary>
    /// GUIウィジット
    /// </summary>
    public GUIContextSupplier Context { get; private init; }

    /// <summary>
    /// アラインメント。Left, Center, Rightのみ有効。
    /// </summary>
    public GUIAlignment Alignment { get; private init; }
    /// <summary>
    /// この要素のために確保する幅。
    /// </summary>
    public float Height { get; private init; }

    public GUIScrollDynamicInnerContent(GUIAlignment alignment, GUIContextSupplier context, float height)
    {
        this.Context = context;
        this.Height = height;
        this.Alignment = alignment;
    }
}

public class TORGUIScrollDynamicViewer : MonoBehaviour
{
    static TORGUIScrollDynamicViewer() => ClassInjector.RegisterTypeInIl2Cpp<TORGUIScrollDynamicViewer>();

    private IReadOnlyList<GUIScrollDynamicInnerContent> contents;
    private GameObject[] objects;
    Scroller scroller;
    Collider2D scrollerCollider;
    private Size scrollViewSize;
    private float lastCheckedLocalY = 0f;

    public void SetUp(IReadOnlyList<GUIScrollDynamicInnerContent> contents, Scroller scroller, Collider2D collider, Size scrollViewSize)
    {
        this.contents = contents;
        this.objects = new GameObject[contents.Count];
        this.scroller = scroller;
        this.scrollerCollider = collider;
        this.scrollViewSize = scrollViewSize;
    }

    void Start()
    {
        UpdateContents();
    }

    void Update()
    {
        CheckAndUpdateContents();
    }

    /// <summary>
    /// 必要に応じてコンテンツを更新します。
    /// </summary>
    void CheckAndUpdateContents()
    {
        if (Math.Abs(lastCheckedLocalY - scroller.Inner.transform.localPosition.y) > 0.45f) UpdateContents();
    }

    /// <summary>
    /// コンテンツを更新します。範囲外のコンテンツを削除し、範囲内のコンテンツを表示します。
    /// </summary>
    void UpdateContents()
    {
        float hSum = 0f;

        //表示範囲の上端
        float currentDiffUpper = scroller.Inner.transform.localPosition.y - scroller.ContentYBounds.min;
        //表示範囲の下端
        float currentDiffLower = currentDiffUpper + scrollViewSize.Height;
        //上端、下端を0.5だけ拡張
        currentDiffUpper -= 0.5f;
        currentDiffLower += 0.5f;

        lastCheckedLocalY = scroller.Inner.transform.localPosition.y;

        for (int i = 0; i < contents.Count; i++)
        {
            float myUpper = hSum;
            hSum += contents[i].Height;
            float myLower = hSum;

            bool show = myLower > currentDiffUpper && myUpper < currentDiffLower;

            if (objects[i] != show)
            {
                if (show)
                {
                    objects[i] = contents[i].Context?.Invoke().Instantiate(new Anchor(new(
                        contents[i].Alignment switch
                        {
                            GUIAlignment.Left => 0f,
                            GUIAlignment.Right => 1f,
                            _ => 0.5f
                        }, 0.5f), new(
                        contents[i].Alignment switch
                        {
                            GUIAlignment.Left => -0.5f * scrollViewSize.Width,
                            GUIAlignment.Right => 0.5f * scrollViewSize.Width,
                            _ => 0f
                        }, scrollViewSize.Height * 0.5f - (myUpper + myLower) * 0.5f
                        )), new Size(scrollViewSize.Width, contents[i].Height), out _)!;
                    if (objects[i])
                    {
                        objects[i].transform.SetParent(scroller.Inner, false);
                        foreach (var button in objects[i].GetComponentsInChildren<PassiveButton>()) button.ClickMask = scrollerCollider;
                    }
                }
                else
                {
                    GameObject.Destroy(objects[i]);
                    objects[i] = null!;
                }
            }
        }
    }
}

public class GUIScrollDynamicView : AbstractGUIContext
{
    public string ScrollerTag { get; init; } = null;
    public UnityEngine.Vector2 Size { get; init; }
    public bool WithMask { get; init; } = true;
    public IReadOnlyList<GUIScrollDynamicInnerContent> contexts { get; init; }
    public GUIScrollDynamicView(GUIAlignment alignment, UnityEngine.Vector2 size, IReadOnlyList<GUIScrollDynamicInnerContent> contexts) : base(alignment)
    {
        this.Size = size;
        this.contexts = contexts;
    }



    internal override GameObject Instantiate(Size size, out Size actualSize)
    {
        var view = Helpers.CreateObject("ScrollView", null, new UnityEngine.Vector3(0f, 0f, 0f), LayerMask.NameToLayer("UI"));
        var inner = Helpers.CreateObject("Inner", view.transform, new UnityEngine.Vector3(-0.2f, 0f, -0.1f));
        var innerSize = Size - new UnityEngine.Vector2(0.4f, 0f);

        if (WithMask)
        {
            view.AddComponent<SortingGroup>();
            var mask = Helpers.CreateObject<SpriteMask>("Mask", view.transform, new UnityEngine.Vector3(-0.2f, 0, 0));
            mask.sprite = VanillaAsset.FullScreenSprite;
            mask.transform.localScale = innerSize;
        }

        var scroller = VanillaAsset.GenerateScroller(Size, view.transform, new UnityEngine.Vector2(Size.x / 2 - 0.15f, 0f), inner.transform, new FloatRange(0, Size.y), Size.y);
        var hitBox = scroller.GetComponent<Collider2D>();

        //中身を生成 ここから
        var innerHeightSum = contexts.Sum(w => w.Height);
        scroller.SetBounds(new FloatRange(0, Math.Max(0f, innerHeightSum - Size.y)), null);
        scroller.Inner.gameObject.AddComponent<TORGUIScrollDynamicViewer>().SetUp(contexts, scroller, hitBox, new(innerSize));
        //中身を生成 ここまで


        if (ScrollerTag != null)
            scroller.Inner.transform.localPosition = scroller.Inner.transform.localPosition +
                new UnityEngine.Vector3(0f, Mathf.Clamp(GUIScrollView.TryGetDistHistory(ScrollerTag) + scroller.ContentYBounds.min, scroller.ContentYBounds.min, scroller.ContentYBounds.max), 0f);

        if (ScrollerTag != null)
            scroller.Inner.gameObject.AddComponent<ScriptBehaviour>().UpdateHandler += () => { GUIScrollView.UpdateDistHistory(ScrollerTag, scroller.Inner.transform.localPosition.y - scroller.ContentYBounds.min); };

        actualSize = new(Size.x + 0.15f, Size.y + 0.08f);
        scroller.UpdateScrollBars();

        return view;
    }
}
