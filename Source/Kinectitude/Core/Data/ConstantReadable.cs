
namespace Kinectitude.Core.Data
{
    internal class ConstantReadable : SpecificReadable
    {
        private readonly string value;

        internal ConstantReadable(string value)
        {
            this.value = value;
        }
        
        public override string GetValue()
        {
            return value;
        }
    }
}
