using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public abstract class ReadableData
    {
        internal static ReadableData CreateReadableData(string value, Event evt, SceneLoader sceneLoader)
        {
            if (value.Contains(','))
            {
                List<ReadableData> readableList = new List<ReadableData>();
                string[] readables = value.Split(',');
                foreach (string readable in readables)
                {
                    readableList.Add(CreateReadableData(readable, evt, sceneLoader));
                }
                return new MultiReader(readableList);
            }
            //this would be like saying the same readable as other
            if ('!' == value[0])
            {
                value = value.Substring(1);
                return evt.AvailableSelectors[value];
            }
            if ('$' == value[0])
            {
                value = value.Substring(1);
                return new PrototypeReader(sceneLoader.IsType[value]);
            }
            if ('#' == value[0])
            {
                value = value.Substring(1);
                return new PrototypeReader(sceneLoader.IsExactType[value]);
            }
            if ("game" == value)
            {
                return new ConstantReadableData(sceneLoader.Game);
            }
            if ("scene" == value)
            {
                return new ConstantReadableData(sceneLoader.Scene);
            }
            if ("this" == value)
            {
                return new ConstantReadableData(evt.Entity);
            }
            return new ConstantReadableData(evt.Entity);
        }

        internal DataContainer DataContainer { get; set; }

        public abstract bool MatchAndSet(DataContainer dataContainer);

        public string GetValue(string key)
        {
            return DataContainer[key];
        }
    }
}
