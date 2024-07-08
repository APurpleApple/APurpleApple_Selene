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
    internal class Card_SeleneAlign : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Align", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new AStatus() { status = Status.evade, statusAmount = 1, targetPlayer = true });
                    actions.Add(new AStatus() { status = Status.droneShift, statusAmount = 1, targetPlayer = true });
                    break;
                case Upgrade.A:
                    actions.Add(new ADroneMove() { dir = 1});
                    actions.Add(new AMove() { dir = -1, targetPlayer = true});
                    actions.Add(new ADrawCard(){count = 1});
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneAlignDrone());
                    actions.Add(new AStatus() { status = Status.droneShift, statusAmount = 1, targetPlayer = true });
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
                    data.cost = 0;
                    data.flippable = true;
                    break;
                case Upgrade.B:
                    data.cost = 1;
                    break;
            }
            return data;
        }

    }
}
