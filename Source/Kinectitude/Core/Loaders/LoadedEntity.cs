using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedEntity : LoadedObject
    {
        //used to see that the entity has everything it needs
        private readonly Dictionary<Type, LoadedComponent> componentDictionary = 
            new Dictionary<Type, LoadedComponent>();

        //used to see that everything needed is provided
        private readonly List<Type> needs = new List<Type>();

        //used to save the loaded components
        private readonly List<LoadedComponent> components = new List<LoadedComponent>();

        private readonly List<LoadedEvent> events = new List<LoadedEvent>();


        private readonly string name;

        private bool firstCreate = true;

        internal LoadedEntity(string name, List<Tuple<string, string>> values): base(values)
        {
            this.name = name;
        }

        internal void AddLoadedComponent(LoadedComponent component)
        {
            components.Add(component);
            componentDictionary.Add(component.ComponentType, component);
            foreach (Type type in ClassFactory.GetRequirements(component.ComponentType))
            {
                needs.Add(type);
            }

            foreach (Type type in ClassFactory.GetProvided(component.GetType()))
            {
                componentDictionary.Add(type, component);
            }
        }

        internal Entity Create(int id)
        {

            if (firstCreate)
            {
                List<Type> missing = new List<Type>();
                foreach (Type type in needs)
                {
                    if (!componentDictionary.ContainsKey(type))
                    {
                        if (!missing.Contains(type))
                        {
                            missing.Add(type);
                        }
                    }
                }

                if (missing.Count != 0)
                {
                    string identity = null != name ? name : "Entity " + id.ToString();
                    throw MissingRequirementsException.MissingRequirement(identity, missing);
                }
                firstCreate = false;
            }

            Entity entity = new Entity(id);

            setValues(entity, null);

            foreach (LoadedEvent loadedEvent in events)
            {
                Event evt = loadedEvent.Create(entity);
                evt.Initialize();
            }
            return entity;
        }
    }
}
