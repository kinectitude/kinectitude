using System;
using System.Windows;
using System.Windows.Controls;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal sealed class DesignerItem : ContentControl
    {
        public static DependencyProperty DesignLeftProperty =
            DependencyProperty.Register("DesignLeft", typeof(double), typeof(DesignerItem), new FrameworkPropertyMetadata(OnDesignChange));

        public static DependencyProperty DesignTopProperty =
            DependencyProperty.Register("DesignTop", typeof(double), typeof(DesignerItem), new FrameworkPropertyMetadata(OnDesignChange));

        public static DependencyProperty DesignWidthProperty =
            DependencyProperty.Register("DesignWidth", typeof(double), typeof(DesignerItem), new FrameworkPropertyMetadata(OnDesignChange));

        public static DependencyProperty DesignHeightProperty =
            DependencyProperty.Register("DesignHeight", typeof(double), typeof(DesignerItem), new FrameworkPropertyMetadata(OnDesignChange));

        public static DependencyProperty DesignRotationProperty =
            DependencyProperty.Register("DesignRotation", typeof(double), typeof(DesignerItem), new FrameworkPropertyMetadata(OnDesignChange));

        public static DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem));

        private static void OnDesignChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as DesignerItem;
            item.canvas.Dispatcher.BeginInvoke(new Action(() => item.canvas.Update()), null);
        }

        private readonly DesignerCanvas canvas;

        public double DesignLeft
        {
            get { return (double)GetValue(DesignLeftProperty); }
            set { SetValue(DesignLeftProperty, value); }
        }

        public double DesignTop
        {
            get { return (double)GetValue(DesignTopProperty); }
            set { SetValue(DesignTopProperty, value); }
        }

        public double DesignWidth
        {
            get { return (double)GetValue(DesignWidthProperty); }
            set { SetValue(DesignWidthProperty, value); }
        }

        public double DesignHeight
        {
            get { return (double)GetValue(DesignHeightProperty); }
            set { SetValue(DesignHeightProperty, value); }
        }

        public double DesignRotation
        {
            get { return (double)GetValue(DesignRotationProperty); }
            set { SetValue(DesignRotationProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public DesignerItem(DesignerCanvas canvas)
        {
            this.canvas = canvas;
        }
    }
}
