using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class RecallPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        if (card.Owner.Creature != Owner)
            return location;
        if (card.IsDupe)
            return location;
        if (location.pileType == PileType.None)
            return location;
        location.pileType = PileType.Hand;
        location.position = CardPilePosition.Top;
        return location;
    }

    public override Task AfterModifyingCardPlayResultLocation(
        CardModel card, CardLocation location)
    {
        Flash();
        PowerCmd.Decrement(this);
        return Task.CompletedTask;
    }
}