using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class VinoSerpento : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private bool _wasUsedThisCombat;
    
    private bool WasUsedThisCombat
    {
        get => _wasUsedThisCombat;
        set
        {
            AssertMutable();
            _wasUsedThisCombat = value;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        WasUsedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    public override Task AfterCombatEnd(CombatRoom _)
    {
        WasUsedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
    
    public override Decimal ModifyPowerAmountGivenAdditive(
        PowerModel power,
        Creature giver,
        Decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return !(power is PoisonPower) || giver != Owner.Creature || WasUsedThisCombat ? 0M : DynamicVars.Poison.BaseValue;
    }

    public override Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        Flash();
        WasUsedThisCombat = true;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}