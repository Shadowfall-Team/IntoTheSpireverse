using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class BedrockPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlatePower>(),
    ];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;
        Flash();
        foreach (var creature in combatState.PlayerCreatures
                     .Where(c => c != null && c.IsAlive)
                     .ToList())
        {
            await PowerCmd.Apply<SlatePower>(
                creature, (decimal)Amount, creature, null);
        }
    }
}