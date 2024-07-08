using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    public class AVFX : CardAction
    {
        public required FX fx;

        public override void Begin(G g, State s, Combat c)
        {
            c.fx.Add(fx);
        }
    }
}
