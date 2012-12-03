using Kinectitude.Core.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Create Entity based on {Prototype}", "")]
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
