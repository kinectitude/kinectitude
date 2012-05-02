using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.Models
{
    public interface IEventContainer
    {
        IEnumerable<Event> Events { get; }

        void AddEvent(Event evt);
        void RemoveEvent(Event evt);
    }
}
