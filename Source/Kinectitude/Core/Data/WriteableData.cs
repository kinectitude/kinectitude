using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core
{
    public class WriteableData
    {
        internal static WriteableData CreateWriteableData(string value, Entity entity, Game game, Scene scene)
        {
            switch (value)
            {
                case "this":
                    return new WriteableData(entity);
                case "game":
                    return new WriteableData(game);
                case "scene":
                    return new WriteableData(scene);
            }
            throw new ArgumentException("You can only write to your entity, the secene, or the game");
        }
        
        public DataContainer DataContainer { get; protected set; }

        public string this[string key]
        {
            get
            {
                return DataContainer[key];
            }
            set
            {
                DataContainer[key] = value;
            }
        }

        internal protected WriteableData() { }

        internal WriteableData(DataContainer dataContainer)
        {
            DataContainer = dataContainer;
        }
        
        public virtual bool IfMatchSet(DataContainer dataContainer)
        {
            return DataContainer == dataContainer;
        }
    }
}
