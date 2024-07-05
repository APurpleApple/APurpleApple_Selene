using APurpleApple.Selene.Artifacts;
using APurpleApple.Selene.ExternalAPIs;
using FSPRO;
using HarmonyLib;
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

        [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage)), HarmonyPostfix]
        public static void DestroyBreakableOnHit(State s, Combat c, Ship __instance, int? maybeWorldGridX)
        {
            if (__instance != s.ship) return;

            object? obj = null;
            if (maybeWorldGridX.HasValue)
            {
                int valueOrDefault = maybeWorldGridX.GetValueOrDefault();
                obj = __instance.GetPartAtWorldX(valueOrDefault);
            }

            Part? part = (Part?)obj;

            if (part != null && part is SelenePart sp && sp.stunModifier == PStunMod.breakable)
            {
                sp.Destroy(s, c);
            }
        }


        //[HarmonyPatch(typeof(Kokoro.VanillaMidrowCheckDroneShiftHook), nameof(Kokoro.VanillaMidrowCheckDroneShiftHook.IsDroneShiftPossible)), HarmonyPostfix]
        public static void IsDroneShiftPossible()
        {
            Console.WriteLine("heyeuhqdkshl");
        }

        [HarmonyPatch(typeof(Combat), nameof(Combat.DoDroneShift)), HarmonyPostfix]
        public static void DoDroneShiftAnyway(G g, int dir, Combat __instance)
        {
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;

            if (g.state.route is Combat combat && (combat.stuff.Count == 0))
            {
                __instance.Queue(new ADroneMove
                {
                    dir = dir
                });
                g.state.ship.Add(Status.droneShift, -1);
            }
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
            tempGun.Destroy(s, c);
        }

        [HarmonyPatch(typeof(Combat), nameof(Combat.RenderDroneShiftButtons)), HarmonyPostfix]
        public static void RenderDroneShiftAnyway(G g, Combat __instance)
        {
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;

            if (__instance.isPlayerTurn && g.state.ship.hull > 0 && __instance.otherShip.hull > 0 && g.state.ship.Get(Status.droneShift) > 0 && (!(g.state.route is Combat combat) || (combat.stuff.Count == 0 )))
            {
                int num = 72;
                G g2 = g;
                UIKey uIKey = UK.btn_moveDrones_left;
                Rect rect = new Rect(Combat.cardCenter.x - (double)num - 22.0, 58.0, 22.0, 21.0);
                UIKey uIKey2 = uIKey;
                Spr spr = PlatformIcons.GetPlatform() switch
                {
                    Platform.Xbox => Spr.buttons_moveDrone_big,
                    Platform.PS => Spr.buttons_moveDrone_big,
                    _ => Spr.buttons_moveDrone,
                };
                Spr spr2 = PlatformIcons.GetPlatform() switch
                {
                    Platform.Xbox => Spr.buttons_moveDrone_big_on,
                    Platform.PS => Spr.buttons_moveDrone_big_on,
                    _ => Spr.buttons_moveDrone_on,
                };
                G g3 = g2;
                Rect rect2 = rect;
                UIKey key = uIKey2;
                Spr sprite = spr;
                Spr spriteHover = spr2;
                OnMouseDown onMouseDown = __instance;
                bool showAsPressed = !__instance.eyeballPeek && Input.GetGpHeld(Btn.BumperL);
                SharedArt.ButtonResult buttonResult = SharedArt.ButtonSprite(g3, rect2, key, sprite, spriteHover, null, null, inactive: false, flipX: true, flipY: false, onMouseDown, autoFocus: false, noHover: false, showAsPressed, gamepadUntargetable: true);
                if (buttonResult.isHover)
                {
                    __instance.isHoveringDroneMove = 2;
                }

                Spr? platformIcon = PlatformIcons.GetPlatformIcon(Btn.BumperL);
                double x2 = buttonResult.v.x + 19.0;
                double y = buttonResult.v.y + 6.0 + (double)((buttonResult.isHover || (!__instance.eyeballPeek && Input.GetGpHeld(Btn.BumperL))) ? 1 : 0);
                Color? color = Colors.moveDroneButtons.fadeAlpha(0.5);
                Vec? originRel = new Vec(1.0);
                Draw.Sprite(platformIcon, x2, y, flipX: false, flipY: false, 0.0, null, originRel, null, null, color);
                g2 = g;
                UIKey uIKey3 = UK.btn_moveDrones_right;
                rect = new Rect(Combat.cardCenter.x + (double)num + 1.0, 58.0, 18.0, 21.0);
                uIKey2 = uIKey3;
                spr = PlatformIcons.GetPlatform() switch
                {
                    Platform.Xbox => Spr.buttons_moveDrone_big,
                    Platform.PS => Spr.buttons_moveDrone_big,
                    _ => Spr.buttons_moveDrone,
                };
                Spr spr3 = PlatformIcons.GetPlatform() switch
                {
                    Platform.Xbox => Spr.buttons_moveDrone_big_on,
                    Platform.PS => Spr.buttons_moveDrone_big_on,
                    _ => Spr.buttons_moveDrone_on,
                };
                G g4 = g2;
                Rect rect3 = rect;
                UIKey key2 = uIKey2;
                Spr sprite2 = spr;
                Spr spriteHover2 = spr3;
                onMouseDown = __instance;
                showAsPressed = !__instance.eyeballPeek && Input.GetGpHeld(Btn.BumperR);
                SharedArt.ButtonResult buttonResult2 = SharedArt.ButtonSprite(g4, rect3, key2, sprite2, spriteHover2, null, null, inactive: false, flipX: false, flipY: false, onMouseDown, autoFocus: false, noHover: false, showAsPressed, gamepadUntargetable: true);
                if (buttonResult2.isHover)
                {
                    __instance.isHoveringDroneMove = 2;
                }

                Spr? platformIcon2 = PlatformIcons.GetPlatformIcon(Btn.BumperR);
                double x3 = buttonResult2.v.x + 3.0;
                double y2 = buttonResult2.v.y + 6.0 + (double)((buttonResult2.isHover || (!__instance.eyeballPeek && Input.GetGpHeld(Btn.BumperR))) ? 1 : 0);
                color = Colors.moveDroneButtons.fadeAlpha(0.5);
                originRel = new Vec(0.0, 0.0);
                Draw.Sprite(platformIcon2, x3, y2, flipX: false, flipY: false, 0.0, null, originRel, null, null, color);
            }
        }

        [HarmonyPatch(typeof(Combat), nameof(Combat.RenderHintsUnderlay)), HarmonyPostfix]
        public static void RenderAddPartHint(G g, Combat __instance)
        {
            Artifact_Selene? artifact_selene = g.state.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null || !artifact_selene.hilight) return;

            if (!__instance.ShouldDrawPlayerUI(g) || !__instance.PlayerCanAct(g.state)) return;

            int insertIndex = artifact_selene.droneX - g.state.ship.x;
            if (insertIndex < -1) return;
            if (insertIndex > g.state.ship.parts.Count) return;

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
                if (part == null) continue;

                Vec vec2 = worldPos + new Vec((part.xLerped ?? ((double)i)) * 16.0, -32.0 + (__instance.isPlayerShip ? part.offset.y : (1.0 + (0.0 - part.offset.y))));
                Vec vec3 = v + vec2;

                part.Render(g, vec3);
            }
        }

        [HarmonyPatch(typeof(Ship), nameof(Ship.RenderPartUI)), HarmonyPostfix]
        public static void DrawPartUI(Ship __instance, G g, Combat? combat, Part part, int localX, string keyPrefix, bool isPreview)
        {
            if (part is not SelenePart sp) return; 
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
                g.tooltips.AddGlossary(pos, DB.Join("part.", part.type.Key()));

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
