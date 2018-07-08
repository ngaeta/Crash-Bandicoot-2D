using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class TriggerIronCrate : IronCrate
    {
        protected List<BorderCrate> cratesToActive;

        public TriggerIronCrate(Vector2 spritePosition, string spriteSheetName = "activationCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            Animation.LoopAtFrame(9);
            Animation.IsActive = false;
            cratesToActive = new List<BorderCrate>();
         
            Rect rect = new Rect(new Vector2(-2, 22), RigidBody, Width - 16, Height/2 - 5);
            RigidBody.SetBoundingBox(rect);
            clipOnHit = AudioManager.GetAudioClip("crateTriggerHit");

           UseGroundableGravity = false;
        }

        public override void OnHit(GameObject hitObject)
        {
            if (!Animation.IsActive)
            {
                base.OnHit(hitObject);

                Animation.IsActive = true;

                for (int i = 0; i < cratesToActive.Count; i++)
                {
                    cratesToActive[i].ActiveCrate();
                }
            }
        }

        public void AddCrate(BorderCrate crate)
        {
            cratesToActive.Add(crate);
        }

        protected override void OnCollisionFromY(Player player, Collision colllisionInfo)
        {
            base.OnCollisionFromY(player, colllisionInfo);

            if (!Animation.IsActive)
            {
                float deltaY = colllisionInfo.Delta.Y;

                if (deltaY < 0)
                {
                    OnHit(player);
                }

                else if (PlayerImpactY >= minInpactToDetectYCollision)
                {
                    player.Jump(bounceMultiplierY);
                    OnHit(player);
                }
            }
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);
            if (checkpoint == null || checkpoint.Position.X < Position.X)
            {
                Animation.Reset();
                Animation.IsActive = false;
                //for (int i = 0; i < cratesToActive.Count; i++)
                //{
                //    NitroCrate nitroToLoad = nitroToExplodeOnCheckpointLoaded[i];
                //    nitroToExplode.Add(new NitroCrate(nitroToLoad.Position));
                //}
            }

        }
    }
}
