using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class IdleAkuAkuState : AkuAkuState
    {
        public override void Update()
        {
            base.Update();

            totalTime += Game.DeltaTime;
    
            machine.Owner.Position = Vector2.Lerp(machine.Owner.Position, Player.Position + Offset, Game.DeltaTime * 3);
            machine.Owner.Position = new Vector2(machine.Owner.Position.X, machine.Owner.Position.Y + (float)Math.Sin(totalTime * 2) * 0.5f);
           
            if (Player.Velocity.Length > 0)
                machine.Switch((int)AkuState.Follow);
        }
    }
}
