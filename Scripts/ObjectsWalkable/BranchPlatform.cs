using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class BranchPlatform : Ground
    {
        private float counter;

        public BranchPlatform(Vector2 spritePosition, string spriteSheetName = "branchPlatform", DrawManager.Layer layer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName)
        {
            clipFootStep = AudioManager.GetAudioClip("woodenFootStep");
        }

        public override void Update()
        {
            base.Update();

            counter += Game.DeltaTime * 1.5f;
            float sin = (float)Math.Sin(counter) * 0.2f;

            Position = new Vector2(Position.X, Position.Y + sin);
        }
    }
}
