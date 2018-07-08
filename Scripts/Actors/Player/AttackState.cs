using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class AttackState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
            Player.Attack();
        }

        public override void Update()
        {
            base.Update();

            if (!Player.Animation.IsPlaying)
            {
                if (Player.IsGrounded)
                    machine.Switch((int)Player.State.Idle);
                else
                    machine.Switch((int)Player.State.JumpStopped);

            }
        }
    }
}
