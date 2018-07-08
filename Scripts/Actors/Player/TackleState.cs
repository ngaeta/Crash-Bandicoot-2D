using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    class SlideState : PlayerState
    {
        const float TIME_TACKLE = 0.45f;

        private float timeTackle;

        public SlideState() : base()
        {
            timeTackle = 0;
        }

        public override void Enter()
        {
            base.Enter();
            timeTackle = TIME_TACKLE;
            Player.Tackle();
        }

        public override void Update()
        {
            if (!Player.IsDead)
            {
                base.Update();

                if (timeTackle <= 0 || !Player.IsGrounded)
                {
                    Player.State stateToSwitch = Player.State.Idle;

                    if(!Player.IsGrounded)
                    {
                        stateToSwitch = Player.State.JumpStopped;
                    }
                    else if (InputManager.GetButton(Button.Down))
                        stateToSwitch = Player.State.Crouch;
                    

                    machine.Switch((int)stateToSwitch);
                }
                else
                {
                    timeTackle -= Game.DeltaTime;
                }

                if (Player.IsInvincible)
                    Player.OffsetHead = new Vector2(Player.Width/10 - 3, Player.Height/10 - 9);
            }
        }

        public override void Exit()
        {
            base.Exit();
            timeTackle = TIME_TACKLE;

            if(!Player.IsDead)
                Player.StandUp();
        }
    }
}
