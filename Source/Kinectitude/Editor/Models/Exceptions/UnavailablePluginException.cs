using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class UnavailablePluginException : EditorException
    {
        public UnavailablePluginException(string name) : base(string.Format("This game uses a plugin '{0}' that is not installed.", name)) { }
    }
}
