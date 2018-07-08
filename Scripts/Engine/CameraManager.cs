using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    struct Limits
    {
        public Limits(float minX, float maxX, float minY, float maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
    }

    static class CameraManager
    {
        public static Camera mainCamera;
        public static Vector2 DeltaCamera { get; private set; }

        public static float ViewPortLeft { get; private set; }
        public static float ViewPortRight { get; private set; }
        public static float ViewPortUp { get; private set; }
        public static float ViewPortDown { get; private set; }

        public static float MaxYOffset { get; set; }
        public static Limits CameraLimits { get; private set; }

        private static GameObject target;
        private static Dictionary<string, Tuple<Camera, float>> cameraList;

        private static float timeToReachTarget;
        private static float counter;
        private static float timeToReturnPlayer;

        private static Vector2 startPosition;
        private static Vector2 endPosition;

        public static bool IsUpdatingPosition { get; private set; }

        public static void Init()
        {
            ResetLimits();
            mainCamera = new Camera();
            cameraList = new Dictionary<string, Tuple<Camera, float>>();
            CalculateViewPort();
        }

        public static void Init(Vector2 position, Vector2 pivot)
        {
            ResetLimits();
            mainCamera = new Camera(position.X, position.Y);
            mainCamera.pivot = pivot;
            counter = 0;
            cameraList = new Dictionary<string, Tuple<Camera, float>>();
            CalculateViewPort();
        }

        public static void ResetCamera()
        {
            mainCamera.pivot = Vector2.Zero;
            mainCamera.position = Vector2.Zero;
            target = null;
            ResetLimits();
            cameraList.Clear();
        }

        public static void AddCamera(string cameraKey, float speedMul, Camera camera=null)
        {
            if(camera == null)
            {
                camera = new Camera(mainCamera.position.X, mainCamera.position.Y);
                camera.pivot = mainCamera.pivot;
            }

            cameraList.Add(cameraKey, new Tuple<Camera, float>(camera, speedMul));
        }

        public static Camera GetCamera(string cameraKey)
        {
            if(cameraList.ContainsKey(cameraKey))
            {
                return cameraList[cameraKey].Item1;
            }

            return null;
        }

        public static void ResetLimits()
        {
             CameraLimits = new Limits(Game.Window.Width / 2, Game.Window.Width * 26f, 210, Game.Window.Height * 0.5f);
        }

        private static void CalculateViewPort()
        {
            ViewPortRight = mainCamera.position.X + Game.Window.Width / 2;
            ViewPortLeft = mainCamera.position.X - Game.Window.Width / 2;
            ViewPortDown = mainCamera.position.Y + Game.Window.Height / 2;
            ViewPortUp = mainCamera.position.Y - Game.Window.Height / 2;
        }

        public static void SetTarget(GameObject newTarget)
        {
            target = newTarget;
        }

        static void CheckLimits()
        {
            if (mainCamera.position.Y > CameraLimits.MaxY)
                mainCamera.position.Y = CameraLimits.MaxY;
            else if (mainCamera.position.Y < CameraLimits.MinY)
                mainCamera.position.Y = CameraLimits.MinY;

            if (mainCamera.position.X > CameraLimits.MaxX)
                mainCamera.position.X = CameraLimits.MaxX;
            else if (mainCamera.position.X < CameraLimits.MinX)
                mainCamera.position.X = CameraLimits.MinX;
        }

        public static void Update()
        {
            DeltaCamera = mainCamera.position;

            if (target != null)
            {
                mainCamera.position = Vector2.Lerp(mainCamera.position, target.Position, Game.DeltaTime * 4);
            }
            

            CheckLimits();

            //per sapere di quanto si è spostata la main camera
            DeltaCamera = mainCamera.position - DeltaCamera;
           
            foreach(var item in cameraList)
            {
                item.Value.Item1.position += DeltaCamera * item.Value.Item2;
            }

            if(DeltaCamera.Length != 0)
                CalculateViewPort();
        }

        public static void MoveTo2(Vector2 endPos, float time, float timeToReturn = 0)
        {
            target = null;
            IsUpdatingPosition = true;
            counter = 0;
             
            startPosition = mainCamera.position;
            endPosition = endPos;

            timeToReachTarget = time;
            timeToReturnPlayer = timeToReturn;
        }

        public static void MoveTo(Vector2 endPos, float duration = 2)
        {
            startPosition = mainCamera.position;
            endPosition = endPos;
            timeToReachTarget = duration;
            IsUpdatingPosition = true;
            counter = 0;
        }

        public static void SetLimits(Limits limit)
        {
            CameraLimits = limit;
        }

        public static bool OutOfCameraViewPort(GameObject obj)
        {
            Vector2 objPosition = obj.Position;
            Vector2 objPivot = obj.GetSprite().pivot;

            return (objPosition.X - objPivot.X >= ViewPortRight || objPosition.X + objPivot.X < ViewPortLeft
                || objPosition.Y - objPivot.Y > ViewPortDown || objPosition.Y + objPivot.Y < ViewPortUp);
        }

        public static void ResetCameraViewPort()
        {
            ViewPortRight = mainCamera.position.X + Game.Window.Width / 2;
            ViewPortLeft = mainCamera.position.X - Game.Window.Width / 2;
            ViewPortDown = mainCamera.position.Y + Game.Window.Height / 2;
            ViewPortUp = mainCamera.position.Y - Game.Window.Height / 2;
        }
    }
}
