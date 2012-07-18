using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Creates an Entity from a prototype", "")]
    class CreateEntityAction : Action
    {
        [Plugin("Name of prototype to make", "")]
        public string Prototype { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.CreateEntity(Prototype);
        }
    }
}
