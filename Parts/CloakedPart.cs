using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Parts
{
    internal class CloakedPart : SelenePart
    {
        public Part? replacedPart;
        public double anim = 1;
        public double targetAnim = 0;
        public CloakedPart()
        {
            skin = "";
            isRendered = true;
            type = PType.empty;
        }
        public override List<Tooltip> GetTooltips()
        {
            return new List<Tooltip>();
        }

        public override void OnTurnStart(State s, Combat c)
        {
            if (replacedPart == null) return;

            int index = s.ship.parts.IndexOf(this);
            s.ship.parts[index] = replacedPart;

            if (replacedPart is SelenePart sp)
            {
                //sp.OnTurnStart(s, c);
            }
        }

        public override void OnCombatEnd(State s)
        {
            if (replacedPart == null) return;

            int index = s.ship.parts.IndexOf(this);
            s.ship.parts[index] = replacedPart;

            if (replacedPart is SelenePart sp)
            {
                sp.OnCombatEnd(s);
            }
        }


        public override void Render(G g, Vec v, int i)
        {
            if (replacedPart == null) return;
            if (replacedPart.skin == null) return;

            anim = Mutil.SnapLerp(anim, targetAnim, 1, g.dt);

            replacedPart.xLerped = xLerped;
            Color c = Color.Lerp(Colors.boldPink.fadeAlpha(.5), Colors.white, anim);
            Draw.Sprite(DB.parts.GetOrNull(replacedPart.skin) ?? SSpr.parts_scaffolding,v.x-1, v.y-1,flipX: replacedPart.flip, color: c);
        }
    }
}
