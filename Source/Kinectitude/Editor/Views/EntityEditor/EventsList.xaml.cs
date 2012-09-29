using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kinectitude.Editor.Models;
using Action = Kinectitude.Editor.Models.Action;
using Condition = Kinectitude.Editor.Models.Condition;
using Kinectitude.Editor.Presenters;

namespace Kinectitude.Editor.Views
{
    /// <summary>
    /// Interaction logic for EventsList.xaml
    /// </summary>
    public partial class EventsList : UserControl
    {
        public EventsList()
        {
            InitializeComponent();
        }

        private void ActionTarget_Drop(object sender, DragEventArgs args)
        {
            DragDropData data = args.Data.GetData(typeof(DragDropData)) as DragDropData;

            if (null != data)
            {
                Plugin plugin = data.Plugin;
                if (null != plugin)
                {
                    FrameworkElement element = sender as FrameworkElement;
                    if (null != element)
                    {
                        Event evt = element.DataContext as Event;
                        if (null != evt)
                        {
                            evt.AddAction(new Action(plugin));
                        }
                        else
                        {
                            Condition condition = element.DataContext as Condition;
                            if (null != condition)
                            {
                                condition.AddAction(new Action(plugin));
                            }
                        }
                    }
                }
            }
        }

        private void EventTarget_Drop(object sender, DragEventArgs args)
        {
            DragDropData data = args.Data.GetData(typeof(DragDropData)) as DragDropData;

            if (null != data)
            {
                Plugin plugin = data.Plugin;
                if (null != plugin)
                {
                    Entity entity = DataContext as Entity;
                    if (null != entity)
                    {
                        entity.AddEvent(new Event(plugin));
                    }
                }
            }

            args.Handled = true;
        }
    }
}
