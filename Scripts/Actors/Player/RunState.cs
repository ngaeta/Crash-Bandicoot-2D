using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    enum Direction { LEFT, RIGHT }

    class RunState : PlayerState
    {
        public static Direction Direction;
        private bool b_JumpPressed;

        public RunState() : base()
        {
        
        }

        public override void Enter()
        {
            base.Enter();

            Player.ChangeState(Player.State.Run);
      
            float vel = Player.Speed.X;

            if (Direction == Direction.LEFT)
                vel = -vel;

            Player.MoveX(vel);
        }

        public override void Input()
        {
            base.Input();

            if (InputManager.GetButton(Button.Down))
            {
                machine.Switch((int)Player.State.Slide);
            }
            else if (!InputManager.GetButton(Button.Left) && !InputManager.GetButton(Button.Right))
            {
                machine.Switch((int)Player.State.Idle);
            }
            else if(InputManager.GetButton(Button.Left) && Direction == Direction.RIGHT)
            {
                machine.Switch((int)Player.State.Idle);
            }
            else if (InputManager.GetButton(Button.Right) && Direction == Direction.LEFT)
            {
                machine.Switch((int)Player.State.Idle);
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
            Player.OffsetHead = new Vector2(Player.Width / 6, -10f);

            if(Player.Animation.CurrFrame == 3 || Player.Animation.CurrFrame==7)
            {
                Player.PlayAudioFootStep();
            }
        }

        public override void Exit()
        {
            base.Exit();
            Player.StopFootStep();
        }
    }
}
