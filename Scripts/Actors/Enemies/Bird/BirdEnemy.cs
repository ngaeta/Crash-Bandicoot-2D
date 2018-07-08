using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class BirdEnemy : Enemy
    {
        public enum AnimationType { Idle, Fly_Down, Fly_Up, Attack}

        private Dictionary<AnimationType, Rect> colliders;

        public bool IsAttacking { get; set; }

        public BirdEnemy(Vector2 spritePosition, float SightRadius = 370f, string spriteSheetName="bird", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            this.SightRadius = SightRadius;
            halfConeAngle = 180f;

            animations[(int)AnimationType.Attack].LoopAtFrame(7);
            animations[(int)AnimationType.Fly_Down].LoopAtFrame(6);

            Machine = new StateMachine(this);

            Machine.RegisterState((int)State.Patrol, new BirdEnemyPatrolState());
            Machine.RegisterState((int)State.Alert, new BirdEnemyAlertState());
            Machine.RegisterState((int)State.Attack, new BirdEnemyAttackState());

            Machine.Switch((int)State.Patrol);

            UseGroundableGravity = false;

            CreateColliders();
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Ground);
            //ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Crate);
        }

        public void ChangeState(AnimationType type)
        {
            Animation = animations[(int)type];
            Animation.Reset();

            ChangeCollider(type);
        }

        protected override void OnCollisionFromX(Collision collisionInfo)
        {
            if (collisionInfo.collider is Player p && !p.IsDead && !p.IsInvincible)
            {
                //se colpisce sul becco mentre l uccello sta attaccando
                 if (IsAttacking && Math.Sign(collisionInfo.Delta.X) == Math.Sign(LookDirection.X))
                {
                    p.OnHit(playerDieAnim);
                    return;
                }
            }

             base.OnCollisionFromX(collisionInfo);
        }

        private void ChangeCollider(AnimationType type)
        {
            if (colliders != null && colliders.ContainsKey(type))
            {
                foreach (KeyValuePair<AnimationType, Rect> collider in colliders)
                {
                    if (collider.Value.IsActive)
                    {
                        collider.Value.IsActive = false;
                        break;
                    }
                }

                Rect colliderToActive = colliders[type];
                RigidBody.SetBoundingBox(colliderToActive);
                colliderToActive.IsActive = true;
            }
        }

        private void CreateColliders()
        {
            colliders = new Dictionary<AnimationType, Rect>();

            Rect colliderAttack = new Rect(new Vector2(0, Height/4), null, Width, Height / 2.5f);
            colliderAttack.IsActive = false;

            colliders.Add(AnimationType.Idle, RigidBody.BoundingBox);
            colliders.Add(AnimationType.Attack, colliderAttack);
        }

        public override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);
            Animation.Reset();
            Velocity = Vector2.Zero;
            LookDirection = new Vector2(-1, 0);

            ChangeState(AnimationType.Idle);
        }
    }
}
