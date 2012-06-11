using System;

namespace Kinectitude.Core.Data
{
    public interface IChangeNotify
    {
        /// <summary>
        /// Calls the callback function when the value of this expression changes
        /// </summary>
        /// <param name="callback">The callback to call</param>
        void notifyOfChange(Action<string> callback);
    }
}
