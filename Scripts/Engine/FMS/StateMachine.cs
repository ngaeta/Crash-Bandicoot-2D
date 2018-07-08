using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class StateMachine
    {
        Dictionary<int, State> states;
        protected State currentState;
       
        private GameObject owner;
        public GameObject Owner { get { return owner; }}

        public StateMachine(GameObject owner)
        {
            states = new Dictionary<int, State>();
            this.owner = owner;
        }

        public void RegisterState(int id, State state)
        {
            states.Add(id, state);
            state.AssignStateMachine(this);
        }

        public void ReplaceState(int id, State state)
        {
            if(states.ContainsKey(id))
            {
                states.Remove(id);
                RegisterState(id, state);
            }
        }

        public void Switch(int id)
        {
            if(currentState != null)
            {
                currentState.Exit();
            }

            currentState = states[id];
            currentState.Enter();
        }

        public virtual void Input()
        {
            if (currentState != null)
            {
                currentState.Input();
            }
        }

        public void Run()
        {
            if(currentState != null)
            {
                currentState.Update();
            }
        }
    }
}
