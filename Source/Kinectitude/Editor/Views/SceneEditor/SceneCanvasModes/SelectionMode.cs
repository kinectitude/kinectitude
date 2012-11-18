using Kinectitude.Editor.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinectitude.Editor.Views
{
    internal sealed class SelectionMode : AbstractMode
    {
        private Point? boxedSelectionStart;
        private Selection selection;

        public SelectionMode(SceneCanvas canvas)
            : base(canvas)
        {

        }

        public override void Initialize()
        {
            selection = new Selection();
            selection.Visibility = Visibility.Collapsed;

            this.SceneCanvas.Children.Add(selection);

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.SceneCanvas);

            ControlAdorner adorner = new ControlAdorner(this.SceneCanvas);
            adorner.Child = new Selection();

            layer.Add(adorner);
        }

        public override void Uninitialize()
        {
            this.SceneCanvas.Children.Remove(selection);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            // TODO: The source can be any area that isn't currently selected.

            if (e.Source == this.SceneCanvas)
            {
                boxedSelectionStart = new Point?(e.GetPosition(this.SceneCanvas));

                this.SceneCanvas.Focus();
                e.Handled = true;
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                boxedSelectionStart = null;
            }

            if (boxedSelectionStart.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.SceneCanvas);
                if (null != adornerLayer)
                {
                    RubberBandAdorner adorner = new RubberBandAdorner(this.SceneCanvas, boxedSelectionStart);
                    adornerLayer.Add(adorner);
                }
            }

            e.Handled = true;
        }

        public override void OnEntityMouseDown(EntityItemEventArgs e)
        {
            
        }

        //public override void OnEntityMouseUp(EntityItemEventArgs e)
        //{
        //    /*if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
        //    {
        //        ClearSelection();
        //    }*/

        //    if (e.EntityItem.IsSelected)
        //    {
        //        RemoveFromSelection(e.EntityItem);
        //    }
        //    else
        //    {
        //        AddToSelection(e.EntityItem);
        //    }

        //    e.Handled = true;
        //}

        private void AddToSelection(EntityItem item)
        {
            if (selection.Visibility == Visibility.Collapsed)
            {
                selection.Visibility = Visibility.Visible;
            }

            this.SceneCanvas.Children.Remove(item);
            selection.AddEntity(item);
        }

        private void RemoveFromSelection(EntityItem item)
        {
            selection.RemoveEntity(item);

            EntityPresenter presenter = item.Content as EntityPresenter;

            Canvas.SetLeft(item, presenter.X);
            Canvas.SetTop(item, presenter.Y);

            this.SceneCanvas.Children.Add(item);

            if (selection.EntityItems.Count() == 0)
            {
                selection.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearSelection()
        {
            IEnumerable<EntityItem> selectedItems = selection.EntityItems.ToList();

            foreach (EntityItem item in selectedItems)
            {
                RemoveFromSelection(item);
            }
        }
    }
}
