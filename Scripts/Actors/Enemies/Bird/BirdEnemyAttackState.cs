using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class BirdEnemyAttackState : EnemyState
    {
        private Vector2 velocityAttack;
        private float xRangeAttack;

        public BirdEnemyAttackState() : base()
        {
            velocityAttack = new Vector2(PlayScene.Player != null ?  PlayScene.Player.Speed.X + 10f : 200f, 0f);   
        }

        public override void Enter()
        {
            base.Enter();

            ((BirdEnemy)Owner).IsAttacking = true;
            ((BirdEnemy)Owner).ChangeState(BirdEnemy.AnimationType.Attack);

            xRangeAttack = Owner.Position.X + Math.Sign(Owner.LookDirection.X) * Owner.SightRadius;
          
            Owner.RigidBody.Velocity =  Owner.LookDirection * velocityAttack;
        }

        public override void Update()
        {
            base.Update();

            if (Owner.LookDirection.X < 0)
            {
                if(Owner.Position.X < xRangeAttack)
                    machine.Switch((int)Enemy.State.Patrol);
            }
            else
            {
                if (Owner.Position.X > xRangeAttack)
                    machine.Switch((int)Enemy.State.Patrol);
            }
        }
    }
}
