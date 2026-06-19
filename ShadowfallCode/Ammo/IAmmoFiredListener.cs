using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Shadowfall.ShadowfallCode.Ammo;

public interface IAmmoFiredListener
{
    Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results);
}