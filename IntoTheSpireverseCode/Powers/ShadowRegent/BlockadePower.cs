using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class BlockadePower : ShadowPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }

}