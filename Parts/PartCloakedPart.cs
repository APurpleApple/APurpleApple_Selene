using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Parts
{
    internal class PartCloakedPart : PartSelene
    {
        public Part? replacedPart;
        public double anim = 1;
        public double targetAnim = 0;
        public PartCloakedPart()
        {
            skin = "";
            isRendered = true;
            type = PType.empty;
            IsTemporary = false;
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

            if (replacedPart is PartSelene sp)
            {
                //sp.OnTurnStart(s, c);
            }
        }

        public override void OnCombatEnd(State s)
        {
            if (replacedPart == null) return;

            int index = s.ship.parts.IndexOf(this);
            s.ship.parts[index] = replacedPart;

            if (replacedPart is PartSelene sp)
            {
                sp.OnCombatEnd(s);
            }
        }


        public override void Render(Ship ship, int localX, G g, Vec v, Vec worldPos)
        {
            if (replacedPart == null) return;
            if (replacedPart.skin == null) return;
            v = GetPartPos(v, worldPos, localX, ship);

            anim = Mutil.SnapLerp(anim, targetAnim, 1, g.dt);

            replacedPart.xLerped = xLerped;
            Color c = Color.Lerp(Colors.boldPink.fadeAlpha(.5), Colors.white, anim);
            Draw.Sprite(DB.parts.GetOrNull(replacedPart.skin) ?? SSpr.parts_scaffolding,v.x-1, v.y-1,flipX: replacedPart.flip, color: c);
        }
    }
}
