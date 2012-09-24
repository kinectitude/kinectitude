using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models;
using Kinectitude.Render;
using System.IO;

namespace Kinectitude.Editor.ViewModels
{
    internal class SpriteEntityVisualViewModel : EntityVisualViewModel
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

        public SpriteEntityVisualViewModel(Entity entity) : base(entity) { }
    }
}
