using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class QuickEscape() : ShadowSilentCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cunning),
    ];

    protected override bool ShouldGlowGoldInternal => ShadowfallKeywords.IsCunningActive(this);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)
    {
        if (ShadowfallKeywords.IsCunningActive(this) && !_costReduced)
        {
            EnergyCost.SetUntilPlayed(0);
            _costReduced = true;
        }
        else if (!ShadowfallKeywords.IsCunningActive(this))
        {
            _costReduced = false;
        }

        return Task.CompletedTask;
    }

    private bool _costReduced;

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
