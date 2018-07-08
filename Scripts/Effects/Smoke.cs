using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class Smoke : GameObject
    {
        public Smoke(Vector2 spritePosition, string spriteSheetName="smoke", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            IsActive = false;
            sprite.SetMultiplyTint(new Vector4(1.5f));
        }

        public void AddSpeedAnimation(float speed)
        {
            Animation.Speed += speed;
        }
    }
}
