using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.VFXs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartBubble : SelenePart
    {
        public override void AfterDroneShift(State s, Combat c, int direction)
        {
            int index = s.ship.parts.IndexOf(this);
            if ( c.stuff.ContainsKey(index + s.ship.x))
            {
                c.stuff[index + s.ship.x + direction].bubbleShield = true;
            }
        }

        public override void AfterPlayerMove(State s, Combat c, int direction)
        {
            int index = s.ship.parts.IndexOf(this);
            if (c.stuff.ContainsKey(index + s.ship.x))
            {
                c.stuff[index + s.ship.x + direction].bubbleShield = true;
            }
        }
    }
}
