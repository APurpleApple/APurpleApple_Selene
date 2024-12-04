using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartShieldProjector : PartSelene, IModPart
    {
        public int blocked = 1;
        [JsonIgnore]
        public double shieldDeployAnim = 0;
        public double shieldPulse = 1;

        public override int RenderDepth => -2;

        public PartShieldProjector()
        {
            type = PMod.pTypes["Shield"].PartType;
        }

        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes["Shield"].PartType;
            return base.GetTooltips(s);
        }

        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary( "Shield",
                () => PMod.sprites["icon_part_shield"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "Shield", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "Shield", "description"]),
            helper);

            PMod.Instance.RegisterPartTypeAndGlossary("ShieldV2",
                () => PMod.sprites["icon_part_shield"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "ShieldV2", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "ShieldV2", "description"]),
            helper);

            PMod.parts["selene_shield"] = helper.Content.Ships.RegisterPart("selene_shield", new PartConfiguration() { Sprite = PMod.sprites["selene_part_shield"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
            PMod.parts["selene_shieldV2"] = helper.Content.Ships.RegisterPart("selene_shieldV2", new PartConfiguration() { Sprite = PMod.sprites["selene_part_shieldV2"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        }

        public override void Render(Ship ship, int localX, G g, Vec v, Vec worldPos)
        {
            if (isRendered)
            {
                v = GetPartPos(v, worldPos, localX, ship);
                shieldDeployAnim = Mutil.SnapLerp(shieldDeployAnim, 1, 10, g.dt);
                Draw.Sprite(PMod.sprites["fx_shield_" + (int)(shieldDeployAnim * 7)].Sprite, v.x + 7, v.y+3, originRel: new Vec(.5, 0));

                if (shieldPulse < 1)
                {
                    Draw.Sprite(PMod.sprites["fx_shieldImpact_" + (int)(shieldPulse * 5) % 5].Sprite, v.x + 7, v.y + 3, color: Colors.healthBarShield, blend: BlendMode.Screen, originRel: new Vec(.5, 0));
                    shieldPulse = Mutil.SnapLerp(shieldPulse, 1, 10, g.dt);
                }
            }
        }
    }
}
