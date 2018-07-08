using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    class JumpState : PlayerState
    {
        protected float x_Vel;

        public JumpState() : base()
        {
            x_Vel = Player.Speed.X / 3f;
        }

        public override void Input()
        {
            base.Input();

            if (InputManager.GetButton(Button.Left))
            {
                Player.MoveX(-x_Vel);
            }
            else if (InputManager.GetButton(Button.Right))
            {
                Player.MoveX(x_Vel);
            }
        }

        public override void Enter()
        {
            base.Enter();

            if (Player.IsCrouched)
                Player.StandUp();

            if (Player.CurrentState == Player.State.Slide)
            {
                Player.RigidBody.SetXVelocity(x_Vel);
                Player.StandUp();
            }

            Player.ChangeState(Player.State.JumpStopped);
        }

        public override void Update()
        {
            base.Update();

            if (Player.Velocity.X > 0)
            {
                Player.RigidBody.SetXVelocity(Player.Velocity.X - Game.FrictionX * Game.DeltaTime);
                if (Player.Velocity.X < 0)
                    Player.RigidBody.SetXVelocity(0);
            }
            else if (Player.Velocity.X < 0)
            {
                Player.RigidBody.SetXVelocity(Player.Velocity.X + Game.FrictionX * Game.DeltaTime);
                if (Player.Velocity.X > 0)
                    Player.RigidBody.SetXVelocity(0);
            }
            
            if (Player.IsGrounded && !Player.IsDead)
            {
                machine.Switch((int)Player.State.Landed);
            }

            //if (Player.IsInvincible)
            //    Player.OffsetHead = new Vector2(Player.Width / 8, Player.Height / 10 - 9);
        }
    }
}
