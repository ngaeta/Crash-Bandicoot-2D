using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class DeathRotationState : DeathState
    {
        public override void Enter()
        {
            base.Enter();
            Player.RigidBody.Velocity = Vector2.Zero;
            Player.IsGrounded = false;
            timeToLoadGame = 4f;
        }

        public override void Update()
        {
            base.Update();
            Player.IsGrounded = false;
        }
    }
}
