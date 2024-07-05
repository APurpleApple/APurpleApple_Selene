﻿using APurpleApple.Selene.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneInsertPart : CardAction
    {
        public required SelenePart part;
        public override void Begin(G g, State s, Combat c)
        {
            timer = 1.0;
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;

            int insertIndex = artifact_selene.droneX - s.ship.x;

            if (insertIndex < -1) return;
            if (insertIndex > s.ship.parts.Count) return;

            part = Mutil.DeepCopy(part);

            if (insertIndex < s.ship.parts.Count / 2)
            {
                foreach (Part part in s.ship.parts)
                {
                    part.xLerped += 1;
                }
                s.ship.x -= 1;
                s.ship.xLerped = s.ship.x;
                insertIndex++;

                part.xLerped = insertIndex +1;
            }
            else
            {
                part.xLerped = insertIndex-1;
            }

            
            artifact_selene.anim = Artifact_Selene.EAnim.PickUpPart;
            artifact_selene.animAlpha = 0;
            artifact_selene.grabbedPart = part;
            artifact_selene.placeLeft = insertIndex <= s.ship.parts.Count / 2;
            artifact_selene.grabbedPartSkin = part.skin;
            part.skin = "scaffolding";
            s.ship.parts.Insert(insertIndex, part);

        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> tooltips = base.GetTooltips(s);

            tooltips.Add(PMod.glossaries["AttachPart"]);
            tooltips.Add(PMod.glossaries[part.tooltip]);


            if (part.singleUse)
            {
                tooltips.Add(PMod.glossaries["SingleUse"]);
            }
            if (part.removedOnCombatEnd)
            {
                tooltips.Add(PMod.glossaries["Temp"]);
            }
            if (part.stunModifier == PStunMod.breakable)
            {
                tooltips.Add(PMod.glossaries["Breakable"]);
            }

            Artifact_Selene? artifact_selene = s.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return tooltips;

            artifact_selene.hilight = true;

            return tooltips;
        }
    }
}
