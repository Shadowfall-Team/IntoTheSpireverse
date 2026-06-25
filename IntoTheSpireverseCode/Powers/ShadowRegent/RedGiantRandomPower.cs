using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Rewards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class RedGiantRandomPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null) return;

        for (var i = 0; i < Amount; i++)
        {
            room.AddExtraReward(Owner.Player, new RandomCardUpgradeReward(Owner.Player));
        }
    }
}