using System.IO;
using System.Windows.Media.Imaging;
using Kinectitude.Editor.Models;
using Kinectitude.Render;
using System.Windows.Media;
using System;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class SpriteEntityVisual : EntityVisual
    {
        private static readonly ImageSourceConverter Converter = new ImageSourceConverter();
        private static readonly BitmapImage DefaultImage;

        static SpriteEntityVisual()
        {
            DefaultImage = new BitmapImage();
            DefaultImage.BeginInit();
            DefaultImage.UriSource = new Uri("pack://application:,,,/Kinectitude.Editor;component/Icons/default_image.png");
            DefaultImage.EndInit();
        }

        public ImageSource Image
        {
            get
            {
                var path = Path.Combine(
                    Workspace.Instance.Project.Location,
                    Workspace.Instance.Project.GameRoot,
                    "Assets",
                    GetStringValue<ImageRenderComponent>("Image")
                );

                if (File.Exists(path))
                {
                    return (ImageSource)Converter.ConvertFromString(path);
                }

                return DefaultImage;
            }
        }

        [DependsOn("Image")]
        public bool HasImage
        {
            get { return !string.IsNullOrWhiteSpace(GetStringValue<ImageRenderComponent>("Image")); }
        }

        public Stretch Stretched
        {
            get { return GetBoolValue<ImageRenderComponent>("Stretched") ? Stretch.Fill : Stretch.None; }
        }

        public SpriteEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
