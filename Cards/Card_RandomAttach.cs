using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Cards
{
    internal class Card_RandomAttach : Card, IModCard
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
                    dontOffer = true
                },
                Art = PMod.sprites["selene_cardBackAttach"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "RandomAttach", "name"]).Localize,
            });
        }

        public override CardData GetData(State state)
        {
            CardData data = base.GetData(state);
            data.description = PMod.Instance.Localizations.Localize(["card", "RandomAttach", "description"]);
            data.cost = -1;
            return data;
        }
    }
}
