using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    static class Game
    {
        public enum SceneLoad { Next, Prev }
        private static Window window;

        public static int NumJoypad { get; private set; }
        public static Window Window { get { return window; } }
        public static float DeltaTime { get { return window.deltaTime; } }
        public static float Gravity { get; private set; }
        public static float FrictionX { get; private set; }
        public static bool IsPlaying { get; set; }
        public static Scene CurrScene { get; set; }
        public static SceneLoad SceneToLoad {get; set;}

        static Game()
        {
            window = new Window(1280, 720, "Crash Bandicoot")
            {
                Position = new Vector2(300, 50)
            };

            window.SetIcon("Assets/extraLife.ico");
            //window.SetCursor(false);

            Gravity = 400.0f;
            FrictionX = 140f;
        }

        public static void Play()
        {
            AudioManager.InitClips();
            AudioManager.Load();
            GfxManager.Load();

            Scene logoScene = new LogoScene();
            Scene menuScene = new MenuScene();

            Scene playScene = new PlayScene();
            Scene gameOver = new GameOverScene();

            logoScene.NextScene = menuScene;
            menuScene.NextScene = playScene;

            playScene.PreviousScene = menuScene;
            playScene.NextScene = gameOver;

            gameOver.NextScene = null;

            CurrScene = logoScene;
        
            CurrScene.Start();

            while (Window.IsOpened)
            {
                //float fps = 1 / Window.deltaTime;
                //Console.SetCursorPosition(0, 0);
                //if (fps < 59)
                //    Console.Write((1 / Window.deltaTime) + "                   ");

                //Input
                InputManager.Update();

                if (Window.GetKey(KeyCode.Esc))
                    break;

                if (!CurrScene.IsPlaying)
                {
                    if (SceneToLoad == SceneLoad.Next)
                    {
                        if (CurrScene.NextScene != null)
                        {
                            CurrScene.OnExit();
                            CurrScene = CurrScene.NextScene;
                            CurrScene.Start();
                        }
                        else
                            return;
                    } else
                    {
                        if (CurrScene.PreviousScene != null)
                        {
                            CurrScene.OnExit();
                            CurrScene = CurrScene.PreviousScene;
                            CurrScene.Start();
                            SceneToLoad = SceneLoad.Next;
                        }
                        else
                            return;
                    }
                }

                CurrScene.Input();
                CurrScene.Update();
                CurrScene.Draw();

                Window.Update();
            }
        }
    }
}
