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
    internal class Card_SeleneAttachDynamo : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachDynamo", "name"]).Localize
            });
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();
            actions.Add(new ASeleneInsertPart()
            {
                part = new PartDynamoCannon()
                {
                    skin = PMod.parts["selene_dynamo"].UniqueName,
                    type = PType.special,
                    icon = PMod.sprites["icon_part_dynamo"].Sprite,
                    stunModifier = PStunMod.breakable,
                    tooltip = "Part_Dynamo",
                }
            });
            switch (upgrade)
            {
                case Upgrade.None:
                    
                    break;
                case Upgrade.A:
                    actions.Add(new AMove() { targetPlayer = true, dir = 1});
                    actions.Add(new AMove() { targetPlayer = true, dir = 1});
                    break;
                case Upgrade.B:
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
                    data.exhaust = true;
                    break;
                case Upgrade.A:
                    data.cost = 2;
                    data.exhaust = true;
                    data.flippable = true;

                    break;
                case Upgrade.B:
                    data.cost = 2;
                    break;
                default:
                    break;
            }

            if (state.EnumerateAllArtifacts().Any(a => a is Artifact_CheapRandom))
            {
                data.cost--;
            }
            return data;
        }

    }
}
