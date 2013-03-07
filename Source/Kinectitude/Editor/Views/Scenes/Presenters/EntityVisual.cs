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
