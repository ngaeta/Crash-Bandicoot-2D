using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    static class LetterManager
    {
        public static Word GetWord(string wordString, Vector2 startPos, float letterSpacing = 1f)
        {
            Word word = new Word();

            if(wordString.Count() > 0)
            {
                float xOffset = 0;
                float letterWidth = 0;

                for(int i=0; i < wordString.Count(); i++)
                {
                    if (char.IsLetter(wordString[i]))
                    {
                        GameObject letter = new GameObject(startPos + new Vector2(xOffset, 0), "Letter_" + char.ToUpper(wordString[i]), DrawManager.Layer.GUI);
                        xOffset += letter.Width + letterSpacing;
                        word.Add(letter);
                        letterWidth = letter.Width;
                    }
                    else if(wordString[i] == ' ')
                    {
                        xOffset += letterSpacing + letterWidth;
                    }
                }
            }

            return word;
        }

        public static GameObject GetSelector(Vector2 pos)
        {
            return new GameObject(pos, "Selector");
        }
    }
}
