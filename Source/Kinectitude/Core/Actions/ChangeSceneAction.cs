using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Actions
{
    [Plugin("Change the scene", "")]
    internal sealed class ChangeSceneAction : Action
    {
        [Plugin("Scene", "")]
        public IExpressionReader Target { get; set; }

        public ChangeSceneAction() { }

        public override void Run()
        {
            Game game = Event.Entity.Scene.Game;
            game.RunScene(Target.GetValue());
        }
    }
}