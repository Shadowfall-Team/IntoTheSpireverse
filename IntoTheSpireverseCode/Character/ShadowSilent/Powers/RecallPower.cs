using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class RecallPower : ShadowPowerModel, ICardDestinationListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
    ];
    
    protected override object InitInternalData() => new Data();

    public CardDestination ModifyCardDestination(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardDestination destination)
    {
        if (card.Owner.Creature != Owner || card.IsDupe || destination.PileType == PileType.None || destination.PileType == PileType.Exhaust)
            return destination;
        return destination with { PileType = PileType.Hand, Position = CardPilePosition.Top };
    }

    public async Task AfterCardDestinationModified(CardModel card, CardDestination destination)
    {
        var internalData = GetInternalData<Data>();
        internalData.wasTriggered = true;
    }
    
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var internalData = GetInternalData<Data>();
        if (internalData.wasTriggered && cardPlay.Card.Owner == Owner.Player)
        {
            internalData.wasTriggered = false;
            void MuddleAfterPlay()
            {
                cardPlay.Card.Played -= MuddleAfterPlay;
                _ = IntoTheSpireverseKeywords.ApplyMuddle(cardPlay.Card);
            }
            cardPlay.Card.Played += MuddleAfterPlay;
            Flash();
            await PowerCmd.Decrement(this);
        }
    }
    
    private class Data
    {
        public bool wasTriggered;
    }
}