using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    class CrouchState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
            Player.Crouch();
        }

        public override void Input()
        {
            if(!InputManager.GetButton(Button.Down) && Player.ObstacleStandUp == null)
            {
                Player.StandUp();
                machine.Switch((int)Player.State.Idle);
            }

            if (InputManager.GetButton(Button.Left) || InputManager.GetButton(Button.Right))
            {
                machine.Switch((int) Player.State.CrouchMoving);
            }
        }

        public override void Update()
        {
            base.Update();

            if(Player.ObstacleStandUp != null && CheckObstaclePosition())
            {
                Player.ObstacleStandUp = null;      
            }

            Player.OffsetHead = new Vector2(Player.Width / 4, Player.Height/16);
        }

        private bool CheckObstaclePosition()
        {
            float obstacleXPos = Player.ObstacleStandUp.Position.X;
            float obstacleWidth = Player.ObstacleStandUp.Width;

            return (Player.Position.X + Player.Width / 3 > obstacleXPos + obstacleWidth ||
                    Player.Position.X - Player.Width / 3 < obstacleXPos - obstacleWidth);
        }
    }
}
