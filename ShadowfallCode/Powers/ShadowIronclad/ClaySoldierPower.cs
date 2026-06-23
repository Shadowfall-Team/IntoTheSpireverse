using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
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

public sealed class ClaySoldierPower : ShadowPowerModel, IHasSecondAmount
{
    private class Data
    {
        public bool activatedThisTurn; // technically "last turn" by the time its used
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object? InitInternalData()
    {
        return new Data();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(0m),
        new PowerVar<SlatePower>(0m),
    ];

    public override int DisplayAmount => DynamicVars.Strength.IntValue;
    public string GetSecondAmount()
    {
        return DynamicVars.Power<SlatePower>().IntValue.ToString();
    }

    public void AddVars(decimal slate, decimal strength)
    {
        AssertMutable();
        DynamicVars.Power<SlatePower>().BaseValue += slate;
        this.InvokeSecondAmountChanged();
        DynamicVars.Strength.BaseValue += strength;
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext,
        Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || GetInternalData<Data>().activatedThisTurn || result.UnblockedDamage <= 0) return;
        GetInternalData<Data>().activatedThisTurn = true;
        // set to flash/indicate as ready? do powers ever do that?
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side is not CombatSide.Player) return;

        if (!GetInternalData<Data>().activatedThisTurn) return;

        Flash();
        await PowerCmd.Apply<ClaySoldierTemporaryStrengthPower>(new ThrowingPlayerChoiceContext(),
            Owner, DynamicVars.Strength.BaseValue, Owner, null);
        await PowerCmd.Apply<SlatePower>(new ThrowingPlayerChoiceContext(),
            Owner, DynamicVars.Power<SlatePower>().BaseValue, Owner, null);

        GetInternalData<Data>().activatedThisTurn = false;
    }
}
