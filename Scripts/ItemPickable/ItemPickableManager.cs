using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    public enum PickableType { Apple, ExtraLife, Crystal }

    static class ItemPickableManager
    {
        static Queue<Pickable>[] items;

        public static void Init()
        {
            items = new Queue<Pickable>[Enum.GetValues(typeof(PickableType)).Length];

            for (int i = 0; i < items.Length; i++) 
            {
                items[i] = new Queue<Pickable>();

                switch ((PickableType)i)
                {
                    case PickableType.Apple:

                        for(int j=0; j < 150; j++)
                        {
                            Apple apple = new Apple(Vector2.Zero);
                            apple.IsActive=false;
                            items[i].Enqueue(apple);
                        }

                        break;

                    case PickableType.ExtraLife:

                        for (int j = 0; j < 50; j++)
                        {
                            ExtraLife life = new ExtraLife(Vector2.Zero);
                            life.IsActive = false;
                            items[i].Enqueue(life);
                        }

                        break;
                }
            }
        }

        public static Pickable GetItem(PickableType type)
        {
            return items[(int)type].Dequeue();
        }

        public static void RestoreItem(Pickable item)
        {
       
            item.IsActive = false;
            items[(int)item.Type].Enqueue(item);
        }

        public static List<Pickable> GetItems(PickableType type, int quantity)
        {
            List<Pickable> list = new List<Pickable>();

            for(int i=0; i < quantity; i++)
            {
                list.Add(items[(int)type].Dequeue());
            }

            return list;
        }
    }
}
