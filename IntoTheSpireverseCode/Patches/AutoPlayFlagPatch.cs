using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Records whether the play currently starting is an auto-play, for the window before the card's
/// CardPlay objects exist.
///
/// IntoTheSpireverseKeywords.WasPlayedIndirectly answers the same question from a CardPlay, but
/// CardModel.OnPlayWrapper builds those only once it is inside the play loop. Anything that has to
/// decide before then - ModifyCardPlayCount in particular, which runs earlier in the same method -
/// has nothing but the card itself to go on. OnPlayWrapper's isAutoPlay argument is that missing
/// bit, so it is captured on the way in.
///
/// The entry is only meaningful while that wrapper call is on the stack. It is left behind
/// afterwards rather than cleared, because the next play of the same card overwrites it and the
/// whole table is dropped at the end of combat.
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
