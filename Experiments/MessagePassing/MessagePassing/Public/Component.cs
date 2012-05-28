
namespace MessagePassing.Public
{
    public class Component : IUpdateable
    {
        private IMessenger messenger;
        private IComponentContainer componentContainer;

        public IMessenger Messenger
        {
            get { return messenger; }
            set
            {
                if (null == messenger)
                {
                    messenger = value;
                }
            }
        }

        public IComponentContainer ComponentContainer
        {
            get { return componentContainer; }
            set
            {
                if (null == componentContainer)
                {
                    componentContainer = value;
                }
            }
        }

        public Component() { }

        public virtual void Initialize() { }

        public virtual void Update(double frameDelta) { }
    }
}
