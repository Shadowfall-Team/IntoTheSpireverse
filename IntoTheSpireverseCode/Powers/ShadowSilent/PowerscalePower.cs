
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public class PowerscalePower : ShadowPowerModel
{
    public const string strengthAppliedKey = "StrengthApplied";
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        DynamicVars[strengthAppliedKey].IntValue != 0 ? PowerStackType.Counter : PowerStackType.None;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1M),
        new DynamicVar(strengthAppliedKey, 0M)
    ];
    

    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != this.Owner)
            return Task.CompletedTask;
        this.GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, DynamicVars.Strength.IntValue);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount;
        if (cardPlay.Card.Owner.Creature != Owner || !GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out amount) || amount <= 0)
            return;
        if (cardPlay.Card is not Scale)
            return;
        Flash();
        
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, amount, Owner, null, true);
        DynamicVars[strengthAppliedKey].BaseValue += DynamicVars.Strength.IntValue;
        InvokeDisplayAmountChanged();
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -DynamicVars["StrengthApplied"].BaseValue, Owner, null, true);
        DynamicVars[strengthAppliedKey].BaseValue = 0;
    }
    
    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
    }
}