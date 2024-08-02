using APurpleApple.Selene.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class ASeleneInsertPart : CardAction
    {
        public required PartSelene part;
        public override void Begin(G g, State s, Combat c)
        {
            timer = 1.0;
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;

            if (s.EnumerateAllArtifacts().Any(a=>a is Artifact_CheapRandom))
            {
                new ADroneMove() {dir = 3, isRandom = true}.Begin(g, s, c);
            }

            int insertIndex = artifact_selene.droneX - s.ship.x;

            if (insertIndex < -1 || insertIndex > s.ship.parts.Count)
            {
                artifact_selene.anim = Artifact_Selene.EAnim.PickUpPartForThrow;
                artifact_selene.animAlpha = 0;
                artifact_selene.grabbedPart = part;
                artifact_selene.placeLeft = insertIndex <= s.ship.parts.Count / 2;
                artifact_selene.grabbedPartSkin = part.skin;

                int damage = 1;
                if (s.EnumerateAllArtifacts().Any(a => a is Artifact_EjectBuff)) damage++;

                c.QueueImmediate(new AThrowPart() { worldX = artifact_selene.droneX, ejectedPart = part, damage = damage});
                timer = .6;
                return;
            }

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

            foreach (var item in s.ship.parts)
            {
                if (item is PartSelene ps && item != part)
                {
                    ps.ShipWasModified(s.ship, s, c);
                }
            }
        }
        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> tooltips = base.GetTooltips(s);

            tooltips.Add(PMod.glossaries["AttachPart"]);
            tooltips.AddRange(part.GetTooltips());

            if (part.singleUse)
            {
                tooltips.Add(PMod.glossaries["SingleUse"]);
            }
            if (part.IsTemporary)
            {
                tooltips.Add(PMod.glossaries["Temp"]);
            }
            if (part.stunModifier == PStunMod.breakable)
            {
                tooltips.Add(PMod.glossaries["Breakable"]);
            }

            switch (part.damageModifier)
            {
                case PDamMod.none:
                    break;
                case PDamMod.weak:
                    tooltips.Add(new TTGlossary("parttrait.weak"));
                    break;
                case PDamMod.armor:
                    tooltips.Add(new TTGlossary("parttrait.armor"));
                    break;
                case PDamMod.brittle:
                    tooltips.Add(new TTGlossary("parttrait.brittle"));
                    break;
            }

            Artifact_Selene? artifact_selene = s.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return tooltips;

            artifact_selene.hilight = true;

            return tooltips;
        }
    }
}
