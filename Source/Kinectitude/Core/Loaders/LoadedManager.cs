using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedManager : LoadedObject
    {
        private readonly string Type;
        private static readonly Dictionary<LoadedScene, Dictionary<string, LoadedManager>> ManagerMap =
            new Dictionary<LoadedScene, Dictionary<string, LoadedManager>>();

        private LoadedManager(string type, PropertyHolder values, LoaderUtility loaderUtil) : base(values, loaderUtil)
        {
            Type = type;
        }

        internal static LoadedManager GetLoadedManager(string type, LoadedScene ls, PropertyHolder values, LoaderUtility loaderUtil)
        {
            Dictionary<string, LoadedManager> managersByName;
            
            if(!ManagerMap.TryGetValue(ls, out managersByName))
            {
                ManagerMap[ls] =  managersByName = new Dictionary<string, LoadedManager>();
            }

            LoadedManager loadedManager;
            if (!managersByName.TryGetValue(type, out loadedManager))
            {
                loadedManager = new LoadedManager(type, values, loaderUtil);
                managersByName[type] = loadedManager;
            }
            else
            {
                if (values != null) loadedManager.Values.AddRange(values);
            }
            return loadedManager;
        }

        internal IManager CreateManager(Scene scene)
        {
            IManager manager = ClassFactory.Create<IManager>(Type);
            setValues(manager, null, null, scene);
            return manager;
        }
    }
}
