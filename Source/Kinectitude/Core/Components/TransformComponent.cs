using System.Collections.Generic;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

public delegate void ChangeDelegate();

namespace Kinectitude.Core.Components
{
    [Plugin("Transform Component", "")]
    public class TransformComponent : Component
    {
        private float x;
        [Plugin("X Position", "")]
        public float X { get; set; }

        [Plugin("Y Position", "")]
        public float Y { get; set; }

        private int width;
        [Plugin("Width", "")]
        public int Width { get; set; }

        [Plugin("Height", "")]
        public int Height { get; set; }

        [Plugin("Rotation", "")]
        public float Rotation { get; set; }

        public override void Destroy() { }
    }
}
