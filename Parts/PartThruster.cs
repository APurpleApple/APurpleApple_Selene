using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.VFXs;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mono.CompilerServices.SymbolWriter.CodeBlockEntry;

namespace APurpleApple.Selene
{
    public class PartThruster : PartSelene, IModPart
    {
        public bool upgraded = false;

        public PartThruster()
        {
            type = PMod.pTypes[upgraded ? (flip ? "ThrusterV2Right" : "ThrusterV2Left") : (flip ? "ThrusterRight" : "ThrusterLeft")].PartType;
        }

        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes[upgraded ? (flip ? "ThrusterV2Right" : "ThrusterV2Left") : (flip ? "ThrusterRight" : "ThrusterLeft")].PartType;
            return base.GetTooltips(s);
        }

        public override void OnTurnEnd(State s, Combat c)
        {
            c.Queue(new AVFX() { fx = new VFX_Thruster() { follow = this}, timer = 0.1 });
            c.Queue(new AMove() { dir = (upgraded ? 2 : 1) * (flip ? -1 : 1), targetPlayer = true });
        }

        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("ThrusterLeft",
                () => PMod.sprites["icon_part_thruster_left"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterLeft", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterLeft", "description"]),
                helper);

            PMod.Instance.RegisterPartTypeAndGlossary("ThrusterRight",
                () => PMod.sprites["icon_part_thruster_right"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterRight", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterRight", "description"]),
                helper);

            PMod.Instance.RegisterPartTypeAndGlossary("ThrusterV2Left",
                () => PMod.sprites["icon_part_thruster_v2_left"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterV2Left", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterV2Left", "description"]),
                helper);

            PMod.Instance.RegisterPartTypeAndGlossary("ThrusterV2Right",
                () => PMod.sprites["icon_part_thruster_v2_right"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterV2Right", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "ThrusterV2Right", "description"]),
                helper);

            PMod.parts["selene_thruster"] = helper.Content.Ships.RegisterPart("selene_thruster", new PartConfiguration() { Sprite = PMod.sprites["selene_part_thruster"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
            PMod.parts["selene_thrusterV2"] = helper.Content.Ships.RegisterPart("selene_thrusterV2", new PartConfiguration() { Sprite = PMod.sprites["selene_part_thrusterV2"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        }
    }
}
