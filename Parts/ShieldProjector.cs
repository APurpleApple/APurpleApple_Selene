using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class ShieldProjector : SelenePart
    {
        public int blocked = 1;
        [JsonIgnore]
        public double shieldDeployAnim = 0;
        public override void Render(G g, Vec v, int i)
        {
            shieldDeployAnim = Mutil.SnapLerp(shieldDeployAnim, 1, 5, g.dt);
            Draw.Sprite(PMod.sprites["FX_ShieldProj"].Sprite, v.x + 7, v.y,blend: BlendMode.Screen, scale: new Vec(shieldDeployAnim,1), originRel : new Vec(.5,0));
        }
    }
}
