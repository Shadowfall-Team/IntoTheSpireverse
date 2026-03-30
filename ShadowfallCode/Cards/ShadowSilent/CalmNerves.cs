using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class CalmNerves() : ShadowSilentCard(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cunning),
    ];

    protected override bool ShouldGlowGoldInternal => ShadowfallKeywords.IsCunningActive(this);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (ShadowfallKeywords.IsCunningTriggered(this))
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
