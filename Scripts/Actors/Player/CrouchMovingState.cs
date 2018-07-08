using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    class CrouchMovingState : PlayerState
    {
        private float x_Vel;

        public override void Enter()
        {
            base.Enter();
            Player.CrouchMoving();
            x_Vel = Player.Speed.X / 3;
            //Player.ChangeState(Player.AnimationType.CrouchMoving, false);
        }

        public override void Input()
        {
            base.Input();

            if(InputManager.GetButton(Button.Left))
            {
                Player.MoveX(-x_Vel);
            }
            else if (InputManager.GetButton(Button.Right))
            {
                Player.MoveX(x_Vel);
            }
            else
            {
                machine.Switch((int)Player.State.Crouch);
            }
        }

        public override void Update()
        {
            base.Update();
            Player.OffsetHead = new Vector2(Player.Width / 3, Player.Height / 4);
        }
    }
}
