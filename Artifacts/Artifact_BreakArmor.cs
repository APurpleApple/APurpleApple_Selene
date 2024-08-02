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
    internal class Artifact_BreakArmor : Artifact, IModArtifact
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
                Sprite = PMod.sprites["artifact_break_armor"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_BreakArmor", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_BreakArmor", "description"]).Localize
            });
        }

        public override List<Tooltip>? GetExtraTooltips()
        {
            List<Tooltip> tooltip = new List<Tooltip>();
            tooltip.Add(PMod.glossaries["Breakable"]);
            return tooltip;
        }
    }

}
