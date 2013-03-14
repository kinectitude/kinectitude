using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class AttributeNameExistsException : EditorException
    {
        public AttributeNameExistsException(string name) : base(string.Format("The attribute '{0}' already exists here.", name)) { }
    }
}
