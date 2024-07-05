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
        public required SelenePart part;
        public override void Begin(G g, State s, Combat c)
        {
            part.Remove(s);
        }
    }
}
