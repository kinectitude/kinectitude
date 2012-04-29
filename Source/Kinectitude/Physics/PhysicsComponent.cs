using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Kinectitude.Core;
using Kinectitude.Attributes;

namespace Kinectitude.Physics
{
    [Plugin("Physics Component", "")]
    public class PhysicsComponent : Component, IPhysics
    {
        private List<CrossesLineEvent> crossesLineEvents = new List<CrossesLineEvent>();

        [Plugin("Horizontal Velocity", "")]
        public double Dx
        {
            get { return double.Parse(Entity["Dx"]); }
            set { Entity["Dx"] = value.ToString(); }
        }

        [Plugin("Vertical Velocity", "")]
        public double Dy
        {
            get { return double.Parse(Entity["Dy"]); }
            set { Entity["Dy"] = value.ToString(); }
        }

        public double Width { get; private set; }
        public double Height { get; private set; }
        public double X
        {
            get { return double.Parse(Entity["x"]); }
            set { Entity["x"] = value.ToString(); }
        }
        public double Y
        {
            get { return double.Parse(Entity["y"]); }
            set { Entity["y"] = value.ToString(); }
        }

        override public Type ManagerType(){return typeof(PhysicsManager);}

        public PhysicsComponent(Entity entity) : base(entity)
        {
            if (null != Entity["radius"])
            {
                double radius = double.Parse(Entity["radius"]);
                Width = radius * 2;
                Height = Width;
            }
            else
            {
                Width = double.Parse(Entity["width"]);
                Height = double.Parse(Entity["height"]);
            }
        }

        public bool Above(IPhysics other)
        {
            return other.Y >= Y + Height;
        }

        public bool HitTest(IPhysics other, double predTimestep)
        {
            double futureX = X + Dx * predTimestep;
            double futureY = Y + Dy * predTimestep;
            
            Rect r1 = new Rect(futureX, futureY, Width, Height);
            Rect r2 = new Rect(other.X + other.Dx * predTimestep, other.Y + other.Dy * predTimestep, other.Width, other.Height);

            bool res = r1.IntersectsWith(r2);
            return res;
        }

        public override void OnUpdate(double t)
        {
            //Since it can be updated while in the update, keep it the same until we are done.
            double delX = Dx;
            double delY = Dy;
            double prevX = X;
            double prevY = Y;
            X += delX;
            Y += delY;

            if (Y < 0 || Y > 600 - Height)
            {
                Dy = -Dy;
            }

            // If we need to check for exited right events
            if (crossesLineEvents.Count != 0)
            {
                foreach (CrossesLineEvent evt in crossesLineEvents)
                {
                    double cross = evt.Position;
                    //from neg and positive
                    bool neg = true;
                    bool pos = true;
                    if (CrossesLineEvent.FromDirection.Negative == evt.From)
                    {
                        pos = false;
                    }
                    else if (CrossesLineEvent.FromDirection.Positive == evt.From)
                    {
                        neg = false;
                    }

                    double compareTo;
                    double next;
                    if (CrossesLineEvent.LineType.X == evt.Cross)
                    {
                        compareTo = prevX;
                        next = delX;
                    }
                    else
                    {
                        compareTo = prevY;
                        next = delY;
                    }
                    if (pos && 0 < next  && cross < next + compareTo && cross >= compareTo)
                    {
                        evt.DoActions();
                    }
                    else if (neg && 0 > next && cross > next + compareTo && cross <= compareTo)
                    {
                        evt.DoActions();
                    }
                }
            }
        }

        public void AddCrossLineEvent(CrossesLineEvent evt)
        {
            crossesLineEvents.Add(evt);
        }

        public void OnSetPositionAction(SetPositionAction action)
        {
            X = action.X;
            Y = action.Y;
        }
    }
}
