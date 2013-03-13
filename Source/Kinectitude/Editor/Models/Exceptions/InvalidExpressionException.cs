using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class InvalidExpressionException : EditorException
    {
        public InvalidExpressionException(string expression) : base(string.Format("'{0}' is not a valid expression.", expression)) { }
    }
}
