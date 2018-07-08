using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class CameraTriggerLimits : TriggerCheckpointLoadable
    {
        Limits limits;
        bool drawBackground;

        public CameraTriggerLimits(Rect rect, Limits newLimits, bool drawBackground = true) : base(rect)
        {
            limits = newLimits;
            this.drawBackground = drawBackground;
        }

        public override void OnTriggerEnter(GameObject collider)
        {
            base.OnTriggerEnter(collider);

            if (collider is Player p && !p.IsDead)
            {
                CameraManager.SetLimits(limits);
            }

            BackgroundManager.DrawBackground = drawBackground;
        }

        public override void OnTriggerExit(GameObject collider)
        {
            base.OnTriggerExit(collider);

            if (!CameraManager.OutOfCameraViewPort(PlayScene.Player))
            {
                CameraManager.ResetLimits();

                if(!drawBackground)
                {
                    BackgroundManager.DrawBackground = true;
                }
            }
        }
    }
}
