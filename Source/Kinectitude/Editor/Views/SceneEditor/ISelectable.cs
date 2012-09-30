using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Views
{
    internal interface ISelectable
    {
        bool IsSelected { get; set; }
    }
}
