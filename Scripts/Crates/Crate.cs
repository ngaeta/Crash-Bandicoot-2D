using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;
using OpenTK;

namespace CrashBandicoot
{
    abstract class Crate : Groundable, IWalkable, IHittable, ICheckpointLoadable
    {
        private Vector2 initialPos;

        protected bool isHitted;
        protected float bounceMultiplierY;
        protected float minInpactToDetectYCollision;
        protected AudioClip clipFootStep;

        public float PlayerImpactY { get; set; }
        public bool CanWalkable { get; set; }

        public AudioClip AudioFootStep { get { return clipFootStep; } }

        public Crate(Vector2 spritePosition, string spriteSheetName, DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            RigidBody = new RigidBody(spritePosition, this);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Crate;
            RigidBody.SetCollisionMask((uint)PhysicsManager.ColliderType.Player | (uint)PhysicsManager.ColliderType.Ground | (uint)PhysicsManager.ColliderType.Crate);
            sprite.scale = new Vector2(1.35f, 1.35f);
            isHitted = false;

            bounceMultiplierY = 1.2f;
            minInpactToDetectYCollision = 0.1f;
            UseGroundableGravity = false;
            CanWalkable = true;

            initialPos = Position;
        }

        public override void Update()
        {
            base.Update();

            //if (CameraManager.OutOfCameraViewPort(this) == false)
            //{
            //    UseGroundableGravity = true;
            //}
        }

        public override void OnCollide(Collision collisionInfo)
        {
            base.OnCollide(collisionInfo);

            if (collisionInfo.collider is Player p && !p.IsDead)
            {
                if (p.CurrentState == Player.State.Attack || p.CurrentState == Player.State.Slide || p.IsInvincible)
                {
                    OnHit(p);
                }

                //if (p.IsInvincible)
                //{
                //    OnHit(p);  //ad alcune casse serve sapere entrambe le cose
                //}

                float deltaX = collisionInfo.Delta.X;
                float deltaY = collisionInfo.Delta.Y;

                if (deltaX < deltaY)
                {
                    OnCollisionFromX(p, collisionInfo);
                }
                else
                {
                    if (p.Position.Y > Position.Y)
                    {
                        collisionInfo.Delta.Y = -collisionInfo.Delta.Y;
                    }

                    OnCollisionFromY(p, collisionInfo);
                }
            }
        }

        protected virtual void OnCollisionFromX(Player player, Collision colllisionInfo)
        {

        }

        protected virtual void OnCollisionFromY(Player player, Collision colllisionInfo)
        {

        }

        public virtual void OnHit(GameObject hitObject)
        {

        }

        public virtual void OnDie()
        {

        }

        protected virtual void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                isHitted = false;
                IsActive = true;
                // Position = initialPos;
            }
            else
            {
                //se checkpoint != null chiama remove su gamemanager altrimenti su checkpoint
                if (!IsActive)
                    Destroy();
            }
        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            OnCheckpointLoad(checkpoint);
        }
    }
}
