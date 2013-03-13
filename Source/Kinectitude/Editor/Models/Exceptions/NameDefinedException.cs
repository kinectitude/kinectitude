using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class NameDefinedException : EditorException
    {
        public NameDefinedException(string name) : base(string.Format("The name '{0}' is already reserved for a plugin.", name)) { }
    }
}
