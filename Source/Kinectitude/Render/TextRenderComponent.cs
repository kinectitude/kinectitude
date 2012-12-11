﻿using System.Drawing;
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
        [PluginProperty("Value", "")]
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
        [PluginProperty("Font Family", "")]
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                if (value != fontFamily)
                {
                    fontFamily = value;
                    Change("FontFamily");
                }
            }
        }

        private FontWeight fontWeight;
        [PluginProperty("Font Weight", "")]
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (value != fontWeight)
                {
                    fontWeight = value;
                    Change("FontWeight");
                }
            }
        }

        public FontStyle fontStyle;
        [PluginProperty("Font Style", "")]
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (value != fontStyle)
                {
                    fontStyle = value;
                    Change("FontStyle");
                }
            }
        }

        private FontStretch fontStretch;
        [PluginProperty("Font Stretch", "")]
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                if (value != fontStretch)
                {
                    fontStretch = value;
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
                    Change("FontSize");
                }
            }
        }

        private string fontColor;
        [PluginProperty("Font Color", "", "black")]
        public string FontColor
        {
            get { return fontColor; }
            set
            {
                if (value != fontColor)
                {
                    fontColor = value;
                    Change("FontColor");
                }
            }
        }

        private string locale;
        [PluginProperty("Locale", "", "en-us")]
        public string Locale
        {
            get { return locale; }
            set
            {
                if (value != locale)
                {
                    locale = value;
                    Change("Locale");
                }
            }
        }

        private FlowDirection flowDirection;
        [PluginProperty("Flow Direction", "")]
        public FlowDirection FlowDirection
        {
            get { return flowDirection; }
            set
            {
                if (value != flowDirection)
                {
                    flowDirection = value;
                    Change("FlowDirection");
                }
            }
        }

        private int tabSize;
        [PluginProperty("Tab Size", "", 4)]
        public int TabSize
        {
            get { return tabSize; }
            set
            {
                if (value != tabSize)
                {
                    tabSize = value;
                    Change("TabSize");
                }
            }
        }

        private ParagraphAlignment paragraphAlignment;
        [PluginProperty("Paragraph Alignment", "")]
        public ParagraphAlignment ParagraphAlignment
        {
            get { return paragraphAlignment; }
            set
            {
                if (value != paragraphAlignment)
                {
                    paragraphAlignment = value;
                    Change("ParagraphAlignment");
                }
            }
        }

        private ReadingDirection readingDirection;
        [PluginProperty("Reading Direction", "")]
        public ReadingDirection ReadingDirection
        {
            get { return readingDirection; }
            set
            {
                if (value != readingDirection)
                {
                    readingDirection = value;
                    Change("ReadingDirection");
                }
            }
        }

        private TextAlignment textAlignment;
        [PluginProperty("Text Alignment", "")]
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                if (value != textAlignment)
                {
                    textAlignment = value;
                    Change("TextAlignment");
                }
            }
        }

        private WordWrapping wordWrapping;
        [PluginProperty("Word Wrapping", "")]
        public WordWrapping WordWrapping
        {
            get { return wordWrapping; }
            set
            {
                if (value != wordWrapping)
                {
                    wordWrapping = value;
                    Change("WordWrapping");
                }
            }
        }

        private float offsetX;
        [PluginProperty("Horizontal Offset", "")]
        public float OffsetX
        {
            get { return offsetX; }
            set
            {
                if (value != offsetX)
                {
                    offsetX = value;
                    Change("OffsetX");
                }
            }
        }

        private float offsetY;
        [PluginProperty("Vertical Offset", "")]
        public float OffsetY
        {
            get { return offsetY; }
            set
            {
                if (value != offsetY)
                {
                    offsetY = value;
                    Change("OffsetY");
                }
            }
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
            Value = "";
        }

        protected override void OnRender(RenderTarget renderTarget)
        {
            OnReady();

            layoutRectangle.X = transformComponent.X + OffsetX;
            layoutRectangle.Y = transformComponent.Y + OffsetY;
            layoutRectangle.Width = transformComponent.Width;
            layoutRectangle.Height = transformComponent.Height;

            renderTarget.DrawText(Value, textFormat, layoutRectangle, brush);
        }

        protected override void OnReady()
        {
            textFormat = renderManager.DirectWriteFactory.CreateTextFormat(FontFamily, FontWeight, FontStyle, FontStretch, FontSize, Locale);
            textFormat.FlowDirection = FlowDirection;
            textFormat.IncrementalTabStop = textFormat.FontSize * TabSize;
            textFormat.ParagraphAlignment = ParagraphAlignment;
            textFormat.ReadingDirection = ReadingDirection;
            textFormat.TextAlignment = TextAlignment;
            textFormat.WordWrapping = WordWrapping;
            brush = renderManager.GetSolidColorBrush(FontColor, Opacity);
            layoutRectangle = new RectangleF();
        }

        protected override void OnDestroy()
        {
            textFormat.Dispose();
        }
    }
}
