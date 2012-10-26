using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Data;

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
        private readonly Dictionary<Type, LoadedComponent> componentDict = new Dictionary<Type, LoadedComponent>();

        private readonly List<LoadedEvent> events = new List<LoadedEvent>();
        private readonly string Name;
        private readonly List<string> isType = new List<string>();
        private readonly List<string> isExactType = new List<string>();

        private bool firstCreate = true;

        int id;

        internal static readonly Dictionary<string, LoadedEntity> Prototypes = new Dictionary<string, LoadedEntity>();

        internal LoadedEntity(string name, PropertyHolder values, int id, IEnumerable<string> prototypes, LoaderUtility loaderUtil)
            : base(values, loaderUtil)
        {
            Name = name;
            this.id = id;
            foreach (string prototype in prototypes)
            {
                isExactType.Add(prototype);
                isType.Add(prototype);
                isType.AddRange(Prototypes[prototype].isType);
            }

            //it is a prototype
            if (id < 0) Prototypes.Add(name, this);
        }

        internal void AddLoadedComponent(LoadedComponent component)
        {
            components.Add(component);
            componentSet.Add(component.Type);
            componentDict[component.Type] = component;
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

            foreach (Tuple<string, object> value in Values)
            {
                IAssignable assignable = LoaderUtil.MakeAssignable(value.Item2, scene, entity, null);
                entity[value.Item1] = assignable as ValueReader;
            }

            scene.EntityById[entity.Id] = entity;
            entity.Scene = scene;
            if (null != entity.Name) scene.EntityByName.Add(entity.Name, entity);
            foreach (string type in isType) addToType(scene.IsType, type, entity.Id);
            foreach (string type in isExactType) addToType(scene.IsExactType, type, entity.Id);

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

        internal void Prepare()
        {
            foreach(string prototypeName in isExactType)
            {
                LoadedEntity prototype = Prototypes[prototypeName];
                Values.MergeWith(prototype.Values);
                needs.AddRange(prototype.needs);
                componentSet.UnionWith(prototype.componentSet);
                foreach (KeyValuePair<Type, LoadedComponent> entry in prototype.componentDict)
                {
                    LoadedComponent component;
                    if (componentDict.TryGetValue(entry.Key, out component))
                    {
                        component.Values.MergeWith(entry.Value.Values);
                    }
                    else
                    {
                        componentDict[entry.Key] = entry.Value;
                        components.Add(entry.Value);
                    }
                }
                events.AddRange(prototype.events);
            }
        }

        private void addToType(Dictionary<string, HashSet<int>> addTo, string type, int id)
        {
            HashSet<int> addSet = null;
            if (!addTo.TryGetValue(type, out addSet))
            {
                addSet = new HashSet<int>();
                addTo.Add(type, addSet);
            }
            addSet.Add(id);
        }
    }
}
