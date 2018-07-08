using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class BounceBehaviour : IUpdatable  //verrà utilizzata da classi Crate che già estendono altre classi, ma hanno lo stesso update
    {
        private float timeTotal;
        private bool bounce;
        private float bounceMultiplierY;
        private Sprite sprite;

        public BounceBehaviour(GameObject owner, float bounceMultiplier = 1.7f)
        {
            bounceMultiplierY = bounceMultiplier;
            bounce = false;
            sprite = owner.GetSprite();
            UpdateManager.AddItem(this);

            //owner.RigidBody.BoundingBox.RelativePosition = new Vector2(0, -22f);
            //owner.SetPivot(new Vector2(sprite.pivot.X, owner.Height / sprite.scale.Y));
        }

        public void Update()
        {
            if (sprite != null)
            {
                if (bounce)
                {
                    timeTotal += Game.DeltaTime;

                    float scaleY = (float)(Math.Sin(timeTotal * 18f) + 0.01f);
                    sprite.scale.Y = 1.5f - scaleY;

                    if (sprite.scale.Y >= 2.2f)
                    {
                        sprite.scale.Y = 1.5f;
                        bounce = false;
                        timeTotal = 0f;
                    }
                }
            }
        }

        public void Bounce(Player player)
        {
            bounce = true;
            player.Jump(bounceMultiplierY);
        }

        public void Attach(GameObject obj)
        {
            sprite = obj.GetSprite();
            bounce = false;
        }

        public void Detach()
        {
            if (sprite != null)
                sprite = null;
        }
    }
}
