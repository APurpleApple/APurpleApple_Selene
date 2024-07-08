using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using System.Reflection.Metadata;

namespace APurpleApple.Selene.Cards
{
    internal class Card_SeleneAttachCannon : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachCannon", "name"]).Localize
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
                        part = new TemporaryGun()
                        {
                            skin = PMod.parts["selene_cannon"].UniqueName,
                            type = PType.cannon,
                            icon = PMod.sprites["icon_part_cannon"].Sprite,
                            stunModifier = PStunMod.breakable,
                            singleUse = true,
                            tooltip = "Part_Cannon",
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
                        part = new TemporaryGun()
                        {
                            skin = PMod.parts["selene_cannon"].UniqueName,
                            type = PType.cannon,
                            icon = PMod.sprites["icon_part_cannon"].Sprite,
                            singleUse = true,
                            tooltip = "Part_Cannon",
                            damageModifier = PDamMod.armor,
                            stunModifier = PStunMod.breakable,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new TemporaryGun()
                        {
                            skin = PMod.parts["selene_cannon"].UniqueName,
                            type = PType.cannon,
                            icon = PMod.sprites["icon_part_cannon"].Sprite,
                            stunModifier = PStunMod.breakable,
                            singleUse = false,
                            tooltip = "Part_Cannon",
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
