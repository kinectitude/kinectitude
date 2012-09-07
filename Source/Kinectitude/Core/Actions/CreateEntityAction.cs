using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Create entity based on {Prototype}", "")]
    class CreateEntityAction : Action
    {
        [Plugin("Prototype", "Name of prototype to make")]
        public string Prototype { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.CreateEntity(Prototype);
        }
    }
}
