using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class AttackAkuAkuState : AkuAkuState
    {
        private Vector2 initialScale;
        private float timeBlink;
        private Vector4 colorAdditive = new Vector4(0.7f, 0.5f, 0.2f, 0f); //piu o meno dorato
        private float currTimeInvulnerability;

        public override void Enter()
        {
            base.Enter();
            initialScale = machine.Owner.GetSprite().scale;
            machine.Owner.GetSprite().scale = Vector2.One;
            timeBlink = 0;
            machine.Owner.GetSprite().SetAdditiveTint(colorAdditive);
            currTimeInvulnerability = AkuAku.TIME_INVULNERABILITY;
        }

        public override void Update()
        {
            base.Update();

            if (currTimeInvulnerability > 0)
            {
                currTimeInvulnerability -= Game.DeltaTime;

                machine.Owner.Position = Player.HeadPosition;
                machine.Owner.GetSprite().FlipX = Player.GetSprite().FlipX;

                if (Player.CurrentState == Player.State.CrouchMoving)
                {
                    machine.Owner.GetSprite().Rotation = !machine.Owner.GetSprite().FlipX ? MathHelper.PiOver3 : -MathHelper.PiOver3;
                }
                else
                    machine.Owner.GetSprite().Rotation = 0;

                timeBlink += Game.DeltaTime * 40;
                float multiply = (float)Math.Cos(timeBlink) + 0.3f;
                machine.Owner.GetSprite().SetMultiplyTint(new Vector4(multiply, multiply, multiply, 1));
            }
            else
                ((AkuAku)machine.Owner).EndInvulnerability();
        }

        public override void Exit()
        {
            base.Exit();
            machine.Owner.GetSprite().scale = initialScale;
            machine.Owner.GetSprite().SetAdditiveTint(Vector4.Zero);
            machine.Owner.GetSprite().SetMultiplyTint(Vector4.One);
            machine.Owner.GetSprite().Rotation = 0;
        }
    }
}
