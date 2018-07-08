using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class LizardPatrolState : EnemyState
    {
        private Tuple<float, float> patrolingArea;

        public LizardPatrolState(Tuple<float, float> patrolingArea) : base()
        {
            this.patrolingArea = patrolingArea;
        }

        public override void Enter()
        {
            base.Enter();
            ((LizardEnemy)Owner).Walk();        
        }

        public override void Update()
        {
            base.Update();

            if (Owner.Position.X > patrolingArea.Item2 || Owner.Position.X < patrolingArea.Item1)
            {
                Owner.Position = new Vector2(Owner.Velocity.X > 0 ? patrolingArea.Item2 : patrolingArea.Item1, Owner.Position.Y);
                ((LizardEnemy)Owner).TurningAround();
            }

            if(!((LizardEnemy) Owner).IsTurningAround && Owner.CheckPlayerInFov())
            {
                Owner.Machine.Switch((int)Enemy.State.Alert);
            }
        }
    }
}
