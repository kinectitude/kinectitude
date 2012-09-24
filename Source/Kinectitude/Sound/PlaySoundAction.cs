using Kinectitude.Core.Attributes;
using Kinectitude.Sound;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Sound
{
    [Plugin("Play the sound associated with this entity", "")]
    public class PlaySoundAction : Action
    {
        public PlaySoundAction() { }

        public override void Run()
        {
            GetComponent<SoundComponent>().Play();
        }
    }
}
