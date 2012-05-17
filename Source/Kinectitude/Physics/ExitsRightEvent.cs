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