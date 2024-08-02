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
    internal class Artifact_Safety : Artifact, IModArtifact
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
                Sprite = PMod.sprites["artifact_safety"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_Safety", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_Safety", "description"]).Localize
            });
        }

        public override void OnCombatStart(State state, Combat combat)
        {
            combat.Queue(new AStatus() { status = PMod.statuses["reinforce"].Status, targetPlayer = true, statusAmount = 2 });
            combat.Queue(new AStatus() { status = PMod.statuses["plating"].Status, targetPlayer = true, statusAmount = 1 });
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> tooltip = new List<Tooltip>();
            tooltip.Add(new TTGlossary($"status.{PMod.statuses["reinforce"].Status}"));
            tooltip.Add(new TTGlossary($"status.{PMod.statuses["plating"].Status}"));
            return tooltip;
        }
    }

}
