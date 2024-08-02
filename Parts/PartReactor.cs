using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.Cards;
using System.Runtime.InteropServices;

namespace APurpleApple.Selene
{
    public class PartReactor : PartSelene
    {
        public int changeAmount = 1;
        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> list = base.GetTooltips();
            list.Add(new TTGlossary((changeAmount > 0) ? "action.gainEnergy" : "action.loseEnergy", changeAmount));
            return list;
        }

        public override void Destroy(State s, Combat c)
        {
            base.Destroy(s, c);
            int x = s.ship.parts.IndexOf(this);

            if (x -1 >= 0)
            {
                s.ship.NormalDamage(s, c, 1, s.ship.x + x - 1);
            }

            if (x +1 < s.ship.parts.Count)
            {
                s.ship.NormalDamage(s, c, 1, s.ship.x + x + 1);
            }
        }

        public override void OnTurnStart(State s, Combat c)
        {
            c.QueueImmediate(new AEnergy() { changeAmount = changeAmount});
        }
    }
}
