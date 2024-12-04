using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using HarmonyLib;
using Nickel;

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

        public virtual List<Tooltip>? GetTooltips(State s)
        {
            return null;
        }

        public virtual void Destroy(State s, Combat c)
        {
            active = false;
            EffectSpawnerExtension.PartExploding(s, GetPartRect(s));
            c.Queue(new ASeleneRemovePart() { uuid = this.uuid });
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

        public virtual bool DoVanillaRender(Ship ship, int localX, G g) => true;
        public virtual void Render(Ship ship, int localX, G g, Vec v, Vec worldPos) { }
       
        public virtual void RenderUI(Ship ship, G g, Combat? combat, int localX, string keyPrefix, bool isPreview, Vec v)
        {
            UIKey key = new UIKey(SUK.part, localX, keyPrefix);
            Box box = g.boxes.Find(b => b.key == key) ?? new Box();
            Vec xy = box.rect.xy;

            if (box.IsHover())
            {
                Vec pos = xy + new Vec(16.0);

                if (stunModifier == PStunMod.breakable)
                {
                    g.tooltips.tooltips.RemoveAll((tt)=> tt is TTGlossary ttg && ttg.key == "parttrait.breakable");
                    g.tooltips.Add(pos, PMod.glossaries["Breakable"]);
                }

                if (singleUse)
                {
                    g.tooltips.Add(pos, PMod.glossaries["SingleUse"]);
                }
            }

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
