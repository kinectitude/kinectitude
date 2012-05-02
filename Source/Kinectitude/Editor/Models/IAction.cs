using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.Models
{
    public interface IAction
    {
        IActionContainer Parent { get; set; }
    }
}
