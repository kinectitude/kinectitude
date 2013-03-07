using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("destroy this entity", "Destroy entity")]
    class DestroyAction : Action
    {
        public override void Run()
        {
            Event.Entity.Destroy();
        }
    }
}
