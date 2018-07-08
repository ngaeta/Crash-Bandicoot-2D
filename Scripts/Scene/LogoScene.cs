using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace CrashBandicoot
{
    class LogoScene : FadeScene
    {
        public LogoScene(float showTime=3, float fadeTime = 3) : base(showTime, fadeTime)
        {
           
        }

        public override void Start()
        {
            base.Start();
            AddGameObjects(new GameObject(new Vector2(Game.Window.Width / 2, Game.Window.Height / 2), "aivLogo"));
        }
    }
}
