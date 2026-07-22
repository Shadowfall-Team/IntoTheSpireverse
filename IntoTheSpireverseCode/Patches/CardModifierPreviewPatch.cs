using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Runs a card's CardModifiers through the dynamic var preview pass alongside the card's own
/// vars, so a modifier's displayed numbers reflect Strength, Dexterity and the like.
///
/// BaseLib ships a transpiler for this (UpdateModifierPreview) but it does not take effect on
/// this game build - instrumenting a modifier showed its UpdateDynamicVarPreview was never
/// called while the host card's vars previewed correctly. This postfix does the same job in a
/// form that cannot silently fail to match. If BaseLib's transpiler starts working again the
/// two are harmless together: the preview pass just recomputes the same value.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.UpdateDynamicVarPreview))]
public static class CardModifierPreviewPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        CardModel __instance,
        CardPreviewMode previewMode,
        Creature? target,
        DynamicVarSet dynamicVarSet)
    {
        // The card calls this once for its own vars and again for its enchantment's. Forward
        // only on the pass for its own set, so modifiers are updated exactly once.
        if (!ReferenceEquals(dynamicVarSet, __instance.DynamicVars)) return;

        var runGlobalHooks = ShouldRunGlobalHooks(__instance);
        foreach (var modifier in CardModifier.Modifiers(__instance))
        {
            modifier.UpdateDynamicVarPreview(previewMode, target, runGlobalHooks);
        }
    }

    /// <summary>
    /// Mirrors the gate CardModel.UpdateDynamicVarPreview applies to its own vars: hooks only
    /// run for cards that are actually in play, or when previewing an upgrade during combat.
    /// </summary>
    private static bool ShouldRunGlobalHooks(CardModel card) =>
        card.CombatState != null
        && (card.Pile?.Type is PileType.Hand or PileType.Play
            || card.UpgradePreviewType == CardUpgradePreviewType.Combat);
}
