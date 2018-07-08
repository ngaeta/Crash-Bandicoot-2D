using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;
using OpenTK;

namespace CrashBandicoot
{
    public enum GroundType
    {
        Static,
        Mobile,
        Orbital,
        Branch,
        Leaf
    }

    class Ground : GameObject, IWalkable
    {
        protected AudioClip clipFootStep;

        public Ground(Vector2 spritePosition, string spriteSheetName = "ground", bool useRigidBody = true, DrawManager.Layer drawLayer = DrawManager.Layer.Middleground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            if (useRigidBody)
            {
                Rect rect = new Rect(new Vector2(0, 2f), RigidBody, Width, Height);
                RigidBody = new RigidBody(sprite.position, this, null, rect);
                RigidBody.Type = (uint)PhysicsManager.ColliderType.Ground;
                clipFootStep = AudioManager.GetAudioClip("grassFootStep");
                CanWalkable = true;
            }         
        }

        public bool CanWalkable { get; set; }
        public AudioClip AudioFootStep { get { return clipFootStep; } }
    }
}
