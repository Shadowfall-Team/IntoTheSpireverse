using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TradeRoutes() : ShadowRegentCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<TradeRoutesPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            this);

    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}