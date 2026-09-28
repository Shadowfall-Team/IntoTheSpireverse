using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// This patch is required as Obsidian Strike's play count is only changed for direct card plays,
/// but ModifyCardPlayCount does not have an isAutoPlay flag to check against. If at some point in
/// the future ModifyCardPlayCount is changed to only apply to direct plays, this patch can be removed.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper), MethodType.Async)]
public static class ObsidianStrikePlayCountPatch
{
    private static readonly MethodInfo GeneratePlayCount = AccessTools.Method(typeof(CardModel), "GeneratePlayCount",
        [typeof(ICombatState), typeof(Creature)]);

    private static readonly MethodInfo AdjustPlayCountAsync = AccessTools.Method(typeof(ObsidianStrikePower),
        nameof(ObsidianStrikePower.AdjustPlayCount));

    private static readonly Type StateMachine =
        AccessTools.Method(typeof(CardModel), nameof(CardModel.OnPlayWrapper))
            .GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType
        ?? throw new MissingMethodException("No state machine found for OnPlayWrapper");

    private static readonly FieldInfo CardField = AccessTools.Field(StateMachine, "<>4__this");
    private static readonly FieldInfo TargetField = AccessTools.Field(StateMachine, "target");
    private static readonly FieldInfo AutoPlayField = AccessTools.Field(StateMachine, "isAutoPlay");

    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            yield return instruction;
            if (!instruction.Calls(GeneratePlayCount)) continue;

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, CardField);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, TargetField);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AutoPlayField);
            yield return new CodeInstruction(OpCodes.Call, AdjustPlayCountAsync);
        }
    }
}