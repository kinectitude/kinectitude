using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Loaders
{
    //TODO make this <string, Expression>
    internal class PropertyHolder : IEnumerable<Tuple<string, object>>
    {
        internal List<Tuple<string, object>> Values = new List<Tuple<string, object>>();
        internal Dictionary<string, object> Properties = new Dictionary<string, object>();

        public IEnumerator<Tuple<string, object>> GetEnumerator()
        {
            return (IEnumerator<Tuple<string, object>>)Values.AsEnumerable<Tuple<string, object>>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) Values.AsEnumerable();
        }

        internal object this[string key] { get { return Properties[key]; } }

        internal void AddValue(string name, object value)
        {
            Values.Add(new Tuple<string, object>(name, value));
            Properties[name] = value;
        }

        internal void AddRange(PropertyHolder values)
        {
            Values.AddRange(values);
            foreach (Tuple<string, object> value in values) Properties[value.Item1] = value.Item2;
        }

        internal void MergeWith(PropertyHolder with)
        {
            foreach (Tuple<string, object> value in with)
            {
                if(!Properties.ContainsKey(value.Item1))
                {
                    Properties[value.Item1] = value.Item2;
                    Values.Add(value);
                }
            }
        }
    }
}
