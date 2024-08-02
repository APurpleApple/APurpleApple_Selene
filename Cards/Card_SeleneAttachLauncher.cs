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
    internal class Card_SeleneAttachLauncher : Card, IModCard
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
                Name = PMod.Instance.AnyLocalizations.Bind(["card", "AttachLauncher", "name"]).Localize
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
                        part = new PartMissileLauncher()
                        {
                            skin = PMod.parts["selene_launcher"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_launcher"].Sprite,
                            stunModifier = PStunMod.breakable,
                            launched = new Missile(),
                            tooltip = "Part_Launcher",
                        }
                    });
                    break;
                case Upgrade.A:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartMissileLauncher()
                        {
                            skin = PMod.parts["selene_launcherHeavy"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_launcherHeavy"].Sprite,
                            stunModifier = PStunMod.breakable,
                            launched = new Missile() { missileType = MissileType.heavy },
                            tooltip = "Part_LauncherHeavy",
                        }
                    });
                    break;
                case Upgrade.B:
                    actions.Add(new ASeleneInsertPart()
                    {
                        part = new PartMissileLauncher()
                        {
                            skin = PMod.parts["selene_launcher"].UniqueName,
                            type = PType.special,
                            icon = PMod.sprites["icon_part_launcher"].Sprite,
                            stunModifier = PStunMod.breakable,
                            launched = new Missile(),
                            tooltip = "Part_Launcher",
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
            data.cost = 2;
            if (upgrade == Upgrade.B)
            {
                data.cost = 0;
                data.exhaust = true;
            }

            if (state.EnumerateAllArtifacts().Any(a => a is Artifact_CheapRandom))
            {
                data.cost--;
            }
            return data;
        }

    }
}
