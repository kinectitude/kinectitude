using Kinectitude.Editor.Models;
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
        }

        private void OnPropertyChanged(Component component, PluginProperty property)
        {
            NotifyPropertyChanged(property.Name);
        }
    }
}
