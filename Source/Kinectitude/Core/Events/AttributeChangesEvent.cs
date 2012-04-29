using System.Xml;
using System;
using System.Linq;
using System.Collections.Generic;
using Kinectitude.Attributes;

namespace Kinectitude.Core
{
    // Use this as a base class for any attribute change.  It will always fire

    [Plugin("Attribute value changes", "")]
    internal class AttributeChangesEvent : Event
    {
        private string target;

        [Plugin("Key", "")]
        public string Target
        {
            get
            {
                return target;
            }
            set
            {
                if (value.Contains('.'))
                {
                    string[] val = value.Split('.');
                    target = val[0];
                    Key = val[1];
                }
                else
                {
                    target = "this";
                    Key = value;
                }
            }
        }

        internal string Key { get; private set; }

        public AttributeChangesEvent() { }

        public override void OnInitialize()
        {
            if ("game" == target)
            {
                Game.AddAttributeChangesEvent(this);
            }
            else if ("scene" == target)
            {
                Scene.AddAttributeChangesEvent(this);
            }
            else
            {
                Entity.AddAttributeChangesEvent(this);
            }
        }

        public virtual bool Trigger(DataContainer dataContainer)
        {
            return true;
        }
    }
}
