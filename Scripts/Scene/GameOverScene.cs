using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class GameOverScene : FadeScene
    {
        private GameObject background;
        private UkaUka ukaUka;
        private float ukaUkaTalkCount;
        private Word gameOverWord;
        private float nextInputCount;

        public GameOverScene(float showTime = 3, float fadeTime = 3) : base(showTime, fadeTime)
        {
            FadeOut = false;
        }

        public override void Start()
        {
            base.Start();

            background = new GameObject(new Vector2(Game.Window.Width / 2, Game.Window.Height / 2), "gameOverBackground", DrawManager.Layer.Background);
            background.GetSprite().scale = new Vector2(1f, 0.9f);
            ukaUka = new UkaUka(new Vector2(Game.Window.Width / 2, Game.Window.Height / 2));
            ukaUkaTalkCount = 1f;

            gameOverWord = LetterManager.GetWord("Game Over", new Vector2(Game.Window.Width / 2 - 220, 100f), 35f);
            gameOverWord.SetScale(new Vector2(4f));
        }

        public override void Update()
        {
            ukaUka.Update();

            if (ukaUka.CurrAnim == UkaUka.AnimationType.Show)
            {
                if (!ukaUka.Animation.IsPlaying)
                {
                    if (ukaUkaTalkCount <= 0)
                    {
                        ukaUka.Talk();
                    }
                    else
                        ukaUkaTalkCount -= Game.DeltaTime;
                }
            }
            else if (ukaUka.CurrAnim == UkaUka.AnimationType.Idle)
            {
                if (ukaUkaTalkCount > 1.5f)
                    Game.CurrScene.IsPlaying = false;
                else
                    ukaUkaTalkCount += Game.DeltaTime;
            }
        }

        public override void Draw()
        {
            background.Draw();
            ukaUka.Draw();
            gameOverWord.Draw();
        }
    }
}
