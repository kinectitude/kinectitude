
namespace Kinectitude.Editor.Models.Properties
{
    internal sealed class TextProperty : Property<string>
    {
        public TextProperty(PropertyDescriptor descriptor) : base(descriptor) { }

        public override bool TryParse(string input)
        {
            Value = null != input ? input : string.Empty;
            return true;
        }
    }
}
