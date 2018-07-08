using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class CheckpointCrate : Crate
    {
        enum AnimationType { OpenCrate }

        private AudioSource audioSource;
        private AudioClip clipHitted;

        private static List<ICheckpointLoadable> listLoadable;

        public CheckpointCrate(Vector2 spritePosition, string spriteSheetName = "checkpoint") : base(spritePosition, spriteSheetName)
        {
            animations[(int)AnimationType.OpenCrate].LoopAtFrame(14);
            Animation.IsActive = false;

            uint collisionMask = RigidBody.CollisionMask;
            PhysicsManager.RemoveItem(RigidBody);

            Rect collider = new Rect(new Vector2(0, 0), null, Width / 2.5f, Height / 2);
            RigidBody = new RigidBody(spritePosition, this, null, collider);
            RigidBody.SetCollisionMask(collisionMask);
            RigidBody.Type = (uint)PhysicsManager.ColliderType.Crate;

            clipHitted = AudioManager.GetAudioClip("checkpointHitted");
        }

        public override void OnHit(GameObject hitObject)
        {
            if(audioSource == null)
                 audioSource = new AudioSource();

            audioSource.Play(clipHitted);
            AudioManager.DisposeAudioSource(audioSource);

            isHitted = true;
            Animation.IsActive = true;
            RigidBody.IsCollisionsAffected = false;

            GameManager.CurrentCheckpoint = this;
        }

        public static void AddObjectsToLoad(ICheckpointLoadable loadable)
        {
            if (listLoadable == null)
                listLoadable = new List<ICheckpointLoadable>();

            listLoadable.Add(loadable);
        }

        public void LoadCheckPoint()
        {
            for (int i = 0; i < listLoadable.Count; i++)
            {
                if (listLoadable[i] == null)
                {
                    listLoadable.RemoveAt(i);
                    i--;
                }
                else
                    listLoadable[i].OnCheckpointLoad(this);
            }
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            player.Jump(bounceMultiplierY);
            OnHit(player);
        }
    }
}
