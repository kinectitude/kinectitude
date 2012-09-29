using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Presenters
{
    internal class SpriteEntityPresenter : EntityPresenter
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

        public SpriteEntityPresenter(Entity entity) : base(entity) { }
    }
}
