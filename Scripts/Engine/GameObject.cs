using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class GameObject:IUpdatable,IDrawable, IActivable, IDestroyable
    {
        protected Sprite sprite;
        protected Texture texture;
        protected DrawManager.Layer layer;

        public virtual Vector2 Velocity {
            get { return (RigidBody != null ? RigidBody.Velocity : Vector2.Zero); }
            set { if (RigidBody != null) RigidBody.Velocity = value; }
        }

        public Vector2 Forward { get { return new Vector2((float)Math.Cos(sprite.Rotation), (float)Math.Sin(sprite.Rotation)).Normalized(); } }
        public RigidBody RigidBody { get; set; }  //protected set, aggiustare
        public Animation Animation { get; protected set; }
        protected List<Animation> animations;
        public int numAnim = 0;
        public int Width { get { return (int)(sprite.Width * sprite.scale.X); } }
        public int Height { get { return (int)(sprite.Height * sprite.scale.Y); } }
        public Vector2 Position { get { return RigidBody != null ? RigidBody.Position : sprite.position; }
            set { sprite.position = value;
                if(RigidBody != null)
                    RigidBody.Position = value;
            }
        }
        public float X { get { return sprite.position.X; } set { sprite.position.X = value; } }
        public float Y { get { return sprite.position.Y; } set { sprite.position.Y = value; } }

        public DrawManager.Layer Layer { get { return layer; } }

        public virtual bool IsActive { get; set; }

        public GameObject()
        {
            //position.X = 0;
            //position.Y = 0;
        }

        public GameObject(
            Vector2 spritePosition, string spriteSheetName, 
            DrawManager.Layer drawLayer = DrawManager.Layer.Playground)
        {
            Tuple<Texture, List<Animation>> ss = GfxManager.GetSpritesheet(spriteSheetName);
            texture = ss.Item1;
            animations = ss.Item2;
            Animation = animations[0];
            sprite = new Sprite(Animation.FrameWidth, Animation.FrameHeight);
            //texture = GfxManager.GetTexture(textureName);
            //sprite = new Sprite(spriteWidth>0 ? spriteWidth : texture.Width, spriteHeight>0 ? spriteHeight : texture.Height);
            sprite.position = spritePosition;
            sprite.pivot = new Vector2(Width / 2, Height / 2);
            //Animation = new Animation((int)sprite.Width, (int)sprite.Height, cols, rows, fps, loop);
            layer = drawLayer;
            IsActive = true;
            UpdateManager.AddItem(this);
            DrawManager.AddItem(this);
            GameManager.Destoyables.Add(this);
        }


        public GameObject(Sprite spriteRef)
        {
            sprite = spriteRef;
        }

        public void Translate(float deltaX, float deltaY)
        {
            sprite.position.X += deltaX;
            sprite.position.Y += deltaY;
        }

        public void SetPivot(Vector2 pos)
        {
            sprite.pivot = pos;
        }

        public void SetSprite(Sprite newSprite)
        {
            sprite = newSprite;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        public virtual void Draw()
        {
            if (IsActive)
            {
                sprite.DrawTexture(texture,
                    Animation.OffsetX, Animation.OffsetY,
                    Animation.FrameWidth, Animation.FrameHeight);
            }
        }

        public virtual void Update()
        {
            if (IsActive)
            {
                if(RigidBody !=null)
                {
                    sprite.position = RigidBody.Position;
                }
                if (Animation.IsActive)
                {
                    Animation.Update();
                }

                //CheckCamera();
            }
        }

        public void AddScale(Vector2 scaleToAdd)
        {
            sprite.scale += scaleToAdd;
        }

        public virtual void OnCollide(Collision collision)
        {

        }

        public virtual void OnTriggerEnter(Collision collision)
        {

        }

        public virtual void OnTriggerExit(Collision collision)
        {

        }

        public void Destroy()
        {
            OnDestroy();
        }

        public virtual void OnDestroy()
        {
            //togliamo tutti i riferimenti senno la garbage non li cancella
            UpdateManager.RemoveItem(this);
            DrawManager.RemoveItem(this);

            if (RigidBody != null)
            {
                PhysicsManager.RemoveItem(RigidBody);
                RigidBody.Destroy();
            }
        }
    }
}
