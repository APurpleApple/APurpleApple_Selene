using APurpleApple.Selene.ExternalAPIs;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Artifacts
{
    internal class Artifact_Selene : Artifact, IModArtifact, IDroneShiftHook, IHookPriority, IEvadeHook
    {
        public bool dodgeLeft = false;

        public static Artifact_Selene? Find(State s)
        {
            return s.EnumerateAllArtifacts().FirstOrDefault(a => a is Artifact_Selene) as Artifact_Selene;
        }
        public static void Register(IModHelper helper)
        {
            Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
            helper.Content.Artifacts.RegisterArtifact(type.Name, new()
            {
                ArtifactType = type,
                Meta = new()
                {
                    owner = PMod.decks["selene"].Deck,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = PMod.sprites["selene_artifact"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_Selene", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_Selene", "description"]).Localize
            });
        }

        public bool? IsDroneShiftPossible(State state, Combat combat, DroneShiftHookContext context)
        {
            if (context == DroneShiftHookContext.Rendering && state.ship.Get(Status.droneShift) > 0)
            {
                return true;
            }

            return null;
        }

        public int droneX = 0;
        public double droneXLerped = 0;
        public int craneExtension = 0;
        public double craneExtensionLerped = 0;
        public double craneAngle = 0;
        public double craneAngleLerped = 0;
        public double clawAngle = 0;
        public double clawAngleLerped = 0;
        public double clawOpening = 0;
        public double clawOpeningLerped = 0;

        public bool placeLeft = false;
        public PartSelene? grabbedPart;
        public string? grabbedPartSkin;

        public bool hilight = false;

        public double animAlpha = 1;
        public EAnim anim = EAnim.Rest;

        public double HookPriority => 3_000_000_000;

        public void Render(G g, Combat c)
        {
            droneXLerped = Mutil.SnapLerp(droneXLerped, droneX, 10.0, g.dt);

            double speed = 2;

            switch (anim)
            {
                case EAnim.Rest:
                    craneAngle = 0;
                    craneExtension = 0;
                    clawAngle = 0;
                    clawOpening = 8;
                    break;
                case EAnim.PlacePart:
                    craneAngle = placeLeft ? -double.Pi * .16 : double.Pi * .16;
                    craneExtension = 10;
                    clawAngle = 0;
                    clawOpening = 8;
                    break;
                case EAnim.PickUpPart:
                    craneAngle = double.Pi;
                    craneExtension = 16;
                    clawAngle = craneAngle;
                    clawOpening = 8;
                    break;
                case EAnim.PickUpPartForThrow:
                    craneAngle = double.Pi;
                    craneExtension = 16;
                    clawAngle = craneAngle;
                    clawOpening = 8;
                    break;
                case EAnim.Throw:
                    craneAngle = placeLeft ? -double.Pi * .16 : double.Pi * .16;
                    craneExtension = 16;
                    clawAngle = craneAngle;
                    clawOpening = 8;
                    speed *= 2;
                    break;
            }

            animAlpha = Mutil.SnapLerp(animAlpha, 1, 4 * speed, g.dt);

            craneExtensionLerped = Mutil.SnapLerp(craneExtensionLerped, craneExtension, 8 * speed, g.dt);
            craneAngleLerped = Mutil.SnapLerp(craneAngleLerped, craneAngle, double.Pi * speed, g.dt);
            clawAngleLerped = Mutil.SnapLerp(clawAngleLerped, clawAngle, double.Pi * speed, g.dt);
            clawOpeningLerped = Mutil.SnapLerp(clawOpeningLerped, clawOpening, 5 * speed, g.dt);

            Rect? rect = default(Rect) + Combat.arenaPos + c.GetCamOffset();
            g.Push(null, rect);

            double x = rect.Value.x + droneXLerped * 16 + 4.5 + 8;
            double y = rect.Value.y + 165;

            bool hasGotArmsRace = MG.inst.g.state.EnumerateAllArtifacts().Any((a) => a is Artifact_SeleneV2);
            Draw.Sprite(PMod.sprites[hasGotArmsRace ? "selene_cdrone_body_gun" : "selene_cdrone_body"].Sprite, x, y, originPx: new Vec(16.5,32));

            double craneTipX = x - double.Sin(craneAngleLerped) * -(craneExtensionLerped+10);
            double craneTipY = y - 12.5 + double.Cos(craneAngleLerped) * -(craneExtensionLerped+10);

            double clawOffsetX = double.Cos(clawAngleLerped) * clawOpeningLerped;
            double clawOffsetY = double.Sin(clawAngleLerped) * clawOpeningLerped;

            Draw.Sprite(PMod.sprites["selene_cdrone_claw"].Sprite, craneTipX + clawOffsetX, craneTipY + clawOffsetY, originPx: new Vec(2.5,0), flipX: clawOffsetX > 0);
            Draw.Sprite(PMod.sprites["selene_cdrone_claw"].Sprite, craneTipX - clawOffsetX, craneTipY - clawOffsetY, originPx: new Vec(2.5,0), flipX: clawOffsetX < 0);

            if ((anim == EAnim.PlacePart || anim == EAnim.Throw) && grabbedPartSkin != null)
            {
                Spr? spr = DB.parts.GetOrNull(grabbedPartSkin) ?? DB.partsOff.GetOrNull(grabbedPartSkin);
                Draw.Sprite(spr, craneTipX, craneTipY, originPx: new Vec(8.5, 32.5), rotation: clawAngleLerped);
            }

            Draw.Sprite(PMod.sprites["selene_cdrone_rail"].Sprite, craneTipX, craneTipY, originPx: new Vec(10.5,1.5), rotation: clawAngleLerped);

            Draw.Sprite(PMod.sprites["selene_cdrone_arm_small"].Sprite, x, y - 12.5, originPx: new Vec(1.5, 12 + craneExtensionLerped), rotation: craneAngleLerped);
            Draw.Sprite(PMod.sprites["selene_cdrone_arm_med"].Sprite, x, y - 12.5, originPx: new Vec(2.5, 10 + craneExtensionLerped * .5), rotation: craneAngleLerped);
            Draw.Sprite(PMod.sprites["selene_cdrone_arm_big"].Sprite, x, y - 12.5, originPx: new Vec(3.5, 8), rotation: craneAngleLerped);

            Draw.Sprite(PMod.sprites["selene_cdrone_top"].Sprite, x, y - 12.5, originPx: new Vec(3.5,3.5));

            g.Pop();

            if (animAlpha == 1)
            {
                switch (anim)
                {
                    case EAnim.PlacePart:
                        animAlpha = 0;
                        anim = EAnim.Rest;
                        if (grabbedPart != null)
                        {
                            grabbedPart.skin = grabbedPartSkin;
                            grabbedPart.isRendered = true;
                        }
                        break;
                    case EAnim.PickUpPart:
                        animAlpha = 0;
                        anim = EAnim.PlacePart;
                        break;
                    case EAnim.PickUpPartForThrow:
                        animAlpha = 0;
                        anim = EAnim.Throw;
                        break;
                    case EAnim.Throw:
                        animAlpha = 0;
                        anim = EAnim.Rest;
                        grabbedPart = null;
                        break;
                }
            }
        }

        public override void OnCombatStart(State state, Combat combat)
        {
            droneX = state.ship.x + state.ship.parts.Count / 2;
        }


        public override void OnCombatEnd(State state)
        {
            foreach (PartSelene item in state.ship.parts.Where(p => p is PartSelene).ToList())
            {
                item.OnCombatEnd(state);
            }
        }

        public override void OnTurnStart(State state, Combat combat)
        {
            foreach (PartSelene item in state.ship.parts.Where(p=>p is PartSelene).ToList())
            {
                item.OnTurnStart(state, combat);
            }
        }

        public override void OnTurnEnd(State state, Combat combat)
        {
            foreach (PartSelene item in state.ship.parts.Where(p => p is PartSelene).ToList())
            {
                item.OnTurnEnd(state, combat);
            }
        }

        public override void OnPlayerTakeNormalDamage(State state, Combat combat, int rawAmount, Part? part)
        {
            if (part is PartSelene sp)
            {
                sp.OnHit(state, combat);
                if (sp.stunModifier == PStunMod.breakable)
                {
                    if(state.ship.Get(PMod.statuses["plating"].Status) > 0)
                    {
                        combat.QueueImmediate(new AStatus() { status = PMod.statuses["plating"].Status, statusAmount = -1, targetPlayer = true });
                        return;
                    }
                    sp.Destroy(state, combat);
                }
            }
        }

        public enum EAnim
        {
            Rest = 0,
            PlacePart = 1,
            PickUpPart = 2,
            PickUpPartForThrow = 3,
            Throw = 4,
        }

    }
}
