
namespace Kinectitude.Core.Base
{
    public abstract class Service : Changeable
    {

        /// <summary>
        /// Tells a manager if it is currently in the running scene
        /// </summary>
        public bool Running { get; private set; }
        
        /// <summary>
        /// Used to start a service
        /// </summary>
        public void Start()
        {
            Running = true;
            OnStart();
        }

        /// <summary>
        /// When a service is started, this will be called after Running has already been set to true.
        /// Custom logic can go here
        /// </summary>
        public abstract void OnStart();

        public void Stop()
        {
            Running = false;
            OnStop();
        }

        /// <summary>
        /// Used to stop a service
        /// </summary>
        public abstract void OnStop();

        /// <summary>
        /// Indicates if a service should be started as soon as it loads
        /// </summary>
        /// <returns></returns>
        public abstract bool AutoStart();
    }
}
