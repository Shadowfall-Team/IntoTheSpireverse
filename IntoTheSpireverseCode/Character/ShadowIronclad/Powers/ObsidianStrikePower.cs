using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// The marked card is played twice rather than copied, so the repeat is a Replay: it resolves
/// inside the same play, keeps the card's own target, and counts as Indirect for everything that
/// keys off that.
///
/// The count is raised through ModifyCardPlayCount, which the engine calls once per play in
/// CardModel.OnPlayWrapper before the play loop starts. That timing also settles two things the
/// old copy-to-pile version needed explicit guards for: the Obsidian Strike that applies this mark
/// does so during its own OnPlay, which is after its play count was generated, so it can never
/// trigger its own mark; and the repeat is part of the same play rather than a fresh one, so it
/// cannot re-enter this hook and consume the next stack.
/// </summary>
public sealed class ObsidianStrikePower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Only an explicitly targeted card counts. Cards that hit ALL enemies carry no Target, so
    /// they are not played "against this enemy".
    /// </summary>
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (target != Owner) return playCount;
        if (IntoTheSpireverseKeywords.WillBePlayedIndirectly(card)) return playCount;

        return playCount + 1;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        await PowerCmd.Decrement(this);
    }
}
