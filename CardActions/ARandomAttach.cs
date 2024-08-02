using APurpleApple.Selene.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APurpleApple.Selene.CardActions
{
    internal class AAddRandomAttachCard : AAddCard
    {
        internal static IReadOnlyList<Type> Generatable_Types { get; } = [
            typeof(Card_SeleneAttachCannon),
            typeof(Card_SeleneAttachBay),
            typeof(Card_SeleneAttachShield),
            typeof(Card_SeleneAttachThruster),
            typeof(Card_SeleneAttachCloaking),
            typeof(Card_SeleneAttachBubble),
            typeof(Card_SeleneAttachLauncher),
            typeof(Card_SeleneAttachDynamo),
            typeof(Card_SeleneAttachReactor),
        ];

        public override void Begin(G g, State s, Combat c)
        {
            int iterations = amount;
            amount = 1;

            for (int i = 0; i < iterations; i++)
            {
                Type type = Generatable_Types.Shuffle(s.rngActions).First();

                Card? newCard = Activator.CreateInstance(type) as Card ?? card;

                newCard.discount = card.discount;
                newCard.retainOverride = card.retainOverride;
                newCard.recycleOverrideIsPermanent = card.recycleOverrideIsPermanent;
                newCard.temporaryOverride = card.temporaryOverride;
                newCard.singleUseOverride = card.singleUseOverride;

                card = newCard;

                base.Begin(g, s, c);
            }
        }
    }
}
