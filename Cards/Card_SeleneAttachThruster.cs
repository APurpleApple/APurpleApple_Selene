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

            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartThruster()
                        {
                            skin = PMod.parts["selene_thruster"].UniqueName,
                            flip = flipped,
                            type = PType.special,
                            icon = PMod.sprites[flipped ? "icon_part_thruster_right" : "icon_part_thruster_left"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = flipped ? "Part_ThrusterRight" : "Part_ThrusterLeft",
                            singleUse = false,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartThruster()
                        {
                            skin = PMod.parts["selene_thrusterV2"].UniqueName,
                            flip = flipped,
                            type = PType.special,
                            icon = PMod.sprites[flipped ? "icon_part_thruster_v2_right" : "icon_part_thruster_v2_left"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = flipped ? "Part_ThrusterV2Right" : "Part_ThrusterV2Left",
                            singleUse = false,
                            upgraded = true,
                            removedOnCombatEnd = true,
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartThruster()
                        {
                            skin = PMod.parts["selene_thruster"].UniqueName,
                            flip = flipped,
                            type = PType.special,
                            icon = PMod.sprites[flipped ? "icon_part_thruster_right" : "icon_part_thruster_left"].Sprite,
                            stunModifier = PStunMod.breakable,
                            tooltip = flipped ? "Part_ThrusterRight" : "Part_ThrusterLeft",
                            singleUse = false,
                            removedOnCombatEnd = true,
                        }
                    });
                    actions.Add(new AStatus() { status = SStatus.evade, statusAmount = 1, targetPlayer = true});
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
            data.flippable = true;
            return data;
        }

    }
}
