using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class EditorException : Exception
    {
        protected EditorException(string message) : base(message) { }
    }
}
