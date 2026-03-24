using System.Reflection;
using System.Reflection.Emit;
using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch]
public static class LingerPatch
{
    // [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add), [typeof(CardModel), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)])]
    // [HarmonyPrefix]
    public static bool NoAddIfLinger(CardModel card, CardPile newPile)
    {
        if (card.Keywords.Contains(ShadowfallKeywords.Linger))
        {
            MainFile.Logger.Info("Hey, that's a Linger trigger!");
            if (newPile.Type == PileType.Discard)
            {
                return false;
            }
        }
        return true;
    }

    // Transpiler here in CombatManager.DoTurnEnd, add extra return for ShadowfallKeywords.Linger
    // public static void 
    [HarmonyPatch(typeof(CombatManager))]
    [HarmonyPatch("DoTurnEnd")]
    [HarmonyPatch(MethodType.Async)]
    public static class CombatManagerReturnLingerToHand
    {
        [HarmonyDebug]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                var matcher = new CodeMatcher(instructions, generator)
                    // Find the ethereal check
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld),
                        new CodeMatch(OpCodes.Callvirt),
                        new CodeMatch(OpCodes.Ldc_I4_2),
                        new CodeMatch(OpCodes.Callvirt)
                    );

                var startIndex = matcher.Pos;

                matcher.MatchEndForward(new CodeMatch(OpCodes.Br_S));

                var endIndex = matcher.Pos;

                var EtherealCheckInstructions = matcher.InstructionsInRange(startIndex, endIndex);
                // Checking that we're at the right instructions
                // MainFile.Logger.Info("\n");
                // foreach(var instruction in EtherealCheckInstructions) {
                //     MainFile.Logger.Info($"{instruction}");
                // }

                var EtherealInstructionMatcher = new CodeMatcher(EtherealCheckInstructions, generator)
                    .MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_2))
                    .RemoveInstruction()
                    .InsertAndAdvance(CodeInstruction.LoadField(typeof(ShadowfallKeywords), nameof(ShadowfallKeywords.Linger)))
                    .MatchStartForward(new CodeMatch(OpCodes.Ldarg_0))
                    .RemoveInstructions(2) // Don't load choice context
                    .Advance(2) // skip card loading
                    .RemoveInstruction() // remove a true from the old call
                    .InsertAndAdvance(
                        CodeInstruction.LoadField(typeof(PileType), nameof(PileType.Draw)),
                        CodeInstruction.LoadField(typeof(CardPilePosition), nameof(CardPilePosition.Random)),
                        new CodeInstruction(OpCodes.Ldnull)
                    )
                    .MatchStartForward(new CodeMatch(OpCodes.Call))
                    .RemoveInstruction()
                    .InsertAndAdvance(CodeInstruction.Call(typeof(CardPileCmd), nameof(CardPileCmd.Add), [typeof(CardModel), typeof(PileType), typeof(CardPilePosition), typeof(AbstractModel), typeof(Boolean)]));
                    // .RemoveInstruction()
                    // .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt))

                var lingerInstructions = EtherealInstructionMatcher.InstructionEnumeration();

                matcher
                    .MatchStartBackwards(
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld),
                        new CodeMatch(OpCodes.Callvirt),
                        new CodeMatch(OpCodes.Ldc_I4_2),
                        new CodeMatch(OpCodes.Callvirt)
                    )
                    .RemoveInstructionsInRange(startIndex, endIndex)
                    .Insert(lingerInstructions);

                MainFile.Logger.Info("");

                var enumeration = matcher.InstructionEnumeration().ToArray();
                for(var i = 0; i < enumeration.Length; i++) {
                    MainFile.Logger.Info($"{i} | {enumeration[i]}");
                }
                return matcher.InstructionEnumeration();
            }
            catch(Exception e){
                MainFile.Logger.Info($"\n {MainFile.ModId} Error | {e}");
            }
            return instructions;
        }

            // [823 7 - 823 56]
            //IL_0393: ldarg.0      // this
            //IL_0394: ldfld        class MegaCrit.Sts2.Core.Models.CardModel MegaCrit.Sts2.Core.Combat.CombatManager/'<DoTurnEnd>d__116'::'<card>5__5'
            //IL_0399: callvirt     instance class [System.Runtime]System.Collections.Generic.IReadOnlySet`1<valuetype MegaCrit.Sts2.Core.Entities.Cards.CardKeyword> MegaCrit.Sts2.Core.Models.CardModel::get_Keywords()
            //IL_039e: ldc.i4.2
            //IL_039f: callvirt     instance bool class [System.Runtime]System.Collections.Generic.IReadOnlySet`1<valuetype MegaCrit.Sts2.Core.Entities.Cards.CardKeyword>::Contains(!0/*valuetype MegaCrit.Sts2.Core.Entities.Cards.CardKeyword*/)
            //IL_03a4: brfalse.s    IL_0413

        // [HarmonyDebug]
        // static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        // {
        //     try
        //     {
        //         var matcher = new CodeMatcher(instructions, generator)
        //             // Find the jump point for the if else
        //             .MatchStartForward(
        //                 new CodeMatch(OpCodes.Call),
        //                 new CodeMatch(OpCodes.Br_S)
        //             )
        //             .MatchStartForward(new CodeMatch(OpCodes.Leave_S))
        //             .CreateLabel(out Label breakOpcode)
        //             .Advance(1)
        //             // .AddLabels([breakOpcode])
        //             // match after the ethereal check
        //             .MatchStartBackwards(
        //                 new CodeMatch(OpCodes.Ldarg_0),
        //                 new CodeMatch(OpCodes.Ldfld),
        //                 new CodeMatch(OpCodes.Ldarg_0),
        //                 new CodeMatch(OpCodes.Ldfld),
        //                 new CodeMatch(OpCodes.Ldc_I4_1),
        //                 new CodeMatch(OpCodes.Ldc_I4_0)
        //             );
        //
        //         var loadCardVarInstructions = matcher.InstructionsWithOffsets(-6, -4);
        //
        //         matcher
        //             .InsertAndAdvance(loadCardVarInstructions) // copy in the card load
        //             .InsertAndAdvance(
        //                 CodeInstruction.LoadField(typeof(ShadowfallKeywords), nameof(ShadowfallKeywords.Linger)),
        //                 new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(IReadOnlySet<CardKeyword>), nameof(IReadOnlySet<CardKeyword>.Contains))),
        //                 new CodeInstruction(OpCodes.Brtrue, breakOpcode)
        //             );
        //
        //         // foreach(var instruction in loadCardVarInstructions) {
        //         //     MainFile.Logger.Info(instruction.ToString());
        //         // }
        //         MainFile.Logger.Info("");
        //         // foreach(var instruction in matcher.InstructionEnumeration()) {
        //         //     MainFile.Logger.Info(instruction.ToString());
        //         // }
        //         var enumeration = matcher.InstructionEnumeration().ToArray();
        //         for(var i = 0; i < enumeration.Length; i++) {
        //             MainFile.Logger.Info(i.ToString() + " | " +  enumeration[i].ToString());
        //         }
        //         return matcher.InstructionEnumeration();
        //     }
        //     catch(Exception e){
        //         MainFile.Logger.Info(e.ToString());
        //     }
        //     return instructions;
        // }
    }
}
