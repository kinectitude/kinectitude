using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Physics
{
    [Plugin("Exits the scene on the right", "")]
    public class ExitsRightEvent : CrossesLineEvent
    {
        public ExitsRightEvent()
        {
            Cross = CrossesLineEvent.LineType.X;
            Position = 800;
            From = FromDirection.Positive;
        }
    }
}