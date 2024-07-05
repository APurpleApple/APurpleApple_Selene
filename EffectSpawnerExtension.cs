using FMOD;
using FSPRO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    internal class EffectSpawnerExtension
    {
        public static void PartExploding(State s, Rect partRect)
        {
            for (int i = 0; i < 10; i++)
            {
                PFX.combatSparks.MakeSparkBounds(partRect, Mutil.NextRand(), Mutil.NextRand(), 0.0);
            }

            for (int j = 0; j < 20; j++)
            {
                double size = 3.0 + Mutil.NextRand() * 8.0;
                Vec pos = new Vec(partRect.x, partRect.y) + new Vec(partRect.w * Mutil.NextRand(), partRect.h * Mutil.NextRand());
                PFX.combatAlpha.Add(new Particle
                {
                    pos = pos,
                    lifetime = 2.0 * Mutil.NextRand(),
                    size = size,
                    color = new Color(1.0, 1.0, 1.0, 0.2),
                    dragCoef = 1.0,
                    dragVel = new Vec(0.0, -10.0)
                });
            }

            for (int k = 0; k < 3; k++)
            {
                double size2 = 3.0 + Mutil.NextRand() * 3.0;
                Vec pos2 = new Vec(partRect.x, partRect.y) + new Vec(partRect.w * Mutil.NextRand(), partRect.h * Mutil.NextRand());
                PFX.combatExplosion.Add(new Particle
                {
                    pos = pos2,
                    lifetime = 2.0 * Mutil.NextRand(),
                    size = size2,
                    dragCoef = 1.0,
                    dragVel = new Vec(0.0, -10.0)
                });
            }

            for (int j = 0; j < 4; j++)
            {
                double size = 1.0;
                Vec pos = new Vec(partRect.x, partRect.y) + new Vec(partRect.w * Mutil.NextRand(), partRect.h * Mutil.NextRand());
                double rot = Mutil.NextRand() * double.Pi;
                PFX.combatAlpha.Add(new Particle
                {
                    pos = pos,
                    lifetime = 2.0 * Mutil.NextRand(),
                    size = size,
                    color = new Color(1.0, 1.0, 1.0, 1.0),
                    dragCoef = 1.0,
                    dragVel = new Vec(0.0, 0.0),
                    sizeMode = SizeMode.Constant,
                    vel = new Vec(double.Cos(rot), double.Sin(rot)) * 100,
                    rotation = (float)rot,
                    sprite = PMod.sprites["selene_fx_part_bit_0"].Sprite,
                });
            }
        }
    }
}
