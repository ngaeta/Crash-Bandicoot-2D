using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class DeathDrownedState : DeathState
    {
        const float Y_VELOCITY = 2f;

        public override void Enter()
        {
            base.Enter();
            timeToLoadGame = 4f;
            
            Player.Velocity = Vector2.Zero;
            Player.RigidBody.IsCollisionsAffected = false;
        }

        public override void Update()
        {
            base.Update();
            Player.RigidBody.SetYVelocity(Y_VELOCITY);
            Player.RigidBody.SetXVelocity(0);
        }
    }
}
