using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class BirdEnemyAlertState : EnemyState
    {
        private Vector2 velocityDescent;
        private float yMinDescent;
        private AudioClip clipAttack;

        public override void Enter()
        {
            base.Enter();

            yMinDescent = Owner.Position.Y + 80f;
            velocityDescent = new Vector2(Math.Sign(Owner.LookDirection.X) * 5f, 80f);

            ((BirdEnemy)Owner).ChangeState(BirdEnemy.AnimationType.Fly_Down);
            Owner.RigidBody.Velocity = velocityDescent;
            Owner.PlayAudio3D(clipAttack);
        }

        public override void AssignStateMachine(StateMachine machine)
        {
            base.AssignStateMachine(machine);

            yMinDescent = Owner.Position.Y + 80f;
            velocityDescent = new Vector2(Math.Sign(Owner.LookDirection.X) * 5f, 80f);
            clipAttack = AudioManager.GetAudioClip("birdAttack");
        }

        public override void Update()
        {
            base.Update();

            if(Owner.Position.Y >= yMinDescent)
            {
                Owner.Position = new Vector2(Owner.Position.X, yMinDescent);
                machine.Switch((int)Enemy.State.Attack);
            }
        }
    }
}
