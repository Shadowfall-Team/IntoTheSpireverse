using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class BloodtapPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Apply<BleedPower>(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, Amount, Owner, null);
        }
    }
}
