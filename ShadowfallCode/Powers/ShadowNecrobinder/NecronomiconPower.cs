using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Rewards;

namespace Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

public class NecronomiconPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        room.AddExtraReward(Owner.Player, new CardTransformReward(Owner.Player) {Amount = Amount, Upgrade = true});
        return Task.CompletedTask;
    }
}
