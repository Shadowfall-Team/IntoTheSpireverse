using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Artifact narrowed to the three attribute debuffs. It follows Artifact's hook pair rather than
/// removing the power after the fact, so the application is cancelled before any of its own
/// on-applied effects can run.
/// </summary>
public sealed class ConstitutionPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<FrailPower>(),
    ];

    public override bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;

        if (target != Owner) return false;
        if (canonicalPower is not (WeakPower or VulnerablePower or FrailPower)) return false;
        
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff) return false;

        modifiedAmount = 0m;
        return true;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        Flash();
        await PowerCmd.Decrement(this);
    }
}
