using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Components;
using Kinectitude.Render;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.ViewModels;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class BasicEntityVisualViewModel : EntityVisualViewModel
    {
        public string Shape
        {
            get { return GetValue<string>(typeof(RenderComponent), "Shape"); }
            set
            {
                SetValue(typeof(RenderComponent), "Shape", value);
                NotifyPropertyChanged("Shape");
            }
        }

        public string FillColor
        {
            get { return GetValue<string>(typeof(RenderComponent), "FillColor"); }
            set
            {
                SetValue(typeof(RenderComponent), "FillColor", value);
                NotifyPropertyChanged("FillColor");
            }
        }

        public float LineThickness
        {
            get { return GetValue<float>(typeof(RenderComponent), "LineThickness"); }
            set
            {
                SetValue(typeof(RenderComponent), "LineThickness", value);
                NotifyPropertyChanged("LineThickness");
            }
        }

        public string LineColor
        {
            get { return GetValue<string>(typeof(RenderComponent), "LineColor"); }
            set
            {
                SetValue(typeof(RenderComponent), "LineColor", value);
                NotifyPropertyChanged("LineColor");
            }
        }

        public bool IsRectangular
        {
            get { return !IsElliptical; }   
        }

        public bool IsElliptical
        {
            get
            {
                string shape = GetValue<string>(typeof(RenderComponent), "Shape");
                return shape == "Ellipse";
            }
        }

        public BasicEntityVisualViewModel(Entity entity) : base(entity) { }
    }
}
