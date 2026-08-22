using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class RecallPower : ShadowPowerModel, ICardDestinationListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public CardDestination ModifyCardDestination(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardDestination destination)
    {
        if (card.Owner.Creature != Owner)
            return destination;
        if (card.IsDupe)
            return destination;
        if (destination.PileType == PileType.None)
            return destination;
        return destination with { PileType = PileType.Hand, Position = CardPilePosition.Top };
    }

    public Task AfterCardDestinationModified(CardModel card, CardDestination destination)
    {
        Flash();
        PowerCmd.Decrement(this);
        return Task.CompletedTask;
    }
}