using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.ViewModels
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
