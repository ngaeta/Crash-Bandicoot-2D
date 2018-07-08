using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class CrystalPickable : Pickable
    {
        public CrystalPickable(Vector2 spritePosition, string spriteSheetName="crystal", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            ClipOnPicked = AudioManager.GetAudioClip("crystalPicked");
        }

        public override void OnPlayerPick()
        {
            base.OnPlayerPick();
            GameManager.CrystalOfLevel = this;
            GuiManager.ActiveCrystal(true);
            OnDie();
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
           
        }

        public override void OnHit(GameObject objectHit)
        {
           
        }

        public override void OnDie()
        {
            IsActive = false;
            RigidBody.IsCollisionsAffected = true;
        }
    }
}
