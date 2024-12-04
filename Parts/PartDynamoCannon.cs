using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartDynamoCannon : PartSelene, IModPart
    {
        public int worldX = 0;

        public PartDynamoCannon()
        {
            type = PMod.pTypes["Dynamo"].PartType;
        }

        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes["Dynamo"].PartType;
            return base.GetTooltips(s);
        }

        public override void Render(Ship ship, int localX, G g, Vec v, Vec worldPos)
        {
            this.worldX = localX + ship.x;
        }

        public override void AfterPlayerMove(State s, Combat c, int direction)
        {
            int newWorldX = s.ship.x + s.ship.parts.IndexOf(this);
            if (worldX != newWorldX)
            {
                worldX = newWorldX;
                c.QueueImmediate(new AAttack() { damage = Card.GetActualDamage(s, 1), targetPlayer = false, fromX = s.ship.parts.IndexOf(this), multiCannonVolley = true });
            }
        }

        public override void ShipWasModified(Ship ship, State s, Combat c)
        {
            int newWorldX = s.ship.x + s.ship.parts.IndexOf(this);
            if (worldX != newWorldX)
            {
                worldX = newWorldX;
                c.QueueImmediate(new AAttack() { damage = Card.GetActualDamage(s, 1), targetPlayer = false, fromX = s.ship.parts.IndexOf(this), multiCannonVolley = true });
            }
        }

        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Dynamo",
                () => PMod.sprites["icon_part_dynamo"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "Dynamo", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "Dynamo", "description"]),
                helper);

            PMod.parts["selene_dynamo"] = helper.Content.Ships.RegisterPart("selene_dynamo", new PartConfiguration() { Sprite = PMod.sprites["selene_part_dynamo"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        }
    }
}
