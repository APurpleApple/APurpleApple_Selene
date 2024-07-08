using APurpleApple.Selene.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneWeld : CardAction
    {
        public bool removeTemp = false;
        public bool removeSingleUse = false;
        public bool removeBreakable = false;
        public override void Begin(G g, State s, Combat c)
        {
            Artifact_Selene? art = Artifact_Selene.Find(s);
            if (art == null) return;

            Part? part = s.ship.GetPartAtWorldX(art.droneX);
            if (part == null || part is not SelenePart sp) return;

            if (removeBreakable && sp.stunModifier == PStunMod.breakable) sp.stunModifier = PStunMod.none;
            if (removeTemp && sp.removedOnCombatEnd) sp.removedOnCombatEnd = false;
            if (removeSingleUse && sp.singleUse) sp.singleUse = false;
        }
        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = base.GetTooltips(s);
            list.Add(PMod.glossaries["WeldPart"]);
            return list;
        }
    }
}
