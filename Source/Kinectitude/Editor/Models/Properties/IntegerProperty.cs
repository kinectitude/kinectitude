
namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class IntegerProperty : Property<int>
    {
        public IntegerProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            int parsed = 0;
            bool ret = int.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }
    }
}
