using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class ExplosionCrate : BrokenCrate
    {
        private List<GameObject> objectAlreadyHitted;
        private uint collisionMask;

        public Player.State PlayerDeathAnim { get; set; }

        public ExplosionCrate(Vector2 spritePosition, string spriteSheetName, string clipExplosionName = "tntExplosion", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(1.8f, 1.8f);
            Animation.IsActive = false;
            ClipBroken = AudioManager.GetAudioClip(clipExplosionName);

            float ray = (float)(Math.Sqrt(Width * Width + Height * Height) / 2) - Width / 6f;
            Circle circleCollider = new Circle(Vector2.Zero, null, ray);
            RigidBody = new RigidBody(spritePosition, this, circleCollider, null, false);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Explosion;

            collisionMask = (uint)(PhysicsManager.ColliderType.Player | PhysicsManager.ColliderType.Crate | PhysicsManager.ColliderType.Pickable | PhysicsManager.ColliderType.Enemy);
            RigidBody.SetCollisionMask(collisionMask);
            objectAlreadyHitted = new List<GameObject>();

            PlayerDeathAnim = Player.State.DeathBurnt;
        }

        public override void OnCollide(Collision collision)
        {
            base.OnCollide(collision);

            if (!objectAlreadyHitted.Contains(collision.collider))
            {
                if(collision.collider is IHittable hit)
                {
                    hit.OnHit(this);
                }
                else if (collision.collider is Player player)
                {
                    player.OnHit(PlayerDeathAnim);
                }

                objectAlreadyHitted.Add(collision.collider);
            }
        }

        //public override void Update()
        //{
        //    base.Update();

        //    if(objectAlreadyHitted.Count > 0 && !IsActive)
        //    {
        //        objectAlreadyHitted.Clear();
        //    }
        //}

        public void ResetObjectAlreadyHitted()
        {
            objectAlreadyHitted.Clear();
        }
    }
}
