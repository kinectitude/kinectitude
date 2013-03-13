using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class PrototypeExistsException : EditorException
    {
        public PrototypeExistsException(string name) : base(string.Format("A prototype named '{0}' already exists.", name)) { }
    }
}
