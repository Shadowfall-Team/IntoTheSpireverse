using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Exists for Obsidian Strike: "the next card directly played against this enemy is played twice."
/// <see cref="Character.ShadowIronclad.Powers.ObsidianStrikePower"/> adds the extra play in
/// ModifyCardPlayCount, which runs before any CardPlay exists, so CardPlay.IsAutoPlay is not yet
/// readable there. The last-pile check catches autoplays from other piles (Havoc's Draw pile), but
/// a card autoplayed straight out of Hand would otherwise count as direct, be doubled, and consume
/// the power.
/// Only records OnPlayWrapper's isAutoPlay; it does not change play or effect order. Entries are
/// only valid during that call; the next play overwrites them and combat end clears them.
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
