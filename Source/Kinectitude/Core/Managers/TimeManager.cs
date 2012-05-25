using Kinectitude.Core.Base;

namespace Kinectitude.Core.Managers
{
    class TimeManager : Manager<Component>
    {

        public TimeManager() { }

        public override void OnUpdate(float t)
        {
            foreach (Component c in Children)
            {
                c.OnUpdate(t);
            }
        }
    }
}
