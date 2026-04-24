using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Bloodtap() : ShadowSilentCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BloodtapPower>(1m),
        new PowerVar<InstinctPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BleedPower>(),
        HoverTipFactory.FromPower<InstinctPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InstinctPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[nameof(InstinctPower)].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<BloodtapPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[nameof(BloodtapPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BloodtapPower)].UpgradeValueBy(1m);
    }
}
