using Kinectitude.Editor.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinectitude.Editor.Views
{
    public class StretchCanvas : Canvas
    {
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

            return size;
        }
    }

    /// <summary>
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : UserControl
    {
        private bool dragging;
        private Point? dragStart;

        internal IEnumerable<EntityItem> EntityItems
        {
            get { return PART_Canvas.Children.OfType<EntityItem>(); }
        }

        public Selection()
        {
            InitializeComponent();

            AddHandler(EntityItem.EntityItemMouseDownEvent, new EntityItemEventHandler(EntityItem_MouseDown));
            AddHandler(EntityItem.EntityItemMouseUpEvent, new EntityItemEventHandler(EntityItem_MouseUp));
        }

        internal void AddEntity(EntityItem item)
        {
            item.IsSelected = true;

            // Reposition ourself within the main canvas
            // Get the minimum logical coordinates of the new item and all other selected ones.
            // Move the selected group to these coordinates.

            double minLeft = Canvas.GetLeft(item);
            double minTop = Canvas.GetTop(item);

            foreach (EntityItem existingItem in this.EntityItems)
            {
                double left = Canvas.GetLeft(existingItem);
                double top = Canvas.GetTop(existingItem);

                if (left < minLeft)
                {
                    minLeft = left;
                }

                if (top < minTop)
                {
                    minTop = top;
                }
            }

            Canvas.SetLeft(this, minLeft);
            Canvas.SetTop(this, minTop);

            // Add the item to the group selection
            PART_Canvas.Children.Add(item);

            // we shouldn't use a binding for position anymore.
            // for all items in the selection, set their left and top properties to
            // selection canvas coordinates.

            // the selected canvas needs to wrap the children visually, so it must use the
            // visual left and top coordinates of all entities. Not the logical x and y.

            foreach (EntityItem selectedItem in this.EntityItems)
            {
                // physical coordinate = scene coordinate - min coordinate

                EntityPresenter presenter = selectedItem.Content as EntityPresenter;

                double left = presenter.X - minLeft;
                double top = presenter.Y - minTop;

                Canvas.SetLeft(selectedItem, left);
                Canvas.SetTop(selectedItem, top);
            }
        }

        internal void RemoveEntity(EntityItem item)
        {
            item.IsSelected = false;
            PART_Canvas.Children.Remove(item);
        }

        private void EntityItem_MouseDown(object sender, EntityItemEventArgs e)
        {
            
        }

        private void EntityItem_MouseUp(object sender, EntityItemEventArgs e)
        {
            
        }
    }
}
