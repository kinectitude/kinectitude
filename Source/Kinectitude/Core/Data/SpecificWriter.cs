using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public class SpecificWriter
    {
        private readonly WriteableData writeableData;
        private readonly string key;

        internal static SpecificWriter CreateSpecificWriteableData(string val, Entity entity, Scene scene)
        {
            string[] vals = val.Split('.');
            if (vals.Length == 1)
            {
                return new SpecificWriter(WriteableData.CreateWriteableData("this", entity, scene), vals[0]);
            }
            return new SpecificWriter(WriteableData.CreateWriteableData(vals[0], entity, scene), vals[1]);
        }

        private SpecificWriter(WriteableData writeableData, string key)
        {
            this.writeableData = writeableData;
            this.key = key;
        }

        public void SetValue(string value)
        {
            writeableData[key] = value;
        }
        
        public string GetValue()
        {
            return writeableData[key];
        }
    }
}
