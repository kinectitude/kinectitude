using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public interface IEventContainer
    {
        void AddEvent(Event evt);
        void RemoveEvent(Event evt);
    }
}
