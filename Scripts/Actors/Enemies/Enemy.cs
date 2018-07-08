using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    public enum EnemyType
    {
        Gecko,
        Lizard,
        BlowGunMan,
        Bird
    }

    abstract class Enemy : Groundable, IHittable, ICheckpointLoadable
    {
        public enum State { Patrol, Alert, Attack }

        private AudioSource audioSource;
        private Vector2 initialPos;

        protected bool initialFlipX;
        protected AudioClip clipDieOnHitSpinner;
        protected AudioClip clipDieOnHitY;

        protected Vector2 rayOffset;
        protected float halfConeAngle;
        protected bool isHitted;
        protected SmokePuff smokeOnHit;
        protected Player.State playerDieAnim;
        protected Player.State playerDieAnimOnAir;
        protected bool useMultiplyRay;
        protected List<PhysicsManager.ColliderType> ignoreMaskRayDownWard;
        protected List<PhysicsManager.ColliderType> ignoreMaskRaySight;
        protected State currState;

        public AudioSource3D AudioSource3D { get; private set; }
        public float SightRadius { get; protected set; }
        public Vector2 LookDirection { get; protected set; }
        public static Player Player { get; private set; }
        public StateMachine Machine { get; protected set; }

        public override Vector2 Velocity
        {
            get => base.Velocity;
            set
            {
                base.Velocity = value;
            }
        }

        public bool FlipX
        {
            get
            {
                return sprite.FlipX;
            }

            set
            {
                sprite.FlipX = value;
                LookDirection = new Vector2(-LookDirection.X, LookDirection.Y);
            }
        }

        public Enemy(Vector2 spritePosition, string spriteSheetName, DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            RigidBody = new RigidBody(spritePosition, this);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Enemy;
            uint collisionMask = (uint)(PhysicsManager.ColliderType.Player | PhysicsManager.ColliderType.Ground | PhysicsManager.ColliderType.Tile | PhysicsManager.ColliderType.Crate);
            RigidBody.SetCollisionMask(collisionMask);

            if (Player == null)
                Player = PlayScene.Player;

            playerDieAnim = Player.State.DeathRotation;
            playerDieAnimOnAir = Player.State.DeathAngel;

            AudioSource3D = new AudioSource3D(this);
            clipDieOnHitSpinner = AudioManager.GetAudioClip("spinHit");
            clipDieOnHitY = AudioManager.GetAudioClip("jumpEnemyDead");

            smokeOnHit = new SmokePuff(Position);
            SightRadius = 150f;
            LookDirection = new Vector2(-1, 0);
            halfConeAngle = MathHelper.DegreesToRadians(30);
            isHitted = false;
            rayOffset = Vector2.Zero;

            ignoreMaskRaySight = new List<PhysicsManager.ColliderType>();
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Water);
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Bullet);
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Explosion);
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Pickable);
            //ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Ground);

            ignoreMaskRayDownWard = new List<PhysicsManager.ColliderType>();
            ignoreMaskRayDownWard.Add(PhysicsManager.ColliderType.Explosion);
            ignoreMaskRayDownWard.Add(PhysicsManager.ColliderType.Water);
            ignoreMaskRaySight.Add(PhysicsManager.ColliderType.Pickable);


            UseGroundableGravity = false;
            initialPos = Position;
            initialFlipX = sprite.FlipX;
        }

        public override void Update()
        {
            base.Update();

            if (!isHitted)
            {
                if (Machine != null)
                    Machine.Run();

                if (RigidBody != null && Velocity.Length != 0)
                    CheckRayCastDownWard();
            }
            else
            {
                if (CameraManager.OutOfCameraViewPort(this))
                {
                    isHitted = false;
                    OnDie();
                }
            }
        }

        public bool CheckPlayerInFov()
        {
            if (!Player.IsDead)
            {
                //Vector2 distance = (Player.Position + new Vector2(0, -Player.RigidBody.BoundingBox.rectCollider.Height)) - (Position + new Vector2(0, -Height / 2));
                Vector2 distance = Player.Position - Position;
                //Vector2 distance = Position - Player.Position;

                if (Math.Abs(distance.Length) <= SightRadius)
                {
                    Vector2 distDirection = distance.Normalized();

                    float dot = Vector2.Dot(distDirection, LookDirection);
                    float deltaAngle = (float)Math.Acos(dot);

                    if (dot > 0 && deltaAngle <= halfConeAngle)
                    {
                        //he's quite near 
                        //-halfconeangle estremo sinistro
                        if (useMultiplyRay)
                        {
                            for (float offset = -halfConeAngle; offset < halfConeAngle; offset += halfConeAngle / 4)
                            {
                                Vector2 newDistDirection = Player.Position + new Vector2(0, halfConeAngle) - (Position + rayOffset);

                                Tuple<RigidBody, float> intersection = PhysicsManager.RayCast(Position, distDirection, RigidBody, distance.Length, ignoreMaskRaySight);

                                //if (intersection != null)
                                //{
                                //    if (intersection.Item1 != null)
                                //        Console.WriteLine(intersection.Item1.GameObject);
                                //}

                                if (intersection.Item1 == Player.RigidBody)
                                    return true;
                            }
                        }
                        else
                        {
                            Tuple<RigidBody, float> intersection = PhysicsManager.RayCast(Position, distDirection, RigidBody, distance.Length, ignoreMaskRaySight);

                            if (intersection.Item1 == Player.RigidBody)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public virtual void OnHit(GameObject hitObject)
        {
            if (hitObject is Player p)
            {
                if (p.Position.Y > Position.Y)
                {
                    OnHitFromY(p);
                    return;
                }

                PlayAudio2D(clipDieOnHitSpinner);
                isHitted = true;
                RigidBody.IsCollisionsAffected = false;
                Vector2 dir = (Position - hitObject.Position).Normalized();
                Velocity = new Vector2(1000 * 2f * dir.X, 1000 * 2f * dir.Y);
            }
            else
            {
                StartSmoke();
                OnDie();
            }
        }

        public override void OnCollide(Collision collisionInfo)
        {
            if (collisionInfo.collider is Player p && p.IsDead)
            {
                return;
            }

            base.OnCollide(collisionInfo);

            float deltaX = collisionInfo.Delta.X;
            float deltaY = collisionInfo.Delta.Y;

            if (deltaX < deltaY)
            {
                if (collisionInfo.collider.Position.X < Position.X)
                {
                    collisionInfo.Delta.X = -collisionInfo.Delta.X;
                }

                OnCollisionFromX(collisionInfo);
            }
            else
            {
                if (collisionInfo.collider.Position.Y > Position.Y)
                {
                    collisionInfo.Delta.Y = -collisionInfo.Delta.Y;
                }

                OnCollisionFromY(collisionInfo);
            }
        }

        protected virtual void OnCollisionFromX(Collision collisionInfo)
        {
            GameObject collider = collisionInfo.collider;

            if (collider is Player p && !p.IsDead)
            {
                if (p.CurrentState == Player.State.Attack || p.CurrentState == Player.State.Slide || p.IsInvincible)
                {
                    OnHit(p);
                }
                else
                    p.OnHit(playerDieAnim);
            }
            else
                OnCollideWithObjX(collider);
        }

        protected virtual void OnCollisionFromY(Collision collisionInfo)
        {
            GameObject collider = collisionInfo.collider;

            if (collider is Player p && !p.IsDead)
            {
                if (collisionInfo.Delta.Y > 0)
                {
                    if (p.CurrentState == Player.State.Attack || p.CurrentState == Player.State.Slide || p.IsInvincible)
                    {
                        OnHit(p);
                    }
                    else
                        OnHitFromY(p);
                }
                else
                    p.OnHit(playerDieAnimOnAir);  //aggiustare perchè cosi il player non puo colpirlo da sotto
            }
            else
                OnCollideWithObjY(collider);
        }

        protected void OnHitFromY(Player p)
        {
            p.Jump();
            StartSmoke();
            OnDie();
        }

        protected void StartSmoke()
        {
            smokeOnHit.SetActive(Position);
            PlayAudio2D(clipDieOnHitY);
        }

        protected virtual void OnDie()
        {
            IsActive = false;
            AudioManager.DisposeAudioSource(audioSource);

            if (AudioSource3D.IsPlaying)
                AudioSource3D.StopAudio();
        }


        protected virtual void OnCollideWithObjX(GameObject obj)
        {

        }
        protected virtual void OnCollideWithObjY(GameObject obj)
        {

        }

        protected void CheckRayCastDownWard()
        {
            Tuple<RigidBody, float> rigid = PhysicsManager.RayCast(Position + Math.Sign(Velocity.X) * new Vector2(35f, 0), new Vector2(0, 1), RigidBody, float.MaxValue, ignoreMaskRayDownWard);

            if (rigid.Item1 == null)
            {
                OnOutOfWalkable(rigid.Item1);
            }
            else if (!(rigid.Item1.GameObject is IWalkable w && w.CanWalkable) || rigid.Item2 >= Height * 2)
                OnOutOfWalkable(rigid.Item1);
        }

        protected virtual void OnOutOfWalkable(RigidBody rigid)
        {

        }

        public void PlayAudio2D(AudioClip clip, bool loop = false, float volume = 0.5f)
        {

            if (audioSource == null)
            {
                audioSource = new AudioSource();
            }

            audioSource.Volume = volume;
            audioSource.Play(clip, loop);

            if (!loop)
            {
                AudioManager.DisposeAudioSource(audioSource);
            }
        }
        public void PlayAudio3D(AudioClip clip, bool loop = false, float pitch = 1f)
        {
            AudioSource3D.PlayAudio(clip, loop, pitch);
        }


        public virtual void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            isHitted = false;
            IsActive = true;
            Position = initialPos;
            FlipX = initialFlipX;
            RigidBody.IsCollisionsAffected = true;

            smokeOnHit.Animation.Reset();
            smokeOnHit.IsActive = false;

            Machine.Switch((int)State.Patrol);
        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                OnCheckpointLoad(checkpoint);
            }

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.Dispose();
                audioSource = null;
            }

            if(AudioSource3D.IsPlaying)
            {
                AudioSource3D.StopAudio();
            }
        }
    }
}
