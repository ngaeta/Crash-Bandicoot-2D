using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class PlantEnemy : Enemy
    {
        public enum AnimationType { Idle, Alert, Attack }

        public PlantEnemy(Vector2 spritePosition, string spriteSheetName = "plant", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(1.2f, 1.5f);
            Rect collider = new Rect(new Vector2(-4, -15), RigidBody, Width / 2 + 4, Height - 20);
            Rect colliderAttack = new Rect(new Vector2(20, 0), RigidBody, Width / 2, Height / 3);
            RigidBody.SetBoundingBox(collider);

            Animation = animations[(int)AnimationType.Idle];
            animations[(int)AnimationType.Alert].LoopAtFrame(2);
        }

        //public override void OnAlertStateEnter()
        //{
        //    base.OnAlertStateEnter();
        //    StartAnim(AnimationType.Alert);
        //}

        //public override void OnPatrolStateEnter()
        //{
        //    base.OnPatrolStateEnter();
        //    StartAnim(AnimationType.Idle);
        //}

        //protected void StartAnim(AnimationType anim)
        //{
        //    Animation = animations[(int)anim];
        //    Animation.Reset();
        //}
        //}
    }
}
