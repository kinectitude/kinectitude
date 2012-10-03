using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Collections.Specialized;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views
{
    internal class SceneCanvas : Canvas
    {
        private Point? boxedSelectionStart;
        private Lazy<SelectionService> selectionService;

        public SelectionService SelectionService
        {
            get { return selectionService.Value; }
        }

        public SceneCanvas()
        {
            selectionService = new Lazy<SelectionService>(() => new SelectionService(this));

            Scene scene = DataContext as Scene;
            if (null != scene)
            {
                UpdateEntities(scene);
            }

            DataContextChanged += SceneCanvas_DataContextChanged;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Source == this)
            {
                boxedSelectionStart = new Point?(e.GetPosition(this));

                SelectionService.ClearSelection();
                Focus();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                boxedSelectionStart = null;
            }

            if (boxedSelectionStart.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (null != adornerLayer)
                {
                    RubberBandAdorner adorner = new RubberBandAdorner(this, boxedSelectionStart);
                    adornerLayer.Add(adorner);
                }
            }

            e.Handled = true;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }

            size.Width += 10;
            size.Height += 10;

            return size;
        }

        private void SceneCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (null != e.OldValue)
            {
                ((Scene)e.OldValue).Entities.CollectionChanged -= Entities_CollectionChanged;
            }

            UpdateEntities(e.NewValue as Scene);
        }

        private void Entities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Entity entity in e.NewItems)
                {
                    AddEntity(entity);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Entity entity in e.OldItems)
                {
                    RemoveEntity(entity);
                }
            }
        }

        private void UpdateEntities(Scene scene)
        {
            ClearEntities();

            if (null != scene)
            {
                scene.Entities.CollectionChanged += Entities_CollectionChanged;

                foreach (Entity entity in scene.Entities)
                {
                    AddEntity(entity);
                }
            }
        }

        private void ClearEntities()
        {
            Children.Clear();
        }

        private void AddEntity(Entity entity)
        {
            EntityItem item = new EntityItem(entity);
            Canvas.SetZIndex(item, Children.Count);
            Children.Add(item);
        }

        private void RemoveEntity(Entity entity)
        {
            foreach (UIElement element in Children)
            {
                EntityItem item = element as EntityItem;
                if (null != item)
                {
                    if (item.Content == entity)
                    {
                        Children.Remove(item);
                        return;
                    }
                }
            }
        }
    }
}
