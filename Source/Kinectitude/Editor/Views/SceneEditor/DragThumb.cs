using Kinectitude.Editor.Presenters;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Kinectitude.Editor.Views
{
    internal class DragThumb : Thumb
    {
        public DragThumb()
        {
            DragDelta += DragThumb_DragDelta;
            DragStarted += DragThumb_DragStarted;
            DragCompleted += DragThumb_DragCompleted;
        }

        private Selection GetSelection()
        {
            DependencyObject result = VisualTreeHelper.GetParent(this);
            Selection selection = result as Selection;

            while (null == selection)
            {
                result = VisualTreeHelper.GetParent(result);

                if (null == result)
                {
                    return null;
                }

                selection = result as Selection;
            }

            return selection;
        }

        void DragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //Selection selection = GetSelection();

            //foreach (EntityItem item in selection.EntityItems)
            //{
            //    EntityPresenter presenter = item.Content as EntityPresenter;

            //    presenter.X = Canvas.GetLeft(selection) + Canvas.GetLeft(item);
            //    presenter.Y = Canvas.GetTop(selection) + Canvas.GetTop(item);
            //}
        }

        void DragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Selection selection = GetSelection();

            //Canvas.SetLeft(selection, Canvas.GetLeft(selection) + e.HorizontalChange);
            //Canvas.SetTop(selection, Canvas.GetTop(selection) + e.VerticalChange);

            DependencyObject obj = VisualTreeHelper.GetParent(this);
            EntityItem item = obj as EntityItem;

            while (null == item)
            {
                obj = VisualTreeHelper.GetParent(obj);

                if (null == obj)
                {
                    return;
                }

                item = obj as EntityItem;
            }

            TranslateTransform translate = item.Template.FindName("EntityTranslate", item) as TranslateTransform;
            //object rotate = item.Template.FindName("EntityRotate", item);

            translate.X += e.HorizontalChange;
            translate.Y += e.VerticalChange;
        }
    }
}
