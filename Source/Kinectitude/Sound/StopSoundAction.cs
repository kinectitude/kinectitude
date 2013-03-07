using Kinectitude.Core.Attributes;
using Kinectitude.Sound;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Sound
{
    [Plugin("stop playing sound {Filename}", "Stop a sound")]
    public class StopSoundAction : Action
    {
        [PluginProperty("File Name", "File name of the sound to stop", 
                         null, 
                        "Waveform Audio Files (.wav)|*.wav;*.wave",
                        "Select the sound file to stop")]
        public string Filename { get; set; }

        public override void Run()
        {
            this.GetManager<SoundManager>().StopSound(Filename);
        }
    }
}