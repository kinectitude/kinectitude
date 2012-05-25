using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;

namespace Module
{
    public class RenderComponent : Component
    {
        public enum Shapes
        {
            Rectangle,
            Ellipse
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public Shapes Shape { get; set; }

        public RenderComponent() { }

        public override void OnUpdate(double frameDelta)
        {
            throw new NotImplementedException();
        }
    }
}
