using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using System.Drawing;
using Aiv.Audio;

namespace CrashBandicoot
{
    class PlayScene : Scene
    {
        public static Player Player { get; set; }
        public bool IsPaused { get; private set; }

        private Pause pause;
        private float currTime = 1f;
        private bool debug;

        public static bool IsLoadingCheckpoint { get; set; }

        public override void Start()
        {
            base.Start();

            AudioManager.InitPlayScene();
            //AudioManager.Load();

            //GfxManager.Load(); fatto in Game
            IsLoadingCheckpoint = false;
            IsPaused = false;
            Player = null;

            Rect.Debug = false;
            Circle.Debug = false;
            PhysicsManager.RayDebug = false;

            Vector2 screenCenter = new Vector2(Game.Window.Width / 2, Game.Window.Height / 2);
            CameraManager.Init(screenCenter, screenCenter);
            CameraManager.AddCamera("GUI", 0f);

            PhysicsManager.Init();
            GuiManager.Init();
            ItemPickableManager.Init();
            BackgroundManager.Init(new Vector2(0, -158));

            GfxManager.LoadTiledMap("Assets/Tiles/level.tmx");

           // CameraManager.mainCamera.position = Player.Position;  //togliere
            CameraManager.SetTarget(Player);

            AudioManager.SetDefaultClipBackground("defaultClipBackground");
        }

        public override void Input()
        {
            Player.Input();
        }

        public override void Update()
        {
            if (!IsPaused)
            {
                AudioManager.Update();
                PhysicsManager.Update();
                UpdateManager.Update();
                PhysicsManager.CheckCollisions();
                TriggerManager.CheckTriggers();
                CameraManager.Update(); // si fa alla fine       
                BackgroundManager.Update();
            }
            else
                pause.Update();
        }

        public override void Draw()
        {
            if (!IsLoadingCheckpoint)
            {
                DrawManager.Draw();
            }
            else
            {
                //RenderTexture renderTexture = new RenderTexture(Game.Window.Width, Game.Window.Height);

                //Game.Window.RenderTo(renderTexture, true);
                //DrawManager.Draw();
                //Game.Window.RenderTo(null);

                //Sprite s = new Sprite(renderTexture.Width, renderTexture.Height);

                //s.pivot = CameraManager.mainCamera.pivot;
                //s.position = CameraManager.mainCamera.position;

                currTime -= Game.DeltaTime;

                if (currTime > 0)
                {
                    //float colorMul = currTime / 2f;

                    //s.SetMultiplyTint(colorMul, colorMul, colorMul, 1);
                    //s.DrawTexture(renderTexture);
                    //renderTexture.Dispose();
                }

                else if (currTime < -1)
                {
                    GameManager.LoadCheckpoint();
                    IsLoadingCheckpoint = false;
                    currTime = 2f;

                    //renderTexture.Dispose();
                    //renderTexture = null;

                    //s.Dispose();
                    //s = null;
                }
            }
        }

        public override void OnExit()
        {
            CameraManager.ResetCamera();
            PhysicsManager.RemoveAll();
            UpdateManager.RemoveAll();
            DrawManager.RemoveAll();

            GameManager.DestroyAll();
        }
    }
}
