using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    static class TriggerManager
    {
        static List<Trigger> triggers;
        static Dictionary<Trigger, bool> alreadyInTrigger;

        static List<Trigger> triggerToRemove;
        static Player player;

        static TriggerManager()
        {
            triggers = new List<Trigger>();
            triggerToRemove = new List<Trigger>();
            alreadyInTrigger = new Dictionary<Trigger, bool>();

            player = PlayScene.Player;
        }

        public static void AddTrigger(Trigger trigger)
        {
            triggers.Add(trigger);
            alreadyInTrigger.Add(trigger, false);
        }

        public static void RemoveTrigger(Trigger trigger)
        {
            triggerToRemove.Add(trigger);
        }

        public static void CheckTriggers()
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                Collision collision = new Collision();

                if (triggers[i].Rect.Collides(player.RigidBody.BoundingBox, ref collision))
                {
                    if (!alreadyInTrigger[triggers[i]])
                    {
                        triggers[i].OnTriggerEnter(collision.collider);
                        alreadyInTrigger[triggers[i]] = true;
                    }
                }
                else if (alreadyInTrigger[triggers[i]]) 
                {
                    triggers[i].OnTriggerExit(collision.collider);
                    alreadyInTrigger[triggers[i]] = false;
                }
            }

            if (triggerToRemove.Count > 0)
            {
                for (int j = 0; j < triggerToRemove.Count; j++)
                {
                    triggers.Remove(triggerToRemove[j]);
                    alreadyInTrigger.Remove(triggerToRemove[j]);
                }

                triggerToRemove.Clear();
            }
        }
    }
}
