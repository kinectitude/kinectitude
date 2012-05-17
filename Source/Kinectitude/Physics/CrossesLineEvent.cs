using System;
using System.Collections.Generic;
using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Interfaces;

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
        public string Line
        {
            set
            {
                switch (value)
                {
                    case "X":
                        Cross = LineType.X;
                        break;
                    case "Y":
                        Cross = LineType.Y;
                        break;
                    default:
                        //TODO
                        break;
                }
            }
        }

        [Plugin("Location", "")]
        public string Location
        {
            set
            {
                Position = double.Parse(value);
            }
            get
            {
                return Position.ToString();
            }
        }

        [Plugin("Direction", "")]
        public string Direction
        {
            set
            {
                switch (value)
                {
                    case "Negative":
                        From = FromDirection.Negative;
                        break;
                    case "Positive":
                        From = FromDirection.Positive;
                        break;
                    case "Both":
                        From = FromDirection.Both;
                        break;
                    default:
                        //TODO
                        break;
                }
            }
        }

        public FromDirection From { set; get; }

        public double Position { set; get; }

        public LineType Cross { set; get; }

        public CrossesLineEvent()
        {
            From = FromDirection.Both;
        }

        public override void OnInitialize()
        {
            PhysicsComponent pc = Entity.GetComponent(typeof(IPhysicsComponent)) as PhysicsComponent;
            if (null == pc)
            {
                List<Type> missing = new List<Type>();
                missing.Add(typeof(PhysicsComponent));
                throw MissingRequirementsException.MissingRequirement(this, missing);
            }
            pc.AddCrossLineEvent(this);
        }
    }
}
