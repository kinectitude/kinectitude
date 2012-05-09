
namespace Kinectitude.Core.Data
{
    public class SpecificContainerReadable : SpecificReadable
    {
        private readonly string value;

        public ReadableData ReadableSelector { get; private set; }

        internal SpecificContainerReadable(string value, ReadableData readableSelector)
        {
            ReadableSelector = readableSelector;
            this.value = value;
        }
        
        public override string GetValue()
        {
            return ReadableSelector.DataContainer[value];
        }
    }
}
