using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    static class GuiManager
    {
        public enum GuiObjectType { APPLE, LIFE}

        public static List<GUIObject> guiObjects;
        public static GUIObject crystalGui;

        public static void Init()
        {
            guiObjects = new List<GUIObject>();
            guiObjects.Add(new NumerableGUIObject(new Vector2(50f, 30f), "apple"));
            guiObjects.Add(new NumerableGUIObject(new Vector2(Game.Window.Width - 135f, 35f), "extraLife"));

            crystalGui = new GUIObject(new Vector2(Game.Window.Width / 2, 45f), "crystal");
            crystalGui.AddScale(new Vector2(-0.95f));
            crystalGui.IsActive = false;
        }

        public static void SetGuiValue(GuiObjectType type, int value)
        {
            ((NumerableGUIObject)guiObjects[(int)type]).SetNumber(value);
        }

        public static GUIObject GetGuiObject(GuiObjectType type)
        {
            return guiObjects[(int)type];
        }

        public static void ActiveCrystal(bool value = true)
        {
            crystalGui.IsActive = value;
        }

        public static void Show(bool show)
        {
            //show gui
        }
    }
}
