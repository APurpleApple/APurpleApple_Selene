using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartBay : PartSelene, IModPart
    {
        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Bay",
                () => PMod.sprites["icon_part_bay"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "Bay", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "Bay", "description"]),
                helper);

            PMod.parts["selene_bay"] = helper.Content.Ships.RegisterPart("selene_bay", new PartConfiguration() { Sprite = PMod.sprites["selene_part_bay"].Sprite, DisabledSprite = SSpr.parts_scaffolding });

        }
    }
}
