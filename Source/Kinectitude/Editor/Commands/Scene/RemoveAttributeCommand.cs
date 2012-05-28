using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Scene
{
    public class RemoveAttributeCommand : IUndoableCommand
    {
        private readonly SceneViewModel scene;
        private readonly AttributeViewModel attribute;

        public string Name
        {
            get { return string.Format("Remove Attribute '{0}'", attribute.Key); }
        }

        public RemoveAttributeCommand(SceneViewModel scene, AttributeViewModel attribute)
        {
            this.scene = scene;
            this.attribute = attribute;
        }

        public void Execute()
        {
            scene.RemoveAttribute(attribute);
        }

        public void Unexecute()
        {
            scene.AddAttribute(attribute);
        }
    }
}
