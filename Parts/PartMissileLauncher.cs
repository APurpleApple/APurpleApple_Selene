using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.Cards;
using System.Runtime.InteropServices;
using Nickel;

namespace APurpleApple.Selene
{
    public class PartMissileLauncher : PartSelene, IModPart
    {
        public required StuffBase launched;
        public bool upgraded = false;

        public PartMissileLauncher()
        {
            type = PMod.pTypes[upgraded ? "LauncherHeavy" :"Launcher"].PartType;
        }

        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Launcher",
               () => PMod.sprites["icon_part_launcher"].Sprite,
               () => PMod.Instance.Localizations.Localize(["parts", "Launcher", "name"]),
               () => PMod.Instance.Localizations.Localize(["parts", "Launcher", "description"]),
            helper);

            PMod.Instance.RegisterPartTypeAndGlossary("LauncherHeavy",
              () => PMod.sprites["icon_part_launcherHeavy"].Sprite,
              () => PMod.Instance.Localizations.Localize(["parts", "Launcher", "name"]),
              () => PMod.Instance.Localizations.Localize(["parts", "Launcher", "descriptionA"]),
            helper);

            PMod.parts["selene_launcher"] = helper.Content.Ships.RegisterPart("selene_launcher", new PartConfiguration() { Sprite = PMod.sprites["selene_part_launcher"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
            PMod.parts["selene_launcherHeavy"] = helper.Content.Ships.RegisterPart("selene_launcherHeavy", new PartConfiguration() { Sprite = PMod.sprites["selene_part_launcherHeavy"].Sprite, DisabledSprite = SSpr.parts_scaffolding });
        }

        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes[upgraded ? "LauncherHeavy" : "Launcher"].PartType;
            List<Tooltip> list = base.GetTooltips(s) ?? new List<Tooltip>();
            list.AddRange(launched.GetTooltips());
            return list;
        }

        public override void OnTurnStart(State s, Combat c)
        {
            int x = s.ship.parts.IndexOf(this);
            c.Queue(new ASpawn() { thing = Mutil.DeepCopy(launched), multiBayVolley = true, fromPlayer = true, fromX = x });
        }
    }
}
