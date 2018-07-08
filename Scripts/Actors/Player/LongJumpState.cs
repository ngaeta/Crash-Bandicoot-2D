using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class LongJumpState : JumpState
    {
        public LongJumpState() : base()
        {
            x_Vel = Player.Speed.X;
        }

        public override void Input()
        {
            x_Vel -= 60 * Game.DeltaTime;
            if (InputManager.GetButton(Button.Left) && Player.Velocity.X <= 0)
            { 
                if (x_Vel > 0)
                {
                    x_Vel = x_Vel < 0 ? 0 : x_Vel;
                    Player.MoveX(-x_Vel);
                }
            }
            else if (InputManager.GetButton(Button.Right) && Player.Velocity.X >= 0)
            {
                if (x_Vel > 0)
                {
                    x_Vel = x_Vel < 0 ? 0 : x_Vel;
                    Player.MoveX(x_Vel);
                }
            }
        }

        public override void Enter()
        {
            Player.RigidBody.SetXVelocity(0);

            if (Player.IsCrouched)
                Player.StandUp();

            if (Player.CurrentState == Player.State.Slide)
            {
                Player.StandUp();
            }

            Player.ChangeState(Player.State.JumpInMovement);
        }

        public override void Exit()
        {
            base.Exit();
            x_Vel = Player.Speed.X;
        }
    }
}
