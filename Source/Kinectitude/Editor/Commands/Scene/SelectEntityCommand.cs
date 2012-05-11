using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Commands.Base;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Commands.Scene
{
    public class SelectEntityCommand : IUndoableCommand
    {
        private readonly SceneViewModel scene;
        private readonly EntityViewModel entity;

        public string Name
        {
            get { return "Select Entity"; }
        }

        public SelectEntityCommand(SceneViewModel scene, EntityViewModel entity)
        {
            this.scene = scene;
            this.entity = entity;
        }

        public void Unexecute()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
