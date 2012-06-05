using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Base;
using Action = Kinectitude.Editor.Models.Plugins.Action;
using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class ActionViewModel : BaseModel
    {
        private readonly Entity entity;
        private readonly Action action;

        /*public bool IsInherited
        {
            get { return action.p
        }*/

        public ActionViewModel(Entity entity, Action action)
        {
            this.entity = entity;
            this.action = action;
        }
    }
}
