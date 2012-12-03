using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Managers
{
    /// <summary>
    /// A basic manager used to update components on how much time has passed since the last update.
    /// </summary>
    [Plugin("Time Manager", "")]
    public class TimeManager : Manager<Component>
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
