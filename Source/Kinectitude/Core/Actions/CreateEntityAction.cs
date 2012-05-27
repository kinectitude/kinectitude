using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Creates an entity from a prototype", "")]
    class CreateEntityAction : Action
    {
        [Plugin("Name of prototype to make", "")]
        public string Prototype { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.createEntity(Prototype);
        }
    }
}
