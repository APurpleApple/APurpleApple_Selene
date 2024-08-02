using APurpleApple.Selene.Artifacts;
using APurpleApple.Selene.Patches;
using FSPRO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneMagnetize : CardAction
    {
        public bool pull = true;

        //public int offset => -9;

        public override Icon? GetIcon(State s) => new Icon(PMod.sprites[pull ? "icon_magPull" : "icon_magPush"].Sprite, null, Colors.white);

        public override void Begin(G g, State s, Combat c)
        {
            int shipCenter = s.ship.parts.Count / 2;
            int shipCenterWorldX = shipCenter + s.ship.x ;

            foreach (KeyValuePair<int, StuffBase> item in c.stuff.Where(x=> (x.Key < shipCenterWorldX) ^ pull).OrderBy<KeyValuePair<int, StuffBase>, int>((KeyValuePair<int, StuffBase> x) => x.Key).ToList())
            {
                DoMoveSingleDrone(s, c, item.Key, ((item.Key > shipCenterWorldX) ^ pull) ? 1 : -1, true);
            }
            foreach (KeyValuePair<int, StuffBase> item in c.stuff.Where(x => (x.Key > shipCenterWorldX) ^ pull).OrderBy<KeyValuePair<int, StuffBase>, int>((KeyValuePair<int, StuffBase> x) => -x.Key).ToList())
            {
                DoMoveSingleDrone(s, c, item.Key, ((item.Key > shipCenterWorldX) ^ pull) ? 1 : -1, true);
            }
            Artifact_Selene? art = s.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (art != null)
            {
                if (art.droneX != shipCenterWorldX)
                {
                    art.droneX += ((art.droneX > shipCenterWorldX) ^ pull) ? 1 : -1;
                }
            }

        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = base.GetTooltips(s);
            Patch_Selene.magnetizeHint = pull;
            list.Add(PMod.glossaries[pull ? "MagPull" : "MagPush"]);
            return list;
        }

        public static void DoMoveSingleDrone(State s, Combat c, int x, int dir, bool playerDidIt)
        {
            if (!c.stuff.TryGetValue(x, out StuffBase? value) || value.Immovable())
            {
                return;
            }

            c.stuff.Remove(x);
            int num = dir;
            bool flag = true;
            while (num != 0 && flag)
            {
                int num2 = Math.Sign(num);
                num -= num2;
                value.x += num2;
                if (value.x < c.leftWall || value.x >= c.rightWall)
                {
                    c.stuff[value.x] = value;
                    Audio.Play(Event.Hits_HitDrone);
                    c.DestroyDroneAt(s, value.x, playerDidIt);
                    flag = false;
                }

                if (!c.stuff.ContainsKey(value.x))
                {
                    continue;
                }

                bool flag2 = false;
                if (!value.Invincible())
                {
                    StuffBase value2 = c.stuff[value.x];
                    c.stuff[value.x] = value;
                    Audio.Play(Event.Hits_HitDrone);
                    flag2 = true;
                    c.DestroyDroneAt(s, value.x, playerDidIt);
                    c.stuff[value.x] = value2;
                    flag = false;
                }

                if (!c.stuff[value.x].Invincible() || value.Invincible())
                {
                    c.DestroyDroneAt(s, value.x, playerDidIt);
                    if (!flag2)
                    {
                        Audio.Play(Event.Hits_HitDrone);
                    }
                }
            }

            if (flag)
            {
                c.stuff[value.x] = value;
            }
        }
    }
}
