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
    public class PartReactor : PartSelene, IModPart
    {
        public int changeAmount = 1;

        public PartReactor()
        {
            type = PMod.pTypes["Reactor"].PartType;
        }
        public override List<Tooltip>? GetTooltips(State s)
        {
            type = PMod.pTypes["Reactor"].PartType;
            List<Tooltip> list = base.GetTooltips(s) ?? new List<Tooltip>();
            list.Add(new TTGlossary((changeAmount > 0) ? "action.gainEnergy" : "action.loseEnergy", changeAmount));
            return list;
        }

        public override void Destroy(State s, Combat c)
        {
            base.Destroy(s, c);
            int x = s.ship.parts.IndexOf(this);

            if (x -1 >= 0)
            {
                s.ship.NormalDamage(s, c, 1, s.ship.x + x - 1);
            }

            if (x +1 < s.ship.parts.Count)
            {
                s.ship.NormalDamage(s, c, 1, s.ship.x + x + 1);
            }
        }

        public override void OnTurnStart(State s, Combat c)
        {
            c.QueueImmediate(new AEnergy() { changeAmount = changeAmount});
        }

        public static void Register(IModHelper helper)
        {
            PMod.Instance.RegisterPartTypeAndGlossary("Reactor",
                () => PMod.sprites["icon_part_reactor"].Sprite,
                () => PMod.Instance.Localizations.Localize(["parts", "Reactor", "name"]),
                () => PMod.Instance.Localizations.Localize(["parts", "Reactor", "description"]),
            helper);

            PMod.parts["selene_reactor"] = helper.Content.Ships.RegisterPart("selene_reactor", new PartConfiguration() { Sprite = PMod.sprites["selene_part_reactor"].Sprite, DisabledSprite = SSpr.parts_scaffolding });

        }
    }
}
