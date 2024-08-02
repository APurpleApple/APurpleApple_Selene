using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using HarmonyLib;

namespace APurpleApple.Selene
{
    public abstract class PartSelene : Part, ICustomPart
    {
        public bool singleUse = false;
        public Spr icon;
        public string tooltip = "";
        public bool isRendered = false;
        public virtual int RenderDepth => 0;
        public bool IsTemporary { get; set; } = true;

        public virtual List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>();
            tooltips.Add(PMod.glossaries[tooltip]);
            return tooltips;
        }

        public virtual void Destroy(State s, Combat c)
        {
            active = false;
            EffectSpawnerExtension.PartExploding(s, GetPartRect(s));
            c.Queue(new ASeleneRemovePart() { part = this });
            isRendered = false;
        }

        public Rect GetPartRect(State s)
        {
            double y = FxPositions.Cannon(0, true).y;
            double y2 = FxPositions.Back(0, true).y;
            return new Rect((s.ship.x + (xLerped ?? 0)) * 16.0, y, 16, y2 - y).round();
        }

        public virtual void ShipWasModified(Ship ship, State s, Combat c)
        {

        }

        public virtual void Render(Ship ship, int localX, G g, Vec v, Vec worldPos) { }
        public virtual bool DoVanillaRender(Ship ship, int localX, G g) => true;

        public virtual void RenderUI(Ship ship, G g, Combat? combat, int localX, string keyPrefix, bool isPreview, Vec v)
        {
            Vec vec = new Vec(localX * 16);
            int num = (isPreview ? 25 : 34);
            if (ship.isPlayerShip)
            {
                vec.y -= num - 6;
            }

            Rect rect = new Rect(vec.x - 1.0, vec.y, 17.0, num);
            Rect value = rect;
            value.h -= 8.0;
            if (!ship.isPlayerShip)
            {
                value.y += 8.0;
            }

            Box box = g.Push(new UIKey((UK.part), localX, "selenePart"), rect, value);
            Vec xy = box.rect.xy;

            if (box.IsHover())
            {
                Vec pos = xy + new Vec(16.0);

                foreach (var item in GetTooltips())
                {
                    g.tooltips.Add(pos, item);
                }

                if (invincible)
                {
                    g.tooltips.Add(pos, new TTGlossary("parttrait.invincible"));
                }
                else
                {
                    if (damageModifier == PDamMod.armor)
                    {
                        g.tooltips.Add(pos, new TTGlossary("parttrait.armor"));
                    }

                    if (damageModifier == PDamMod.weak)
                    {
                        g.tooltips.Add(pos, new TTGlossary("parttrait.weak"));
                    }

                    if (damageModifier == PDamMod.brittle && !brittleIsHidden)
                    {
                        g.tooltips.Add(pos, new TTGlossary("parttrait.brittle"));
                    }
                }

                if (stunModifier == PStunMod.stunnable)
                {
                    g.tooltips.Add(pos, new TTGlossary("parttrait.stunnable"));
                }

                if (stunModifier == PStunMod.unstunnable)
                {
                    g.tooltips.Add(pos, new TTGlossary("parttrait.unstunnable"));
                }

                if (stunModifier == PStunMod.breakable)
                {
                    g.tooltips.Add(pos, PMod.glossaries["Breakable"]);
                }

                if (singleUse)
                {
                    g.tooltips.Add(pos, PMod.glossaries["SingleUse"]);
                }
                if (IsTemporary)
                {
                    g.tooltips.Add(pos, PMod.glossaries["Temp"]);
                }
            }
            g.Pop();

            Color color = new Color(1.0, 1.0, 1.0, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);

            if (singleUse)
            {
                Draw.Sprite(PMod.sprites["icon_single"].Sprite, v.x-1, v.y+14, flipX: false, flipY: false, 0.0, null, null, null, null, color);
            }
        }

        public Vec GetPartPos(Vec v, Vec worldPos, int localX, Ship ship)
        {
           return v + worldPos + new Vec((xLerped ?? ((double)localX)) * 16.0, -32.0 + (ship.isPlayerShip ? offset.y : (1.0 + (0.0 - offset.y))));
        }

        public virtual void Remove(State s)
        {
            int index = s.ship.parts.IndexOf(this);

            if (index < s.ship.parts.Count / 2)
            {
                s.ship.x += 1;
                s.ship.xLerped = s.ship.x;

                foreach (var part in s.ship.parts)
                {
                    part.xLerped -= 1;
                }
            }

            s.ship.parts.Remove(this);

            if (s.route is Combat c)
            {
                foreach (var item in s.ship.parts)
                {
                    if (item is PartSelene ps)
                    {
                        ps.ShipWasModified(s.ship, s, c);
                    }
                }
            }
        }

        public virtual void OnTurnStart(State s, Combat c)
        {

        }

        public virtual void OnCombatEnd(State s)
        {

        }

        public virtual void OnTurnEnd(State s, Combat c)
        {

        }

        public virtual void OnHit(State s, Combat c)
        {
        }

        public virtual void AfterDroneShift(State s, Combat c, int direction)
        {

        }

        public virtual void AfterPlayerMove(State s, Combat c, int direction)
        {

        }
    }
}
