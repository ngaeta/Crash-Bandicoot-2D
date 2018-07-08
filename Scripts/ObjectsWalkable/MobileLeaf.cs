using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class MobileLeaf : Ground
    {
        private Tuple<float, float> rangePosition;

        public MobileLeaf(Vector2 spritePosition, float rangeXDist = 120, int xVel = 40, string spriteSheetName = "leafPlatform") : base(spritePosition, spriteSheetName, true, DrawManager.Layer.Background)
        {
            PhysicsManager.RemoveItem(RigidBody);

            sprite.scale = new Vector2(0.5f);

            Rect colliderRect = new Rect(new Vector2(0, 3f), null, Width, Height - 20);
            RigidBody = new RigidBody(spritePosition, this, null, colliderRect);
            RigidBody.Type = (uint) PhysicsManager.ColliderType.Ground;

            rangePosition = new Tuple<float, float>(Position.X - rangeXDist, Position.X + rangeXDist);
            Velocity = new Vector2(xVel, 0);
        }

        public override void Update()
        {
            base.Update();

            if (Position.X > rangePosition.Item2 || Position.X < rangePosition.Item1)
            {
                Position = new Vector2(Velocity.X > 0 ? rangePosition.Item2 : rangePosition.Item1, Position.Y);
                Velocity = -Velocity;
            }
        }
    }
}
