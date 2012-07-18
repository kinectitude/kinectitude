using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedComponent : LoadedObject
    {
        internal readonly string Name;
        internal readonly Type Type;

        internal LoadedComponent(string name, List<Tuple<string, string>> values) : base(values) 
        {
            Name = name;
            Type = ClassFactory.TypesDict[Name];
        }

        internal Component Create(Entity entity)
        {
            Component created = ClassFactory.Create<Component>(Name);
            created.Entity = entity;
            setValues(created, null, entity);
            return created;
        }
    }
}
