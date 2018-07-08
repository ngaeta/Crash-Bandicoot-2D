using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class WaterSplash : GameObject
    {
        private AudioSource audioSource;
        private AudioClip clipSplash;

        public Vector2 Scale { get { return sprite.scale; } set { sprite.scale = value; } }  

        public WaterSplash(Vector2 spritePosition, string spriteSheetName="waterSplash", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {      
            clipSplash = AudioManager.GetAudioClip("waterSplash");
            IsActive = false;
        }

        public override void Update()
        {
            base.Update();

            if(IsActive && !Animation.IsPlaying)
            {
                IsActive = false;
                Animation.Reset();
                Scale = Vector2.One;
                clipSplash.Rewind();
            }
        }

        public void Active()
        {
            IsActive = true;
            audioSource = new AudioSource();
            audioSource.Play(clipSplash);
            AudioManager.DisposeAudioSource(audioSource);
        }
    }
}
