﻿using System.Collections.Generic;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.ComponentInterfaces;

public delegate void ChangeDelegate();

namespace Kinectitude.Core.Components
{
    [Plugin("Transform Component", "")]
    [Provides(typeof(TransformComponent))]
    public class TransformComponent : Component
    {
        private float x;
        [Plugin("X Position", "")]
        public float X {
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
        [Plugin("Y Position", "")]
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
        [Plugin("Width", "")]
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
        [Plugin("Height", "")]
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
        [Plugin("Rotation", "")]
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
