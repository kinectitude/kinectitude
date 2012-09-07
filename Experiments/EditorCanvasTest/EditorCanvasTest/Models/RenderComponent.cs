using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorCanvasTest.Models
{
    public class RenderComponent
    {
        public enum ShapeType
        {
            Rectangle,
            Ellipse
        }

        public ShapeType Shape
        {
            get;
            set;
        }

        public string Color
        {
            get;
            set;
        }

        public RenderComponent(ShapeType shape, string color)
        {
            Shape = shape;
            Color = color;
        }
    }
}
