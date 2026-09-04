using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Records the pile each card most recently left, so a card resolving in the Play pile can tell
/// whether it was played from Hand ("directly") or from anywhere else ("Indirectly").
/// </summary>
[HarmonyPatch(typeof(CardPile), nameof(CardPile.RemoveInternal))]
public static class IndirectPlayTrackingPatch
{
    internal static readonly Dictionary<CardModel, PileType> LastPileLeft = new();

    static void Prefix(CardPile __instance, CardModel card)
    {
        LastPileLeft[card] = __instance.Type;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCombatEnd))]
public static class IndirectPlayTrackingCleanupPatch
{
    static void Prefix() => IndirectPlayTrackingPatch.LastPileLeft.Clear();
}
