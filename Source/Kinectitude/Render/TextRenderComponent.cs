using System.Drawing;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using FontStyle = SlimDX.DirectWrite.FontStyle;
using RenderTarget = SlimDX.Direct2D.RenderTarget;

namespace Kinectitude.Render
{
    [Plugin("Text Render Component", "")]
    public class TextRenderComponent : Component, IRender
    {
        private Color4 renderColor;
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
            set { renderColor = RenderService.ColorFromString(value); }
        }

        public TextRenderComponent() { }

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

            textFormat = renderManager.DirectWriteFactory.CreateTextFormat(FontFamily, FontWeight, FontStyle, FontStretch, FontSize, "en-us");
            textFormat.FlowDirection = FlowDirection.TopToBottom;
            textFormat.IncrementalTabStop = textFormat.FontSize * 4.0f;
            textFormat.ParagraphAlignment = ParagraphAlignment.Near;
            textFormat.ReadingDirection = ReadingDirection.LeftToRight;
            textFormat.TextAlignment = TextAlignment.Leading;
            textFormat.WordWrapping = WordWrapping.NoWrap;

            brush = renderManager.CreateSolidColorBrush(renderColor);

            transformComponent = GetComponent<TransformComponent>();
            transformComponent.SubscribeToX(this, OnTransformChanged);
            transformComponent.SubscribeToY(this, OnTransformChanged);
            transformComponent.SubscribeToWidth(this, OnTransformChanged);
            transformComponent.SubscribeToHeight(this, OnTransformChanged);

            layoutRectangle = new RectangleF()
            {
                X = transformComponent.X,
                Y = transformComponent.Y,
                Width = transformComponent.Width,
                Height = transformComponent.Height
            };
        }

        public void OnTransformChanged()
        {
            layoutRectangle.X = transformComponent.X;
            layoutRectangle.Y = transformComponent.Y;
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
