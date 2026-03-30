using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(CardPile), nameof(CardPile.RemoveInternal))]
public static class HandPositionTrackingPatch
{
    internal static readonly Dictionary<CardModel, bool> WasLeftmostInHand = new();
    internal static readonly Dictionary<CardModel, List<CardModel>> AdjacentCards = new();

    static void Prefix(CardPile __instance, CardModel card)
    {
        if (__instance.Type != PileType.Hand)
            return;

        var cards = __instance.Cards;
        WasLeftmostInHand[card] = cards.Count > 0 && cards[0] == card;

        int idx = cards.IndexOf(card);
        var neighbors = new List<CardModel>(2);
        if (idx > 0) neighbors.Add(cards[idx - 1]);
        if (idx < cards.Count - 1) neighbors.Add(cards[idx + 1]);
        AdjacentCards[card] = neighbors;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCombatEnd))]
public static class HandPositionTrackingCleanupPatch
{
    static void Prefix()
    {
        HandPositionTrackingPatch.WasLeftmostInHand.Clear();
        HandPositionTrackingPatch.AdjacentCards.Clear();
    }
}
