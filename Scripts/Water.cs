using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;
using Aiv.Audio;

namespace CrashBandicoot
{
    class Water : GameObject
    {
        const float ALPHA = 0.75f;

        private WaterSplash splash;

        public Water(Vector2 spritePosition, string spriteSheetName = "underWater", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.SetMultiplyTint(ALPHA, ALPHA, ALPHA, ALPHA);

            RigidBody = new RigidBody(spritePosition, this);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Water;
            RigidBody.SetCollisionMask((uint)PhysicsManager.ColliderType.Player);

            splash = new WaterSplash(Vector2.Zero);
        }

        public override void OnCollide(Collision collision)
        {
            base.OnCollide(collision);

            if (collision.collider is Player p && !p.IsDead)
            {
                if (!splash.Animation.IsPlaying && p.Velocity.Length > 10)
                {
                    splash.Position = collision.collider.Position + new Vector2(0, 5f);
                    splash.Scale += new Vector2(collision.collider.Velocity.Y / 1000);
                    splash.Active();
                }

                if (collision.Delta.Y >= p.Height / 1.2f)
                {
                    p.OnDie(Player.State.DeathDrowned);
                    collision.collider.GetSprite().SetMultiplyTint(1, 1.5f, 3f, 1);
                }
            }
        }

        public void SetScale(Vector2 newScale)
        {
            sprite.scale = newScale;
            PhysicsManager.RemoveItem(RigidBody);

            RigidBody = new RigidBody(Position, this);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Water;
            RigidBody.SetCollisionMask((uint)PhysicsManager.ColliderType.Player);
        }
    }
}
