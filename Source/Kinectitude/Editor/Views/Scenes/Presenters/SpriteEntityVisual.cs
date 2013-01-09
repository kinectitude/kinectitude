using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class SpriteEntityVisual : EntityVisual
    {
        public string File
        {
            get
            {
                //string assetName = GetValue<string>(typeof(ImageRenderComponent), "Image");
                //return Path.Combine(AssetDirectory, assetName);
                return null;
            }
        }

        public SpriteEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
