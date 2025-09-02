using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using static TheOtherRoles.MetaContext.IMetaContextOld;

namespace TheOtherRoles.MetaContext
{
    public enum BackgroundSetting
    {
        Off,
        Old,
        Modern,
    }

    public class MetaScreen : MonoBehaviour, GUIScreen
    {
        static MetaScreen()
        {
            ClassInjector.RegisterTypeInIl2Cpp<MetaScreen>();
        }

        public void Start()
        {
            gameObject.AddComponent<SortingGroup>();
        }

        private GameObject combinedObject = null;
        public LineRenderer borderLine = null;
        private Vector2 border;
        public Vector2 Border
        {
            get => border; private set
            {
                bool lastShownFlag = ShowBorderLine;
                ShowBorderLine = false;
                border = value;
                ShowBorderLine = lastShownFlag;
            }
        }

        public bool ShowBorderLine
        {
            get => borderLine != null; set
            {
                if (borderLine == null && value)
                {
                    var lineObj = new GameObject("BorderLine")
                    {
                        layer = LayerMask.NameToLayer("UI")
                    };
                    lineObj.transform.SetParent(transform);
                    lineObj.transform.localPosition = new Vector3(0, 0, 0);
                    borderLine = lineObj.AddComponent<LineRenderer>();
                    borderLine.material.shader = Shader.Find("Sprites/Default");
                    borderLine.positionCount = 4;
                    borderLine.loop = true;
                    float x = Border.x / 2, y = Border.y / 2;
                    borderLine.SetPositions(new Vector3[] { new(-x, -y), new(-x, y), new(x, y), new(x, -y) });
                    borderLine.SetWidth(0.1f, 0.1f);
                    borderLine.SetColors(Color.cyan, Color.cyan);
                    borderLine.useWorldSpace = false;
                }
                else if (borderLine != null && !value)
                {
                    GameObject.Destroy(borderLine.gameObject);
                    borderLine = null;
                }
            }
        }

        public float SetContext(Vector2? border, IMetaContextOld context)
        {
            if (border != null) Border = border.Value;

            return SetContext(context);
        }

        private void ClearContext()
        {
            gameObject.ForEachChild((Il2CppSystem.Action<GameObject>)((obj) =>
            {
                if (obj.name != "BorderLine") GameObject.Destroy(obj);
            }));
        }
        public float SetContext(IMetaContextOld context, out (float min, float max) width)
        {
            ClearContext();

            if (context == null)
            {
                width = (0f, 0f);
                return 0f;
            }

            Vector2 cursor = Border / 2f;
            cursor.x *= -1f;
            return context.Generate(gameObject, cursor, Border, out width);
        }

        public float SetContext(IMetaContextOld context) => SetContext(context, out var _);

        public void CloseScreen()
        {
            try
            {
                GameObject.Destroy(combinedObject ?? gameObject);
            }
            catch { }
        }

        static private MultiImage closeButtonSprite = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.CloseButton.png", 100f, 2, 1);
        static readonly private Image backFrameSprite = new ResourceExpandableSpriteLoader("TheOtherRoles.Resources.Background_Frame.png", 150f, 60, 60);
        static readonly private Image backInnerSprite = new ResourceExpandableSpriteLoader("TheOtherRoles.Resources.Background_Inner.png", 150f, 60, 60);
        static public Image BackFrameImage => backFrameSprite;
        static public Image BackInnerImage => backInnerSprite;

