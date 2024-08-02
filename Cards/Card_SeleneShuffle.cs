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
    internal class Card_SeleneShuffle : Card, IModCard
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
                    rarity = Rarity.rare,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = false
                },
                Art = PMod.sprites["selene_cardBackAttach"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Shuffle", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new AShuffleShip() { targetPlayer = true});
                    actions.Add(new AStatus() { targetPlayer = true, status = SStatus.tempShield, statusAmount = 2 });
                    break;
                case Upgrade.A:
                    actions.Add(new AShuffleShip() { targetPlayer = true });
                    actions.Add(new AStatus() { targetPlayer = true, status = SStatus.tempShield, statusAmount = 2 });
                    break;
                case Upgrade.B:
                    actions.Add(new AShuffleShip() { targetPlayer = false });
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
                    data.description = PMod.Instance.Localizations.Localize(["card", "Shuffle", "description"]);
                    break;
                case Upgrade.A:
                    data.cost = 0;
                    data.description = PMod.Instance.Localizations.Localize(["card", "Shuffle", "description"]);
                    break;
                case Upgrade.B:
                    data.cost = 2;
                    data.description = PMod.Instance.Localizations.Localize(["card", "Shuffle", "descriptionEnemy"]);
                    break;
            }
            return data;
        }

    }
}
