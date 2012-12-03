using Kinectitude.Core.Attributes;
using Kinectitude.Sound;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Sound
{
    [Plugin("Play the sound associated with this Entity", "")]
    public class PlaySoundAction : Action
    {
        [PluginProperty("Filename", "File name of sound to play")]
        public string Filename { get; set; }

        [PluginProperty("Looping", "Whether this sound loops or not")]
        public bool Looping { get; set; }

        [PluginProperty("Volume", "The volume to play this sound at")]
        public float Volume { get; set; }

        public override void Run()
        {
            SoundComponent sc = new SoundComponent();
            sc.Filename = Filename;
            sc.Looping = Looping;
            sc.Volume = Volume;

            this.GetManager<SoundManager>().AddSound(sc);
        }
    }
}
