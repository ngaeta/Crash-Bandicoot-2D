using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class Groundable : GameObject
    {
        public bool IsGrounded
        {
            get { return !RigidBody.IsGravityAffected; }
            set { RigidBody.IsGravityAffected = !value; }
        }

        public bool UseGroundableGravity { get; set; }

        public Groundable(Vector2 spritePosition, string textureName, DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, textureName, drawLayer)
        {
            //IsGrounded = true;
            UseGroundableGravity = true;
        }

        public override void Update()
        {
            base.Update();

            if (IsActive && UseGroundableGravity)
            {
                IsGrounded = false;
            }
        }

        public override void OnCollide(Collision collisionInfo)
        {
            base.OnCollide(collisionInfo);

            if (collisionInfo.collider is IWalkable)
            {
                float deltaX = collisionInfo.Delta.X;
                float deltaY = collisionInfo.Delta.Y;

                if (deltaX < deltaY)
                {
                    if (Velocity.X != 0)
                    {
                        //collision from right or left
                        if (Position.X < collisionInfo.collider.X)
                        {
                            //from letf
                            deltaX = -deltaX;
                        }

                        Position = new Vector2(Position.X + deltaX, Position.Y);
                        //Velocity = new Vector2(0, Velocity.Y);    
                    }
                }
                else
                {
                    //collision from top or bottom
                    if (Position.Y < collisionInfo.collider.Y)
                    {
                        //from top
                        deltaY = -deltaY;

                        if (UseGroundableGravity && collisionInfo.collider is IWalkable)
                        {
                            if (!IsGrounded)
                            {
                                IsGrounded = true;
                            }
                        }
                    }

                    Position = new Vector2(Position.X, Position.Y + deltaY);
                    //Velocity = new Vector2(Velocity.X, 0);
                }
            }
        }
    }
}
