using BaseLib.Common.Rewards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class RedGiantRandomPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return;
        for (int i = 0; i < Amount; i++)
        {
            room.AddExtraReward(Owner.Player, new RandomCardUpgradeReward(Owner.Player));
        }
    }
}