using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Presenters;

namespace Kinectitude.Editor.Views
{
    delegate void EntityItemEventHandler(object sender, EntityItemEventArgs e);

    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    internal class EntityItem : ContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(EntityItem), new FrameworkPropertyMetadata(false));

        public static readonly RoutedEvent EntityItemMouseDownEvent =
            EventManager.RegisterRoutedEvent("EntityItemMouseDown", RoutingStrategy.Bubble, typeof(EntityItemEventHandler), typeof(EntityItem));

        public static readonly RoutedEvent EntityItemMouseUpEvent =
            EventManager.RegisterRoutedEvent("EntityItemMouseUp", RoutingStrategy.Bubble, typeof(EntityItemEventHandler), typeof(EntityItem));

        private readonly Entity entity;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public EntityItem(Entity entity)
        {
            this.entity = entity;

            entity.Components.CollectionChanged += Components_CollectionChanged;

            UpdatePresenter();
        }

        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePresenter();
        }

        private void UpdatePresenter()
        {
            EntityPresenter presenter = EntityPresenter.Create(entity);
            Content = presenter;

            Canvas.SetLeft(this, presenter.X);
            Canvas.SetTop(this, presenter.Y);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            RaiseEvent(new EntityItemEventArgs(EntityItemMouseDownEvent, e, this));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            RaiseEvent(new EntityItemEventArgs(EntityItemMouseUpEvent, e, this));
        }
    }
}
