using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.Presenters
{
    internal sealed class BasicEntityPresenter : EntityPresenter
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

        public BasicEntityPresenter(Entity entity) : base(entity) { }
    }
}
