using Kinectitude.Core.Attributes;
using Kinectitude.Sound;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Sound
{
    [Plugin("Stop the sound associated with this Entity", "")]
    public class StopSoundAction : Action
    {
        [PluginProperty("Filename", "File name of the sound to stop")]
        public string Filename { get; set; }

        public override void Run()
        {
            this.GetManager<SoundManager>().StopSound(Filename);
        }
    }
}