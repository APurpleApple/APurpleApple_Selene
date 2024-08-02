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
    internal class Card_SeleneMagnetize : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Magnetize", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneMagnetize() { pull = flipped });
                    actions.Add(new AStatus() { targetPlayer = true, status = SStatus.droneShift, statusAmount = 1 });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneMagnetize() { pull = flipped });
                    actions.Add(new AStatus() { targetPlayer = true, status = SStatus.droneShift, statusAmount = 1 });
                    actions.Add(new ADrawCard() { count = 1 });
                    break;
                case Upgrade.B:
                    actions.Add(PMod.kokoroApi!.ActionCosts.Make(
                        cost: PMod.kokoroApi.ActionCosts.Cost(
                            PMod.kokoroApi.ActionCosts.StatusResource(
                                status: SStatus.droneShift,
                                costUnsatisfiedIcon: PMod.sprites["cost_droneshiftOff"].Sprite,
                                costSatisfiedIcon: PMod.sprites["cost_droneshift"].Sprite
                                ),
                            amount: 1
                            ),
                        action: new ASeleneMagnetize() { pull = flipped }
                        ));

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
                    data.cost = 0;
                    data.flippable = true;
                    break;
                case Upgrade.A:
                    data.cost = 0;
                    data.flippable = true;
                    break;
                case Upgrade.B:
                    data.cost = 0;
                    data.infinite = true;
                    data.retain = true;
                    data.flippable = true;
                    break;
            }
            data.art = PMod.sprites[flipped ? "back_MagPull" : "back_MagPush"].Sprite;
            return data;
        }

    }
}
