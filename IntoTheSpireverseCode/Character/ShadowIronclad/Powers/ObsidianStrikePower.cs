using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class ObsidianStrikePower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    /// Only an explicitly targeted card counts. Cards that hit ALL enemies carry no Target, so
    /// they are not played "against this enemy".
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
