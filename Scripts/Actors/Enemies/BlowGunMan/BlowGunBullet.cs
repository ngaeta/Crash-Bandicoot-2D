using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class BlowGunBullet : GameObject
    {
        private float xSpeed;
        private uint collisionMask;

        public BlowGunBullet(Vector2 spritePosition, string spriteSheetName= "blowGunBullet", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(0.3f);
            xSpeed = 220f;
            IsActive = false;

            RigidBody = new RigidBody(spritePosition, this);
            RigidBody.Type = (uint) PhysicsManager.ColliderType.Bullet;
            RigidBody.SetXVelocity(-50f);
            collisionMask = ((uint) (PhysicsManager.ColliderType.Player | PhysicsManager.ColliderType.Crate | PhysicsManager.ColliderType.Ground | PhysicsManager.ColliderType.Trap));

            RigidBody.SetCollisionMask(collisionMask);
        }

        public void Shoot(Vector2 direction)
        {
            IsActive = true;
            RigidBody.Velocity = xSpeed * direction;
           
        }

        public override void OnCollide(Collision collision)
        {
            if (collision.collider is Player p)
            {
                IsActive = false;
                p.OnHit(Player.State.DeathRotation);
            }
            else
                IsActive = false;
        }

        public override void Update()
        {
            base.Update();

            if (IsActive)
            {
                if (CameraManager.OutOfCameraViewPort(this))
                {
                    IsActive = false;
                }
            }
        }
    }
}
