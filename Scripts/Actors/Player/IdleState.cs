using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class IdleState : PlayerState
    {
        const float TIME_TO_WAITING_ANIMATION = 10f;

        private float timeInIdle;
        private bool b_JumpPressed;

        public IdleState() : base()
        {
            timeInIdle = 0;
        }

        public override void Enter()
        {
            base.Enter();
            b_JumpPressed = true;
            Player.ChangeState((int)Player.State.Idle);
            Player.Velocity = Vector2.Zero;
            timeInIdle = TIME_TO_WAITING_ANIMATION;
        }

        public override void Input()
        {
            base.Input();

            if (InputManager.GetButton(Button.Left) && !InputManager.GetButton(Button.Right))
            {
                RunState.Direction = Direction.LEFT;
                machine.Switch((int)Player.State.Run);
            }
            else if (InputManager.GetButton(Button.Right) && !InputManager.GetButton(Button.Left))
            {
                RunState.Direction = Direction.RIGHT;
                machine.Switch((int)Player.State.Run);
            }
            else if(InputManager.GetButton(Button.Down) && !Player.IsCrouched)
            {
                machine.Switch((int)Player.State.Crouch);
            }

            if (InputManager.GetButton(Button.Up))
            {
                if (!b_JumpPressed)
                {
                    Player.Jump();
                    b_JumpPressed = true;
                }
            }
            else
                b_JumpPressed = false;
        }

        public override void Update()
        {
            base.Update();

            if (!Player.IsInvincible && !Player.IsDead)
            {
                if (timeInIdle < 0)
                {
                    Player.ChangeState(Player.State.Waiting);
                    timeInIdle = 0;
                }
                else if (timeInIdle > 0)
                    timeInIdle -= Game.DeltaTime;
            }
            else if(timeInIdle != TIME_TO_WAITING_ANIMATION)
                timeInIdle = TIME_TO_WAITING_ANIMATION;

            Player.OffsetHead= new Vector2(Player.Width / 7.8f, -Player.Height / 6);
        }

        public override void Exit()
        {
            base.Exit();
            timeInIdle = TIME_TO_WAITING_ANIMATION;
        }
    }
}
