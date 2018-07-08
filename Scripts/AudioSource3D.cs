using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    class AudioSource3D : IDestroyable
    {
        static Player player;

        private GameObject owner;
        private AudioSource audioSource;

        public bool CheckVolume { get; private set; }
        public bool IsPlaying { get { return audioSource != null ? audioSource.IsPlaying : false; } }
        public bool InCameraViewPort { get; private set; }
        public bool PlayAudioWhenEnterInCamera { get; set; }
        public float VolumeOffset { get; set; }

        public AudioSource3D(GameObject owner)
        {
            if (player == null)
                player = PlayScene.Player;

            this.owner = owner;
            VolumeOffset = 0.1f;

            AudioManager.AddAudioSource3D(this);
        }

        public void Update()
        {
            if (!player.IsDead && owner.IsActive)
            {
                if (CameraManager.OutOfCameraViewPort(owner))
                {
                    if (InCameraViewPort)
                        InCameraViewPort = false;

                    if (IsPlaying)
                    {
                        StopAudio();
                    }
                }
                else
                {
                    if (!InCameraViewPort)
                        InCameraViewPort = true;

                    if (CheckVolume)
                    {
                        float maxValue = owner.Position.X;

                        float volume = (1 - (Math.Abs(player.Position.X - owner.Position.X) / maxValue)) + VolumeOffset;

                        if (volume > 1)
                            volume = 1;

                        audioSource.Volume = volume;
                    }
                }
            }
        }

        public void PlayAudio(AudioClip clip, bool loop = false, float pitch = 1f)
        {
            if (InCameraViewPort)
            {
                if (audioSource == null)
                {
                    audioSource = new AudioSource();
                }

                float maxValue = owner.Position.X;
                float volume = (1 - (Math.Abs(player.Position.X - owner.Position.X) / maxValue)) + VolumeOffset;

                if (volume > 1)
                    volume = 1;

                audioSource.Volume = volume;
                audioSource.Pitch = pitch;

                audioSource.Play(clip, loop);
                CheckVolume = true;
            }
        }

        public void StopAudio(bool dispose = true)
        {
            if (audioSource != null)
            {
                audioSource.Stop();

                if(dispose)
                    audioSource.Dispose();
            }

            CheckVolume = false;
        }

        public void Destroy()
        {
            owner = null;
            audioSource = null;
        }
    }
}
