using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class LizardAlertState : EnemyState
    {
        private float counter = 0;

        public override void Enter()
        {
            base.Enter();

            ((LizardEnemy)Owner).OnAlert();
            counter = 0;
        }

        public override void Update()
        {
            base.Update();

            counter += Game.DeltaTime;

            float colorAdd = (float)Math.Sin(counter);
            float shakeX = (float)Math.Sin(counter * 50);

            if (colorAdd >= 0.95f)
            {
                colorAdd = 1;
                Owner.Machine.Switch((int)Enemy.State.Attack);
            }

            Owner.Position = new Vector2(Owner.Position.X + shakeX, Owner.Position.Y);
            Owner.GetSprite().SetAdditiveTint(colorAdd, 0, 0, 0);
        }
    }
}
