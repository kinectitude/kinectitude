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
        private AudioBuffer buffer;

        public override void Ready()
        {
            device = new XAudio2();
            masteringVoice = new MasteringVoice(device);

            soundManager = GetManager<SoundManager>();
            soundManager.Add(this);

            // Add our sound to the sound library
            var s = System.IO.File.OpenRead(Path.Combine("Sounds", filename));
            WaveStream stream = new WaveStream(s);
            s.Close();

            if (!soundManager.SoundDictionary.ContainsKey(filename))
            {
                soundManager.SoundDictionary[filename] = stream;
            }
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
            /*WaveStream ws = soundManager.SoundDictionary[filename];
            ws.Position = 0;
            se.AudioData = ws;
            se.AudioBytes = (int)ws.Length;
            se.Flags = BufferFlags.None;

            SourceVoice sv = new SourceVoice(device, ws.Format);
            sv.SubmitSourceBuffer(se); // Errors here
            sv.Start();*/

            WaveStream stream = soundManager.SoundDictionary[filename];
            stream.Position = 0;

            buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            buffer.Flags = BufferFlags.EndOfStream;

            //currentlyPlaying = new SourceVoice(device, stream.Format);
            //currentlyPlaying.SubmitSourceBuffer(buffer);
            //currentlyPlaying.Start();
        }

        public void Stop()
        {
            if (currentlyPlaying != null)
            {
                currentlyPlaying.Stop();
                currentlyPlaying.FlushSourceBuffers();
            }
        }

        public void Update()
        {
            /*if (null != currentlyPlaying)
            {
                if (currentlyPlaying.State.BuffersQueued <= 0)
                {
                    // cleanup the voice
                    buffer.Dispose();
                    currentlyPlaying.Dispose();
                    //stream.Dispose();
                    //Stop();
                }
            }*/
        }

        public override void Destroy()
        {
            masteringVoice.Dispose();
            device.Dispose();
            soundManager.Remove(this);
        }
    }
}
