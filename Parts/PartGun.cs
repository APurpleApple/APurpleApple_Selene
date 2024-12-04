using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene
{
    public class PartGun : PartSelene, IModPart
    {
        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Cannon",
                () => PMod.sprites["icon_part_cannon"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "Cannon", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "Cannon", "description"]),
                helper);

            PMod.parts["selene_cannon"] = helper.Content.Ships.RegisterPart("selene_cannon", new PartConfiguration() { Sprite = PMod.sprites["selene_part_cannon"].Sprite, DisabledSprite = SSpr.parts_scaffolding });

        }
    }
}
