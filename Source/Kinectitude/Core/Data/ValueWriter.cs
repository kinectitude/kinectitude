using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Data
{
    internal abstract class ValueWriter : IValueWriter
    {
        protected readonly DataContainer DataContainer;
        
        internal static ValueWriter CreateValueWriter(string key, Entity entity)
        {
            if (key.Contains('.'))
            {
                bool isDef = true;
                string [] keyParts = key.Split('.');
                DataContainer dc;
                switch (keyParts[0])
                {
                    case "scene":
                        dc = entity.Scene;
                        break;
                    case "game":
                        dc = entity.Scene.Game;
                        break;
                    case "this":
                        dc = entity;
                        break;
                    default:
                        if (keyParts.Length != 2) throw new InvalidValueWriterException();
                        dc = entity;
                        isDef = false;
                        break;
                }
                if (isDef)
                {
                    switch (keyParts.Length)
                    {
                        case 2:
                            return new AttributeWriter(keyParts[1], dc);
                        case 3:
                            return new PropertyWriter(keyParts[1], keyParts[2], dc);
                        default:
                            throw new InvalidValueWriterException();
                    }
                }
                else
                {
                    return new PropertyWriter(keyParts[0], keyParts[1], dc);
                }
            }
            return new AttributeWriter(key, entity);
        }

        protected ValueWriter(DataContainer dc)
        {
            DataContainer = dc;
        }


        public abstract string Value { get; set; }
    }
}
