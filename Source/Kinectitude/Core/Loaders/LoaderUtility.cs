﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Data;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    interface LoaderUtility
    {
        object EntityType { get; }
        object ActionType { get; }
        object ConditionType { get; }
        object ServiceType { get; }
        object ManagerType { get; }
        object EventType { get; }
        object ComponentType { get; }
        object UsingType { get; }
        object DefineType { get; }
        object PrototypeType { get; }
        object SceneType { get; }
        object Else { get; }

        object GetGame();
        PropertyHolder GetProperties(object from);
        string GetName(object from);
        IEnumerable<object> GetOfType(object from, object type);
        string GetFile(object from);
        IEnumerable<Tuple<string, string>> GetDefines(object from);
        bool IsAciton(object obj);
        bool IsAssignment(object obj);
        IEnumerable<string> GetPrototypes(object from);
        string GetType(object from);
        object GetCondition(object from);
        object MakeAssignable(object obj, Scene scene = null, Entity entity = null, Event evt = null);
        /// <summary>
        /// Tuple is Target, type (*=, /= etc) or null for =, then Value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Tuple<object, object, object> GetAssignment(object obj);
        ValueReader MakeAssignmentValue(ValueReader ls, object type, ValueReader rs);
    }
}
