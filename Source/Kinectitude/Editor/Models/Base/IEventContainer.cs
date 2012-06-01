using System.Collections.Generic;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.Models.Base
{
    internal interface IEventContainer
    {
        IEnumerable<Event> Events { get; }

        void AddEvent(Event evt);
        void RemoveEvent(Event evt);
    }
}
