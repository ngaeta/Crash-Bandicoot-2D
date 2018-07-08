using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class RigidBody : IUpdatable
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Circle BoundingCircle { get; protected set; }
        public Rect BoundingBox { get; protected set; }
        public GameObject GameObject { get; protected set; }
        public bool IsGravityAffected { get; set; }
        public bool IsCollisionsAffected { get; set; }
        public uint Type { get; set; }
        public uint CollisionMask { get; protected set; }



        public RigidBody(Vector2 position, GameObject owner, Circle boundingCircle = null, Rect boundingBox = null, bool createBoundingBox = true)
        {
            Position = position;
            GameObject = owner;

            IsCollisionsAffected = true;

            if (boundingCircle == null)
            {
                //need to create a circle
                float ray = (float)(Math.Sqrt(GameObject.Width * GameObject.Width + GameObject.Height * GameObject.Height) / 2);
                BoundingCircle = new Circle(Vector2.Zero, this, ray);
            }
            else
            {
                BoundingCircle = boundingCircle;
                BoundingCircle.RigidBody = this;
            }

            if (boundingBox == null)
            {
                if (createBoundingBox)
                {
                    //need to create a boundingBox
                    BoundingBox = new Rect(Vector2.Zero, this, GameObject.Width, GameObject.Height);
                }
            }
            else
            {
                BoundingBox = boundingBox;
                BoundingBox.RigidBody = this;
            }

            PhysicsManager.AddItem(this);
        }

        public void AddVelocity(float vX, float vY)
        {
            Velocity += new Vector2(vX, vY);
        }

        public void SetXVelocity(float vX)
        {
            Velocity = new Vector2(vX, Velocity.Y);
        }

        public void SetYVelocity(float vY)
        {
            Velocity = new Vector2(Velocity.X, vY);
        }

        public void SetBoundingBox(Rect rect)
        {
            BoundingBox.RigidBody = null;
            BoundingBox = rect;
            BoundingBox.RigidBody = this;
        }

        public bool Collides(RigidBody other, ref Collision collisionInfo)
        {
            //circle vs circle collision
            if (BoundingCircle.Collides(other.BoundingCircle))
            {
                if (BoundingBox != null && other.BoundingBox != null)
                {
                    //rect vs rect
                    return BoundingBox.Collides(other.BoundingBox, ref collisionInfo);
                }
                else
                {
                    if (BoundingBox != null)
                    {
                        //rect vs circle
                        return BoundingBox.Collides(other.BoundingCircle);
                    }
                    else if (other.BoundingBox != null)
                    {
                        //other rect vs my circle
                        return other.BoundingBox.Collides(BoundingCircle);
                    }
                    else
                    {
                        //none has rect
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckCollisionWith(RigidBody rb)
        {
            return (CollisionMask & rb.Type) != 0;
        }

        public void SetCollisionMask(uint mask)
        {
            CollisionMask = mask;
        }

        public void AddCollision(uint mask)
        {
            CollisionMask |= mask;
        }

        public void Update()
        {
            if (IsGravityAffected)
            {
                AddVelocity(0, Game.Gravity * Game.DeltaTime);
            }

            Position += Velocity * Game.DeltaTime;
        }

        public void Destroy()
        {
            GameObject = null;

            if (BoundingCircle != null)
                BoundingCircle.Destroy();

            if (BoundingBox != null)
                BoundingBox.Destroy();

            BoundingBox = null;
            BoundingCircle = null;
        }
    }
}
