using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal interface IDataContainer
    {
        ValueReader this[string key] { get; set; }
        void NotifyOfChange(string key, IChanges callback);
        void NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback);
        object GetParam(IChangeable what, string property);
    }
}
