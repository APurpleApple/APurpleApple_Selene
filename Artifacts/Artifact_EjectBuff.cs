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
    internal class Artifact_EjectBuff : Artifact, IModArtifact
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
                Sprite = PMod.sprites["artifact_eject_buff"].Sprite,
                Name = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_EjectBuff", "name"]).Localize,
                Description = PMod.Instance.AnyLocalizations.Bind(["artifact", "Artifact_EjectBuff", "description"]).Localize
            });
        }
    }

}
