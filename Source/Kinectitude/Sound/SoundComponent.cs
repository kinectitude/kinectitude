using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using System.Threading;
using System.IO;

namespace Kinectitude.Sound
{
    [Plugin("Sound Component", "")]
    [Provides(typeof(ISound))]
    public class SoundComponent : Component, ISound
    {
        private SoundManager soundManager;
        private XAudio2 device;
        private MasteringVoice masteringVoice;
        private SourceVoice currentlyPlaying;

        public override void Ready()
        {
            device = new XAudio2();
            masteringVoice = new MasteringVoice(device);

            soundManager = GetManager<SoundManager>();
            soundManager.Add(this);
        }

        private string filename;
        [Plugin("Filename", "")]
        public string Filename
        {
            get { return filename; }
            set
            {
                if (filename != value)
                {
                    filename = value;
                    Change("Filename");
                }
            }
        }

        private bool looping = false;
        [Plugin("Looping", "")]
        public bool Looping
        {
            get { return looping; }
            set
            {
                looping = value;
                Change("Looping");
            }
        }

        private float volume = 1.0f;
        [Plugin("Volume", "")]
        public float Volume
        {
            get { return volume; }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    Change("Volume");
                }
            }
        }

        public void Play()
        {
            var s = System.IO.File.OpenRead(Path.Combine("Sounds", filename));
            WaveStream stream = new WaveStream(s);
            s.Close();

            AudioBuffer buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            buffer.Flags = BufferFlags.EndOfStream;

            currentlyPlaying = new SourceVoice(device, stream.Format);
            currentlyPlaying.SubmitSourceBuffer(buffer);
            currentlyPlaying.Start();

            // loop until the sound is done playing
            while (currentlyPlaying.State.BuffersQueued > 0)
            {
                Thread.Sleep(10);
            }

            // cleanup the voice
            buffer.Dispose();
            currentlyPlaying.Dispose();
            stream.Dispose();
        }

        public void Stop()
        {
            if (currentlyPlaying != null)
            {
                currentlyPlaying.Stop();
                currentlyPlaying.FlushSourceBuffers();
            }
        }

        public override void Destroy()
        {
            masteringVoice.Dispose();
            device.Dispose();
            soundManager.Remove(this);
        }
    }
}
