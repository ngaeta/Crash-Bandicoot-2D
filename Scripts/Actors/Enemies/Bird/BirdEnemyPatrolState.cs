using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class BirdEnemyPatrolState : EnemyState
    {
        private float yPositionPatrol;
        private Vector2 velocityFlyUp;
        private AudioClip clipFlying;

        public BirdEnemyPatrolState() : base()
        {
            velocityFlyUp = new Vector2(0, -50f);
            clipFlying = AudioManager.GetAudioClip("birdFly");
        }

        public override void AssignStateMachine(StateMachine machine)
        {
            base.AssignStateMachine(machine);
            yPositionPatrol = Owner.Position.Y;
        }

        public override void Enter()
        {
            base.Enter();

            ((BirdEnemy)Owner).IsAttacking = false;

            if (Owner.Position.Y > yPositionPatrol)
            {
                ((BirdEnemy)Owner).ChangeState(BirdEnemy.AnimationType.Fly_Up);
                Owner.Velocity = velocityFlyUp;
                Owner.FlipX = !Owner.FlipX;
            }

            if (!CameraManager.OutOfCameraViewPort(Owner) && !Owner.AudioSource3D.IsPlaying)
                Owner.AudioSource3D.PlayAudio(clipFlying, true);
        }

        public override void Update()
        {
            base.Update();

            if (!CameraManager.OutOfCameraViewPort(Owner) && !Owner.AudioSource3D.IsPlaying)
            {
                Owner.AudioSource3D.PlayAudio(clipFlying, true);
            }

            if (Owner.Position.Y < yPositionPatrol)
            {
                Owner.Position = new Vector2(Owner.Position.X, yPositionPatrol);
                Owner.RigidBody.Velocity = Vector2.Zero;
                ((BirdEnemy)Owner).ChangeState(BirdEnemy.AnimationType.Idle);
            }

            else if (Owner.Position.Y == yPositionPatrol && Owner.CheckPlayerInFov())
            {
                machine.Switch((int)Enemy.State.Alert);
            }
        }
    }
}
