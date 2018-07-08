using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    abstract class Scene
    {
        public bool IsPlaying { get; set; }
        public Scene PreviousScene { get; set; }
        public Scene NextScene { get; set; }
        public Scene()
        {

        }

        public virtual void Start()
        {
            IsPlaying = true;
        }

        public abstract void Input();

        public virtual void Reset()
        {
            OnExit();
            Start();
        }

        public virtual void OnExit()
        {

        }

        public abstract void Update();

        public abstract void Draw();
    }
}
