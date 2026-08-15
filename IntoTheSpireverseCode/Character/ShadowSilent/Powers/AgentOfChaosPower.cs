using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class AgentOfChaosPower : ShadowPowerModel, IModifyCardPlayResultLocation
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
        if (Owner.Player == null || Owner.Player.PlayerCombatState == null) return Task.CompletedTask;
        foreach (CardModel card in Owner.Player.PlayerCombatState.AllCards.Where(c => c.Type == CardType.Attack))
            CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        return Task.CompletedTask;
    }
    
    public override async Task AfterCardDiscarded(
        PlayerChoiceContext choiceContext, CardModel card)
    {
        if (Owner.CombatState == null) return;
        if (card.Owner != Owner.Player)
            return;
        if (Owner.Side != Owner.CombatState.CurrentSide)
            return;

        Flash();
        if (!card.Keywords.Contains(CardKeyword.Sly))
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        else
        {
            GetInternalData<Data>().DiscardedSlyCards.Add(card);
        }
    }
    
    public CardLocationCompatibility ModifyCardPlayResultLocationCompatibility(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocationCompatibility location)
    {
        if (card.Owner.Creature != Owner)
            return location;
        if (!GetInternalData<Data>().DiscardedSlyCards.Contains(card))
            return location;
        return new CardLocationCompatibility(card.Owner, PileType.Exhaust, CardPilePosition.Bottom);
    }
    
    
    private class Data
    {
        public List<CardModel> DiscardedSlyCards = new();
    }
}

public record struct CardLocationCompatibility(Player Player, PileType PileType, CardPilePosition Position);