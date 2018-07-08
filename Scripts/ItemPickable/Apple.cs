using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class Apple : Pickable
    {
        public static NumerableGUIObject GuiObject;

        private AudioClip clipOnTarget;
        private Vector2 speed;
        private Vector2 initialPos;

        public Apple(Vector2 spritePosition, string spriteSheetName = "apple", DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(1.3f, 1.3f);
            speed = new Vector2(650f);
            Type = PickableType.Apple;

            clipOnTarget = AudioManager.GetAudioClip("bleep");
            ClipOnPicked = AudioManager.GetAudioClip("applePicked");

            initialPos = spritePosition;
        }

        public override void OnPlayerPick()
        {
            base.OnPlayerPick();
            Vector2 direction = (GuiObject.PositionInCamera - Position).Normalized();
            Velocity = new Vector2(direction.X * speed.X, direction.Y * speed.Y);
        }

        public override void Update()
        {
            if (!CameraManager.OutOfCameraViewPort(this))
                base.Update();

            if (isPicked)
            {
                Position += CameraManager.DeltaCamera;

                if ((Position - GuiObject.PositionInCamera).Length < 15f || CameraManager.OutOfCameraViewPort(this))
                {
                    isPicked = false;
                    player.Apples++;

                    if (clipOnTarget != null)
                    {
                        PlayAudio(clipOnTarget);
                    }

                    OnDie();
                }
            }
            else if (isHitted)
            {
                if (CameraManager.OutOfCameraViewPort(this))
                {
                    isHitted = false;
                    OnDie();
                }
            }
        }

        protected override void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            base.OnCheckpointLoad(checkpoint);

            Velocity = Vector2.Zero;
            Position = initialPos;
            IsActive = true;
        }
    }
}
