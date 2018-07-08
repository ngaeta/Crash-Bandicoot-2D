using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    static class NumberSpriteManager
    {
        const int COPY_OF_NUMBERS = 6;

        public struct NumberGUI
        {
            public NumberGUI(GUIObject numberObj, int number)
            {
                this.number = number;
                numberGui = numberObj;
                numberGui.IsActive = false;
                UpdateManager.RemoveItem(numberGui);
            }

            public GUIObject numberGui;
            public int number;
        }

        static Dictionary<int, Queue<NumberGUI>> numbersDict;

        static NumberSpriteManager()
        {
            numbersDict = new Dictionary<int, Queue<NumberGUI>>();

            //creates COPY_OF_NUMBERS copy for every number between 0 and 9
            for (int i = 0; i <= 9; i++)
            {
                Queue<NumberGUI> queue = new Queue<NumberGUI>();

                for (int j = 0; j < COPY_OF_NUMBERS; j++)
                {
                    NumberGUI num = new NumberGUI(new GUIObject(Vector2.Zero, "num_" + i), i);
                    queue.Enqueue(num);
                }

                numbersDict.Add(i, queue);
            }
        }

        public static List<NumberGUI> GetNumber(int number, ref List<NumberGUI> listNumbers)
        {
            listNumbers.Clear();

            string stringNumber = number.ToString();

            for (int i = 0; i < stringNumber.Length; i++)
            {
                NumberGUI num = numbersDict[int.Parse(stringNumber[i].ToString())].Dequeue();
                listNumbers.Add(num);
            }

            return listNumbers;
        }

        public static void RestoreNumbers(List<NumberGUI> numberRestore)
        {
            for(int i=0; i <numberRestore.Count; i++)
            {
                numberRestore[i].numberGui.IsActive = false;
                numbersDict[numberRestore[i].number].Enqueue(numberRestore[i]);
            }
        }

    }
}
