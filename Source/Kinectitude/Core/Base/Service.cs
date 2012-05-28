
namespace Kinectitude.Core.Base
{
    public abstract class Service
    {
        public bool Running { get; private set; }
        
        public void Start()
        {
            Running = true;
            OnStart();
        }

        public abstract void OnStart();

        public void Stop()
        {
            Running = false;
            OnStop();
        }

        public abstract void OnStop();

        public abstract bool AutoStart();
    }
}
