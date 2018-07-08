using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    public enum TrapType
    {
        Thorn,
        IdolHead,
        StoneWheel,
        BranchTrap
    }

    class Trap : GameObject
    {
        protected Player.State playerAnimDie;

        public Trap(Vector2 spritePosition, string spriteSheetName = "trapThorns", DrawManager.Layer layer = DrawManager.Layer.Middleground) : base(spritePosition, spriteSheetName, layer)
        {
            playerAnimDie = Player.State.DeathAngel;

            RigidBody = new RigidBody(spritePosition, this);
            RigidBody.SetCollisionMask((uint)PhysicsManager.ColliderType.Player);
            RigidBody.Type = (uint) PhysicsManager.ColliderType.Trap;  //togliere se ci sono problemi con le spine
        }

        public override void OnCollide(Collision collision)
        {
            base.OnCollide(collision);

            if (collision.collider is Player p && !p.IsDead)
            {
                OnCollisionWithPlayer(p, collision);
            }
            else
                OnCollisionWithObject(collision);
        }

        protected virtual void OnCollisionWithPlayer(Player p, Collision collisionInfo)
        {
            p.OnDie(playerAnimDie);
        }

        protected virtual void OnCollisionWithObject(Collision collider)
        {

        }
    }
}
