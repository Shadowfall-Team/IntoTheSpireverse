using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class ClaySoldierPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private class Data
    {
        public bool activatedThisTurn;
    }

    protected override object? InitInternalData()
    {
        return new Data();
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<StrengthPower>(2m),
            new PowerVar<SlatePower>(0m)
        ];

    public void SetSlate(decimal slate)
    {
        AssertMutable();
        DynamicVars.Power<SlatePower>().BaseValue = slate;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || GetInternalData<Data>().activatedThisTurn || result.UnblockedDamage <= 0) return;
        GetInternalData<Data>().activatedThisTurn = true;
        await PowerCmd.Apply<ClaySoldierNextTurnStrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner, DynamicVars.Strength.BaseValue, Owner, null);
        await PowerCmd.Apply<SlateNextTurnPower>(
            new ThrowingPlayerChoiceContext(),
            Owner, DynamicVars.Power<SlatePower>().BaseValue, Owner, null
        );
    }
}
