using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Captures OnPlayWrapper's isAutoPlay for <see cref="Keywords.IntoTheSpireverseKeywords.WillBePlayedIndirectly"/>,
/// which runs before any CardPlay exists. Entries are only valid during that call; the next play
/// overwrites them and combat end clears them.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class AutoPlayFlagPatch
{
    private static readonly Dictionary<CardModel, bool> IsAutoPlayByCard = new();

    static void Prefix(CardModel __instance, bool isAutoPlay) => IsAutoPlayByCard[__instance] = isAutoPlay;

    public static bool IsCurrentPlayAuto(CardModel card) =>
        IsAutoPlayByCard.TryGetValue(card, out var isAutoPlay) && isAutoPlay;

    public static void Clear() => IsAutoPlayByCard.Clear();
}
