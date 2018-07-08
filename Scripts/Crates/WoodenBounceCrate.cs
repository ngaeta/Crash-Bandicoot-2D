using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class WoodenBounceCrate : DestructibleCrate
    {
        protected BounceBehaviour bounceBehaviour;
        protected AudioClip clipBounce;

        private AudioSource audioSourceBounce;

        public WoodenBounceCrate(Vector2 spritePosition, string spriteSheetName = "woodenBounceCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            Animation.IsActive = false; //sprite è quella vecchia con piu immagini, cambiare
            bounceMultiplierY += 0.5f;
            bounceBehaviour = new BounceBehaviour(this, bounceMultiplierY);
            
            clipBounce = AudioManager.GetAudioClip("crateBounce");
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            float deltaY = colllisionInfo.Delta.Y;
            
            if (deltaY < 0)
            {
                OnHit(player);
            }
            else if (PlayerImpactY >= minInpactToDetectYCollision)
            {
                bounceMultiplierY = Math.Abs(bounceMultiplierY);
                bounceBehaviour.Bounce(player);

                audioSourceBounce = new AudioSource();
                audioSourceBounce.Play(clipBounce);
                AudioManager.DisposeAudioSource(audioSourceBounce);
            }
            //else //dici al player che è grounded? mentre il player se è bounce non fa nulla??? è il player che chiama Bounce?? on collision del player viene chiamato alla fine
        }
    }
}
