using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartShieldProjector : PartSelene
    {
        public int blocked = 1;
        [JsonIgnore]
        public double shieldDeployAnim = 0;
        public double shieldPulse = 1;

        public override int RenderDepth => -2;

        public override void Render(Ship ship, int localX, G g, Vec v, Vec worldPos)
        {
            if (isRendered)
            {
                v = GetPartPos(v, worldPos, localX, ship);
                shieldDeployAnim = Mutil.SnapLerp(shieldDeployAnim, 1, 10, g.dt);
                Draw.Sprite(PMod.sprites["fx_shield_" + (int)(shieldDeployAnim * 7)].Sprite, v.x + 7, v.y+3, originRel: new Vec(.5, 0));

                if (shieldPulse < 1)
                {
                    Draw.Sprite(PMod.sprites["fx_shieldImpact_" + (int)(shieldPulse * 5) % 5].Sprite, v.x + 7, v.y + 3, color: Colors.healthBarShield, blend: BlendMode.Screen, originRel: new Vec(.5, 0));
                    shieldPulse = Mutil.SnapLerp(shieldPulse, 1, 10, g.dt);
                }
            }
        }
    }
}
