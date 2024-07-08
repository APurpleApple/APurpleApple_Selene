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
    internal class Card_SeleneWeld : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Weld", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneWeld() { removeSingleUse = true});
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneWeld() { removeSingleUse = true, removeTemp = true });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneWeld() { removeSingleUse = true, removeTemp = true, removeBreakable = true });
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
                    data.cost = 2;
                    data.description = PMod.Instance.Localizations.Localize(["card", "Weld", "description"]);
                    data.exhaust = true;
                    break;
                case Upgrade.A:
                    data.cost = 2;
                    data.description = PMod.Instance.Localizations.Localize(["card", "Weld", "descriptionA"]);
                    data.singleUse = true;
                    break;
                case Upgrade.B:
                    data.cost = 4;
                    data.description = PMod.Instance.Localizations.Localize(["card", "Weld", "descriptionB"]);
                    data.singleUse = true;
                    break;
            }
            return data;
        }

    }
}
