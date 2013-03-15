using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.ComponentInterfaces;

namespace Kinectitude.Core.Components
{
    [Plugin("Transform Component", "")]
    public class TransformComponent : Component
    {
        private sealed class DefaultTransform : ITransform
        {
            private float x;
            private float y;
            private float width;
            private float height;
            private float rotation;

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

            public float Width
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

            public float Height
            {
                get { return height; }
                set
                {
                    if (height != value)
                    {
                        height = value;
                        Change("Heiht");
                    }
                }
            }

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

            public event ChangeEventHandler Changed;

            private void Change(string property)
            {
                if (null != Changed)
                {
                    Changed(property);
                }
            }
        }

        private ITransform transform;

        public ITransform Transform
        {
            get { return transform; }
            set
            {
                if (transform != value)
                {
                    if (null != transform)
                    {
                        transform.Changed -= OnTransformChanged;
                    }

                    if (null == value)
                    {
                        value = new DefaultTransform();
                    }

                    transform = value;
                    transform.Changed += OnTransformChanged;
                }
            }
        }

        [PluginProperty("X Position", "", 0)]
        public float X
        {
            get { return transform.X; }
            set { transform.X = value; }
        }

        [PluginProperty("Y Position", "", 0)]
        public float Y 
        {
            get { return transform.Y; }
            set { transform.Y = value; }
        }

        [PluginProperty("Width", "", 0)]
        public float Width 
        {
            get { return transform.Width; }
            set { transform.Width = value; }
        }

        [PluginProperty("Height", "", 0)]
        public float Height
        {
            get { return transform.Height; }
            set { transform.Height = value; }
        }

        [PluginProperty("Rotation", "", 0)]
        public float Rotation 
        {
            get { return transform.Rotation; }
            set { transform.Rotation = value; }
        }

        public TransformComponent()
        {
            Transform = new DefaultTransform();
        }

        private void OnTransformChanged(string property)
        {
            Change(property);
        }

        public override void Destroy() { }
    }
}
