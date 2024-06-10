using APurpleApple.Selene.ExternalAPIs;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.Artifacts
{
    internal class Artifact_Selene : Artifact, IModArtifact
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
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = PMod.sprites["selene_artifact"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Asteroid", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Asteroid", "description"]).Localize
            });
        }

        public int droneX = 0;
        public double droneXLerped = 0;
        
    }
}
