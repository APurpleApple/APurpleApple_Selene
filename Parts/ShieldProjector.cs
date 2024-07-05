using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class ShieldProjector : SelenePart
    {
        public override void Render(G g, Vec v)
        {
            Draw.Sprite(PMod.sprites["FX_ShieldProj"].Sprite, v.x - 17, v.y,blend: BlendMode.Screen);
        }
    }
}
