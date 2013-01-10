using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal sealed class DefaultEntityVisual : EntityVisual
    {
        public DefaultEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }
    }
}
