using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class LandedState : PlayerState
    {
        public LandedState() : base()
        {
        }

        public override void Enter()
        {
            base.Enter();

            Player.ChangeState(Player.State.Landed);
            Player.RigidBody.Velocity = Vector2.Zero;
            Player.PlayAudioFootStep();
        }

        public override void Input()
        {
            base.Input();

            if (Player.VelocityLanded.Y < 300f)
            {
                if (InputManager.GetButton(Button.Left) || InputManager.GetButton(Button.Right))
                {
                    machine.Switch((int)Player.State.Idle);
                }
                else if (InputManager.GetButton(Button.Down) && !Player.IsCrouched)
                {
                    machine.Switch((int)Player.State.Crouch);
                }
                //else if (InputManager.GetButton(Button.Up))
                //    Player.Jump();
            }
        }

        public override void Update()
        {
            base.Update();

            if (!Player.Animation.IsPlaying)
            {
                Player.StateMachine.Switch((int)Player.State.Idle);
            }
        }
    }
}
