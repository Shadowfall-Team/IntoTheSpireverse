using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using Shadowfall.ShadowfallCode.Ammo;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class NextVolleyDamagePower : CustomPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PowerCmd.Remove(this);
    }
}