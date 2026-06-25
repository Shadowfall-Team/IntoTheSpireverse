using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class DefensiveCannonadePower : ShadowPowerModel, IHasSecondAmount, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public int ShotsRemaining
    {
        get => DynamicVars["ShotsRemaining"].IntValue;
        set
        {
            DynamicVars["ShotsRemaining"].BaseValue = value;
            InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("ShotsRemaining", 2)
    ];

    public string GetSecondAmount() => ShotsRemaining.ToString();

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        if (player.Creature != Owner) return;

        Flash();

        ShotsRemaining--;
        if (ShotsRemaining <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}