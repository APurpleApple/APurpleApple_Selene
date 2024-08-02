using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class AAddBubbleShield : CardAction
    {
        public int partuuid;
        public override void Begin(G g, State s, Combat c)
        {
            
            int partIndex = s.ship.parts.FindIndex(p => p.uuid == partuuid);
            int worldX = s.ship.x + partIndex;
            if (c.stuff.ContainsKey(worldX))
            {
                c.stuff[worldX].bubbleShield = true;
            }
            timer = .05;
        }
    }
}
