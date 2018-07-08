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
    class Player : GameObject, ICheckpointLoadable
    {
        public enum State
        {
            Idle,
            Run,
            Attack,
            JumpStopped,
            Landed,
            Slide,
            JumpInMovement,
            Crouch,
            CrouchMoving,
            DeathBurnt,
            DeathAngel,
            DeathFalling,
            DeathMashed,
            DeathDrowned,
            DeathRotation,
            Waiting
        }

        const float TIME_INVULNERABILITY_AFTER_HITTED = 2.5f;
        const float TIME_BETWEEN_ATTACK = 0.8f;

        private bool isHitted;
        private float currTimeInvulnerability;
        private float timeBlink;
        private bool isGrounded;
        private int apples;
        private int life;
        private Dictionary<State, Rect> colliders;
        private AkuAku akuAku;
        private AudioSource audioSource;
        private Dictionary<State, AudioClip> clipsState;
        private GameObject parent;
        private Vector2 offsetParent;
        private Vector2 initialPosition;
        private AudioSource audioSourceFootStep;
        private AudioClip clipFootStep;

        public bool StopInput { get; set; }
        public bool IsInvincible { get { return akuAku.IsInvincible; } }
        public float TimeInAir { get; private set; }
        public float TimeNextAttack { get; protected set; }
        public Vector2 Speed { get; set; }
        public State CurrentState { get; protected set; }
        public StateMachine StateMachine { get; protected set; }
        public bool IsCrouched { get; private set; }
        public bool IsDead { get; private set; }
        public Vector2 VelocityLanded { get; private set; }
        public Vector2 OffsetHead { get; set; }
        public Vector2 HeadPosition { get; set; }

        //meglio usar raycast verso l alto???
        public GameObject ObstacleStandUp { get; set; }

        public int Apples
        {
            get { return apples; }
            set
            {
                if (value >= 100)
                {
                    apples = 0;
                    new ExtraLife(Vector2.Zero).OnPlayerPick();
                }
                else
                    apples = value;

                GuiManager.SetGuiValue(GuiManager.GuiObjectType.APPLE, apples);
            }
        }

        public int Life
        {
            get { return life; }
            set
            {
                if (value < 100)
                {
                    life = value;
                    GuiManager.SetGuiValue(GuiManager.GuiObjectType.LIFE, value);
                }
            }
        }

        public bool IsGrounded
        {
            get { return isGrounded; }
            set
            {
                isGrounded = value;
                RigidBody.IsGravityAffected = !isGrounded;

                if (value)
                {
                    TimeInAir = 0;
                    VelocityLanded = Velocity;
                    RigidBody.SetYVelocity(0);
                }
            }
        }

        public Player(Vector2 spritePosition, string spriteSheetName = "crash", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            CreateColliders();
            SetAudioClips();

            RigidBody = new RigidBody(sprite.position, this, null, colliders[State.Idle], true);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Player;
            RigidBody.SetCollisionMask((uint)(PhysicsManager.ColliderType.Ground | PhysicsManager.ColliderType.Crate | PhysicsManager.ColliderType.Trap));

            sprite.scale = new Vector2(1.5f, 1.5f);
            Speed = new Vector2(150f, -220f);
            VelocityLanded = Vector2.Zero;

            audioSource = new AudioSource();
            audioSourceFootStep = new AudioSource();

            animations[(int)State.Waiting].LoopAtFrame(12);
            animations[(int)State.JumpStopped].LoopAtFrame(3);
            animations[(int)State.JumpInMovement].LoopAtFrame(7);
            animations[(int)State.Slide].LoopAtFrame(4);
            animations[(int)State.Crouch].LoopAtFrame(2);
            animations[(int)State.DeathBurnt].WaitForNextFrame(0, 1.5f);
            animations[(int)State.DeathBurnt].LoopAtFrame(14);
            animations[(int)State.DeathFalling].LoopAtFrame(7);
            animations[(int)State.DeathDrowned].LoopAtFrame(5);
            animations[(int)State.DeathRotation].LoopAtFrame(9);

            StateMachine = new StateMachine(this);
            PlayerState.Player = this;

            StateMachine.RegisterState((int)State.Idle, new IdleState());
            StateMachine.RegisterState((int)State.Run, new RunState());
            StateMachine.RegisterState((int)State.JumpStopped, new JumpState());
            StateMachine.RegisterState((int)State.JumpInMovement, new LongJumpState());
            StateMachine.RegisterState((int)State.Landed, new LandedState());
            StateMachine.RegisterState((int)State.Attack, new AttackState());
            StateMachine.RegisterState((int)State.Slide, new SlideState());
            StateMachine.RegisterState((int)State.Crouch, new CrouchState());
            StateMachine.RegisterState((int)State.CrouchMoving, new CrouchMovingState());
            StateMachine.RegisterState((int)State.DeathBurnt, new DeathBurntState());
            StateMachine.RegisterState((int)State.DeathAngel, new DeathAngelState());
            StateMachine.RegisterState((int)State.DeathFalling, new DeathFalling());
            StateMachine.RegisterState((int)State.DeathDrowned, new DeathDrownedState());
            StateMachine.RegisterState((int)State.DeathMashed, new DeathMashedState());
            StateMachine.RegisterState((int)State.DeathRotation, new DeathRotationState());
            //registrare altre due deaths

            StateMachine.Switch((int)State.Idle);

            isHitted = false;
            currTimeInvulnerability = TIME_INVULNERABILITY_AFTER_HITTED;
            Apples = 0;
            Life = 4;
            timeBlink = 0; //when the player is hitted
            akuAku = new AkuAku(Vector2.Zero, this);

            offsetParent = Vector2.Zero;

            ExtraLife.GuiObject = (NumerableGUIObject)GuiManager.GetGuiObject(GuiManager.GuiObjectType.LIFE);
            Apple.GuiObject = (NumerableGUIObject)GuiManager.GetGuiObject(GuiManager.GuiObjectType.APPLE);

            initialPosition = Position;

            clipFootStep = AudioManager.GetAudioClip("grassFootStep");
            //OnAkuAkuPicked();
            //OnAkuAkuPicked();
            //OnAkuAkuPicked();
        }


        public void Input()
        {
            if (IsActive && !IsDead && !StopInput)
            {
                StateMachine.Input();

                //valido per ogni stato tranne tackle
                if (InputManager.GetButton(Button.Attack) && !IsCrouched && !IsInvincible)
                {
                    if (TimeNextAttack <= 0 && CurrentState != State.Attack && CurrentState != State.Slide)
                    {
                        TimeNextAttack = TIME_BETWEEN_ATTACK;
                        StateMachine.Switch((int)State.Attack);
                    }
                    else if (TimeNextAttack > 0)
                        TimeNextAttack -= Game.DeltaTime;
                }
                else if (TimeNextAttack > 0)
                    TimeNextAttack -= Game.DeltaTime;
            }
        }

        public override void Update()
        {
            base.Update();
            StateMachine.Run();

            if (!IsDead)
            {
                if (isHitted)
                {
                    if (currTimeInvulnerability > 0)
                    {
                        currTimeInvulnerability -= Game.DeltaTime;
                        timeBlink += Game.DeltaTime * 30;

                        float multiply = (float)Math.Cos(timeBlink);
                        sprite.SetMultiplyTint(new Vector4(multiply, multiply, multiply, multiply));
                    }
                    else
                    {
                        isHitted = false;
                        timeBlink = 0;
                        currTimeInvulnerability = TIME_INVULNERABILITY_AFTER_HITTED;
                        sprite.SetMultiplyTint(Vector4.One);
                    }
                }

                //va controllato in qualunque stato o quasi
                if (!IsGrounded)
                {
                    TimeInAir += Game.DeltaTime;

                    //se sta in aria ma non è in landscape o non sta attaccando, prima tme in air era 0.1f
                    if (TimeInAir > 0.2f && CurrentState != State.JumpStopped && CurrentState != State.Attack && CurrentState != State.JumpInMovement)
                    {
                        Fall();
                    }

                    if (Position.Y - Width / 2 > CameraManager.ViewPortDown && !IsDead)
                    {
                        OnDie(State.DeathFalling);
                        return;
                    }
                }

                if (IsInvincible)
                {
                    OffsetHead = new Vector2(sprite.FlipX ? -Math.Abs(OffsetHead.X) : OffsetHead.X, OffsetHead.Y);
                    HeadPosition = Position + OffsetHead;
                }

                if (parent != null)
                {
                    if (Velocity.Length == 0 && offsetParent != Vector2.Zero)
                    {
                        Position = parent.Position - new Vector2(0, Height / 2) + new Vector2(offsetParent.X, 0);
                        offsetParent = Vector2.Zero;
                    }
                    else
                        offsetParent = Position - parent.Position;
                }

                parent = null;
                IsGrounded = false;
            }
        }

        public override void OnCollide(Collision collisionInfo)
        {
            if (collisionInfo.collider is IWalkable w && w.CanWalkable)
            {
                float deltaX = collisionInfo.Delta.X;
                float deltaY = collisionInfo.Delta.Y;

                if (deltaX < deltaY)
                {
                    if (Velocity.X != 0)//modifica, in caso di problemi togliere
                    {
                        //collision from right or left
                        if (Position.X < collisionInfo.collider.X)
                        {
                            //from letf
                            deltaX = -deltaX;
                        }

                        Position = new Vector2(Position.X + deltaX, Position.Y);
                        //Velocity = new Vector2(0, Velocity.Y);    //problemi con il tackle
                    }
                }
                else
                {
                    //collision from top or bottom
                    if (Position.Y < collisionInfo.collider.Y)
                    {
                        //from top
                        deltaY = -deltaY;

                        if (w.AudioFootStep != null)
                            clipFootStep = w.AudioFootStep;

                        if (collisionInfo.collider is Crate c)
                            c.PlayerImpactY = TimeInAir;

                        if (collisionInfo.collider.Velocity.Length != 0)
                        {
                            parent = collisionInfo.collider;
                            offsetParent = Position - parent.Position;
                        }

                        if (!IsGrounded)
                        {
                            IsGrounded = true;
                        }
                    }
                    else
                    {
                        //fromBottom
                        if (CurrentState == State.Idle)
                        {
                            StateMachine.Switch((int)State.Crouch);
                            ObstacleStandUp = collisionInfo.collider;
                            deltaY = 0;  //entrava nel pavimento
                        }
                        else
                            deltaY *= 2;

                    }

                    if (parent == null)
                    {
                        Velocity = new Vector2(Velocity.X, 0);
                        Position = new Vector2(Position.X, Position.Y + deltaY);
                    }
                    else
                    {
                        Position = new Vector2(Position.X, Position.Y + deltaY);
                    }
                }
            }
        }

        public void MoveX(float velocity)
        {
            if (velocity < 0)
                sprite.FlipX = true;
            else
                sprite.FlipX = false;

            RigidBody.SetXVelocity(velocity);
        }

        public void Attack()
        {
            ChangeState(State.Attack);
            //ActiveCollider(State.Attack);
        }

        public void Jump(float multiplierForce = 1f)
        {
            if (IsGrounded)
            {
                IsGrounded = false;
            }

            RigidBody.SetYVelocity(Speed.Y * multiplierForce);

            State jumpType = State.JumpStopped;

            if ((sprite.FlipX && Velocity.X < 0) || (!sprite.FlipX && Velocity.X > 0))
            {
                jumpType = Math.Abs(Velocity.X) < 35f ? State.JumpStopped : State.JumpInMovement;
            }

            StateMachine.Switch((int)jumpType);
        }

        public void Fall()
        {
            if (IsGrounded)
            {
                IsGrounded = false;
            }

            StateMachine.Switch((int)State.JumpStopped);
        }

        public void Tackle()
        {
            RigidBody.SetXVelocity(Velocity.X * 1.5f);
            //ActiveCollider(State.Slide);
            ChangeState(State.Slide);
        }

        public void Crouch()
        {
            IsCrouched = true;
            RigidBody.SetXVelocity(0);
            //ActiveCollider(State.Crouch);
            ChangeState(State.Crouch);
        }

        public void CrouchMoving()
        {
            IsCrouched = true;
            //ActiveCollider(State.CrouchMoving);
            ChangeState(State.CrouchMoving, false);
        }

        public void StandUp()
        {
            IsCrouched = false;
            RigidBody.SetXVelocity(0);
            //ActiveCollider(State.Idle);
            ChangeState(State.Idle);
        }

        public void OnHit(State deathType)
        {
            //se è vulnerabile
            if (!isHitted && !IsInvincible)
            {
                if (akuAku.IsActive)
                {
                    isHitted = true;
                    akuAku.LevelDown();

                    if (!IsCrouched)
                        Jump();
                }
                else
                {
                    OnDie(deathType);
                }
            }
        }

        public void OnDie(State deathType)
        {
            if (!IsDead)
            {
                IsDead = true;
                IsCrouched = false;

                if (akuAku.IsActive)
                    akuAku.ResetLevel();

                ChangeState(deathType);
                StateMachine.Switch((int)deathType);
            }
        }

        public void OnAkuAkuPicked()
        {
            if (akuAku.LevelUp())
            {
                RigidBody.SetXVelocity(0);
                Jump(0.5f);
            }
        }

        public void ChangeState(State anim, bool reset = true)
        {
            CurrentState = anim;
            ActiveCollider(anim);
            Animation = animations[(int)anim];
            PlayAudio();

            if (reset)
                Animation.Reset();
        }

        public bool AudioSourceIsPlaying()
        {
            return audioSource.IsPlaying;
        }

        private void PlayAudio()
        {
            if (clipsState.ContainsKey(CurrentState))
            {
                audioSource.Play(clipsState[CurrentState]);
            }
        }

        public void PlayAudioFootStep()
        {
            if (!audioSourceFootStep.IsPlaying)
            {
                float randomPitch = RandomGenerator.GetRandom(700, 1000) / 1000f;

                audioSourceFootStep.Pitch = randomPitch;
                audioSourceFootStep.Play(clipFootStep);
            }
        }

        public void StopFootStep()
        {
            audioSourceFootStep.Stop();
        }

        private void CreateColliders()
        {
            colliders = new Dictionary<State, Rect>();

            Rect colliderIdle = new Rect(new Vector2(0, 7), null, Width - 5, Height + 8);

            Rect colliderTackle = new Rect(new Vector2(0, 18), null, Width + 5, Height - 14);
            colliderTackle.IsActive = false;

            Rect colliderCrouch = new Rect(new Vector2(9, 18), null, Width - 10, Height - 14);
            colliderCrouch.IsActive = false;

            Rect colliderCrouchMove = new Rect(new Vector2(4, 18), null, Width + 18, Height - 14);
            colliderCrouchMove.IsActive = false;

            Rect colliderAttack = new Rect(new Vector2(0, 7), null, Width + 15, Height + 8);
            colliderAttack.IsActive = false;

            Rect colliderJump = new Rect(new Vector2(0, 1), null, Width - 14, Height + 13);
            colliderJump.IsActive = false;

            Rect colliderJumpMovement = new Rect(new Vector2(0, 5), null, Width - 5, Height + 8);
            colliderJumpMovement.IsActive = false;

            colliders.Add(State.Idle, colliderIdle);
            colliders.Add(State.Slide, colliderTackle);
            colliders.Add(State.Crouch, colliderCrouch);
            colliders.Add(State.CrouchMoving, colliderCrouchMove);
            colliders.Add(State.Attack, colliderAttack);
            colliders.Add(State.JumpStopped, colliderJump);
            colliders.Add(State.JumpInMovement, colliderJumpMovement);
        }

        private void ActiveCollider(State colliderKey)
        {
            if (colliders.ContainsKey(colliderKey))
            {
                foreach (KeyValuePair<State, Rect> collider in colliders)
                {
                    if (collider.Value.IsActive)
                    {
                        collider.Value.IsActive = false;
                    }
                }

                Rect colliderToActive = colliders[colliderKey];
                RigidBody.SetBoundingBox(colliderToActive);
                colliderToActive.IsActive = true;

                Vector2 offsetPosition = new Vector2(Math.Abs(colliderToActive.RelativePosition.X), colliderToActive.RelativePosition.Y);

                if (sprite.FlipX)
                    offsetPosition.X = -offsetPosition.X;

                colliderToActive.RelativePosition = offsetPosition;
            }
        }

        private void SetAudioClips()
        {
            clipsState = new Dictionary<State, AudioClip>();

            clipsState.Add(State.Attack, AudioManager.GetAudioClip("crashSpin"));
            clipsState.Add(State.JumpStopped, AudioManager.GetAudioClip("crashHop"));
            clipsState.Add(State.JumpInMovement, AudioManager.GetAudioClip("crashHop"));
            clipsState.Add(State.Slide, AudioManager.GetAudioClip("crashSlide"));
            clipsState.Add(State.DeathBurnt, AudioManager.GetAudioClip("crashWoah"));
            clipsState.Add(State.DeathAngel, AudioManager.GetAudioClip("deathAngel"));
            clipsState.Add(State.DeathFalling, AudioManager.GetAudioClip("fallingDeath"));
            clipsState.Add(State.DeathDrowned, AudioManager.GetAudioClip("crashWoah"));
            clipsState.Add(State.DeathMashed, AudioManager.GetAudioClip("crashWoah"));
            clipsState.Add(State.DeathRotation, AudioManager.GetAudioClip("crashWoah"));
        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            IsDead = false;
            isHitted = false;

            timeBlink = 0;

            Position = checkpoint != null ? checkpoint.Position : initialPosition;
            Position = new Vector2(Position.X, Position.Y - Height);
            CameraManager.mainCamera.position = Position;
            CameraManager.ResetCameraViewPort();

            Apple.GuiObject.ResetPosition();
            ExtraLife.GuiObject.ResetPosition();

            Velocity = Vector2.Zero;
            RigidBody.IsCollisionsAffected = true;
            sprite.SetMultiplyTint(Vector4.One);

            StateMachine.Switch((int)State.Idle);

            akuAku.OnCheckpointLoad(checkpoint);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            colliders = null;
            akuAku = null;
            audioSource = null;
            clipsState = null;
            parent = null;
            audioSourceFootStep = null;
            clipFootStep = null;
            StateMachine = null;
            ObstacleStandUp = null;
        }
    }
}
