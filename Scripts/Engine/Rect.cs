using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class Rect : IDrawable, IUpdatable, IDestroyable
    {
        protected Vector2 relativePosition;

        public Vector2 RelativePosition { get { return relativePosition; } set { relativePosition = value; } }
        public Vector2 Position {
            get
            {
                if (RigidBody != null)
                    return RigidBody.Position + relativePosition;
                else
                    return relativePosition;
            }
        }
        public RigidBody RigidBody { get; set; }

        public float HalfWidth { get; protected set; }
        public float HalfHeight { get; protected set; }
        public static bool Debug {get; set; }
        public Sprite rectCollider;
        public Texture texture;

        public bool IsTrigger { get; set; }
        public bool IsActive { get; set; }

        public DrawManager.Layer Layer { get; set; }

        public Rect(Vector2 offset, RigidBody owner, float width, float height)
        {
            relativePosition = offset;
            RigidBody = owner;
            HalfWidth = width / 2;
            HalfHeight = height / 2;

            if(Debug)
            {
                IsActive = true;
                rectCollider = new Sprite(width, height);
                texture = GfxManager.GetSpritesheet("rectangle").Item1;
                Layer = DrawManager.Layer.Foreground;
                DrawManager.AddItem(this);
                UpdateManager.AddItem(this);
            }
        }

        public bool Collides(Rect rect, ref Collision collisionInfo)
        {
            Vector2 distance = rect.Position - Position;
            
            float deltaX = Math.Abs(distance.X) - (HalfWidth + rect.HalfWidth);
            float deltaY = Math.Abs(distance.Y) - (HalfHeight + rect.HalfHeight);
            bool collides = (deltaX <= 0 && deltaY <= 0);
            
            if(collides)
            {
                collisionInfo.collider = rect.RigidBody.GameObject;
                collisionInfo.Type = Collision.CollisionType.RectsIntersection;
                collisionInfo.Delta = new Vector2(-deltaX, -deltaY);
            }

            return collides;
        }


        public bool Collides(Circle circle)
        {
            bool collision = false;

            float left = Position.X - HalfWidth;
            float right = Position.X + HalfWidth;
            float top = Position.Y - HalfHeight;
            float bottom = Position.Y + HalfHeight;

            //searching for the nearest point to the circle center
            float nearestX = Math.Max(left, Math.Min(circle.Position.X, right));
            float nearestY = Math.Max(top, Math.Min(circle.Position.Y, bottom));

            //check collision

            float deltaX = circle.Position.X - nearestX;
            float deltaY = circle.Position.Y - nearestY;

            return ((deltaX * deltaX + deltaY * deltaY) <= circle.Ray * circle.Ray);
        }

        public void Draw()
        {
            if(Debug && IsActive && RigidBody != null)
            {
                rectCollider.DrawTexture(texture);
            }
        }

        public void Update()
        {
            if (Debug && IsActive)
            {
                if (RigidBody != null)
                    rectCollider.position = new Vector2(Position.X - HalfWidth, Position.Y - HalfHeight);
                else
                {
                    UpdateManager.RemoveItem(this);
                    DrawManager.RemoveItem(this);
                }
            }
        }

        public void Destroy()
        {
            RigidBody = null;
        }
    }
}
