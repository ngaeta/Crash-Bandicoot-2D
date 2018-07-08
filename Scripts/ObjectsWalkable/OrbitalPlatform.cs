using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class OrbitalPlatform : InvisiblePlatform
    {
        private float minDistToCenter;
        private float rotationAngle;
        private float rotationSpeed;
        private Vector2 orbitalPoint;

        private Vector2 nextPoint;

        public OrbitalPlatform(Vector2 spritePosition, Vector2 orbitalPoint, float rotationSpeed = 100f, string spriteSheetName = "platform") : base(spritePosition, spriteSheetName)
        {
            this.orbitalPoint = orbitalPoint;
            this.rotationSpeed = rotationSpeed;

            Vector2 distanceVector = Position - orbitalPoint;
            minDistToCenter = distanceVector.Length;

            CalculateNextPoint();
        }

        public override void Update()
        {
            base.Update();

            float distancePos = (nextPoint - Position).Length;

            if (Math.Abs(distancePos) <= 20f)
            {
                CalculateNextPoint();
            }
            //rotationAngle += rotationSpeed * Game.DeltaTime;

            //float x = (float)Math.Cos(rotationAngle);
            //float y = (float)Math.Sin(rotationAngle);

            // Vector2 nextPoint = (new Vector2(x, y) * minDistToCenter) + orbitalPoint;
            //Position = nextPoint

        }

        private void CalculateNextPoint()
        {
            Vector2 distanceVector = Position - orbitalPoint;
            rotationAngle = (float)Math.Atan2(distanceVector.Y, distanceVector.X);

            rotationAngle += rotationSpeed;

            float x = (float)Math.Cos(rotationAngle);
            float y = (float)Math.Sin(rotationAngle);

            nextPoint = (new Vector2(x, y) * minDistToCenter) + orbitalPoint;
            Velocity = ((nextPoint - Position).Normalized()) * rotationSpeed / 2;
        }
    }
}
