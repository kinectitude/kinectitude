using System;

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

        /// <summary>
        /// Calls the callback function when the value of this expression changes
        /// </summary>
        /// <param name="callback">The callback to call</param>
        void notifyOfChange(Action<string> callback);

    }
}
