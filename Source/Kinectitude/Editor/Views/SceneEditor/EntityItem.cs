using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Kinectitude.Editor.Views
{
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    internal class EntityItem : ContentControl, ISelectable
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(EntityItem), new FrameworkPropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public EntityItem()
        {
            //FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityItem), new FrameworkPropertyMetadata(typeof(EntityItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            SceneCanvas canvas = VisualTreeHelper.GetParent(this) as SceneCanvas;
            if (null != canvas)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    if (IsSelected)
                    {
                        canvas.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        canvas.SelectionService.AddToSelection(this);
                    }
                }
                else if (!IsSelected)
                {
                    canvas.SelectionService.SelectItem(this);
                }

                Focus();
            }

            e.Handled = false;
        }
    }
}
