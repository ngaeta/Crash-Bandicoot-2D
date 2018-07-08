using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class EnemyState : State
    {
        public Enemy Owner { get; private set; }

        public override void AssignStateMachine(StateMachine machine)
        {
            base.AssignStateMachine(machine);
            Owner = (Enemy) machine.Owner;
        }
    }
}
