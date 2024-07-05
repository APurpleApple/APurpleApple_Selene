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
    internal class Artifact_Selene : Artifact, IModArtifact
    {
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
        public Part? grabbedPart;
        public string? grabbedPartSkin;

        public bool hilight = false;

        public double animAlpha = 1;
        public EAnim anim = EAnim.Rest;

        public void Render(G g, Combat c)
        {
            droneXLerped = Mutil.SnapLerp(droneXLerped, droneX, 25.0, g.dt);

            double speed = 2;
            animAlpha = Mutil.SnapLerp(animAlpha, 1, 4 * speed, g.dt);

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
                default:
                    break;
            }

            craneExtensionLerped = Mutil.SnapLerp(craneExtensionLerped, craneExtension, 8 * speed, g.dt);
            craneAngleLerped = Mutil.SnapLerp(craneAngleLerped, craneAngle, double.Pi * speed, g.dt);
            clawAngleLerped = Mutil.SnapLerp(clawAngleLerped, clawAngle, double.Pi * speed, g.dt);
            clawOpeningLerped = Mutil.SnapLerp(clawOpeningLerped, clawOpening, 5 * speed, g.dt);

            Rect? rect = default(Rect) + Combat.arenaPos + c.GetCamOffset();
            g.Push(null, rect);

            double x = rect.Value.x + droneXLerped * 16 + 4.5 + 8;
            double y = rect.Value.y + 165;

            Draw.Sprite(PMod.sprites["selene_cdrone_body"].Sprite, x, y, originPx: new Vec(16.5,32));

            double craneTipX = x - double.Sin(craneAngleLerped) * -(craneExtensionLerped+10);
            double craneTipY = y - 12.5 + double.Cos(craneAngleLerped) * -(craneExtensionLerped+10);

            double clawOffsetX = double.Cos(clawAngleLerped) * clawOpeningLerped;
            double clawOffsetY = double.Sin(clawAngleLerped) * clawOpeningLerped;

            Draw.Sprite(PMod.sprites["selene_cdrone_claw"].Sprite, craneTipX + clawOffsetX, craneTipY + clawOffsetY, originPx: new Vec(2.5,0), flipX: clawOffsetX > 0);
            Draw.Sprite(PMod.sprites["selene_cdrone_claw"].Sprite, craneTipX - clawOffsetX, craneTipY - clawOffsetY, originPx: new Vec(2.5,0), flipX: clawOffsetX < 0);

            if (anim == EAnim.PlacePart && grabbedPartSkin != null)
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
                        }
                        break;
                    case EAnim.PickUpPart:
                        animAlpha = 0;
                        anim = EAnim.PlacePart;
                        
                        break;
                }
            }
        }

        public override void OnCombatStart(State state, Combat combat)
        {
            droneX = state.ship.x;
        }

        public override void OnCombatEnd(State state)
        {
            for (int i = state.ship.parts.Count -1; i >= 0 ; i--)
            {
                if (state.ship.parts[i] is SelenePart sp && sp.removedOnCombatEnd)
                {
                    sp.Remove(state);
                }
            }
        }

        public enum EAnim
        {
            Rest = 0,
            PlacePart = 1,
            PickUpPart = 2,
        }
    }
}
