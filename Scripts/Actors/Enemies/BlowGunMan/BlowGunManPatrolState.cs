using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class BlowGunManPatrolState : EnemyState
    {
        public override void Enter()
        {
            base.Enter();
            ((BlowGunManEnemy)Owner).StandUp();
        }

        public override void Update()
        {
            base.Update();

            if (((BlowGunManEnemy)Owner).CurrentAnimation == BlowGunManEnemy.AnimationType.STAND_UP)
            {
                if (!Owner.Animation.IsPlaying)
                {
                    ((BlowGunManEnemy)Owner).Idle();
                }
            }

            else if (Owner.CheckPlayerInFov())
            {
                Owner.Machine.Switch((int)Enemy.State.Alert);
            }
        }
    }
}
