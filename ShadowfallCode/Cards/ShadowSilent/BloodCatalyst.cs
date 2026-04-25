using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class BloodCatalyst() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int multiplier = IsUpgraded ? 2 : 1;

        int bleed = cardPlay.Target.GetPowerAmount<BleedPower>();
        if (bleed > 0)
            await PowerCmd.Apply<BleedPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, bleed * multiplier, Owner.Creature, this);

        int weak = cardPlay.Target.GetPowerAmount<WeakPower>();
        if (weak > 0)
            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, weak * multiplier, Owner.Creature, this);

        int vulnerable = cardPlay.Target.GetPowerAmount<VulnerablePower>();
        if (vulnerable > 0)
            await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, vulnerable * multiplier, Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}
