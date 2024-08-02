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
    internal class Card_SeleneDelivery : Card, IModCard
    {
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
                    dontOffer = false
                },
                Art = PMod.sprites["selene_cardBackAttach"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Delivery", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            Card template = new Card_RandomAttach();
            template.temporaryOverride = true;
            template.singleUseOverride = true;

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new AStatus() { targetPlayer = true, status = SStatus.droneShift, statusAmount = 1});
                    actions.Add(new AAddRandomAttachCard() { card = template, destination = CardDestination.Hand });

                    break;
                case Upgrade.A:
                    template.discount = -1;
                    actions.Add(new AAddRandomAttachCard() { card = template, amount = 2, destination = CardDestination.Hand});

                    break;
                case Upgrade.B:
                    template.retainOverride = true;
                    template.retainOverrideIsPermanent = true;
                    actions.Add(new AStatus() { targetPlayer = true, status = SStatus.droneShift, statusAmount = 2 });
                    actions.Add(new AAddRandomAttachCard() { card = template, destination = CardDestination.Hand });

                    break;
                default:
                    break;
            }
            

            return actions;
        }

        public override CardData GetData(State state)
        {
            CardData data = new CardData();
            switch (upgrade)
            {
                case Upgrade.None:
                    data.cost = 1;
                    break;
                case Upgrade.A:
                    data.cost = 1;
                    break;
                case Upgrade.B:
                    data.cost = 1;
                    break;
            }
            return data;
        }

    }
}
