﻿using Kinectitude.Core.Base;
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
        private SourceVoice currentlyPlaying;
        private AudioBuffer buffer;
        private bool playing;

        public void Setup(SoundManager sm)
        {
            soundManager = sm;
        }

        public bool Playing
        {
            get { return playing; }
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
            WaveStream stream;

            if (!soundManager.SoundDictionary.ContainsKey(filename))
            {
                // Add our sound to the sound library
                var s = System.IO.File.OpenRead(Path.Combine("Sounds", filename));
                stream = new WaveStream(s);
                s.Close();
                soundManager.SoundDictionary[filename] = stream;
            }
            else
            {
                stream = soundManager.SoundDictionary[filename];
            }

            WaveFormat format = stream.Format;

            buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            buffer.Flags = BufferFlags.EndOfStream;
            buffer.AudioData.Position = 0;

            currentlyPlaying = new SourceVoice(soundManager.device, format);
            currentlyPlaying.BufferEnd += (s, e) => playing = false;
            currentlyPlaying.Start();
            currentlyPlaying.SubmitSourceBuffer(buffer);

            playing = true;
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

        }

        public override void Destroy()
        {
            buffer.Dispose();
            currentlyPlaying.Dispose();
        }
    }
}