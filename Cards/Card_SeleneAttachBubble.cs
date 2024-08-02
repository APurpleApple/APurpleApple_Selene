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
                    rarity = Rarity.uncommon,
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
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 1, disabled = flipped, targetPlayer = true });
                    actions.Add(new ASeleneInsertPart()
                    {
                        disabled = flipped,
                        part = new PartBubble()
                        {
                            skin = PMod.parts["selene_bubble"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_bubble"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.armor,
                            tooltip = "Part_Bubble",
                            singleUse = false,
                        }
                    });
                    actions.Add(new ADummyAction());
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 1, disabled = !flipped, targetPlayer = true });
                    actions.Add(new ASpawn() { thing = new Asteroid() { targetPlayer = true }, disabled = !flipped });
                    break;
                case Upgrade.A:
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 2, disabled = flipped, targetPlayer = true });
                    actions.Add(new ASeleneInsertPart()
                    {
                        disabled = flipped,
                        part = new PartBubble()
                        {
                            skin = PMod.parts["selene_bubble"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_bubble"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.armor,
                            tooltip = "Part_Bubble",
                            singleUse = false,
                        }
                    });
                    actions.Add(new ADummyAction());
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 1, disabled = !flipped, targetPlayer = true });
                    actions.Add(new ASpawn() { thing = new ShieldDrone(){targetPlayer = true }, disabled = !flipped });
                    break;
                case Upgrade.B:
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 2, disabled = flipped, targetPlayer = true });
                    actions.Add(new ASeleneInsertPart()
                    {
                        disabled = flipped,
                        part = new PartBubble()
                        {
                            skin = PMod.parts["selene_bubble"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_bubble"].Sprite,
                            stunModifier = PStunMod.breakable,
                            damageModifier = PDamMod.armor,
                            tooltip = "Part_Bubble",
                            singleUse = false,
                        }
                    });
                    actions.Add(new ADummyAction());
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 1, disabled = !flipped, targetPlayer = true });
                    actions.Add(new ASpawn() { thing = new AttackDrone(), disabled = !flipped });
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
                    data.floppable = true;
                    break;
                case Upgrade.A:
                    data.cost = 1;
                    data.floppable = true;
                    break;
                case Upgrade.B:
                    data.cost = 1;
                    data.floppable = true;
                    break;
                default:
                    break;
            }

            data.artTint = "ffffff";
            data.art = PMod.sprites[flipped ? "selene_cardBackAttach_tbot" : "selene_cardBackAttach_ttop"].Sprite;

            if (state.EnumerateAllArtifacts().Any(a => a is Artifact_CheapRandom))
            {
                data.cost--;
            }
            return data;
        }

    }
}
