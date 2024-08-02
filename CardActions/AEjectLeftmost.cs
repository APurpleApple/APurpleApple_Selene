using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;

namespace APurpleApple.Selene
{
    internal class ASeleneEjectLeftmost : AEjectPart
    {
        public bool flipped = false;
        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> result = base.GetTooltips(s);

            result.Add(new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => PMod.sprites[flipped ? "icon_ejectRight":"icon_ejectLeft"].Sprite,
            () => PMod.Instance.Localizations.Localize(["action", "Eject", flipped ?"nameF" : "name"]),
            () => PMod.Instance.Localizations.Localize(["action", "Eject", flipped ? "descriptionF" : "description"]),
            [() => damage]
            ));
            return result;
        }
        public override Icon? GetIcon(State s)
        {
            if (s != DB.fakeState)
            {
                disabled = ejectedPart == null;
            }
            return new Icon(PMod.sprites[flipped ? "icon_ejectRight" : "icon_ejectLeft"].Sprite, damage, Colors.attack);
        }
    }
}
