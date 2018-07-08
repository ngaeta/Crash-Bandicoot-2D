using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class SmokePuff : GameObject
    {
        public SmokePuff(Vector2 spritePosition, string spriteSheetName="smokePuff", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            IsActive = false;
            sprite.scale = sprite.scale / 2;
            sprite.SetMultiplyTint(new Vector4(0.6f, 0.6f, 0.6f, 0.6f));
        }

        public void SetActive(Vector2 position, bool value = true)
        {
            Position = position;
            IsActive = value;
        }
    }
}
