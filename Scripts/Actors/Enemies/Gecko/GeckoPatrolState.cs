using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class GeckoPatrolState : EnemyState
    {
        private Tuple<float, float> patrolingArea;

        public GeckoPatrolState(Tuple<float, float> patrolingArea) : base()
        {
            this.patrolingArea = patrolingArea;
        }

        public override void Enter()
        {
            base.Enter();
            ((GeckoEnemy) Owner).Walk();
        }

        public override void Update()
        {
            base.Update();

            if (((GeckoEnemy)Owner).IsTurningAround)
            {
                if (!Owner.Animation.IsPlaying)
                {
                    Owner.FlipX = !Owner.FlipX;                
                    ((GeckoEnemy)Owner).Walk();
                }
            }
            else 
            {
                if (Owner.Position.X > patrolingArea.Item2 || Owner.Position.X < patrolingArea.Item1)
                {
                    Owner.Position = new Vector2(Owner.Velocity.X > 0 ? patrolingArea.Item2 : patrolingArea.Item1, Owner.Position.Y);
                    ((GeckoEnemy)Owner).TurningAround();
                }
            }
        }
    }
}
