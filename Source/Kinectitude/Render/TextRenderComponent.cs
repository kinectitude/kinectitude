using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using FontStyle = SlimDX.DirectWrite.FontStyle;
using RenderTarget = SlimDX.Direct2D.RenderTarget;

namespace Kinectitude.Render
{
    [Plugin("Text Render Component", "")]
    public class TextRenderComponent : Component, IRender
    {
        private SolidColorBrush brush;
        private TextFormat textFormat;
        private RectangleF layoutRectangle;
        private TransformComponent transformComponent;
        private RenderManager renderManager;

        [Plugin("Value", "")]
        public string Value
        {
            get;
            set;
        }

        [Plugin("Font Family", "")]
        public string FontFamily
        {
            get;
            set;
        }

        [Plugin("Font Weight", "")]
        public FontWeight FontWeight
        {
            get;
            set;
        }

        [Plugin("Font Style", "")]
        public FontStyle FontStyle
        {
            get;
            set;
        }

        [Plugin("Font Stretch", "")]
        public FontStretch FontStretch
        {
            get;
            set;
        }

        [Plugin("Font Size", "")]
        public float FontSize
        {
            get;
            set;
        }

        [Plugin("Font Color", "")]
        public string FontColor
        {
            get;
            set;
        }

        [Plugin("Locale", "")]
        public string Locale
        {
            get;
            set;
        }

        [Plugin("Flow Direction", "")]
        public FlowDirection FlowDirection
        {
            get;
            set;
        }

        [Plugin("Tab Size", "")]
        public int TabSize
        {
            get;
            set;
        }

        [Plugin("Paragraph Alignment", "")]
        public ParagraphAlignment ParagraphAlignment
        {
            get;
            set;
        }

        [Plugin("Reading Direction", "")]
        public ReadingDirection ReadingDirection
        {
            get;
            set;
        }

        [Plugin("Text Alignment", "")]
        public TextAlignment TextAlignment
        {
            get;
            set;
        }

        [Plugin("Word Wrapping", "")]
        public WordWrapping WordWrapping
        {
            get;
            set;
        }

        [Plugin("Horizontal Offset", "")]
        public float OffsetX
        {
            get;
            set;
        }

        [Plugin("Vertical Offset", "")]
        public float OffsetY
        {
            get;
            set;
        }

        [Plugin("Opacity", "")]
        public float Opacity
        {
            get;
            set;
        }

        public TextRenderComponent()
        {
            FontFamily = "Arial";
            FontWeight = FontWeight.Normal;
            FontStyle = FontStyle.Normal;
            FontStretch = FontStretch.Normal;
            FontSize = 36.0f;
            Locale = "en-us";
            FlowDirection = FlowDirection.TopToBottom;
            TabSize = 4;
            ParagraphAlignment = ParagraphAlignment.Near;
            ReadingDirection = ReadingDirection.LeftToRight;
            TextAlignment = TextAlignment.Leading;
            WordWrapping = WordWrapping.NoWrap;
            Opacity = 1.0f;
        }

        public void OnSetTextAction(SetTextAction action)
        {
            Value = action.Value.GetValue();
        }

        public void Render(RenderTarget renderTarget)
        {
            renderTarget.DrawText(Value, textFormat, layoutRectangle, brush);
        }

        public override void Ready()
        {
            renderManager = GetManager<RenderManager>();
            renderManager.Add(this);

            textFormat = renderManager.DirectWriteFactory.CreateTextFormat(FontFamily, FontWeight, FontStyle, FontStretch, FontSize, Locale);
            textFormat.FlowDirection = FlowDirection;
            textFormat.IncrementalTabStop = textFormat.FontSize * TabSize;
            textFormat.ParagraphAlignment = ParagraphAlignment;
            textFormat.ReadingDirection = ReadingDirection;
            textFormat.TextAlignment = TextAlignment;
            textFormat.WordWrapping = WordWrapping;

            brush = renderManager.GetSolidColorBrush(FontColor, Opacity);

            transformComponent = GetComponent<TransformComponent>();
            transformComponent.SubscribeToX(this, UpdateTransform);
            transformComponent.SubscribeToY(this, UpdateTransform);
            transformComponent.SubscribeToWidth(this, UpdateTransform);
            transformComponent.SubscribeToHeight(this, UpdateTransform);

            layoutRectangle = new RectangleF();
            UpdateTransform();
        }

        public void UpdateTransform()
        {
            layoutRectangle.X = transformComponent.X + OffsetX;
            layoutRectangle.Y = transformComponent.Y + OffsetY;
            layoutRectangle.Width = transformComponent.Width;
            layoutRectangle.Height = transformComponent.Height;
        }

        public override void Destroy()
        {
            textFormat.Dispose();
            renderManager.Remove(this);
        }
    }
}
