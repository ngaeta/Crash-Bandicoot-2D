using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class LizardEnemy : Enemy
    {
        enum AnimationType { Idle, Walk, Turning_Around }

        const float VELOCITY_MULTIPLIER_TURNING = 3f;
        const int MIN_RANDOM_CRY = 4;
        const int MAX_RANDOM_CRY = 6;

        private Player.State playerDieAnimationOnHitted;
        private float counterShake;
        private Smoke smoke;
        private float walkAnimSpeed;
        private float speedRun;
        private float animationSpeedIncrement;
        private float rangeAttack;
        private float speedWalk;
        private AudioClip clipCry;
        private float nextCry;

        public bool IsTurningAround { get; private set; }
        public float XFinishAttack { get; protected set; }
        public float XStartAttack { get; protected set; }

        public LizardEnemy(Vector2 spritePosition, float patrolXDist = 120, string spriteSheetName = "lizard", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(1.3f, 1.3f);

            speedWalk = 50f;
            speedRun = PlayScene.Player != null ? PlayScene.Player.Speed.X + 70 : 230f;
            SightRadius = 250f;

            IsTurningAround = false;

            animationSpeedIncrement = 0.15f;
            rangeAttack = SightRadius * 2f;

            Machine = new StateMachine(this);
            Machine.RegisterState((int)State.Patrol, new LizardPatrolState(new Tuple<float, float>(Position.X - patrolXDist, Position.X + patrolXDist)));
            Machine.RegisterState((int)State.Alert, new LizardAlertState());
            Machine.RegisterState((int)State.Attack, new LizardAttackState());
            Machine.Switch((int)State.Patrol);

            playerDieAnimationOnHitted = Player.State.DeathBurnt;
            smoke = new Smoke(Position, "smoke", DrawManager.Layer.Middleground);

            initialFlipX = FlipX;
            clipCry = AudioManager.GetAudioClip("lizard");
            nextCry = 2f;
        }

        public override void Update()
        {
            base.Update();

            if (isHitted)
            {
                smoke.IsActive = false;
            }
            else if (smoke.IsActive)
            {
                smoke.Position = Position;
            }

            if (!isHitted)
            {
                if (IsTurningAround)
                    TurningAroundUpdate();

                if (!CameraManager.OutOfCameraViewPort(this) && IsActive)
                {
                    if (nextCry <= 0)
                    {
                        float randomPitch = RandomGenerator.GetRandom(10, 16) / 10;
                        PlayAudio3D(clipCry, false, randomPitch);
                        NextCry();
                    }
                    else
                        nextCry -= Game.DeltaTime;
                }
            }      
        }

        public void Walk()
        {
            Animation = animations[(int)AnimationType.Walk];
            currState = State.Patrol;

            RigidBody.SetXVelocity(speedWalk);
            Velocity = FlipX ? Velocity : -Velocity;
            sprite.SetAdditiveTint(Vector4.Zero);

            if (smoke != null && smoke.IsActive)
            {
                smoke.IsActive = false;
            }
        }

        public void OnAlert()
        {
           PlayAudio3D(clipCry, false, 2f);

            currState = State.Alert;
            RigidBody.Velocity = Vector2.Zero;
        }

        public void OnAttack()
        {
            currState = State.Attack;
            smoke.IsActive = true;

            XStartAttack = Position.X;
            float playerDir = Player.Position.X - Position.X;
            XFinishAttack = Position.X + (Math.Sign(playerDir) * (rangeAttack + RandomGenerator.GetRandom(0, 50)));

            RigidBody.SetXVelocity(Math.Sign(playerDir) * speedRun);
            Animation.Speed -= animationSpeedIncrement;
        }

        public void FinishAttack()
        {
            Animation.Speed += animationSpeedIncrement;
        }

        protected override void OnCollisionFromX(Collision collisionInfo)
        {
            if (collisionInfo.collider is Player p)
            {
                if (currState == State.Attack)
                {
                    p.OnHit(playerDieAnimationOnHitted);
                }
            }

            base.OnCollisionFromX(collisionInfo);
        }

        protected override void OnCollisionFromY(Collision collisionInfo)
        {
            if (collisionInfo.collider is Player p)
            {
                if (currState == State.Attack)
                {
                    p.OnHit(playerDieAnimationOnHitted);
                }
                else
                {
                    p.OnHit(playerDieAnim);
                }
            }
        }

        protected override void OnCollideWithObjX(GameObject obj)
        {

            base.OnCollideWithObjX(obj);

            if (currState == State.Attack)
            {
                ReturnPatrol();
            }
            else
                TurningAround();

        }

        protected override void OnOutOfWalkable(RigidBody rigid)
        {
            base.OnOutOfWalkable(rigid);

            if (currState == State.Attack)
            {
                ReturnPatrol();
            }
            else
                TurningAround();
        }

        public void ReturnPatrol()
        {
            Machine.Switch((int)State.Patrol);
        }

        public void TurningAround()
        {
            Animation = animations[(int)AnimationType.Turning_Around];
            Animation.Reset();
            Animation.Play();  //forziamo isPlaying a true altrimenti turninaroundupdate uscira subito.

            IsTurningAround = true;
            Velocity = -Velocity;
            Velocity /= VELOCITY_MULTIPLIER_TURNING;
        }

        private void TurningAroundUpdate()
        {
            if (!Animation.IsPlaying)
            {
                Animation = animations[(int)AnimationType.Walk];
                IsTurningAround = false;
                FlipX = !FlipX;
                Velocity *= VELOCITY_MULTIPLIER_TURNING;
            }
        }

        public override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);

            LookDirection = new Vector2(-1, 0);
            IsTurningAround = false;
            Machine.Switch((int)State.Patrol);
        }

        protected override void OnDie()
        {
            base.OnDie();
            smoke.IsActive = false;
        }

        private void NextCry()
        {
            nextCry = RandomGenerator.GetRandom(MIN_RANDOM_CRY, MAX_RANDOM_CRY + 1);
        }
    }
}
