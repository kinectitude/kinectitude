using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class PropertyWriter : ValueWriter
    {
        //keep as string, if the manager is not yet set, then I need to get it
        private readonly string who;
        private readonly string set;

        internal PropertyWriter(string who, string set, DataContainer dc) : base(dc) 
        {
            this.who = who;
            this.set = set;
        }

        public override string Value
        {
            get
            {
                object valObj = DataContainer.GetComponentOrManager(who);
                return ClassFactory.GetStringParam(valObj, set);
            }
            set
            {
                object valObj = DataContainer.GetComponentOrManager(who);
                ClassFactory.SetParam(valObj, set, value, null, null);
            }
        }
    }
}
