using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

public class HeartOfTheMountain : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(16)];

    private int _healedThisCombat;

    private int HealedThisCombat
    {
        get { return _healedThisCombat; }
        set
        {
            _healedThisCombat = value;
            UpdateDisplay();
        }
    }

    public override int DisplayAmount => DynamicVars.Heal.IntValue - HealedThisCombat;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress && DisplayAmount > 0;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (HealedThisCombat >= DynamicVars.Heal.IntValue || target != Owner.Creature || result.UnblockedDamage <= 0)
            return;

        Flash();
        var healAmount = Math.Clamp(result.UnblockedDamage, 0, DynamicVars.Heal.IntValue - HealedThisCombat);
        await CreatureCmd.Heal(target, healAmount, false);
        HealedThisCombat += healAmount;

        UpdateDisplay();
    }

    public override Task BeforeCombatStart()
    {
        UpdateDisplay();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        HealedThisCombat = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private void UpdateDisplay()
    {
        Status = HealedThisCombat >= DynamicVars.Heal.IntValue ? RelicStatus.Disabled : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }
}
