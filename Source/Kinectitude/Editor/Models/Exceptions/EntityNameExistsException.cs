using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class EntityNameExistsException : EditorException
    {
        public EntityNameExistsException(string name) : base(string.Format("An entity named '{0}' already exists here.", name)) { }
    }
}
