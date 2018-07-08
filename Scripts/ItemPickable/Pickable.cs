using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    abstract class Pickable : GameObject, IHittable, ICheckpointLoadable
    {
        private Vector2 initialPos;

        protected static Player player;
        protected bool isHitted;
        protected bool isPicked;

        protected AudioSource audioSource;
        protected AudioClip clipOnHitted;

        public AudioClip ClipOnPicked { get; protected set; }
        public PickableType Type { get; set; }

        public Pickable(Vector2 spritePosition, string spriteSheetName, DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            RigidBody = new RigidBody(spritePosition, this);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Pickable;
            RigidBody.SetCollisionMask((uint)PhysicsManager.ColliderType.Player);
            player = PlayScene.Player;

            clipOnHitted = AudioManager.GetAudioClip("wumpaHit");
            initialPos = Position;
        }

        public override void OnCollide(Collision collision)
        {
            base.OnCollide(collision);

            if (collision.collider is Player p && !p.IsDead)
            {
                if (p.CurrentState != Player.State.Attack)
                {
                    OnPlayerPick();
                }
                else
                    OnHit(p);
            }
        }

        public virtual void OnHit(GameObject objectHit)
        {
            PlayAudio(clipOnHitted);

            isHitted = true;
            RigidBody.IsCollisionsAffected = false;
            Vector2 dir = (Position - objectHit.Position).Normalized();
            Velocity = new Vector2(1000 * 2f * dir.X, 1000 * 2f * dir.Y);
        }

        public virtual void OnPlayerPick()
        {
            isPicked = true;
            RigidBody.IsCollisionsAffected = false;

            PlayAudio(ClipOnPicked);
        }

        public virtual void OnDie()
        {
            IsActive = false;
            RigidBody.IsCollisionsAffected = true;
            ItemPickableManager.RestoreItem(this);       
        }

        protected virtual void PlayAudio(AudioClip clip, float volume = 0.5f)
        {
            audioSource = new AudioSource();

            audioSource.Volume = volume;
            audioSource.Play(clip);
            AudioManager.DisposeAudioSource(audioSource);
        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if (checkpoint == null || checkpoint.Position.X < initialPos.X)
            {
                OnCheckpointLoad(checkpoint);
            }
        }

        protected virtual void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            isPicked = false;
            isHitted = false;
        }
    }
}
