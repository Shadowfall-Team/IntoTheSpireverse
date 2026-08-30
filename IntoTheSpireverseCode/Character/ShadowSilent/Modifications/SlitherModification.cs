using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

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

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != Owner || Owner.Pile == null || Owner.Pile.Type != PileType.Hand)
            return;
        await IntoTheSpireverseKeywords.ApplyMuddle(Owner);
        return;
    }

}