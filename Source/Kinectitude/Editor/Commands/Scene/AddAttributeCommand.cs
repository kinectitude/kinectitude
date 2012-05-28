using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Scene
{
    public class AddAttributeCommand : IUndoableCommand
    {
        private readonly SceneViewModel scene;
        private readonly AttributeViewModel attribute;

        public string Name
        {
            get { return string.Format("Add Attribute '{0}'", attribute.Key); }
        }

        public AddAttributeCommand(SceneViewModel scene, AttributeViewModel attribute)
        {
            this.scene = scene;
            this.attribute = attribute;
        }

        public void Execute()
        {
            scene.AddAttribute(attribute);
        }

        public void Unexecute()
        {
            scene.RemoveAttribute(attribute);
        }
    }
}
