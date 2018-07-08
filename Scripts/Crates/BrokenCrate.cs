using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class BrokenCrate : GameObject
    {
        private AudioSource audioSource;

        public Vector2 Scale { set { sprite.scale = value; } }
        public AudioClip ClipBroken { get; protected set; }

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                if (value)
                {
                    if (Animation != null)
                    {
                        Animation.IsActive = true;
                    }

                    if (ClipBroken != null)
                    {
                        audioSource = new AudioSource();
                        audioSource.Play(ClipBroken);
                    }
                }

                base.IsActive = value;
            }
        }

        public BrokenCrate(Vector2 spritePosition, string spriteSheetName = "brokenCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            ClipBroken = AudioManager.GetAudioClip("crateBreak");
            IsActive = false;
        }

        public override void Update()
        {
            base.Update();

            if (IsActive)
            {
                if (!Animation.IsPlaying)
                {
                    OnDie();
                }
            }
        }

        protected virtual void OnDie()
        {
            IsActive = false;
            Animation.Reset();

            AudioManager.DisposeAudioSource(audioSource);
        }
    }
}
