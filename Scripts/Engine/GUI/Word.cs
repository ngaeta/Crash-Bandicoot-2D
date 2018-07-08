using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrashBandicoot
{
    class Word 
    {
        List<GameObject> word;

        public Vector2 Position { get { return word[0].Position; } }
        public float Height { get { return word[0].Height; } }
        public float Width { get { return word[0].Width; } }

        public DrawManager.Layer Layer { get; set; }

        public Word() 
        {
            word = new List<GameObject>();
        }

        public void Add(GameObject letter)
        {
            word.Add(letter);
        }

        public void Draw()
        {
            for (int i = 0; i < word.Count; i++)
            {
                word[i].Draw();
            }
        }

        public void SetScale(Vector2 newScale)
        {
            for(int i=0; i < word.Count; i++)
            {
                word[i].GetSprite().scale = newScale;
            }
        }

        public void IsActive(bool value = true)
        {
            for (int i = 0; i < word.Count; i++)
            {
                word[i].IsActive = value;
            }
        }
    }
}
