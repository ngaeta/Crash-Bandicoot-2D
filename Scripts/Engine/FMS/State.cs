using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    public enum States { Idle, Chase, Attack }

    abstract class State
    {
        protected StateMachine machine;

        public virtual void Enter()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Exit()
        {

        }

        public virtual void Input()
        {

        }

        public virtual void AssignStateMachine(StateMachine machine)
        {
            this.machine = machine;
        }
    }
}
