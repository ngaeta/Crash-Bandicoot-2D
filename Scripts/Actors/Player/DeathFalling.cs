using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class DeathFalling : DeathState
    {
        const float YVelocity = -320f;

        public override void Enter()
        {
            base.Enter();
            Player.Animation.IsActive = false;
            Player.RigidBody.IsGravityAffected = false;
            Player.RigidBody.Velocity = Vector2.Zero;
            timeToLoadGame = 5.5f;
        }

        public override void Update()
        {
            base.Update();

            if (!Player.Animation.IsActive && !Player.AudioSourceIsPlaying())
            {
                Player.Animation.IsActive = true;
                Player.RigidBody.IsGravityAffected = true;
                Player.RigidBody.SetYVelocity(YVelocity);
            }
        }
    }
}
