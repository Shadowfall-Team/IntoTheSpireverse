using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class VolleyDamageThisTurnPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext,
        CombatSide side)
    {
        if (side == Owner.Side)
        {
            Flash();
            await PowerCmd.Remove(this);
        }
    }
}