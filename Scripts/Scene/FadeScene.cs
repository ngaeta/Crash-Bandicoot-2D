using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class FadeScene : Scene
    {
        public enum Stats { FadeIn, FadeOut, Show, End}

        public bool FadeIn
        {
            get { return fadeIn; }
            set
            {
                fadeIn = value;
                if (fadeIn)
                    statsScene = Stats.FadeIn;
                else
                    statsScene = Stats.Show;
            }
        }

        public bool FadeOut { get; set; }

        private bool fadeIn;
        private float showTime;
        protected float fadeTime;
        private float colorMul;
        protected bool buttonPressed;
        protected Stats statsScene;
        protected float currTime;

        protected List<GameObject> gameObjects;

        public FadeScene(float showTime = 3, float fadeTime = 3)
        {
            FadeIn = true;
            FadeOut = true;

            this.showTime = showTime;
            this.fadeTime = fadeTime;

            statsScene = Stats.FadeIn;
            buttonPressed = false;
        }

        public override void Start()
        {
            base.Start();
            gameObjects = new List<GameObject>();
        }

        public override void Update()
        {
            if (statsScene == Stats.FadeIn)
            {
                if (currTime <= fadeTime)
                {
                    currTime += Game.DeltaTime;
                    colorMul = currTime / fadeTime;
                    FadeSprites();
                }
                else
                {
                    statsScene = Stats.Show;
                    currTime = 0;
                }
            }
            else if (statsScene == Stats.Show)
            {
                if (currTime >= showTime)
                {
                    if (FadeOut)
                    {
                        statsScene = Stats.FadeOut;
                        currTime = fadeTime;
                    }
                    else
                        statsScene = Stats.End;
                }
                else
                    currTime += Game.DeltaTime;
            }
            else if (statsScene == Stats.FadeOut)
            {
                if (currTime >= 0)
                {
                    currTime -= Game.DeltaTime;
                    colorMul = currTime / fadeTime;
                    FadeSprites();
                }
                else
                    statsScene = Stats.End;
            }

            else if (statsScene == Stats.End)
                IsPlaying = false;
        }

        public override void Input()
        {
            if(InputManager.GetButton(Button.Enter) && !buttonPressed)
            {
                buttonPressed = true;
                if (FadeOut && statsScene != Stats.FadeOut)
                {
                    statsScene = Stats.FadeOut;
                    currTime = fadeTime;
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                    statsScene = Stats.End;
                }
            }
        }

        public override void Draw()
        {
            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Draw();
        }

        public override void OnExit()
        {
            base.OnExit();
            gameObjects.Clear();

            DrawManager.RemoveAll();
            UpdateManager.RemoveAll();
        }

        public void AddGameObjects(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        private void FadeSprites()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].GetSprite().SetMultiplyTint(colorMul, colorMul, colorMul, 1);
            }
        }
    }
}
