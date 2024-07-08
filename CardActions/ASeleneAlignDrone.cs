using APurpleApple.Selene.Artifacts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneAlignDrone : CardAction, IAOversized
    {
        public int offset => -9;

        public Icon icon => new Icon(PMod.sprites["icon_alignDrone"].Sprite, null, Colors.white);

        public override void Begin(G g, State s, Combat c)
        {
            Artifact_Selene? art = s.EnumerateAllArtifacts().FirstOrDefault(a=>a is Artifact_Selene) as Artifact_Selene;
            if (art == null) return;
            int cockpitLocalX = s.ship.parts.FindIndex(p => p.type == PType.cockpit);
            if (cockpitLocalX == -1) cockpitLocalX = s.ship.parts.Count / 2;
            art.droneX = cockpitLocalX + s.ship.x;
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = base.GetTooltips(s);
            list.Add(PMod.glossaries["AlignDrone"]);
            return list;
        }
    }
}
