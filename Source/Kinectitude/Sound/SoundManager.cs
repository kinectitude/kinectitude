using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using System.Collections.Generic;

namespace Kinectitude.Sound
{
    [Plugin("Sound Manager", "")]
    public class SoundManager : Manager<SoundComponent>
    {
        public XAudio2 device;
        public MasteringVoice masteringVoice;

        public SoundManager()
        {
            device = new XAudio2();
            masteringVoice = new MasteringVoice(device);
            SoundDictionary = new Dictionary<string, WaveStream>();
        }

        // Create a mapping of our sound filename to the wavestream object for easy retrieval
        // when we want to play a sound
        public Dictionary<string, WaveStream> SoundDictionary{ get; private set; }

        public void AddSound(SoundComponent sc)
        {
            this.Add(sc);
            sc.Setup(this);
            sc.Play();
        }

        public override void OnUpdate(float t)
        {
            List<SoundComponent> temp = Children;
            foreach (SoundComponent sc in temp)
            {
                if (!sc.Playing && !sc.Looping)
                {
                    sc.Destroy();
                    this.Remove(sc);
                }
                else
                {
                    sc.OnUpdate(t);
                }
            }
        }

        public void MuteAllSounds()
        {
            foreach (SoundComponent sc in Children)
            {
                sc.Stop();
            }
        }

        public void StopSound(string filename)
        {
            List<SoundComponent> temp = Children;
            foreach (SoundComponent sc in temp)
            {
                if (sc.Filename == filename)
                {
                    sc.Stop();
                    this.Remove(sc);
                }
            }

            SoundDictionary.Remove(filename);
        }
    }
}
