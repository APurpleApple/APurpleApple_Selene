using APurpleApple.Selene.Artifacts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneEjectAll : CardAction
    {
        public int damage = 1;
        public override void Begin(G g, State s, Combat c)
        {
            for (int i = s.ship.parts.Count-1; i >0 ; i--)
            {
                if (s.ship.parts[i] is SelenePart sp && sp.removedOnCombatEnd == true)
                {
                    c.QueueImmediate (new AEjectPart() { ejectedPart = sp, damage = damage });
                }
            }
        }
    }
}
