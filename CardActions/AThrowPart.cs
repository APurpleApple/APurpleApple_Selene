﻿using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.Artifacts;

namespace APurpleApple.Selene
{
    internal class AThrowPart : CardAction
    {
        public required Part ejectedPart;
        public int worldX;
        public int damage = 1;

        public override void Begin(G g, State s, Combat c)
        {

            Ship ship = c.otherShip;
            RaycastResult raycastResult = CombatUtils.RaycastGlobal(c, ship, false, worldX);

            c.fx.Add(new VFX_PartEjection() { part = ejectedPart, worldX = worldX * 16, spins = raycastResult.hitDrone || raycastResult.hitShip, hitDrone = raycastResult.hitDrone });
            DamageDone dmg = new DamageDone();
            if (raycastResult.hitShip)
            {
                dmg = ship.NormalDamage(s, c, damage, raycastResult.worldX, false);
                Part? partAtWorldX = ship.GetPartAtWorldX(raycastResult.worldX);
                if (partAtWorldX != null && partAtWorldX.stunModifier == PStunMod.stunnable)
                {
                    c.QueueImmediate(new AStunPart
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if ((ship.Get(SStatus.payback) > 0 || ship.Get(SStatus.tempPayback) > 0))
                {
                    c.QueueImmediate(new AAttack
                    {
                        paybackCounter = 1,
                        damage = Card.GetActualDamage(s, ship.Get(SStatus.payback) + ship.Get(SStatus.tempPayback), true),
                        targetPlayer = true,
                        fast = true,
                        storyFromPayback = true
                    });
                }

                if (ship.Get(SStatus.reflexiveCoating) >= 1)
                {
                    c.QueueImmediate(new AArmor
                    {
                        worldX = raycastResult.worldX,
                        targetPlayer = false,
                        justTheActiveOverride = true
                    });
                }
                EffectSpawner.NonCannonHit(g, false, raycastResult, dmg);
            }

            if (raycastResult.hitDrone)
            {
                bool flag2 = c.stuff[raycastResult.worldX].Invincible();
                foreach (Artifact item5 in s.EnumerateAllArtifacts())
                {
                    if (item5.ModifyDroneInvincibility(s, c, c.stuff[raycastResult.worldX]) == true)
                    {
                        flag2 = true;
                        item5.Pulse();
                    }
                }

                if (c.stuff[raycastResult.worldX].bubbleShield)
                {
                    c.stuff[raycastResult.worldX].bubbleShield = false;
                }
                else if (flag2)
                {
                    c.QueueImmediate(c.stuff[raycastResult.worldX].GetActionsOnShotWhileInvincible(s, c, true, damage));
                }
                else
                {
                    c.DestroyDroneAt(s, raycastResult.worldX, true);
                }
                EffectSpawner.NonCannonHit(g, false, raycastResult, dmg);
            }

        }
    }
}
