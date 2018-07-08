using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class NitroCrate : DestructibleCrate
    {
        public NitroCrate(Vector2 spritePosition, string spriteSheetName = "nitroCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale -= new Vector2(0.15f, 0.15f);

            brokenCrate = new ExplosionCrate(spritePosition - new Vector2(0, 24), "nitroExplosion", "nitroExplosion");
            ((ExplosionCrate)brokenCrate).PlayerDeathAnim = Player.State.DeathAngel;

            minInpactToDetectYCollision = 0f;
            CanWalkable = false;
        }

        protected override void OnCollisionFromX(Player player, Collision colllisionInfo)
        {
            OnHit(player);
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            OnHit(player);
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);

            if (checkpoint == null || checkpoint.Position.X < Position.X)
                ((ExplosionCrate)brokenCrate).ResetObjectAlreadyHitted();
        }
    }
}
