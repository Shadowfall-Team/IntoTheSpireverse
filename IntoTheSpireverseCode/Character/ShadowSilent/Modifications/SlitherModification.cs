using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Modifications;

public sealed class SlitherModification : Modification
{
    protected override bool AppendsTextToCardDescription => false;
    public override void AddTips(List<IHoverTip> tips)
    {
        base.AddTips(tips);
        tips.Add(HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle));
    }

    protected override ModelId SourceCardId => ModelDb.Card<Perplex>().Id;
    
    public override Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != Owner || Owner.Pile.Type != PileType.Hand)
            return Task.CompletedTask;
        IntoTheSpireverseKeywords.ApplyMuddle(Owner);
        return Task.CompletedTask;
    }

}