//-----------------------------------------------------------------------
// <copyright file="LoadedComponent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedComponent : LoadedObject
    {
        internal readonly string Name;
        internal readonly Type Type;

        internal LoadedComponent(string name, PropertyHolder values, LoaderUtility loaderUtil)
            : base(values, loaderUtil)
        {
            Name = name;
            Type = ClassFactory.TypesDict[Name];
        }

        internal Component Create(Entity entity)
        {
            Component created = ClassFactory.Create<Component>(Name);
            created.Entity = entity;
            setValues(created, null, entity, entity.Scene);
            return created;
        }

        internal LoadedComponent clone()
        {
            return new LoadedComponent(Name, Values.clone(), LoaderUtil);
        }
    }
}
