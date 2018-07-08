using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    struct Collision
    {
        public enum CollisionType { None, RectsIntersection}
        public Vector2 Delta; //di quanto è entrato un rettangolo nell'altro
        public GameObject collider;

        public CollisionType Type;
    }
}
