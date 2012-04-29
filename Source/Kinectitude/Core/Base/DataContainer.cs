using System;
using System.Collections.Generic;
using System.Xml;

namespace Kinectitude.Core
{
    public class DataContainer
    {
        private readonly Dictionary<string, string> attributes;
        private readonly Dictionary<string, List<AttributeChangesEvent>> changeEvents;
        
        internal int Id { get; private set; }
        
        public string Name { get; set; }

        public string this[string key]
        {
            get
            {
                if (attributes.ContainsKey(key))
                {
                    return attributes[key];
                }
                return null;
            }

            set
            {
                if (attributes.ContainsKey(key))
                {
                    attributes[key] = value;
                }
                else
                {
                    attributes.Add(key, value);
                }
                if (changeEvents.ContainsKey(key))
                {
                    foreach (AttributeChangesEvent evt in changeEvents[key])
                    {
                        if (evt.Trigger(this))
                        {
                            evt.DoActions();
                        }
                    }
                }
            }
        }

        public DataContainer(int id) 
        {
            this.Id = id;

            attributes = new Dictionary<string, string>();
            changeEvents = new Dictionary<string, List<AttributeChangesEvent>>();
        }

        internal void AddAttributeChangesEvent(AttributeChangesEvent evt)
        {
            if (!changeEvents.ContainsKey(evt.Key))
            {
                changeEvents[evt.Key] = new List<AttributeChangesEvent>();
            }
            changeEvents[evt.Key].Add(evt);
        }
    }
}
