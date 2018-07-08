using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class Bar:GameObject
    {
        protected float value;
        protected float barWidth;
        protected Sprite frame;
        protected Texture frameTexture;

        public float MaxValue { get; set; }

        public Vector2 BarOffset
        {
            set
            {
                sprite.position = frame.position + value;
            }
        }

        public virtual void SetValue(float newValue)
        {
            value = newValue;
            ResizeBar();
        }

        public Bar(Vector2 position, string textureName= "playerBar", float maxValue=100, int height = 0) : base(position,textureName,DrawManager.Layer.GUI)
        {
            barWidth = texture.Width;
            value = MaxValue = maxValue;
            //sprite.scale = new Vector2(texture.Width, 1);

            frameTexture = GfxManager.GetSpritesheet("barFrame").Item1;
            frame = new Sprite(frameTexture.Width, frameTexture.Height);
            frame.position = position;

        }

        protected virtual void ResizeBar()
        {
            float scale = value / MaxValue;
            barWidth = texture.Width * scale;
            sprite.scale = new Vector2(scale, 1);
            //sprite.SetMultiplyTint((1-scale)*1f,scale*0.6f,scale*0.95f, 1);
            sprite.SetAdditiveTint(1-scale, scale-1,  scale-1, 0);
        }

        public override void Draw()
        {
            if (IsActive)
            {
                frame.DrawTexture(frameTexture);
                sprite.DrawTexture(texture, 0, 0, (int)barWidth, (int)sprite.Height);
            }
        }

        public void SetXPosition(float newX)
        {
            frame.position.X = newX;

            float xOffSet = newX-frame.position.X;
            sprite.position.X += xOffSet;
        }
    }
}
