using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class TriggerCheckpointLoadable : Trigger, ICheckpointLoadable
    {
        public TriggerCheckpointLoadable(Rect rect) : base(rect)
        {
        }

        Vector2 ICheckpointLoadable.Position => Rect.Position;

        protected virtual void OnCheckpointLoadable(CheckpointCrate checkpoint)
        {

        }

        void ICheckpointLoadable.OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            OnCheckpointLoadable(checkpoint);
        }
    }
}
