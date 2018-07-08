using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    static class PhysicsManager
    {
        public enum ColliderType : uint { Player = 1, Enemy = 2, Pickable = 4, Crate = 8, Ground = 16, Explosion = 32, Tile = 64, Water = 128, Bullet = 256, Trap = 512 }

        static List<RigidBody> items;
        static Collision collisionInfo;
        public static bool RayDebug;

        static GameObject ray;

        public static void Init()
        {
            items = new List<RigidBody>();
            collisionInfo = new Collision();

            if (RayDebug)
            {
                ray = new GameObject(Vector2.Zero, "ray", DrawManager.Layer.GUI);
                ray.GetSprite().scale = new Vector2(0.2f, 0.2f);
            }
        }

        public static void AddItem(RigidBody item)
        {
            items.Add(item);
        }

        public static void RemoveItem(RigidBody item)
        {
            items.Remove(item);
        }

        public static void RemoveAll()
        {
            items.Clear();
            collisionInfo = new Collision();
        }

        public static void Update()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].GameObject.IsActive)
                    items[i].Update();
            }
        }

        public static void CheckCollisions()
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                if (CameraManager.OutOfCameraViewPort(items[i].GameObject) == false || items[i].GameObject is Player)
                {
                    if (items[i].GameObject.IsActive && items[i].IsCollisionsAffected)
                    {
                        for (int j = i + 1; j < items.Count; j++)
                        {
                            if ((items[i].Position - items[j].Position).Length < Game.Window.Width / 2)
                            {
                                if (items[j].GameObject.IsActive && items[j].IsCollisionsAffected)
                                {
                                    bool checkFirst = items[i].CheckCollisionWith(items[j]);
                                    bool checkSecond = items[j].CheckCollisionWith(items[i]);

                                    if ((checkFirst || checkSecond) && items[i].Collides(items[j], ref collisionInfo))
                                    {
                                        if (checkFirst)
                                        {
                                            collisionInfo.collider = items[j].GameObject;
                                            items[i].GameObject.OnCollide(collisionInfo);
                                        }

                                        if (checkSecond)
                                        {
                                            collisionInfo.collider = items[i].GameObject;
                                            items[j].GameObject.OnCollide(collisionInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static Tuple<RigidBody, float> RayCast(Vector2 origin, Vector2 direction, RigidBody mySelf, float lenght = float.MaxValue, List<ColliderType> ignoreMask = null)
        {
            //controllare anche la maschera???
            RigidBody nearestCollider = null;
            float nearestPoint = float.MaxValue;

            foreach (RigidBody collider in items)
            {
                if (collider == mySelf  || !collider.GameObject.IsActive)
                    continue;

                if (ignoreMask != null && ignoreMask.Contains((ColliderType)collider.Type))
                {
                    // Console.WriteLine(collider.GameObject + " non calcolato");
                    continue;
                }

                // tca
                Vector2 l = collider.Position - origin;

                if (lenght != int.MaxValue && Math.Abs(l.Length) > lenght)
                {
                    continue;
                }

                float tca = Vector2.Dot(l, direction);

                if (RayDebug)
                {
                    ray.Position = l + origin;
                    ray.GetSprite().Rotation = tca;
                }

                if (tca < 0)
                    continue;

                float d = (float)Math.Sqrt(l.Length * l.Length - tca * tca);
                if (d > collider.BoundingCircle.Ray)
                    continue;

                float thc = (float)Math.Sqrt(collider.BoundingCircle.Ray * collider.BoundingCircle.Ray - d * d);

                float p = tca - thc;
                float p1 = tca + thc;

                if (p < nearestPoint || p1 < nearestPoint)
                {
                    nearestCollider = collider;
                    nearestPoint = Math.Min(p, p1);
                }

                //Vector2 pCoords = origin + direction * p;
                //Vector2 p1Coords = origin + direction * p1;
            }
            return new Tuple<RigidBody, float>(nearestCollider, nearestPoint);
        }

    }
}
