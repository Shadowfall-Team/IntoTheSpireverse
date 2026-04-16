using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class SmithingStrikePatch
{
    [ThreadStatic]
    public static CardModel? LeftNeighbor;
    [ThreadStatic]
    public static CardModel? RightNeighbor;

    public static void Prefix(CardModel __instance)
    {
        if (__instance is not SmithingStrike)
            return;

        var hand = PileType.Hand.GetPile(__instance.Owner).Cards.ToList();
        int index = hand.IndexOf(__instance);

        LeftNeighbor = index > 0 ? hand[index - 1] : null;
        RightNeighbor = index >= 0 && index < hand.Count - 1 ? hand[index + 1] : null;
    }
}