using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.VFXs
{

    internal class VFX_Thruster : FX
    {
        public required PartThruster follow;
        public override void Render(G g, Vec v)
        {
            double x = v.x + (follow.xLerped ?? 0) * 16 + FxPositions.Hull(g.state.ship.x, true).x + (follow.flip ? 5:-5) ;
            double y = v.y + FxPositions.Hull(g.state.ship.x, true).y - 2;

            /*
            Spr? id = Missile.exhaustSprites.GetModulo((int)(g.state.time * 36.0 + (double)(x * 10)));
            Vec? originRel = new Vec(.5, 1.0);
            Color? color = new Color("fff387");
            Draw.Sprite(id, x, y, false, false, double.Pi * (follow.flip ? .5 : -.5), null, originRel, Vec.One * (follow.upgraded ? 2.0 : 1.5), null, color);*/

            Spr? id = Missile.exhaustSprites.GetModulo((int)(g.state.time * 36.0 + (double)(x * 10)));
            Vec? originRel = new Vec(.5, 1.0);
            Color? color = new Color("fff387");
            Draw.Sprite(id, x, y - 17, false, false, double.Pi * (follow.upgraded ? .4 : .3) * (follow.flip ? 1 : -1), null, originRel, Vec.One * (follow.upgraded ? 1.5 : 1.0), null, color);
            Draw.Sprite(id, x, y + 17, false, false, double.Pi * (follow.upgraded ? .6 : .7) * (follow.flip ? 1 : -1), null, originRel, Vec.One * (follow.upgraded ? 1.5 : 1.0), null, color);
        }

        public override void Update(G g)
        {
            if (age == 0.0)
            {
                Start(g);
            }

            age += g.dt;

            if (age > (follow.upgraded ? .6 : .4))
            {
                age = 2.0;
            }
        }
    }
}
