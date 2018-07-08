using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;
using OpenTK;

namespace CrashBandicoot
{
    class DeathMashedState : DeathState
    {
        private AudioSource audioSource;
        private AudioClip clipSmashed;

        public DeathMashedState()
        {
           clipSmashed = AudioManager.GetAudioClip("smashed");
        }

        public override void Enter()
        {
            base.Enter();
            //Player.GetSprite().SetAdditiveTint(0, 0.15f, 0, 0);
            Player.RigidBody.Velocity = Vector2.Zero;
            timeToLoadGame = 4f;

            audioSource = new AudioSource();
            audioSource.Play(clipSmashed);
            AudioManager.DisposeAudioSource(audioSource);
        }

        public override void Update()
        {
            base.Update();
            Player.RigidBody.Velocity = Vector2.Zero;
        }

        public override void Exit()
        {
            base.Exit();
            //Player.GetSprite().SetAdditiveTint(Vector4.Zero);
        }
    }
}
