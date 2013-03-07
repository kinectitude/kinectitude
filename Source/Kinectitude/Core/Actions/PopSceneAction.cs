using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Actions
{
    [Plugin("pop this scene", "Pop scene")]
    internal sealed class PopSceneAction : Action
    {
        public PopSceneAction() { }

        public override void Run()
        {
            Game game = Event.Entity.Scene.Game;
            game.PopScene();
        }
    }
}
