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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            if (part is not PartGun tempGun) return;
            if (g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiCannonVolley) return;
            if (__instance.fromDroneX != null) return;
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

            if (part is not PartBay tempBay) return;
            if (g.state.ship.GetPartTypeCount(PType.missiles) > 1 && !__instance.multiBayVolley) return;
            if (!tempBay.singleUse) return;

            if (s.ship.Get(PMod.statuses["reinforce"].Status) > 0)
            {
                c.QueueImmediate(new AStatus() { status = PMod.statuses["reinforce"].Status, statusAmount = -1, targetPlayer = true });
                return;
            }
            tempBay.Destroy(s, c);
        }

        [HarmonyPatch(typeof(ABreakPart), nameof(ABreakPart.Begin)), HarmonyPrefix]
        public static void DestroyBreakableParts(ABreakPart __instance, G g, State s, Combat c)
        {
            Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
            if (ship == null)
                return;
            // ISSUE: method pointer
            Part? part = ship.parts.Find(p=>p.uuid == __instance.uuid);

            if (part == null) return;

            /*
            if (ship.Get(PMod.statuses["plating"].Status) > 0)
            {
                c.QueueImmediate(new AStatus() { status = PMod.statuses["plating"].Status, statusAmount = -1, targetPlayer = __instance.targetPlayer });
                __instance.uuid = -1;
                return;
            }*/

            if (part is PartSelene ps)
            {
                __instance.uuid = -1;
            }
        }

        public static bool? magnetizeHint = null;

        [HarmonyPatch(typeof(Combat), nameof(Combat.RenderHintsUnderlay)), HarmonyPostfix]
        public static void RenderMagnetizeHint(G g, Combat __instance)
        {
            if (magnetizeHint.HasValue)
            {
                Vec v = g.Peek().xy + __instance.GetCamOffset() + Combat.arenaPos + new Vec((g.state.ship.x + g.state.ship.parts.Count / 2) * 16, 49);

                Color color = magnetizeHint.Value ? Colors.attackStatusHintPlayer : Colors.attackStatusHint;

                if (!magnetizeHint.Value)
                {
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_push"].Sprite, v.x - 14, v.y, flipX: false, blend: BlendMode.Screen, color: color);
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_push"].Sprite, v.x + 16, v.y, flipX: true, blend: BlendMode.Screen, color: color);
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_push"].Sprite, v.x - 14, v.y + 70, flipX: false, blend: BlendMode.Screen, color: color);
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_push"].Sprite, v.x + 16, v.y + 70, flipX: true, blend: BlendMode.Screen, color: color);
                    Draw.Rect(0, v.y, v.x, 30, color, BlendMode.Screen);
                    Draw.Rect(v.x + 16, v.y, 500, 30, color, BlendMode.Screen);
                    Draw.Rect(0, v.y + 70, v.x, 30, color, BlendMode.Screen);
                    Draw.Rect(v.x + 16, v.y + 70, 500, 30, color, BlendMode.Screen);
                }
                else
                {
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_pull"].Sprite, v.x - 14, v.y, flipX: true, blend: BlendMode.Screen, color: color);
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_pull"].Sprite, v.x + 16, v.y, flipX: false, blend: BlendMode.Screen, color: color);
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_pull"].Sprite, v.x - 14, v.y + 70, flipX: true, blend: BlendMode.Screen, color: color);
                    Draw.Sprite(PMod.sprites["selene_hint_magnetize_pull"].Sprite, v.x + 16, v.y + 70, flipX: false, blend: BlendMode.Screen, color: color);
                    Draw.Rect(0, v.y, v.x - 14, 30, color, BlendMode.Screen);
                    Draw.Rect(v.x + 30, v.y, 500, 30, color, BlendMode.Screen);
                    Draw.Rect(0, v.y + 70, v.x - 14, 30, color, BlendMode.Screen);
                    Draw.Rect(v.x + 30, v.y + 70, 500, 30, color, BlendMode.Screen);
                }



                magnetizeHint = null;
            }
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
                Draw.Sprite(PMod.sprites["selene_hint_ship_widden"].Sprite, v.x + shipRect.x - 18, v.y + shipRect.y - 10, color: value, blend: BlendMode.Screen);
            }
            else
            {
                Rect shipRect = g.state.ship.GetShipRect();
                Draw.Sprite(PMod.sprites["selene_hint_ship_widden"].Sprite, v.x + shipRect.x2, v.y + shipRect.y - 10, flipX: true, color: value, blend: BlendMode.Screen);
            }

            g.Pop();
            artifact_selene.hilight = false;
        }

        /*
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
        }*/

        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin)), HarmonyPrefix]
        public static void ConstructorDroneDodgeAnim(AAttack __instance, Combat c, State s)
        {
            if (!__instance.targetPlayer) return;
            Artifact_Selene? art = s.EnumerateAllArtifacts().FirstOrDefault((a) => a is Artifact_Selene) as Artifact_Selene;
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
                if (s.ship.parts[i] is PartCloakedPart cp)
                {
                    cp.OnTurnStart(s, c);
                }
            }
        }

        [HarmonyPatch(typeof(AMove), nameof(AMove.Begin)), HarmonyPostfix]
        public static void AfterPlayerMove(AMove __instance, G g, State s, Combat c)
        {
            if (!__instance.targetPlayer) return;
            if (__instance.dir == 0) return;

            for (int i = 0; i < s.ship.parts.Count; i++)
            {
                if (s.ship.parts[i] is PartSelene ps)
                {
                    ps.AfterPlayerMove(s, c, __instance.dir);
                }
            }
        }

        [HarmonyPatch(typeof(ADroneMove), nameof(ADroneMove.Begin)), HarmonyPostfix]
        public static void AfterDroneshift(AMove __instance, G g, State s, Combat c)
        {
            if (__instance.dir == 0) return;

            for (int i = 0; i < s.ship.parts.Count; i++)
            {
                if (s.ship.parts[i] is PartSelene ps)
                {
                    ps.AfterDroneShift(s, c, __instance.dir);
                }
            }
        }

        [HarmonyPatch(typeof(Ship), nameof(Ship.ModifyDamageDueToParts)), HarmonyPostfix]
        public static void ReduceDamageToPartProtectedByShield(Ship __instance, State s, Combat c, Part part, ref int __result)
        {
            int partLocalX = __instance.parts.IndexOf(part);
            int protection = 0;
            for (int i = int.Max(partLocalX - 1, 0); i < int.Min(partLocalX + 2, __instance.parts.Count); i++)
            {
                if (__instance.parts[i] is PartShieldProjector sp)
                {
                    protection = int.Max(sp.blocked, protection);
                    sp.shieldPulse = 0;
                }
            }

            if (s.EnumerateAllArtifacts().Any(a=>a is Artifact_BreakArmor))
            {
                if (part.stunModifier == PStunMod.breakable)
                {
                    __result -= 1;
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

        [HarmonyPatch(typeof(AShuffleShip), nameof(AShuffleShip.Begin)), HarmonyPostfix]
        public static void AfterShipShuffle(AShuffleShip __instance, G g, State s, Combat c)
        {
            if (!__instance.targetPlayer) return;
            foreach (var item in s.ship.parts)
            {
                if (item is PartSelene ps)
                {
                    ps.ShipWasModified(s.ship, s, c);
                }
            }
        }
    }
}
