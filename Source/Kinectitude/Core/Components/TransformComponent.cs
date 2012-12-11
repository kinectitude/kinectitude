using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Components
{
    [Plugin("Transform Component", "")]
    public class TransformComponent : Component
    {
        private float x;
        [PluginProperty("X Position", "", 0)]
        public float X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                    Change("X");
                }
            }
        }

        private float y;
        [PluginProperty("Y Position", "", 0)]
        public float Y 
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    Change("Y");
                }
            }
        }

        private int width;
        [PluginProperty("Width", "", 0)]
        public int Width 
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    Change("Width");
                }
            }
        }

        private int height;
        [PluginProperty("Height", "", 0)]
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    Change("Height");
                }
            }
        }

        private float rotation;
        [PluginProperty("Rotation", "", 0)]
        public float Rotation 
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    Change("Rotation");
                }
            }
        }

        public override void Destroy() { }
    }
}
