using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.ShouldGlowGold), MethodType.Getter)]
internal static class CardGlowGoldListenerPatch
{
    [HarmonyPostfix]
    internal static void Postfix(CardModel __instance, ref bool __result)
    {
        if (!__result)
        {
            foreach (var model in __instance.Owner.Creature.CombatState!.IterateHookListeners().ToList())
            {
                if (model is ICardGlowGoldListener glowGoldListener)
                    __result |= glowGoldListener.ShouldCardGlowGold(__instance);
            }
        }
    }
}

public interface ICardGlowGoldListener
{
    bool ShouldCardGlowGold(CardModel card);
}
