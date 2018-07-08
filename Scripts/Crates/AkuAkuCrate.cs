using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class AkuAkuCrate : DestructibleCrate
    {
        public AkuAkuCrate(Vector2 spritePosition, string spriteSheetName = "akuCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
        }

        public override void OnHit(GameObject hitObject)
        {
            if(hitObject is Player p && !isHitted)
            {
                p.OnAkuAkuPicked();
            }

            base.OnHit(hitObject);
        }
    }
}
