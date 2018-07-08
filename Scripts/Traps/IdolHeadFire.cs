using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class IdolHeadFire : GameObject
    {
        private Fire fire;

        private List<float> randomTimesList;
        private float currTimeToFire;
        private int indexFire;

        public IdolHeadFire(Vector2 spritePosition, string spriteSheetName = "idolHead") : base(spritePosition, spriteSheetName, DrawManager.Layer.Middleground)
        {
            fire = new Fire(spritePosition + new Vector2(0, 36f));

            List<float> randomNumbers = new List<float>
            {
                2.5f,
                3f,
                3.5f,
                5f,
            };

            randomTimesList = new List<float>();

            while (randomNumbers.Count > 0)
            {
                float randomNumber = randomNumbers[RandomGenerator.GetRandom(0, randomNumbers.Count)];
                randomTimesList.Add(randomNumber);

                randomNumbers.Remove(randomNumber);
            }

            indexFire = 0;
            currTimeToFire = indexFire;
        }

        public override void Update()
        {
            base.Update();

            if (!fire.IsActive)
            {
                if (currTimeToFire <= 0)
                {
                    fire.StartFire();
                    indexFire = ++indexFire % randomTimesList.Count;
                    currTimeToFire = randomTimesList[indexFire];
                }
                else
                    currTimeToFire -= Game.DeltaTime;
            }
        }
    }
}
