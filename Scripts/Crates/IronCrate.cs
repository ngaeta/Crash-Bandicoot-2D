using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class IronCrate : Crate
    { 
        protected AudioSource audioSource;
        protected AudioClip clipOnHit;

        public IronCrate(Vector2 spritePosition, string spriteSheetName="ironCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            audioSource = new AudioSource();
            clipOnHit = AudioManager.GetAudioClip("ironCrateHit");
            clipFootStep = AudioManager.GetAudioClip("metal2FootStep");
        }

        public override void OnHit(GameObject hitObject)
        {
            base.OnHit(hitObject);

            if(hitObject is Player p && p.IsInvincible)
            {
                return;
            }

            if(!audioSource.IsPlaying)
                audioSource.Play(clipOnHit);
        }
    }
}
