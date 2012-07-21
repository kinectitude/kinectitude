using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Loaders
{
    internal abstract class LoaderUtility
    {
        internal object EntityType;
        internal object ActionType;
        internal object ConditionType;
        internal object ManagerType;
        internal object EventType;
        internal object TriggerType;
        internal object ComponentType;
        internal object UsingType;
        internal object DefineType;
        internal object PrototypeType;
        internal object SceneType;

        protected string FileName;

        internal LanguageKeywords Lang { get; set; }

        internal abstract object Load();
        internal abstract List<Tuple<string, string>> GetValues(object from, HashSet<string> ignore);
        internal abstract Dictionary<string, string> GetProperties(object from, HashSet<string> specialWords);
        internal abstract IEnumerable<object> GetOfType(object entity, object type);
        internal abstract IEnumerable<object> GetAll(object evt);
        internal abstract bool IsAciton(object obj);
        internal abstract void CreateWithPrototype(GameLoader gl, string name, ref object entity, int id);
        internal abstract void MergePrototpye(ref object newPrototype, string myName, string mergeWith);
    }
}
