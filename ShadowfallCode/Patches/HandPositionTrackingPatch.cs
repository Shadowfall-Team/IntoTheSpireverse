using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(CardPile), nameof(CardPile.RemoveInternal))]
public static class HandPositionTrackingPatch
{
    internal static readonly Dictionary<CardModel, bool> WasLeftmostInHand = new();

    static void Prefix(CardPile __instance, CardModel card)
    {
        if (__instance.Type == PileType.Hand)
            WasLeftmostInHand[card] = __instance.Cards.Count > 0 && __instance.Cards[0] == card;
    }
}
