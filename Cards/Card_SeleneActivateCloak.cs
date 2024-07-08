using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;

namespace APurpleApple.Selene.Cards
{
    internal class Card_SeleneActivateCloak : Card, IModCard
    {
        public int linkedPartuuid;

        public static void Register(IModHelper helper)
        {
            Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
            helper.Content.Cards.RegisterCard(type.Name, new()
            {
                CardType = type,
                Meta = new()
                {
                    deck = PMod.decks["selene"].Deck,
                    rarity = Rarity.common,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = true
                },
                Art = PMod.sprites["selene_cardBackAttach"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "ActivateCloak", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneActivateCloak() { cloakingPartUUID = linkedPartuuid });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneCloakAll());
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneActivateCloak() { cloakingPartUUID = linkedPartuuid });
                    break;
                default:
                    break;
            }
            

            return actions;
        }

        public override CardData GetData(State state)
        {
            CardData data = new CardData();
            data.cost = 2;
            data.description = PMod.Instance.Localizations.Localize(["card", "ActivateCloak", "description"]);
            data.singleUse = true;
            data.temporary = true;
            data.retain = true;

            switch (upgrade)
            {
                case Upgrade.None:
                    break;
                case Upgrade.A:
                    data.cost = 0;
                    data.description = PMod.Instance.Localizations.Localize(["card", "ActivateCloak", "descriptionB"]);
                    break;
                case Upgrade.B:
                    data.cost = 1;
                    break;
                default:
                    break;
            }
            return data;
        }

    }
}
