using Kinectitude.Attributes;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("Pop the scene", "")]
    public sealed class PopSceneAction : Action
    {
        public PopSceneAction() { }

        public override void Run()
        {
            Event.Scene.Game.PopScene();
        }
    }
}
