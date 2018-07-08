using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class LifeCrate : DestructibleCrate
    {
        public LifeCrate(Vector2 spritePosition, string spriteSheetName = "lifeCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
        }

        public override void OnHit(GameObject hitObject)
        {
            if (hitObject is Player && !isHitted)
            {
                ExtraLife extraLife = (ExtraLife)ItemPickableManager.GetItem(PickableType.ExtraLife);
                extraLife.IsActive = true;
                extraLife.Position = Position;
                extraLife.OnPlayerPick();
            }

            base.OnHit(hitObject);
        }
    }
}
