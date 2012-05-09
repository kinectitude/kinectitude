using Kinectitude.Attributes;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Events
{
    [Plugin("Scene starts", "")]
    public class SceneStartsEvent : Event
    {
        public SceneStartsEvent() { }

        public override void OnInitialize()
        {
            Scene.OnStart.Add(this);
        }
    }
}