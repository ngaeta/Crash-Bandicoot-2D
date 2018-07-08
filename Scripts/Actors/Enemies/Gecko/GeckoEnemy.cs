using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class GeckoEnemy : Enemy
    {
        enum AnimationType { Walk, Turning_Around }

        const int MIN_RANDOM_CRY = 4;
        const int MAX_RANDOM_CRY = 7;

        private float speed;
        private float speedTurningAround;
        private float nextCry;
        private AudioClip clipCry;

        public bool IsTurningAround { get; private set; }

        public GeckoEnemy(Vector2 spritePosition, float patrolXDist = 120, string spriteSheetName = "gecko", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(1.2f, 1.2f);
            speed = 50f;
            speedTurningAround = speed / 2;
            Velocity = new Vector2(-speed, 0);
            IsTurningAround = false;
            playerDieAnim = Player.State.DeathAngel;
            RigidBody.SetBoundingBox(new Rect(Vector2.Zero, null, Width, Height / 2));

            Machine = new StateMachine(this);
            Machine.RegisterState((int)State.Patrol, new GeckoPatrolState(new Tuple<float, float>(Position.X - patrolXDist, Position.X + patrolXDist)));
            Machine.Switch((int)State.Patrol);

            AudioSource3D.VolumeOffset = -0.3f;
            clipCry = AudioManager.GetAudioClip("gecko");
            nextCry = 2f;
        }

        public override void Update()
        {
            base.Update();

            if (IsActive)
            {
                if (!CameraManager.OutOfCameraViewPort(this) && !isHitted)
                {
                    if (nextCry <= 0)
                    {
                        PlayAudio3D(clipCry);
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
            IsTurningAround = false;
            RigidBody.SetXVelocity(Math.Sign(Velocity.X) * speed);
        }

        public void TurningAround()
        {
            Animation = animations[(int)AnimationType.Turning_Around];
            Animation.Reset();

            IsTurningAround = true;
            RigidBody.SetXVelocity(-(Math.Sign(Velocity.X)) * speedTurningAround);
        }

        protected override void OnCollideWithObjX(GameObject obj)
        {
            base.OnCollideWithObjX(obj);

            TurningAround();
        }

        protected override void OnOutOfWalkable(RigidBody rigid)
        {
            base.OnOutOfWalkable(rigid);

            TurningAround();
        }

        private void NextCry()
        {
            nextCry = RandomGenerator.GetRandom(MIN_RANDOM_CRY, MAX_RANDOM_CRY + 1);
        }

        public override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);
            Animation.Reset();
            Velocity = new Vector2(-speed, 0);
            NextCry();
        }
    }
}
