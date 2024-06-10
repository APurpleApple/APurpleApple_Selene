using APurpleApple.Selene.Artifacts;
using APurpleApple.Selene.ExternalAPIs;
using FSPRO;
using HarmonyLib;
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
            artifact_selene.droneXLerped = Mutil.SnapLerp(artifact_selene.droneXLerped, artifact_selene.droneX, 25.0, g.dt);

            Rect? rect = default(Rect) + Combat.arenaPos + __instance.GetCamOffset();
            g.Push(null, rect);

            Draw.Sprite(PMod.sprites["selene_constructorDrone"].ObtainTexture(), rect.Value.x + artifact_selene.droneXLerped * 16 + 20, rect.Value.y + 106 + 32);

            g.Pop();
        }

        [HarmonyPatch(typeof(ADroneMove), nameof(ADroneMove.Begin)), HarmonyPostfix]
        public static void MoveDroneWithDroneshift(G g, State s, Combat c, ADroneMove __instance)
        {
            Artifact_Selene? artifact_selene = s.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
            if (artifact_selene == null) return;

            artifact_selene.droneX += __instance.dir;
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
    }
}
