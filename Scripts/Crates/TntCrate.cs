using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class TntCrate : DestructibleCrate
    {
        private bool countdownStarted;
        private AudioSource audioSourceCountDown;
        private AudioClip clipCountdown;

        public TntCrate(Vector2 spritePosition, string spriteSheetName = "tntCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            Animation.IsActive = false;
            brokenCrate = new ExplosionCrate(spritePosition - new Vector2(0, 12), "tntExplosion", "tntExplosion");
            countdownStarted = false;
            clipCountdown = AudioManager.GetAudioClip("tntCountdown");

            // minVelocityHitY = 35f;
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            float deltaY = colllisionInfo.Delta.Y;


            if (!countdownStarted)
            {
                if (deltaY < 0)
                {
                    StartCountDown();
                }
                else if (PlayerImpactY > 0.1f)
                {
                    StartCountDown();

                    if (player.CurrentState == Player.State.JumpStopped || player.CurrentState == Player.State.JumpInMovement)
                    {
                        player.Jump(bounceMultiplierY);
                    }
                }
            }
        }

        private void StartCountDown()
        {
            if(audioSourceCountDown == null)
             audioSourceCountDown = new AudioSource();

            audioSourceCountDown.Play(clipCountdown);
            AudioManager.DisposeAudioSource(audioSourceCountDown);

            Animation.NextFrame();
            Animation.IsActive = true;
            countdownStarted = true;
        }

        public override void Update()
        {
            base.Update();

            if (countdownStarted)
            {
                if (!Animation.IsPlaying)
                {
                    OnDie();
                }
            }
        }

        public override void OnDie()
        {
            base.OnDie();

            if (countdownStarted)
            {
                audioSourceCountDown.Stop();
                AudioManager.DisposeAudioSource(audioSourceCountDown);
                countdownStarted = false;
            }
        }

        public override void OnHit(GameObject hitObject)
        {
            base.OnHit(hitObject);

            if (countdownStarted)
            {
                audioSourceCountDown.Stop();
                AudioManager.DisposeAudioSource(audioSourceCountDown);
                countdownStarted = false;
            }
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);
            countdownStarted = false;

            if(audioSourceCountDown != null)
            {
                if(audioSourceCountDown.IsPlaying)
                    audioSourceCountDown.Stop();

                audioSourceCountDown.Dispose();
                audioSourceCountDown = null;
            }

            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                Animation.Reset();
                Animation.IsActive = false;
                countdownStarted = false;
                ((ExplosionCrate)brokenCrate).ResetObjectAlreadyHitted();
            }
        }
    }
}
