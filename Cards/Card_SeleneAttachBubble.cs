using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using Microsoft.Win32.SafeHandles;

namespace APurpleApple.Selene.Cards
{
    internal class Card_SeleneAttachBubble: Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachBubble", "name"]).Localize
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
                        part = new PartBubble()
                        {
                            skin = PMod.parts["selene_bubble"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_bubble"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Bubble",
                            singleUse = false,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartBubble()
                        {
                            skin = PMod.parts["selene_bubble"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_bubble"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Bubble",
                            singleUse = false,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartBubble()
                        {
                            skin = PMod.parts["selene_bubble"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_bubble"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Bubble",
                            singleUse = false,
                            removedOnCombatEnd = true,
                        }
                    });
                    actions.Add(new ASpawn() { thing = new AttackDrone()});
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
                    break;
                case Upgrade.A:
                    data.cost = 1;
                    break;
                case Upgrade.B:
                    data.cost = 2;
                    break;
                default:
                    break;
            }
            return data;
        }

    }
}
