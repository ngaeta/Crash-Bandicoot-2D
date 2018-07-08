using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;

namespace CrashBandicoot
{
    //markup interface for elements where the player can run.
    interface IWalkable
    {
        bool CanWalkable { get; set; }
        AudioClip AudioFootStep { get; }
    }
}
