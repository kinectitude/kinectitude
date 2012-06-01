
namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class BooleanProperty : Property<bool>
    {
        public BooleanProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            bool parsed = false;
            bool ret = bool.TryParse(input, out parsed);
            Value = parsed;
            return ret;
        }
    }
}