        static public MetaScreen GenerateScreen(Vector2 size, Transform parent, Vector3 localPos, bool withBackground, bool withBlackScreen, bool withClickGuard)
        => GenerateScreen(size, parent, localPos, withBackground ? BackgroundSetting.Old : BackgroundSetting.Off, withBlackScreen, withClickGuard);
        static public MetaScreen GenerateScreen(Vector2 size, Transform parent, Vector3 localPos, BackgroundSetting backgroundSetting, bool withBlackScreen, bool withClickGuard)
        {
            var window = Helpers.CreateObject("MetaWindow", parent, localPos);
            if (backgroundSetting == BackgroundSetting.Old)
            {
                var renderer = Helpers.CreateObject<SpriteRenderer>("Background", window.transform, new Vector3(0, 0, 0.1f));
                renderer.sprite = VanillaAsset.PopUpBackSprite;
                renderer.drawMode = SpriteDrawMode.Sliced;
                renderer.tileMode = SpriteTileMode.Continuous;
                renderer.size = size + new Vector2(0.45f, 0.35f);
                renderer.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            else if (backgroundSetting == BackgroundSetting.Modern)
            {
                var inner = Helpers.CreateObject<SpriteRenderer>("Inner", window.transform, new Vector3(0, 0, 0.1f));
                inner.sprite = backInnerSprite.GetSprite();
                inner.drawMode = SpriteDrawMode.Sliced;
                inner.tileMode = SpriteTileMode.Continuous;
                inner.size = size + new Vector2(0.75f, 0.75f);
                inner.gameObject.layer = LayerMask.NameToLayer("UI");
                inner.color = Color.white.RGBMultiplied(0.55f);

                var frame = Helpers.CreateObject<SpriteRenderer>("Frame", window.transform, new Vector3(0, 0, -0.01f));
                frame.sprite = backFrameSprite.GetSprite();
                frame.drawMode = SpriteDrawMode.Sliced;
                frame.tileMode = SpriteTileMode.Continuous;
                frame.size = size + new Vector2(0.75f, 0.75f);
                frame.gameObject.layer = LayerMask.NameToLayer("UI");
            }

            if (withBlackScreen)
            {
                var renderer = Helpers.CreateObject<SpriteRenderer>("BlackScreen", window.transform, new Vector3(0, 0, 0.2f));
                renderer.sprite = VanillaAsset.FullScreenSprite;
                renderer.color = new Color(0, 0, 0, 0.4226f);
                renderer.transform.localScale = new Vector3(100f, 100f);
                renderer.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            if (withClickGuard)
            {
                var collider = Helpers.CreateObject<BoxCollider2D>("ClickGuard", window.transform, new Vector3(0, 0, 0.2f));
                collider.isTrigger = true;
                collider.gameObject.layer = LayerMask.NameToLayer("UI");
                collider.size = new Vector2(100f, 100f);
                collider.gameObject.SetUpButton();
            }
            var screen = Helpers.CreateObject<MetaScreen>("Screen", window.transform, Vector3.zero);
            screen.Border = size;
            screen.combinedObject = window;

            return screen;
        }

        /// <summary>
        /// 枠線および不透明な黒い領域を持たない透明ウィンドウを生成します。
        /// このウィンドウはほかのウィンドウと同様に順番に整列します。
        /// </summary>
        /// <param name="size"></param>
        /// <param name="parent"></param>
        /// <param name="localPos"></param>
        /// <param name="closeButtonPos"></param>
        /// <param name="withBlackScreen"></param>
        /// <returns></returns>
        static public MetaScreen GenerateBlankWindow(Vector2 size, Transform parent, Vector3 localPos, Vector2 closeButtonPos, bool withBlackScreen)
        {
            var screen = GenerateScreen(size, parent, localPos, true, withBlackScreen, true);
            var obj = screen.transform.parent.gameObject;
            var button = InstantiateCloseButton(screen, closeButtonPos);
            TORGUIManager.Instance.RegisterUI(obj, button);
            return screen;
        }

        static public PassiveButton InstantiateCloseButton(Transform parent, Vector3 localPos)
        {
            var collider = Helpers.CreateObject<CircleCollider2D>("CloseButton", parent, localPos);
            collider.isTrigger = true;
            collider.gameObject.layer = LayerMask.NameToLayer("UI");
            collider.radius = 0.25f;
            SpriteRenderer renderer = null;
            renderer = collider.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = VanillaAsset.CloseButtonSprite;
            var button = collider.gameObject.SetUpButton(true, renderer);

            return button;
        }

        static public PassiveButton InstantiateCloseButton(MetaScreen target, Vector3 localPos)
        {
            var button = InstantiateCloseButton(target.transform.parent, localPos);
            var targetObj = target.combinedObject;
            button.OnClick.AddListener((Action)(() => GameObject.Destroy(targetObj)));

            return button;
        }

        static public MetaScreen GenerateWindow(Vector2 size, Transform parent, Vector3 localPos, bool withBlackScreen, bool closeOnClickOutside, bool withMask = false, BackgroundSetting background = BackgroundSetting.Old)
        {
            var screen = GenerateScreen(size, parent, localPos, background, withBlackScreen, true);

            var obj = screen.transform.parent.gameObject;

            if (background == BackgroundSetting.Modern)
            {
                var collider = Helpers.CreateObject<BoxCollider2D>("CloseButton", obj.transform, new Vector3(-size.x / 2f - 0.3f, size.y / 2f + 0.2f, -1f));
                collider.transform.localScale = new(0.57f, 0.57f, 1f);
                collider.isTrigger = true;
                collider.gameObject.layer = LayerMask.NameToLayer("UI");
                collider.size = new(0.85f, 0.85f);
                SpriteRenderer renderer = null;
                renderer = collider.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = closeButtonSprite.GetSprite(0);
                var button = collider.gameObject.SetUpButton(true);
                button.OnClick.AddListener((Action)(() => GameObject.Destroy(obj)));
                button.OnMouseOver.AddListener((Action)(() => renderer.sprite = closeButtonSprite.GetSprite(1)));
                button.OnMouseOut.AddListener((Action)(() => renderer.sprite = closeButtonSprite.GetSprite(0)));
                TORGUIManager.Instance.RegisterUI(obj, button);
            }
            else
            {
                var button = InstantiateCloseButton(screen, new Vector3(-size.x / 2f - 0.6f, size.y / 2f + 0.25f, 0f));
                TORGUIManager.Instance.RegisterUI(obj, button);
            }

            if (closeOnClickOutside)
            {
                obj.transform.FindChild("ClickGuard").GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => GameObject.Destroy(obj)));
                var myCollider = Helpers.CreateObject<BoxCollider2D>("MyScreenCollider", obj.transform, new Vector3(0f, 0f, 0.1f));
                myCollider.isTrigger = true;
                myCollider.size = size;
                myCollider.gameObject.SetUpButton();
            }

            if (withMask)
            {
                var group = Helpers.CreateObject<SortingGroup>("Group", obj.transform, Vector3.zero);
                var mask = Helpers.CreateObject<SpriteMask>("Mask", group.transform, Vector3.zero);
                mask.sprite = VanillaAsset.FullScreenSprite;
                mask.transform.localScale = size;

                obj.transform.FindChild("Screen").SetParent(group.transform);
            }

            return screen;
        }

        public void SetBorder(Vector2 border)
        {
            this.border = border;
        }

        public void SetContext(GUIContext context, out Size actualSize) => SetContext(context, new Vector2(0f, 1f), out actualSize);
        public void SetContext(GUIContext context, Image image, out Size actualSize) => SetContext(context, new(0f, 1f), image, out actualSize);
        public void SetContext(GUIContext context, UnityEngine.Vector2 anchor, out Size actualSize)
        {
            ClearContext();

            var obj = context.Instantiate(new Anchor(new(anchor.x, anchor.y), new Vector3(border.x * (anchor.x - 0.5f), border.y * (anchor.y - 0.5f), -0.1f)), new(border), out actualSize);
            if (obj != null)
            {
                obj.transform.SetParent(transform, false);
            }
        }

        public void SetContext(GUIContext context, UnityEngine.Vector2 anchor, Image image, out Size actualSize)
        {
            SetContext(context, anchor, out actualSize);
            SetBackImage(image);
        }

        //既存のコンテンツを削除せずに背景画像を追加します。
        public void SetBackImage(Image image, float brightness = 0.25f)
        {
            SetBackImage(image, brightness, 0.6f, new Vector3(border.x, -border.y + 0.5f) * 0.27f, new Vector2(border.x * 0.974f + 0.45f, border.y * 0.96f + 0.35f), 0.7f);
        }

        public void ClearBackImage()
        {
            try
            {
                var image = transform.FindChild("BackImage");
                if (image) GameObject.Destroy(image.gameObject);
            }
            catch
            {
            }
        }

        public void SetBackImage(Image image, float brightness, float alpha, Vector2 imagePosition, Vector2 maskScale, float imageScale, bool grayout = false)
        {
            if (image?.GetSprite() == null) return;

            var group = Helpers.CreateObject<SortingGroup>("BackImage", this.transform, new(0f, 0f, 0.06f));
            group.sortingOrder = -10;
            var mask = Helpers.CreateObject<SpriteMask>("Mask", group.transform, Vector3.zero);
            mask.sprite = VanillaAsset.FullScreenSprite;
            mask.transform.localScale = maskScale;

            var renderer = Helpers.CreateObject<SpriteRenderer>("MaskedImage", group.transform, imagePosition);
            renderer.sprite = image.GetSprite();
            renderer.transform.localScale = new(imageScale, imageScale);
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            renderer.color = new(brightness, brightness, brightness, alpha);
            if (grayout) renderer.material.shader = Modules.Achievement.materialShader;
        }
    }

