using System.Collections.Generic;
using Kinectitude.Core;
using System.Xml;
using Kinectitude.Attributes;

namespace Kinectitude.Physics
{
    [Plugin("Exits the scene on the left", "")]
    public class ExitsLeftEvent : CrossesLineEvent
    {
        public ExitsLeftEvent()
        {
            Cross = CrossesLineEvent.LineType.X;
            Position = 0;
            From = FromDirection.Negative;
        }
    }
}
