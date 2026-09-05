using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// A pool of HP that is refunded as it is lost, and that counts toward surviving a lethal hit.
///
/// Every source of the effect stacks into this one power rather than tracking its own pool. Two
/// separate pools each absorbing the same hit would refund it twice, so a 3 damage hit against 5+5
/// would have healed for 6.
///
/// AfterDamageReceived never runs on a lethal hit, so the pool has to be counted as effective HP
/// before the death check: ModifyHpLostAfterOstyLate captures the incoming loss, ShouldDieLate
/// compares it against HP plus the pool, and AfterPreventingDeath settles the survivor's HP.
/// </summary>
public sealed class StoneHealthPower : ShadowPowerModel
{
    private int _hpBeforeHpLoss;
    private int _finalUnblockedDamage;

    private int EffectiveHp => _hpBeforeHpLoss + Amount;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner)
        {
            _hpBeforeHpLoss = target.CurrentHp;
            _finalUnblockedDamage = (int)amount;
        }
        return amount;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress) return;
        if (target != Owner || Owner.IsDead) return;
        if (result.UnblockedDamage <= 0 || Amount <= 0) return;

        int absorbed = Math.Min(result.UnblockedDamage, Amount);
        Flash();
        await CreatureCmd.Heal(Owner, absorbed, false);
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -absorbed, Owner, null);
    }

    public override bool ShouldDieLate(Creature creature)
    {
        if (!CombatManager.Instance.IsInProgress || creature != Owner) return true;
        return _finalUnblockedDamage >= EffectiveHp;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (!CombatManager.Instance.IsInProgress || creature != Owner) return;

        int absorbed = Math.Min(_finalUnblockedDamage, Amount);
        int postDamageHp = _hpBeforeHpLoss - _finalUnblockedDamage + absorbed;
        Flash();
        await CreatureCmd.Heal(creature, postDamageHp);
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -absorbed, Owner, null);
    }
}
