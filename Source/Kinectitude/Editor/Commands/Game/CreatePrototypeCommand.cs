using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    internal sealed class CreatePrototypeCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly EntityViewModel prototype;

        public string Name
        {
            get { return string.Format("Create Prototype '{0}'", prototype.Name); }
        }

        public CreatePrototypeCommand(GameViewModel game, EntityViewModel prototype)
        {
            this.game = game;
            this.prototype = prototype;
        }

        public void Execute()
        {
            if (null != prototype)
            {
                game.AddPrototype(prototype);
            }
        }

        public void Unexecute()
        {
            if (null != prototype)
            {
                game.RemovePrototype(prototype);
            }
        }
    }
}
