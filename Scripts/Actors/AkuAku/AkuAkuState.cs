using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    enum AkuState { Idle, Follow, Attack}

    abstract class AkuAkuState : State
    {
        public static Player Player;
        public static Vector2 Offset;
        protected float totalTime = 0;
    }
}
