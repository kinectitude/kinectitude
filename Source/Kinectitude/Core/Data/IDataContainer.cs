using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal interface IDataContainer
    {
        ValueReader this[string key] { get; set; }
        void NotifyOfChange(string key, IChangeable callback);
        void NotifyOfComponentChange(string what, IChangeable callback);
    }
}
