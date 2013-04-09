//-----------------------------------------------------------------------
// <copyright file="SpriteEntityVisual.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Windows.Media.Imaging;
using Kinectitude.Editor.Models;
using Kinectitude.Render;
using System.Windows.Media;
using System;
using System.Collections.Generic;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class SpriteEntityVisual : EntityVisual
    {
        private static readonly ImageSourceConverter Converter = new ImageSourceConverter();
        private static readonly Dictionary<string, BitmapImage> Images = new Dictionary<string, BitmapImage>();
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
                    BitmapImage image;
                    Images.TryGetValue(path, out image);

                    if (null == image)
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = new Uri(path);
                        image.EndInit();
                        Images[path] = image;
                    }

                    return image;
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
