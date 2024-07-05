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
    internal class Card_SeleneAttachThruster : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachThruster", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            if (flipped)
            {
                actions.Add(new ASeleneInsertPart()
                {
                    part = new PartThruster()
                    {
                        skin = PMod.parts["selene_thruster"].UniqueName,
                        flip = true,
                        type = PType.special,
                        icon = PMod.sprites["icon_part_thruster_right"].Sprite,
                        stunModifier = PStunMod.breakable,
                        tooltip = "Part_ThrusterRight",
                        singleUse = false,
                        removedOnCombatEnd = true,
                    }
                });
            }
            else
            {
                actions.Add(new ASeleneInsertPart()
                {
                    part = new PartThruster()
                    {
                        skin = PMod.parts["selene_thruster"].UniqueName,
                        type = PType.special,
                        icon = PMod.sprites["icon_part_thruster_left"].Sprite,
                        stunModifier = PStunMod.breakable,
                        tooltip = "Part_ThrusterLeft",
                        singleUse = false,
                        removedOnCombatEnd = true,
                    }
                });
            }
            

            return actions;
        }

        public override CardData GetData(State state)
        {
            CardData data = new CardData();
            data.cost = 1;
            data.flippable = true;
            return data;
        }

    }
}
