using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Events
{
    class OnCreateEvent : Event
    {
        public override void OnInitialize()
        {
            Entity.addOnCreate(this);
        }
    }
}
