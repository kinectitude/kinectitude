using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Events
{
    [Plugin("Scene starts", "")]
    internal sealed class SceneStartsEvent : Event
    {
        public SceneStartsEvent() { }

        public override void OnInitialize()
        {
            Scene scene = Entity.Scene;
            scene.OnStart.Add(this);
        }
    }
}