using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedManager : LoadedObject
    {
        private readonly string Type;
        private static readonly Dictionary<LoadedScene, Dictionary<string, LoadedManager>> ManagerMap =
            new Dictionary<LoadedScene, Dictionary<string, LoadedManager>>();

        private LoadedManager(string type, List<Tuple<string ,string>> values):base(values)
        {
            Type = type;
        }

        internal static LoadedManager GetLoadedManager(string type, LoadedScene ls, List<Tuple<string, string>> values)
        {
            Dictionary<string, LoadedManager> managersByName;
            
            if(!ManagerMap.TryGetValue(ls, out managersByName))
            {
                ManagerMap[ls] =  managersByName = new Dictionary<string, LoadedManager>();
            }

            LoadedManager loadedManager;
            if (!managersByName.TryGetValue(type, out loadedManager))
            {
                loadedManager = new LoadedManager(type, values);
                managersByName[type] = loadedManager;
            }
            else
            {
                if (values != null) loadedManager.Values.AddRange(values);
            }
            return loadedManager;
        }

        internal IManager CreateManager()
        {
            IManager manager = ClassFactory.Create<IManager>(Type);
            setValues(manager, null, null);
            return manager;
        }
    }
}
