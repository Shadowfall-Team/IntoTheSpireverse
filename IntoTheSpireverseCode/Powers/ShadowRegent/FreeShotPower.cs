using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class FreeShotPower : ShadowPowerModel, IModifiesShotCost, IAmmoFiringListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int ModifyShotCost(int current) => Amount > 0 ? 0 : current;

    public async Task OnAmmoFiring(Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
