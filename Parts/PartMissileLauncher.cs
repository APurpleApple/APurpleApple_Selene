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
    public class PartMissileLauncher : PartSelene
    {
        public required StuffBase launched;
        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> list = base.GetTooltips();
            list.AddRange(launched.GetTooltips());
            return list;
        }

        public override void OnTurnStart(State s, Combat c)
        {
            int x = s.ship.parts.IndexOf(this);
            c.Queue(new ASpawn() { thing = Mutil.DeepCopy(launched), multiBayVolley = true, fromPlayer = true, fromX = x });
        }
    }
}
