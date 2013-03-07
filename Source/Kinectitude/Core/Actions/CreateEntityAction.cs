using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("create entity based on {Prototype}", "Create an entity")]
    class CreateEntityAction : Action
    {
        [PluginProperty("Prototype", "Name of Prototype to make")]
        public string Prototype { get; set; }

        public override void Run()
        {
            Event.Entity.Scene.CreateEntity(Prototype);
        }
    }
}
