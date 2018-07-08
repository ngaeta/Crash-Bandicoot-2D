using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class BlowGunManAlertState : EnemyState
    {
        public override void Enter()
        {
            base.Enter();

            if (Enemy.Player.IsCrouched)
            {
                ((BlowGunManEnemy)Owner).Crouch();
            }
            else
            {
                ((BlowGunManEnemy)Owner).StandUp();
            }
        }

        public override void Update()
        {
            base.Update();

            if (((BlowGunManEnemy)Owner).CurrentAnimation == BlowGunManEnemy.AnimationType.IDLE || !Owner.Animation.IsPlaying)
            {
                machine.Switch((int)Enemy.State.Attack);
            }
        }
    }
}
