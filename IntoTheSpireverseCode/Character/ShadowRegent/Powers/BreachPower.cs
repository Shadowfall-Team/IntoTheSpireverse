using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class BreachPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyXValue(CardModel card, int originalValue)
    {
        if (Owner != card.Owner.Creature)
        {
            return originalValue;
        }

        return originalValue + Amount;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.EnergyCost.CostsX && cardPlay.Card.Owner.Creature == Owner)
        {
            Flash();
            await PowerCmd.Remove(this);
        }
    }
}