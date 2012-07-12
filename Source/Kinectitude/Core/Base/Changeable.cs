using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    public class Changeable
    {
        internal DataContainer DataContainer { get; set; }

        internal bool ShouldCheck = false;

        private readonly string change;

        protected Changeable()
        {
            change = ClassFactory.GetReferedName(GetType());
        }

        protected void Change(string str)
        {
            if (ShouldCheck)
            {
                //TODO there must be a better way?
                string value = ClassFactory.GetStringParam(this, str);
                DataContainer.ChangedProperty(change + '.' + str, value);
            }
        }
    }
}
