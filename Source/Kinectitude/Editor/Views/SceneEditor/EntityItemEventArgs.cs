using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Kinectitude.Editor.Views
{
    internal class EntityItemEventArgs : RoutedEventArgs
    {
        private readonly EntityItem item;
        private readonly MouseButtonEventArgs mouseEvent;

        public EntityItem EntityItem
        {
            get { return item; }
        }

        public MouseButtonEventArgs MouseEvent
        {
            get { return mouseEvent; }
        }

        public EntityItemEventArgs(RoutedEvent routedEvent, MouseButtonEventArgs mouseEvent, EntityItem item)
            : base(routedEvent, item)
        {
            this.item = item;
            this.mouseEvent = mouseEvent;
        }
    }
}
