using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Storage
{
    interface IValueMaker
    {
        bool HasErrors(string value);
        ValueReader CreateValueReader(string value, IScene scene, IDataContainer entity, Event evt = null);
    }
}
