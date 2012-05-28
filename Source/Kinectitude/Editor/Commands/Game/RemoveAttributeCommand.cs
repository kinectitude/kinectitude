using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    public class RemoveAttributeCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly AttributeViewModel attribute;

        public string Name
        {
            get { return string.Format("Remove Attribute '{0}'", attribute.Key); }
        }

        public RemoveAttributeCommand(GameViewModel game, AttributeViewModel attribute)
        {
            this.game = game;
            this.attribute = attribute;
        }

        public void Execute()
        {
            game.RemoveAttribute(attribute);
        }

        public void Unexecute()
        {
            game.AddAttribute(attribute);
        }
    }
}
