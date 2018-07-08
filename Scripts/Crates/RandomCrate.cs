using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class RandomCrate : DestructibleCrate
    {
        const int MAX_APPLE = 6;
        const int MIN_APPLE = 2;
        const float PROB_LIFE = 15;

        public RandomCrate(Vector2 spritePosition, string spriteSheetName = "randomCrate", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
        }

        public override void OnHit(GameObject hitObject)
        {
            if (hitObject is Player)
            {
                if (RandomGenerator.GetRandom(0, 100) < PROB_LIFE)
                {
                    ExtraLife extraLife = (ExtraLife)ItemPickableManager.GetItem(PickableType.ExtraLife);
                    extraLife.IsActive = true;
                    extraLife.Position = Position;
                    extraLife.OnPlayerPick();
                }
                else
                {
                    int randomApples = RandomGenerator.GetRandom(MIN_APPLE, MAX_APPLE + 1);
                    List<Pickable> apples = ItemPickableManager.GetItems(PickableType.Apple, randomApples);
                    Vector2 offsetBetweenApples = Vector2.Zero;

                    for (int i = 0; i < apples.Count; i++)
                    {
                        Apple apple = (Apple)apples[i];
                        apple.IsActive = true;
                        apple.Position = Position + offsetBetweenApples;
                        offsetBetweenApples += new Vector2(apple.Width / 3, apple.Height / 2);
                        apple.OnPlayerPick();
                    }
                }
            }

            base.OnHit(hitObject);
        }
    }
}
