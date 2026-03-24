using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class BloodtapPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Apply<BleedPower>(CombatState.HittableEnemies, Amount, Owner, null);
        }
    }
}
