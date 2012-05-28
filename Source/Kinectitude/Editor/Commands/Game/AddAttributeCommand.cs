using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Game
{
    public class AddAttributeCommand : IUndoableCommand
    {
        private readonly GameViewModel game;
        private readonly AttributeViewModel attribute;

        public string Name
        {
            get { return string.Format("Add Attribute '{0}'", attribute.Key); }
        }

        public AddAttributeCommand(GameViewModel game, AttributeViewModel attribute)
        {
            this.game = game;
            this.attribute = attribute;
        }

        public void Execute()
        {
            game.AddAttribute(attribute);
        }

        public void Unexecute()
        {
            game.RemoveAttribute(attribute);
        }
    }
}
