//-----------------------------------------------------------------------
// <copyright file="EntityVisual.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Notifications;
using System;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class EntityVisual : EntityBase
    {
        protected static readonly BrushConverter BrushConverter = new BrushConverter();

        private readonly EntityPresenter presenter;
        private Component render;

        public EntityPresenter Presenter
        {
            get { return presenter; }
        }

        protected Component RenderComponent
        {
            get { return render; }
        }

        protected EntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(entity)
        {
            this.presenter = presenter;
            this.render = render;

            if (null != render)
            {
                render.LocalPropertyChanged += OnPropertyChanged;
            }

            entity.AddHandler<EffectiveValueChanged>(OnEffectiveValueChanged);
        }

        private void OnEffectiveValueChanged(EffectiveValueChanged e)
        {
            if (null != render && render.Plugin == e.Plugin)
            {
                NotifyPropertyChanged(e.PluginProperty.Name);
            }
        }

        private void OnPropertyChanged(Component component, PluginProperty property)
        {
            NotifyPropertyChanged(property.Name);
        }
    }
}
