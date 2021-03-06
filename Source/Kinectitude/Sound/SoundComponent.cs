//-----------------------------------------------------------------------
// <copyright file="SoundComponent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.ComponentInterfaces;
using Kinectitude.Core.Components;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using System.Threading;
using System.IO;

namespace Kinectitude.Sound
{
    [Plugin("Sound Component", "")]
    [Provides(typeof(ISound))]
    public class SoundComponent : ISound
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
        [PluginProperty("File Name", "")]
        public string Filename
        {
            get { return filename; }
            set
            {
                if (filename != value)
                {
                    filename = value;
                }
            }
        }

        private bool looping = false;
        [PluginProperty("Looping", "")]
        public bool Looping
        {
            get { return looping; }
            set
            {
                looping = value;
            }
        }

        private float volume = 1.0f;
        [PluginProperty("Volume", "")]
        public float Volume
        {
            get { return volume; }
            set
            {
                if (volume != value)
                {
                    volume = value;
                }
            }
        }

        public void Play()
        {
            WaveStream stream;

            if (!soundManager.SoundDictionary.ContainsKey(filename))
            {
                // Add our sound to the sound library
                var s = System.IO.File.OpenRead(Path.Combine("Assets", filename));
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

            if (Looping == true)
            {
                buffer.LoopCount = XAudio2.LoopInfinite;
                buffer.LoopLength = 0;
            }

            currentlyPlaying = new SourceVoice(soundManager.device, format);
            currentlyPlaying.Volume = this.Volume;
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
                currentlyPlaying.Dispose();
                buffer.Dispose();
                playing = false;
            }
        }

        public void OnUpdate(float t)
        {

        }

        public void Destroy()
        {
            buffer.Dispose();
            currentlyPlaying.Dispose();
        }
    }
}
