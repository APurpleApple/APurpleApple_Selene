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
    internal class Card_SeleneEject : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "Eject", "name"]).Localize
            });
        }

        public Part? GetEjectedPart(State s)
        {
            if(flipped)
            {
                return s.ship.parts.FirstOrDefault(p => PMod.SPEApi!.GetCustomPart(p)?.IsTemporary ?? false);
            }

            return s.ship.parts.LastOrDefault(p => PMod.SPEApi!.GetCustomPart(p)?.IsTemporary ?? false);
        }

        public int GetDamage(State s)
        {
            int damage = 2;
            if (s.EnumerateAllArtifacts().Any(a => a is Artifact_EjectBuff)) damage++;
            return damage;
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new List<CardAction>();

            Part? ejectedPart = GetEjectedPart(s);
            switch (upgrade)
            {
                case Upgrade.None:
                    actions.Add(new ASeleneEjectLeftmost() {flipped = flipped, ejectedPart = ejectedPart, damage = GetDamage(s) });
                    actions.Add(new ADrawCard() {count = 1 });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneEjectLeftmost() {flipped = flipped, ejectedPart = ejectedPart, damage = GetDamage(s) });
                    actions.Add(new ADrawCard() { count = 1 });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneEjectLeftmost() {flipped = flipped, ejectedPart = ejectedPart, damage = GetDamage(s) });
                    actions.Add(new ADrawCard() { count = 1 });
                    actions.Add(new AStatus() { status = SStatus.droneShift, statusAmount = 1, targetPlayer = true });
                    actions.Add(new AStatus() { status = SStatus.evade, statusAmount = 1, targetPlayer = true });
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
                    data.flippable = true;
                    break;
                case Upgrade.A:
                    data.cost = 0;
                    data.flippable = true;
                    break;
                case Upgrade.B:
                    data.cost = 1;
                    data.flippable = true;
                    break;
            }
            return data;
        }

    }
}
