using System;
using System.Collections.Generic;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Physics
{
    [Plugin("Crosses a line", "")]
    public class CrossesLineEvent : Event
    {
        public enum LineType
        {
            X,
            Y
        };
        
        public enum FromDirection
        {
            Positive = 1,
            Negative = 2,
            Both = Positive | Negative
        };

        [Plugin("Axis", "")]
        public LineType Line { set; get; }

        [Plugin("Location", "")]
        public double Location { set; get; }

        [Plugin("Direction", "")]
        public FromDirection Direction { set; get; }

        public CrossesLineEvent()
        {
            Direction = FromDirection.Both;
        }

        public override void OnInitialize()
        {
            PhysicsComponent pc = GetComponent<PhysicsComponent>();
            if (null == pc)
            {
                List<Type> missing = new List<Type>();
                missing.Add(typeof(PhysicsComponent));
                //TODO this will be detected later
                //throw MissingRequirementsException.MissingRequirement(this, missing);
            }
            pc.AddCrossLineEvent(this);
        }
    }
}
