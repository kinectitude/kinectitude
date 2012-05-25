using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used for writing to an attribute.
    /// </summary>
    public interface IValueWriter
    {
        /// <summary>
        /// The value you want to set to the attribute equal to
        /// </summary>
        string Value { get; set; }
    }
}
