using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    abstract class DestructibleCrate : Crate
    {
        protected BrokenCrate brokenCrate;

        public DestructibleCrate(Vector2 spritePosition, string spriteSheetName, DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            brokenCrate = new BrokenCrate(sprite.position);
            brokenCrate.Scale = sprite.scale;
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            float deltaY = colllisionInfo.Delta.Y;

            if (deltaY < 0)
            {
                OnHit(player);
            }

            else if (PlayerImpactY >= minInpactToDetectYCollision)
            {
                player.Jump(bounceMultiplierY);
                OnHit(player);
            }
        }

        public override void OnHit(GameObject hitObject)
        {
            isHitted = true;
            OnDie();
        }

        public override void OnDie()
        {
            IsActive = false;
            brokenCrate.Position = Position;
            brokenCrate.IsActive = true;
            RigidBody.IsCollisionsAffected = false;
           // Position = Vector2.Zero;  //importante perchè cosi se il player sta sotto poi si rialza, potrebbe dare problemi con il checkpoint
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);

            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                RigidBody.IsCollisionsAffected = true;
                brokenCrate.Animation.Reset();
                brokenCrate.IsActive = false;
            }
        }
    }
}
