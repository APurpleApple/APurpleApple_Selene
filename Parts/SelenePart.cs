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
    public class SelenePart : Part
    {
        public bool removedOnCombatEnd = false;
        public bool singleUse = false;
        public Spr icon;
        public string tooltip = "";
        public bool isRendered = false;

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

        public virtual void Render(G g, Vec v, int i)
        {

        }

        public virtual void RenderUI(G g, Vec v)
        {
            Color color = new Color(1.0, 1.0, 1.0, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);

            if (singleUse)
            {
                Draw.Sprite(PMod.sprites["icon_single"].Sprite, v.x, v.y + 10, flipX: false, flipY: false, 0.0, null, null, null, null, color);
            }
            
            if (removedOnCombatEnd)
            {
                Draw.Sprite(SSpr.icons_temporary, v.x + 8, v.y + 10, flipX: false, flipY: false, 0.0, null, null, null, null, color);
            }
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
