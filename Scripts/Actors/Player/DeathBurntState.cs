using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class DeathBurntState : DeathState
    {
        public override void Enter()
        {
            base.Enter();
            Player.RigidBody.Velocity = Vector2.Zero;
            Player.IsGrounded = false;
            timeToLoadGame = 5.5f;
        }

        public override void Update()
        {
            base.Update();
            Player.IsGrounded = false;
        }
    }
}
