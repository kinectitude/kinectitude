using System.IO;
using System.Windows.Media.Imaging;
using Kinectitude.Editor.Models;
using Kinectitude.Render;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class SpriteEntityVisual : EntityVisual
    {
        public string Image
        {
            get { return GetValue<ImageRenderComponent, string>("Image"); }
            set { SetValue<ImageRenderComponent, string>("Image", value); }
        }

        public SpriteEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
