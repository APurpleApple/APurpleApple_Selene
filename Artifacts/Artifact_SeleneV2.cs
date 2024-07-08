using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Artifacts
{
    internal class Artifact_SeleneV2 : Artifact, IModArtifact
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
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = PMod.sprites["selene_artifact_gun"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_SeleneV2", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_SeleneV2", "description"]).Localize
            });
        }
        public override void OnPlayerAttack(State state, Combat combat)
        {

        }
    }

}
