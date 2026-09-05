using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Records the pile each card most recently left.
///
/// This is the secondary half of the Indirectly check. CardPlay.IsAutoPlay is the primary signal
/// and catches everything that routes through CardCmd.AutoPlay, which is every non-manual play the
/// game currently has. This tracking stays behind it so a card that reaches the Play pile from
/// somewhere other than Hand without going through AutoPlay is still treated as Indirect.
/// See IntoTheSpireverseKeywords.WasPlayedIndirectly.
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
