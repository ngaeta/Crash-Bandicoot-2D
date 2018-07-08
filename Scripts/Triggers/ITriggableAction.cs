using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    public enum TriggerType
    {
        Trigger_Camera,
        Trigger_Action,
        Trigger_Coco
    }

    interface ITriggableAction
    {
        void ActiveAction();
    }
}
