using System.Collections.Generic;
using System.Linq;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public abstract class ReadableData
    {
        internal static ReadableData CreateReadableData(string value, Event evt, Scene scene)
        {
            if (value.Contains(','))
            {
                List<ReadableData> readableList = new List<ReadableData>();
                string[] readables = value.Split(',');
                foreach (string readable in readables)
                {
                    readableList.Add(CreateReadableData(readable, evt, scene));
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
                return new PrototypeReader(scene.IsType[value]);
            }
            if ('#' == value[0])
            {
                value = value.Substring(1);
                return new PrototypeReader(scene.IsExactType[value]);
            }
            if ("game" == value)
            {
                return new ConstantReadableData(scene.Game);
            }
            if ("scene" == value)
            {
                return new ConstantReadableData(scene);
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
