using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ABubbleBayAnim : CardAction
    {
        public int partuuid = 0;
        public override void Begin(G g, State s, Combat c)
        {
            Part? part = s.ship.parts.Find(p=>p.uuid == partuuid);
            if (part != null && part is PartBubble pb)
            {
                int partIndex = s.ship.parts.FindIndex(p => p.uuid == partuuid);
                int worldX = s.ship.x + partIndex;
                if (c.stuff.ContainsKey(worldX) && !c.stuff[worldX].bubbleShield)
                {
                    pb.bubbleAnim = 0;
                    timer = .35;
                    c.QueueImmediate(new AAddBubbleShield() { partuuid = partuuid });
                    return;
                }
            }
            timer = 0;
        }
    }
}
