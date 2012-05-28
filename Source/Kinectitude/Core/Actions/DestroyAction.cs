using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    class DestroyAction : Action
    {
        public override void Run()
        {
            Event.Entity.Destroy();
        }
    }
}
