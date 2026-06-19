using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(AttackCommand))]
public static class AttackCommandPatches
{
    [HarmonyPatch("GetPossibleTargets")]
    [HarmonyPrefix]
    public static bool GetPossibleTargetsPatch(ref IReadOnlyList<Creature> __result,
        AttackCommand __instance)
    {
        if (__instance.ModelSource != null && __instance.ModelSource.Id == ModelDb.Card<AmmoVolley>().Id &&
            __instance._combatState != null)
        {
            __result = SelectTarget(__instance._combatState);
            return false;
        }

        return true;
    }

    private static IReadOnlyList<Creature> SelectTarget(ICombatState combatState)
    {
        var validTargets = combatState.Enemies.Where(e => e.IsAlive).ToList();
        var prefTargets = validTargets.Where(t => t.HasPower<TargetedPower>()).ToList();
        return prefTargets.Count != 0 ? prefTargets : validTargets;
    }
}