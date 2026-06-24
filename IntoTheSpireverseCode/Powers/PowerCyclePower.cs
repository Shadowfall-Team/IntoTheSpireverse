using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers;

public sealed class PowerCyclePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => (object) new PowerCyclePower.Data();
    
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner.Player || cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;
        this.GetInternalData<PowerCyclePower.Data>().amountsForPlayedCards.Add(cardPlay.Card, this.Amount);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        int cycleCount;
        if (cardPlay.Card.Owner != Owner.Player || !GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out cycleCount) || cycleCount <= 0)
            return;
        Flash();
        await CycleCmd.Cycle(context, cardPlay.Card.Owner, cycleCount);
    }
    
    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
    }
}