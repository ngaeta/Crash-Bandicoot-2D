using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class AppleBounceCrate : DestructibleCrate
    {
        const int NUM_APPLES = 10;
        const int NUM_APPLES_AT_BOUNCE = 2;
        protected AudioClip clipBounce;
        protected int apples;

        private AudioSource audioSourceBounce;

        public AppleBounceCrate(Vector2 spritePosition, bool useGravity = false, string spriteSheetName = "stripedCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            Animation.IsActive = false;
            apples = NUM_APPLES;

            clipBounce = AudioManager.GetAudioClip("crateBounce");
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            float deltaY = colllisionInfo.Delta.Y;

            if (deltaY < 0)
            {
                bounceMultiplierY = -bounceMultiplierY;
                Bounce(player);
            }
            else if (PlayerImpactY >= minInpactToDetectYCollision)
            {
                bounceMultiplierY = Math.Abs(bounceMultiplierY);
                Bounce(player);
            }
            //else //dici al player che è grounded? mentre il player se è bounce non fa nulla??? è il player che chiama Bounce?? on collision del player viene chiamato alla fine
        }

        private void Bounce(Player player)
        {
            if (audioSourceBounce == null)
                audioSourceBounce = new AudioSource();

            if (!audioSourceBounce.IsPlaying)
            {
                audioSourceBounce.Play(clipBounce);
            }

            Animation.Reset();
            Animation.NextFrame();
            player.Jump(bounceMultiplierY);

            apples -= NUM_APPLES_AT_BOUNCE;

            List<Pickable> appleList = ItemPickableManager.GetItems(PickableType.Apple, NUM_APPLES_AT_BOUNCE);
            Vector2 offset = Vector2.Zero;

            foreach (Apple apple in appleList)
            {
                apple.IsActive = true;
                apple.Position = Position + offset;
                apple.OnPlayerPick();
                offset += new Vector2((apple.Width * 2) / NUM_APPLES_AT_BOUNCE, (apple.Height * 2) / NUM_APPLES_AT_BOUNCE);
            }

            if (apples <= 0)
            {
                OnDie();
            }
        }

        public override void Update()
        {
            base.Update();

            if (apples < NUM_APPLES && !Animation.IsPlaying)
            {
                Animation.Reset();
                Animation.IsActive = false;
            }
        }

        public override void OnDie()
        {
            base.OnDie();
            apples = NUM_APPLES;
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);

            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                Animation.Reset();
                Animation.IsActive = false;
                apples = NUM_APPLES;
            }
        }
    }
}
