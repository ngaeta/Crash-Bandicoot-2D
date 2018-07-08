using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class GUIObject : GameObject
    {
        public Vector2 PositionInCamera { get; protected set; }
        private Vector2 initialPos;

        public GUIObject(Vector2 spritePosition, string spriteSheetName) : base(spritePosition, spriteSheetName, DrawManager.Layer.GUI)
        {
            sprite.scale = new Vector2(2.5f, 2.5f);
            sprite.Camera = CameraManager.GetCamera("GUI");
            PositionInCamera = Position;

            initialPos = Position;
        }

        public override void Update()
        {
            base.Update();
            PositionInCamera+= CameraManager.DeltaCamera;
        }

        public void ResetPosition()
        {
            PositionInCamera = new Vector2(CameraManager.ViewPortLeft, CameraManager.ViewPortUp) + initialPos;
        }
    }
}
