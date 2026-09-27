using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

/// <summary>
/// The pile each card last left, for the pile check in
/// <see cref="Keywords.IntoTheSpireverseKeywords.WasPlayedIndirectly"/>. Both play paths fire
/// AfterCardChangedPiles before OnPlay, so the entry is current when a card checks itself.
/// </summary>
public class IndirectPlayTracker() : CustomSingletonModel(HookType.Combat)
{
    private static readonly Dictionary<CardModel, PileType> LastPileLeft = new();

    public static bool TryGetLastPileLeft(CardModel card, out PileType pile) =>
        LastPileLeft.TryGetValue(card, out pile);

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        LastPileLeft[card] = oldPileType;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        LastPileLeft.Clear();
        AutoPlayFlagPatch.Clear();
        TransformPayoutPatches.Clear();
        return Task.CompletedTask;
    }

    // Also cleared on the way in: a run lost mid-play leaves a play task that never completes, and
    // its unreleased depth would otherwise hold every later Mud payout forever.
    public override Task BeforeCombatStart()
    {
        TransformPayoutPatches.Clear();
        return Task.CompletedTask;
    }
}
