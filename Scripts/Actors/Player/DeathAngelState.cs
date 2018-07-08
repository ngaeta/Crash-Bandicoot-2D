using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class DeathAngelState : DeathState
    {
        const float Y_SPEED = -30f;
        const float WAIT_FLY = 0.15f;

        private float count;

        public override void Enter()
        {
            Player.RigidBody.IsCollisionsAffected=false;  //viene fatto al prossimo update, collide col terreno e la velocità viene settata di nuovo a 0
            Player.RigidBody.Velocity = new Vector2(0, Y_SPEED);
            Player.IsGrounded = false;
            count = 0;
            timeToLoadGame = 5f;
        }

        public override void Update()
        {
            Player.RigidBody.IsCollisionsAffected = false;  //viene fatto al prossimo update, collide col terreno e la velocità viene settata di nuovo a 0

            base.Update();

            if (Player.RigidBody.Velocity == Vector2.Zero || Player.RigidBody.IsGravityAffected)
            {
                if (count <= 0)
                {
                    Player.RigidBody.IsGravityAffected = false;
                    Player.RigidBody.Velocity = new Vector2(0, Y_SPEED);
                }
                else
                    count -= Game.DeltaTime;
            }

            if(Player.Animation.IsRestarting)
            {
                Player.RigidBody.IsGravityAffected = true;
                count = WAIT_FLY;
            }
        }

        public override void Exit()
        {
            base.Exit();
            Player.RigidBody.IsCollisionsAffected = true;
        }
    }
}
