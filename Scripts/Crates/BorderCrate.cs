using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    public enum CrateType
    {
        Apple,
        Random,
        Life,
        Aku,
        Bounce_Apple,
        Bounce_Wooden,
        Bounce_Iron,
        Iron,
        Trigger_Iron,
        Tnt,
        Nitro,
        Border_Crate,
        Trigger_Iron_Nitro,
        Checkpoint_Crate
    }

    class BorderCrate : GameObject, ICheckpointLoadable
    {
        const float TIME_TO_TRIGGER_CRATE = 0.6f;

        private Crate crate;
        private AudioSource audioSource;
        private bool crateTriggered;
        private float timeToTriggered;

        protected AudioClip clipTriggered;

        public Crate Crate
        {
            set
            {
                crate = value;
                crate.UseGroundableGravity = false;
                sprite.scale = value.GetSprite().scale;
                Position = crate.Position;
                value.IsActive = false;
            }
        }

        public BorderCrate(Crate crate) : base(Vector2.Zero, "borderCrate", DrawManager.Layer.Playground)
        {
            Crate = crate;
           
            clipTriggered = AudioManager.GetAudioClip("crateTriggered");
            crateTriggered = false;
            timeToTriggered = TIME_TO_TRIGGER_CRATE;

            PhysicsManager.RemoveItem(RigidBody);
            RigidBody = null;
        }

        public BorderCrate(Vector2 position, CrateType type) : base(position, "borderCrate", DrawManager.Layer.Playground)
        {
            Crate c = null;

            switch (type)
            {
                case CrateType.Apple:
                    c = new AppleCrate(position);
                    break;
                case CrateType.Random:
                    c = new RandomCrate(position);
                    break;
                case CrateType.Life:
                    c = new LifeCrate(position);
                    break;
                case CrateType.Aku:
                    c = new AkuAkuCrate(position);
                    break;
                case CrateType.Bounce_Apple:
                    c = new AppleBounceCrate(position);
                    break;
                case CrateType.Bounce_Wooden:
                    c = new WoodenBounceCrate(position);
                    break;
                case CrateType.Bounce_Iron:
                    c = new IronBounceCrate(position);
                    break;
                case CrateType.Iron:
                    c = new IronCrate(position);
                    break;
                case CrateType.Trigger_Iron:
                    c = new TriggerIronCrate(position);
                    break;
                case CrateType.Tnt:
                    c = new TntCrate(position);
                    break;
                case CrateType.Nitro:
                    c = new NitroCrate(position);
                    break;
                default:
                    c = new IronCrate(position);
                    break;
            }

            Crate = c;

            clipTriggered = AudioManager.GetAudioClip("crateTriggered");
            crateTriggered = false;
            timeToTriggered = TIME_TO_TRIGGER_CRATE;

            PhysicsManager.RemoveItem(RigidBody);
            RigidBody = null;
    }

        public override void Update()
        {
            if(crateTriggered)
            {
                if (timeToTriggered <= 0)
                {
                    audioSource = new AudioSource();
                    audioSource.Play(clipTriggered);
                    AudioManager.DisposeAudioSource(audioSource);

                    IsActive = false;
                    crate.IsActive = true;
                    crateTriggered = false;
                }
                else
                    timeToTriggered -= Game.DeltaTime;
            }
        }

        public void ActiveCrate()
        {
            crateTriggered = true; 
        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                IsActive = true;
                crateTriggered = false;
                crate.IsActive = false;
                timeToTriggered = TIME_TO_TRIGGER_CRATE;
            }

            if(audioSource != null)
            {
                audioSource.Stop();
                audioSource.Dispose();
                audioSource = null;
            }
        }
    }
}
