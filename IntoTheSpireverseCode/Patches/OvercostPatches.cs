using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal static class OvercostListenerPatch
{
    [HarmonyPostfix]
    internal static void Postfix(CardModel card, ref bool __result)
    {
        if (!__result)
        {
            foreach (var model in card.Owner.Creature.CombatState!.IterateHookListeners().ToList())
            {
                if (model is IOvercostListener overcostListener)
                    __result |= overcostListener.ShouldPlayAnyway(card);
            }
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class BeforeEnergySpentListenerPatch
{
    [HarmonyPrefix]
    internal static async void Prefix(CardModel __instance)
    {
        foreach (var model in __instance.Owner.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IBeforeEnergySpentListener energySpentListener)
                await energySpentListener.BeforeEnergySpent(__instance);
        }
    }
}

public interface IOvercostListener
{
    bool ShouldPlayAnyway(CardModel card);
}

public interface IBeforeEnergySpentListener
{
    Task BeforeEnergySpent(CardModel card);
}
