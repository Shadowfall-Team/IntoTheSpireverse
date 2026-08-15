using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class RecallPower : ShadowPowerModel, IModifyCardPlayResultLocation
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public CardLocationCompatibility ModifyCardPlayResultLocationCompatibility(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocationCompatibility location)
    {
        if (card.Owner.Creature != Owner)
            return location;
        if (card.IsDupe)
            return location;
        if (card is {Type: CardType.Power})
            return location;
        return new CardLocationCompatibility(card.Owner, PileType.Hand, CardPilePosition.Top);
    }

    public Task AfterModifyingCardPlayResultLocationCompatibility(
        CardModel card, CardLocationCompatibility location)
    {
        Flash();
        PowerCmd.Decrement(this);
        return Task.CompletedTask;
    }
}