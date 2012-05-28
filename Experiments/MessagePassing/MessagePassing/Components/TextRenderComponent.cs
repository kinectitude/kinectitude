using MessagePassing.Public;

namespace MessagePassing.Components
{
    public class TextRenderComponent : Component
    {
        private double x;
        private double y;

        public bool Success
        {
            get;
            private set;
        }
        
        public override void Initialize()
        {
            // You can also subscribe to any event in C#
            Messenger.Subscribe("Transform.X", OnTransformX);
            Messenger.Subscribe("Transform.Y", OnTransformY);
        }

        private void OnTransformX(object[] data)
        {
            x = (double)data[0];
            Success = true;
        }

        private void OnTransformY(object[] data)
        {
            y = (double)data[0];
            Success = true;
        }
    }
}
