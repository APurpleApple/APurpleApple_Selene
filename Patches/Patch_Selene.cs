using APurpleApple.Selene.Artifacts;
using APurpleApple.Selene.ExternalAPIs;
using APurpleApple.Selene.Parts;
using FSPRO;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Patches
{
    [HarmonyPatch]
    internal static class Patch_Selene
    {
        [HarmonyPatch(typeof(Combat), nameof(Combat.RenderDrones)), HarmonyPostfix]
        public static void DrawDrone(G g, Combat __instance)
        {
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;
            artifact_selene.Render(g, __instance);
        }

        [HarmonyPatch(typeof(ADroneMove), nameof(ADroneMove.Begin)), HarmonyPostfix]
        public static void MoveDroneWithDroneshift(G g, State s, Combat c, ADroneMove __instance)
        {
            Artifact_Selene? artifact_selene = s.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;

            artifact_selene.droneX += __instance.dir;
        }

        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin)), HarmonyPostfix]
        public static void DestroyTempCannonsOnAttack(G g, State s, Combat c, AAttack __instance)
        {
            if (__instance.targetPlayer) return;
            int? x = __instance.GetFromX(s, c);
            if (!x.HasValue) return;

            Part? part = g.state.ship.GetPartAtLocalX(x.Value);
            if (part == null) return;

            if (part is not TemporaryGun tempGun) return;
            if (g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiCannonVolley) return;
            if (!tempGun.singleUse) return;

            if (s.ship.Get(PMod.statuses["reinforce"].Status) > 0)
            {
                c.QueueImmediate(new AStatus() { status = PMod.statuses["reinforce"].Status, statusAmount = -1, targetPlayer = true });
                return;
            }
            tempGun.Destroy(s, c);
        }

        [HarmonyPatch(typeof(ASpawn), nameof(ASpawn.Begin)), HarmonyPostfix]
        public static void DestroyTempBaysOnSpawn(G g, State s, Combat c, ASpawn __instance)
        {
            if (!__instance.fromPlayer) return;

            Part? part = g.state.ship.GetPartAtLocalX(__instance.fromX ?? 0);
            if (part == null) return;

            if (part is not TemporaryBay tempBay) return;
            if (g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiBayVolley) return;
            if (!tempBay.singleUse) return;

            if (s.ship.Get(PMod.statuses["reinforce"].Status) > 0)
            {
                c.QueueImmediate(new AStatus() { status = PMod.statuses["reinforce"].Status, statusAmount = -1, targetPlayer = true });
                return;
            }
            tempBay.Destroy(s, c);
        }

        [HarmonyPatch(typeof(Combat), nameof(Combat.RenderHintsUnderlay)), HarmonyPostfix]
        public static void RenderAddPartHint(G g, Combat __instance)
        {
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null || !artifact_selene.hilight) return;

            if (!__instance.ShouldDrawPlayerUI(g) || !__instance.PlayerCanAct(g.state)) return;

            int insertIndex = artifact_selene.droneX - g.state.ship.x;

            Rect? rect = default(Rect) + Combat.arenaPos;
            Vec xy = g.Push(null, rect).rect.xy;
            Vec v = xy + __instance.GetCamOffset();
            Color value = Colors.attackStatusHintPlayer;

            Vec vec = new Vec(g.state.ship.xLerped * 16.0) + FxPositions.WayBack(insertIndex, false) - new Vec(7.0);
            Vec vec2 = new Vec(g.state.ship.xLerped * 16.0) + FxPositions.Hull(insertIndex, true) + new Vec(8.0, 50.0);
            vec = vec.round();
            vec2 = vec2.round();

            Rect r = Rect.FromPoints(vec, vec2);
            Draw.Rect(v.x + r.x, v.y + r.y, r.w, r.h, value, BlendMode.Screen);

            if (insertIndex < -1 || insertIndex > g.state.ship.parts.Count)
            {
                g.Pop();
                artifact_selene.hilight = false;
                return;
            }

            if (insertIndex < g.state.ship.parts.Count / 2)
            {
                Rect shipRect = g.state.ship.GetShipRect();
                Draw.Sprite(PMod.sprites["selene_hint_ship_widden"].Sprite, v.x + shipRect.x - 18, v.y + shipRect.y-10, color : value, blend: BlendMode.Screen);
            }
            else
            {
                Rect shipRect = g.state.ship.GetShipRect();
                Draw.Sprite(PMod.sprites["selene_hint_ship_widden"].Sprite, v.x + shipRect.x2, v.y + shipRect.y-10, flipX: true, color: value, blend: BlendMode.Screen);
            }

            g.Pop();
            artifact_selene.hilight = false;
        }


        [HarmonyPatch(typeof(Ship), nameof(Ship.DrawTopLayer)), HarmonyPostfix]
        public static void DrawParts(Ship __instance, G __0, Vec __1, Vec __2)
        {
            Vec worldPos = __2;
            Vec v = __1;
            G g = __0;
            for (int i = 0; i < __instance.parts.Count; i++)
            {
                SelenePart? part = __instance.parts[i] as SelenePart;
                if (part == null || part.isRendered == false) continue;

                Vec vec2 = worldPos + new Vec((part.xLerped ?? ((double)i)) * 16.0, -32.0 + (__instance.isPlayerShip ? part.offset.y : (1.0 + (0.0 - part.offset.y))));
                Vec vec3 = v + vec2;

                part.Render(g, vec3, i);
            }
        }

        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin)), HarmonyPostfix]
        public static void ConstructorDroneDodgeAnim(AAttack __instance, Combat c, State s)
        {
            if (!__instance.targetPlayer) return;
            Artifact_Selene? art = s.EnumerateAllArtifacts().FirstOrDefault((a)=> a is Artifact_Selene) as Artifact_Selene;
            if (art == null) return;
            int? num = __instance.GetFromX(s, c);
            RaycastResult? raycastResult = (__instance.fromDroneX.HasValue ? CombatUtils.RaycastGlobal(c, s.ship, fromDrone: true, __instance.fromDroneX.Value) : (num.HasValue ? CombatUtils.RaycastFromShipLocal(s, c, num.Value, true) : null));
            if (raycastResult == null) return;
            if (art.droneX != raycastResult.worldX) return;
            if (raycastResult.hitDrone || raycastResult.hitShip) return;

            art.droneXLerped += art.dodgeLeft ? -1.5 : 1.5;
            art.dodgeLeft = !art.dodgeLeft;
        }

        [HarmonyPatch(typeof(AEnemyTurnAfter), nameof(AEnemyTurnAfter.Begin)), HarmonyPrefix]
        public static void NoEmptyFix(G g, State s, Combat c)
        {
            for (int i = 0; i < s.ship.parts.Count; i++)
            {
                if (s.ship.parts[i] is CloakedPart cp)
                {
                    cp.OnTurnStart(s, c);
                }
            }
        }

        [HarmonyPatch(typeof(Ship), nameof(Ship.ModifyDamageDueToParts)), HarmonyPostfix]
        public static void ReduceDamageToPartProtectedByShield(Ship __instance, State s, Combat c, Part part, ref int __result)
        {
            int partLocalX = __instance.parts.IndexOf(part);
            int protection = 0;
            for (int i = int.Max(partLocalX -1, 0); i < int.Min(partLocalX +2, __instance.parts.Count); i++)
            {
                if (__instance.parts[i] is ShieldProjector sp)
                {
                    protection = int.Max(sp.blocked, protection);
                }
            }

            __result -= protection;
            if (protection > 0)
            {
                c.fx.Add(new ShieldHit
                {
                    pos = FxPositions.Shield(__instance.x + partLocalX, __instance == s.ship)
                });
                ParticleBursts.ShieldImpact(MG.inst.g, FxPositions.Shield(__instance.x + partLocalX, __instance == s.ship), __instance == s.ship);
            }
        }

        [HarmonyPatch(typeof(Ship), nameof(Ship.RenderPartUI)), HarmonyPostfix]
        public static void DrawPartUI(Ship __instance, G g, Combat? combat, Part part, int localX, string keyPrefix, bool isPreview)
        {
            if (part is not SelenePart sp || sp.isRendered == false) return; 
            Vec vec = new Vec(localX * 16);
            int num = (isPreview ? 25 : 34);
            if (__instance.isPlayerShip)
            {
                vec.y -= num - 6;
            }

            Rect rect = new Rect(vec.x - 1.0, vec.y, 17.0, num);
            Rect value = rect;
            value.h -= 8.0;
            if (!__instance.isPlayerShip)
            {
                value.y += 8.0;
            }

            Box box = g.Push(new UIKey((UK.part), localX, "selenePart"), rect, value);
            Vec xy = box.rect.xy;

            if (box.IsHover())
            {
                Vec pos = xy + new Vec(16.0);

                foreach (var item in sp.GetTooltips())
                {
                    g.tooltips.Add(pos, item);
                }

                if (part.invincible)
                {
                    g.tooltips.Add(pos, new TTGlossary("parttrait.invincible"));
                }
                else
                {
                    if (part.damageModifier == PDamMod.armor)
                    {
                        g.tooltips.Add(pos, new TTGlossary("parttrait.armor"));
                    }

                    if (part.damageModifier == PDamMod.weak)
                    {
                        g.tooltips.Add(pos, new TTGlossary("parttrait.weak"));
                    }

                    if (part.damageModifier == PDamMod.brittle && !part.brittleIsHidden)
                    {
                        g.tooltips.Add(pos, new TTGlossary("parttrait.brittle"));
                    }
                }

                if (part.stunModifier == PStunMod.stunnable)
                {
                    g.tooltips.Add(pos, new TTGlossary("parttrait.stunnable"));
                }

                if (part.stunModifier == PStunMod.unstunnable)
                {
                    g.tooltips.Add(pos, new TTGlossary("parttrait.unstunnable"));
                }

                if (part.stunModifier == PStunMod.breakable)
                {
                    g.tooltips.Add(pos, PMod.glossaries["Breakable"]);
                }

                if (sp.singleUse)
                {
                    g.tooltips.Add(pos, PMod.glossaries["SingleUse"]);
                }
                if(sp.removedOnCombatEnd)
                {
                    g.tooltips.Add(pos, PMod.glossaries["Temp"]);
                }
            }
            sp.RenderUI(g, xy);
            g.Pop();
        }
    }
}
