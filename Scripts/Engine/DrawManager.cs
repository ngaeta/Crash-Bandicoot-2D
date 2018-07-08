using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    static class DrawManager
    {
        public enum Layer { Background, Middleground, Playground, Foreground, GUI }
        static List<IDrawable>[] itemsList;

        static DrawManager()
        {
            itemsList = new List<IDrawable>[(int)Layer.GUI + 1];

            for (int i = 0; i < itemsList.Length; i++)
            {
                itemsList[i] = new List<IDrawable>();
            }
        }

        public static void AddItem(IDrawable item)
        {
            itemsList[(int)item.Layer].Add(item);
        }

        public static void RemoveItem(IDrawable item)
        {
            itemsList[(int)item.Layer].Remove(item);
        }

        public static void RemoveAll()
        {
            for (int i = 0; i < itemsList.Length; i++)
            {
                itemsList[i].Clear();
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < itemsList.Length; i++)
            {
                for (int j = 0; j < itemsList[i].Count; j++)
                {
                    if (i == 0 || i==4 || itemsList[i][j] is Rect || itemsList[i][j] is Circle)
                    {
                        itemsList[i][j].Draw();
                    }
                    else if (itemsList[i][j] is Water || CameraManager.OutOfCameraViewPort((GameObject)itemsList[i][j]) == false)
                    {
                        itemsList[i][j].Draw();
                    }
                }
            }
        }
    }
}
