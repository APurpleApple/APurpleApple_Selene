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
    internal class Card_SeleneAttachCloaking : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachCloak", "name"]).Localize
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
                        part = new CloakingDevice()
                        {
                            skin = PMod.parts["selene_cloak"].UniqueName,
                            type = PType.missiles,
                            icon = PMod.sprites["icon_part_cloak"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Cloak",
                            singleUse = true,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new CloakingDevice()
                        {
                            skin = PMod.parts["selene_cloak"].UniqueName,
                            type = PType.missiles,
                            icon = PMod.sprites["icon_part_cloak"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Cloak",
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new CloakingDevice()
                        {
                            upgrade = Upgrade.B,
                            skin = PMod.parts["selene_cloak"].UniqueName,
                            type = PType.missiles,
                            icon = PMod.sprites["icon_part_cloak"].Sprite,
                            stunModifier = PStunMod.breakable,
                            singleUse = true,
                            tooltip = "Part_Cloak",
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
            data.cost = 1;
            return data;
        }

    }
}
