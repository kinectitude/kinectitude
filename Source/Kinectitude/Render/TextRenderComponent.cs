using System;
using System.Windows.Media;
using Kinectitude.Core;
using Kinectitude.Attributes;
using SlimDX;
using SlimDX.DirectWrite;
using System.Drawing;
using FontStyle = SlimDX.DirectWrite.FontStyle;

namespace Kinectitude.Render
{
    [Plugin("Text Render Component", "")]
    public class TextRenderComponent : Component, IRender
    {
        private Color4 renderColor;
        private TextFormat textFormat;

        [Plugin("Value", "")]
        public string Value
        {
            get;
            set;
        }

        [Plugin("Font Color", "")]
        public string Fontcolor
        {
            set
            {
                System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value);
                renderColor = new Color4((float)color.R / 255.0f, (float)color.G / 255.0f, (float)color.B / 255.0f);
            }
        }

        private double X
        {
            get { return double.Parse(Entity["x"]); }
        }

        private double Y
        {
            get { return double.Parse(Entity["y"]); }
        }

        public TextRenderComponent(Entity entity) : base(entity) { }

        public void OnSetTextAction(SetTextAction action)
        {
            Value = action.Value.GetValue();
        }

        public override Type ManagerType()
        {
            return typeof(RenderManager);
        }

        public void Initialize(RenderManager manager)
        {
            textFormat = manager.TextFormat;
        }

        public void Render(SlimDX.Direct2D.RenderTarget renderTarget)
        {
            renderTarget.DrawText(Value, textFormat, new RectangleF((float)X, (float)Y, 0.0f, 0.0f), new SlimDX.Direct2D.SolidColorBrush(renderTarget, renderColor));
        }
    }
}
