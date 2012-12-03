using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

public delegate void ChangeDelegate();

namespace Kinectitude.Core.Components
{
    [Plugin("Transform Component", "")]
    [Provides(typeof(TransformComponent))]
    public class TransformComponent : Component
    {
        private float x;
        [PluginProperty("X Position", "")]
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
        [PluginProperty("Y Position", "")]
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
        [PluginProperty("Width", "")]
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
        [PluginProperty("Height", "")]
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
        [PluginProperty("Rotation", "")]
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
