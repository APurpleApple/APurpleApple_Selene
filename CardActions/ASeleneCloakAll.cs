using APurpleApple.Selene.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneCloakAll : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            for (int i = 0; i < s.ship.parts.Count; i++)
            {
                if (s.ship.parts[i] is CloakedPart) continue;

                s.ship.parts[i] = new CloakedPart() { replacedPart = s.ship.parts[i] };
            }
        }
    }
}
