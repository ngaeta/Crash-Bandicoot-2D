using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    abstract class DestructableCrate : Crate
    {
        

        public DestructableCrate(Vector2 spritePosition, string spriteSheetName, DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
           
        }

       
    }
}
