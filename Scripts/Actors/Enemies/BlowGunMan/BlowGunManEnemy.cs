using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class BlowGunManEnemy : Enemy
    {
        public enum AnimationType { IDLE, ATTACK, CROUCH, ATTACK_CROUCH, STAND_UP }

        private Vector2 standUpShootOffset;
        private Vector2 crouchShootOffset;
        private Rect colliderCrouch;
        private Rect colliderStandUp;
        private AudioClip clipShoot;

        public bool PlayerInRay { get; set; }
        public AnimationType CurrentAnimation { get; private set; }
        public bool IsCrouched { get; private set; }

        public BlowGunManEnemy(Vector2 spritePosition, string spriteSheetName = "blowGunMan", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(1.3f, 1.1f);

            Machine = new StateMachine(this);
            Machine.RegisterState((int)State.Patrol, new BlowGunManPatrolState());
            Machine.RegisterState((int)State.Alert, new BlowGunManAlertState());
            Machine.RegisterState((int)State.Attack, new BlowGunManAttackState());

            Machine.Switch((int)State.Patrol);

            standUpShootOffset = new Vector2(Width / 2, -Height / 5);
            crouchShootOffset = new Vector2(Width / 2, 6f); //o 5 al posto di 6
            SightRadius = 450f;
            halfConeAngle = MathHelper.DegreesToRadians(15);

            colliderStandUp = new Rect(new Vector2(0, 0), RigidBody, Width / 2, Height - 5);
            colliderCrouch = new Rect(new Vector2(0, 10), RigidBody, Width/2, Height - 25);
            RigidBody.SetBoundingBox(colliderStandUp);

            colliderCrouch.IsActive = false;

            IsCrouched = false;
            useMultiplyRay = true;
            rayOffset = new Vector2(0, -Height / 2.5f);
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Trap);

            clipShoot = AudioManager.GetAudioClip("blowGun");
        }

        public override void Update()
        {
            base.Update();

            if (Player.Position.X > Position.X)
            {
                FlipX = true;
            }
            else if(FlipX)
                FlipX = false;
        }

        public void Shoot()
        {
            Vector2 attackOffset;

            if (!IsCrouched)
            {
                attackOffset = standUpShootOffset;
            }
            else
            {
                attackOffset = crouchShootOffset;
            }

            attackOffset.X = FlipX ? attackOffset.X : -attackOffset.X;
            PlayAudio3D(clipShoot);
            new BlowGunBullet(Position + attackOffset).Shoot(new Vector2(Math.Sign(attackOffset.X), 0));  //Queue pooling???
        }

        public void StandUp()
        {
            if (IsCrouched)
            {
                IsCrouched = false;
                ChangeAnim(AnimationType.STAND_UP);
                SwitchCollider();
            }
            else
                Idle();
        }

        public void Crouch()
        {
            if (!IsCrouched)
            {
                IsCrouched = true;
                ChangeAnim(AnimationType.CROUCH);
                SwitchCollider();
            }
        }

        public void Idle()
        {
            ChangeAnim(AnimationType.IDLE);
        }

        public void StartAttackAnim()
        {
            if (IsCrouched)
            {
                ChangeAnim(AnimationType.ATTACK_CROUCH);
            }
            else
            {
                ChangeAnim(AnimationType.ATTACK);
            }
        }

        private void ChangeAnim(AnimationType type)
        {
            Animation = animations[(int)type];
            Animation.Reset();

            CurrentAnimation = type;
        }

        private void SwitchCollider()
        {
            if (colliderStandUp.IsActive)
            {
                colliderStandUp.IsActive = false;
                colliderCrouch.IsActive = true;
                RigidBody.SetBoundingBox(colliderCrouch);

            }
            else
            {
                colliderCrouch.IsActive = false;
                colliderStandUp.IsActive = true;
                RigidBody.SetBoundingBox(colliderStandUp);
            }
        }

        public override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            Velocity = Vector2.Zero;
            base.OnCheckpointLoad(checkpoint);
            IsCrouched = false;
            LookDirection = new Vector2(-1, 0);

           // Animation.Reset();
        }
    }
}
