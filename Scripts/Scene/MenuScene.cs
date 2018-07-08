using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class MenuScene : FadeScene
    {
        enum WordSelected {Play, Exit }

        private float nextInputCount;

        private Word playDemoWord;
        private Word exitWord;

        private Word informationWords;

        private AudioSource audioSourceBackground;
        private AudioSource audioSourceSelection;

        private AudioClip clipSelector;
        private AudioClip clipMenu;

        private GameObject selector;
        private WordSelected wordSelected;

        public MenuScene(float showTime = 1f, float fadeTime = 1.5f) : base(showTime, fadeTime)
        {
            FadeOut = false;
        }

        public override void Start()
        {
            base.Start();

            GameObject background = new GameObject(new Vector2(Game.Window.Width / 2, Game.Window.Height + 60), "background_forest");
            GameObject board = new GameObject(new Vector2(Game.Window.Width / 2, 230f), "board");
            GameObject crashLabel = new GameObject(board.Position + new Vector2(0, 70f), "crashLabel");
            crashLabel.GetSprite().scale = new Vector2(3.5f);

            GameObject arrow1 = new GameObject(board.Position + new Vector2(-board.Width / 5.5f, 30f), "arrowMenu1");
            arrow1.GetSprite().scale = new Vector2(4f);
            GameObject arrow2 = new GameObject(board.Position + new Vector2(board.Width / 6.5f, 0f), "arrowMenu2");
            arrow2.GetSprite().scale = new Vector2(4f);
            arrow2.GetSprite().Rotation -= MathHelper.DegreesToRadians(15f);

            playDemoWord = LetterManager.GetWord("PLAY DEMO", board.Position + new Vector2(-board.Width / 10, board.Height / 1.5f), 3f);
            exitWord = LetterManager.GetWord("EXIT", playDemoWord.Position + new Vector2(0, playDemoWord.Height * 2));
            informationWords = LetterManager.GetWord("Demo developed by Gaeta Nicola",
                new Vector2(Game.Window.Width / 4, Game.Window.Height - 20f));

            wordSelected = WordSelected.Play;

            selector = LetterManager.GetSelector(playDemoWord.Position);
            selector.GetSprite().FlipX = true;
            SetSelectorPosition();

            gameObjects.Add(background);
            gameObjects.Add(arrow1);
            gameObjects.Add(arrow2);
            gameObjects.Add(board);
            gameObjects.Add(crashLabel);

            audioSourceBackground = new AudioSource();
            audioSourceSelection = new AudioSource();

            clipMenu = new AudioClip("Assets/Audio/BackgroundTheme.ogg");
            clipSelector = new AudioClip("Assets/Audio/Bleep.wav");
        }

        public override void Input()
        {
            if (statsScene == Stats.Show)
            {
                if (nextInputCount <= 0 && !buttonPressed)
                {
                    if (InputManager.GetButton(Button.Down))
                    {
                        if ((int)wordSelected < 1)
                        {
                            wordSelected++;
                            SetSelectorPosition();
                            nextInputCount = 0.3f;
                        }
                    }
                    else if (InputManager.GetButton(Button.Up))
                    {
                        if (wordSelected > 0)
                        {
                            wordSelected--;
                            SetSelectorPosition();
                            nextInputCount = 0.3f;
                        }
                    }

                    if (InputManager.GetButton(Button.Enter))
                    {
                        buttonPressed = true;
                        audioSourceSelection.Play(clipSelector);

                        if (wordSelected == WordSelected.Play)
                        {
                            Game.CurrScene.IsPlaying = false;
                        }
                        else if (wordSelected == WordSelected.Exit)
                        {
                            Game.CurrScene.NextScene = null;
                            Game.CurrScene.IsPlaying = false;
                        }
                    }
                }
                else
                    nextInputCount -= Game.DeltaTime;
            }
        }

        public override void Update()
        {
            audioSourceBackground.Stream(clipMenu, Game.DeltaTime);

            if (statsScene != Stats.Show)
            {
                base.Update();
            }


            InputManager.Update();
        }

        public override void Draw()
        {
            base.Draw();
            playDemoWord.Draw();
            selector.Draw();
            exitWord.Draw();
            informationWords.Draw();
        }

        public override void OnExit()
        {
            base.OnExit();

            playDemoWord = exitWord = null;
            selector = null;
        }

        private void SetSelectorPosition()
        {
            Vector2 selectorOffset = Vector2.Zero;
            Word wordSelect = playDemoWord;

            switch (wordSelected)
            {
                case WordSelected.Play:
                    wordSelect = playDemoWord;
                    break;
                case WordSelected.Exit:
                    wordSelect = exitWord;
                    break;
            }

            selector.Position = wordSelect.Position + new Vector2(-wordSelect.Width * 2, 0);
        }
    }
}
