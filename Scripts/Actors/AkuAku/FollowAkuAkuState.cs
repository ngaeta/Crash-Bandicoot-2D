using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class FollowAkuAkuState : AkuAkuState
    {
        private float lerpVelocity = 3f;

        public override void Update()
        {
            bool flipX = machine.Owner.GetSprite().FlipX;

            totalTime += Game.DeltaTime;
            machine.Owner.Position = Vector2.Lerp(machine.Owner.Position, Player.Position + Offset, Game.DeltaTime * lerpVelocity);
            machine.Owner.Position = new Vector2(machine.Owner.Position.X, machine.Owner.Position.Y + (float)Math.Sin(totalTime * 2) * 0.5f);

            if (Player.Velocity.X < 0 && !flipX)
            {
                machine.Owner.GetSprite().FlipX = true;
                Offset.X = Math.Abs(Offset.X);
            }
            else if (Player.Velocity.X > 0 && flipX)
            {
                machine.Owner.GetSprite().FlipX = false;
                Offset.X = -Offset.X;
            }
            else if (Player.Velocity.Length == 0)
            {
                if ((machine.Owner.Position - Player.Position + Offset).Length < Offset.Length * 2)
                    machine.Switch((int)AkuState.Idle);
            }
        }
    }
}
