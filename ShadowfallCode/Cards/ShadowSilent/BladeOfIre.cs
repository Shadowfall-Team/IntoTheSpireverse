using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class BladeOfIre() : ShadowSilentCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
        new PowerVar<BladeOfIrePower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    IsUpgraded ?
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<BladeOfIrePower>(),
    ] : 
    [   HoverTipFactory.FromPower<BladeOfIrePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
        }
        await PowerCmd.Apply<BladeOfIrePower>(Owner.Creature, DynamicVars[nameof(BladeOfIrePower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }

}
