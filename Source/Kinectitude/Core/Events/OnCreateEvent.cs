using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;

namespace Kinectitude.Core.Events
{
    [Plugin("when this entity is created", "Entity created")]
    class OnCreateEvent : Event
    {
        public override void OnInitialize()
        {
            Entity.addOnCreate(this);
        }
    }
}
