using System;
using System.Linq;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Kinectitude.Editor.Models;
using System.Collections.Generic;

namespace Kinectitude.Editor.Views
{
    internal class SceneCanvas : Canvas
    {
        private readonly Stack<AbstractMode> modes;

        public IEnumerable<EntityItem> EntityItems
        {
            get { return this.Children.OfType<EntityItem>(); }
        }

        public SceneCanvas()
        {
            DataContextChanged += SceneCanvas_DataContextChanged;
            Loaded += SceneCanvas_Loaded;

            Scene scene = DataContext as Scene;
            if (null != scene)
            {
                UpdateEntities(scene);
            }

            modes = new Stack<AbstractMode>();

            AddHandler(EntityItem.EntityItemMouseDownEvent, new EntityItemEventHandler(EntityItem_MouseDown));
            AddHandler(EntityItem.EntityItemMouseUpEvent, new EntityItemEventHandler(EntityItem_MouseUp));
        }

        private void SceneCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            SetMode(new SelectionMode(this));
        }

        public void SetMode(AbstractMode mode)
        {
            while (modes.Count > 0)
            {
                AbstractMode previous = modes.Pop();
                previous.Uninitialize();
            }

            mode.Initialize();
            modes.Push(mode);
        }

        public void PushMode(AbstractMode mode)
        {
            modes.Peek().Pause();

            mode.Initialize();
            modes.Push(mode);
        }

        public void PopMode()
        {
            AbstractMode previous = modes.Pop();
            previous.Uninitialize();

            modes.Peek().Resume();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            modes.Peek().OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            modes.Peek().OnMouseMove(e);
        }

        private void EntityItem_MouseDown(object sender, EntityItemEventArgs e)
        {
            modes.Peek().OnEntityMouseDown(e);
        }

        private void EntityItem_MouseUp(object sender, EntityItemEventArgs e)
        {
            modes.Peek().OnEntityMouseUp(e);
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
