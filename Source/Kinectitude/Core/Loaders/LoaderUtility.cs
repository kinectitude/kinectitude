using System;
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
        object ManagerType { get; }
        object EventType { get; }
        object ComponentType { get; }
        object UsingType { get; }
        object DefineType { get; }
        object PrototypeType { get; }
        object SceneType { get; }

        object GetGame();
        PropertyHolder GetProperties(object from);
        string GetName(object from);
        IEnumerable<object> GetOfType(object from, object type);
        string GetFile(object from);
        IEnumerable<Tuple<string, string>> GetDefines(object from);
        bool IsAciton(object obj);
        IEnumerable<string> GetPrototypes(object from);
        string GetType(object from);
        object GetCondition(object from);
        IAssignable MakeAssignable(object obj, Scene scene, Entity entity, Event evt);
    }
}
