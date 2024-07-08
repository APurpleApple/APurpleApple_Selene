using APurpleApple.Selene.CardActions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Patches
{
    [HarmonyPatch]
    internal class Patch_AInsertPart
    {

        [HarmonyPatch(typeof(Card), nameof(Card.RenderAction)), HarmonyPostfix]
        public static void RenderOversizedActionsPostfix(CardAction action, G g, bool dontDraw)
        {
            if (!dontDraw && action is ASeleneInsertPart attachAction)
            {
                int w = -15;
                if (attachAction.part.damageModifier != PDamMod.none)
                {
                    w -= 4;
                }

                Rect? rect = new Rect(w, -1);
                Vec xy = g.Push(null, rect).rect.xy;
                double x = xy.x;
                double y = xy.y;
                Color? color = (action.disabled ? Colors.disabledIconTint : new Color("ffffff"));
                Draw.Sprite(PMod.sprites["icon_attachPart"].Sprite, x, y, flipX: false, flipY: false, 0.0, null, null, null, null, color);
                Draw.Sprite(attachAction.part.icon, x + 9, y, flipX: false, flipY: false, 0.0, null, null, null, null, color);

                Spr? modSpr = null;

                if (attachAction.part.singleUse)
                {
                    modSpr = PMod.sprites["icon_single"].Sprite;

                    if (attachAction.part.removedOnCombatEnd)
                    {
                        modSpr = PMod.sprites["icon_singleTemp"].Sprite;

                        if (attachAction.part.stunModifier == PStunMod.breakable)
                        {
                            modSpr = PMod.sprites["icon_breakTempSingle"].Sprite;
                        }
                    }
                }
                else if (attachAction.part.removedOnCombatEnd)
                {
                    modSpr = SSpr.icons_temporary;

                    if (attachAction.part.stunModifier == PStunMod.breakable)
                    {
                        modSpr = PMod.sprites["icon_breakTemp"].Sprite;
                    }
                }
                else if (attachAction.part.stunModifier == PStunMod.breakable)
                {
                    modSpr = SSpr.icons_breakable;
                }

                if (modSpr != null)
                {
                    Draw.Sprite(modSpr, x + 18, y, flipX: false, flipY: false, 0.0, null, null, null, null, color);
                }

                switch (attachAction.part.damageModifier)
                {
                    case PDamMod.none:
                        break;
                    case PDamMod.weak:
                        Draw.Sprite(SSpr.icons_weak, x + 27, y, flipX: false, flipY: false, 0.0, null, null, null, null, color);
                        break;
                    case PDamMod.armor:
                        Draw.Sprite(SSpr.icons_armor, x + 27, y, flipX: false, flipY: false, 0.0, null, null, null, null, color);
                        break;
                    case PDamMod.brittle:
                        Draw.Sprite(SSpr.icons_brittle, x + 27, y, flipX: false, flipY: false, 0.0, null, null, null, null, color);
                        break;
                    default:
                        break;
                }
                g.Pop();
            }
        }
    }
}
