using APurpleApple.Selene.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneActivateCloak : CardAction
    {
        public int cloakingPartUUID;
        public override void Begin(G g, State s, Combat c)
        {
            int partLocalX = s.ship.parts.FindIndex(p => p.uuid == cloakingPartUUID);
            if (partLocalX == -1) return;
            PartCloaking? cloak = s.ship.parts[partLocalX] as PartCloaking;
            if (cloak != null) cloak.hasBeenUsed = true;

            for (int i = int.Max(partLocalX - 1, 0); i < int.Min(partLocalX + 2, s.ship.parts.Count); i++)
            {
                if (s.ship.parts[i] is PartCloakedPart) continue;

                s.ship.parts[i] = new PartCloakedPart() { replacedPart = s.ship.parts[i] };
            }
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            Part? cloak = s.ship.parts.Find(p => p.uuid == cloakingPartUUID);
            if (cloak != null)
            {
                cloak.hilight = true;
            }

            return base.GetTooltips(s);
        }
    }
}
