using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class BigGunsPower : ShadowPowerModel, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public string GetSecondAmount()
    {
        return (DynamicVars["EnergySpent"].BaseValue % 10).ToString();
    }


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("EnergySpent", 0)
    ];

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress && amount > 0)
            {
                DynamicVars["EnergySpent"].BaseValue += amount;
                InvokeDisplayAmountChanged();
                if (DynamicVars["EnergySpent"].BaseValue % 9 == 0)
                {
                    StartPulsing();
                }

                if (DynamicVars["EnergySpent"].BaseValue > 9)
                {
                    Flash();

                    await LoadAmmoCmd.LoadAmmo(Amount, Owner.Player);

                    DynamicVars["EnergySpent"].BaseValue -= 10;
                    StopPulsing();
                }
            }
        }
    }
}