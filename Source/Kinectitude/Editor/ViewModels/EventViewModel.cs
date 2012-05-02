using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    public class EventViewModel
    {
        private readonly Event evt;

        public EventViewModel(Event evt)
        {
            this.evt = evt;
        }
    }
}
