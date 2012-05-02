using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Views
{
    public class PluginTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComponentTemplate { get; set; }
        public DataTemplate EventTemplate { get; set; }
        public DataTemplate ActionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //PluginDescriptorViewModel 
            return base.SelectTemplate(item, container);
        }
    }
}
