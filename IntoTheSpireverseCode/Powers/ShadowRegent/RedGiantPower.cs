using IntoTheSpireverse.IntoTheSpireverseCode.Rewards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class RedGiantPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return Task.CompletedTask;
        for (var i = 0; i < Amount; i++)
        {
            room.AddExtraReward(Owner.Player, new CardUpgradeReward(Owner.Player));
        }

        return Task.CompletedTask;
    }
}