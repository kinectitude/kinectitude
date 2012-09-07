using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.ViewModels
{
    internal class TextEntityViewModel : EntityViewModel
    {
        public string Value
        {
            get { return GetValue<string>(typeof(TextRenderComponent), "Value"); }
            set
            {
                SetValue(typeof(TextRenderComponent), "Value", value);
                NotifyPropertyChanged("Value");
            }
        }

        public float FontSize
        {
            get { return GetValue<float>(typeof(TextRenderComponent), "FontSize"); }
            set
            {
                SetValue(typeof(TextRenderComponent), "FontSize", value);
                NotifyPropertyChanged("FontSize");
            }
        }

        public string FontColor
        {
            get { return GetValue<string>(typeof(TextRenderComponent), "FontColor"); }
            set
            {
                SetValue(typeof(TextRenderComponent), "FontColor", value);
                NotifyPropertyChanged("FontColor");
            }
        }

        public TextEntityViewModel(Entity entity) : base(entity) { }
    }
}
