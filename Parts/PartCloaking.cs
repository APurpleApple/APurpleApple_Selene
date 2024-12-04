using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.Cards;
using System.Runtime.InteropServices;
using Nickel;

namespace APurpleApple.Selene
{
    public class PartCloaking : PartSelene, IModPart
    {
        public Upgrade upgrade = Upgrade.None;
        public bool hasBeenUsed = false;

        public PartCloaking()
        {
            type = PMod.pTypes["Cloaking"].PartType;
        }


        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Cloak",
            () => PMod.sprites["icon_part_cloak"].Sprite,
            () => PMod.Instance.Localizations.Localize(["parts", "Cloak", "name"]),
            () => PMod.Instance.Localizations.Localize(["parts", "Cloak", "description"]),
            helper);
            PMod.parts["selene_cloak"] = helper.Content.Ships.RegisterPart("selene_cloak", new PartConfiguration() { Sprite = PMod.sprites["selene_part_cloak"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        }

        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes["Cloaking"].PartType;
            List<Tooltip> list = base.GetTooltips(s) ?? new List<Tooltip>();
            list.Add(new TTCard() { card = new Card_SeleneActivateCloak() { upgrade = upgrade } });
            return list;
        }

        public override void OnTurnStart(State s, Combat c)
        {
            bool giveCard = true;
            if (hasBeenUsed && singleUse)
            {
                if (s.ship.Get(PMod.statuses["reinforce"].Status) > 0)
                {
                    c.QueueImmediate(new AStatus() { status = PMod.statuses["reinforce"].Status, statusAmount = -1, targetPlayer = true });
                }
                else
                {
                    giveCard = false;
                    Destroy(s, c);
                }
            }
            hasBeenUsed = false;

            if (giveCard)
            {
                if (c.hand.Any(x => x is Card_SeleneActivateCloak ca && ca.linkedPartuuid == uuid)) return;
                c.Queue(new AAddCard() { amount = 1, card = new Card_SeleneActivateCloak() { linkedPartuuid = uuid, upgrade = upgrade }, destination = CardDestination.Hand });
            }
            
        }
    }
}