    public interface IMetaParallelPlacableOld
    {
        public float Generate(GameObject screen, Vector2 center, out float width);

    }

    public interface IMetaContextOld
    {
        public enum AlignmentOption
        {
            Left,
            Center,
            Right
        }
        public AlignmentOption Alignment { get; }
        public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width);
        public float Generate(GameObject screen, Vector2 cursor, Vector2 size) => Generate(screen, cursor, size, out var _);

        protected static (float, float) CalcWidth(AlignmentOption alignment, Vector2 cursor, Vector2 size, float width) => CalcWidth(alignment, cursor, size, width, -width / 2f, width / 2f);
        protected static (float, float) CalcWidth(AlignmentOption alignment, Vector2 cursor, Vector2 size, float width, float actualMin, float actualMax)
        {
            float center = 0;
            switch (alignment)
            {
                case AlignmentOption.Left:
                    center = cursor.x + width / 2;
                    break;
                case AlignmentOption.Right:
                    center = cursor.x + size.x - width / 2;
                    break;
                case AlignmentOption.Center:
                default:
                    center = cursor.x + size.x / 2;
                    break;
            }
            return new(center + actualMin, center + actualMax);
        }

        protected static Vector3 ReflectAlignment(AlignmentOption alignment, Vector2 mySize, Vector2 cursor, Vector2 size)
        {
            switch (alignment)
            {
                case AlignmentOption.Left:
                    return new Vector3(cursor.x + mySize.x / 2, cursor.y - mySize.y / 2);
                case AlignmentOption.Right:
                    return new Vector3(cursor.x + size.x - mySize.x / 2, cursor.y - mySize.y / 2);
                case AlignmentOption.Center:
                default:
                    return new Vector3(cursor.x + size.x / 2, cursor.y - mySize.y / 2);
            }
        }
    }

    public class WrapSpriteLoader : Image
    {
        Func<Sprite> supplier;

        public WrapSpriteLoader(Func<Sprite> supplier)
        {
            this.supplier = supplier;
        }

        public Sprite GetSprite() => supplier.Invoke();

        public void UnloadAsset() { }

        public System.Collections.IEnumerator LoadAsset() { yield break; }
        public void MarkAsUnloadAsset() { }
    }

    public class TranslateTextComponent : TextComponent
    {
        public string TranslationKey { get; set; }
        public string GetString() => ModTranslation.getString(TranslationKey);
        public TranslateTextComponent(string translationKey)
        {
            TranslationKey = translationKey;
        }
    }

    public class LazyTextComponent : TextComponent
    {
        private Func<string> supplier;
        public LazyTextComponent(Func<string> supplier)
        {
            this.supplier = supplier;
        }

        public string GetString() => supplier.Invoke();
    }

    public class ColorTextComponent : TextComponent
    {
        public Color Color { get; set; }
        TextComponent Inner { get; set; }
        public string GetString() => Inner.GetString().Color(Color);
        public ColorTextComponent(Color color, TextComponent inner)
        {
            Color = color;
            Inner = inner;
        }
    }

    public class RawTextComponent : TextComponent
    {
        public string RawText { get; set; }
        public string GetString() => RawText;

        public RawTextComponent(string text)
        {
            RawText = text;
        }
    }

    public class ScriptBehaviour : MonoBehaviour
    {
        static ScriptBehaviour() => ClassInjector.RegisterTypeInIl2Cpp<ScriptBehaviour>();

        public event Action UpdateHandler;
        public event Action ActiveHandler;
        public void Update()
        {
            UpdateHandler?.Invoke();
        }

        public void OnEnable()
        {
            ActiveHandler?.Invoke();
        }
    }

    public class ParallelContextOld : IMetaContextOld
    {
        Tuple<IMetaContextOld, float>[] contents;
        public ParallelContextOld(params Tuple<IMetaContextOld, float>[] contents)
        {
            this.contents = contents;
        }

        public AlignmentOption Alignment { get; set; }

        public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
        {
            float sum = contents.Sum(c => c.Item2);
            float height = 0;

            if (Alignment == AlignmentOption.Right)
                cursor.x += size.x - sum;
            else if (Alignment == AlignmentOption.Center)
                cursor.x += (size.x - sum) / 2f;

            float widthMin = size.x / 2f;
            float widthMax = cursor.x;

            foreach (var c in contents)
            {
                float myX = (c.Item2 / sum) * size.x;
                float temp = c.Item1.Generate(screen, cursor, new Vector2(myX, size.y), out var innerWidth);

                widthMin = Mathf.Min(widthMin, innerWidth.min);
                widthMax = Mathf.Min(widthMax, innerWidth.max);

                cursor.x += myX;
                if (temp > height) height = temp;
            }

            width = new(widthMin, widthMax);

            return height;
        }
    }

    public class CombinedContextOld : IMetaContextOld, IMetaParallelPlacableOld
    {
        IMetaParallelPlacableOld[] contents;
        public AlignmentOption Alignment { get; set; }
        float? height;
        public Action<GameObject> PostBuilder = null;

        public CombinedContextOld(float height, AlignmentOption alignment, params IMetaParallelPlacableOld[] contents)
        {
            this.contents = contents.ToArray();
            this.Alignment = alignment;
            this.height = height < 0f ? null : height;
        }

        public CombinedContextOld(float height, params IMetaParallelPlacableOld[] contents) : this(height, AlignmentOption.Center, contents) { }
        public CombinedContextOld(params IMetaParallelPlacableOld[] contents) : this(-1f, AlignmentOption.Center, contents) { }

        public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
        {
            var combinedScreen = Helpers.CreateObject("CombinedScreen", screen.transform, Vector3.zero);
            float x = 0f;
            float height = 0f;
            foreach (var c in contents)
            {
                var alloc = Helpers.CreateObject("Allocator", combinedScreen.transform, new Vector3(x, 0f, 0f));
                height = Mathf.Max(height, c.Generate(alloc, Vector2.zero, out float cWidth));
                alloc.transform.localPosition += new Vector3(cWidth * 0.5f, 0f);
                x += cWidth;
            }
            if (this.height.HasValue) height = this.height.Value;

            float centerY = cursor.y - height / 2f;

            float leftPos;
            switch (Alignment)
            {
                case AlignmentOption.Left:
                    leftPos = cursor.x; break;
                case AlignmentOption.Right:
                    leftPos = cursor.x + size.x - x; break;
                case AlignmentOption.Center:
                default:
                    leftPos = cursor.x + (size.x - x) / 2f; break;
            }

            combinedScreen.transform.localPosition += new Vector3(leftPos, centerY, 0f);

            width = CalcWidth(Alignment, cursor, size, x, -x / 2f, x / 2f);

            PostBuilder?.Invoke(combinedScreen);

            return height;
        }

        public float Generate(GameObject screen, Vector2 center, out float width)
        {
            var combinedScreen = Helpers.CreateObject("CombinedScreen", screen.transform, center);
            float x = 0f;
            float height = 0f;
            foreach (var c in contents)
            {
                var alloc = Helpers.CreateObject("Allocator", combinedScreen.transform, new Vector3(x, 0f, 0f));
                height = Mathf.Max(height, c.Generate(alloc, Vector2.zero, out float cWidth));
                alloc.transform.localPosition += new Vector3(cWidth * 0.5f, 0f);
                x += cWidth;
            }
            if (this.height.HasValue) height = this.height.Value;


            combinedScreen.transform.localPosition += new Vector3(-x / 2f, 0f, 0f);

            width = x;

            PostBuilder?.Invoke(combinedScreen);

            return height;
        }

    }

    public class MetaContextOld : IMetaContextOld, IMetaParallelPlacableOld
    {
        List<IMetaContextOld> contents = new();

        public MetaContextOld(params IMetaContextOld[] contents) { foreach (var c in contents) Append(c); }

        public int Count => contents.Count;
        public AlignmentOption Alignment => AlignmentOption.Center;
        public float? MaxWidth = null;
        public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float, float) width)
        {
            if (contents.Count == 0)
            {
                width = (cursor.x, cursor.x);
                return 0f;
            }

            size.x = size.x > 0f ? Mathf.Min(MaxWidth ?? size.x, size.x) : (MaxWidth ?? size.x);

            float widthMin = size.x / 2;
            float widthMax = cursor.x;

            float heightSum = 0;
            foreach (var c in contents)
            {
                float height = c.Generate(screen, cursor, size, out var innerWidth);
                widthMin = Math.Min(widthMin, innerWidth.min);
                widthMax = Math.Max(widthMax, innerWidth.max);
                heightSum += height;
                cursor.y -= height;
                size.y -= height;
            }

            width = new(widthMin, widthMax);
            return heightSum;
        }

        public float Generate(GameObject screen, Vector2 center, out float width)
        {
            if (contents.Count == 0)
            {
                width = 0f;
                return 0f;
            }

            var subscreen = Helpers.CreateObject("MetaContext", screen.transform, new Vector3(center.x, center.y, -0.1f));

            float widthMin = float.MaxValue;
            float widthMax = float.MinValue;

            float heightSum = 0;
            foreach (var c in contents)
            {
                float height = c.Generate(subscreen, new Vector2(0, -heightSum), new Vector2(MaxWidth ?? 0f, 0), out var innerWidth);
                widthMin = Math.Min(widthMin, innerWidth.min);
                widthMax = Math.Max(widthMax, innerWidth.max);
                heightSum += height;
            }

            width = widthMax - widthMin;

            subscreen.transform.localPosition -= new Vector3(width * 0.5f, -heightSum * 0.5f);
            return heightSum;
        }

        public MetaContextOld Append(IMetaContextOld content)
        {
            if (content != null) contents.Add(content);
            return this;
        }

        //linesを-1にすると全部を並べる
        public MetaContextOld Append<T>(IEnumerable<T> enumerable, Func<T, IMetaParallelPlacableOld> converter, int perLine, int lines, int page, float height, bool fixedHeight = false, AlignmentOption alignment = AlignmentOption.Center)
        {
            int skip = lines > 0 ? page * lines : 0;
            int leftLines = lines;
            int index = 0;
            IMetaParallelPlacableOld[] contextList = new IMetaParallelPlacableOld[perLine];

            foreach (var content in enumerable)
            {
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                contextList[index] = converter.Invoke(content);
                index++;

                if (index == perLine)
                {
                    Append(new CombinedContextOld(height, contextList) { Alignment = alignment });
                    index = 0;
                    leftLines--;

                    if (leftLines == 0) break;
                }
            }

            if (index != 0)
            {
                for (; index < perLine; index++) contextList[index] = new HorizonalMargin(0f);
                Append(new CombinedContextOld(height, contextList) { Alignment = alignment });
                leftLines--;
            }

            if (fixedHeight && leftLines > 0) for (int i = 0; i < leftLines; i++) Append(new VerticalMargin(height));

            return this;
        }

        public MetaContextOld[] Split(params float[] ratios)
        {
            var contents = ratios.Select(ratio => new Tuple<IMetaContextOld, float>(new MetaContextOld(), ratio)).ToArray();
            Append(new ParallelContextOld(contents));
            return contents.Select(c => (MetaContextOld)c.Item1).ToArray();
        }

        public class Image : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment { get; set; }
            public float Width { get; set; }
            public Sprite Sprite { get; set; }
            public Action<SpriteRenderer> PostBuilder { get; set; }

            public Image(Sprite sprite)
            {
                Sprite = sprite;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var renderer = Helpers.CreateObject<SpriteRenderer>("Image", screen.transform, center);
                renderer.sprite = Sprite;
                renderer.sortingOrder = 10;
                if (Sprite) renderer.transform.localScale = Vector3.one * (Width / Sprite!.bounds.size.x);
                PostBuilder?.Invoke(renderer);
                width = Width + 0.1f;
                return (Sprite ? renderer.transform.localScale.y * Sprite!.bounds.size.y : 0f) + 0.1f;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                Vector2 mySize = Sprite ? Sprite!.bounds.size * (Width / Sprite!.bounds.size.x) : new(Width, Width);

                if (Sprite)
                {
                    var renderer = Helpers.CreateObject<SpriteRenderer>("Image", screen.transform, ReflectAlignment(Alignment, mySize + new Vector2(0.1f, 0.1f), cursor, size));
                    renderer.sprite = Sprite;
                    renderer.transform.localScale = Vector3.one * (Width / Sprite!.bounds.size.x);
                    renderer.sortingOrder = 10;
                    PostBuilder?.Invoke(renderer);
                }

                width = CalcWidth(Alignment, cursor, size, mySize.x + 0.1f);

                return mySize.y + 0.1f;
            }
        }

        public class Text : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment { get; set; }
            public TextAttribute TextAttribute { get; set; }

            public TextComponent MyText { get; set; } = null;
            public string RawText { set { MyText = new RawTextComponent(value); } }
            public string TranslationKey { set { MyText = new TranslateTextComponent(value); } }

            public Action<TMPro.TextMeshPro> PostBuilder { get; set; }

            public Text(TextAttribute attribute)
            {
                TextAttribute = attribute;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var text = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, screen.transform);
                TextAttribute.Reflect(text);
                text.text = MyText?.GetString() ?? "";
                text.transform.localPosition = center;
                text.sortingOrder = 10;

                PostBuilder?.Invoke(text);

                width = TextAttribute.Size.x;

                return TextAttribute.Size.y;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var text = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, screen.transform);
                TextAttribute.Reflect(text);
                if (!(TextAttribute.Size.x > 0)) text.rectTransform.sizeDelta = new Vector2(size.x, TextAttribute.Size.y);
                text.text = MyText?.GetString() ?? "";
                text.transform.localPosition = ReflectAlignment(Alignment, TextAttribute.Size, cursor, size);
                text.sortingOrder = 10;

                text.ForceMeshUpdate();

                var bounds = text.GetTextBounds();
                width = CalcWidth(Alignment, cursor, size, TextAttribute.Size.x, bounds.min.x, bounds.max.x);

                PostBuilder?.Invoke(text);

                return TextAttribute.Size.y;
            }
        }

        public class VariableText : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment { get; set; }
            public TextAttribute TextAttribute { get; set; }
            public TextComponent MyText { get; set; } = null;
            public string RawText { set { MyText = new RawTextComponent(value); } }
            public string TranslationKey { set { MyText = new TranslateTextComponent(value); } }
            public Action<TMPro.TextMeshPro> PostBuilder { get; set; } = null;

            public VariableText(TextAttribute attribute)
            {
                TextAttribute = attribute;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var text = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, screen.transform);
                TextAttribute.Reflect(text);
                text.text = MyText?.GetString() ?? "";
                text.transform.localPosition = center;
                text.sortingOrder = 10;

                text.ForceMeshUpdate();
                text.rectTransform.sizeDelta = new(text.preferredWidth, text.preferredHeight);

                text.ForceMeshUpdate();

                PostBuilder?.Invoke(text);

                width = text.rectTransform.sizeDelta.x;

                return text.rectTransform.sizeDelta.y;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var text = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, screen.transform);
                TextAttribute.Reflect(text);
                if (!(TextAttribute.Size.x > 0)) text.rectTransform.sizeDelta = new Vector2(size.x, TextAttribute.Size.y);
                text.text = MyText?.GetString() ?? "";
                text.sortingOrder = 10;

                text.ForceMeshUpdate();
                text.rectTransform.sizeDelta = new(text.preferredWidth, text.preferredHeight);

                text.ForceMeshUpdate();
                var bounds = text.GetTextBounds();
                width = CalcWidth(Alignment, cursor, text.rectTransform.sizeDelta, text.rectTransform.sizeDelta.x, bounds.min.x, bounds.max.x);

                text.transform.localPosition = ReflectAlignment(Alignment, text.rectTransform.sizeDelta, cursor, size);

                PostBuilder?.Invoke(text);

                return text.rectTransform.sizeDelta.y;
            }
        }

        public class Button : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment { get; set; }
            public TextAttribute TextAttribute { get; set; }
            public Action OnClick { get; set; }
            public Action OnMouseOver { get; set; }
            public Action OnMouseOut { get; set; }
            public Color? Color { get; set; }

            public TextComponent Text { get; set; }
            public string RawText { set { Text = new RawTextComponent(value); } }
            public string TranslationKey { set { Text = new TranslateTextComponent(value); } }

            public Action<PassiveButton, SpriteRenderer, TMPro.TextMeshPro> PostBuilder { get; set; }
            public Button(Action onClick, TextAttribute attribute)
            {
                TextAttribute = attribute;
                OnClick = onClick;
            }

            public Button SetAsMaskedButton()
            {
                PostBuilder = (_, renderer, _) => renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                return this;
            }

            private const float TextMargin = 0.26f;

            private void Generate(SpriteRenderer button, out PassiveButton passiveButton, out TMPro.TextMeshPro text)
            {
                button.sprite = VanillaAsset.TextButtonSprite;
                button.drawMode = SpriteDrawMode.Sliced;
                button.tileMode = SpriteTileMode.Continuous;
                button.size = TextAttribute.Size + new Vector2(TextMargin * 0.84f, TextMargin * 0.84f);
                button.gameObject.layer = LayerMask.NameToLayer("UI");
                button.gameObject.AddComponent<SortingGroup>();

                text = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, button.transform);
                TextAttribute.Reflect(text);
                text.text = Text?.GetString() ?? "";
                text.transform.localPosition = new Vector3(0, 0, -0.1f);
                text.sortingOrder = 15;

                var collider = button.gameObject.AddComponent<BoxCollider2D>();
                collider.size = TextAttribute.Size + new Vector2(TextMargin * 0.6f, TextMargin * 0.6f);
                collider.isTrigger = true;

                passiveButton = button.gameObject.SetUpButton(true, button, Color);
                if (OnClick != null) passiveButton.OnClick.AddListener(OnClick);
                if (OnMouseOut != null) passiveButton.OnMouseOut.AddListener(OnMouseOut);
                if (OnMouseOver != null) passiveButton.OnMouseOver.AddListener(OnMouseOver);
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var renderer = Helpers.CreateObject<SpriteRenderer>("Button", screen.transform, center);
                Generate(renderer, out var button, out var text);
                PostBuilder?.Invoke(button, renderer, text);

                width = TextAttribute.Size.x + TextMargin;

                return TextAttribute.Size.y + TextMargin;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var renderer = Helpers.CreateObject<SpriteRenderer>("Button", screen.transform,
                    ReflectAlignment(Alignment, TextAttribute.Size + new Vector2(TextMargin, TextMargin), cursor, size));
                renderer.sortingOrder = 5;
                Generate(renderer, out var button, out var text);
                PostBuilder?.Invoke(button, renderer, text);

                var mySize = TextAttribute.Size.x + TextMargin;
                width = CalcWidth(Alignment, cursor, size, mySize, -mySize / 2f, mySize / 2f);

                return TextAttribute.Size.y + TextMargin;
            }

            static public MetaContextOld GetTwoWayButton(Action<bool> buttonAction)
            {
                MetaContextOld context = new();

                for (int i = 0; i < 2; i++)
                {
                    int copied = i;
                    context.Append(new Button(() => buttonAction.Invoke(copied == 0), new(TextAttribute.BoldAttr) { Size = new(0.18f, 0.08f) }) { RawText = copied == 0 ? "▲" : "▼" });
                }


                return context;
            }
        }

        public class StateButton : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment { get; set; }
            public Action<bool> OnChanged { get; set; }
            public Reference<bool> StateRef { get; set; }
            public bool WithMaskMaterial { get; set; } = false;
            public bool FirstState = false;
            public StateButton() { }

            private void Generate(GameObject obj)
            {
                var attr = new TextAttribute(TextAttribute.NormalAttr) { Size = new(0.36f, 0.36f), FontMaterial = WithMaskMaterial ? VanillaAsset.StandardMaskedFontMaterial : null }.EditFontSize(2f, 1f, 2f);

                var checkMark = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, obj.transform);
                attr.Reflect(checkMark);
                checkMark.text = "<b>✓</b>";
                checkMark.transform.localPosition = new Vector3(0, 0, -0.2f);
                checkMark.sortingOrder = 16;
                checkMark.color = Color.green;
                checkMark.gameObject.SetActive(StateRef?.Value ?? false);

                var box = GameObject.Instantiate(VanillaAsset.StandardTextPrefab, obj.transform);
                attr.Reflect(box);
                box.text = "□";
                box.transform.localPosition = new Vector3(0, 0, -0.1f);
                box.sortingOrder = 15;

                var collider = obj.gameObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.2f, 0.2f);
                collider.isTrigger = true;

                var copiedState = StateRef;
                var copiedChanged = OnChanged;
                void SetState(bool state, bool callEvent = true)
                {
                    if (copiedState != null) copiedState.Value = state;
                    checkMark.gameObject.SetActive(state);
                    if (callEvent) copiedChanged?.Invoke(state);
                }
                void SwitchState() => SetState(!checkMark.gameObject.activeSelf);

                var passiveButton = obj.SetUpButton(true);
                passiveButton.OnClick.AddListener((Action)(() => SwitchState()));
                passiveButton.OnMouseOut.AddListener((Action)(() => box.color = Color.white));
                passiveButton.OnMouseOver.AddListener((Action)(() => box.color = Color.green));
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var obj = Helpers.CreateObject("CheckBox", screen.transform, center);
                Generate(obj);

                width = 0.25f;
                return 0.25f;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var obj = Helpers.CreateObject("CheckBox", screen.transform, ReflectAlignment(Alignment, new Vector2(0.25f, 0.25f), cursor, size));
                Generate(obj);

                width = CalcWidth(Alignment, cursor, size, 0.25f);

                return 0.25f;
            }

            public static CombinedContextOld CheckBox(string translateKey, float width, bool isBold, Reference<bool> state, Action<bool> onChanged, bool maskMaterial = false)
            {
                return new CombinedContextOld(
                    new StateButton() { StateRef = state, OnChanged = onChanged, WithMaskMaterial = maskMaterial },
                    new Text(new(isBold ? TextAttribute.BoldAttr : TextAttribute.NormalAttr) { FontMaterial = maskMaterial ? VanillaAsset.StandardMaskedFontMaterial : null, Size = new(width, 0.3f), Alignment = TMPro.TextAlignmentOptions.Left }) { TranslationKey = translateKey }
                    );
            }

            public static CombinedContextOld TopLabelCheckBox(string translateKey, float? width, bool isBold, Reference<bool> state, Action<bool> onChanged, bool maskMaterial = false)
            {
                IMetaParallelPlacableOld label;
                if (width == null)
                    label = new VariableText(new(isBold ? TextAttribute.BoldAttr : TextAttribute.NormalAttr) { FontMaterial = maskMaterial ? VanillaAsset.StandardMaskedFontMaterial : null, Size = new(5f, 0.3f), Alignment = TMPro.TextAlignmentOptions.Left }) { TranslationKey = translateKey };
                else
                    label = new Text(new(isBold ? TextAttribute.BoldAttr : TextAttribute.NormalAttr) { FontMaterial = maskMaterial ? VanillaAsset.StandardMaskedFontMaterial : null, Size = new(width.Value, 0.3f), Alignment = TMPro.TextAlignmentOptions.Left }) { TranslationKey = translateKey };

                return new CombinedContextOld(
                    label,
                    new Text(new(isBold ? TextAttribute.BoldAttr : TextAttribute.NormalAttr) { FontMaterial = maskMaterial ? VanillaAsset.StandardMaskedFontMaterial : null, Size = new(0.15f, 0.3f), Alignment = TMPro.TextAlignmentOptions.Center }) { RawText = ":" },
                    new StateButton() { StateRef = state, OnChanged = onChanged, WithMaskMaterial = maskMaterial }
                    );
            }
        }

        public class CustomContext : IMetaContextOld, IMetaParallelPlacableOld
        {
            public Vector2 Size { get; set; }

            public AlignmentOption Alignment { get; set; }
            public Generator ContentGenerator;

            public delegate void Generator(Transform parent, Vector2 center);

            public CustomContext(Vector2 size, AlignmentOption alignment, Generator generator)
            {
                Size = size;
                Alignment = alignment;
                ContentGenerator = generator;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                width = CalcWidth(Alignment, cursor, size, Size.x);
                ContentGenerator.Invoke(screen.transform, new Vector2((width.min + width.max) * 0.5f, cursor.y - Size.y * 0.5f));
                return Size.y;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                width = Size.x;
                ContentGenerator.Invoke(screen.transform, center);
                return Size.y;
            }
        }

        public class WrappedContext : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment => Context.Alignment switch { GUIAlignment.Left => AlignmentOption.Left, GUIAlignment.Right => AlignmentOption.Right, _ => AlignmentOption.Center };
            public GUIContext Context;

            public WrappedContext(GUIContext context)
            {
                Context = context;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var result = Context.Instantiate(new Anchor(new(0f, 1f), new(cursor.x, cursor.y)), new(size), out var actual);
                if (result != null) result.transform.SetParent(screen.transform, false);
                width = CalcWidth(Alignment, cursor, size, actual.Width);
                return actual.Height;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var result = Context.Instantiate(new Anchor(new(0.5f, 0.5f), new(0f, 0f)), new(10f, 10f), out var actual);
                if (result != null) result.transform.SetParent(screen.transform, false);

                width = actual.Width;
                return actual.Height;
            }
        }

        public class VerticalMargin : IMetaContextOld, IMetaParallelPlacableOld
        {
            public float Margin { get; set; }

            public AlignmentOption Alignment => AlignmentOption.Center;

            public VerticalMargin(float margin)
            {
                Margin = margin;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                width = new(cursor.x, cursor.x);
                return Margin;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                width = 0f;
                return Margin;
            }
        }

        public class HorizonalMargin : IMetaContextOld, IMetaParallelPlacableOld
        {
            public float Width { get; set; }

            public AlignmentOption Alignment => AlignmentOption.Center;

            public HorizonalMargin(float margin)
            {
                Width = margin;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                width = new(cursor.x, cursor.x + Width);
                return 0f;
            }
            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                width = Width;
                return 0f;
            }
        }

        public class ScrollView : IMetaContextOld, IMetaParallelPlacableOld
        {
            private static Dictionary<string, float> distDic = new();
            public static void RemoveDistHistory(string tag) => distDic.Remove(tag);
            public static void UpdateDistHistory(string tag, float y) => distDic[tag] = y;
            public static float TryGetDistHistory(string tag) => distDic.TryGetValue(tag, out var val) ? val : 0f;
            public static Action GetDistHistoryUpdater(Func<float> y, string tag) => () => UpdateDistHistory(tag, y.Invoke());

            //ビュー内のコンテンツが後から変更できる
            public class InnerScreen
            {
                public bool IsValid => screen;

                private GameObject screen;
                private Vector2 innerSize;
                private float scrollViewSizeY;
                private Scroller scroller;
                private Collider2D scrollerCollider;
                public void SetContext(IMetaContextOld context)
                {
                    //子を削除
                    screen.ForEachChild((Il2CppSystem.Action<GameObject>)((obj) => GameObject.Destroy(obj)));

                    float height = context?.Generate(screen, new Vector2(-innerSize.x / 2f, innerSize.y / 2f), innerSize) ?? 0f;
                    scroller.SetBounds(new FloatRange(0, height - scrollViewSizeY), null);
                    scroller.ScrollRelative(Vector2.zero);

                    foreach (var button in screen.GetComponentsInChildren<PassiveButton>()) button.ClickMask = scrollerCollider;
                }

                public void SetStaticContext(IMetaParallelPlacableOld context)
                {
                    //子を削除
                    screen.ForEachChild((Il2CppSystem.Action<GameObject>)((obj) => GameObject.Destroy(obj)));

                    context?.Generate(screen, new Vector2(0f, 0f), out _);
                    scroller.SetBounds(new FloatRange(0f, 0f), null);
                    scroller.ScrollRelative(Vector2.zero);
                }

                public void SetLoadingContext() => SetStaticContext(new MetaContextOld.Text(new TextAttribute(TextAttribute.BoldAttr).EditFontSize(2.8f)) { TranslationKey = "ui.common.loading" });


                public InnerScreen(GameObject screen, Vector2 innerSize, Scroller scroller, Collider2D scrollerCollider, float scrollViewSizeY)
                {
                    this.screen = screen;
                    this.innerSize = innerSize;
                    this.scroller = scroller;
                    this.scrollerCollider = scrollerCollider;
                    this.scrollViewSizeY = scrollViewSizeY;
                }
            }

            public AlignmentOption Alignment { get; set; }
            public IMetaContextOld Inner = null;
            public Vector2 Size;
            public bool WithMask;
            public Reference<InnerScreen> InnerRef = null;
            public string ScrollerTag = null;
            public Action PostBuilder = null;

            public ScrollView(Vector2 size, IMetaContextOld inner, bool withMask = true)
            {
                this.Size = size;
                this.Inner = inner;
                this.WithMask = withMask;
            }

            public ScrollView(Vector2 size, Reference<InnerScreen> reference, bool withMask = true)
            {
                this.Size = size;
                this.InnerRef = reference;
                this.WithMask = withMask;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                var view = Helpers.CreateObject("ScrollView", screen.transform, new Vector3(center.x, center.y, -0.01f));
                var inner = Helpers.CreateObject("Inner", view.transform, new Vector3(0, 0, 0));
                var innerSize = Size - new Vector2(0.4f, 0f);

                if (WithMask)
                {
                    view.AddComponent<SortingGroup>();
                    var mask = Helpers.CreateObject<SpriteMask>("Mask", view.transform, new Vector3(0, 0, 0));
                    mask.sprite = VanillaAsset.FullScreenSprite;
                    mask.transform.localScale = innerSize;
                }

                float height = Inner?.Generate(inner, new Vector2(-innerSize.x / 2f, innerSize.y / 2f), innerSize) ?? 0f;
                var scroller = VanillaAsset.GenerateScroller(Size, view.transform, new Vector2(Size.x / 2 - 0.15f, 0f), inner.transform, new FloatRange(0, height - Size.y), Size.y);

                if (ScrollerTag != null && distDic.TryGetValue(ScrollerTag, out var val))
                    scroller.Inner.transform.localPosition = scroller.Inner.transform.localPosition +
                        new Vector3(0f, Mathf.Clamp(val + scroller.ContentYBounds.min, scroller.ContentYBounds.min, scroller.ContentYBounds.max), 0f);
                if (ScrollerTag != null)
                {
                    scroller.Inner.gameObject.AddComponent<ScriptBehaviour>().UpdateHandler += () => { distDic[ScrollerTag] = scroller.Inner.transform.localPosition.y - scroller.ContentYBounds.min; };
                }

                var hitBox = scroller.GetComponent<Collider2D>();
                foreach (var button in inner.GetComponentsInChildren<PassiveButton>()) button.ClickMask = hitBox;

                InnerRef?.Set(new(inner, innerSize, scroller, hitBox, Size.y));

                width = Size.x;

                PostBuilder?.Invoke();

                return Size.y;
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var view = Helpers.CreateObject("ScrollView", screen.transform, ReflectAlignment(Alignment, Size, cursor, size));
                var inner = Helpers.CreateObject("Inner", view.transform, new Vector3(0, 0, 0));
                var innerSize = Size - new Vector2(0.2f, 0f);

                if (WithMask)
                {
                    view.AddComponent<SortingGroup>();
                    var mask = Helpers.CreateObject<SpriteMask>("Mask", view.transform, new Vector3(0, 0, 0));
                    mask.sprite = VanillaAsset.FullScreenSprite;
                    mask.transform.localScale = innerSize;
                }

                float height = Inner?.Generate(inner, new Vector2(-innerSize.x / 2f, innerSize.y / 2f), innerSize) ?? 0f;
                var scroller = VanillaAsset.GenerateScroller(Size, view.transform, new Vector2(Size.x / 2 - 0.1f, 0f), inner.transform, new FloatRange(0, height - Size.y), Size.y);

                if (ScrollerTag != null && distDic.TryGetValue(ScrollerTag, out var val))
                {
                    scroller.Inner.transform.localPosition += new Vector3(0f, val, 0f);
                    scroller.ScrollRelative(Vector2.zero);
                }
                if (ScrollerTag != null)
                {
                    scroller.Inner.gameObject.AddComponent<ScriptBehaviour>().UpdateHandler += () => { distDic[ScrollerTag] = scroller.Inner.transform.localPosition.y - scroller.ContentYBounds.min; };
                }

                var hitBox = scroller.GetComponent<Collider2D>();
                foreach (var button in inner.GetComponentsInChildren<PassiveButton>()) button.ClickMask = hitBox;

                width = CalcWidth(Alignment, cursor, size, Size.x, -Size.x / 2f, Size.x / 2f);

                InnerRef?.Set(new(inner, innerSize, scroller, hitBox, Size.y));

                PostBuilder?.Invoke();

                return Size.y;
            }
        }

        public class TextInput : IMetaContextOld, IMetaParallelPlacableOld
        {

            public AlignmentOption Alignment { get; set; } = AlignmentOption.Left;
            private Vector2 fieldSize;
            private int maxLines;
            private float fontSize;
            private bool useSharpFieldFlag;
            public Reference<TextField> TextFieldRef;
            public Predicate<char> TextPredicate;
            public Action<TextField> PostBuilder;
            public string DefaultText = "";
            public string Hint = null;
            public bool WithMaskMaterial = false;

            public TextInput(int maxLines, float fontSize, Vector2 size, bool useSharpField = false)
            {
                this.maxLines = maxLines;
                this.fontSize = fontSize;
                fieldSize = size;
                useSharpFieldFlag = useSharpField;
            }

            private Vector2 ActualSize => fieldSize + new Vector2(0.15f, 0.15f);
            private void Generate(Transform screen, Vector2 center)
            {
                Vector2 actualSize = ActualSize;
                var obj = Helpers.CreateObject("TextField", screen, center);

                var field = Helpers.CreateObject<TextField>("Text", obj.transform, new Vector3(0, 0, -0.5f));
                field.SetSize(fieldSize, fontSize, maxLines);
                field.InputPredicate = TextPredicate;
                if (WithMaskMaterial) field.AsMaskedText();

                var background = Helpers.CreateObject<SpriteRenderer>("Background", obj.transform, Vector3.zero);
                background.sprite = useSharpFieldFlag ? Helpers.loadSpriteFromResources("TheOtherRoles.Resources.StatisticsBackground.png", 100f) : VanillaAsset.TextButtonSprite;
                background.drawMode = SpriteDrawMode.Sliced;
                background.tileMode = SpriteTileMode.Continuous;
                background.size = actualSize;
                background.sortingOrder = 5;
                if (WithMaskMaterial) background.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                var collider = background.gameObject.AddComponent<BoxCollider2D>();
                collider.size = actualSize;
                collider.isTrigger = true;
                var button = background.gameObject.SetUpButton(true, background);
                button.OnClick.AddListener((Action)(() => field.GainFocus()));
                TextFieldRef?.Set(field);
                if (DefaultText.Length > 0) field.SetText(DefaultText);
                if (Hint != null) field.SetHint(Hint!);

                PostBuilder?.Invoke(field);
            }

            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                Vector2 actualSize = ActualSize;
                Generate(screen.transform, ReflectAlignment(Alignment, actualSize, cursor, size));
                width = CalcWidth(Alignment, cursor, size, actualSize.x, -actualSize.x / 2f, actualSize.x / 2f);

                return actualSize.y + 0.05f;
            }
            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                Generate(screen.transform, center);
                width = ActualSize.x;

                return ActualSize.y + 0.05f;
            }
        }

        public class FramedContext : IMetaContextOld, IMetaParallelPlacableOld
        {
            public AlignmentOption Alignment => (context as IMetaContextOld)?.Alignment ?? AlignmentOption.Center;
            object context { get; set; }
            Vector2 extended { get; set; }
            public Color? HighlightColor { get; set; }
            public Action<SpriteRenderer> PostBuilder { get; set; }

            public FramedContext(IMetaContextOld context, Vector2 extended)
            {
                this.context = context;
                this.extended = extended;
            }

            public FramedContext(IMetaParallelPlacableOld context, Vector2 extended)
            {
                this.context = context;
                this.extended = extended;
            }


            public float Generate(GameObject screen, Vector2 cursor, Vector2 size, out (float min, float max) width)
            {
                var frame = Helpers.CreateObject("SizedFrame", screen.transform, new(0f, 0f, -0.5f));

                IMetaContextOld mc = (context as IMetaContextOld) ?? new MetaContextOld();


                float height = mc.Generate(frame, cursor + new Vector2(extended.x, -extended.y), size - new Vector2(extended.x, extended.y) * 2f, out width);
                if (HighlightColor != null)
                {
                    var backGround = Helpers.CreateSharpBackground(new Vector2(width.max - width.min, height) + extended * 1.8f, HighlightColor.Value, screen.transform);
                    backGround.transform.localPosition += new Vector3((width.min + width.max) * 0.5f, cursor.y - height * 0.5f - extended.y, 0.5f);
                    backGround.sortingOrder = 7;
                    PostBuilder?.Invoke(backGround);
                }
                width = (width.min - extended.x, width.max + extended.x);
                return extended.y * 2f + height;
            }

            public float Generate(GameObject screen, Vector2 center, out float width)
            {
                float height = 0f;
                var frame = Helpers.CreateObject("SizedFrame", screen.transform, new(center.x, center.y, -1f));
                width = 0f;
                if (context is IMetaParallelPlacableOld mpp)
                {
                    height = mpp.Generate(frame, Vector2.zero, out width);
                }

                if (HighlightColor != null)
                {
                    var backGround = Helpers.CreateSharpBackground(new Vector2(width + extended.x * 1.8f, height + extended.y * 1.8f), HighlightColor.Value, frame.transform);
                    backGround.transform.localPosition += new Vector3(0, 0, 0.5f);
                    backGround.sortingOrder = 7;
                    PostBuilder?.Invoke(backGround);
                }

                height += extended.y * 2f;
                width += extended.x * 2f;
                return height;
            }
        }
    }
}
