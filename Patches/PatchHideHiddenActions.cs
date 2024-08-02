using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APurpleApple.Selene.CardActions;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using APurpleApple.Selene.Interfaces;

namespace APurpleApple.Selene.Patches
{
    [HarmonyPatch]
    public static class PatchHideHiddenActions
    {
        [HarmonyPatch(typeof(Card), nameof(Card.MakeAllActionIcons)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CardMakeAllActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase originalMethod)
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldloc(0),
                    ILMatches.AnyCall,
                    ILMatches.LdcI4(-6),
                    ILMatches.Instruction(OpCodes.Mul),
                    ILMatches.Stloc(1)
                )
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,
                [
                    new CodeInstruction(OpCodes.Ldloca, 0),
                    new CodeInstruction(OpCodes.Call, typeof(PatchHideHiddenActions).GetMethod(nameof(RemoveHiddenActionsFromList), BindingFlags.NonPublic | BindingFlags.Static)),
                ]
                )
                .AllElements();
        }

        private static void RemoveHiddenActionsFromList(ref List<CardAction> list)
        {
            for (int i = list.Count-1; i >= 0; i--)
            {
                if (list[i] is IHiddenAction)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}
