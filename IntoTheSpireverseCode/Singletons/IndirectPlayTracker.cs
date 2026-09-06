using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

/// <summary>
/// Records the pile each card most recently left.
///
/// This is the secondary half of the Indirectly check. CardPlay.IsAutoPlay is the primary signal
/// and catches everything that routes through CardCmd.AutoPlay, which is every non-manual play the
/// game currently has. This tracking stays behind it so a card that reaches the Play pile from
/// somewhere other than Hand without going through AutoPlay is still treated as Indirect.
/// See IntoTheSpireverseKeywords.WasPlayedIndirectly.
///
/// Both play paths dispatch AfterCardChangedPiles before OnPlay runs - the manual one at the end of
/// CardPileCmd.AddDuringManualCardPlay, the autoplay one at the end of CardPileCmd.Add - so the
/// entry is always current by the time a card asks about itself.
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
        return Task.CompletedTask;
    }
}
