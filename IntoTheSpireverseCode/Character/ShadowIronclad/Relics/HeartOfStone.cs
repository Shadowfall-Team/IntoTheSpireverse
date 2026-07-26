using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

public class HeartOfStone : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const string _absorbKey = "Absorb";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(_absorbKey, 8)];

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<HeartOfTheMountain>();

    private int _absorbedThisCombat;
    private int AbsorbedThisCombat
    {
        get => _absorbedThisCombat;
        set
        {
            _absorbedThisCombat = value;
            UpdateDisplay();
        }
    }

    private int _hpBeforeHpLoss;
    private int _finalUnblockedDamage;

    private int RemainingAbsorbAmount => DynamicVars[_absorbKey].IntValue - AbsorbedThisCombat;
    private int EffectiveHp => _hpBeforeHpLoss + RemainingAbsorbAmount;

    public override int DisplayAmount => RemainingAbsorbAmount;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress && DisplayAmount > 0;

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner.Creature)
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
        if (!CombatManager.Instance.IsInProgress || target != Owner.Creature || RemainingAbsorbAmount <= 0 || result.UnblockedDamage <= 0)
            return;

        int absorbed = Math.Min(result.UnblockedDamage, RemainingAbsorbAmount);

        Flash();
        await CreatureCmd.Heal(target, absorbed, false);
        AbsorbedThisCombat += absorbed;
    }

    public override bool ShouldDieLate(Creature creature)
    {
        if (!CombatManager.Instance.IsInProgress || creature != Owner.Creature) return true;

        return _finalUnblockedDamage >= EffectiveHp;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (!CombatManager.Instance.IsInProgress || creature != Owner.Creature) return;

        int absorbed = Math.Min(_finalUnblockedDamage, RemainingAbsorbAmount);

        int postDamageHp = _hpBeforeHpLoss - _finalUnblockedDamage + absorbed;

        Flash();
        await CreatureCmd.Heal(creature, postDamageHp);

        AbsorbedThisCombat += absorbed;
    }

    public override Task BeforeCombatStart()
    {
        UpdateDisplay();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        AbsorbedThisCombat = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private void UpdateDisplay()
    {
        Status = RemainingAbsorbAmount <= 0 ? RelicStatus.Disabled : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }
}
