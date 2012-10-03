using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Presenters;
using System.Collections.Specialized;
using System.Windows.Data;

namespace Kinectitude.Editor.Views
{
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    internal class EntityItem : ContentControl, ISelectable
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(EntityItem), new FrameworkPropertyMetadata(false));

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

        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePresenter();
        }

        private void UpdatePresenter()
        {
            EntityPresenter presenter = EntityPresenter.Create(entity);
            Content = presenter;

            Binding widthBinding = new Binding("Width");
            widthBinding.Source = presenter;
            widthBinding.Mode = BindingMode.TwoWay;

            this.SetBinding(ContentControl.WidthProperty, widthBinding);

            Binding heightBinding = new Binding("Height");
            heightBinding.Source = presenter;
            heightBinding.Mode = BindingMode.TwoWay;

            this.SetBinding(ContentControl.HeightProperty, heightBinding);

            Binding leftBinding = new Binding("X");
            leftBinding.Source = presenter;
            leftBinding.Mode = BindingMode.TwoWay;

            this.SetBinding(Canvas.LeftProperty, leftBinding);

            Binding topBinding = new Binding("Y");
            topBinding.Source = presenter;
            topBinding.Mode = BindingMode.TwoWay;

            this.SetBinding(Canvas.TopProperty, topBinding);
        }
    }
}
