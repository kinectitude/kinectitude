using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class InvalidEntityNameException : EditorException
    {
        public InvalidEntityNameException(string name) : base(string.Format("'{0}' is not a valid entity name.", name)) { }
    }
}
