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
    // Health bar rendering is disabled until BaseLib ships the forecast directions it needs.
    //
    // Stone Health used to draw its own bands by patching NHealthBar, which fought BaseLib's overlay
    // for the same nodes (see PR 276, StoneHealthBarPatches). That patch is gone and this override
    // replaces it, but OutwardFromCurrentHp and InwardFromMaxHp do not exist in the published BaseLib
    // yet - they are the subject of an open PR against that project. Uncomment this block once a
    // BaseLib release carrying them is out; the package reference floats, so a restore is all that is
    // needed. Until then the pool has no bands on the bar and only the HP label parenthetical shows.
    //
    // private static readonly Color GreyColor = new("A8A8A8");
    // private static readonly Color WhiteColor = new("FFFFFF");
    //
    // /// <summary>
    // /// Draws the pool onto the health bar: grey for the part that fits below max HP, white for the
    // /// part that overcaps it.
    // ///
    // /// White is emitted first and pinned to the max edge, so at full HP - where grey has nowhere to
    // /// go - it is the only thing that shows, painting back over the red band. BaseLib clips grey to
    // /// whatever white leaves behind, so the two never overlap and one pixel still means one HP.
    // ///
    // /// Neither segment affects the HP label: this is HP the player has, not damage they are about to
    // /// take, so it must never colour the label as lethal.
    // /// </summary>
    // public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
    //     HealthBarForecastContext context)
    // {
    //     if (!IntoTheSpireverseConfig.ShowStoneHealthOnBar) yield break;
    //     if (Amount <= 0) yield break;
    //
    //     var creature = context.Creature;
    //     if (creature.CurrentHp <= 0 || creature.MaxHp <= 0) yield break;
    //
    //     var overflow = Math.Max(0, creature.CurrentHp + Amount - creature.MaxHp);
    //     if (overflow > 0)
    //     {
    //         yield return new HealthBarForecastSegment(
    //             overflow, WhiteColor, HealthBarForecastDirection.InwardFromMaxHp)
    //         {
    //             AffectsHpLabel = false,
    //         };
    //     }
    //
    //     yield return new HealthBarForecastSegment(
    //         Amount, GreyColor, HealthBarForecastDirection.OutwardFromCurrentHp)
    //     {
    //         AffectsHpLabel = false,
    //     };
    // }

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
