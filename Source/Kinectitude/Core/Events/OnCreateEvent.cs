using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;

namespace Kinectitude.Core.Events
{
    [Plugin("Occurs when an entity is created", "")]
    class OnCreateEvent : Event
    {
        public override void OnInitialize()
        {
            Entity.addOnCreate(this);
        }
    }
}
