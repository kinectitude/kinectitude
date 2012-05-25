using System;
using System.Collections.Generic;
using System.Drawing;
using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
//using Kinectitude.Core.Exceptions;
using SlimDX;
using SlimDX.DirectWrite;

namespace Kinectitude.Render
{
    [Plugin("Text Render Component", "")]
    public class TextRenderComponent : Component, IRender
    {
        private Color4 renderColor;
        private TextFormat textFormat;
        private TransformComponent tc;

        [Plugin("Value", "")]
        public string Value
        {
            get;
            set;
        }

        [Plugin("Font Color", "")]
        public string FontColor
        {
            set
            {
                System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value);
                renderColor = new Color4((float)color.R / 255.0f, (float)color.G / 255.0f, (float)color.B / 255.0f);
            }
        }

        public TextRenderComponent() : base() { }

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
            renderTarget.DrawText(Value, textFormat, new RectangleF(tc.X, tc.Y, 0.0f, 0.0f), 
                new SlimDX.Direct2D.SolidColorBrush(renderTarget, renderColor));
        }

        public override void Ready()
        {
            tc = GetComponent<TransformComponent>();
            //TODO this will be done automatically
            if (null == tc)
            {
                List<Type> missing = new List<Type>();
                missing.Add(typeof(TransformComponent));
                //throw MissingRequirementsException.MissingRequirement(this, missing);
            }
        }
    }
}
