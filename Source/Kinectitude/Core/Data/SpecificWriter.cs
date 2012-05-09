
namespace Kinectitude.Core.Data
{
    public class SpecificWriter
    {
        private readonly WriteableData writeableData;
        private readonly string key;

        internal SpecificWriter(WriteableData writeableData, string key)
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
