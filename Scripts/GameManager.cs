using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;

namespace CrashBandicoot
{
    static class GameManager
    {
        public static CheckpointCrate CurrentCheckpoint;
        public static List<ICheckpointLoadable> Loadables;
        public static List<IDestroyable> Destoyables;
        public static CrystalPickable CrystalOfLevel { get; set; }

        static GameManager()
        {
            Destoyables = new List<IDestroyable>();
        }

        public static void LoadCheckpoint()
        {
            if (PlayScene.Player.Life > 0)
            {
                PlayScene.Player.Life--;

                if (CurrentCheckpoint != null)
                {
                    CurrentCheckpoint.LoadCheckPoint();
                }
                else
                {
                    for (int i = 0; i < Loadables.Count; i++)
                    {
                        if (Loadables[i] == null)
                        {
                            Loadables.RemoveAt(i);
                            i--;
                        }
                        else
                            Loadables[i].OnCheckpointLoad(null);
                    }
                }
            }
            else
                Game.CurrScene.IsPlaying = false;
        }

        public static void DestroyAll()
        {
            foreach(IDestroyable d in Destoyables)
            {
                d.Destroy();
            }

            Destoyables.Clear();
            Loadables.Clear();
            CurrentCheckpoint = null;
        }
    }
}
