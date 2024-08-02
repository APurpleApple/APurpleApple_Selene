using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using Microsoft.Win32.SafeHandles;
using APurpleApple.Selene.Artifacts;

namespace APurpleApple.Selene.Cards
{
    internal class Card_SeleneAttachReactor : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachReactor", "name"]).Localize
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
                        part = new PartReactor()
                        {
                            skin = PMod.parts["selene_reactor"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_reactor"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.weak,
                            tooltip = "Part_Reactor",
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new AStatus()
                    {
                        status = PMod.statuses["plating"].Status,
                        targetPlayer = true,
                        statusAmount = 1
                    });
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartReactor()
                        {
                            skin = PMod.parts["selene_reactor"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_reactor"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.none,
                            tooltip = "Part_Reactor",
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartReactor()
                        {
                            skin = PMod.parts["selene_reactor"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_reactor"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.weak,
                            tooltip = "Part_Reactor",
                        }
                    });
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartReactor()
                        {
                            skin = PMod.parts["selene_reactor"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_reactor"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.weak,
                            tooltip = "Part_Reactor",
                        }
                    });
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartReactor()
                        {
                            skin = PMod.parts["selene_reactor"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_reactor"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.weak,
                            tooltip = "Part_Reactor",
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
                    data.exhaust = true;
                    break;
                case Upgrade.A:
                    data.cost = 1;
                    data.exhaust = true;
                    break;
                case Upgrade.B:
                    data.cost = 2;
                    data.exhaust = true;
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
