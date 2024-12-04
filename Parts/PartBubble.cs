using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.VFXs;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartBubble : PartSelene, IModPart
    {
        public double bubbleAnim = 1;

        public PartBubble()
        {
            type = PMod.pTypes["Bubble"].PartType;
        }

        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes["Bubble"].PartType;
            return base.GetTooltips(s);
        }

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

        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Bubble",
           () => PMod.sprites["icon_part_bubble"].Sprite,
           () => PMod.Instance.Localizations.Localize(["parts", "Bubble", "name"]),
           () => PMod.Instance.Localizations.Localize(["parts", "Bubble", "description"]),
           helper);

            PMod.parts["selene_bubble"] = helper.Content.Ships.RegisterPart("selene_bubble", new PartConfiguration() { Sprite = PMod.sprites["selene_part_bubble"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        }
    }
}
