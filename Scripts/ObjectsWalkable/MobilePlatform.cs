using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class MobilePlatform : InvisiblePlatform
    {
        private Tuple<Vector2, Vector2> offsetMovement;

        public MobilePlatform(Vector2 spritePosition, Vector2 Offset, Vector2 velocity, string spriteSheetName = "platform", DrawManager.Layer layer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, layer)
        {
            Velocity = velocity;

            if (Velocity.X > 0 || Velocity.Y > 0)
            {
                offsetMovement = new Tuple<Vector2, Vector2>(Position, Position + Offset);
            }
            else
            {
                offsetMovement = new Tuple<Vector2, Vector2>(Position + Offset, Position);
            }
        }

        public override void Update()
        {
            base.Update();

            if (Position.Length > offsetMovement.Item2.Length || Position.Length < offsetMovement.Item1.Length)
            {
                float distanceToMaxPos = (offsetMovement.Item2 - Position).Length;
                float distanceToMinPos = (offsetMovement.Item1 - Position).Length;

                Position =  distanceToMaxPos > distanceToMinPos ? offsetMovement.Item1 : offsetMovement.Item2;
                Velocity = -Velocity;
            }
        }
    }
}
