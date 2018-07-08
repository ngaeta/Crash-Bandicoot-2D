using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    static class BackgroundManager
    {
        const float OFFSET_X = 10f;

        private static GameObject background1;
        private static GameObject background2;
        
        private static GameObject backgroundToCheck;

        public static bool DrawBackground
        {
            set
            {
                background1.IsActive = background2.IsActive = value;
            }
        }


        public static void Init(Vector2 startPosition)
        {
            background1 = new GameObject(startPosition, "background_forest", DrawManager.Layer.Background);
            background1.AddScale(new Vector2(0.2f));
            background1.SetPivot(Vector2.Zero);

            background2 = new GameObject(startPosition + new Vector2(background1.Width - OFFSET_X, 0), "background_forest", DrawManager.Layer.Background);
            background2.AddScale(new Vector2(0.2f));
            background2.SetPivot(Vector2.Zero);

            backgroundToCheck = background1;
            DrawBackground = true;
        }

        public static  void Update()
        {
            if (backgroundToCheck.Position.X + backgroundToCheck.Width < CameraManager.ViewPortLeft)
            {
                GameObject other = backgroundToCheck == background1 ? background2 : background1;

                backgroundToCheck.Position = new Vector2(other.Position.X + backgroundToCheck.Width - OFFSET_X, backgroundToCheck.Y);
                backgroundToCheck = other;
            }

            else if (backgroundToCheck.Position.X > CameraManager.ViewPortLeft)
            {
                GameObject other = backgroundToCheck == background1 ? background2 : background1;

                other.Position = new Vector2(backgroundToCheck.Position.X - other.Width + OFFSET_X, backgroundToCheck.Y);
                backgroundToCheck = other;
            }
        }
    }
}
