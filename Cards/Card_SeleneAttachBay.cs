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
    internal class Card_SeleneAttachBay : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachBay", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new TemporaryBay()
                        {
                            skin = PMod.parts["selene_bay"].UniqueName,
                            type = PType.missiles,
                            icon = PMod.sprites["icon_part_bay"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Bay",
                            singleUse = true,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new ADroneMove()
                    {
                        dir = 1,
                    });
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new TemporaryBay()
                        {
                            skin = PMod.parts["selene_bay"].UniqueName,
                            type = PType.missiles,
                            icon = PMod.sprites["icon_part_bay"].Sprite,
                            singleUse = true,
                            tooltip = "Part_Bay",
                            damageModifier = PDamMod.armor,
                            stunModifier = PStunMod.breakable,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new TemporaryBay()
                        {
                            skin = PMod.parts["selene_bay"].UniqueName,
                            type = PType.missiles,
                            icon = PMod.sprites["icon_part_bay"].Sprite,
                            stunModifier = PStunMod.breakable,
                            singleUse = false,
                            tooltip = "Part_Bay",
                            removedOnCombatEnd = true,
                        }
                    });
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
                    data.flippable = true;

                    break;
                case Upgrade.B:
                    data.cost = 4;

                    break;
                default:
                    break;
            }
            return data;
        }

    }
}
