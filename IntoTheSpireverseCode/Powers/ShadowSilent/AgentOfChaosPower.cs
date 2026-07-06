
using BaseLib.Abstracts;
using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
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

public class AgentOfChaosPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];  
    
    protected override object InitInternalData() => new Data();
    

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Type != CardType.Attack  || card.Owner != this.Owner.Player)
            return Task.CompletedTask;
        CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        foreach (CardModel card in this.Owner.Player.PlayerCombatState.AllCards.Where(c => c.Type == CardType.Attack))
            CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        return Task.CompletedTask;
    }
    
    public override async Task AfterCardDiscarded(
        PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner.Player)
            return;
        if (Owner.Side != Owner.CombatState.CurrentSide)
            return;

        Flash();
        if (!card.Keywords.Contains(CardKeyword.Sly))
        {
            await CardCmd.Exhaust(choiceContext, card, false, false);
        }
        else
        {
            GetInternalData<Data>().discardedSlyCards.Add(card);
        }
    }
    
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        PileType pileType,
        CardPilePosition position)
    {
        if (card.Owner.Creature != this.Owner)
            return (pileType, position);
        return !GetInternalData<Data>().discardedSlyCards.Contains(card) ? (pileType, position) : (PileType.Exhaust, position);
    }
    
    
    private class Data
    {
        public List<CardModel> discardedSlyCards = new();
    }
}