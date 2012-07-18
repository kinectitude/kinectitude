using System.ComponentModel;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EditorModels.ViewModels
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        private Dictionary<string, IEnumerable<string>> dependencies;

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        protected class DependsOnAttribute : Attribute
        {
            public string Property
            {
                get;
                private set;
            }

            public DependsOnAttribute(string property)
            {
                Property = property;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseViewModel()
        {
            dependencies = new Dictionary<string, IEnumerable<string>>();
        }

        protected void NotifyPropertyChanged(string name)
        {
            if (null != PropertyChanged)
            {
                foreach (string property in AllNotifiedProperties(name))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        private IEnumerable<string> DependentProperties(string name)
        {
            return from property in GetType().GetProperties()
                   where property.GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>().Any(x => x.Property == name)
                   select property.Name;
        }

        private IEnumerable<string> NotifiedProperties(IEnumerable<string> properties)
        {
            IEnumerable<string> dependentProperties = from property in properties
                                                      from dependentProperty in DependentProperties(property)
                                                      select dependentProperty;

            return properties.Union(dependentProperties).Distinct();
        }

        private IEnumerable<string> AllNotifiedProperties(string name)
        {
            IEnumerable<string> results;
            dependencies.TryGetValue(name, out results);

            if (null == results)
            {
                results = new[] { name };
                while (NotifiedProperties(results).Count() > results.Count())
                {
                    results = NotifiedProperties(results);
                }

                dependencies[name] = results;
            }

            return results;
        }
    }
}
