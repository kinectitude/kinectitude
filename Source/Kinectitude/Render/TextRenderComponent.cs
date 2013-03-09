using System.Drawing;
using Kinectitude.Core.Attributes;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using FontStyle = SlimDX.DirectWrite.FontStyle;
using RenderTarget = SlimDX.Direct2D.RenderTarget;

namespace Kinectitude.Render
{
    [Plugin("Text Render Component", "")]
    public class TextRenderComponent : BaseRenderComponent
    {
        private SolidColorBrush brush;
        private TextFormat textFormat;
        private RectangleF layoutRectangle;


        private string _value = "";
        [PluginProperty("Value", "", "")]
        public string Value
        {
            get { return _value ?? ""; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    Change("Value");
                }
            }
        }

        private string fontFamily;
        [PluginProperty("Font Family", "", "Arial")]
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                if (value != fontFamily)
                {
                    fontFamily = value;
                    UpdateTextFormat();
                    Change("FontFamily");
                }
            }
        }

        private FontWeight fontWeight;
        [PluginProperty("Font Weight", "", FontWeight.Normal)]
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (value != fontWeight)
                {
                    fontWeight = value;
                    UpdateTextFormat();
                    Change("FontWeight");
                }
            }
        }

        public FontStyle fontStyle;
        [PluginProperty("Font Style", "", FontStyle.Normal)]
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (value != fontStyle)
                {
                    fontStyle = value;
                    UpdateTextFormat();
                    Change("FontStyle");
                }
            }
        }

        private FontStretch fontStretch;
        [PluginProperty("Font Stretch", "", FontStretch.Normal)]
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                if (value != fontStretch)
                {
                    fontStretch = value;
                    UpdateTextFormat();
                    Change("FontStretch");
                }
            }
        }

        private float fontSize;
        [PluginProperty("Font Size", "", 12)]
        public float FontSize
        {
            get { return fontSize; }
            set
            {
                if (value != fontSize)
                {
                    fontSize = value;
                    UpdateTextFormat();
                    Change("FontSize");
                }
            }
        }

        private string fontColor;
        [PluginProperty("Font Color", "", "Black")]
        public string FontColor
        {
            get { return fontColor; }
            set
            {
                if (value != fontColor)
                {
                    fontColor = value;
                    UpdateBrush();
                    Change("FontColor");
                }
            }
        }

        private TextAlignment textAlignment;
        [PluginProperty("Text Alignment", "", TextAlignment.Leading)]
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                if (value != textAlignment)
                {
                    textAlignment = value;
                    UpdateTextFormat();
                    Change("TextAlignment");
                }
            }
        }

        public TextRenderComponent()
        {
            layoutRectangle = new RectangleF();
        }

        protected override void OnRender(RenderTarget renderTarget)
        {
            layoutRectangle.X = transformComponent.X;
            layoutRectangle.Y = transformComponent.Y;
            layoutRectangle.Width = transformComponent.Width;
            layoutRectangle.Height = transformComponent.Height;

            renderTarget.DrawText(Value, textFormat, layoutRectangle, brush);
        }

        private void UpdateTextFormat()
        {
            if (null != textFormat)
            {
                textFormat.Dispose();
            }

            if (null != renderManager)
            {
                textFormat = renderManager.DirectWriteFactory.CreateTextFormat(FontFamily, FontWeight, FontStyle, FontStretch, FontSize, "en-us");
                textFormat.TextAlignment = TextAlignment;
                textFormat.FlowDirection = FlowDirection.TopToBottom;
                textFormat.IncrementalTabStop = textFormat.FontSize * 4;
                textFormat.ParagraphAlignment = ParagraphAlignment.Near;
                textFormat.ReadingDirection = ReadingDirection.LeftToRight;
                textFormat.WordWrapping = WordWrapping.NoWrap;
            }
        }

        private void UpdateBrush()
        {
            if (null != renderManager)
            {
                brush = renderManager.GetSolidColorBrush(FontColor, Opacity);
            }
        }

        protected override void OnReady()
        {
            UpdateTextFormat();
            UpdateBrush();
        }

        protected override void OnDestroy()
        {
            textFormat.Dispose();
        }
    }
}
