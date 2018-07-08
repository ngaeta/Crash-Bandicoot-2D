using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class StoneWhellTrap : Trap, IWalkable, ITriggableAction, ICheckpointLoadable
    {
        const float ROTATION_SPEED = MathHelper.Pi * 20;

        private bool startRotation;
        private AudioSource audioSource;
        private AudioClip clipRotation;
        private AudioClip clipStopped;
        private float initialRotation;
        private Vector2 initialPos;

        public float VelocityOffset { get; set; }
        public bool CanWalkable { get; set; }

        public AudioClip AudioFootStep { get { return null; } }

        public StoneWhellTrap(Vector2 spritePosition, string spriteSheetName = "stoneWheel") : base(spritePosition, spriteSheetName, DrawManager.Layer.Foreground)
        {
            PhysicsManager.RemoveItem(RigidBody);

            Circle circle = new Circle(Vector2.Zero, null, Height / 1.8f);
            RigidBody = new RigidBody(spritePosition, this, circle);
            RigidBody.AddCollision((uint)PhysicsManager.ColliderType.Ground | (uint)PhysicsManager.ColliderType.Crate | (uint) PhysicsManager.ColliderType.Enemy | (uint) PhysicsManager.ColliderType.Player);
            RigidBody.Type = (uint) PhysicsManager.ColliderType.Trap;
            playerAnimDie = Player.State.DeathMashed;

            CanWalkable = true;

            initialPos = Position;
            initialRotation = sprite.EulerRotation;
            VelocityOffset = 8f;
            clipRotation = AudioManager.GetAudioClip("stoneWheelRotation");
            clipStopped = AudioManager.GetAudioClip("stoneHit");
        }

        protected override void OnCollisionWithObject(Collision collision)
        {
            base.OnCollisionWithObject(collision);

            if (collision.collider is IHittable h)
            {
                h.OnHit(this);
            }
            else
            {
                float deltaX = collision.Delta.X;
                float deltaY = collision.Delta.Y;

                if (deltaY > deltaX && Velocity.Length > 0 && !(collision.collider is Player p))
                {
                    Velocity = Vector2.Zero;
                    CanWalkable = true;
                    startRotation = false;
                    Position = new Vector2(Position.X - deltaX, Position.Y);
                    PlayAudio(clipStopped);

                    if(!PlayScene.Player.IsDead)
                    {
                        AudioManager.DisposeAudioSource(audioSource);
                    }
                }
            }
        }

        protected override void OnCollisionWithPlayer(Player p, Collision collisionInfo)
        {
            if (startRotation)
            {
                if (collisionInfo.Delta.X < collisionInfo.Delta.Y)
                    p.OnDie(playerAnimDie);
                else
                    p.OnDie(Player.State.DeathAngel);
            }
        }

        public override void Update()
        {
            base.Update();

            if (startRotation)
                sprite.EulerRotation += ROTATION_SPEED * Game.DeltaTime;
        }

        public void StartRotation()
        {
            PlayAudio(clipRotation, true);

            startRotation = true;
            CanWalkable = false;
            Velocity = PlayScene.Player.Speed + new Vector2(VelocityOffset, 0);
            RigidBody.SetYVelocity(0);
        }

        private void PlayAudio(AudioClip clip, bool loop = false)
        {
            if (audioSource == null)
            {
                audioSource = new AudioSource();
            }

            audioSource.Play(clip, loop);
        }

        void ITriggableAction.ActiveAction()
        {
            StartRotation();
        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if(audioSource != null && audioSource.IsPlaying)
            {
                audioSource.Stop();
            }

            startRotation = false;
            CanWalkable = true;
            Velocity = Vector2.Zero;
            Position = initialPos;
            sprite.EulerRotation = initialRotation;
        }
    }
}
