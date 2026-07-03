using System.Reflection.Emit;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Allows <c>EnchantBlockAdditive</c> and <c>EnchantBlockMultiplicative</c> to apply without a card source.
/// Call <c>CreatureCmd.GainBlock</c> inside <see cref="WithEnchantment"/>.
/// </summary>
public static class EnchantBlockWithoutCardPlayPatch
{
    private static readonly SpireField<PlayerCombatState, EnchantmentModel> Enchantment = new(() =>
        null
    );

    public static async Task WithEnchantment(
        EnchantmentModel? enchantment,
        PlayerCombatState? combatState,
        Func<Task> action
    )
    {
        if (enchantment == null || combatState == null)
        {
            await action();
            return;
        }

        try
        {
            Enchantment.Set(combatState, enchantment);
            await action();
        }
        finally
        {
            Enchantment.Set(combatState, null);
        }
    }

    private static bool GetEnchantment(
        CardModel? cardSource,
        Creature target,
        out EnchantmentModel? enchantment
    )
    {
        if (
            target?.Player?.PlayerCombatState is PlayerCombatState combatState
            && Enchantment.Get(combatState) is EnchantmentModel enchant
        )
        {
            enchantment = enchant;
            return true;
        }

        if (cardSource?.Enchantment != null)
        {
            enchantment = cardSource.Enchantment;
            return true;
        }

        enchantment = null;
        return false;
    }

    [HarmonyPatch(typeof(Hook), nameof(Hook.ModifyBlock))]
    private static class Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var codeMatcher = new CodeMatcher(instructions);

            codeMatcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_S), // load cardSource
                new CodeMatch(OpCodes.Brfalse_S), // if cardSource == null, skip if block
                new CodeMatch(OpCodes.Ldarg_S), // load cardSource
                new CodeMatch(i => i.opcode == OpCodes.Callvirt), // cardSource.Enchantment
                new CodeMatch(OpCodes.Brfalse_S), // if cardSource.Enchantment == null, skip if block
                new CodeMatch(OpCodes.Ldarg_S), // load cardSource
                new CodeMatch(i => i.opcode == OpCodes.Callvirt), // cardSource.Enchantment
                new CodeMatch(OpCodes.Stloc_2) // store into enchantment (local 2)
            );

            if (codeMatcher.IsInvalid)
            {
                MainFile.Logger.Warn("failed to match during EnchantBlockWithoutCardPlayPatch");
                return instructions;
            }

            // capture jump target (where to go if the condition is false)
            var skipBlockLabel = codeMatcher.InstructionAt(1).operand;

            codeMatcher
                .RemoveInstructions(8)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, 4), // load cardSource (arg 4)
                    new CodeInstruction(OpCodes.Ldarg_S, 1), // load target (arg 1)
                    new CodeInstruction(OpCodes.Ldloca_S, 2), // load enchantment (local 2)
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(
                            typeof(EnchantBlockWithoutCardPlayPatch),
                            nameof(GetEnchantment)
                        )
                    ), // call GetEnchantment, which will set enchantment local via 'out'
                    new CodeInstruction(OpCodes.Brfalse_S, skipBlockLabel) // if GetEnchantment returns false, skip
                );

            return codeMatcher.InstructionEnumeration();
        }
    }
}
