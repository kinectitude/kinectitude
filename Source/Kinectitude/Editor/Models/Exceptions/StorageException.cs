using Irony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Exceptions
{
    internal class StorageException : EditorException
    {
        public StorageException(string message) : base(message) {}
    }
}
