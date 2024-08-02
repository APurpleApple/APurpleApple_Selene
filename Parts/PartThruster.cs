using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.VFXs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartThruster : PartSelene
    {
        public bool upgraded = false;

        public override void OnTurnEnd(State s, Combat c)
        {
            c.Queue(new AVFX() { fx = new VFX_Thruster() { follow = this}, timer = 0.1 });
            c.Queue(new AMove() { dir = (upgraded ? 2 : 1) * (flip ? -1 : 1), targetPlayer = true });
        }
    }
}
