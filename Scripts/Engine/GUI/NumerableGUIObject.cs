using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    class NumerableGUIObject : GUIObject
    {
        private List<NumberSpriteManager.NumberGUI> number;

        public NumerableGUIObject(Vector2 spritePosition, string spriteSheetName) : base(spritePosition, spriteSheetName)
        {
            number = new List<NumberSpriteManager.NumberGUI>();
            SetNumber(0);
        }

        public void SetNumber(int newNumber)
        {
            NumberSpriteManager.RestoreNumbers(number);
            number = NumberSpriteManager.GetNumber(newNumber, ref number);
            Vector2 pos = sprite.position;

            for (int i = 0; i < number.Count; i++)
            {
                number[i].numberGui.Position = new Vector2(pos.X + number[i].numberGui.Width, pos.Y + 5f);
                number[i].numberGui.IsActive = true;
                pos.X = number[i].numberGui.Position.X;
            }
        }
    }
}
