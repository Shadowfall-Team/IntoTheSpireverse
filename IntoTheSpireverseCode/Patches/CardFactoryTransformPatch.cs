using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(CardFactory), "GetDefaultTransformationOptions")]
public static class CardFactoryTransformPatch
{
    public static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var poolGetter = AccessTools.PropertyGetter(typeof(CardModel), "Pool");
        var coalesceMethod = AccessTools.Method(typeof(CardFactoryTransformPatch),
            nameof(CoalescePool));

        foreach (var instruction in instructions)
        {
            yield return instruction;
            if (instruction.opcode == OpCodes.Callvirt &&
                instruction.operand is MethodInfo mi && mi == poolGetter)
            {
                yield return new CodeInstruction(OpCodes.Stloc_1);
                yield return new CodeInstruction(OpCodes.Ldloc_1);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, coalesceMethod);
            }
        }
    }

    private static CardPoolModel CoalescePool(CardPoolModel originalPool,
        CardModel original)
    {
        var characterPool = original.Owner.Character.CardPool;
        if (characterPool.AllCardIds.Contains(original.Id) &&
            characterPool != originalPool)
        {
            return characterPool;
        }

        return originalPool;
    }
}