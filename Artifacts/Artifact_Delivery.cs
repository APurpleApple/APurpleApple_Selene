using APurpleApple.Selene.CardActions;
using APurpleApple.Selene.Cards;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Artifacts
{
    internal class Artifact_Delivery : Artifact, IModArtifact
    {
        public static void Register(IModHelper helper)
        {
            Type type = MethodBase.GetCurrentMethod()!.DeclaringType!;
            helper.Content.Artifacts.RegisterArtifact(type.Name, new()
            {
                ArtifactType = type,
                Meta = new()
                {
                    owner = PMod.decks["selene"].Deck,
                    pools = [ArtifactPool.Common],
                },
                Sprite = PMod.sprites["artifact_delivery"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_Delivery", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_Delivery", "description"]).Localize
            });
        }

        public override void OnCombatStart(State state, Combat combat)
        {
            Card template = new Card_RandomAttach();
            template.discount = -1;
            template.temporaryOverride = true;
            template.singleUseOverride = true;
            template.retainOverride = true;
            combat.Queue(new AAddRandomAttachCard() { card = template, destination = CardDestination.Hand });
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> tooltip = new List<Tooltip>();
            Card template = new Card_RandomAttach();
            template.discount = -1;
            template.temporaryOverride = true;
            template.singleUseOverride = true;
            template.retainOverride = true;
            tooltip.Add(new TTCard() { card = template});

            return tooltip;
        }
    }

}
