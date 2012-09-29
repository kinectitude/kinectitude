using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.Presenters
{
    internal class TextEntityPresenter : EntityPresenter
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

        public TextEntityPresenter(Entity entity) : base(entity) { }
    }
}
