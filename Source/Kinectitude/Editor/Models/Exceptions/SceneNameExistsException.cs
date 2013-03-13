using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class SceneNameExistsException : EditorException
    {
        public SceneNameExistsException(string name) : base(string.Format("A scene named '{0}' already exists", name)) { }
    }
}
