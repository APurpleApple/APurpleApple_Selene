using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.VFXs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartBubble : PartSelene
    {
        public double bubbleAnim = 1;
        public override void AfterDroneShift(State s, Combat c, int direction)
        {
            c.QueueImmediate(new ABubbleBayAnim { partuuid = uuid });
        }

        public override void AfterPlayerMove(State s, Combat c, int direction)
        {
            c.QueueImmediate(new ABubbleBayAnim { partuuid = uuid });
        }

        public override void ShipWasModified(Ship ship, State s, Combat c)
        {
            c.QueueImmediate(new ABubbleBayAnim { partuuid = uuid });
        }

        public override void Render(Ship ship, int localX, G g, Vec v, Vec worldPos)
        {
            if (isRendered)
            {
                v = GetPartPos(v, worldPos, localX, ship);
                bubbleAnim = Mutil.SnapLerp(bubbleAnim, 1, 10, g.dt);
                Draw.Sprite(PMod.sprites["fx_bubble_" + (int)(bubbleAnim * 5) % 5].Sprite, v.x + 7, v.y - 27, originRel: new Vec(.5, 0));
            }
        }
    }
}
