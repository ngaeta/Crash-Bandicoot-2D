using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class BlowGunManAttackState : EnemyState
    {
        private int standUpAttackFrame = 8;
        private int crouchedAttackFrame = 5;
        private int frameToControl;
        private bool alreadyShooted;

        public override void Enter()
        {
            base.Enter();

            if (((BlowGunManEnemy) Owner).CurrentAnimation == BlowGunManEnemy.AnimationType.CROUCH)
            {
                frameToControl = crouchedAttackFrame;
            }
            else
                frameToControl = standUpAttackFrame;

            ((BlowGunManEnemy)Owner).StartAttackAnim();

            alreadyShooted = false;
        }

        public override void Update()
        {
            base.Update();

            if (Owner.Animation.IsPlaying)
            {
                if (!alreadyShooted && Owner.Animation.CurrFrame == frameToControl)
                {
                    alreadyShooted = true;
                    ((BlowGunManEnemy)Owner).Shoot();
                }
            }
            else 
            {
                if (!Owner.CheckPlayerInFov())
                    machine.Switch((int)Enemy.State.Patrol);
                else
                    machine.Switch((int)Enemy.State.Alert); 
            }
        }
    }
}
