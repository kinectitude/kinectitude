
namespace Kinectitude.Core.Base
{
    public abstract class Action
    {
        public Event Event { get; set; }

        public abstract void Run();
    }
}
