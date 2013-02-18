using Kinectitude.Core.Attributes;
using Kinectitude.Sound;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Sound
{
    [Plugin("Stop playing the following sound. Filename: {Filename}", "")]
    public class StopSoundAction : Action
    {
        [PluginProperty("Filename", "File name of the sound to stop", null, true)]
        public string Filename { get; set; }

        public override void Run()
        {
            this.GetManager<SoundManager>().StopSound(Filename);
        }
    }
}