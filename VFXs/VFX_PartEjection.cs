using Nanoray.PluginManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class VFX_PartEjection : FX
    {
        public Part? part;
        public int worldX;
        public bool spins = true; 
        public bool hitDrone = false; 

        public override void Render(G g, Vec v)
        {
            if (part == null) return;
            Spr? spr = DB.parts.GetOrNull(part.skin ?? part.type.Key());

            Vec pos = new Vec(v.x + worldX + 8, v.y - age * 300 + 32 + 60);
            Draw.Sprite(spr, pos.x, pos.y, originPx: new Vec(8, 32), rotation: spins ? Ease.InSin(age) * 20 : Ease.InSin(age) * 4);
            
            if (spins && age > (hitDrone ? .1 : .15))
            {
                for (int j = 0; j < 5; j++)
                {
                    double size = 1.0;
                    double rot = Mutil.NextRand() * double.Pi * .25 + double.Pi * .375;
                    PFX.combatAlpha.Add(new Particle
                    {
                        pos = new Vec(worldX, pos.y - 20) + new Vec(double.Cos(rot), double.Sin(rot)) * -10,
                        lifetime = 2.0 * Mutil.NextRand(),
                        size = size,
                        color = new Color(1.0, 1.0, 1.0, 1.0),
                        dragCoef = 1.0,
                        dragVel = new Vec(0.0, 0.0),
                        sizeMode = SizeMode.Constant,
                        vel = new Vec(double.Cos(rot), double.Sin(rot)) * -200,
                        rotation = (float)rot,
                        sprite = PMod.sprites["selene_fx_part_bit_0"].Sprite,
                    });
                }

                age += 999;
            }

        }
    }
}
