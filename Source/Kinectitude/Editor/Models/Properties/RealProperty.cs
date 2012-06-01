
namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class RealProperty : Property<double>
    {
        public RealProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            double parsed = 0;
            bool ret = double.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }
    }
}
