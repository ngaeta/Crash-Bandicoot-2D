using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    interface IHittable
    {
        void OnHit(GameObject objectHit);
    }
}
