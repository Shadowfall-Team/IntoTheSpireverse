using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public class ResolvePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    [ThreadStatic]
    public static decimal LastAbsorbed;

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || amount <= 0) return amount;
        if (CombatState?.CurrentSide != Owner.Side) return amount;

        var absorbed = Math.Min(amount, Amount);
        LastAbsorbed = absorbed;
        return amount - absorbed;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (LastAbsorbed <= 0) return;
        Flash();
        await PowerCmd.ModifyAmount(this, -(int)LastAbsorbed, null, null);
    }
}