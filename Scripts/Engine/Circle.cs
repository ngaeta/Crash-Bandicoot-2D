using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class Circle : IDrawable, IUpdatable
    {
        protected Vector2 relativePosition;

        public Vector2 Position { get { return RigidBody.Position + relativePosition; }}
        public RigidBody RigidBody { get; set; }
        public float Ray { get; protected set; }

        public static bool Debug { get; set; }
        public DrawManager.Layer Layer { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrigger { get; set; }

        private Sprite circleCollider;
        private Texture texture;

        public Circle(Vector2 offset, RigidBody owner, float ray)
        {
            relativePosition = offset;
            RigidBody = owner;
            Ray = ray;

            if (Debug)
            {
                IsActive = true;
                circleCollider = new Sprite(ray * 2, ray * 2);
                texture = GfxManager.GetSpritesheet("circle").Item1;
                Layer = DrawManager.Layer.Foreground;
                DrawManager.AddItem(this);
                UpdateManager.AddItem(this);
            }
        }

        public bool Contains(Vector2 point)
        {
            Vector2 dist = point - Position;

            return dist.Length <= Ray;
        }

        public bool Collides(Circle circle)
        {
            Vector2 dist = circle.Position - Position;
            return dist.Length <= Ray + circle.Ray;
        }

        public void Draw()
        {
            if (Debug && IsActive)
            {
                circleCollider.DrawTexture(texture);
            }
        }

        public void Update()
        {
            if (Debug && IsActive)
                circleCollider.position = new Vector2(Position.X - Ray, Position.Y - Ray);
        }

        public void Destroy()
        {
            RigidBody = null;
        }
    }
}
