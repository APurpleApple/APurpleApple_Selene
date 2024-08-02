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
    internal class Card_SeleneRecycle : Card, IModCard
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
                    rarity = Rarity.uncommon,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = false
                },
                Art = PMod.sprites["selene_cardBackAttach"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Recycle", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            actions.Add(
                PMod.kokoroApi!.ConditionalActions.Make(
                new ASeleneRecycle.Condition(),
                action: PMod.kokoroApi!.Actions.MakeContinue(out var stopId)
                ));

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.AddRange(PMod.kokoroApi!.Actions.MakeContinued(stopId,
                        [
                        new ASeleneRecycle(),
                        new AEnergy(){changeAmount = 2}

                        ]));
                    break;
                    case Upgrade.A:
                    Card template = new Card_RandomAttach();
                    template.temporaryOverride = true;
                    template.singleUseOverride = true;
                    actions.AddRange(PMod.kokoroApi!.Actions.MakeContinued(stopId,
                        [
                        new ASeleneRecycle(),
                        new AAddRandomAttachCard() { card = template, amount = 1, destination = CardDestination.Hand}

                        ]));
                    break;
                    case Upgrade.B:
                    actions.AddRange(PMod.kokoroApi!.Actions.MakeContinued(stopId,
                        [
                        new ASeleneRecycle(),
                        new AEnergy(){changeAmount = 2}

                        ]));
                    break;
                default:
                    break;
            }
            
            

            return actions;
        }

        public override CardData GetData(State state)
        {
            CardData data = new CardData();
            data.cost = 0;
            switch (upgrade)
            {
                case Upgrade.None:
                    break;
                case Upgrade.A:
                    data.infinite = true;
                    break;
                case Upgrade.B:
                    data.retain = true;
                    break;
                default:
                    break;
            }
            return data;
        }

    }
}
