using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    abstract class Trigger 
    {
        public Rect Rect;

        public Trigger(Rect rect)
        {
            Rect = rect;
            TriggerManager.AddTrigger(this);
        }

        public virtual void OnTriggerEnter(GameObject collider)
        {
           
        }

        public virtual void OnTriggerExit(GameObject collider)
        {
          
        }
    }
}
