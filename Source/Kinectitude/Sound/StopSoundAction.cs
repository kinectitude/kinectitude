using Kinectitude.Core.Attributes;
using Kinectitude.Sound;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Sound
{
    [Plugin("Stop the sound associated with this entity", "")]
    public class StopSoundAction : Action
    {
        public StopSoundAction() { }

        public override void Run()
        {
            GetComponent<SoundComponent>().Stop();
        }
    }
}