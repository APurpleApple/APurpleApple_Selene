using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartDynamoCannon : PartSelene
    {
        public int worldX = 0;

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
    }
}
