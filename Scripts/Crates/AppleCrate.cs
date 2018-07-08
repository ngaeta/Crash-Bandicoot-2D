using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class AppleCrate : DestructibleCrate
    {
        public AppleCrate(Vector2 spritePosition, string spriteSheetName = "appleCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
        }

        public override void OnHit(GameObject hitObject)
        {
            if (hitObject is Player)
            {
                Apple apple = (Apple)ItemPickableManager.GetItem(PickableType.Apple);
                apple.IsActive = true;
                apple.Position = Position;
                apple.OnPlayerPick();
            }

            base.OnHit(hitObject);
        }
    }
}
