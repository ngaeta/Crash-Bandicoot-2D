using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    abstract class DeathState : PlayerState
    {
        protected float timeToLoadGame = 4f;

        public override void Update()
        {
            base.Update();

            if (timeToLoadGame <= 0)
            {
                PlayScene.IsLoadingCheckpoint = true;     
            }
            else
                timeToLoadGame -= Game.DeltaTime;
        }
    }
}
