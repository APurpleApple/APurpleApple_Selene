using APurpleApple.Selene.Artifacts;
using FSPRO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneRecycle : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            Artifact_Selene? art = Artifact_Selene.Find(s);
            if (art == null) return;

            Part? part = s.ship.GetPartAtWorldX(art.droneX);
            if (part == null || part is not SelenePart sp || !sp.removedOnCombatEnd) return;

            sp.Destroy(s, c);
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = base.GetTooltips(s);
            list.Add(PMod.glossaries["RecyclePart"]);
            return list;
        }

        public class Condition : ExternalAPIs.IKokoroApi.IConditionalActionApi.IBoolExpression
        {
            public string GetTooltipDescription(State state, Combat? combat)
            {
                return PMod.Instance.Localizations.Localize(["action", "RecyclePart", "condition"]);
            }

            public bool GetValue(State state, Combat combat)
            {
                Artifact_Selene? art = Artifact_Selene.Find(state);
                if (art == null) return false;
                Part? part = state.ship.GetPartAtWorldX(art.droneX);
                if (part != null && part is SelenePart sp && sp.removedOnCombatEnd) return true;
                return false;
            }

            public void Render(G g, ref Vec position, bool isDisabled, bool dontRender)
            {
                if (dontRender) return;
                position.x -= 4;
                Draw.Sprite(PMod.sprites["icon_recyclePart"].Sprite, position.x, position.y);
                position.x += 9;
            }
        }
    }
}
