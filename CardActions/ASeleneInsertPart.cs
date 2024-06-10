using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneInsertPart : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            int insertIndex = 2;

            for (int i = 0; i < s.ship.parts.Count; i++)
            {
                if (i < insertIndex) {
                    //s.ship.parts[i].xLerped -= .5;
                }
                else
                {
                    //s.ship.parts[i].xLerped -= .5;
                }
            }
            s.ship.x -= 1;
            s.ship.xLerped -= 1;
            s.ship.parts.Insert(insertIndex, new Part() { skin = ""});

        }
    }
}
