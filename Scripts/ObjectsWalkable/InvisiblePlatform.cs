using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class InvisiblePlatform : Ground
    {
        enum State { Disappeared, Appeared }

        private State currState;

        private float currCountAlpha;
        private float currCountDisappear;

        private bool itDisappearAndAppear;
        private float timeToDisappear;
        private float timeToAppear;

        public bool ItDisappearAndAppear
        {
            get
            {
                return itDisappearAndAppear;
            }
            set
            {
                itDisappearAndAppear = value;
                TimeToDisappear = TimeToAppear = 2f;
                currState = State.Disappeared;
                currCountDisappear = timeToDisappear;
            }
        }

        public float TimeToDisappear
        {
            get { return timeToDisappear; }
            set
            {
                timeToDisappear = value;
                if (value == 0)
                {
                    itDisappearAndAppear = false;
                }
            }
        }

        public float TimeToAppear
        {
            get { return timeToAppear; }
            set
            {
                timeToAppear = value;
                if (value == 0)
                {
                    itDisappearAndAppear = false;
                }
            }
        }

        public InvisiblePlatform(Vector2 spritePosition, string spriteSheetName = "platform", DrawManager.Layer layer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, true, layer)
        {
            itDisappearAndAppear = false;
            clipFootStep = AudioManager.GetAudioClip("metal2FootStep");
        }

        public override void Update()
        {
            base.Update();

            if (itDisappearAndAppear)
            {
                if (currCountDisappear <= 0)
                {
                    currCountAlpha += Game.DeltaTime;
                    float multiply = currState == State.Disappeared ? (float)Math.Cos(currCountAlpha) : (float)Math.Sin(currCountAlpha);

                    if (currCountAlpha >= MathHelper.PiOver2)
                    {
                        multiply = currState == State.Disappeared ? 0 : 1;
                        CanWalkable = (currState == State.Appeared);
                        currState = currState == State.Disappeared ? State.Appeared : State.Disappeared;
                        currCountDisappear = currState == State.Appeared ? TimeToDisappear : TimeToAppear;

                        currCountAlpha = 0;
                    }

                    sprite.SetMultiplyTint(new Vector4(multiply, multiply, multiply, multiply));
                }
                else
                    currCountDisappear -= Game.DeltaTime;
            }
        }
    }
}
