using BaseLib.Hooks;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Every source stacks into this one power, since separate pools would each refund the same hit.
/// AfterDamageReceived never runs on a lethal hit, so ShouldDieLate counts the pool as HP before
/// the death check.
/// </summary>
public sealed class StoneHealthPower : ShadowPowerModel
{
    private static readonly Color GreyColor = new("A8A8A8");
    private static readonly Color WhiteColor = new("FFFFFF");

    /// <summary>
    /// Grey for the pool below max HP, white for the overcap. White is emitted first and pinned to
    /// the max edge so it still shows at full HP; BaseLib clips grey to what is left. Neither tints
    /// the HP label, because this is HP the player has, not damage incoming.
    /// </summary>
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        if (!IntoTheSpireverseConfig.ShowStoneHealthOnBar) yield break;
        if (Amount <= 0) yield break;

        var creature = context.Creature;
        if (creature.CurrentHp <= 0 || creature.MaxHp <= 0) yield break;

        var overflow = Math.Max(0, creature.CurrentHp + Amount - creature.MaxHp);
        if (overflow > 0)
        {
            yield return new HealthBarForecastSegment(
                overflow, WhiteColor, HealthBarForecastDirection.InwardFromMaxHp)
            {
                AffectsHpLabel = false,
            };
        }

        yield return new HealthBarForecastSegment(
            Amount, GreyColor, HealthBarForecastDirection.OutwardFromCurrentHp)
        {
            AffectsHpLabel = false,
        };
    }

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
        await PowerCmd.ModifyAmount(choiceContext, this, -absorbed, Owner, null);
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
