
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
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

public class OathOfSilencePower : ShadowPowerModel
{
    public const string cardPlayThresholdKey = "CardPlay";
    public const int cardPlayThresholdValue = 3;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => AttacksPlayedThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(cardPlayThresholdKey, cardPlayThresholdValue)
    ];
    

    protected override object InitInternalData() => new Data();

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        Data internalData = GetInternalData<Data>();
        if (internalData.alreadyDeactivatedThisTurn)
            return;
        InvokeDisplayAmountChanged();
        if (AttacksPlayedThisTurn <= cardPlayThresholdValue)
            return;
        internalData.alreadyDeactivatedThisTurn = true;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        
        if (!GetInternalData<Data>().alreadyDeactivatedThisTurn)
            PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, (Decimal) Amount, Owner, null);
        
        GetInternalData<Data>().alreadyDeactivatedThisTurn = false;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private int AttacksPlayedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.CardPlaysFinished.Count((c => c.HappenedThisTurn(Owner.CombatState) && c.CardPlay.Card.Owner == Owner.Player));
        }
    }
    
    private class Data
    {
        public bool alreadyDeactivatedThisTurn;
    }
}