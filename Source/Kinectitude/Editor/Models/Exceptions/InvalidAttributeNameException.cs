using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class InvalidAttributeNameException : EditorException
    {
        public InvalidAttributeNameException(string name) : base(string.Format("'{0}' is not a valid attribute name.", name)) { }
    }
}
