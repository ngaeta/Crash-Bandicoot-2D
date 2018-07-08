using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class TriggerActiveAction : TriggerCheckpointLoadable
    {
        ITriggableAction objectToActiveAction;
        bool isActiveTrigger;

        public TriggerActiveAction(Rect rect, ITriggableAction objectToActiveAction) : base(rect)
        {
            this.objectToActiveAction = objectToActiveAction;
        }

        public override void OnTriggerEnter(GameObject collider)
        {
            if (!isActiveTrigger)
            {
                base.OnTriggerEnter(collider);
                isActiveTrigger = true;
                objectToActiveAction.ActiveAction();
            }           
        }

        protected override void OnCheckpointLoadable(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoadable(checkpoint);

            if (checkpoint == null)
            {
                isActiveTrigger = false;
                return;
            }

            float distanceCheckpoint = checkpoint.Position.X - Rect.Position.X;

            if(Math.Abs(distanceCheckpoint) < Game.Window.Width || checkpoint.Position.X < Rect.Position.X)
            {
                isActiveTrigger = false;
            }
        }
    }
}
