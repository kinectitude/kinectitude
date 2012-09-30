using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using System.Collections.Generic;

namespace Kinectitude.Sound
{
    public class SoundManager : Manager<SoundComponent>
    {
        public SoundManager()
        {
            SoundDictionary = new Dictionary<string, WaveStream>();
        }

        // Create a mapping of our sound filename to the wavestream object for easy retrieval
        // when we want to play a sound
        public Dictionary<string, WaveStream> SoundDictionary{ get; private set; }

        public override void OnUpdate(float t)
        {
            foreach (SoundComponent sc in Children)
            {
                //sc.Update();
            }
        }

        public void MuteSounds()
        {
            foreach (SoundComponent sc in Children)
            {
                sc.Stop();
            }
        }
    }
}
