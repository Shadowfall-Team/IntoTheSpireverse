using BaseLib.Abstracts;
using BaseLib.Common.Rewards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

public class NecronomiconPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player != null)
        {
            room.AddExtraReward(Owner.Player, new CardTransformReward(Owner.Player) {Amount = Amount, Upgrade = true});
        }
        return Task.CompletedTask;
    }
}
