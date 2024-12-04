using APurpleApple.Selene.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneRemovePart : CardAction
    {
        public required int uuid;
        public override void Begin(G g, State s, Combat c)
        {
            int index = s.ship.parts.FindIndex(p=>p.uuid == uuid);

            if (index < s.ship.parts.Count / 2)
            {
                s.ship.x += 1;
                s.ship.xLerped = s.ship.x;

                foreach (var part in s.ship.parts)
                {
                    part.xLerped -= 1;
                }
            }

            s.ship.parts.RemoveAt(index);

            foreach (var item in s.ship.parts)
            {
                if (item is PartSelene ps)
                {
                    ps.ShipWasModified(s.ship, s, c);
                }
            }
        }
    }
}
