using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class BrilliancePower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => new Data();
    
    public override async Task AfterCardDrawnEarly(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        Data internalData =  GetInternalData<Data>();
        if (card.Owner.Creature == Owner && Filter(card) && internalData.cardsMuddledThisTurn < Amount)
        {
            IntoTheSpireverseKeywords.ApplyMuddle(card);
            ++internalData.cardsMuddledThisTurn;
        }
    }
    
    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;
        GetInternalData<Data>().cardsMuddledThisTurn = 0;
        return Task.CompletedTask;
    }
    
    private bool Filter(CardModel card)
    {
        return card.EnergyCost.GetWithModifiers(CostModifiers.All) >= 3 && IntoTheSpireverseKeywords.CanMuddle(card);
    }
    
    private class Data
    {
        public int cardsMuddledThisTurn;
    }
}