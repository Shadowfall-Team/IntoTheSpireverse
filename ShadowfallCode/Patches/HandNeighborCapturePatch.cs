using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;
using Shadowfall.ShadowfallCode.Interfaces;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class HandNeighborCapturePatch
{
    public static void Prefix(CardModel __instance)
    {
        if (__instance is not IHandNeighborAware aware) return;

        var hand = PileType.Hand.GetPile(__instance.Owner).Cards.ToList();
        int index = hand.IndexOf(__instance);
        aware.CapturedLeftNeighbor = index > 0 ? hand[index - 1] : null;
        aware.CapturedRightNeighbor = index >= 0 && index < hand.Count - 1 ? hand[index + 1] : null;
    }
}