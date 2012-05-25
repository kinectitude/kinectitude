using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Data
{
    internal sealed class ValueWriter : IValueWriter
    {
        private readonly string key;
        private readonly DataContainer dataContainer;

        internal static ValueWriter CreateValueWriter(string key, Entity entity)
        {
            if (key.Contains('.'))
            {
                string [] keyParts = key.Split('.');
                switch (keyParts[0])
                {
                    case "this":
                        return new ValueWriter(keyParts[1], entity);
                    case "scene":
                        return new ValueWriter(keyParts[1], entity.Scene);
                    case "game":
                        return new ValueWriter(keyParts[1], entity.Scene.Game);
                    default:
                        throw new InvalidValueWriterException();
                }
            }
            return new ValueWriter(key, entity);
        }

        private ValueWriter(string key, DataContainer dataContainer)
        {
            this.dataContainer = dataContainer;
            this.key = key;
        }

        public string Value
        {
            get
            {
                return dataContainer[key];
            }
            set
            {
                dataContainer[key] = value;
            }
        }
    }
}
