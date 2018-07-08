using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class Fire : Trap
    {
        enum AnimationType { Start, Loop, End }

        const float TIME_LOOP_FIRE = 3f;

        private AnimationType currAnim;
        private float timeLoop;
        private AudioSource3D audioSourceFire;
        private AudioClip clipFireBurn;
        private AudioClip clipBurned;
        private AudioSource audioSourceBurn;

        public Fire(Vector2 spritePosition, string spriteSheetName = "fire") : base(spritePosition, spriteSheetName, DrawManager.Layer.Foreground)
        {
            sprite.scale = new Vector2(1f, 0.65f);

            PhysicsManager.RemoveItem(RigidBody);

            Rect rect = new Rect(Vector2.Zero, null, Width / 1.8f, Height);
            RigidBody = new RigidBody(spritePosition, this, null, rect);
            RigidBody.SetCollisionMask((uint) PhysicsManager.ColliderType.Player);

            currAnim = AnimationType.Start;
            timeLoop = TIME_LOOP_FIRE;
            playerAnimDie = Player.State.DeathBurnt;
            IsActive = false;

            audioSourceFire = new AudioSource3D(this);

            clipFireBurn = AudioManager.GetAudioClip("fireBurn");
            clipBurned = AudioManager.GetAudioClip("clipBurn");
        }

        public override void Update()
        {
            base.Update();

            if (IsActive)
            {
                if (currAnim == AnimationType.Start)
                {
                    if (!Animation.IsPlaying)
                    {
                        ChangeAnim(AnimationType.Loop);
                    }
                }
                else if (currAnim == AnimationType.Loop)
                {
                    if (timeLoop <= 0)
                    {
                        timeLoop = TIME_LOOP_FIRE;
                        ChangeAnim(AnimationType.End);
                    }
                    else
                        timeLoop -= Game.DeltaTime;
                }
                else if (currAnim == AnimationType.End)
                {
                    if (!Animation.IsPlaying)
                    {
                        IsActive = false;

                        if(audioSourceFire.IsPlaying)
                            audioSourceFire.StopAudio(false);
                    }
                }
            }
        }

        public void StartFire()
        {
            IsActive = true;
            ChangeAnim(AnimationType.Start);

            audioSourceFire.PlayAudio(clipFireBurn, true);
        }

        private void ChangeAnim(AnimationType anim)
        {
            Animation = animations[(int)anim];
            Animation.Reset();
            currAnim = anim;         
        }

        protected override void OnCollisionWithPlayer(Player p, Collision collisionInfo)
        {
            if(audioSourceBurn == null)
            {
                audioSourceBurn = new AudioSource();
            }

            audioSourceBurn.Play(clipBurned);
            AudioManager.DisposeAudioSource(audioSourceBurn);

            p.OnHit(playerAnimDie);
        }
    }
}
