using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Destroy this Entity", "")]
    class DestroyAction : Action
    {
        public override void Run()
        {
            Event.Entity.Destroy();
        }
    }
}
