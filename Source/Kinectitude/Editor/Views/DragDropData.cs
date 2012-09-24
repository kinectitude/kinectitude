using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views
{
    internal class DragDropData
    {
        public Plugin Plugin
        {
            get;
            private set;
        }

        public DragDropData(Plugin plugin)
        {
            Plugin = plugin;
        }
    }
}
