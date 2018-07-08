using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class ExtraLife : Pickable
    {
        public static NumerableGUIObject GuiObject;
        const float TIME_BEFORE_MOVE_GUI = 1f;
        const float TIME_TO_TARGET = 60f;

        private float totalTime;
        private float timeToMoveGui;
        private Vector2 initialPos;
        private Vector2 initialScale;


        public ExtraLife(Vector2 spritePosition, string spriteSheetName = "extraLife", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            Type = PickableType.ExtraLife;
            ClipOnPicked = AudioManager.GetAudioClip("extraLifePicked");

            initialPos = spritePosition;
            initialScale = sprite.scale;
        }

        public override void OnPlayerPick()
        {
            base.OnPlayerPick();

            Position = GuiObject.PositionInCamera - new Vector2(110f, 0);
            sprite.scale = GuiObject.GetSprite().scale;
            totalTime = 1f;
        }

        public override void Update()
        {
            if(isHitted)
            {
                base.Update();

                if (CameraManager.OutOfCameraViewPort(this))
                {
                    isHitted = false;
                    OnDie();
                }
            }

            else if (!isPicked)
            {
                totalTime += Game.DeltaTime * 2;
                sprite.position.Y += (float)Math.Cos(totalTime) * 0.05f;
            }

            else if (isPicked)
            {
                Position += CameraManager.DeltaCamera;

                if (GuiObject.PositionInCamera.X - Position.X <= 0)
                {
                    isPicked = false;
                    player.Life++;
                    OnDie();
                }
                else
                {
                    totalTime += Game.DeltaTime * 20;
                    float multiply = (float)Math.Cos(totalTime);
                    sprite.SetMultiplyTint(new Vector4(multiply, multiply, multiply, multiply));

                    if (timeToMoveGui <= 0)
                    {
                        Position = new Vector2(Position.X + (TIME_TO_TARGET * Game.DeltaTime), Position.Y);
                    }
                    else
                        timeToMoveGui -= Game.DeltaTime;
                }
            }
        }

        public override void OnDie()
        {
            base.OnDie();
            totalTime = 0;
            timeToMoveGui = TIME_BEFORE_MOVE_GUI;
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);


                Position = initialPos;
                Velocity = Vector2.Zero;

                IsActive = true;
                isHitted = false;

                totalTime = 0;
                timeToMoveGui = TIME_BEFORE_MOVE_GUI;

                sprite.SetMultiplyTint(Vector4.One);
                sprite.scale = initialScale;
            
        }
    }
}
