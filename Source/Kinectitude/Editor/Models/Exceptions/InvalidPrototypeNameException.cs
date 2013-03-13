using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class InvalidPrototypeNameException : EditorException
    {
        public InvalidPrototypeNameException() : base("A prototype must have a name") { }
    }
}
