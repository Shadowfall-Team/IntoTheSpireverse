using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

public class HeartOfStone : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Absorb", 8m)];

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<HeartOfTheMountain>();

    private int _absorbedThisCombat;

    // Set in ModifyHpLostAfterOstyLate (which must stay a pure calculation — it also decides the displayed
    // damage numbers), then committed in AfterModifyingHpLostAfterOsty, which the game only invokes when
    // the damage was actually applied. Same pattern as the base game's BufferPower.
    private decimal _pendingAbsorb;

    private int AbsorbedThisCombat
    {
        get { return _absorbedThisCombat; }
        set
        {
            _absorbedThisCombat = value;
            UpdateDisplay();
        }
    }

    public override int DisplayAmount => DynamicVars["Absorb"].IntValue - AbsorbedThisCombat;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress && DisplayAmount > 0;

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner.Creature || amount <= 0m)
            return amount;

        // Only absorb combat damage — event/non-combat HP loss must pass through untouched.
        if (!CombatManager.Instance.IsInProgress)
            return amount;

        decimal remaining = DynamicVars["Absorb"].IntValue - AbsorbedThisCombat;
        if (remaining <= 0m)
            return amount;

        _pendingAbsorb = Math.Min(amount, remaining);
        return amount - _pendingAbsorb;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        AbsorbedThisCombat += (int)_pendingAbsorb;
        _pendingAbsorb = 0m;
        return Task.CompletedTask;
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
        Status = AbsorbedThisCombat >= DynamicVars["Absorb"].IntValue ? RelicStatus.Disabled : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }
}
