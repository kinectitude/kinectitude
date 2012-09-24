using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using SlimDX.XAudio2;

namespace Kinectitude.Sound
{
    public class SoundManager : Manager<SoundComponent>
    {
        public SoundManager()
        {
            
        }

        public override void OnUpdate(float t)
        {

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
