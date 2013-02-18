using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class TextEntityVisual : EntityVisual
    {
        public string Value
        {
            get { return GetStringValue<TextRenderComponent>("Value"); }
            set { SetValue<TextRenderComponent, string>("Value", value); }
        }

        public float FontSize
        {
            get { return GetFloatValue<TextRenderComponent>("FontSize"); }
            set { SetValue<TextRenderComponent, float>("FontSize", value); }
        }

        public string FontColor
        {
            get { return GetStringValue<TextRenderComponent>("FontColor"); }
            set { SetValue<TextRenderComponent, string>("FontColor", value); }
        }

        public TextEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
