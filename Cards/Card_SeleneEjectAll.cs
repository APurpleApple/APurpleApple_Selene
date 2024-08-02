using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.Artifacts;

namespace APurpleApple.Selene.Cards
{
    internal class Card_SeleneEjectAll : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "EjectAll", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();
            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneEjectAll() { damage = GetDamage(s) });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneEjectAll() { damage = GetDamage(s) });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneEjectAll() { damage = GetDamage(s) });
                    break;
                default:
                    break;
            }
            

            return actions;
        }

        public int GetDamage(State s)
        {
            int damage = 3;
            if (s.EnumerateAllArtifacts().Any(a => a is Artifact_EjectBuff)) damage++;
            if (upgrade == Upgrade.A)
            {
                damage += 1;
            }
            return damage;
        }

        public override CardData GetData(State state)
        {
            CardData data = new CardData();
            switch (upgrade)
            {
                case Upgrade.None:
                    data.cost = 1;
                    data.description = PMod.Instance.Localizations.Localize(["card", "EjectAll", "description"], GetDamage(state));
                    break;
                case Upgrade.A:
                    data.cost = 1;
                    data.description = PMod.Instance.Localizations.Localize(["card", "EjectAll", "description"], GetDamage(state));
                    break;
                case Upgrade.B:
                    data.cost = 0;
                    data.exhaust = true;
                    data.retain = true;
                    data.description = PMod.Instance.Localizations.Localize(["card", "EjectAll", "description"], GetDamage(state));
                    break;
            }
            return data;
        }

    }
}
