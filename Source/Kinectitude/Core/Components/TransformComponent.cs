﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Managers;

public delegate void ChangeDelegate();

namespace Kinectitude.Core.Components
{
    public class TransformComponent:Component
    {
        private List<ChangeDelegate> notifyXUpdate = new List<ChangeDelegate>();
        private List<ChangeDelegate> notifyYUpdate = new List<ChangeDelegate>();
        private List<ChangeDelegate> notifyWidthUpdate = new List<ChangeDelegate>();
        private List<ChangeDelegate> notifyHeightUpdate = new List<ChangeDelegate>();

        private Dictionary<ChangeDelegate, Component> creators = new Dictionary<ChangeDelegate, Component>();

        private float x;
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                foreach (ChangeDelegate changeDelegate in notifyXUpdate)
                {
                    changeDelegate();
                }
            }
        }
        public void setX(Component avoid, float x)
        {
            this.x = x;
            foreach (ChangeDelegate changeDelegate in notifyXUpdate)
            {
                if (creators[changeDelegate] != avoid)
                {
                    changeDelegate();
                }
            }
        }

        private float y;
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                foreach (ChangeDelegate changeDelegate in notifyYUpdate)
                {
                    changeDelegate();
                }
            }
        }
        public void setY(Component avoid, float y)
        {
            this.y = y;
            foreach (ChangeDelegate changeDelegate in notifyYUpdate)
            {
                if (creators[changeDelegate] != avoid)
                {
                    changeDelegate();
                }
            }
        }

        private int width;
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                foreach (ChangeDelegate changeDelegate in notifyWidthUpdate)
                {
                    changeDelegate();
                }
            }
        }
        public void setWidth(Component avoid, int width)
        {
            this.width = width;
            foreach (ChangeDelegate changeDelegate in notifyWidthUpdate)
            {
                if (creators[changeDelegate] != avoid)
                {
                    changeDelegate();
                }
            }
        }

        private int height;
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                foreach (ChangeDelegate changeDelegate in notifyWidthUpdate)
                {
                    changeDelegate();
                }
            }
        }
        public void setHeight(Component avoid, int height)
        {
            this.height = height;
            foreach (ChangeDelegate changeDelegate in notifyHeightUpdate)
            {
                if (creators[changeDelegate] != avoid)
                {
                    changeDelegate();
                }
            }
        }

        public TransformComponent(Entity entity) : base(entity) { }

        public override Type ManagerType()
        {
            return typeof(TimeManager);
        }

        public void SubscribeToX(Component component, ChangeDelegate changeDelegate)
        {
            creators[changeDelegate] =  component;
            notifyXUpdate.Add(changeDelegate);
        }

        public void SubscribeToY(Component component, ChangeDelegate changeDelegate)
        {
            creators[changeDelegate] =  component;
            notifyYUpdate.Add(changeDelegate);
        }

        public void SubscribeToWidht(Component component, ChangeDelegate changeDelegate)
        {
            creators[changeDelegate] = component;
            notifyWidthUpdate.Add(changeDelegate);
        }

        public void SubscribeToHeight(Component component, ChangeDelegate changeDelegate)
        {
            creators[changeDelegate] = component;
            notifyHeightUpdate.Add(changeDelegate);
        }
    }
}