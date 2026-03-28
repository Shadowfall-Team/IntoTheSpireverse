using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class GainAmmoNextTurnPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterSideTurnStart(CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Side || AmountOnTurnStart == 0)
            return;

        await PowerCmd.Apply<AmmoPower>(Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}