using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOtherRoles.MetaContext
{
    public class TextAttribute
    {
        static public readonly TextAttribute TitleAttr = new()
        {
            Alignment = TMPro.TextAlignmentOptions.Left,
            Styles = TMPro.FontStyles.Bold,
            FontMaxSize = 3f,
            FontMinSize = 1f,
            FontSize = 3f
        };
        static public readonly TextAttribute ContentAttr = new()
        {
            Alignment = TMPro.TextAlignmentOptions.TopLeft,
            Styles = TMPro.FontStyles.Normal,
            FontMaxSize = 1.5f,
            FontMinSize = 0.5f,
            FontSize = 1.5f,
            Size = new Vector2(8f, 2f)
        };
        static public readonly TextAttribute NormalAttr = new()
        {
            Alignment = TMPro.TextAlignmentOptions.Center,
            Styles = TMPro.FontStyles.Normal,
            FontMaxSize = 2f,
            FontMinSize = 1f,
            FontSize = 1.8f,
            Size = new Vector2(1.7f, 0.3f)
        };
        static public readonly TextAttribute NormalAttrLeft = new(NormalAttr) { Alignment = TMPro.TextAlignmentOptions.Left };

        static public readonly TextAttribute BoldAttr = new()
        {
            Alignment = TMPro.TextAlignmentOptions.Center,
            Styles = TMPro.FontStyles.Bold,
            FontMaxSize = 2f,
            FontMinSize = 1f,
            FontSize = 1.8f,
            Size = new Vector2(1.7f, 0.3f)
        };
        static public readonly TextAttribute BoldAttrLeft = new(BoldAttr) { Alignment = TMPro.TextAlignmentOptions.Left };

        public Color Color { get; set; } = Color.white;
        public TMPro.TextAlignmentOptions Alignment { get; set; } = TMPro.TextAlignmentOptions.Center;
        public TMPro.FontStyles Styles { get; set; } = TMPro.FontStyles.Normal;
        public Material FontMaterial { get; set; } = null;
        public TMPro.TMP_FontAsset Font { get; set; } = null;
        public float FontMinSize { get; set; } = 0.6f;
        public float FontMaxSize { get; set; } = 2f;
        public float FontSize { get; set; } = 1.5f;
        public bool AllowAutoSizing { get; set; } = true;
        public Vector2 Size { get; set; } = new Vector2(3f, 0.5f);
        public TextAttribute EditFontSize(float size) => EditFontSize(size, size, size);
        public TextAttribute EditFontSize(float size, float min, float max)
        {
            FontMaxSize = max;
            FontMinSize = min;
            FontSize = size;
            return this;
        }

        public void Reflect(TMPro.TextMeshPro text)
        {
            text.color = Color;
            text.alignment = Alignment;
            text.fontStyle = Styles;
            text.fontSize = FontSize;
            text.fontSizeMin = FontMinSize;
            text.fontSizeMax = FontMaxSize;
            text.enableAutoSizing = AllowAutoSizing;
            text.rectTransform.sizeDelta = Size;
            text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            if (Font != null) text.font = Font;
            if (FontMaterial != null) text.fontMaterial = FontMaterial;
        }

        public TextAttribute() { }
        public TextAttribute(TextAttribute orig)
        {
            Color = orig.Color;
            Alignment = orig.Alignment;
            Styles = orig.Styles;
            FontSize = orig.FontSize;
            FontMinSize = orig.FontMinSize;
            FontMaxSize = orig.FontMaxSize;
            AllowAutoSizing = orig.AllowAutoSizing;
            Size = orig.Size;
            FontMaterial = orig.FontMaterial;
        }

        public TextAttribute AlterColor(Color color) => new(this) { Color = color };
        public TextAttribute AlterAutoSizing(bool allowAutoSizing) => new(this) { AllowAutoSizing = allowAutoSizing };
    }
}
