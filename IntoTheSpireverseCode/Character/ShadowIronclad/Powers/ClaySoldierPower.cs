using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class ClaySoldierPower : ShadowPowerModel, IHasSecondAmount
{
    private class Data
    {
        // Damage taken during the player's turn and during the enemy's turn are tracked separately, so a
        // hit on each side stacks up and both resolve at the start of the player's next turn.
        public bool activatedOnPlayerTurn;
        public bool activatedOnEnemyTurn;
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
        if (target != Owner || result.UnblockedDamage <= 0) return;

        var data = GetInternalData<Data>();
        if (CombatState?.CurrentSide == CombatSide.Enemy)
            data.activatedOnEnemyTurn = true;
        else
            data.activatedOnPlayerTurn = true;
        // set to flash/indicate as ready? do powers ever do that?
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side is not CombatSide.Player) return;

        var data = GetInternalData<Data>();
        var triggers = (data.activatedOnPlayerTurn ? 1 : 0) + (data.activatedOnEnemyTurn ? 1 : 0);
        if (triggers == 0) return;

        data.activatedOnPlayerTurn = false;
        data.activatedOnEnemyTurn = false;

        for (var i = 0; i < triggers; i++)
        {
            Flash();
            await PowerCmd.Apply<ClaySoldierTemporaryStrengthPower>(new ThrowingPlayerChoiceContext(),
                Owner, DynamicVars.Strength.BaseValue, Owner, null);
            await PowerCmd.Apply<SlatePower>(new ThrowingPlayerChoiceContext(),
                Owner, DynamicVars.Power<SlatePower>().BaseValue, Owner, null);
        }
    }
}
