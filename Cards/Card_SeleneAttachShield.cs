﻿using Nickel;
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
    internal class Card_SeleneAttachShield : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachShield", "name"]).Localize
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
                        part = new PartShieldProjector()
                        {
                            skin = PMod.parts["selene_shield"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_shield"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Shield",
                            damageModifier = PDamMod.weak,
                            singleUse = false,
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartShieldProjector()
                        {
                            skin = PMod.parts["selene_shield"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_shield"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_Shield",
                            damageModifier = PDamMod.armor,
                            singleUse = false,
                        }
                    });
                    actions.Add(new AStatus() { status = SStatus.shield, statusAmount = 1, targetPlayer = true });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartShieldProjector()
                        {
                            blocked = 2,
                            skin = PMod.parts["selene_shieldV2"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_shield_v2"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = "Part_ShieldV2",
                            damageModifier = PDamMod.weak,
                            singleUse = false,
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
                    data.cost = 2;
                    break;
                case Upgrade.A:
                    data.cost = 2;
                    break;
                case Upgrade.B:
                    data.cost = 3;
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
