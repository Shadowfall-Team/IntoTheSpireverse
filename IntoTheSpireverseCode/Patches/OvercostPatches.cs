using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowNecrobinder;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
public class OvercostListenerPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel card, ref bool __result)
    {
        if(!__result)
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
public class BeforeEnergySpentListenerPatch
{
    [HarmonyPrefix]
    static void Prefix(CardModel __instance)
    {
        foreach (var model in __instance.Owner.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IBeforeEnergySpentListener energySpentListener)
                energySpentListener.BeforeEnergySpent(__instance);
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