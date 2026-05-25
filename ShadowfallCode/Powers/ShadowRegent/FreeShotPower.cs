using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using Shadowfall.ShadowfallCode.Ammo;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class FreeShotPower : CustomPowerModel, IModifiesShotCost, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int ModifyShotCost() => 0;

    public async void OnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
