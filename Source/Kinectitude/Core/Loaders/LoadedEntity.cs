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
        private readonly HashSet<Type> componentSet = new HashSet<Type>();

        //used to see that everything needed is provided
        private readonly List<Type> needs = new List<Type>();

        //used to save the loaded components
        private readonly List<LoadedComponent> components = new List<LoadedComponent>();

        private readonly List<LoadedEvent> events = new List<LoadedEvent>();
        private readonly string Name;
        private readonly List<string> isType;
        private readonly List<string> isExactType;

        private bool firstCreate = true;

        int id;

        internal LoadedEntity(string name, List<Tuple<string, string>> values, 
            int id, List<string> isType, List<string> isExactType): base(values)
        {
            Name = name;
            this.id = id;
            this.isExactType = isExactType;
            this.isType = isType;
        }

        internal void AddLoadedComponent(LoadedComponent component)
        {
            components.Add(component);
            componentSet.Add(component.Type);
            foreach (Type type in ClassFactory.GetRequirements(component.Type))
            {
                needs.Add(type);
            }

            foreach (Type type in ClassFactory.GetProvided(component.Type))
            {
                componentSet.Add(type);
            }
        }

        internal Entity Create(int id, Scene scene)
        {

            if (firstCreate)
            {
                List<Type> missing = new List<Type>();
                foreach (Type type in needs)
                {
                    if (!componentSet.Contains(type))
                    {
                        if (!missing.Contains(type))
                        {
                            missing.Add(type);
                        }
                    }
                }

                if (missing.Count != 0)
                {
                    string identity = null != Name ? Name : "Entity " + id.ToString();
                    throw MissingRequirementsException.MissingRequirement(identity, missing);
                }
                firstCreate = false;
            }

            Entity entity = new Entity(id);
            entity.Name = Name;
            entity.Scene = scene;

            setValues(entity);

            foreach (LoadedComponent loadedComponent in components)
            {
                entity.AddComponent(loadedComponent.Create(entity), loadedComponent.Name);
            }

            foreach (LoadedEvent loadedEvent in events)
            {
                Event evt = loadedEvent.Create(entity);
                evt.Entity = entity;
                evt.Initialize();
            }

            return entity;
        }

        internal Entity Create(Scene scene)
        {
            return Create(id, scene);
        }

        internal void AddLoadedEvent(LoadedEvent evt)
        {
            events.Add(evt);
        }

    }
}
