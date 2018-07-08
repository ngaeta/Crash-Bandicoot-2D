using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class IronBounceCrate : IronCrate
    {
        protected BounceBehaviour bounceBehaviour;
        protected AudioClip clipBounce;

        private AudioSource audioSourceBounce;

        public IronBounceCrate(Vector2 spritePosition, string spriteSheetName = "ironBounceCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            bounceMultiplierY += 0.5f;
            bounceBehaviour = new BounceBehaviour(this, bounceMultiplierY);
            audioSourceBounce = new AudioSource();
            clipBounce = AudioManager.GetAudioClip("crateBounce");
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            float deltaY = colllisionInfo.Delta.Y;

            if (deltaY < 0)
            {
                bounceMultiplierY = -bounceMultiplierY;
            }
            else if (PlayerImpactY >= minInpactToDetectYCollision)
            {
                bounceMultiplierY = Math.Abs(bounceMultiplierY);
                bounceBehaviour.Bounce(player);
                audioSourceBounce.Play(clipBounce);
            }
        }

        public override void OnHit(GameObject hitObject)
        {
            base.OnHit(hitObject);
        }
    }
}
