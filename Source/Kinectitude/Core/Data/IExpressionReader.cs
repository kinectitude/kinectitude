using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used to get the value of an expression
    /// </summary>
    public interface IExpressionReader
    {
        /// <summary>
        /// Gets the value of an expression
        /// </summary>
        /// <returns>The string value of an expression</returns>
        string GetValue();

        void notifyOfChange(Action<string, string> callback);

    }
}
