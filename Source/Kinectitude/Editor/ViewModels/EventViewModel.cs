using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.ViewModels
{
    internal class EventViewModel : BaseModel
    {
        private readonly AbstractEvent evt;
        private List<object> header;

        public IEnumerable<object> Header
        {
            get { return header; }
        }

        public EventViewModel(AbstractEvent evt)
        {
            this.evt = evt;

            header = new List<object>();

            string headerString = evt.Header;

            /*while (headerString.Length > 0)
            {
                int idx = headerString.IndexOf('{');
                string segment = headerString.
            }*/
        }
    }
}
